//  --------------------------------------------------------------------------------------------------------------------
// <copyright company="Profesor79. file="FileValidatorActor.cs">
// Copyright (c) 2017 All Right Reserved
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
// </copyright>
// <summary>
// Created: 2017-04-15, 10:16 PM
// Last changed by: profesor79, 2017-04-27, 4:04 PM 
// </summary>
//   --------------------------------------------------------------------------------------------------------------------

namespace Profesor79.Merge.ActorSystem.FileReader
{
    using System.Text.RegularExpressions;

    using Akka.Actor;

    using Profesor79.Merge.ActorSystem.BaseObjects;
    using Profesor79.Merge.ActorSystem.RootActor;
    using Profesor79.Merge.ActorSystem.ValidatorActor;

    /// <summary>The file validator actor.</summary>
    public class FileValidatorActor : BaseActorClass
    {
        /// <summary>The _file reader actor.</summary>
        private readonly string _fileReaderActor = "FileReaderActor";

        /// <summary>The _validation pattern.</summary>
        private string _validationPattern;

        /// <summary>Initializes a new instance of the <see cref="FileValidatorActor"/> class.</summary>
        public FileValidatorActor()
        {
            Receive<FileMessages.CheckInputFile>(
                fileMessage =>
                    {
                        _log.Info($"Received validation request for:{fileMessage.FileName}");
                        _actorDictionary = fileMessage.ActorDictionary;
                        _actorDictionary[_fileReaderActor].Tell(fileMessage);
                        _validationPattern = fileMessage.RegexPattern;
                        Become(WaitingForPhysicalFileValidation);
                    });
        }

        /// <summary>The waiting for physical file validation.</summary>
        public void WaitingForPhysicalFileValidation()
        {
            Receive<FileMessages.FileValidated>(
                fileValidated =>
                    {
                        if (!fileValidated.CanBeRead)
                        {
                            // fail fast
                            SendResponseToRoot("file cannot be read");
                            return;
                        }

                        // ask for first line
                        _log.Info("Getting header ");
                        _actorDictionary[_fileReaderActor].Tell(new FileMessages.ReadHeader());
                        Become(WaitingForHeader);
                    });
        }

        /// <summary>The send response to root.</summary>
        /// <param name="message">The message.</param>
        private void SendResponseToRoot(string message)
        {
            _log.Error($"Cannot read file reason: {message}");
            _actorDictionary["root"].Tell(new RootActorMessages.FatalError(message));
        }

        /// <summary>The waiting for header validation.</summary>
        private void WaitingForHeader()
        {
            Receive<FileMessages.FileHeader>(
                o =>
                    {
                        _log.Info($"Header received: {o.Header}");
                        var options = RegexOptions.None;
                        var regex = new Regex(_validationPattern, options);

                        var match = regex.Match(o.Header);
                        if (match.Success)
                        {
                            _actorDictionary["ValidatorActor"].Tell(new ValidatorMessages.InputFileReadyToProcess());
                        }
                        else
                        {
                            SendResponseToRoot("File Header Corrupted");
                        }

                        Become(Working); // close the loop
                    });
        }

        /// <summary>The working.</summary>
        private void Working()
        {
            Receive<FileMessages.CheckInputFile>(
                fileMessage =>
                    {
                        _actorDictionary = fileMessage.ActorDictionary;

                        _actorDictionary[_fileReaderActor].Tell(fileMessage);

                        Become(WaitingForPhysicalFileValidation);
                    });
        }
    }
}

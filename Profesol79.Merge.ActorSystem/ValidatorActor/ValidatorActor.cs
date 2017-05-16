//  --------------------------------------------------------------------------------------------------------------------
// <copyright company="Profesor79. file="ValidatorActor.cs">
// Copyright (c) 2017 All Right Reserved
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
// </copyright>
// <summary>
// Created: 2017-04-23, 10:34 PM
// Last changed by: profesor79, 2017-04-27, 4:04 PM 
// </summary>
//   --------------------------------------------------------------------------------------------------------------------

namespace Profesor79.Merge.ActorSystem.ValidatorActor
{
    using Akka.Actor;

    using Profesor79.Merge.ActorSystem.BaseObjects;
    using Profesor79.Merge.ActorSystem.FileReader;
    using Profesor79.Merge.ActorSystem.FileWriter;
    using Profesor79.Merge.ActorSystem.RootActor;
    using Profesor79.Merge.ActorSystem.WebCrawler;
    using Profesor79.Merge.Contracts;

    /// <summary>The validator actor.</summary>
    public class ValidatorActor : BaseActorClass
    {
        /// <summary>The _system configuration.</summary>
        private readonly ISystemConfiguration _systemConfiguration;

        /// <summary>The _api state.</summary>
        private bool _apiState;

        /// <summary>The _file created.</summary>
        private bool _fileCreated;

        /// <summary>The _header created.</summary>
        private bool _headerCreated;

        /// <summary>The _input validated.</summary>
        private bool _inputValidated;

        /// <summary>The _root.</summary>
        private IActorRef _root;

        /// <summary>Initializes a new instance of the <see cref="ValidatorActor"/> class.</summary>
        /// <param name="systemConfiguration">The system configuration.</param>
        public ValidatorActor(ISystemConfiguration systemConfiguration)
        {
            _systemConfiguration = systemConfiguration;
            Receive<ValidatorMessages.Validate>(
                m =>
                    {
                        Become(WaitingForResponses);
                        _actorDictionary = m.ActorDictionary;
                        _root = Sender;
                        _log.Info("Check starts ");

                        _log.Info("Output file ");

                        _actorDictionary["FileWriterActor"].Tell(
                            new FileWriterMessages.CreateFile(m.OutputFilePath, _systemConfiguration.StopIfDestinationFileExists, _actorDictionary));

                        _log.Info("Input file ");
                        _actorDictionary["FileValidatorActor"].Tell(
                            new FileMessages.CheckInputFile(m.InputFilePath, _actorDictionary, _systemConfiguration.HeaderValidationRegex));

                        _log.Info("Web ");
                        _actorDictionary["WebCheckerActor"].Tell(new CrawlerMessages.CheckEndpoint());
                    });
        }

        /// <summary>The can we go ahead.</summary>
        private void CanWeGoAhead()
        {
            if (_inputValidated && _fileCreated && _headerCreated && _apiState)
            {
                _log.Info(
                    $"CanWeGoAhead: _inputValidated:{_inputValidated} _fileCreated:{_fileCreated} _headerCreated:{_headerCreated}, apiState:{_apiState}");
                _actorDictionary["FlowControlActor"].Tell(new FlowControlMessages.StartProcessing(_actorDictionary));
                _actorDictionary["WebCheckerActor"].Tell(new CrawlerMessages.StartChecking());
            }
        }

        /// <summary>The waiting for responses.</summary>
        private void WaitingForResponses()
        {
            Receive<ValidatorMessages.FileCreated>(
                m =>
                    {
                        _log.Info("File created - writing header");
                        _fileCreated = true;
                        Sender.Tell(new FileWriterMessages.WriteHeader(_systemConfiguration.OutputFileHeader));
                    });

            Receive<ValidatorMessages.HeaderCreated>(
                m =>
                    {
                        _log.Info("Header saved - output file checks ok");
                        _headerCreated = true;
                        CanWeGoAhead();
                    });

            Receive<ValidatorMessages.CannotCreateFile>(
                e =>
                    {
                        _log.Error("Cannot create file - aborting...");
                        _root.Tell(new RootActorMessages.FatalError("ValidationFailed"));
                    });

            Receive<ValidatorMessages.InputFileReadyToProcess>(
                e =>
                    {
                        _log.Info("Input File Ready To Process");
                        _inputValidated = true;
                        CanWeGoAhead();
                    });

            Receive<ValidatorMessages.ApiState>(
                a =>
                    {
                        _apiState = a.IsOnline;
                        if (!_apiState)
                        {
                            var message = "Api is down";
                            _log.Error(message);
                            _root.Tell(new RootActorMessages.FatalError(message));
                        }

                        CanWeGoAhead();
                    });
        }
    }
}

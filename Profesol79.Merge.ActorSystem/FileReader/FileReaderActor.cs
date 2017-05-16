//  --------------------------------------------------------------------------------------------------------------------
// <copyright company="Profesor79. file="FileReaderActor.cs">
// Copyright (c) 2017 All Right Reserved
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
// </copyright>
// <summary>
// Created: 2017-04-16, 9:57 PM
// Last changed by: profesor79, 2017-04-27, 4:04 PM 
// </summary>
//   --------------------------------------------------------------------------------------------------------------------

namespace Profesor79.Merge.ActorSystem.FileReader
{
    using System;
    using System.Linq;

    using Akka.Actor;

    using Profesor79.Merge.ActorSystem.BaseObjects;
    using Profesor79.Merge.ActorSystem.RootActor;
    using Profesor79.Merge.ActorSystem.WebCrawler;
    using Profesor79.Merge.Contracts;

    /// <summary>The file reader actor.</summary>
    public class FileReaderActor : BaseActorClass
    {
        /// <summary>The _file reader.</summary>
        private readonly IFileReader _fileReader;

        /// <summary>The _file name.</summary>
        private string _fileName;

        /// <summary>The _internal chunk size.</summary>
        private readonly uint _internalChunkSize;

        /// <summary>Initializes a new instance of the <see cref="FileReaderActor"/> class.</summary>
        /// <param name="fileReader">The file reader.</param>
        /// <param name="systemConfiguration">The system Configuration.</param>
        public FileReaderActor(IFileReader fileReader, ISystemConfiguration systemConfiguration)
        {
            _fileReader = fileReader;
            _internalChunkSize = systemConfiguration.InternalChunkSize;
            Receive<RootActorMessages.AddressBook>(b => { _actorDictionary = b.ActorDictionary; });

            Receive<FileMessages.CheckInputFile>(
                fileMessage =>
                    {
                        _log.Info($"Processing input file validation for: {fileMessage.FileName}");
                        _fileName = fileMessage.FileName;
                        var checkResult = CheckFile(fileMessage.FileName);
                        var response = new FileMessages.FileValidated(fileMessage.FileName, checkResult);
                        Sender.Tell(response);

                        if (checkResult)
                        {
                            // change state only when file is ok
                            Become(ValidatingHeader);
                        }
                    });
        }

        /// <summary>The check file.</summary>
        /// <param name="fileName">The file Name.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        private bool CheckFile(string fileName)
        {
            bool check;
            try
            {
                check = _fileReader.CanWeReadFile(fileName);
            }
            catch (Exception e)
            {
                _log.Error($"Cannot read file: {_fileName}, reason:{e.Message}");
                check = false;
            }

            return check;
        }

        /// <summary>The reading data.</summary>
        private void ReadingData()
        {
            Receive<FileMessages.ReadLines>(
                readLines =>
                    {
                        try
                        {
                            for (uint i = 0; i < readLines.HowMany; i = i + _internalChunkSize)
                            {
                                var lines = _fileReader.ReadLines(_internalChunkSize);
                                if (lines.Any())
                                {
                                    _actorDictionary["DataDistributorActor"].Tell(new FileMessages.FileLines(lines));
                                    _actorDictionary["FlowControlActor"].Tell(new FlowControlMessages.LinesRead((uint)lines.Count));
                                }
                                else
                                {
                                    Sender.Tell(new FileMessages.EndOfFile());
                                    break;
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            _log.Error($"Cannot read file: {_fileName}, reason:{e.Message}");
                            Sender.Tell(new FileMessages.CannotReadFile());
                        }
                    });
        }

        /// <summary>The validating header.</summary>
        private void ValidatingHeader()
        {
            Receive<FileMessages.ReadHeader>(
                fileMessage =>
                    {
                        try
                        {
                            var header = _fileReader.ReadFirstLine(_fileName);
                            Sender.Tell(new FileMessages.FileHeader(header));
                            Become(ReadingData);
                        }
                        catch (Exception e)
                        {
                            _log.Error($"Cannot read file: {_fileName}, reason:{e.Message}");
                            Sender.Tell(new FileMessages.CannotReadFile());
                        }
                    });
        }
    }
}

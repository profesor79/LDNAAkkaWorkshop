//  --------------------------------------------------------------------------------------------------------------------
// <copyright company="profesor79.pl" file="DataDispatcherActor.cs">
// Copyright (c) 2017 All Right Reserved
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
// </copyright>
// <summary>
// Created: 2017-05-15, 2:37 PM
// Last changed by: profesor79, 2017-05-26, 8:20 AM 
// </summary>
//   --------------------------------------------------------------------------------------------------------------------

namespace Profesor79.Merge.ActorSystem.FileReader
{
    using Akka.Actor;

    using Profesor79.Merge.ActorSystem.BaseObjects;
    using Profesor79.Merge.ActorSystem.RootActor;
    using Profesor79.Merge.ActorSystem.WebCrawler;
    using Profesor79.Merge.Contracts;

    /// <summary>The data dispatcher actor.</summary>
    public class DataDispatcherActor : BaseActorClass, IWithUnboundedStash
    {
        /// <summary>The _read lines batch size.</summary>
        private readonly uint _readLinesBatchSize;

        /// <summary>The _crawler.</summary>
        private IActorRef _crawler;

        /// <summary>The _flow control.</summary>
        private IActorRef _flowControl;

        /// <summary>The _system configuration.</summary>
        private ISystemConfiguration _systemConfiguration;

        /// <summary>Initializes a new instance of the <see cref="DataDispatcherActor"/> class.</summary>
        /// <param name="systemConfiguration">The system configuration.</param>
        public DataDispatcherActor(ISystemConfiguration systemConfiguration)
        {
            SetupActor(systemConfiguration);
            _readLinesBatchSize = _systemConfiguration.ReadLinesBatchSize;

            Receive<FileMessages.StartProcessing>(
                e =>
                    {
                        _actorDictionary = e.ActorDict;
                        _crawler = _actorDictionary["WebCrawlerActor"];
                        _flowControl = _actorDictionary["FlowControlActor"];
                        RequestNewLines();
                    });

            Receive<FileMessages.GetNextChunk>(a => { RequestNewLines(); });

            Receive<FileMessages.CannotReadFile>(
                error => { _actorDictionary["root"].Tell(new RootActorMessages.FatalError("Cannot Read Input File")); });

            Receive<FileMessages.EndOfFile>(a => { _flowControl.Tell(new FlowControlMessages.EoF()); });
        }

        /// <summary>Gets or sets the stash.</summary>
        public IStash Stash { get; set; }

        /// <summary>The request new lines.</summary>
        private void RequestNewLines()
        {
            _actorDictionary["FileReaderActor"].Tell(new FileMessages.ReadLines(_readLinesBatchSize));
        }

        /// <summary>The setup actor.</summary>
        /// <param name="systemConfiguration">The system configuration.</param>
        private void SetupActor(ISystemConfiguration systemConfiguration)
        {
            _systemConfiguration = systemConfiguration;
        }
    }
}

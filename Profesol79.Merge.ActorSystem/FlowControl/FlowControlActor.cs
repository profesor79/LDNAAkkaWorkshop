//  --------------------------------------------------------------------------------------------------------------------
// <copyright company="WPE" file="FlowControlActor.cs">
// Copyright (c) 2017 All Right Reserved
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
// </copyright>
// <summary>
// Created: 2017-05-15, 2:37 PM
// Last changed by: A happy WPE candidate, 2017-05-16, 10:47 AM 
// </summary>
//   --------------------------------------------------------------------------------------------------------------------

namespace Profesor79.Merge.ActorSystem.FlowControl
{
    using System;

    using Akka.Actor;

    using Profesor79.Merge.ActorSystem.BaseObjects;
    using Profesor79.Merge.ActorSystem.FileReader;
    using Profesor79.Merge.ActorSystem.FileWriter;
    using Profesor79.Merge.ActorSystem.RootActor;
    using Profesor79.Merge.ActorSystem.WebCrawler;
    using Profesor79.Merge.Contracts;

    /// <summary>The flow control actor.</summary>
    public class FlowControlActor : BaseActorClass
    {
        /// <summary>The _system configuration.</summary>
        private readonly ISystemConfiguration _systemConfiguration;

        /// <summary>The _api bad responses.</summary>
        private uint _apiBadResponses;

        /// <summary>The _api down.</summary>
        private uint _apiDown;

        /// <summary>The _api responses.</summary>
        private uint _apiResponses;

        /// <summary>The _api valid responses.</summary>
        private uint _apiValidResponses;

        /// <summary>The _bad lines.</summary>
        private uint _badLines;

        /// <summary>The _cancel timer.</summary>
        private ICancelable _cancelTimer;

        /// <summary>The _end of input file.</summary>
        private bool _endOfInputFile;

        /// <summary>The _last chunk sent.</summary>
        private DateTime _lastChunkSent;

        /// <summary>The _lines count.</summary>
        private uint _linesCount;

        /// <summary>The _lines saved.</summary>
        private uint _linesSaved;

        /// <summary>The _processed lines.</summary>
        private uint _processedLines;

        /// <summary>The _valid lines.</summary>
        private uint _validLines;

        /// <summary>Initializes a new instance of the <see cref="FlowControlActor"/> class.</summary>
        /// <param name="systemConfiguration">The system configuration.</param>
        public FlowControlActor(ISystemConfiguration systemConfiguration)
        {
            _systemConfiguration = systemConfiguration;
            _lastChunkSent = DateTime.Now;

            Receive<FlowControlMessages.Timer>(t => { _log.Info("Wainting for: StartProcessing"); });

            Receive<FlowControlMessages.StartProcessing>(
                m =>
                    {
                        _actorDictionary = m.ActorDictionary;
                        Become(Processing);
                        _actorDictionary["DataDispatcherActor"].Tell(new FileMessages.StartProcessing(_actorDictionary));
                    });

            Receive<FlowControlMessages.GetNewLinesForCrawler>(
                m =>
                    {
                        /* swallow that as we are not ready yet */
                    });
        }

        /// <summary>The pre start.</summary>
        protected override void PreStart()
        {
            _cancelTimer = new Cancelable(Context.System.Scheduler);

            Context.System.Scheduler.ScheduleTellRepeatedly(
                TimeSpan.FromSeconds(2),
                TimeSpan.FromSeconds(2),
                Self,
                new FlowControlMessages.Timer(),
                Self,
                _cancelTimer);

            base.PreStart();
        }

        /// <summary>The check if finished.</summary>
        private void CheckIfFinished()
        {
            if (_endOfInputFile && _validLines == _apiResponses && _apiResponses == _linesSaved)
            {
                _log.Info($"Job done!");
                _actorDictionary["root"].Tell(new RootActorMessages.ProcessFinished());

                // ask is an antypattern in local actor system
                // but in this case we dont want to loose data 
                var closed = _actorDictionary["FileWriterActor"].Ask(new FileWriterMessages.CloseFile()).Result;
            }
        }

        /// <summary>The check server replys.</summary>
        private void CheckServerReplys()
        {
            // as we can have a 403 or 500 responsess 
            // so when first 10 responses are error ones
            // shutdow the client

            if (_apiValidResponses == 0 && _apiBadResponses > _systemConfiguration.InitialApiErrorThresholdToShutdownSystem)
            {
                LogErrorAndStopSystem("to many bad responses");
            }
        }

        /// <summary>The handle api state.</summary>
        /// <param name="a">The a.</param>
        private void HandleApiState(FlowControlMessages.ApiState a)
        {
            if (!a.IsOnline)
            {
                _apiDown++;
                _log.Warning("Api heartbeat failed...");

                if (_apiDown > _systemConfiguration.InitialApiErrorThresholdToShutdownSystem)
                {
                    LogErrorAndStopSystem("Api is down");
                }
            }
            else
            {
                _apiDown = 0;
            }
        }

        /// <summary>The handle bad line.</summary>
        private void HandleBadLine()
        {
            _badLines++;
            _processedLines++;
            _log.Debug($"FL: BadLine:{_badLines}, {_processedLines}");
        }

        /// <summary>The handle eo f.</summary>
        private void HandleEoF()
        {
            _log.Debug($"FL: EoF");
            _endOfInputFile = true;
        }

        /// <summary>The handle get new lines for crawler.</summary>
        private void HandleGetNewLinesForCrawler()
        {
            // as we can get messages in same time from differet sources
            // we will serve only to first actor
            if (_processedLines > 0 && Sender.Path.Name == "$a")
            {
                _log.Info($"FlowControlMessages.GetNextChunk: sent, requester:{Sender.Path.Name}");
                _actorDictionary["DataDispatcherActor"].Tell(new FileMessages.GetNextChunk());
            }
        }

        /// <summary>The handle lines read.</summary>
        /// <param name="l">The l.</param>
        private void HandleLinesRead(FlowControlMessages.LinesRead l)
        {
            _log.Debug($"FL: LinesRead: {l.LinesCount}");
            _linesCount += l.LinesCount;
        }

        /// <summary>The handle lines saved.</summary>
        /// <param name="m">The m.</param>
        private void HandleLinesSaved(FlowControlMessages.LinesSaved m)
        {
            _log.Info($"FL: LinesSaved:{m.LinesCount}");
            _linesSaved += m.LinesCount;

            if (_linesSaved == _validLines && _endOfInputFile)
            {
                _log.Info($"Job finished - terminating");
                _actorDictionary["root"].Tell(new RootActorMessages.ProcessFinished());
            }
        }

        /// <summary>The handle timer.</summary>
        private void HandleTimer()
        {
            _log.Info(
                $"Status: eof:{_endOfInputFile} linesCount:{_linesCount}, validLines:{_validLines}, apiResponses:{_apiResponses}, linesSaved:{_linesSaved}");
            CheckServerReplys();
            CheckIfFinished();
        }

        /// <summary>The handle valid line.</summary>
        private void HandleValidLine()
        {
            _validLines++;
            _processedLines++;
            _log.Debug($"FL: ValidLine:{_validLines},{_processedLines}");
        }

        /// <summary>The handle web api got bad response.</summary>
        private void HandleWebApiGotBadResponse()
        {
            _apiResponses++;
            _apiBadResponses++;
            _log.Debug($"FL: WebApiGotBadResponse:{_apiBadResponses}, {_apiResponses}");
        }

        /// <summary>The handle web api got valid response.</summary>
        private void HandleWebApiGotValidResponse()
        {
            _apiResponses++;
            _apiValidResponses++;
            _log.Debug($"FL: WebApiGotValidResponse:{_apiValidResponses}, {_apiResponses}");
        }

        /// <summary>The log error and stop system.</summary>
        /// <param name="message">The message.</param>
        private void LogErrorAndStopSystem(string message)
        {
            _log.Error(message);
            _actorDictionary["root"].Tell(new RootActorMessages.FatalError(message));
        }

        /// <summary>The processing.</summary>
        private void Processing()
        {
            Receive<FlowControlMessages.ApiState>(a => { HandleApiState(a); });

            Receive<FlowControlMessages.GetNewLinesForCrawler>(m => { HandleGetNewLinesForCrawler(); });

            Receive<FlowControlMessages.Timer>(t => { HandleTimer(); });

            Receive<FlowControlMessages.LinesRead>(l => { HandleLinesRead(l); });

            Receive<FlowControlMessages.EoF>(l => { HandleEoF(); });

            Receive<FlowControlMessages.ValidLine>(l => { HandleValidLine(); });

            Receive<FlowControlMessages.BadLine>(l => { HandleBadLine(); });

            Receive<FlowControlMessages.WebApiGotValidResponse>(l => { HandleWebApiGotValidResponse(); });

            Receive<FlowControlMessages.WebApiGotBadResponse>(l => { HandleWebApiGotBadResponse(); });

            Receive<FlowControlMessages.LinesSaved>(m => { HandleLinesSaved(m); });
        }
    }
}

//  --------------------------------------------------------------------------------------------------------------------
// <copyright company="profesor79.pl" file="WebCheckerActor.cs">
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

namespace Profesor79.Merge.ActorSystem.WebCrawler
{
    using System;
    using System.Net.Http;
    using System.Threading;

    using Akka.Actor;

    using Profesor79.Merge.ActorSystem.BaseObjects;
    using Profesor79.Merge.ActorSystem.ValidatorActor;
    using Profesor79.Merge.Contracts;

    /// <summary>The web checker actor.</summary>
    public class WebCheckerActor : BaseActorClass
    {
        /// <summary>The _client.</summary>
        private readonly HttpClient _client;

        /// <summary>The _system configuration.</summary>
        private readonly ISystemConfiguration _systemConfiguration;

        /// <summary>The _cancel timer.</summary>
        private ICancelable _cancelTimer;

        /// <summary>The _is online.</summary>
        private bool _isOnline;

        /// <summary>The _reporting actor.</summary>
        private IActorRef _reportingActor;

        /// <summary>Initializes a new instance of the <see cref="WebCheckerActor"/> class.</summary>
        /// <param name="systemConfiguration">The system configuration.</param>
        public WebCheckerActor(ISystemConfiguration systemConfiguration)
        {
            _systemConfiguration = systemConfiguration;
            _client = new HttpClient { Timeout = TimeSpan.FromMilliseconds(2 * _systemConfiguration.HttpTimeoutInMiniseconds) };

            Receive<CrawlerMessages.CheckEndpoint>(
                c =>
                    {
                        _reportingActor = Sender;

                        CheckApi();

                        _log.Info($"Api is online:{_isOnline}");
                        _reportingActor.Tell(new ValidatorMessages.ApiState(_isOnline));
                    });

            Receive<CrawlerMessages.StartChecking>(
                a =>
                    {
                        Become(Checking);
                        _reportingActor = Sender;

                        _cancelTimer = new Cancelable(Context.System.Scheduler);

                        Context.System.Scheduler.ScheduleTellRepeatedly(
                            TimeSpan.FromMilliseconds(_systemConfiguration.HttpTimeoutInMiniseconds),
                            TimeSpan.FromMilliseconds(_systemConfiguration.HttpTimeoutInMiniseconds),
                            Self,
                            new CrawlerMessages.Timer(),
                            Self,
                            _cancelTimer);
                    });
        }

        /// <summary>The check api.</summary>
        private void CheckApi()
        {
            try
            {
                for (var i = 0; i < _systemConfiguration.InitialApiErrorThresholdToShutdownSystem; i++)
                {
                    _isOnline = Ping();
                    if (_isOnline)
                    {
                        break;
                    }

                    _log.Warning("Api is not responding");
                    Thread.Sleep(TimeSpan.FromMilliseconds(_systemConfiguration.HttpTimeoutInMiniseconds));
                }
            }
            catch (Exception e)
            {
                _log.Error($"Cannot connect to:{_systemConfiguration.ApiEndPoint}", e);
                _isOnline = false;
            }
        }

        /// <summary>The checking.</summary>
        private void Checking()
        {
            Receive<CrawlerMessages.Timer>(
                m =>
                    {
                        // most servers are need some time to start when idle.
                        // so we will hit server 5 times in defined delays
                        CheckApi();
                        _log.Info($"Api is online: {_isOnline}");

                        _reportingActor.Tell(new ValidatorMessages.ApiState(_isOnline));
                    });
        }

        /// <summary>The ping.</summary>
        /// <returns>The <see cref="bool"/>.</returns>
        private bool Ping()
        {
            // the response can be whatever from 200 via 404 to 503 and beyond, but that means there is a link to server
            // if there is a network issue or bad addres then we will get exception here
            var url = new Uri(_systemConfiguration.ApiEndPoint);
            var address = url.GetLeftPart(UriPartial.Authority);

            var httpResponseMessage = _client.GetAsync(address).Result;
            return true;
        }
    }
}

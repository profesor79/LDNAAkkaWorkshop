//  --------------------------------------------------------------------------------------------------------------------
// <copyright company="profesor79.pl" file="RootActor.cs">
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

using System.Collections.Generic;
using Akka.Cluster.Routing;

namespace Profesor79.Merge.ActorSystem.RootActor
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Threading;

    using Akka.Actor;
    using Akka.DI.Core;
    using Akka.Routing;

    using Petabridge.Cmd.QuickStart;

    using Profesor79.Merge.ActorSystem.BaseObjects;
    using Profesor79.Merge.ActorSystem.FileReader;
    using Profesor79.Merge.ActorSystem.FileWriter;
    using Profesor79.Merge.ActorSystem.FlowControl;
    using Profesor79.Merge.ActorSystem.ValidatorActor;
    using Profesor79.Merge.ActorSystem.WebCrawler;
    using Profesor79.Merge.Contracts;
    using Profesor79.Merge.Domain;

    /// <summary>The root actor.</summary>
    public partial class RootActor : BaseActorClass
    {
        /// <summary>The _system configuration.</summary>
        private readonly ISystemConfiguration _systemConfiguration;

        private DateTime _started;

        private RootActorMessages.StartSystem _startSystemMessage;

        /// <summary>Initializes a new instance of the <see cref="RootActor"/> class.</summary>
        /// <param name="systemConfiguration">The system Configuration.</param>
        public RootActor(ISystemConfiguration systemConfiguration)
        {
            _systemConfiguration = systemConfiguration;

            Receive<RootActorMessages.AddressBookRequest>(
                a =>
                    {
                        Sender.Tell(new RootActorMessages.AddressBook(_actorDictionary));
                    });
            Receive<RootActorMessages.StartSystem>(
                me =>
                    {
                        _started = DateTime.Now;
                        if (systemConfiguration.WaitForClusterStartMessage)
                        {
                            _startSystemMessage = me;
                        }
                        else
                        {
                            StartSystem();
                        }
                    });

            Receive<RootActorMessages.FatalError>(
                a =>
                    {
                        _log.Error($"shutting down system - fatal error received, description: {a.Description ?? "none provided..."}");
                        StopSystem();
                    });

            Receive<RootActorMessages.HaltSystem>(
                a =>
                    {
                        _log.Info("Halting system");
                        var duration = (DateTime.Now - _started).TotalSeconds;
                        _log.Info("**************************************");
                        _log.Info($"Time consumed: {duration} seconds");
                        _log.Info("**************************************");
                        Context.System.Terminate();
                        Thread.Sleep(3000); // allow to flush log buffer
                    });

            Receive<RootActorMessages.ProcessFinished>(a => { StopSystem(); });
            Receive<StartFromCli>(
                o =>
                    {
                      StartSystem();
                    });
        }

        private void StartSystem()
        {
            CreateActors();
            SendActorBook();
            _actorDictionary["ValidatorActor"].Tell(
                new ValidatorMessages.Validate(_startSystemMessage.InputFilePath, _startSystemMessage.OutputFilePath, _actorDictionary));
        }

        /// <summary>The create actors could be implemented as a factory
        /// this way gives ability to send actor references.</summary>
        private void CreateActors()
        {
            var actorSuffix = Guid.NewGuid().ToString().Substring(0, 6);
            _actorDictionary.Add("root", Self);
            _actorDictionary.Add("FileValidatorActor", Context.ActorOf(Context.DI().Props<FileValidatorActor>(), $"FileValidatorActor{actorSuffix}"));
            _actorDictionary.Add(
                "FileReaderActor",
                Context.ActorOf(Context.DI().Props<FileReaderActor>().WithDispatcher("my-dispatcher"), $"FileReaderActor{actorSuffix}"));

            _actorDictionary.Add("FileWriterActor", Context.ActorOf(Context.DI().Props<FileWriterActor>(), $"FileWriterActor{actorSuffix}"));
            _actorDictionary.Add("WebCheckerActor", Context.ActorOf(Context.DI().Props<WebCheckerActor>(), $"WebCheckerActor{actorSuffix}"));
            _actorDictionary.Add(
                "FlowControlActor",
                Context.ActorOf(Context.DI().Props<FlowControlActor>().WithDispatcher("my-dispatcher"), $"FlowControlActor{actorSuffix}"));
            _actorDictionary.Add(
                "DataDispatcherActor",
                Context.ActorOf(Context.DI().Props<DataDispatcherActor>(), $"DataDispatcherActor{actorSuffix}"));
            _actorDictionary.Add("ValidatorActor", Context.ActorOf(Context.DI().Props<ValidatorActor>(), $"ValidatorActor{actorSuffix}"));

            _actorDictionary.Add(
                "DataDistributorActor",
                Context.ActorOf(
                    Context.DI()
                        .Props<DataDistributorActor>()
                        .WithRouter(new RoundRobinPool((int)_systemConfiguration.DataDistributorActorCount))
                        .WithDispatcher("my-dispatcher"),
                    $"DataDistributorActor{actorSuffix}"));


            CreateCrawlerByConfig();

        }

        private void CreateCrawlerByConfig()
        {
            switch (_systemConfiguration.HowToScale)
            {
                case HowToScaleEnum.Asis:
                    AddByAsis();
                    break;

                case HowToScaleEnum.ManyWorkers:
                    AddByManyLocalWorkers();
                    break;

                case HowToScaleEnum.RemoteDeploy:
                    CreateRemoteCrawlerGroup();
                    break;

                case HowToScaleEnum.LocalDynamic:
                    AddByLocalDynamicPoll();

                    break;
                case HowToScaleEnum.Cluster:
                    CreateCluster();
                    break;
            }
        }

        private void CreateCluster()
        {
            var remoteEcho222 =
                Context.ActorOf(
                    Props.Create(() => new WebCrawlerActor(new AppSettingsConfiguration(), Self))
                        .WithRouter(new ClusterRouterPool(new RoundRobinPool(5), new ClusterRouterPoolSettings(5, 1, true, "crawler"))),
                    "WebCrawlerActor2a");

            _actorDictionary.Add("WebCrawlerActor", remoteEcho222);
        }

        private void AddByLocalDynamicPoll()
        {
            var remoteEcho42 =
                Context.ActorOf(
                    Props.Create(() => new WebCrawlerActor(new AppSettingsConfiguration(), Self))
                        .WithRouter(new RoundRobinPool(1, new DefaultResizer(1, 64, messagesPerResize: 100))));

            _actorDictionary.Add("WebCrawlerActor", remoteEcho42);
        }

        private void AddByManyLocalWorkers()
        {
            var remoteEcho3 =
                Context.ActorOf(
                    Props.Create(() => new WebCrawlerActor(new AppSettingsConfiguration(), Self))
                        .WithRouter(new RoundRobinPool((int)_systemConfiguration.CrawlerActorsCount)) // new DefaultResizer(1, 2, messagesPerResize: 500)
                        .WithDispatcher("my-dispatcher"),
                    "WebCrawlerActor2");
            _actorDictionary.Add("WebCrawlerActor", remoteEcho3);
        }

        private void AddByAsis()
        {
            var remoteEcho2 = Context.ActorOf(
                Props.Create(() => new WebCrawlerActor(new AppSettingsConfiguration(), Self)).WithDispatcher("my-dispatcher"),
                "WebCrawlerActor");
            _actorDictionary.Add("WebCrawlerActor", remoteEcho2);
        }

        private void CreateRemoteCrawlerGroup()
        {
            var hostname = _systemConfiguration.RemoteHost1;

            var remoteAddress = Address.Parse($"akka.tcp://DeployTarget@{hostname}:8090");

            var remoteScope = new RemoteScope(remoteAddress);
            var remoteCrawler2 =
                Context.ActorOf(
            Props.Create(() => new WebCrawlerActor(new AppSettingsConfiguration(), Self))
            .WithRouter(new RoundRobinPool(2))
                             .WithDispatcher("my-dispatcher")
            .WithDeploy(Deploy.None.WithScope(remoteScope)), "remoteCrawler01");

            _actorDictionary.Add("WebCrawlerActor", remoteCrawler2);
        }

        /// <summary>The send actor book.</summary>
        private void SendActorBook()
        {


            for (var i = 0; i < _systemConfiguration.DataDistributorActorCount; i++)
            {
                _actorDictionary["DataDistributorActor"].Tell(new RootActorMessages.AddressBook(_actorDictionary));
            }

            _actorDictionary["FileReaderActor"].Tell(new RootActorMessages.AddressBook(_actorDictionary));
        }

        /// <summary>The stop system.</summary>
        private void StopSystem()
        {
            foreach (var actor in _actorDictionary.Where(o => o.Key != "root"))
            {
                _log.Info($"Shutting actor:{actor.Key}");
                actor.Value.Tell(PoisonPill.Instance);
            }

            Self.Tell(new RootActorMessages.HaltSystem());
        }
    }


}

//  --------------------------------------------------------------------------------------------------------------------
// <copyright company="profesor79.pl" file="SystemLauncher.cs">
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

using Serilog.Core;

namespace Profesor79.Merge.ActorSystem.RootActor
{
    using System;

    using Akka.Actor;
    using Akka.Configuration;
    using Akka.DI.AutoFac;
    using Akka.DI.Core;
    

    using Autofac;

    

    using Petabridge.Cmd.Cluster;
    using Petabridge.Cmd.Host;

    using Profesor79.Merge.ActorSystem.FileReader;
    using Profesor79.Merge.ActorSystem.FileWriter;
    using Profesor79.Merge.ActorSystem.FlowControl;
    using Profesor79.Merge.ActorSystem.ValidatorActor;
    using Profesor79.Merge.ActorSystem.WebCrawler;
    using Profesor79.Merge.Contracts;
    using Profesor79.Merge.Domain;
    using Profesor79.Merge.Domain.Helpers;

    /// <summary>The system start.</summary>
    public class SystemLauncher
    {
        /// <summary>The _root.</summary>
        private IActorRef _root;

        /// <summary>Initializes a new instance of the <see cref="SystemLauncher"/> class.</summary>

        /// <summary>Gets the dialer actor system.</summary>
        public ActorSystem MergeActorSystem { get; set; }

        /// <summary>The start.</summary>
        /// <param name="inputFilePath">The input file path.</param>
        /// <param name="outputFilePath">The output file path.</param>
        public void Start(string inputFilePath, string outputFilePath)
        {
            var config = ConfigurationHelper.GetClusterConfiguration();

            MergeActorSystem = ActorSystem.Create("ClusterSystem", ConfigurationFactory.ParseString(config));
            var cmd = PetabridgeCmd.Get(MergeActorSystem);
            cmd.RegisterCommandPalette(ClusterCommands.Instance);
            cmd.Start();
           

            // Create and build your container
            var builder = ContainerBuilder();
            var container = builder.Build();
            var propsResolver = new AutoFacDependencyResolver(container, MergeActorSystem);
            // first actor 
            _root = MergeActorSystem.ActorOf(MergeActorSystem.DI().Props<RootActor>(), "root");

            _root.Tell(new RootActorMessages.StartSystem(inputFilePath, outputFilePath));
            MergeActorSystem.WhenTerminated.Wait();
        }

        private static ContainerBuilder ContainerBuilder()
        {
            var builder = new Autofac.ContainerBuilder();

            var config = ConfigurationFactory.ParseString(ConfigurationHelper.GetClusterConfiguration());
            var dev = config.GetString("application.environment");
            var configBase = $"application.{dev}.";
            var useFixedConfigFile = config.GetBoolean($"{configBase}useFixedConfigFile");
            if (useFixedConfigFile)
            {
                var fixedConfigClassName = config.GetString($"{configBase}FixedConfigClassName");

                // Type typeYouWant = Type.GetType("NamespaceOfType.TypeName, AssemblyName");
                // http://stackoverflow.com/a/25913864/5919473
                var type = Type.GetType($"Profesor79.Merge.Domain.Configuration.{fixedConfigClassName}, Profesor79.Merge.Domain");
                builder.RegisterType(type).As<ISystemConfiguration>();

            }
            else
            {
                builder.RegisterType<AppSettingsConfiguration>().As<ISystemConfiguration>().SingleInstance();
            }


            builder.RegisterType<FileWriter>().As<IFileWriter>();
            builder.RegisterType<FileReader>().As<IFileReader>();
            builder.RegisterType<RootActor>().AsSelf();

            builder.RegisterType<FileValidatorActor>().AsSelf();
            builder.RegisterType<FileReaderActor>().AsSelf();
            builder.RegisterType<FileWriterActor>().AsSelf();
            builder.RegisterType<WebCheckerActor>().AsSelf();
            builder.RegisterType<FlowControlActor>().AsSelf();
            builder.RegisterType<DataDispatcherActor>().AsSelf();
            builder.RegisterType<ValidatorActor>().AsSelf();
            builder.RegisterType<DataDistributorActor>().AsSelf();
            builder.RegisterType<WebCrawlerActor>().AsSelf();
            //builder.RegisterType<>().AsSelf();
            //builder.RegisterType<>().AsSelf();
            //builder.RegisterType<>().AsSelf();
            //builder.RegisterType<>().AsSelf();
            //builder.RegisterType<>().AsSelf();
            //builder.RegisterType<>().AsSelf();
            //builder.RegisterType<>().AsSelf();



            return builder;
        }

        /// <summary>The stop.</summary>
        public void Stop() { MergeActorSystem.Terminate(); }


    }
}

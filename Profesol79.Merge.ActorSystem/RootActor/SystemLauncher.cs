//  --------------------------------------------------------------------------------------------------------------------
// <copyright company="WPE" file="SystemLauncher.cs">
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

namespace Profesor79.Merge.ActorSystem.RootActor
{
    using System;

    using Akka.Actor;
    using Akka.Configuration;
    using Akka.DI.Core;
    using Akka.DI.Ninject;

    using Ninject;

    using NLog;

    using Profesor79.Merge.Contracts;
    using Profesor79.Merge.Domain;

    /// <summary>The system start.</summary>
    public class SystemLauncher
    {
        /// <summary>The logger.</summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>The _root.</summary>
        private IActorRef _root;

        /// <summary>Initializes a new instance of the <see cref="SystemLauncher"/> class.</summary>
        public SystemLauncher() { MergeActorSystem = ActorSystem.Create("MergeActorSystem"); }

        /// <summary>Gets the dialer actor system.</summary>
        public ActorSystem MergeActorSystem { get; }

        /// <summary>The start.</summary>
        /// <param name="inputFilePath">The input file path.</param>
        /// <param name="outputFilePath">The output file path.</param>
        public void Start(string inputFilePath, string outputFilePath)
        {
            var container = CreateKernel();

            // propsResolver instance takes care about injection
            // nothing more to do with it :-)
            var propsResolver = new NinjectDependencyResolver(container, MergeActorSystem);

            // first actor 
            _root = MergeActorSystem.ActorOf(MergeActorSystem.DI().Props<RootActor>(), $"root{Guid.NewGuid().ToString().Substring(0, 6)}");

            _root.Tell(new RootActorMessages.StartSystem(inputFilePath, outputFilePath));
            MergeActorSystem.WhenTerminated.Wait();
        }

        /// <summary>The stop.</summary>
        public void Stop() { MergeActorSystem.Terminate(); }

        /// <summary>The create kernel.</summary>
        /// <returns>The <see cref="IKernel"/>.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            try
            {
                RegisterServices(kernel);
                return kernel;
            }
            catch (Exception e)
            {
                Logger.Error("Something goes wrong: KERNEL .....");
                Logger.Error(e);
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>Load your modules or register your services here!</summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            var config = ConfigurationFactory.Load();
            var dev = config.GetString("application.environment");
            var configBase = $"application.{dev}.";
            var useFixedConfigFile = config.GetBoolean($"{configBase}useFixedConfigFile");
            if (useFixedConfigFile)
            {
                var fixedConfigClassName = config.GetString($"{configBase}FixedConfigClassName");

                // Type typeYouWant = Type.GetType("NamespaceOfType.TypeName, AssemblyName");
                // http://stackoverflow.com/a/25913864/5919473
                var type = Type.GetType($"Profesor79.Merge.Domain.Configuration.{fixedConfigClassName}, Profesor79.Merge.Domain");
                kernel.Bind<ISystemConfiguration>().To(type);
            }
            else
            {
                kernel.Bind<ISystemConfiguration>().To<AppSettingsConfiguration>().InSingletonScope();
            }

            kernel.Bind<IFileWriter>().To<FileWriter>();
            kernel.Bind<IFileReader>().To<FileReader>();

            // kernel.Bind<IHttpWrapper>().To<HttpWrapper>();
        }
    }
}

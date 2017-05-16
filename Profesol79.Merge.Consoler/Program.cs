//  --------------------------------------------------------------------------------------------------------------------
// <copyright company="WPE" file="Program.cs">
// Copyright (c) 2017 All Right Reserved
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
// </copyright>
// <summary>
// Created: 2017-05-15, 2:37 PM
// Last changed by: A happy WPE candidate, 2017-05-16, 10:48 AM 
// </summary>
//   --------------------------------------------------------------------------------------------------------------------

namespace Profesor79.Merge.Consoler
{
    using System;
    using System.Reflection;

    using NLog;

    using Profesor79.Merge.ActorSystem.RootActor;

    /// <summary>The class 1.</summary>
    public static class Program
    {
        /// <summary>The main.</summary>
        /// <param name="args">The args.</param>
        /// <returns>The <see cref="int"/>.</returns>
        private static int Main(string[] args)
        {
            var logger = LogManager.GetCurrentClassLogger();

            var assembly = Assembly.GetExecutingAssembly();
            var version = AssemblyName.GetAssemblyName(assembly.Location).Version.ToSt‌​ring();
            logger.Info($"Current version: {version}");
            logger.Info("Starting WP MERGE.");

            // we need to have 2 elements in array as a parameters
            // input and output file
            if (args.Length != 2)
            {
                logger.Error("Usage: wpe_merge <input_file> <output_file>");
                logger.Error("Example: wpe_merge \"name with spaces\" noSpacesHere");
                logger.Info("Stopping WP MERGE.");
                logger.Info("Stopped WP MERGE.");
                return -1;
            }

#if DEBUG
            Console.WriteLine("Debug session starting");
#endif
            var system = new SystemLauncher();
            var inputFile = args[0];
            var outputFile = args[1];

#if DEBUG

            // when in debug we use vs commad line parameters
            // so every execution output file is overwritten
            // this allow to keep track of history
            outputFile = $"{DateTime.Now.ToString("O").Replace(":", "_")}_{outputFile}";
#endif

            system.Start(inputFile, outputFile);

#if DEBUG
            Console.WriteLine("Debug session finshed");
            Console.WriteLine("Press enter to close");
            Console.ReadLine();
#endif
            logger.Info("Stopping....");
            logger.Info("Stopped WP MERGE.");
            return 0;
        }
    }
}

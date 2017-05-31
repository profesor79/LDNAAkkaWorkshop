//  --------------------------------------------------------------------------------------------------------------------
// <copyright company="profesor79.pl" file="Program.cs">
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

using Serilog;

namespace Profesor79.Merge.Consoler
{
    using System;
    using System.Reflection;



    using Profesor79.Merge.ActorSystem.RootActor;

    /// <summary>The class 1.</summary>
    public static class Program
    {
        /// <summary>The main.</summary>
        /// <param name="args">The args.</param>
        /// <returns>The <see cref="int"/>.</returns>
        private static int Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
    .WriteTo.LiterateConsole()
.CreateLogger();


            var assembly = Assembly.GetExecutingAssembly();
            var version = AssemblyName.GetAssemblyName(assembly.Location).Version.ToSt‌​ring();
            Log.Logger.Information($"Current version: {version}");
            Log.Logger.Information("Starting LDNA DEMO.");

            // we need to have 2 elements in array as a parameters
            // input and output file
            if (args.Length != 2)
            {
                Log.Logger.Information("Usage: Profesor79.Merge.Consoler <input_file> <output_file>");
                Log.Logger.Information("Example: Profesor79.Merge.Consoler \"name with spaces\" noSpacesHere");
                Log.Logger.Information("Stopping LDNA DEMO.");
                Log.Logger.Information("Stopped LDNA DEMO.");
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
            Console.WriteLine("Debug LDNA DEMO session finshed");
            Console.WriteLine("Press enter to close");
            Console.ReadLine();
#endif
            Log.Logger.Information("Stopping LDNA DEMO....");
            Log.Logger.Information("Stopped LDNA DEMO.");
            return 0;
        }
    }
}

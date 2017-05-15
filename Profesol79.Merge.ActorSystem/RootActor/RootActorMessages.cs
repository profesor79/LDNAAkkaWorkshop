//  --------------------------------------------------------------------------------------------------------------------
// <copyright company="Profesor79. file="RootActorMessages.cs">
// Copyright (c) 2017 All Right Reserved
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
// </copyright>
// <summary>
// Created: 2017-04-15, 10:07 PM
// Last changed by: profesor79, 2017-04-27, 4:04 PM 
// </summary>
//   --------------------------------------------------------------------------------------------------------------------

namespace Profesor79.Merge.ActorSystem.RootActor
{
    using System.Collections.Generic;

    using Akka.Actor;

    /// <summary>The class holds all messages that are important system wide and are used to start/stop system wide actions. </summary>
    public class RootActorMessages
    {
        /// <summary>The address book.</summary>
        public class AddressBook
        {
            /// <summary>Initializes a new instance of the <see cref="AddressBook"/> class.</summary>
            /// <param name="actorDictionary">The actor dictionary.</param>
            public AddressBook(Dictionary<string, IActorRef> actorDictionary) { ActorDictionary = actorDictionary; }

            /// <summary>Gets the actor dictionary.</summary>
            public Dictionary<string, IActorRef> ActorDictionary { get; }
        }

        /// <summary>The end of file.</summary>
        public class EndOfFile
        {
        }

        /// <summary>The fatal error.</summary>
        public class FatalError
        {
            /// <summary>Initializes a new instance of the <see cref="FatalError"/> class.</summary>
            /// <param name="description">The description.</param>
            public FatalError(string description) { Description = description; }

            /// <summary>Gets the description.</summary>
            public string Description { get; }
        }

        /// <summary>The global network failure message.</summary>
        public class GlobalNetworkFailure
        {
        }

        /// <summary>The halt system.</summary>
        public class HaltSystem
        {
        }

        /// <summary>The process finished.</summary>
        public class ProcessFinished
        {
        }

        /// <summary>The input file ready to process message.</summary>
        /// <summary>The start system message.</summary>
        public class StartSystem
        {
            /// <summary>Initializes a new instance of the <see cref="StartSystem"/> class.</summary>
            /// <param name="inputFilePath">The input file path.</param>
            /// <param name="outputFilePath">The output file path.</param>
            public StartSystem(string inputFilePath, string outputFilePath)
            {
                InputFilePath = inputFilePath;
                OutputFilePath = outputFilePath;
            }

            /// <summary>Gets the input file path.</summary>
            public string InputFilePath { get; }

            /// <summary>Gets the output file path.</summary>
            public string OutputFilePath { get; }
        }
    }
}

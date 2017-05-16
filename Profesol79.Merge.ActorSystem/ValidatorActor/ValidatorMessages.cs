//  --------------------------------------------------------------------------------------------------------------------
// <copyright company="Profesor79. file="ValidatorMessages.cs">
// Copyright (c) 2017 All Right Reserved
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
// </copyright>
// <summary>
// Created: 2017-04-23, 10:34 PM
// Last changed by: profesor79, 2017-04-27, 4:04 PM 
// </summary>
//   --------------------------------------------------------------------------------------------------------------------

namespace Profesor79.Merge.ActorSystem.ValidatorActor
{
    using System.Collections.Generic;

    using Akka.Actor;

    /// <summary>The validator messages.</summary>
    public partial class ValidatorMessages
    {
        /// <summary>The api state.</summary>
        public class ApiState
        {
            /// <summary>Initializes a new instance of the <see cref="ApiState"/> class.</summary>
            /// <param name="isOnline">The is online.</param>
            public ApiState(bool isOnline) { IsOnline = isOnline; }

            /// <summary>Gets a value indicating whether is online.</summary>
            public bool IsOnline { get; }
        }

        /// <summary>The cannot create file.</summary>
        public class CannotCreateFile
        {
        }

        /// <summary>The cannot create header.</summary>
        public class CannotCreateHeader
        {
        }

        /// <summary>The file created.</summary>
        public class FileCreated
        {
        }

        /// <summary>The header created.</summary>
        public class HeaderCreated
        {
        }

        /// <summary>The input file ready to process.</summary>
        public class InputFileReadyToProcess
        {
        }

        /// <summary>The validate.</summary>
        public class Validate
        {
            /// <summary>Initializes a new instance of the <see cref="Validate"/> class.</summary>
            /// <param name="inputFilePath">The input file path.</param>
            /// <param name="outputFilePath">The output file path.</param>
            /// <param name="actorDictionary">The actor dictionary.</param>
            public Validate(string inputFilePath, string outputFilePath, Dictionary<string, IActorRef> actorDictionary)
            {
                InputFilePath = inputFilePath;
                OutputFilePath = outputFilePath;
                ActorDictionary = actorDictionary;
            }

            /// <summary>Gets the actor dictionary.</summary>
            public Dictionary<string, IActorRef> ActorDictionary { get; }

            /// <summary>Gets the input file path.</summary>
            public string InputFilePath { get; }

            /// <summary>Gets the output file path.</summary>
            public string OutputFilePath { get; }
        }
    }
}

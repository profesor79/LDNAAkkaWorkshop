//  --------------------------------------------------------------------------------------------------------------------
// <copyright company="Profesor79. file="FlowControlMessages.cs">
// Copyright (c) 2017 All Right Reserved
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
// </copyright>
// <summary>
// Created: 2017-04-23, 10:05 PM
// Last changed by: profesor79, 2017-04-27, 4:04 PM 
// </summary>
//   --------------------------------------------------------------------------------------------------------------------

namespace Profesor79.Merge.ActorSystem.WebCrawler
{
    using System.Collections.Generic;

    using Akka.Actor;

    /// <summary>The flow control messages.</summary>
    public class FlowControlMessages
    {
        /// <summary>The actor dictionary.</summary>
        public class ActorDictionary
        {
            /// <summary>Initializes a new instance of the <see cref="ActorDictionary"/> class.</summary>
            /// <param name="actorDictionary">The actor dictionary.</param>
            public ActorDictionary(Dictionary<string, IActorRef> actorDictionary) { Dictionary = actorDictionary; }

            /// <summary>Gets the dictionary.</summary>
            public Dictionary<string, IActorRef> Dictionary { get; }
        }

        /// <summary>The api state.</summary>
        public class ApiState
        {
            /// <summary>Initializes a new instance of the <see cref="ApiState"/> class.</summary>
            /// <param name="isOnline">The is online.</param>
            public ApiState(bool isOnline) { IsOnline = isOnline; }

            /// <summary>Gets a value indicating whether is online.</summary>
            public bool IsOnline { get; }
        }

        /// <summary>The bad line.</summary>
        public class BadLine
        {
        }

        /// <summary>The eo f.</summary>
        public class EoF
        {
        }

        /// <summary>The file closed.</summary>
        public class FileClosed
        {
        }

        /// <summary>The get new lines for crawler.</summary>
        public class GetNewLinesForCrawler
        {
        }

        /// <summary>The lines read.</summary>
        public class LinesRead
        {
            /// <summary>Initializes a new instance of the <see cref="LinesRead"/> class.</summary>
            /// <param name="linesCount">The lines count.</param>
            public LinesRead(uint linesCount) { LinesCount = linesCount; }

            /// <summary>Gets the lines count.</summary>
            public uint LinesCount { get; }
        }

        /// <summary>The lines saved.</summary>
        public class LinesSaved
        {
            /// <summary>Initializes a new instance of the <see cref="LinesSaved"/> class.</summary>
            /// <param name="linesCount">The lines count.</param>
            public LinesSaved(uint linesCount) { LinesCount = linesCount; }

            /// <summary>Gets the lines count.</summary>
            public uint LinesCount { get; }
        }

        /// <summary>The start processing.</summary>
        public class StartProcessing
        {
            /// <summary>Initializes a new instance of the <see cref="StartProcessing"/> class.</summary>
            /// <param name="actorDictionary">The actor dictionary.</param>
            public StartProcessing(Dictionary<string, IActorRef> actorDictionary) { ActorDictionary = actorDictionary; }

            /// <summary>Gets the actor dictionary.</summary>
            public Dictionary<string, IActorRef> ActorDictionary { get; }
        }

        /// <summary>The timer.</summary>
        public class Timer
        {
        }

        /// <summary>The valid line.</summary>
        public class ValidLine
        {
        }

        /// <summary>The web api got bad response.</summary>
        public class WebApiGotBadResponse
        {
        }

        /// <summary>The web api got valid response.</summary>
        public class WebApiGotValidResponse
        {
        }
    }
}

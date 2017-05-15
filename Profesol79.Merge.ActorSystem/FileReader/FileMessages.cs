//  --------------------------------------------------------------------------------------------------------------------
// <copyright company="Profesor79. file="FileMessages.cs">
// Copyright (c) 2017 All Right Reserved
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
// </copyright>
// <summary>
// Created: 2017-04-15, 10:41 PM
// Last changed by: profesor79, 2017-04-27, 4:04 PM 
// </summary>
//   --------------------------------------------------------------------------------------------------------------------

namespace Profesor79.Merge.ActorSystem.FileReader
{
    using System.Collections.Generic;

    using Akka.Actor;

    /// <summary>The file reader actor.</summary>
    public class FileMessages
    {
        /// <summary>
        /// This message is sent when file that was tested as readable is now not accessible 
        /// for any reason
        /// </summary>
        public class CannotReadFile
        {
        }

        /// <summary>The read file message.</summary>
        public class CheckInputFile
        {
            /// <summary>Initializes a new instance of the <see cref="CheckInputFile"/> class.</summary>
            /// <param name="fileName">The file name.</param>
            /// <param name="actorDictionary">The actor dictionary.</param>
            /// <param name="regexPattern">The regex pattern.</param>
            public CheckInputFile(string fileName, Dictionary<string, IActorRef> actorDictionary, string regexPattern)
            {
                FileName = fileName;
                ActorDictionary = actorDictionary;
                RegexPattern = regexPattern;
            }

            /// <summary>Gets the actor dictionary.</summary>
            public Dictionary<string, IActorRef> ActorDictionary { get; private set; }

            /// <summary>Gets the file name.</summary>
            public string FileName { get; private set; }

            /// <summary>Gets the regex pattern.</summary>
            public string RegexPattern { get; }
        }

        /// <summary>The close file.</summary>
        public class CloseFile
        {
        }

        /// <summary>The end of file.</summary>
        public class EndOfFile
        {
        }

        /// <summary>The header message.</summary>
        public class FileHeader
        {
            /// <summary>Initializes a new instance of the <see cref="FileHeader"/> class.</summary>
            /// <param name="header">The header.</param>
            public FileHeader(string header) { Header = header; }

            /// <summary>Gets the header.</summary>
            public string Header { get; }
        }

        /// <summary>The file lines.</summary>
        public class FileLines
        {
            /// <summary>Initializes a new instance of the <see cref="FileLines"/> class.</summary>
            /// <param name="lines">The lines.</param>
            public FileLines(List<string> lines) { Lines = lines; }

            /// <summary>Gets the lines.</summary>
            public List<string> Lines { get; }
        }

        /// <summary>The file validated.</summary>
        public class FileValidated
        {
            /// <summary>Initializes a new instance of the <see cref="FileValidated"/> class.</summary>
            /// <param name="fileName">The file name.</param>
            /// <param name="canBeRead">The can be read.</param>
            public FileValidated(string fileName, bool canBeRead)
            {
                FileName = fileName;
                CanBeRead = canBeRead;
            }

            /// <summary>Gets a value indicating whether can be read.</summary>
            public bool CanBeRead { get; }

            /// <summary>Gets the file name.</summary>
            public string FileName { get; }
        }

        /// <summary>The get next chunk.</summary>
        public class GetNextChunk
        {
        }

        /// <summary>The read header.</summary>
        public class ReadHeader
        {
        }

        /// <summary>The read lines.</summary>
        public class ReadLines
        {
            /// <summary>Initializes a new instance of the <see cref="ReadLines"/> class.</summary>
            /// <param name="howMany">The how many.</param>
            public ReadLines(uint howMany) { HowMany = howMany; }

            /// <summary>Gets the how many.</summary>
            public uint HowMany { get; }
        }

        /// <summary>The send lines to crawler.</summary>
        public class SendLinesToCrawler
        {
        }

        /// <summary>The start processing.</summary>
        public class StartProcessing
        {
            /// <summary>Initializes a new instance of the <see cref="StartProcessing"/> class.</summary>
            /// <param name="actorDict">The actor dict.</param>
            public StartProcessing(Dictionary<string, IActorRef> actorDict) { ActorDict = actorDict; }

            /// <summary>Gets the actor dict.</summary>
            public Dictionary<string, IActorRef> ActorDict { get; }
        }
    }
}

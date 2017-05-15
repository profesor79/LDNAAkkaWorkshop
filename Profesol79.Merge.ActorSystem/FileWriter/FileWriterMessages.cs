//  --------------------------------------------------------------------------------------------------------------------
// <copyright company="Profesor79. file="FileWriterMessages.cs">
// Copyright (c) 2017 All Right Reserved
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
// </copyright>
// <summary>
// Created: 2017-04-21, 12:21 AM
// Last changed by: profesor79, 2017-04-27, 4:04 PM 
// </summary>
//   --------------------------------------------------------------------------------------------------------------------

namespace Profesor79.Merge.ActorSystem.FileWriter
{
    using System.Collections.Generic;

    using Akka.Actor;

    using Profesor79.Merge.Models;

    /// <summary>The file writer messages.</summary>
    public class FileWriterMessages
    {
        /// <summary>The close file.</summary>
        public class CloseFile
        {
        }

        /// <summary>The create file.</summary>
        public class CreateFile
        {
            /// <summary>Initializes a new instance of the <see cref="CreateFile"/> class.</summary>
            /// <param name="fileName">The file name.</param>
            /// <param name="stopIfDestinationFileExists">The stop if destination file exists.</param>
            /// <param name="actorDict">The actor dict.</param>
            public CreateFile(string fileName, bool stopIfDestinationFileExists, Dictionary<string, IActorRef> actorDict)
            {
                FileName = fileName;
                StopIfDestinationFileExists = stopIfDestinationFileExists;
                ActorDict = actorDict;
            }

            /// <summary>Gets the actor dict.</summary>
            public Dictionary<string, IActorRef> ActorDict { get; }

            /// <summary>Gets the file name.</summary>
            public string FileName { get; }

            /// <summary>Gets a value indicating whether stop if destination file exists.</summary>
            public bool StopIfDestinationFileExists { get; }
        }

        /// <summary>The lines saved.</summary>
        public class LinesSaved
        {
            /// <summary>Initializes a new instance of the <see cref="LinesSaved"/> class.</summary>
            /// <param name="linesCount">The lines count.</param>
            public LinesSaved(int linesCount) { LinesCount = linesCount; }

            /// <summary>Gets the lines count.</summary>
            public int LinesCount { get; }
        }

        /// <summary>The save web response.</summary>
        public class SaveWebResponse
        {
            /// <summary>Initializes a new instance of the <see cref="SaveWebResponse"/> class.</summary>
            /// <param name="mergeObjectDto">The merge object dto.</param>
            public SaveWebResponse(MergeObjectDto mergeObjectDto) { MergeObjectDto = mergeObjectDto; }

            /// <summary>Gets the merge object dto.</summary>
            public MergeObjectDto MergeObjectDto { get; }
        }

        /// <summary>The timer.</summary>
        public class Timer
        {
        }

        /// <summary>The waiting for header.</summary>
        /// <summary>The write header.</summary>
        public class WriteHeader
        {
            /// <summary>Initializes a new instance of the <see cref="WriteHeader"/> class.</summary>
            /// <param name="fileHeader">The file header.</param>
            public WriteHeader(string fileHeader) { FileHeader = fileHeader; }

            /// <summary>Gets the file header.</summary>
            public string FileHeader { get; }
        }
    }
}

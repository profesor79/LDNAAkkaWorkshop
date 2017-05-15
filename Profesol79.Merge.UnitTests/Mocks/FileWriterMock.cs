//  --------------------------------------------------------------------------------------------------------------------
// <copyright company="WPE" file="FileWriterMock.cs">
// Copyright (c) 2017 All Right Reserved
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
// </copyright>
// <summary>
// Created: 2017-04-21, 9:24 PM
// Last changed by: profesor79, 2017-04-27, 4:04 PM 
// </summary>
//   --------------------------------------------------------------------------------------------------------------------

namespace Profesor79.Merge.UnitTests.Mocks
{
    using System;
    using System.Collections.Generic;

    using Profesor79.Merge.Contracts;

    /// <summary>The file reader mock.</summary>
    public class FileWriterMock : IFileWriter
    {
        /// <summary>The append to file.</summary>
        /// <param name="lines">The lines.</param>
        public void AppendToFile(List<string> lines) { }

        /// <summary>The close file.</summary>
        public void CloseFile() { }

        /// <summary>The create file.</summary>
        /// <param name="fileName">The file name.</param>
        /// <param name="stopIfDestinationFileExists">The stop if destination file exists.</param>
        /// <exception cref="AccessViolationException"></exception>
        public void CreateFile(string fileName, bool stopIfDestinationFileExists)
        {
            if (fileName.Contains("throw"))
            {
                throw new AccessViolationException();
            }
        }

        /// <summary>The write header.</summary>
        /// <param name="fileHeader">The file header.</param>
        /// <exception cref="AccessViolationException"></exception>
        public void WriteHeader(string fileHeader)
        {
            if (fileHeader.Contains("throw"))
            {
                throw new AccessViolationException();
            }
        }
    }
}

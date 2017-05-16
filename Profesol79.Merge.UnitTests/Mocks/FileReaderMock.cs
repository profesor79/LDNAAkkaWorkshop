//  --------------------------------------------------------------------------------------------------------------------
// <copyright company="profesor79" file="FileReaderMock.cs">
// Copyright (c) 2017 All Right Reserved
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
// </copyright>
// <summary>
// Created: 2017-04-15, 10:28 PM
// Last changed by: profesor79, 2017-04-27, 4:04 PM 
// </summary>
//   --------------------------------------------------------------------------------------------------------------------

namespace Profesor79.Merge.UnitTests.Mocks
{
    using System.Collections.Generic;
    using System.IO;

    using Profesor79.Merge.Contracts;

    /// <summary>The file reader mock.</summary>
    public class FileReaderMock : IFileReader
    {
        /// <summary>The can we read file.</summary>
        /// <param name="fileName">The file name.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public bool CanWeReadFile(string fileName) { return fileName.StartsWith("ok"); }

        /// <summary>The read all.</summary>
        /// <param name="fileName">The file name.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public string ReadAll(string fileName) { return null; }

        /// <summary>The read first line.</summary>
        /// <param name="fileName">The file name.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public string ReadFirstLine(string fileName)
        {
            if (fileName.Contains("headerThrows"))
            {
                throw new FileLoadException();
            }

            return fileName;
        }

        /// <summary>The read lines.</summary>
        /// <param name="howMany">The how many.</param>
        /// <returns>The <see cref="List"/>.</returns>
        /// <exception cref="FileLoadException"></exception>
        public List<string> ReadLines(uint howMany)
        {
            if (howMany == 0)
            {
                throw new FileLoadException();
            }

            if (howMany == 1)
            {
                return new List<string>();
            }

            return new List<string> { "1", "2", "3" };
        }
    }
}

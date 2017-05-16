//  --------------------------------------------------------------------------------------------------------------------
// <copyright company="profesor79" file="FileReader.cs">
// Copyright (c) 2017 All Right Reserved
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
// </copyright>
// <summary>
// Created: 2017-04-15, 1:47 AM
// Last changed by: profesor79, 2017-04-27, 4:04 PM 
// </summary>
//   --------------------------------------------------------------------------------------------------------------------

namespace Profesor79.Merge.Domain
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    using NLog;

    using Profesor79.Merge.Contracts;

    /// <summary>The file reader.</summary>
    public class FileReader : IFileReader
    {
        /// <summary>The logger.</summary>
        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>The _stream reader.</summary>
        private StreamReader _streamReader;

        /// <summary>The can we read file.</summary>
        /// <param name="fileName">The file name.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public bool CanWeReadFile(string fileName)
        {
            var checkResult = false;
            try
            {
                // this will throw exception when file name contains invalid characters
                // ReSharper disable once ReturnValueOfPureMethodIsNotUsed 
                Path.GetFullPath(fileName);

                checkResult = File.Exists(fileName);
            }
            catch (Exception e)
            {
                checkResult = false;
                Logger.Fatal(e);
            }

            return checkResult;
        }

        /// <summary>The read all.</summary>
        /// <param name="fileName">The file name.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public string ReadAll(string fileName) { return _streamReader.ReadToEndAsync().Result; }

        /// <summary>The read first line.</summary>
        /// <param name="fileName">The file name.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public string ReadFirstLine(string fileName)
        {
            try
            {
                // setup stream reader here with bom detection
                _streamReader = new StreamReader(fileName, Encoding.UTF8, true);

                // stream reader will deal with line endings 
                var firstLine = _streamReader.ReadLine();

                return firstLine;
            }
            catch (Exception e)
            {
                Logger.Fatal(e);
                return string.Empty;
            }
        }

        /// <summary>The read lines.</summary>
        /// <param name="howMany">The how many.</param>
        /// <returns>The <see cref="List"/>.</returns>
        public List<string> ReadLines(uint howMany)
        {
            var listOfLines = new List<string>();
            for (var i = 0; i < howMany; i++)
            {
                if (!_streamReader.EndOfStream)
                {
                    listOfLines.Add(_streamReader.ReadLine());
                }
                else
                {
                    break; // eof
                }
            }

            return listOfLines;
        }
    }
}

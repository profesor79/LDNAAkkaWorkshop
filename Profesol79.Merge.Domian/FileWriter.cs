//  --------------------------------------------------------------------------------------------------------------------
// <copyright company="profesor79.pl" file="FileWriter.cs">
// Copyright (c) 2017 All Right Reserved
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
// </copyright>
// <summary>
// Created: 2017-05-15, 2:37 PM
// Last changed by: profesor79, 2017-05-26, 8:21 AM 
// </summary>
//   --------------------------------------------------------------------------------------------------------------------

namespace Profesor79.Merge.Domain
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    using NLog;

    using Profesor79.Merge.Contracts;

    /// <summary>The file writer.</summary>
    public class FileWriter : IFileWriter
    {
        /// <summary>The logger.</summary>
        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>The _file stream.</summary>
        private FileStream _fileStream;

        /// <summary>The _stream writer.</summary>
        private StreamWriter _streamWriter;

        /// <summary>The append to file.</summary>
        /// <param name="lines">The lines.</param>
        public void AppendToFile(List<string> lines)
        {
            foreach (var line in lines)
            {
                _streamWriter.WriteLine(line);
            }

            _streamWriter.Flush();
        }

        /// <summary>The close file.</summary>
        public void CloseFile()
        {
            _streamWriter.Flush();
            _streamWriter.Close();
            _streamWriter.Dispose();
            _fileStream.Flush();
            _fileStream.Close();
            _fileStream.Dispose();
        }

        /// <summary>The create file.</summary>
        /// <param name="fileName">The file name.</param>
        /// <param name="stopIfDestinationFileExists">The stop if destination file exists.</param>
        /// <exception cref="FileLoadException"></exception>
        public void CreateFile(string fileName, bool stopIfDestinationFileExists)
        {
            if (stopIfDestinationFileExists)
            {
                var exists = File.Exists(fileName);
                if (exists)
                {
                    throw new FileLoadException("file exist");
                }
            }

            _fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None);
            _streamWriter = new StreamWriter(_fileStream, Encoding.UTF8, 512, true);
        }

        /// <summary>The write header.</summary>
        /// <param name="fileHeader">The file header.</param>
        public void WriteHeader(string fileHeader)
        {
            _streamWriter.WriteLine(fileHeader);
            _streamWriter.Flush();
        }
    }
}

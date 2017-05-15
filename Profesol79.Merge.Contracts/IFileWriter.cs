//  --------------------------------------------------------------------------------------------------------------------
// <copyright company="WPE" file="IFileWriter.cs">
// Copyright (c) 2017 All Right Reserved
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
// </copyright>
// <summary>
// Created: 2017-04-15, 1:45 AM
// Last changed by: profesor79, 2017-04-27, 4:04 PM 
// </summary>
//   --------------------------------------------------------------------------------------------------------------------

namespace Profesor79.Merge.Contracts
{
    using System.Collections.Generic;

    /// <summary>The FileWriter interface.</summary>
    public interface IFileWriter
    {
        /// <summary>The append to file.</summary>
        /// <param name="lines">The lines.</param>
        void AppendToFile(List<string> lines);

        /// <summary>The close file.</summary>
        void CloseFile();

        /// <summary>The create file.</summary>
        /// <param name="fileName">The file name.</param>
        /// <param name="stopIfDestinationFileExists">The stop If Destination File Exists.</param>
        void CreateFile(string fileName, bool stopIfDestinationFileExists);

        /// <summary>The write header.</summary>
        /// <param name="fileHeader">The file header.</param>
        void WriteHeader(string fileHeader);
    }
}

//  --------------------------------------------------------------------------------------------------------------------
// <copyright company="WPE" file="IFileReader.cs">
// Copyright (c) 2017 All Right Reserved
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
// </copyright>
// <summary>
// Created: 2017-05-15, 2:37 PM
// Last changed by: A happy WPE candidate, 2017-05-16, 10:47 AM 
// </summary>
//   --------------------------------------------------------------------------------------------------------------------

namespace Profesor79.Merge.Contracts
{
    using System.Collections.Generic;

    /// <summary>The FileReader interface.</summary>
    public interface IFileReader
    {
        /// <summary>The can we read file.</summary>
        /// <param name="fileName">The file name.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        bool CanWeReadFile(string fileName);

        /// <summary>The read all.</summary>
        /// <param name="fileName">The file name.</param>
        /// <returns>The <see cref="string"/>.</returns>
        string ReadAll(string fileName);

        /// <summary>The read first line.</summary>
        /// <param name="fileName">The file name.</param>
        /// <returns>The <see cref="string"/>.</returns>
        string ReadFirstLine(string fileName);

        /// <summary>The read lines.</summary>
        /// <param name="howMany">The how many.</param>
        /// <returns>The <see cref="List"/>.</returns>
        List<string> ReadLines(uint howMany);
    }
}

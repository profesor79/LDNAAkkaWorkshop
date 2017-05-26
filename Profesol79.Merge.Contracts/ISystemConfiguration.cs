//  --------------------------------------------------------------------------------------------------------------------
// <copyright company="profesor79.pl" file="ISystemConfiguration.cs">
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

namespace Profesor79.Merge.Contracts
{
    /// <summary>The SystemConfiguration interface.</summary>
    public interface ISystemConfiguration
    {
        /// <summary>Gets or sets the api end point.</summary>
        string ApiEndPoint { get; set; }

        /// <summary>Gets or sets the crawler actors count.</summary>
        uint CrawlerActorsCount { get; set; }

        /// <summary>Gets or sets the csv line validation regex.</summary>
        string CsvLineValidationRegex { get; set; }

        /// <summary>Gets or sets the data distributor actor count.</summary>
        uint DataDistributorActorCount { get; set; }

        /// <summary>Gets or sets the date validation regex.</summary>
        string DateValidationRegex { get; set; }

        /// <summary>Gets or sets the environment.</summary>
        string Environment { get; set; }

        /// <summary>Gets or sets the header validation regex.</summary>
        string HeaderValidationRegex { get; set; }

        /// <summary>Gets or sets the http timeout retries.</summary>
        uint HttpRetries { get; set; }

        /// <summary>Gets or sets the http timeout in miniseconds.</summary>
        uint HttpTimeoutInMiniseconds { get; set; }

        /// <summary>Gets or sets the initial api error threshold to shutdown system.</summary>
        uint InitialApiErrorThresholdToShutdownSystem { get; set; }

        /// <summary>Gets or sets the internal chunk size used to read lines and distribute in small batches.</summary>
        uint InternalChunkSize { get; set; }

        /// <summary>Gets or sets the output file header.</summary>
        string OutputFileHeader { get; set; }

        /// <summary>Gets or sets the read lines batch size.</summary>
        uint ReadLinesBatchSize { get; set; }

        /// <summary>Gets or sets a value indicating whether stop if destination file exists.</summary>
        bool StopIfDestinationFileExists { get; set; }

        /// <summary>Gets or sets the worker idle time.</summary>
        double WorkerIdleTime { get; set; }

        /// <summary>Gets or sets the write wait cycle in miniseconds.</summary>
        uint WriteWaitCycleInMiniseconds { get; set; }
    }
}

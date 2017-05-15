//  --------------------------------------------------------------------------------------------------------------------
// <copyright company="WPE" file="DevMockConfig.cs">
// Copyright (c) 2017 All Right Reserved
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
// </copyright>
// <summary>
// Created: 2017-04-24, 10:25 PM
// Last changed by: profesor79, 2017-04-27, 4:04 PM 
// </summary>
//   --------------------------------------------------------------------------------------------------------------------

namespace Profesor79.Merge.Domain.Configuration
{
    using Profesor79.Merge.Contracts;

    /// <summary>The app setting configuration.</summary>
    public class DevMockConfig : ISystemConfiguration
    {
        /// <summary>Initializes a new instance of the <see cref="DevMockConfig"/> class.</summary>
        public DevMockConfig()
        {
            ApiEndPoint = "http://localhost:51069/api/Shop";


            Environment = "DevMock";
            CsvLineValidationRegex = @"^(\d+)\s?,([^,]*?),([^,]*?),(\d+\/\d+\/\d+[^,]\s*?)$";
            HeaderValidationRegex = "^data id,shop name,city,created on$";
            DateValidationRegex = @"(\d+?\/\d+?\/\d+?[^,]\s*?)$";

            CrawlerActorsCount = 16;
            HttpTimeoutInMiniseconds = 2000;
            HttpRetries = 2;
            WorkerIdleTime = 200;
            InitialApiErrorThresholdToShutdownSystem = 20;

            OutputFileHeader = "data id,sale";
            StopIfDestinationFileExists = false;
            WriteWaitCycleInMiniseconds = 1000;

            ReadLinesBatchSize = 1000;
            InternalChunkSize = 50;
            DataDistributorActorCount = 4;
        }

        /// <summary>Gets or sets the api end point.</summary>
        public string ApiEndPoint { get; set; }

        /// <summary>Gets or sets the crawler actors count.</summary>
        public uint CrawlerActorsCount { get; set; }

        /// <summary>Gets or sets the csv line validation regex.</summary>
        public string CsvLineValidationRegex { get; set; }

        /// <summary>Gets or sets the data distributor actor count.</summary>
        public uint DataDistributorActorCount { get; set; }

        /// <summary>Gets or sets the date validation regex.</summary>
        public string DateValidationRegex { get; set; }

        /// <summary>Gets or sets the environment.</summary>
        public string Environment { get; set; }

        /// <summary>Gets or sets the header validation regex.</summary>
        public string HeaderValidationRegex { get; set; }

        /// <summary>Gets or sets the http retries.</summary>
        public uint HttpRetries { get; set; }

        /// <summary>Gets or sets the http timeout in miniseconds.</summary>
        public uint HttpTimeoutInMiniseconds { get; set; }

        /// <summary>Gets or sets the initial api error threshold to shutdown system.</summary>
        public uint InitialApiErrorThresholdToShutdownSystem { get; set; }

        /// <summary>Gets or sets the internal chunk size.</summary>
        public uint InternalChunkSize { get; set; }

        /// <summary>Gets or sets the output file header.</summary>
        public string OutputFileHeader { get; set; }

        /// <summary>Gets or sets the read lines batch size.</summary>
        public uint ReadLinesBatchSize { get; set; }

        /// <summary>Gets or sets a value indicating whether stop if destination file exists.</summary>
        public bool StopIfDestinationFileExists { get; set; }

        /// <summary>Gets or sets the worker idle time.</summary>
        public double WorkerIdleTime { get; set; }

        /// <summary>Gets or sets the write wait cycle in miniseconds.</summary>
        public uint WriteWaitCycleInMiniseconds { get; set; }
    }
}

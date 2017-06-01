//  --------------------------------------------------------------------------------------------------------------------
// <copyright company="profesor79.pl" file="AppSettingsConfiguration.cs">
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

using System;
using System.Threading;

namespace Profesor79.Merge.Domain
{
    using Akka.Configuration;

    using Profesor79.Merge.Contracts;

    /// <summary>The app setting configuration.</summary>
    public class AppSettingsConfiguration : ISystemConfiguration
    {

        private string GetConfiguration()
        {
            var needToRead = true;
            string text = string.Empty;
            while (needToRead)
            {
                try
                {
                    text = System.IO.File.ReadAllText(@"C:\dockerExchange\cluster.config");

                    // check readings
                    needToRead = string.IsNullOrWhiteSpace(text);

                    if (!needToRead)
                    {
                        text = text.Replace("__hostname__", System.Net.Dns.GetHostName());
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("config file is absent");
                    Thread.Sleep(250);
                }


            }

            return text;
        }
        /// <summary>Initializes a new instance of the <see cref="AppSettingsConfiguration"/> class.</summary>
        public AppSettingsConfiguration()
        {
            var config = ConfigurationFactory.ParseString(GetConfiguration());

            var env = config.GetString("application.environment");
            var configBase = $"application.{env}.";

            Environment = config.GetString($"{configBase}Environment");

            // ****  input file ****
            CsvLineValidationRegex = config.GetString($"{configBase}CsvLineValidationRegex");
            HeaderValidationRegex = config.GetString($"{configBase}HeaderValidationRegex");
            DateValidationRegex = config.GetString($"{configBase}DateValidationRegex");

            // the batch size will determine how many lines we will read from file
            // then internal chunk size is the amout thah we will feed dispatcher with data
            // per actor system the best approach is to use more smaller chunks 
            ReadLinesBatchSize = (uint)config.GetInt($"{configBase}ReadLinesBatchSize");
            InternalChunkSize = (uint)config.GetInt($"{configBase}InternalChunkSize");

            // this actor is created in a round robin pool 
            // and it task is to send valid lines to web worker actor
            DataDistributorActorCount = (uint)config.GetInt($"{configBase}DataDistributorActorCount");

            // ****  web crawler ****
            ApiEndPoint = config.GetString($"{configBase}ApiEndPoint");

            // CrawlerActorsCount parameter set amout of workers getting data from web api
            // this setting is a throttle to the whole system
            // depend on api response time / network bandwith we can play with the settings 
            // to achive optimal point.
            CrawlerActorsCount = (uint)config.GetInt($"{configBase}CrawlerActorsCount");
            HttpTimeoutInMiniseconds = (uint)config.GetInt($"{configBase}HttpTimeoutInMiniseconds");
            HttpRetries = (uint)config.GetInt($"{configBase}HttpRetries");

            // WorkerIdleTime  - idle time before ask for new lines 
            WorkerIdleTime = (uint)config.GetInt($"{configBase}WorkerIdleTime");

            // that will indicate how fast we want to stop system if requests are failing from beginning
            InitialApiErrorThresholdToShutdownSystem = (uint)config.GetInt($"{configBase}InitialApiErrorThresholdToShutdownSystem");

            // ****  web crawler ****
            OutputFileHeader = config.GetString($"{configBase}OutputFileHeader");
            StopIfDestinationFileExists = config.GetBoolean($"{configBase}StopIfDestinationFileExists");

            // this is main interval when buffered  lines are saved
            // on slow disk it is better to save in bigger gaps
            WriteWaitCycleInMiniseconds = (uint)config.GetInt($"{configBase}WriteWaitCycleInMiniseconds");
        }

        /// <summary>Gets or sets the api end point.</summary>
        public string ApiEndPoint { get; set; }

        /// <summary>Gets or sets the api end point backup.</summary>
        public string ApiEndPointBackup { get; set; }

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

        /// <summary>Gets or sets the http timeout retries.</summary>
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

        /// <summary>Gets or sets the threshold fill factor.</summary>
        public decimal ThresholdFillFactor { get; set; }

        /// <summary>Gets or sets the worker idle time.</summary>
        public double WorkerIdleTime { get; set; }

        /// <summary>Gets or sets the write wait cycle in miniseconds.</summary>
        public uint WriteWaitCycleInMiniseconds { get; set; }
    }
}

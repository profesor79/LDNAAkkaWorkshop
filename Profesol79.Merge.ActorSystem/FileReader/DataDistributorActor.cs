//  --------------------------------------------------------------------------------------------------------------------
// <copyright company="WPE" file="DataDistributorActor.cs">
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

namespace Profesor79.Merge.ActorSystem.FileReader
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;

    using Akka.Actor;

    using Microsoft.VisualBasic.FileIO;

    using Profesor79.Merge.ActorSystem.BaseObjects;
    using Profesor79.Merge.ActorSystem.RootActor;
    using Profesor79.Merge.ActorSystem.WebCrawler;
    using Profesor79.Merge.Contracts;
    using Profesor79.Merge.Models;

    /// <summary>The data distributor actor.</summary>
    public class DataDistributorActor : BaseActorClass
    {
        /// <summary>The _date validator.</summary>
        private readonly Regex _dateValidator;

        /// <summary>The _line validator.</summary>
        private readonly Regex _lineValidator;

        /// <summary>The _system configuration.</summary>
        private readonly ISystemConfiguration _systemConfiguration;

        /// <summary>The _crawler.</summary>
        private IActorRef _crawler;

        /// <summary>The _flow control.</summary>
        private IActorRef _flowControl;

        /// <summary>The _lines read regex.</summary>
        private ulong _linesReadByRegex;

        /// <summary>The _lines read parser.</summary>
        private ulong _linesReadByTextParser;

        /// <summary>Initializes a new instance of the <see cref="DataDistributorActor"/> class.</summary>
        /// <param name="systemConfiguration">The system configuration.</param>
        public DataDistributorActor(ISystemConfiguration systemConfiguration)
        {
            _systemConfiguration = systemConfiguration;
            _lineValidator = new Regex(_systemConfiguration.CsvLineValidationRegex);
            _dateValidator = new Regex(_systemConfiguration.DateValidationRegex);

            Receive<RootActorMessages.AddressBook>(
                b =>
                    {
                        _actorDictionary = b.ActorDictionary;
                        _crawler = _actorDictionary["WebCrawlerActor"];
                        _flowControl = _actorDictionary["FlowControlActor"];
                    });

            Receive<FileMessages.FileLines>(
                lines =>
                    {
                        if (lines.Lines.Any())
                        {
                            ProcessLines(lines.Lines);
                        }
                        else
                        {
                            _flowControl.Tell(new FlowControlMessages.EoF());
                        }
                    });
        }

        /// <summary>The match in quotes.</summary>
        /// <param name="line">The line.</param>
        /// <returns>The <see cref="string[]"/>.</returns>
        private string[] MatchInQuotes(string line)
        {
            // this solution was fond on SO, as working with regex 
            // will not be so clear and easy to follow
            // http://stackoverflow.com/a/3148691/5919473
            var reader = new StringReader(line);
            var fieldParser = new TextFieldParser(reader) { TextFieldType = FieldType.Delimited };
            fieldParser.SetDelimiters(",");
            try
            {
                var currentRow = fieldParser.ReadFields();
                if (currentRow != null && currentRow.Length == 4)
                {
                    var succeed = int.TryParse(currentRow[0], out int id);
                    if (succeed)
                    {
                        // now parse last field
                        var match = _dateValidator.Match(currentRow[3]);
                        if (match.Success)
                        {
                            _log.Debug($"data row:{id}, parsed with text parser");
                            _linesReadByTextParser++;
                            return currentRow;
                        }
                    }
                }
            }
            catch (Exception)
            {
                _log.Error($"Cannot read line:{line}");
            }

            return new string[0];
        }

        /// <summary>The parse by regex.</summary>
        /// <param name="line">The line.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        private bool ParseByRegex(string line)
        {
            var match = _lineValidator.Match(line);
            if (match.Success)
            {
                var mergeObject = new MergeObjectDto { DataId = int.Parse(match.Groups[1].Value) };

                _linesReadByRegex++;
                _log.Debug($"Sending to crawler: {mergeObject.DataId}");
                _crawler.Tell(new CrawlerMessages.GetData(mergeObject));
                _flowControl.Tell(new FlowControlMessages.ValidLine());
                return true;
            }

            return false;
        }

        /// <summary>The parse by text parser.</summary>
        /// <param name="line">The line.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        private bool ParseByTextParser(string line)
        {
            // check if we have quotes first
            if (!line.Contains("\""))
            {
                return false; // fail fast
            }

            var quotesMatch = MatchInQuotes(line);
            if (quotesMatch.Any())
            {
                var mergeObject = new MergeObjectDto { DataId = int.Parse(quotesMatch[0]) };

                _log.Debug($"Sending to crawler: {mergeObject.DataId}");
                _crawler.Tell(new CrawlerMessages.GetData(mergeObject));
                _flowControl.Tell(new FlowControlMessages.ValidLine());
                return true;
            }

            return false;
        }

        /// <summary>The process lines.</summary>
        /// <param name="lines">The lines.</param>
        private void ProcessLines(List<string> lines)
        {
            // we have data validate and push request to crawler
            foreach (var line in lines)
            {
                // as quotes can be randomly in the file, 
                // it is hard to determine which parser to use
                // so let te most successful parser be called first
                if (_linesReadByRegex >= _linesReadByTextParser)
                {
                    if (ParseByRegex(line))
                    {
                        continue;
                    }
                }

                // as this heavy parser will do the job, no need to veryfy by regex
                if (ParseByTextParser(line))
                {
                    continue;
                }

                _actorDictionary["FlowControlActor"].Tell(new FlowControlMessages.BadLine());
            }
        }
    }
}

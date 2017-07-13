//  --------------------------------------------------------------------------------------------------------------------
// <copyright company="profesor79.pl" file="FileWriterActor.cs">
// Copyright (c) 2017 All Right Reserved
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
// </copyright>
// <summary>
// Created: 2017-05-15, 2:37 PM
// Last changed by: profesor79, 2017-07-13, 12:21 PM 
// </summary>
//   --------------------------------------------------------------------------------------------------------------------

namespace Profesor79.Merge.ActorSystem.FileWriter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Akka.Actor;

    using Profesor79.Merge.ActorSystem.BaseObjects;
    using Profesor79.Merge.ActorSystem.RootActor;
    using Profesor79.Merge.ActorSystem.ValidatorActor;
    using Profesor79.Merge.ActorSystem.WebCrawler;
    using Profesor79.Merge.Contracts;
    using Profesor79.Merge.Models;

    /// <summary>The file writer actor.</summary>
    public class FileWriterActor : BaseActorClass, IWithUnboundedStash
    {
        /// <summary>The _file writer.</summary>
        private readonly IFileWriter _fileWriter;

        /// <summary>The _lines.</summary>
        private readonly List<string> _lines = new List<string>();

        /// <summary>The _sorted list.</summary>
        private readonly SortedSet<long> _sortedList = new SortedSet<long>();

        /// <summary>The _system configuration.</summary>
        private readonly ISystemConfiguration _systemConfiguration;

        /// <summary>The _cancel timer.</summary>
        private ICancelable _cancelTimer;

        /// <summary>The _got lines without header.</summary>
        private bool _gotLinesWithoutHeader;

        /// <summary>Initializes a new instance of the <see cref="FileWriterActor"/> class.</summary>
        /// <param name="fileWriter">The file writer.</param>
        /// <param name="systemConfiguration"></param>
        public FileWriterActor(IFileWriter fileWriter, ISystemConfiguration systemConfiguration)
        {
            _fileWriter = fileWriter;
            _systemConfiguration = systemConfiguration;
            Receive<FileWriterMessages.CreateFile>(
                o =>
                    {
                        try
                        {
                            _fileWriter.CreateFile(o.FileName, o.StopIfDestinationFileExists);
                            Sender.Tell(new ValidatorMessages.FileCreated());
                            _actorDictionary = o.ActorDict;
                            Become(WaitingForheader);
                        }
                        catch (Exception e)
                        {
                            _log.Error($"Cannot create file:{o.FileName}", e);
                            Sender.Tell(new ValidatorMessages.CannotCreateFile());
                        }
                    });

            Receive<FileWriterMessages.Timer>(
                t =>
                    {
                        /* swallow */
                    });
        }

        /// <summary>Gets or sets the stash.</summary>
        public IStash Stash { get; set; }

        /// <summary>The post stop.</summary>
        protected override void PostStop()
        {
            _cancelTimer?.Cancel();
            base.PostStop();
        }

        /// <summary>The pre start.</summary>
        protected override void PreStart()
        {
            _cancelTimer = new Cancelable(Context.System.Scheduler);

            Context.System.Scheduler.ScheduleTellRepeatedly(
                TimeSpan.FromMilliseconds(_systemConfiguration.WriteWaitCycleInMiniseconds),
                TimeSpan.FromMilliseconds(_systemConfiguration.WriteWaitCycleInMiniseconds),
                Self,
                new FileWriterMessages.Timer(),
                Self,
                _cancelTimer);

            base.PreStart();
        }

        /// <summary>The add line to list.</summary>
        /// <param name="data">The data.</param>
        private void AddLineToList(MergeObjectDto data)
        {
            // when using at least one delivery then message can be duplicatad

            if (_sortedList.Contains(data.DataId))
            {
                Console.Write("-*-");
                return;
            }
            _sortedList.Add(data.DataId);
            _lines.Add($"{data.DataId},{data.SaleValue},{data.ActorName}");
        }

        /// <summary>The buffering data.</summary>
        private void BufferingData()
        {
            Receive<FileWriterMessages.Timer>(
                t =>
                    {
                        if (_lines.Any())
                        {
                            // _log.Info($"Saving {_lines.Count} lines");
                            SaveLinesToFile();
                        }
                    });

            Receive<FileWriterMessages.SaveWebResponse>(line => { AddLineToList(line.MergeObjectDto); });

            Receive<FileWriterMessages.CloseFile>(
                a =>
                    {
                        _fileWriter.CloseFile();
                        UnbecomeStacked();
                        Sender.Tell(new FlowControlMessages.FileClosed());
                    });
        }

        /// <summary>The save lines to file.</summary>
        private void SaveLinesToFile()
        {
            BecomeStacked(WritingData); // as we dont know how long write process will taxe

            try
            {
                _log.Debug($"Writing {_lines.Count} lines to file");
                var dt = DateTime.UtcNow;
                _fileWriter.AppendToFile(_lines);
                var elapsed = (DateTime.UtcNow - dt).TotalMilliseconds;
                _actorDictionary["FlowControlActor"].Tell(new FlowControlMessages.LinesSaved((uint)_lines.Count));

                _lines.Clear();
            }
            catch (Exception e)
            {
                _log.Error(e, "Cannot write lines");
                _actorDictionary["root"].Tell(new RootActorMessages.FatalError(e.Message));
            }

            UnbecomeStacked();
            Stash.UnstashAll();
        }

        /// <summary>The waiting forheader.</summary>
        private void WaitingForheader()
        {
            Receive<FileWriterMessages.WriteHeader>(
                h =>
                    {
                        try
                        {
                            _fileWriter.WriteHeader(h.FileHeader);
                            Sender.Tell(new ValidatorMessages.HeaderCreated());
                            Become(BufferingData);
                            Stash.UnstashAll(); // release messages if any
                        }
                        catch (Exception e)
                        {
                            _log.Error($"Cannot write header:{h.FileHeader}", e);
                            Sender.Tell(new ValidatorMessages.CannotCreateHeader());
                        }
                    });

            Receive<FileWriterMessages.SaveWebResponse>(
                line =>
                    {
                        _gotLinesWithoutHeader = true;
                        Stash.Stash();
                    });

            Receive<FileWriterMessages.Timer>(
                t =>
                    {
                        if (_gotLinesWithoutHeader)
                        {
                            _log.Warning("Received lines and still waiting for hadeer");
                        }
                    });
        }

        /// <summary>The writing data.</summary>
        private void WritingData()
        {
            Receive<FileWriterMessages.Timer>(t => { _log.Debug("Writing data in progress"); });

            ReceiveAny(a => { Stash.Stash(); });
        }
    }
}

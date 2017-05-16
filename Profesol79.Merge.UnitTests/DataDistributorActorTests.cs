//  --------------------------------------------------------------------------------------------------------------------
// <copyright company="WPE" file="DataDistributorActorTests.cs">
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

namespace Profesor79.Merge.UnitTests
{
    using System;
    using System.Collections.Generic;

    using Akka.Actor;

    using NUnit.Framework;

    using Profesor79.Merge.ActorSystem.FileReader;
    using Profesor79.Merge.ActorSystem.RootActor;
    using Profesor79.Merge.ActorSystem.WebCrawler;

    using Should;

    /// <summary>The data distributor actor tests.</summary>
    public class DataDistributorActorTests : BaseActorTestClass
    {
        /// <summary>The when 01 receive read lines message with bad data then bad line message sent.</summary>
        [Test]
        public void When01ReceiveReadLinesMessageWithBadDataThenBadLineMessageSent()
        {
            // arrange
            _sut = Sys.ActorOf(Props.Create(() => new DataDistributorActor(_systemConfiguration)));
            _sut.Tell(new RootActorMessages.AddressBook(_actorDict));
            var line = new List<string> { "invalid line" };

            // act 
            _sut.Tell(new FileMessages.FileLines(line));

            // assert
            _testProbe.ExpectMsg<FlowControlMessages.BadLine>(TimeSpan.FromSeconds(1));
        }

        /// <summary>The when 02 receive read lines message with data then get data message sent.</summary>
        [Test]
        public void When02ReceiveReadLinesMessageWithDataThenGetDataMessageSent()
        {
            // arrange
            When01ReceiveReadLinesMessageWithBadDataThenBadLineMessageSent();
            var line = new List<string> { "918293,j;qjq,oeoeu,4/29/14   " };

            // act 
            _sut.Tell(new FileMessages.FileLines(line));

            // assert
            _testProbe.ExpectMsg<CrawlerMessages.GetData>(a => { a.MergeObject.DataId.ShouldEqual(918293); }, TimeSpan.FromSeconds(1));

            _testProbe.ExpectMsg<FlowControlMessages.ValidLine>(TimeSpan.FromSeconds(1));
        }

        /// <summary>The when 03 receive read lines with quotes message with bad data then bad line message sent.</summary>
        [Test]
        public void When03ReceiveReadLinesWithQuotesMessageWithBadDataThenBadLineMessageSent()
        {
            // arrange
            When01ReceiveReadLinesMessageWithBadDataThenBadLineMessageSent();
            var line = new List<string> { "918293,\"os,co,rp\",\"Nor, man\",4/29/1o14  " };

            // act 
            _sut.Tell(new FileMessages.FileLines(line));

            // assert

            _testProbe.ExpectMsg<FlowControlMessages.BadLine>(TimeSpan.FromSeconds(1));
        }

        /// <summary>The when 04 receive read lines with quotes message with data then get data message sent.</summary>
        [Test]
        public void When04ReceiveReadLinesWithQuotesMessageWithDataThenGetDataMessageSent()
        {
            // arrange
            When01ReceiveReadLinesMessageWithBadDataThenBadLineMessageSent();
            var line = new List<string> { "918293,\"os,co,rp\",\"Nor, man\",4/29/14   " };

            // act 
            _sut.Tell(new FileMessages.FileLines(line));

            // assert
            _testProbe.ExpectMsg<CrawlerMessages.GetData>(a => { a.MergeObject.DataId.ShouldEqual(918293); }, TimeSpan.FromSeconds(1));
            _testProbe.ExpectMsg<FlowControlMessages.ValidLine>(TimeSpan.FromSeconds(1));
        }

        /// <summary>The when 05 receive read lines with with no lines then eof message sent.</summary>
        [Test]
        public void When05ReceiveReadLinesWithWithNoLinesThenEofMessageSent()
        {
            // arrange
            When01ReceiveReadLinesMessageWithBadDataThenBadLineMessageSent();
            var line = new List<string>();

            // act 
            _sut.Tell(new FileMessages.FileLines(line));

            // assert

            _testProbe.ExpectMsg<FlowControlMessages.EoF>(TimeSpan.FromSeconds(1));
        }
    }
}

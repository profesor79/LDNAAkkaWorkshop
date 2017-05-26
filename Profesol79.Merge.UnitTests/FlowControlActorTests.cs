//  --------------------------------------------------------------------------------------------------------------------
// <copyright company="profesor79.pl" file="FlowControlActorTests.cs">
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

namespace Profesor79.Merge.UnitTests
{
    using System;

    using Akka.Actor;

    using NUnit.Framework;

    using Profesor79.Merge.ActorSystem.FileReader;
    using Profesor79.Merge.ActorSystem.FileWriter;
    using Profesor79.Merge.ActorSystem.FlowControl;
    using Profesor79.Merge.ActorSystem.RootActor;
    using Profesor79.Merge.ActorSystem.WebCrawler;

    /// <summary>The flow control actor tests.</summary>
    [TestFixture]
    public class FlowControlActorTests : BaseActorTestClass
    {
        /// <summary>The when 01 receive start processing then start processing message sent.</summary>
        [Test]
        public void When01ReceiveStartProcessingThenStartProcessingMessageSent()
        {
            // arrange
            _sut = Sys.ActorOf(Props.Create(() => new FlowControlActor(_systemConfiguration)));

            // act
            _sut.Tell(new FlowControlMessages.StartProcessing(_actorDict));

            // assert
            _testProbe.ExpectMsg<FileMessages.StartProcessing>(TimeSpan.FromSeconds(1));
        }

        /// <summary>The when 01 receive then.</summary>
        [Test]
        public void When01ReceiveTimerThenNoMessageSent()
        {
            // arrange
            _sut = Sys.ActorOf(Props.Create(() => new FlowControlActor(_systemConfiguration)));

            // act
            _sut.Tell(new FlowControlMessages.Timer());

            // assert
            NoMessageSent();
        }

        /// <summary>The when 02 receive api state of fline then fatall error message sent.</summary>
        [Test]
        public void When02ReceiveApiStateOFFlineThenFatallErrorMessageSent()
        {
            // arrange -> set processing state
            When01ReceiveStartProcessingThenStartProcessingMessageSent();

            // act
            for (var i = 0; i < 4; i++)
            {
                _sut.Tell(new FlowControlMessages.ApiState(false));
            }

            // assert
            _testProbe.ExpectMsg<RootActorMessages.FatalError>(TimeSpan.FromSeconds(1));
        }

        /// <summary>The when 02 receive api state online then no message sent.</summary>
        [Test]
        public void When02ReceiveApiStateOnlineThenNoMessageSent()
        {
            // arrange -> set processing state
            When01ReceiveStartProcessingThenStartProcessingMessageSent();

            // act
            _sut.Tell(new FlowControlMessages.ApiState(true));

            // assert
            NoMessageSent();
        }

        /// <summary>The when 02 receive bad api responses and no good ones then fatall error message sent.</summary>
        [Test]
        public void When02ReceiveBadApiResponsesAndNoGoodOnesThenFatallErrorMessageSent()
        {
            // arrange -> set processing state
            When01ReceiveStartProcessingThenStartProcessingMessageSent();

            // act
            for (var i = 0; i < 4; i++)
            {
                _sut.Tell(new FlowControlMessages.WebApiGotBadResponse());
            }

            _sut.Tell(new FlowControlMessages.Timer());

            // assert
            _testProbe.ExpectMsg<RootActorMessages.FatalError>(TimeSpan.FromSeconds(1));
        }

        /// <summary>The when 02 receive bad api responses and one good one then no message sent.</summary>
        [Test]
        public void When02ReceiveBadApiResponsesAndOneGoodOneThenNoMessageSent()
        {
            // arrange -> set processing state
            When01ReceiveStartProcessingThenStartProcessingMessageSent();

            // act
            for (var i = 0; i < 4; i++)
            {
                _sut.Tell(new FlowControlMessages.WebApiGotBadResponse());
            }

            _sut.Tell(new FlowControlMessages.WebApiGotValidResponse());
            _sut.Tell(new FlowControlMessages.Timer());

            // assert
            NoMessageSent();
        }

        /// <summary>The when 02 receive get new lines for crawler then get next chunk message sent.</summary>
        [Test]
        public void When02ReceiveGetNewLinesForCrawlerThenGetNextChunkMessageSent()
        {
            // arrange -> set processing state
            When01ReceiveStartProcessingThenStartProcessingMessageSent();

            // act
            _sut.Tell(new FlowControlMessages.GetNewLinesForCrawler());

            // assert
            NoMessageSent();

            // as we cannot set actor name this test is abit tricky to pass
            // _testProbe.ExpectMsg<FileMessages.GetNextChunk>(TimeSpan.FromSeconds(1));
        }

        /// <summary>The when 02 receive get new lines for crawler twice then get next chunk only one message sent.</summary>
        [Test]
        public void When02ReceiveGetNewLinesForCrawlerTwiceThenGetNextChunkOnlyOneMessageSent()
        {
            // arrange -> set processing state
            When02ReceiveGetNewLinesForCrawlerThenGetNextChunkMessageSent();

            // act
            _sut.Tell(new FlowControlMessages.GetNewLinesForCrawler());

            // assert
            NoMessageSent(10);
        }

        /// <summary>The when 02 receive timer and eof but not all lines are saved then no message sent.</summary>
        [Test]
        public void When02ReceiveTimerAndEOFButNotAllLinesAreSavedThenNoMessageSent()
        {
            // arrange -> set processing state
            When01ReceiveStartProcessingThenStartProcessingMessageSent();
            _sut.Tell(new FlowControlMessages.LinesSaved(5));
            _sut.Tell(new FlowControlMessages.EoF());

            // act
            _sut.Tell(new FlowControlMessages.Timer());

            // assert
            NoMessageSent();
        }

        /// <summary>The when 02 receive timer and eof then process finished message sent.</summary>
        [Test]
        public void When02ReceiveTimerAndEOFThenProcessFinishedMessageSent()
        {
            // arrange -> set processing state
            When01ReceiveStartProcessingThenStartProcessingMessageSent();
            _sut.Tell(new FlowControlMessages.EoF());

            // act
            _sut.Tell(new FlowControlMessages.Timer());

            // assert
            _testProbe.ExpectMsg<RootActorMessages.ProcessFinished>(TimeSpan.FromSeconds(.1));
            _testProbe.ExpectMsg<FileWriterMessages.CloseFile>(TimeSpan.FromSeconds(.1));
        }

        /// <summary>The when 02 receive timer then no message sent.</summary>
        [Test]
        public void When02ReceiveTimerThenNoMessageSent()
        {
            // arrange -> set processing state
            When01ReceiveStartProcessingThenStartProcessingMessageSent();

            // act
            _sut.Tell(new FlowControlMessages.Timer());

            // assert
            NoMessageSent();
        }

        /*
         * to do 
         * test actor by sending
         * lines read 
         * and api responses bad/good
         * asd lines written      
         
         
         */
    }
}

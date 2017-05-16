//  --------------------------------------------------------------------------------------------------------------------
// <copyright company="WPE" file="DataDispatcherActorTests.cs">
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

    using Akka.Actor;

    using NUnit.Framework;

    using Profesor79.Merge.ActorSystem.FileReader;
    using Profesor79.Merge.ActorSystem.RootActor;
    using Profesor79.Merge.ActorSystem.WebCrawler;

    /// <summary>The data dispatcher actor tests.</summary>
    [TestFixture]
    public class DataDispatcherActorTests : BaseActorTestClass
    {
        /// <summary>The when receive cannot read file then fatal error message sent.</summary>
        [Test]
        public void WhenReceiveCannotReadFileThenFatalErrorMessageSent()
        {
            // arrange
            WhenReceiveStartProcessingThenReadLinesMessageSent();

            // act
            _sut.Tell(new FileMessages.CannotReadFile());

            // assert
            _testProbe.ExpectMsg<RootActorMessages.FatalError>(TimeSpan.FromSeconds(100));
        }

        /// <summary>The when receive end of file then flow control messages eo f message sent.</summary>
        [Test]
        public void WhenReceiveEndOfFileThenFlowControlMessagesEoFMessageSent()
        {
            // arrange
            WhenReceiveStartProcessingThenReadLinesMessageSent();

            // act
            _sut.Tell(new FileMessages.EndOfFile());

            // assert
            _testProbe.ExpectMsg<FlowControlMessages.EoF>(TimeSpan.FromSeconds(100));
        }

        /// <summary>The when receive get next chunk then read lines message sent.</summary>
        [Test]
        public void WhenReceiveGetNextChunkThenReadLinesMessageSent()
        {
            // arrange
            WhenReceiveStartProcessingThenReadLinesMessageSent();

            // act
            _sut.Tell(new FileMessages.GetNextChunk());

            // assert
            _testProbe.ExpectMsg<FileMessages.ReadLines>(TimeSpan.FromSeconds(1));
        }

        /// <summary>The when receive start processing then read lines message sent.</summary>
        [Test]
        public void WhenReceiveStartProcessingThenReadLinesMessageSent()
        {
            // arrange
            _sut = Sys.ActorOf(Props.Create(() => new DataDispatcherActor(_systemConfiguration)));

            // act
            _sut.Tell(new FileMessages.StartProcessing(_actorDict));

            // assert
            _testProbe.ExpectMsg<FileMessages.ReadLines>(TimeSpan.FromSeconds(1));
        }
    }
}

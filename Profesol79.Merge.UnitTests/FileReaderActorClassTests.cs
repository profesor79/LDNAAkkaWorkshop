//  --------------------------------------------------------------------------------------------------------------------
// <copyright company="WPE" file="FileReaderActorClassTests.cs">
// Copyright (c) 2017 All Right Reserved
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
// </copyright>
// <summary>
// Created: 2017-04-16, 10:21 PM
// Last changed by: profesor79, 2017-04-27, 4:04 PM 
// </summary>
//   --------------------------------------------------------------------------------------------------------------------

namespace Profesor79.Merge.UnitTests
{
    using System;

    using Akka.Actor;

    using NUnit.Framework;

    using Should;

    using Profesor79.Merge.ActorSystem.FileReader;
    using Profesor79.Merge.ActorSystem.RootActor;
    using Profesor79.Merge.UnitTests.Mocks;

    /// <summary>The file reader actor class tests.</summary>
    [TestFixture]
    public class FileReaderActorClassTests : BaseActorTestClass
    {
        /// <summary>The when file header corrupted then error message sent.</summary>
        [Test]
        public void When01ReceiveCheckFileMessageAndGoodFileNameThenFileValidatedWithOKMessageSent()
        {
            // arrange
            _sut = Sys.ActorOf(Props.Create(() => new FileReaderActor(new FileReaderMock(), _systemConfiguration)));

            // act
            _sut.Tell(new FileMessages.CheckInputFile("ok", _actorDict, _systemConfiguration.HeaderValidationRegex));

            // assert
            ExpectMsg<FileMessages.FileValidated>(message => { message.CanBeRead.ShouldBeTrue(); }, TimeSpan.FromSeconds(1));
        }

        /// <summary>The when 02 receive check file message and bad file name then file validated with nok message sent.</summary>
        [Test]
        public void When02ReceiveCheckFileMessageAndBadFileNameThenFileValidatedWithNokMessageSent()
        {
            // arrange
            _sut = Sys.ActorOf(Props.Create(() => new FileReaderActor(new FileReaderMock(), _systemConfiguration)));

            // act
            _sut.Tell(new FileMessages.CheckInputFile("nok", _actorDict, _systemConfiguration.HeaderValidationRegex));

            // assert
            ExpectMsg<FileMessages.FileValidated>(message => { message.CanBeRead.ShouldBeFalse(); }, TimeSpan.FromSeconds(1));
        }

        /// <summary>The when 03 receive read first line and file gone then cannot read file is sent.</summary>
        [Test]
        public void When03ReceiveReadFirstLineAndFileGoneThenCannotReadFileIsSent()
        {
            // arrange
            _sut = Sys.ActorOf(Props.Create(() => new FileReaderActor(new FileReaderMock(), _systemConfiguration)));
            _sut.Tell(new FileMessages.CheckInputFile("ok,headerThrows", _actorDict, _systemConfiguration.HeaderValidationRegex));
            ExpectMsg<FileMessages.FileValidated>(TimeSpan.FromSeconds(1));

            // act
            _sut.Tell(new FileMessages.ReadHeader());

            // assert
            ExpectMsg<FileMessages.CannotReadFile>(TimeSpan.FromSeconds(1));
        }

        /// <summary>The when 04 receive read first line then header is sent.</summary>
        [Test]
        public void When04ReceiveReadFirstLineThenHeaderIsSent()
        {
            // arrange
            var fileName = "ok,headerOk";
            _sut = Sys.ActorOf(Props.Create(() => new FileReaderActor(new FileReaderMock(), _systemConfiguration)));
            _sut.Tell(new FileMessages.CheckInputFile(fileName, _actorDict, _systemConfiguration.HeaderValidationRegex));

            ExpectMsg<FileMessages.FileValidated>(TimeSpan.FromSeconds(1));

            // act
            _sut.Tell(new FileMessages.ReadHeader());

            // assert
            ExpectMsg<FileMessages.FileHeader>(m => { m.Header.ShouldEqual(fileName); }, TimeSpan.FromSeconds(1));
        }

        /// <summary>The when 05 receive read lines and file is gone then cannot read file is sent.</summary>
        [Test]
        public void When05ReceiveReadLinesAndFileIsGoneThenCannotReadFileIsSent()
        {
            // arrange
            _systemConfiguration.InternalChunkSize = 0;
            SetReadingState();

            // act
            _sut.Tell(new FileMessages.ReadLines(500));

            // assert
            ExpectMsg<FileMessages.CannotReadFile>(TimeSpan.FromSeconds(100));
        }

        /// <summary>The when 06 receive read lines and file is empty then end of file is sent.</summary>
        [Test]
        public void When06ReceiveReadLinesAndFileIsEmptyThenEndOfFileIsSent()
        {
            // arrange
            _systemConfiguration.InternalChunkSize = 1;
            SetReadingState();

            // act
            _sut.Tell(new FileMessages.ReadLines(1001));

            // assert
            ExpectMsg<FileMessages.EndOfFile>(TimeSpan.FromSeconds(1));
        }

        /// <summary>The when 07 receive read lines then message with lines is sent.</summary>
        [Test]
        public void When07ReceiveReadLinesThenMessageWithLinesIsSent()
        {
            // arrange
            _systemConfiguration.InternalChunkSize = 3;
            SetReadingState();

            // act
            _sut.Tell(new FileMessages.ReadLines(2));

            // assert
            _testProbe.ExpectMsg<FileMessages.FileLines>(
                m =>
                    {
                        m.Lines[0].ShouldEqual("1");
                        m.Lines[1].ShouldEqual("2");
                        m.Lines[2].ShouldEqual("3");
                    },
                TimeSpan.FromSeconds(1));
        }

        /// <summary>The set reading state.</summary>
        private void SetReadingState()
        {
            var fileName = "ok,headerOk";
            _sut = Sys.ActorOf(Props.Create(() => new FileReaderActor(new FileReaderMock(), _systemConfiguration)));
            _sut.Tell(new RootActorMessages.AddressBook(_actorDict));
            _sut.Tell(new FileMessages.CheckInputFile(fileName, _actorDict, _systemConfiguration.HeaderValidationRegex));
            ExpectMsg<FileMessages.FileValidated>(TimeSpan.FromSeconds(1));
            _sut.Tell(new FileMessages.ReadHeader());
            ExpectMsg<FileMessages.FileHeader>(m => { m.Header.ShouldEqual(fileName); }, TimeSpan.FromSeconds(1));
        }
    }
}

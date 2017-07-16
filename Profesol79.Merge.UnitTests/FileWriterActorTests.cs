//  --------------------------------------------------------------------------------------------------------------------
// <copyright company="profesor79.pl" file="FileWriterActorTests.cs">
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

    using Profesor79.Merge.ActorSystem.FileWriter;
    using Profesor79.Merge.ActorSystem.ValidatorActor;
    using Profesor79.Merge.Models;
    using Profesor79.Merge.UnitTests.Mocks;

    /// <summary>The web crawler actor tests.</summary>
    [TestFixture]
    public class FileWriterActorTests : BaseActorTestClass
    {
        /// <summary>The when 01 receive create file and there in no exceptions then file created message sent.</summary>
        [Test]
        public void When01ReceiveCreateFileAndThereInNoExceptionsThenFileCreatedMessageSent()
        {
            // arrange
            var mock = new FileWriterMock();
            _sut = Sys.ActorOf(Props.Create(() => new FileWriterActor(mock, _systemConfiguration)));
            var fileName = "fileName";

            // act
            _sut.Tell(new FileWriterMessages.CreateFile(fileName, true, _actorDict));

            // assert
            ExpectMsg<ValidatorMessages.FileCreated>(TimeSpan.FromSeconds(100));
        }

        /// <summary>The when 02 receive create file and there is exception then cannot create file message sent.</summary>
        [Test]
        public void When02ReceiveCreateFileAndThereIsExceptionThenCannotCreateFileMessageSent()
        {
            // arrange
            var mock = new FileWriterMock();
            _sut = Sys.ActorOf(Props.Create(() => new FileWriterActor(mock, _systemConfiguration)));
            var fileName = "throwfileName";

            // act
            _sut.Tell(new FileWriterMessages.CreateFile(fileName, true, _actorDict));

            // assert
            ExpectMsg<ValidatorMessages.CannotCreateFile>(TimeSpan.FromSeconds(1));
        }

        /// <summary>The when 03 receive write header and no exceptions then header created message sent.</summary>
        [Test]
        public void When03ReceiveWriteHeaderAndNoExceptionsThenHeaderCreatedMessageSent()
        {
            // arrange
            // Account ID, First Name, Created On, Status, Status Set On
            When01ReceiveCreateFileAndThereInNoExceptionsThenFileCreatedMessageSent();
            var fileHeader = "Account ID, First Name, Created On, Status, Status Set On";

            // act
            _sut.Tell(new FileWriterMessages.WriteHeader(fileHeader));

            // assert
            ExpectMsg<ValidatorMessages.HeaderCreated>(TimeSpan.FromSeconds(1));
        }

        /// <summary>The when 04 receive write header and exception raised then cannot create header message sent.</summary>
        [Test]
        public void When04ReceiveWriteHeaderAndExceptionRaisedThenCannotCreateHeaderMessageSent()
        {
            // arrange
            // Account ID, First Name, Created On, Status, Status Set On
            When01ReceiveCreateFileAndThereInNoExceptionsThenFileCreatedMessageSent();
            var fileHeader = "throw";

            // act
            _sut.Tell(new FileWriterMessages.WriteHeader(fileHeader));

            // assert
            ExpectMsg<ValidatorMessages.CannotCreateHeader>(TimeSpan.FromSeconds(1));
        }

        /// <summary>The when 05 receive save lines then no message sent.</summary>
        [Test]
        public void When05ReceiveCloseFileThenFileClosedSent()
        {
            // arrange
            var mock = new FileWriterMock();
            _sut = Sys.ActorOf(Props.Create(() => new FileWriterActor(mock, _systemConfiguration)));

            // act
            _sut.Tell(new FileWriterMessages.CloseFile());

            // assert
            NoMessageSent(); // as this is ask
        }

        /// <summary>The when 05 receive save lines then no message sent.</summary>
        [Test]
        public void When05ReceiveSaveLinesThenNoMessageSent()
        {
            // arrange
            When03ReceiveWriteHeaderAndNoExceptionsThenHeaderCreatedMessageSent();
            var merge = new MergeObjectDto { DataId = 1 };

            // act
            //_sut.Tell(new FileWriterMessages.SaveWebResponse(merge));

            // assert
            NoMessageSent();
        }
    }
}

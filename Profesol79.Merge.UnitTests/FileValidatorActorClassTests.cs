//  --------------------------------------------------------------------------------------------------------------------
// <copyright company="WPE" file="FileValidatorActorClassTests.cs">
// Copyright (c) 2017 All Right Reserved
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
// </copyright>
// <summary>
// Created: 2017-05-15, 2:37 PM
// Last changed by: A happy WPE candidate, 2017-05-16, 10:48 AM 
// </summary>
//   --------------------------------------------------------------------------------------------------------------------

namespace Profesor79.Merge.UnitTests
{
    using System;

    using Akka.Actor;

    using NUnit.Framework;

    using Profesor79.Merge.ActorSystem.FileReader;
    using Profesor79.Merge.ActorSystem.RootActor;

    /// <summary>The file validator actor class tests.</summary>
    [TestFixture]
    public class FileValidatorActorClassTests : BaseActorTestClass
    {
        /// <summary>The _regex pattern.</summary>
        private const string _regexPattern = @"^Account ID\,Account Name\,First Name\,Created On$";

        /// <summary>The _invalid header 1.</summary>
        private readonly string _invalidHeader1 = "Account ID,Account Name,First Name,Created on";

        /// <summary>The _invalid header 2.</summary>
        private readonly string _invalidHeader2 = "Account ICreated on";

        /// <summary>The _invalid header 3.</summary>
        private readonly string _invalidHeader3 = "Account ID.Account Name.First Name.Created on";

        /// <summary>The _invalid header 4.</summary>
        private readonly string _invalidHeader4 = string.Empty;

        /// <summary>The _valid header.</summary>
        private readonly string _validHeader = "Account ID,Account Name,First Name,Created On";

        /// <summary>The when 01 receive request then check file message is sent with invalid name.</summary>
        [Test]
        public void When01ReceiveRequestThenCheckFileMessageIsSentWithInvalidName()
        {
            // arrange
            _sut = Sys.ActorOf(Props.Create(() => new FileValidatorActor()));

            // act
            _sut.Tell(new FileMessages.CheckInputFile("ok", _actorDict, _systemConfiguration.HeaderValidationRegex));

            // assert
            _testProbe.ExpectMsg<FileMessages.CheckInputFile>();
        }

        /// <summary>The when 02 get negative response from reader actor.</summary>
        [Test]
        public void When02GetNegativeResponseFromReaderActor()
        {
            // arrange
            // as we are changing states we need to folow 
            // message exchange pattern used to set "become" state
            When01ReceiveRequestThenCheckFileMessageIsSentWithInvalidName();

            // act
            _sut.Tell(new FileMessages.FileValidated("none", false));

            // assert
            _testProbe.ExpectMsg<RootActorMessages.FatalError>(TimeSpan.FromSeconds(1));
        }

        /// <summary>The when 03 valid file name sent then verify header.</summary>
        [Test]
        public void When03ValidFileNameSentThenVerifyHeader()
        {
            // arrange
            // as we are changing states we need to folow 
            // message exchange pattern used to set "become" state
            _sut = Sys.ActorOf(Props.Create(() => new FileValidatorActor()));

            _sut.Tell(new FileMessages.CheckInputFile("ok,headerThrows", _actorDict, _systemConfiguration.HeaderValidationRegex));
            _testProbe.ExpectMsg<FileMessages.CheckInputFile>();

            // act
            _sut.Tell(new FileMessages.FileValidated("ok", true));

            // assert
            _testProbe.ExpectMsg<FileMessages.ReadHeader>(TimeSpan.FromSeconds(1));
        }

        /// <summary>The when 04 valid file name invalid header then cannot read input file message sent.</summary>
        [Test]
        public void When04ValidFileNameInvalidHeader1ThenCannotReadInputFileMessageSent()
        {
            // arrange
            When03ValidFileNameSentThenVerifyHeader();

            // act
            _sut.Tell(new FileMessages.FileHeader(_invalidHeader1));

            // assert
            _testProbe.ExpectMsg<RootActorMessages.FatalError>(TimeSpan.FromSeconds(100));
        }

        /// <summary>The when 04 valid file name invalid header then cannot read input file message sent.</summary>
        [Test]
        public void When04ValidFileNameInvalidHeader2ThenCannotReadInputFileMessageSent()
        {
            // arrange
            When03ValidFileNameSentThenVerifyHeader();

            // act
            _sut.Tell(new FileMessages.FileHeader(_invalidHeader2));

            // assert
            _testProbe.ExpectMsg<RootActorMessages.FatalError>(TimeSpan.FromSeconds(1));
        }

        /// <summary>The when 04 valid file name invalid header then cannot read input file message sent.</summary>
        [Test]
        public void When04ValidFileNameInvalidHeader3ThenCannotReadInputFileMessageSent()
        {
            // arrange
            When03ValidFileNameSentThenVerifyHeader();

            // act
            _sut.Tell(new FileMessages.FileHeader(_invalidHeader3));

            // assert
            _testProbe.ExpectMsg<RootActorMessages.FatalError>(TimeSpan.FromSeconds(1));
        }

        /// <summary>The when 04 valid file name invalid header then cannot read input file message sent.</summary>
        [Test]
        public void When04ValidFileNameInvalidHeader4ThenCannotReadInputFileMessageSent()
        {
            // arrange
            When03ValidFileNameSentThenVerifyHeader();

            // act
            _sut.Tell(new FileMessages.FileHeader(_invalidHeader4));

            // assert
            _testProbe.ExpectMsg<RootActorMessages.FatalError>(TimeSpan.FromSeconds(1));
        }
    }
}

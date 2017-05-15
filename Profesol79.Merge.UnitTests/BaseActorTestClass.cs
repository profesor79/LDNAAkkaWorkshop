//  --------------------------------------------------------------------------------------------------------------------
// <copyright company="WPE" file="BaseActorTestClass.cs">
// Copyright (c) 2017 All Right Reserved
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
// </copyright>
// <summary>
// Created: 2017-04-15, 10:46 PM
// Last changed by: profesor79, 2017-04-27, 4:04 PM 
// </summary>
//   --------------------------------------------------------------------------------------------------------------------

namespace Profesor79.Merge.UnitTests
{
    using System;
    using System.Collections.Generic;

    using Akka.Actor;
    using Akka.TestKit;
    using Akka.TestKit.Xunit;

    using NUnit.Framework;

    using Profesor79.Merge.Contracts;
    using Profesor79.Merge.UnitTests.Mocks;

    /// <summary>The base actor test class.</summary>
    public class BaseActorTestClass : TestKit
    {
        /// <summary>The _actor dict.</summary>
        internal Dictionary<string, IActorRef> _actorDict;

        /// <summary>The _sut.</summary>
        internal IActorRef _sut;

        /// <summary>The _system configuration.</summary>
        internal ISystemConfiguration _systemConfiguration = new SystemTestingConfiguration();

        /// <summary>The _test probe.</summary>
        internal TestProbe _testProbe;

        /// <summary>The no message sent.</summary>
        /// <param name="i">The i.</param>
        public void NoMessageSent(int i = 100)
        {
            ExpectNoMsg(TimeSpan.FromMilliseconds(i));
            _testProbe.ExpectNoMsg(TimeSpan.FromMilliseconds(i));
        }

        /// <summary>The setup.</summary>
        [SetUp]
        public void Setup()
        {
            _testProbe = CreateTestProbe("testProbe");

            _actorDict = new Dictionary<string, IActorRef>
                             {
                                 { "root", _testProbe },
                                 { "FileValidatorActor", _testProbe },
                                 { "FileReaderActor", _testProbe },
                                 { "WebCrawlerActor", _testProbe },
                                 { "FileWriterActor", _testProbe },
                                 { "FlowControlActor", _testProbe },
                                 { "DataDispatcherActor", _testProbe },
                                 { "DataDistributorActor", _testProbe }
                             };
        }

        /// <summary>The tear down.</summary>
        [TearDown]
        public void TearDown()
        {
            _testProbe.Tell(PoisonPill.Instance);
            _actorDict = null;
        }
    }
}

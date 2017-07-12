//  --------------------------------------------------------------------------------------------------------------------
// <copyright company="profesor79.pl" file="WebCrawlerActorTests.cs">
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
    using System.Net;

    using Akka.Actor;

    using NUnit.Framework;

    using Profesor79.Merge.ActorSystem.FileWriter;
    using Profesor79.Merge.ActorSystem.RootActor;
    using Profesor79.Merge.ActorSystem.WebCrawler;
    using Profesor79.Merge.Models;

    using RichardSzalay.MockHttp;

    using Should;

    /// <summary>The web crawler actor tests.</summary>
    [TestFixture]
    public class WebCrawlerActorTests : BaseActorTestClass
    {
        /// <summary>The when 01 receive get data then response message sent.</summary>
        [Test]
        public void When01ReceiveGetDataThenResponseMessageSent()
        {
            // arrange
            SetupValidResponse();
            var mergeDto = new MergeObjectDto { DataId = 1 };

            // act
            _sut.Tell(new CrawlerMessages.GetData(mergeDto, ""));

            // assert
            _testProbe.ExpectMsg<FlowControlMessages.WebApiGotValidResponse>(TimeSpan.FromSeconds(1));
            _testProbe.ExpectMsg<FileWriterMessages.SaveWebResponse>(a => { a.MergeObjectDto.DataId.ShouldEqual(1); }, TimeSpan.FromSeconds(1));
        }

        /// <summary>The when 02 receive get data and time out then response message sent.</summary>
        [Test]
        public void When02ReceiveGetDataAndNo200BackThenBadResponseMessageSent()
        {
            // arrange
            SetupInvaldResponse();

            var mergeDto = new MergeObjectDto { DataId = 1 };

            // act
            _sut.Tell(new CrawlerMessages.GetData(mergeDto, ""));

            // assert
            _testProbe.ExpectMsg<FileWriterMessages.SaveWebResponse>(TimeSpan.FromSeconds(1));
            _testProbe.ExpectMsg<FlowControlMessages.WebApiGotBadResponse>(TimeSpan.FromSeconds(1));
        }

        /// <summary>The web crawler actor tests.</summary>
        /// <summary>The when 01 receive get data then response message sent.</summary>
        [Test]
        public void When03ReceiveWebApiErrorResponseAndGetValidResponseThenResponseMessageSent()
        {
            // arrange
            SetupValidResponse();
            var mergeDto = new MergeObjectDto { DataId = 1 };
            var message = new CrawlerMessages.WebApiErrorResponse(1, mergeDto, $"{_systemConfiguration.ApiEndPoint}/1", 1);


            // act
            _sut.Tell(message);

            // assert
            _testProbe.ExpectMsg<FlowControlMessages.WebApiGotValidResponse>(TimeSpan.FromSeconds(1));
            _testProbe.ExpectMsg<FileWriterMessages.SaveWebResponse>(a => { a.MergeObjectDto.DataId.ShouldEqual(1); }, TimeSpan.FromSeconds(1));
        }

        /// <summary>The when 02 receive get data and time out then response message sent.</summary>
        [Test]
        public void When04ReceiveWebApiErrorResponseAndNo200BackThenBadResponseMessageSent()
        {
            // arrange
            SetupInvaldResponse();
            var mergeDto = new MergeObjectDto { DataId = 1 };
            var message = new CrawlerMessages.WebApiErrorResponse(1, mergeDto, $"{_systemConfiguration.ApiEndPoint}/1", 1);

            // act
            _sut.Tell(message);

            // assert
            _testProbe.ExpectMsg<FileWriterMessages.SaveWebResponse>(TimeSpan.FromSeconds(1));
            _testProbe.ExpectMsg<FlowControlMessages.WebApiGotBadResponse>(TimeSpan.FromSeconds(1));
        }

        /// <summary>The setup invald response.</summary>
        private void SetupInvaldResponse()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.Expect("/http://localhost/api/1").Respond(HttpStatusCode.Unauthorized);

            // Inject the handler or client into your application code
            var client = mockHttp.ToHttpClient();

            _systemConfiguration.ApiEndPoint = "http://localhost/api";
            _sut = Sys.ActorOf(Props.Create(() => new WebCrawlerActor(_systemConfiguration, _testProbe)));
            _sut.Tell(new RootActorMessages.AddressBook(_actorDict));
        }

        /// <summary>The setup valid response.</summary>
        private void SetupValidResponse()
        {
            var mockHttp = new MockHttpMessageHandler();

            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When("http://localhost/api/*")
                .Respond("application/json", "{\"account_id\": 1, \"status\": \"good\", \"created_on\": \"2011-01-12\"}");

            // Respond with JSON
            // Inject the handler or client into your application code
            var client = mockHttp.ToHttpClient();

            _systemConfiguration.ApiEndPoint = "http://localhost/api";
            _sut = Sys.ActorOf(Props.Create(() => new WebCrawlerActor(_systemConfiguration, _testProbe)));
            _sut.Tell(new RootActorMessages.AddressBook(_actorDict));
        }
    }
}

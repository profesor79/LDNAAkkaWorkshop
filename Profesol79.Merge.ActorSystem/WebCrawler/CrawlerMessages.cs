//  --------------------------------------------------------------------------------------------------------------------
// <copyright company="profesor79.pl" file="CrawlerMessages.cs">
// Copyright (c) 2017 All Right Reserved
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
// </copyright>
// <summary>
// Created: 2017-05-15, 2:37 PM
// Last changed by: profesor79, 2017-05-26, 8:20 AM 
// </summary>
//   --------------------------------------------------------------------------------------------------------------------

namespace Profesor79.Merge.ActorSystem.WebCrawler
{
    using System;

    using Profesor79.Merge.Models;

    /// <summary>The crawler messages.</summary>
    public class CrawlerMessages
    {
        /// <summary>The check endpoint.</summary>
        public class CheckEndpoint
        {
        }

        /// <summary>The get data.</summary>
        public class GetData
        {
            /// <summary>Initializes a new instance of the <see cref="GetData"/> class.</summary>
            /// <param name="mergeObject">The merge object.</param>
            /// <param name = "apiEndPoint" ></param>
            /// <param name = "utcNow" ></param>
            public GetData(MergeObjectDto mergeObject, string apiEndPoint, DateTime utcNow)
            {
                MergeObject = mergeObject;
                ApiEndPoint = apiEndPoint;
                UtcNow = utcNow;
            }

            /// <summary>Gets the merge object.</summary>
            public MergeObjectDto MergeObject { get; }

            public string ApiEndPoint { get; }

            public DateTime UtcNow { get; }
        }

        /// <summary>The piped request.</summary>
        public class PipedRequest
        {
            /// <summary>Initializes a new instance of the <see cref="PipedRequest"/> class.</summary>
            /// <param name="requestResult">The request result.</param>
            /// <param name="mergeObject">The merge object.</param>
            /// <param name = "messageId" ></param>
            public PipedRequest(WebApiResponseDto requestResult, MergeObjectDto mergeObject, long messageId)
            {
                RequestResult = requestResult;
                MergeObject = mergeObject;
                MessageId = messageId;
            }

            /// <summary>Gets the merge object.</summary>
            public MergeObjectDto MergeObject { get; }

            public long MessageId { get; }

            /// <summary>Gets the request result.</summary>
            public WebApiResponseDto RequestResult { get; }
        }

        /// <summary>The start timer.</summary>
        public class StartChecking
        {
        }

        /// <summary>The web api error response.</summary>
        public class WebApiErrorResponse
        {
            public int Attempt { get; }

            public MergeObjectDto MergeObjectDto { get; }

            public string Url { get; }

            public long MessageId { get; }

            public WebApiErrorResponse(int attempt, MergeObjectDto mergeObjectDto, string url, long messageId)
            {
                Attempt = attempt;
                MergeObjectDto = mergeObjectDto;
                Url = url;
                MessageId = messageId;
            }
        }

        /// <summary>The timer.</summary>
        internal class Timer
        {

        }
    }
}

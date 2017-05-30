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
            public GetData(MergeObjectDto mergeObject, string apiEndPoint)
            {
                MergeObject = mergeObject;
                ApiEndPoint = apiEndPoint;
            }

            /// <summary>Gets the merge object.</summary>
            public MergeObjectDto MergeObject { get; }

            public string ApiEndPoint { get; }
        }

        /// <summary>The piped request.</summary>
        public class PipedRequest
        {
            /// <summary>Initializes a new instance of the <see cref="PipedRequest"/> class.</summary>
            /// <param name="requestResult">The request result.</param>
            /// <param name="mergeObject">The merge object.</param>
            public PipedRequest(WebApiResponseDto requestResult, MergeObjectDto mergeObject)
            {
                RequestResult = requestResult;
                MergeObject = mergeObject;
            }

            /// <summary>Gets the merge object.</summary>
            public MergeObjectDto MergeObject { get; }

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
            /// <summary>Gets or sets the attempt.</summary>
            public int attempt { get; set; }

            /// <summary>Gets or sets the merge object dto.</summary>
            public MergeObjectDto MergeObjectDto { get; set; }

            /// <summary>Gets or sets the url.</summary>
            public string url { get; set; }
        }

        /// <summary>The timer.</summary>
        internal class Timer
        {
            /// <summary>Initializes a new instance of the <see cref="Timer"/> class.</summary>
            public Timer() { }
        }
    }
}

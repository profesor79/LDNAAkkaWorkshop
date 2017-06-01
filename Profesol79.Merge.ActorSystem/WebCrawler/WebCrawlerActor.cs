//  --------------------------------------------------------------------------------------------------------------------
// <copyright company="profesor79.pl" file="WebCrawlerActor.cs">
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
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;

    using Akka.Actor;

    using Profesor79.Merge.ActorSystem.BaseObjects;
    using Profesor79.Merge.ActorSystem.FileWriter;
    using Profesor79.Merge.ActorSystem.RootActor;
    using Profesor79.Merge.Contracts;
    using Profesor79.Merge.Models;

    /// <summary>The web crawler actor.</summary>
    public class WebCrawlerActor : BaseActorClass, IWithUnboundedStash
    {
        /// <summary>The _client.</summary>
        private readonly HttpClient _client;

        /// <summary>The _error status.</summary>
        private readonly string _errorStatus = "----ErrorGettingData----";

        /// <summary>The _http timeout in miniseconds.</summary>
        private readonly uint _httpTimeoutInMiniseconds;

        /// <summary>The _http wrapper.</summary>
        // private readonly IHttpWrapper _httpWrapper;
        /// <summary>The _system configuration.</summary>
        private readonly ISystemConfiguration _systemConfiguration;

        private readonly IActorRef _root;

        /// <summary>The _cancel timer.</summary>
        private ICancelable _cancelTimer;

        /// <summary>The _got book.</summary>
        private bool _gotBook;

        /// <summary>The _last activity.</summary>
        private DateTime _lastActivity;

        private ulong _processed;

        /// <summary>Initializes a new instance of the <see cref="WebCrawlerActor"/> class.</summary>
        /// <param name="systemConfiguration">The system configuration.</param>
        /// <param name="client"></param>
        public WebCrawlerActor(ISystemConfiguration systemConfiguration, IActorRef root)
        {
            _systemConfiguration = systemConfiguration;
            _root = root;
            _client = new HttpClient();
            _httpTimeoutInMiniseconds = systemConfiguration.HttpTimeoutInMiniseconds;

            SetupHttpClient();

            Receive<RootActorMessages.AddressBook>(
                b =>
                    {
                        _actorDictionary = b.ActorDictionary;
                        _gotBook = true;
                        Stash.UnstashAll();
                    });

            Receive<CrawlerMessages.GetData>(
                m =>
                    {
                        if (_gotBook)
                        {
                            ProcessGetMessage(m);
                            _lastActivity = DateTime.Now;
                        }
                        else
                        {
                            Stash.Stash();
                        }
                    });

            Receive<CrawlerMessages.WebApiErrorResponse>(a => { ProcessWebErrorMessage(a); });

            Receive<CrawlerMessages.PipedRequest>(o => { ProcessPipedRequest(o); });

            Receive<CrawlerMessages.Timer>(
                t =>
                    {
                        if (_gotBook)
                        {
                            ProcessTimer();
                        }
                        else
                        {
                            Stash.Stash();
                        }


                    });
        }

        /// <summary>Gets or sets the stash.</summary>
        public IStash Stash { get; set; }

        protected override void PostStop()
        {
            _cancelTimer?.Cancel();
            base.PostStop();
        }

        /// <summary>The pre start.</summary>
        protected override void PreStart()
        {

            _cancelTimer = new Cancelable(Context.System.Scheduler);

            Context.System.Scheduler.ScheduleTellRepeatedly(
                TimeSpan.FromSeconds(2),
                TimeSpan.FromSeconds(0.5),
                Self,
                new CrawlerMessages.Timer(),
                Self,
                _cancelTimer);


            _root.Tell(new RootActorMessages.AddressBookRequest());
            _lastActivity = DateTime.Now;
            base.PreStart();
        }


        /// <summary>The process get message.</summary>
        /// <param name="getDataMessage">The m.</param>
        private void ProcessGetMessage(CrawlerMessages.GetData getDataMessage)
        {
            try
            {
                HeavyJob(5);

                _log.Debug($"get data received for: {getDataMessage.MergeObject.DataId}");
                var mergeObject = getDataMessage.MergeObject;
                var url = $"{getDataMessage.ApiEndPoint}/{mergeObject.DataId}";
                var self = Context.Self;

                _client.GetAsync(url).ContinueWith(
                    response =>
                        {
                            try
                            {
                                var webResponse = response.Result;
                                if (webResponse.StatusCode == HttpStatusCode.OK)
                                {
                                    webResponse.Content.ReadAsAsync<WebApiResponseDto>().ContinueWith(
                                        request =>
                                            {
                                                if (request.Exception == null)
                                                {
                                                    _log.Info($" read api for:{getDataMessage.MergeObject.DataId} status:{webResponse.StatusCode.ToString()}");
                                                    return new CrawlerMessages.PipedRequest(request.Result, mergeObject);
                                                }

                                                throw request.Exception;
                                            },
                                        TaskContinuationOptions.AttachedToParent & TaskContinuationOptions.ExecuteSynchronously).PipeTo(self);
                                }
                                else
                                {
                                    // there is a connection but for any reason server response is not OK, so log that and 
                                    _log.Debug($"cannot read api for:{getDataMessage.MergeObject.DataId} status:{webResponse.StatusCode.ToString()}");
                                    SendBadResponse(mergeObject, $"---Error:{webResponse.StatusCode.ToString()}---");

                                }

                            }
                            catch (Exception e)
                            {
                                _log.Error("cannot get data from api inner catch ", e);
                                self.Tell(new CrawlerMessages.WebApiErrorResponse { url = url, MergeObjectDto = mergeObject, attempt = 1 });
                            }
                        });
            }
            catch (Exception e)
            {
                _log.Error($"cannot read api main catch for:{getDataMessage.MergeObject.DataId} ", e);
                SendBadResponse(getDataMessage.MergeObject, _errorStatus);
            }
        }

        private static void HeavyJob(int i1)
        {
            // heavy job
            for (var i = 0; i < i1 * 100; i++)
            {
                var ae = DateTime.Now.AddHours(1);
                var ee = ae.Date.AddHours(254).Ticks;
            }
        }

        /// <summary>The process piped request.</summary>
        /// <param name="pipedRequest">The o.</param>
        private void ProcessPipedRequest(CrawlerMessages.PipedRequest pipedRequest)
        {
            var response = pipedRequest.RequestResult;
            var mergeObject = pipedRequest.MergeObject;
            if (response.IsError)
            {
                _actorDictionary["FlowControlActor"].Tell(new FlowControlMessages.WebApiGotBadResponse());
            }
            else
            {
                _actorDictionary["FlowControlActor"].Tell(new FlowControlMessages.WebApiGotValidResponse());
            }

            _actorDictionary["FileWriterActor"].Tell(new FileWriterMessages.SaveWebResponse(mergeObject));
            _processed++;

            if (_processed % 100 == 0)
            {
                Console.WriteLine($"Processed requests: {_processed}, actor:{Self.Path.Name}");
            }

            _lastActivity = DateTime.Now;
        }

        /// <summary>The process timer.</summary>
        private void ProcessTimer()
        {

            var delta = (DateTime.Now - _lastActivity).TotalMilliseconds;
            _log.Info($"ProcessTimer current delta:{delta}");
            if (delta > _systemConfiguration.WorkerIdleTime)
            {
                _log.Debug("GetNewLinesForCrawler: pull");
                _actorDictionary["FlowControlActor"].Tell(new FlowControlMessages.GetNewLinesForCrawler());
            }


        }


        /// <summary>The process web error message.</summary>
        /// <param name="webApuResponse">The a.</param>
        private void ProcessWebErrorMessage(CrawlerMessages.WebApiErrorResponse webApuResponse)
        {
            try
            {
                _log.Debug($"retrying for: {webApuResponse.MergeObjectDto.DataId}");
                var mergeObject = webApuResponse.MergeObjectDto;
                var self = Context.Self;

                _client.GetAsync(webApuResponse.url).ContinueWith(
                    response =>
                        {
                            try
                            {
                                var webResponse = response.Result;
                                if (webResponse.StatusCode == HttpStatusCode.OK)
                                {
                                    webResponse.Content.ReadAsAsync<WebApiResponseDto>()
                                        .ContinueWith(
                                            request => { return new CrawlerMessages.PipedRequest(request.Result, mergeObject); },
                                            TaskContinuationOptions.AttachedToParent & TaskContinuationOptions.ExecuteSynchronously)
                                        .PipeTo(self);
                                }
                                else
                                {
                                    // there is a connection but for any reason server response is not OK, so log that and 
                                    // do not retry that message
                                    SendBadResponse(mergeObject, $"---Error:{webResponse.StatusCode.ToString()}---");
                                    return;
                                }
                            }
                            catch (Exception e)
                            {
                                _log.Error("Api connection error inner catch WebApiErrorResponse", e);
                            }

                            if (webApuResponse.attempt <= _systemConfiguration.HttpRetries)
                            {
                                self.Tell(
                                    new CrawlerMessages.WebApiErrorResponse { url = webApuResponse.url, MergeObjectDto = mergeObject, attempt = ++webApuResponse.attempt });
                            }
                            else
                            {
                                SendBadResponse(mergeObject, $"--- NetworkIssue ---");
                            }
                        });
            }
            catch (Exception e)
            {
                _log.Error($"cannot read api outer catch WebApiErrorResponse for:{webApuResponse.MergeObjectDto.DataId} ", e);
                SendBadResponse(webApuResponse.MergeObjectDto, _errorStatus);

            }
            finally
            {
                _lastActivity = DateTime.Now;
            }
        }

        /// <summary>The send bad response.</summary>
        /// <param name="mergeObject">The merge object.</param>
        /// <param name="status">The status.</param>
        private void SendBadResponse(MergeObjectDto mergeObject, string status = "none")
        {
            _actorDictionary["FileWriterActor"].Tell(new FileWriterMessages.SaveWebResponse(mergeObject));
            _actorDictionary["FlowControlActor"].Tell(new FlowControlMessages.WebApiGotBadResponse());
        }

        /// <summary>The setup http client.</summary>
        private void SetupHttpClient()
        {
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.Timeout = TimeSpan.FromSeconds(300);
        }
    }
}

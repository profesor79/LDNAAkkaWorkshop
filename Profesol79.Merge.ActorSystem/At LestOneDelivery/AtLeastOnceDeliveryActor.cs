// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="AtLeastOnceDeliveryActor.cs">
//   
// </copyright>
// <summary>
//   Created: 2017-07-12, 11:22 PM
//   Last changed by: profesor79, 2017-07-12, 11:34 PM
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Profesor79.Merge.ActorSystem.At_LestOneDelivery
{
    using System;

    using Akka.Actor;
    using Akka.Persistence;

    using Profesor79.Merge.ActorSystem.WebCrawler;
    using Profesor79.Merge.Models;

    /// <summary>The at least once delivery actor.</summary>
    public class AtLeastOnceDeliveryActor : AtLeastOnceDeliveryReceiveActor
    {
        /// <summary>The _recurring snapshot cleanup.</summary>
        private ICancelable _recurringSnapshotCleanup;

        /// <summary>The _target actor.</summary>
        private IActorRef _targetActor;

        /// <summary>Initializes a new instance of the <see cref="AtLeastOnceDeliveryActor"/> class.</summary>
        /// <param name="targetActor">The target actor.</param>
        public AtLeastOnceDeliveryActor(IActorRef targetActor)
        {
            _targetActor = targetActor;

            // recover the most recent at least once delivery state
            Recover<SnapshotOffer>(offer => offer.Snapshot is AtLeastOnceDeliverySnapshot, offer =>
            {
                var snapshot = offer.Snapshot as AtLeastOnceDeliverySnapshot;
                SetDeliverySnapshot(snapshot);
            });

            Command<CrawlerMessages.GetData>(send =>
            {
                Self.Tell(new GetDataRegistered(send.MergeObject, send.ApiEndPoint));
            });

            Command<GetDataRegistered>(write =>
                {
                    var message = new CrawlerMessages.GetData(write.MergeObject, write.ApiEndPoint);
                    Deliver(_targetActor.Path, messageId => new ReliableDeliveryEnvelope<CrawlerMessages.GetData>(message, messageId));

                    // save the full state of the at least once delivery actor
                    // so we don't lose any messages upon crash
                    SaveSnapshot(GetDeliverySnapshot());
                });


            Command<ReliableDeliveryAck>(ack =>
            {
                ConfirmDelivery(ack.MessageId);
            });

            Command<CleanSnapshots>(clean =>
            {
                // save the current state (grabs confirmations)
                SaveSnapshot(GetDeliverySnapshot());
            });

            Command<SaveSnapshotSuccess>(saved =>
            {
                var seqNo = saved.Metadata.SequenceNr;
                DeleteSnapshots(new SnapshotSelectionCriteria(seqNo, saved.Metadata.Timestamp.AddMilliseconds(-1))); // delete all but the most current snapshot
            });

            Command<SaveSnapshotFailure>(failure =>
            {
                // log or do something else
            });
        }

        /// <summary>The persistence id.</summary>
        public override string PersistenceId => Context.Self.Path.Name;

        /// <summary>The post stop.</summary>
        protected override void PostStop()
        {
            _recurringSnapshotCleanup?.Cancel();
            base.PostStop();
        }

        /// <summary>The pre start.</summary>
        protected override void PreStart()
        {
             
            _recurringSnapshotCleanup =
                Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(TimeSpan.FromSeconds(10),
                    TimeSpan.FromSeconds(10), Self, new CleanSnapshots(), ActorRefs.NoSender);

            base.PreStart();
        }

        /// <summary>The reliable delivery ack.</summary>
        public class ReliableDeliveryAck
        {
            /// <summary>Initializes a new instance of the <see cref="ReliableDeliveryAck"/> class.</summary>
            /// <param name="messageId">The message id.</param>
            public ReliableDeliveryAck(long messageId)
            {
                MessageId = messageId;
            }

            /// <summary>Gets the message id.</summary>
            public long MessageId { get; private set; }
        }

        /// <summary>The reliable delivery envelope.</summary>
        /// <typeparam name="TMessage"></typeparam>
        public class ReliableDeliveryEnvelope<TMessage>
        {
            /// <summary>Initializes a new instance of the <see cref="ReliableDeliveryEnvelope{TMessage}"/> class.</summary>
            /// <param name="message">The message.</param>
            /// <param name="messageId">The message id.</param>
            public ReliableDeliveryEnvelope(TMessage message, long messageId)
            {
                Message = message;
                MessageId = messageId;
            }

            /// <summary>Gets the message.</summary>
            public TMessage Message { get; private set; }

            /// <summary>Gets the message id.</summary>
            public long MessageId { get; private set; }
        }

        /// <summary>The write.</summary>
        public class Write
        {
            /// <summary>Initializes a new instance of the <see cref="Write"/> class.</summary>
            /// <param name="content">The content.</param>
            public Write(string content)
            {
                Content = content;
            }

            /// <summary>Gets the content.</summary>
            public string Content { get; private set; }
        }

        /// <summary>The clean snapshots.</summary>
        private class CleanSnapshots { }

  


        /// <summary>The get data.</summary>
        public class GetDataRegistered
        {
            /// <summary>Initializes a new instance of the <see cref="CrawlerMessages.GetData"/> class.</summary>
            /// <param name="mergeObject">The merge object.</param>
            public GetDataRegistered(MergeObjectDto mergeObject, string apiEndPoint)
            {
                MergeObject = mergeObject;
                ApiEndPoint = apiEndPoint;
            }

            /// <summary>Gets the merge object.</summary>
            public MergeObjectDto MergeObject { get; }

            public string ApiEndPoint { get; }
        }

    }
}

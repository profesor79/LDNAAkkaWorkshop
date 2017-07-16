namespace Profesor79.Merge.ActorSystem.Persistence
{
    using System;

    using Akka.Actor;
    using Akka.Persistence;

    using Profesor79.Merge.ActorSystem.WebCrawler;

    public class PersistenceActor : AtLeastOnceDeliveryReceiveActor
    {
        // Going to use our name for persistence purposes
        public override string PersistenceId => Guid.NewGuid().ToString();
        
                private ICancelable _recurringSnapshotCleanup;
        private readonly IActorRef _targetActor;

        private class DoSend { }
        private class CleanSnapshots { }

        public PersistenceActor(IActorRef targetActor)
        {
            _targetActor = targetActor;

            // recover the most recent at least once delivery state
            Recover<SnapshotOffer>(offer => offer.Snapshot is Akka.Persistence.AtLeastOnceDeliverySnapshot, offer =>
            {
                var snapshot = offer.Snapshot as Akka.Persistence.AtLeastOnceDeliverySnapshot;
                SetDeliverySnapshot(snapshot);
            });

            Command<DoSend>(send =>
            {
                //Self.Tell(new Write("Message "));
            });

            Command<CrawlerMessages.GetData>(crawlerMessagesGetData =>
            {
                Deliver(_targetActor.Path, messageId => new ReliableDeliveryEnvelope<CrawlerMessages.GetData>(crawlerMessagesGetData, messageId));

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

        protected override void PreStart()
        {
            
            _recurringSnapshotCleanup =
                Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(TimeSpan.FromSeconds(10),
                    TimeSpan.FromSeconds(10), Self, new CleanSnapshots(), ActorRefs.NoSender);

            base.PreStart();
        }

        protected override void PostStop()
        {
            _recurringSnapshotCleanup?.Cancel();

            base.PostStop();
        }
    }
}
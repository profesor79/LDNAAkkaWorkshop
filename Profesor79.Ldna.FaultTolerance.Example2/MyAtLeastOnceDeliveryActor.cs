//  --------------------------------------------------------------------------------------------------------------------
// <copyright company="profesor79.pl" file="MyAtLeastOnceDeliveryActor.cs">
// Copyright (c) 2017 All Right Reserved
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
// </copyright>
// <summary>
// Created: 2017-07-13, 2:38 PM
// Last changed by: profesor79, 2017-07-13, 5:24 PM 
// </summary>
//   --------------------------------------------------------------------------------------------------------------------

namespace AtLeastOnceDelivery.Console
{
    using System;

    using Akka.Actor;
    using Akka.Persistence;

    /// <summary>The my at least once delivery actor.</summary>
    public class MyAtLeastOnceDeliveryActor : AtLeastOnceDeliveryReceiveActor
    {
        /// <summary>The characters.</summary>
        private const string Characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        /// <summary>The _target actor.</summary>
        private readonly IActorRef _targetActor;

        /// <summary>The _counter.</summary>
        private int _counter = 0;

        /// <summary>The _recurring message send.</summary>
        private ICancelable _recurringMessageSend;

        /// <summary>The _recurring snapshot cleanup.</summary>
        private ICancelable _recurringSnapshotCleanup;

        /// <summary>Initializes a new instance of the <see cref="MyAtLeastOnceDeliveryActor"/> class.</summary>
        /// <param name="targetActor">The target actor.</param>
        public MyAtLeastOnceDeliveryActor(IActorRef targetActor)
        {
            _targetActor = targetActor;

            // recover the most recent at least once delivery state
            Recover<SnapshotOffer>(
                offer => offer.Snapshot is AtLeastOnceDeliverySnapshot,
                offer =>
                    {
                        var snapshot = offer.Snapshot as AtLeastOnceDeliverySnapshot;
                        SetDeliverySnapshot(snapshot);
                    });

            Command<DoSend>(send => { Self.Tell(new Write("Message " + Characters[_counter++ % Characters.Length])); });

            Command<Write>(
                write =>
                    {
                        Deliver(_targetActor.Path, messageId => new ReliableDeliveryEnvelope<Write>(write, messageId));

                        // save the full state of the at least once delivery actor
                        // so we don't lose any messages upon crash
                        SaveSnapshot(GetDeliverySnapshot());
                    });

            Command<ReliableDeliveryAck>(ack => { ConfirmDelivery(ack.MessageId); });

            Command<CleanSnapshots>(
                clean =>
                    {
                        // save the current state (grabs confirmations)
                        SaveSnapshot(GetDeliverySnapshot());
                    });

            Command<SaveSnapshotSuccess>(
                saved =>
                    {
                        var seqNo = saved.Metadata.SequenceNr;
                        DeleteSnapshots(new SnapshotSelectionCriteria(seqNo, saved.Metadata.Timestamp.AddMilliseconds(-1)));
                            // delete all but the most current snapshot
                    });

            Command<SaveSnapshotFailure>(
                failure =>
                    {
                        // log or do something else
                    });
        }

        // Going to use our name for persistence purposes
        /// <summary>The persistence id.</summary>
        public override string PersistenceId => Context.Self.Path.Name;

        /// <summary>The post stop.</summary>
        protected override void PostStop()
        {
            _recurringSnapshotCleanup?.Cancel();
            _recurringMessageSend?.Cancel();

            base.PostStop();
        }

        /// <summary>The pre start.</summary>
        protected override void PreStart()
        {
            _recurringMessageSend = Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(1),
                Self,
                new DoSend(),
                Self);

            _recurringSnapshotCleanup = Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(
                TimeSpan.FromSeconds(10),
                TimeSpan.FromSeconds(10),
                Self,
                new CleanSnapshots(),
                ActorRefs.NoSender);

            base.PreStart();
        }

        /// <summary>The clean snapshots.</summary>
        private class CleanSnapshots
        {
        }

        /// <summary>The do send.</summary>
        private class DoSend
        {
        }
    }
}

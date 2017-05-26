//  --------------------------------------------------------------------------------------------------------------------
// <copyright company="profesor79.pl" file="BaseActorClass.cs">
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

namespace Profesor79.Merge.ActorSystem.BaseObjects
{
    using System.Collections.Generic;

    using Akka.Actor;
    using Akka.Event;

    /// <summary>The base actor class.</summary>
    public class BaseActorClass : ReceiveActor, ILogReceive
    {
        /// <summary>The _log.</summary>
        public readonly ILoggingAdapter _log = Context.GetLogger();

        /// <summary>The _actor dictionary.</summary>
        public Dictionary<string, IActorRef> _actorDictionary = new Dictionary<string, IActorRef>();

        // if any child, e.g. the logger above. throws an exception
        // apply the rules below
        // e.g. Restart the child, if 10 exceptions occur in 30 seconds or
        // less, then stop the actor

        /// <summary>The supervisor strategy.</summary>
        /// <returns>The <see cref="SupervisorStrategy"/>.</returns>
        protected override SupervisorStrategy SupervisorStrategy() { return new OneForOneStrategy(x => Directive.Stop); }
    }
}

//  --------------------------------------------------------------------------------------------------------------------
// <copyright company="profesor79.pl" file="TestActor.cs">
// Copyright (c) 2017 All Right Reserved
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
// </copyright>
// <summary>
// Created: 2017-07-13, 5:23 PM
// Last changed by: profesor79, 2017-07-13, 5:24 PM 
// </summary>
//   --------------------------------------------------------------------------------------------------------------------

namespace AtLeastOnceDelivery.Console
{
    using System;

    using Akka.Actor;

    /// <summary>The test actor.</summary>
    public class TestActor : ReceiveActor
    {
        /// <summary>Initializes a new instance of the <see cref="TestActor"/> class.</summary>
        public TestActor()
        {
            Receive<Messages.TestFirstCase>(
                m =>
                    {
                        var actor = Context.ActorOf(Props.Create(() => new ArithmeticExceptionActor()), "ArithmeticExceptionActor");
                        for (var i = 1; i < 30; i++)
                        {

                            if (i % 3 == 0)
                            {
                                actor.Tell(new Messages.GenerateException(i));
                            }
                            else
                            {
                                actor.Tell(new Messages.GenerateWork(i));
                            }
                        }

                        Console.WriteLine("Ready");
                    });

            Receive<Messages.TestSecondCase>(
                m =>
                    {
                        var actor = Context.ActorOf(Props.Create(() => new NotSupportedExceptionActor()), "NotSupportedExceptionActor");
                        for (var i = 1; i < 30; i++)
                        {

                            if (i % 3 == 0)
                            {
                                actor.Tell(new Messages.GenerateException(i));
                            }
                            else
                            {
                                actor.Tell(new Messages.GenerateWork(i));
                            }
                        }

                        Console.WriteLine("Ready");
                    });

            Receive<Messages.TestForothCase>(
                m =>
                    {
                        var actor = Context.ActorOf(Props.Create(() => new NotSupportedExceptionActor()), "NotSupportedExceptionActor");
                        for (var i = 1; i < 50; i++)
                        {

                            if (i % 3 == 0)
                            {
                                actor.Tell(new Messages.GenerateException(i));
                            }
                            else
                            {
                                actor.Tell(new Messages.GenerateWork(i));
                            }
                        }

                        Console.WriteLine("Ready");
                    });

            Receive<Messages.TestThirdCase>(
                m =>
                    {
                        var actor = Context.ActorOf(Props.Create(() => new OtherExceptionActor()), "OtherExceptionActor");
                        for (var i = 1; i < 30; i++)
                        {

                            if (i % 3 == 0)
                            {
                                actor.Tell(new Messages.GenerateException(i));
                            }
                            else
                            {
                                actor.Tell(new Messages.GenerateWork(i));
                            }
                        }

                        Console.WriteLine("Ready");
                    });
        }

        // if any child, e.g. the logger above. throws an exception
        // apply the rules below
        // e.g. Restart the child, if 10 exceptions occur in 30 seconds or
        // less, then stop the actor
        /// <summary>The supervisor strategy.</summary>
        /// <returns>The <see cref="SupervisorStrategy"/>.</returns>
        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy(
                10,
                TimeSpan.FromSeconds(30),
                Decider.From(
                    x =>
                        {
                            // Maybe we consider ArithmeticException to not be application critical
                            // so we just ignore the error and keep going.
                            if (x is ArithmeticException)
                            {
                                return Directive.Resume;
                            }

                            // Error that we cannot recover from, stop the failing actor
                            else
                            {
                                if (x is NotSupportedException)
                                {
                                    return Directive.Stop;
                                }

                                // In all other cases, just restart the failing actor
                                else
                                {
                                    return Directive.Restart;
                                }
                            }
                        }));
        }
    }
}

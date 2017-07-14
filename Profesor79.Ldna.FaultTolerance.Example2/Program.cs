//  --------------------------------------------------------------------------------------------------------------------
// <copyright company="profesor79.pl" file="Program.cs">
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

    /// <summary>The program.</summary>
    internal class Program
    {
        /// <summary>The main.</summary>
        /// <param name="args">The args.</param>
        private static void Main(string[] args)
        {
            using (var actorSystem = ActorSystem.Create("AtLeastOnceDeliveryDemo"))
            {
                var actor = actorSystem.ActorOf(Props.Create(() => new TestActor()), "TestActor");

                 actor.Tell(new Messages.TestFirstCase());
                // actor.Tell(new Messages.TestSecondCase());
                // actor.Tell(new Messages.TestThirdCase());
                Console.WriteLine("waiting for termination");
                Console.ReadLine();
                actorSystem.Terminate();
            }
        }
    }
}

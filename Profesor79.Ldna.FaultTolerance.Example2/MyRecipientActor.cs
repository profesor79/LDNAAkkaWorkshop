//  --------------------------------------------------------------------------------------------------------------------
// <copyright company="profesor79.pl" file="MyRecipientActor.cs">
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

    /// <summary>The my recipient actor.</summary>
    public class MyRecipientActor : ReceiveActor
    {
        /// <summary>Initializes a new instance of the <see cref="MyRecipientActor"/> class.</summary>
        public MyRecipientActor()
        {
            Receive<ReliableDeliveryEnvelope<Write>>(
                write =>
                    {
                        Console.WriteLine("Received message {0} [id: {1}] from {2} - accept?", write.Message.Content, write.MessageId, Sender);
                        var response = Console.ReadLine()?.ToLowerInvariant();
                        if (!string.IsNullOrEmpty(response) && (response.ToLower().Equals("yes") || response.ToLower().Equals("y")))
                        {
                            // confirm delivery only if the user explicitly agrees
                            Sender.Tell(new ReliableDeliveryAck(write.MessageId));
                            Console.WriteLine("Confirmed delivery of message ID {0}", write.MessageId);
                        }
                        else
                        {
                            Console.WriteLine("Did not confirm delivery of message ID {0}", write.MessageId);
                        }
                    });
        }
    }
}

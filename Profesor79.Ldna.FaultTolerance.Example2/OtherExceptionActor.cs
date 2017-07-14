//  --------------------------------------------------------------------------------------------------------------------
// <copyright company="profesor79.pl" file="OtherExceptionActor.cs">
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

    /// <summary>The other exception actor.</summary>
    public class OtherExceptionActor : ReceiveActor
    {
        /// <summary>Initializes a new instance of the <see cref="OtherExceptionActor"/> class. Initializes a new instance of the <see cref="NotSupportedExceptionActor"/> class.</summary>
        /// <exception cref="NotSupportedException"></exception>
        public OtherExceptionActor()
        {
            Receive<Messages.GenerateWork>(m => { Console.WriteLine($"OtherExceptionActor: Generate work received all seems to be ok, order:{m.OrderNumber}"); });

            Receive<Messages.GenerateException>(
                m =>
                    {
                        Console.WriteLine($"OtherExceptionActor: Generate Exception received, throwing, order:{m.OrderNumber}");
                        throw new StackOverflowException($"OtherExceptionActor: throwing:  StackOverflowException....., order:{m.OrderNumber}");
                    });
        }
    }
}

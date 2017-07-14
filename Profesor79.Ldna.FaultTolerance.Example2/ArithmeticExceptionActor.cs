//  --------------------------------------------------------------------------------------------------------------------
// <copyright company="profesor79.pl" file="ArithmeticExceptionActor.cs">
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

    /// <summary>The arithmetic exception actor.</summary>
    public class ArithmeticExceptionActor : ReceiveActor
    {
        private int _counter;

        /// <summary>Initializes a new instance of the <see cref="ArithmeticExceptionActor"/> class.</summary>
        /// <exception cref="ArithmeticException"></exception>
        public ArithmeticExceptionActor()
        {
            Receive<Messages.GenerateWork>(
                m =>
                    {
                        _counter++;
                        Console.WriteLine($"ArithmeticExceptionActor: Generate work received all seems to be ok, order:{m.OrderNumber}, counuter:{_counter}");
                    });

            Receive<Messages.GenerateException>(
                m =>
                    {
                        _counter++;
                        Console.WriteLine($"ArithmeticExceptionActor: Generate Exception received, throwing, order:{m.OrderNumber}counuter:{_counter}");
                        throw new StackOverflowException();
                    });
        }
    }
}

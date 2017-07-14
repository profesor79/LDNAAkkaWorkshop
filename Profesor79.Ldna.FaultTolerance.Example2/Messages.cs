//  --------------------------------------------------------------------------------------------------------------------
// <copyright company="profesor79.pl" file="Messages.cs">
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
    /// <summary>The messages.</summary>
    public class Messages
    {
        /// <summary>The generate exception.</summary>
        public class GenerateException
        {
            /// <summary>Initializes a new instance of the <see cref="GenerateException"/> class.</summary>
            /// <param name="orderNumber">The order number.</param>
            public GenerateException(int orderNumber) { OrderNumber = orderNumber; }

            /// <summary>Gets the order number.</summary>
            public int OrderNumber { get; }
        }

        /// <summary>The generate work.</summary>
        public class GenerateWork
        {
            /// <summary>Initializes a new instance of the <see cref="GenerateWork"/> class.</summary>
            /// <param name="orderNumber">The order number.</param>
            public GenerateWork(int orderNumber) { OrderNumber = orderNumber; }

            /// <summary>Gets the order number.</summary>
            public int OrderNumber { get; }
        }

        /// <summary>The test first case.</summary>
        public class TestFirstCase
        {
        }

        /// <summary>The test foroth case.</summary>
        public class TestForothCase
        {
        }

        /// <summary>The test second case.</summary>
        public class TestSecondCase
        {
        }

        /// <summary>The test third case.</summary>
        public class TestThirdCase
        {
        }
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

    /// <summary>The reliable delivery ack.</summary>
    public class ReliableDeliveryAck
    {
        /// <summary>Initializes a new instance of the <see cref="ReliableDeliveryAck"/> class.</summary>
        /// <param name="messageId">The message id.</param>
        public ReliableDeliveryAck(long messageId) { MessageId = messageId; }

        /// <summary>Gets the message id.</summary>
        public long MessageId { get; private set; }
    }

    /// <summary>The write.</summary>
    public class Write
    {
        /// <summary>Initializes a new instance of the <see cref="Write"/> class.</summary>
        /// <param name="content">The content.</param>
        public Write(string content) { Content = content; }

        /// <summary>Gets the content.</summary>
        public string Content { get; private set; }
    }
}

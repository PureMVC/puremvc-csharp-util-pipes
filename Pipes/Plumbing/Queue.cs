//
//  PureMVC C# Multicore Utility - Pipes
//
//  Copyright(c) 2017-2027 Saad Shams <saad.shams@puremvc.org>
//  Your reuse is governed by the Creative Commons Attribution 3.0 License
//

using Pipes.Interfaces;
using Pipes.Messages;
using System.Collections.Generic;

namespace Pipes.Plumbing
{
    /// <summary>
    /// Pipe Queue.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The Queue always stores inbound messages until you send it
    ///         a FLUSH control message, at which point it writes its buffer 
    ///         to the output pipe fitting. The Queue can be sent a SORT 
    ///         control message to go into sort-by-priority mode or a FIFO 
    ///         control message to cancel sort mode and return the
    ///         default mode of operation, FIFO.
    ///     </para>
    ///     <para>
    ///         NOTE: There can effectively be only one Queue on a given 
    ///         pipeline, since the first Queue acts on any queue control 
    ///         message. Multiple queues in one pipeline are of dubious 
    ///         use, and so having to name them would make their operation 
    ///         more complex than need be.
    ///     </para>
    /// </remarks>
    public class Queue : Pipe
    {
        /// <summary>
        /// Queue constructor
        /// </summary>
        /// <param name="output">Optional output pipe</param>
        public Queue(IPipeFitting output=null) : base(output)
        {
        }

        /// <summary>
        /// Handle the incoming message.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Normal messages are enqueued.
        ///     </para>
        ///     <para>
        ///         The FLUSH message type tells the Queue to write all 
        ///         stored messages to the ouptut PipeFitting, then 
        ///         return to normal enqueing operation.
        ///     </para>
        ///     <para>
        ///         The SORT message type tells the Queue to sort all 
        ///         <I>subsequent</I> incoming messages by priority. If there
        ///         are unflushed messages in the queue, they will not be
        ///         sorted unless a new message is sent before the next FLUSH.
        ///         Sorting-by-priority behavior continues even after a FLUSH, 
        ///         and can be turned off by sending a FIFO message, which is 
        ///         the default behavior for enqueue/dequeue.
        ///     </para>
        /// </remarks>
        /// <param name="message"></param>
        /// <returns></returns>
        public override bool Write(IPipeMessage message)
        {
            bool success = true;
            switch(message.Type)
            {
                // Store normal messages
                case Message.NORMAL:
                    Store(message);
                    break;

                // Flush the queue
                case QueueControlMessage.FLUSH:
                    success = Flush();
                    break;

                // Put Queue into Priority Sort or FIFO mode 
                // Subsequent messages written to the queue
                // will be affected. Sorted messages cannot
                // be put back into FIFO order!
                case QueueControlMessage.SORT:
                case QueueControlMessage.FIFO:
                    Mode = message.Type;
                    break;
            }
            return success;
        }

        /// <summary>
        /// Store a message.
        /// </summary>
        /// <param name="message">the IPipeMessage to enqueue.</param>
        protected virtual void Store(IPipeMessage message)
        {
            Messages.Add(message);

            // Sort the Messages by priority.
            if (Mode == QueueControlMessage.SORT) Messages.Sort(delegate(IPipeMessage msgA, IPipeMessage msgB) {
                int num = 0;
                if (msgA.Priority < msgB.Priority) num = -1;
                if (msgA.Priority > msgB.Priority) num = 1;
                return num;
            });
        }

        /// <summary>
        /// Flush the queue.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         NOTE: This empties the queue.
        ///     </para>
        /// </remarks>
        /// <returns>true if all messages written successfully.</returns>
        protected virtual bool Flush()
        {
            bool success = true;
            Messages.RemoveAll(message => {
                bool ok = Output.Write(message);
                if (!ok) success = false;
                return true;
            });
            return success;
        }

        /// <summary>List to store messages</summary>
        protected List<IPipeMessage> Messages { get; } = new List<IPipeMessage>();

        /// <summary>Get or Set the mode.</summary>
        protected string Mode{ get; set; } = QueueControlMessage.SORT;
    }
}

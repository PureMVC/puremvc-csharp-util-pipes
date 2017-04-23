//
//  PureMVC C# Multicore Utility - Pipes
//
//  Copyright(c) 2017-2027 Saad Shams <saad.shams@puremvc.org>
//  Your reuse is governed by the Creative Commons Attribution 3.0 License
//

namespace Pipes.Messages
{
    /// <summary>
    /// Queue Control Message.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         A special message for controlling the behavior of a Queue.
    ///     </para>
    ///     <para>
    ///         When written to a pipeline containing a Queue, the type
    ///         of the message is interpreted and acted upon by the Queue.
    ///     </para>
    ///     <para>
    ///         Unlike filters, multiple serially connected queues aren't 
    ///         very useful and so they do not require a name. If multiple
    ///         queues are connected serially, the message will be acted 
    ///         upon by the first queue only
    ///     </para>
    /// </remarks>
    public class QueueControlMessage: Message
    {
        /// <summary>Flush the queue.</summary>
        public const string FLUSH = "http://puremvc.org/namespaces/pipes/messages/queue/flush";

        /// <summary>Toggle to sort-by-priority operation mode.</summary>
        public const string SORT = "http://puremvc.org/namespaces/pipes/messages/queue/sort";

        /// <summary>Toggle to FIFO operation mode (default behavior).</summary>
        public const string FIFO = "http://puremvc.org/namespaces/pipes/messages/queue/fifo";

        /// <summary>Constructor</summary>
        public QueueControlMessage(string type): base(type)
        {
        }
    }
}

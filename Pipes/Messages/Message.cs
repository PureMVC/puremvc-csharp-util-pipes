//
//  PureMVC C# Multicore Utility - Pipes
//
//  Copyright(c) 2017-2027 Saad Shams <saad.shams@puremvc.org>
//  Your reuse is governed by the Creative Commons Attribution 3.0 License
//

using Pipes.Interfaces;

namespace Pipes.Messages
{
    /// <summary>
    /// Pipe Message.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Messages travelling through a Pipeline can
    ///         be filtered, and queued. In a queue, they may
    ///         be sorted by priority. Based on type, they 
    ///         may used as control messages to modify the
    ///         behavior of filter or queue fittings connected
    ///         to the pipleline into which they are written.
    ///     </para>
    /// </remarks>
    public class Message: IPipeMessage
    {
        /// <summary>High priority Messages can be sorted to the front of the queue</summary>
        public const int PRIORITY_HIGH = 1;

        /// <summary>Medium priority Messages are the default</summary>
        public const int PRIORITY_MED = 5;

        /// <summary>Low priority Messages can be sorted to the back of the queue</summary>
        public const int PRIORITY_LOW = 10;

        /// <summary>Normal Message type.</summary>
        public const string NORMAL = "http://puremvc.org/namespaces/pipes/messages/normal/";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type"></param>
        /// <param name="header"></param>
        /// <param name="body"></param>
        /// <param name="priority"></param>
        public Message(string type, object header = null, object body = null, int priority = 5)
        {
            Type = type;
            Header = header;
            Body = body;
            Priority = priority;
        }

        /// <summary>Get or Set the type of this message</summary>
        public string Type { get; set; }

        /// <summary>Get or Set the header of this message</summary>
        public object Header { get; set; }

        /// <summary>Get or Set the priority of this message</summary>
        public int Priority { get; set; }

        /// <summary>Get or Set the body of this message</summary>
        public object Body { get; set; }
    }
}

//
//  PureMVC C# Multicore Utility - Pipes
//
//  Copyright(c) 2017-2027 Saad Shams <saad.shams@puremvc.org>
//  Your reuse is governed by the Creative Commons Attribution 3.0 License
//

using System;
using Pipes.Interfaces;

namespace Pipes.Plumbing
{
    /// <summary>
    /// Pipe Listener.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Allows a class that does not implement <c>IPipeFitting</c> to
    ///         be the final recipient of the messages in a pipeline.
    ///     </para>
    /// </remarks>
    public class PipeListener : IPipeFitting
    {
        /// <summary>Constructor - Receives context object and callback method</summary>
        public PipeListener(object context, Action<IPipeMessage> listener)
        {
            Context = context;
            Listener = listener;
        }

        /// <summary>
        /// Can't connect anything beyond this.
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        public virtual bool Connect(IPipeFitting output)
        {
            return false;
        }

        /// <summary>
        /// Can't disconnect since you can't connect, either.
        /// </summary>
        /// <returns></returns>
        public virtual IPipeFitting Disconnect()
        {
            return null;
        }

        /// <summary>Write the message to the listener</summary>
        public virtual bool Write(IPipeMessage message)
        {
            Listener(message);
            return true;
        }

        /// <summary>Context of the Object</summary>
        public object Context { get; set; }

        /// <summary>Callback method</summary>
        public Action<IPipeMessage> Listener { get; set; }
    }
}

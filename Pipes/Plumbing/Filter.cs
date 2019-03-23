//
//  PureMVC C# Multicore Utility - Pipes
//
//  Copyright(c) 2017-2027 Saad Shams <saad.shams@puremvc.org>
//  Your reuse is governed by the Creative Commons Attribution 3.0 License
//

using System;
using Pipes.Interfaces;
using Pipes.Messages;

namespace Pipes.Plumbing
{
    /// <summary>
    /// Pipe Filter.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Filters may modify the contents of messages before writing them to 
    ///         their output pipe fitting. They may also have their parameters and
    ///         filter function passed to them by control message, as well as having
    ///         their Bypass/Filter operation mode toggled via control message.
    ///     </para>
    /// </remarks>
    public class Filter : Pipe
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Name of the filter</param>
        /// <param name="output">Output pipe</param>
        /// <param name="filter">Filter function</param>
        /// <param name="params">Filter function parameters</param>
        public Filter(string name, IPipeFitting output = null, Func<IPipeMessage, object, bool> filter = null, object @params = null) : base(output)
        {
            this.name = name;
            if (filter != null) SetFilter(filter);
            if (@params != null) SetParams(@params);
        }

        /// <summary>
        /// Handle the incoming message.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         If message type is normal, filter the message (unless in BYPASS mode)
        ///         and write the result to the output pipe fitting if the filter 
        ///         operation is successful.
        ///     </para>
        ///     <para>
        ///         The FilterControlMessage.SET_PARAMS message type tells the Filter
        ///         that the message class is FilterControlMessage, which it 
        ///         casts the message to in order to retrieve the filter parameters
        ///         object if the message is addressed to this filter.
        ///     </para>
        ///     <para>
        ///         The FilterControlMessage.SET_FILTER message type tells the Filter
        ///         that the message class is FilterControlMessage, which it 
        ///         casts the message to in order to retrieve the filter function.
        ///     </para>
        ///     <para>
        ///         The FilterControlMessage.BYPASS message type tells the Filter
        ///         that it should go into Bypass mode operation, passing all normal
        ///         messages through unfiltered.
        ///     </para>
        ///     <para>
        ///         The FilterControlMessage.FILTER message type tells the Filter
        ///         that it should go into Filtering mode operation, filtering all
        ///         normal normal messages before writing out. This is the default
        ///         mode of operation and so this message type need only be sent to
        ///         cancel a previous BYPASS message.
        ///     </para>
        ///     <para>
        ///         The Filter only acts on the control message if it is targeted 
        ///         to this named filter instance. Otherwise it writes through to the
        ///         output.
        ///     </para>
        /// </remarks>
        /// <param name="message">Message</param>
        /// <returns>Boolean True if the filter process does not throw an error and subsequent operations in the pipeline succede.</returns>
        public override bool Write(IPipeMessage message)
        {
            bool success = true;

            // Filter normal messages
            switch (message.Type)
            {
                case Message.NORMAL:
                    if (Mode == FilterControlMessage.FILTER)
                    {
                        success = ApplyFilter(message) ? Output.Write(message) : false;
                    }
                    else
                    {
                        success = Output.Write(message);
                    }                    
                    break;

                // Accept parameters from control message 
                case FilterControlMessage.SET_PARAMS:
                    if (IsTarget(message))
                    {
                        SetParams(((FilterControlMessage)message).Params);
                    }
                    else
                    {
                        success = Output.Write(message);
                    }
                    break;

                // Accept filter function from control message 
                case FilterControlMessage.SET_FILTER:
                    if (IsTarget(message))
                    {
                        SetFilter(((FilterControlMessage)message).Filter);
                    }
                    else
                    {
                        success = Output.Write(message);
                    }
                    break;

                // Toggle between Filter or Bypass operational modes
                case FilterControlMessage.BYPASS:
                case FilterControlMessage.FILTER:
                    if (IsTarget(message))
                    {
                        Mode = message.Type;
                    }
                    else
                    {
                        success = Output.Write(message);
                    }
                    break;

                // Write control messages for other fittings through
                default:
                    success = Output.Write(message);
                    break;
            }

            return success;
        }

        /// <summary>
        /// Is the message directed at this filter instance?
        /// </summary>
        /// <param name="message">message</param>
        /// <returns></returns>
        protected virtual bool IsTarget(IPipeMessage message)
        {
            return ((FilterControlMessage)message).Name == name;
        }

        /// <summary>
        /// Filter the message.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        protected virtual bool ApplyFilter(IPipeMessage message)
        {
            return filter(message, @params);
        }

        /// <summary>
        /// Set the Filter parameters.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         This can be an object can contain whatever arbitrary 
        ///         properties and values your filter method requires to
        ///         operate.
        ///     </para>
        /// </remarks>
        /// <param name="params">the parameters object</param>
        public void SetParams(object @params)
        {
            this.@params = @params;
        }

        /// <summary>
        /// Set the Filter function.
        /// </summary>
        /// <remarks>
        ///     It must accept an Action receiveing two arguments; 
        ///     an IPipeMessage, and a parameter Object, which can 
        ///     contain whatever arbitrary properties and values your 
        ///     filter method requires.
        /// </remarks>
        /// <param name="value"></param>
        public void SetFilter(Func<IPipeMessage, object, bool> value)
        {
            filter = value;
        }

        /// <summary>Get or set the Filter mode, default is <c>FilterControlMessage.FILTER</c></summary>
        public string Mode { get; set; } = FilterControlMessage.FILTER;

        /// <summary> name of the filter</summary>
        protected string name;

        /// <summary>filter function</summary>
        protected Func<IPipeMessage, object, bool> filter = (message, @params) => { return true; };

        /// <summary> parameters for the filter message</summary>
        protected object @params = new { };
    }
}
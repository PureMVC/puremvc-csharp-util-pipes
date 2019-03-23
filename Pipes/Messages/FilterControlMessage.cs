//
//  PureMVC C# Multicore Utility - Pipes
//
//  Copyright(c) 2017-2027 Saad Shams <saad.shams@puremvc.org>
//  Your reuse is governed by the Creative Commons Attribution 3.0 License
//

using System;
using Pipes.Interfaces;

namespace Pipes.Messages
{
    /// <summary>
    /// Filter Control Message.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         A special message type for controlling the behavior of a Filter.
    ///     </para>
    ///     <para>
    ///         The <c>FilterControlMessage.SET_PARAMS</c> message type tells the Filter
    ///         to retrieve the filter parameters object.
    ///     </para>
    ///     <para>
    ///         The <c>FilterControlMessage.SET_FILTER</c> message type tells the Filter
    ///         to retrieve the filter function.
    ///     </para>
    ///     <para>
    ///         The <c>FilterControlMessage.BYPASS</c> message type tells the Filter
    ///         that it should go into Bypass mode operation, passing all normal
    ///         messages through unfiltered.
    ///     </para>
    ///     <para>
    ///         The <c>FilterControlMessage.FILTER</c> message type tells the Filter
    ///         that it should go into Filtering mode operation, filtering all
    ///         normal normal messages before writing out. This is the default
    ///         mode of operation and so this message type need only be sent to
    ///         cancel a previous <c>FilterControlMessage.BYPASS</c> message.
    ///     </para>
    ///     <para>
    ///         The Filter only acts on a control message if it is targeted 
    ///         to this named filter instance. Otherwise it writes the message
    ///         through to its output unchanged.
    ///     </para>
    /// </remarks>
    public class FilterControlMessage : Message
    {
        /// <summary>Set filter parameters.</summary>
        public const string SET_PARAMS = "http://puremvc.org/namespaces/pipes/messages/filter-control/setParams";

        /// <summary>Set filter function.</summary>
        public const string SET_FILTER = "http://puremvc.org/namespaces/pipes/messages/filter-control/setFilter";

        /// <summary>Toggle to filter bypass mode.</summary>
        public const string BYPASS = "http://puremvc.org/namespaces/pipes/messages/filter-control/bypass";

        /// <summary>Toggle to filtering mode. (default behavior).</summary>
        public const string FILTER = "http://puremvc.org/namespaces/pipes/messages/filter-control/filter";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="filter"></param>
        /// <param name="params"></param>
        public FilterControlMessage(string type, string name, Func<IPipeMessage, object, bool> filter = null, object @params = null) : base(type)
        {
            Name = name;
            Filter = filter;
            Params = @params;
        }

        /// <summary>Get or Set the target filter name.</summary>
        public string Name { get; set; }

        /// <summary>Get or Set the filter function.</summary>
        public Func<IPipeMessage, object, bool> Filter { get; set; }

        /// <summary>Get or Set the parameters object.</summary>
        public object Params { get; set; }
    }
}
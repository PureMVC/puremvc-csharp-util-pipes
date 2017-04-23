//
//  PureMVC C# Multicore Utility - Pipes
//
//  Copyright(c) 2017-2027 Saad Shams <saad.shams@puremvc.org>
//  Your reuse is governed by the Creative Commons Attribution 3.0 License
//

namespace Pipes.Interfaces
{
    /// <summary>
    /// Pipe Message Interface.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         <c>IPipeMessage</c>s are objects written intoto a Pipeline,
    ///         composed of <c>IPipeFitting</c>s. The message is passed from 
    ///         one fitting to the next in syncrhonous fashion.
    ///     </para>
    ///     <para>
    ///         Depending on type, messages may be handled differently by the 
    ///         fittings.
    ///     </para>
    /// </remarks>
    public interface IPipeMessage
    {
        /// <summary>Get or set type of this message</summary>
        string Type { get; set; }

        /// <summary>Get or set header of this message</summary>
        object Header { get; set; }

        /// <summary>Get or set priority of this message</summary>
        int Priority { get; set; }

        /// <summary>Get or set body of this message</summary>
        object Body { get; set; }
    }
}

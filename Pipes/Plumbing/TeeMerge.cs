//
//  PureMVC C# Multicore Utility - Pipes
//
//  Copyright(c) 2017-2027 Saad Shams <saad.shams@puremvc.org>
//  Your reuse is governed by the Creative Commons Attribution 3.0 License
//

using Pipes.Interfaces;

namespace Pipes.Plumbing
{
    /// <summary>
    ///  Merging Pipe Tee.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Writes the messages from multiple input pipelines into
    ///         a single output pipe fitting.
    ///     </para>
    /// </remarks>
    public class TeeMerge: Pipe
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Create the TeeMerge and the two optional constructor inputs.
        ///         This is the most common configuration, though you can connect
        ///         as many inputs as necessary by calling <c>connectInput</c>
        ///         repeatedly.
        ///     </para>
        ///     <para>
        ///         Connect the single output fitting normally by calling the
        ///         <c>connect</c> method, as you would with any other IPipeFitting.
        ///     </para>
        /// </remarks>
        /// <param name="input1">Input pipe</param>
        /// <param name="input2">Input pipe</param>
        public TeeMerge(IPipeFitting input1 = null, IPipeFitting input2 = null)
        {
            if (input1 != null) ConnectInput(input1);
            if (input2 != null) ConnectInput(input2);
        }

        /// <summary>
        /// Connect an input IPipeFitting.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         NOTE: You can connect as many inputs as you want
        ///         by calling this method repeatedly.
        ///     </para>
        /// </remarks>
        /// <param name="input">the IPipeFitting to connect for input.</param>
        /// <returns>true if pipe connected successfully</returns>
        public virtual bool ConnectInput(IPipeFitting input)
        {
            return input.Connect(this);
        }
    }
}

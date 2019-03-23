//
//  PureMVC C# Multicore Utility - Pipes
//
//  Copyright(c) 2017-2027 Saad Shams <saad.shams@puremvc.org>
//  Your reuse is governed by the Creative Commons Attribution 3.0 License
//

using Pipes.Interfaces;
using System.Collections.Generic;

namespace Pipes.Plumbing
{
    /// <summary>
    /// Splitting Pipe Tee.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Writes input messages to multiple output pipe fittings.
    ///     </para>
    /// </remarks>
    public class TeeSplit: IPipeFitting
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Create the TeeSplit and connect the up two optional outputs.
        ///         This is the most common configuration, though you can connect
        ///         as many outputs as necessary by calling <c>connect</c>.
        ///     </para>
        /// </remarks>
        /// <param name="output1">Output pipe</param>
        /// <param name="output2">Output pipe</param>
        public TeeSplit(IPipeFitting output1 = null, IPipeFitting output2 = null)
        {
            if (output1 != null) Connect(output1);
            if (output2 != null) Connect(output2);
        }

        /// <summary>
        /// Connect the output IPipeFitting.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         NOTE: You can connect as many outputs as you want
        ///         by calling this method repeatedly.
        ///     </para>
        /// </remarks>
        /// <param name="output">the IPipeFitting to connect for output.</param>
        /// <returns>true if connect was successful</returns>
        public virtual bool Connect(IPipeFitting output)
        {
            outputs.Add(output);
            return true;
        }

        /// <summary>
        /// Disconnect the most recently connected output fitting. (LIFO)
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         To disconnect all outputs, you must call this 
        ///         method repeatedly untill it returns null.
        ///     </para>
        /// </remarks>
        /// <returns>the disconnected IPipeFitting to connect for output.</returns>
        public virtual IPipeFitting Disconnect()
        {
            if(outputs.Count > 0)
            {
                IPipeFitting pipe = outputs[outputs.Count - 1];
                outputs.RemoveAt(outputs.Count - 1);
                return pipe;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Disconnect a given output fitting.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         If the fitting passed in is connected
        ///         as an output of this <c>TeeSplit</c>, then
        ///         it is disconnected and the reference returned.
        ///     </para>
        ///     <para>
        ///          If the fitting passed in is not connected as an 
        ///          output of this <c>TeeSplit</c>, then <c>null</c>
        ///          is returned.
        ///     </para>
        /// </remarks>
        /// <param name="target">Pipe to disconnect</param>
        /// <returns>the disconnected IPipeFitting to connect for output.</returns>
        public virtual IPipeFitting DisconnectFitting(IPipeFitting target)
        {
            IPipeFitting removed = null;
            int index = outputs.FindIndex(output => output == target);
            if(index != -1)
            {
                removed = outputs[index];
                outputs.RemoveAt(index);
            }
            return removed;
        }

        /// <summary>
        /// Write the message to all connected outputs.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Returns false if any output returns false,
        ///         but all outputs are written to regardless.
        ///     </para>
        /// </remarks>
        /// <param name="message">the message to write</param>
        /// <returns>whether any connected outputs failed</returns>
        public virtual bool Write(IPipeMessage message)
        {
            bool success = true;
            List<IPipeFitting> temp = new List<IPipeFitting>(outputs);
            foreach(IPipeFitting output in temp)
            {
                if (!output.Write(message)) success = false;
            }
            return success;
        }

        /// <summary>Output pipes</summary>
        protected List<IPipeFitting> outputs = new List<IPipeFitting>();
    }
}

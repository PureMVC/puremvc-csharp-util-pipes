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
    /// Pipe.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This is the most basic <c>IPipeFitting</c>,
    ///         simply allowing the connection of an output
    ///         fitting and writing of a message to that output.
    ///     </para>
    /// </remarks>
    public class Pipe : IPipeFitting
    {
        /// <summary>
        /// Constructor with an optional output <c>Pipe</c>
        /// </summary>
        /// <param name="output"></param>
        public Pipe(IPipeFitting output = null)
        {
            if (output != null) Connect(output);
        }

        /// <summary>
        /// Connect another PipeFitting to the output.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         PipeFittings connect to and write to other 
        ///         PipeFittings in a one-way, syncrhonous chain.
        ///     </para>
        /// </remarks>
        /// <param name="output">Pipefitting to connect</param>
        /// <returns>Boolean true if no other fitting was already connected.</returns>
        public virtual bool Connect(IPipeFitting output)
        {
            bool success = false;
            if(Output == null)
            {
                Output = output;
                success = true;
            }
            return success;
        }

        /// <summary>
        /// Disconnect the Pipe Fitting connected to the output.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         This disconnects the output fitting, returning a 
        ///         reference to it. If you were splicing another fitting
        ///         into a pipeline, you need to keep (at least briefly) 
        ///         a reference to both sides of the pipeline in order to 
        ///         connect them to the input and output of whatever 
        ///         fiting that you're splicing in.
        ///     </para>
        /// </remarks>
        /// <returns>IPipeFitting the now disconnected output fitting</returns>
        public virtual IPipeFitting Disconnect()
        {
            IPipeFitting disconnectedFitting = Output;
            Output = null;
            return disconnectedFitting;
        }

        /// <summary>
        /// Write the message to the connected output.
        /// </summary>
        /// <param name="message">the message to writ</param>
        /// <returns>Boolean whether any connected downpipe outputs failed</returns>
        public virtual bool Write(IPipeMessage message)
        {
            return Output.Write(message);
        }

        /// <summary>Output pipe</summary>
        protected IPipeFitting Output { get; private set; }
    }
}

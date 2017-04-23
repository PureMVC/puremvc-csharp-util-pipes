//
//  PureMVC C# Multicore Utility - Pipes
//
//  Copyright(c) 2017-2027 Saad Shams <saad.shams@puremvc.org>
//  Your reuse is governed by the Creative Commons Attribution 3.0 License
//

using Pipes.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Pipes.Plumbing
{
    /// <summary>
    /// Pipe Junction.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Manages Pipes for a Module. 
    ///     </para>
    ///     <para>
    ///         When you register a Pipe with a Junction, it is 
    ///         declared as being an INPUT pipe or an OUTPUT pipe.
    ///     </para>
    ///     <para>
    ///         You can retrieve or remove a registered Pipe by name, 
    ///         check to see if a Pipe with a given name exists,or if 
    ///         it exists AND is an INPUT or an OUTPUT Pipe.
    ///     </para>
    ///     <para>
    ///         You can send an <c>IPipeMessage</c> on a named INPUT Pipe 
    ///         or add a <c>PipeListener</c> to registered INPUT Pipe.
    ///     </para>
    /// </remarks>
    public class Junction
    {
        /// <summary>
        /// Constructor. 
        /// </summary>
        public Junction()
        {
        }

        /// <summary>
        /// Register a pipe with the junction.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Pipes are registered by unique name and type,
        ///         which must be either <c>Junction.INPUT</c>
        ///         or <c>Junction.OUTPUT</c>.
        ///     </para>
        ///     <para>
        ///         NOTE: You cannot have an INPUT pipe and an OUTPUT
        ///         pipe registered with the same name. All pipe names
        ///         must be unique regardless of type.
        ///     </para>
        /// </remarks>
        /// <param name="name">name of the pipe</param>
        /// <param name="type">type (INPUT/OUTPUT) of the pipe</param>
        /// <param name="pipe">Pipefitting</param>
        /// <returns>true if successfully registered. false if another pipe exists by that name.</returns>
        public virtual bool RegisterPipe(string name, string type, IPipeFitting pipe)
        {
            bool success = true;

            if(PipesMaps.TryGetValue(name, out IPipeFitting value) == false && PipesMaps.TryAdd(name, pipe) && PipeTypesMap.TryAdd(name, type))
            {
                switch(type)
                {
                    case INPUT:
                        InputPipes.Add(name);
                        break;
                    case OUTPUT:
                        OutputPipes.Add(name);
                        break;
                    default:
                        success = false;
                        break;
                }
            } else
            {
                success = false;
            }

            return success;
        }

        /// <summary>
        /// Does this junction have a pipe by this name?
        /// </summary>
        /// <param name="name">name the pipe to check for</param>
        /// <returns>Boolean whether as pipe is registered with that name.</returns>
        public virtual bool HasPipe(string name)
        {
            return PipesMaps.ContainsKey(name);
        }

        /// <summary>
        /// Does this junction have an INPUT pipe by this name?
        /// </summary>
        /// <param name="name">name the pipe to check for </param>
        /// <returns>Boolean whether an INPUT pipe is registered with that name.</returns>
        public virtual bool HasInputPipe(string name)
        {
            return HasPipe(name) && PipeTypesMap.TryGetValue(name, out string value) && value == INPUT;
        }

        /// <summary>
        /// Does this junction have an OUTPUT pipe by this name?
        /// </summary>
        /// <param name="name">name the pipe to check for </param>
        /// <returns>Boolean whether an OUTPUT pipe is registered with that name.</returns>
        public virtual bool HasOutputPipe(string name)
        {
            return HasPipe(name) && PipeTypesMap.TryGetValue(name, out string value) && value == OUTPUT;
        }

        /// <summary>
        /// Remove the pipe with this name if it is registered.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         NOTE: You cannot have an INPUT pipe and an OUTPUT
        ///         pipe registered with the same name. All pipe names
        ///         must be unique regardless of type.
        ///     </para>
        /// </remarks>
        /// <param name="name">name the pipe to remove</param>
        public virtual void RemovePipe(string name)
        {
            if(HasPipe(name))
            {
                PipeTypesMap.TryGetValue(name, out string type);
                switch(type)
                {
                    case INPUT:
                        InputPipes.Remove(name);
                        break;
                    case OUTPUT:
                        OutputPipes.Remove(name);
                        break;
                }
                PipesMaps.TryRemove(name, out IPipeFitting pipe);
                PipeTypesMap.TryRemove(name, out string value);
            }
        }

        /// <summary>
        /// Retrieve the named pipe.
        /// </summary>
        /// <param name="name">name the pipe to retrieve</param>
        /// <returns>IPipeFitting the pipe registered by the given name if it exists</returns>
        public virtual IPipeFitting RetrievePipe(string name)
        {
            return PipesMaps.TryGetValue(name, out IPipeFitting pipe) ? pipe : null;
        }

        /// <summary>
        /// Add a PipeListener to an INPUT pipe.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         NOTE: there can only be one PipeListener per pipe,
        ///         and the listener function must accept an IPipeMessage
        ///         as its sole argument.
        ///     </para>
        /// </remarks>
        /// <param name="inputPipeName">name the INPUT pipe to add a PipeListener to</param>
        /// <param name="context">context the calling context or 'this' object </param>
        /// <param name="listener">listener the function on the context to call</param>
        /// <returns></returns>
        public virtual bool AddPipeListener(string inputPipeName, object context, Action<IPipeMessage> listener)
        {
            bool success = false;
            if(HasInputPipe(inputPipeName) && PipesMaps.TryGetValue(inputPipeName, out IPipeFitting pipe))
            {
                success = pipe.Connect(new PipeListener(context, listener));
            }
            return success;
        }

        /// <summary>
        /// Send a message on an OUTPUT pipe.
        /// </summary>
        /// <param name="outputPipeName">name the OUTPUT pipe to send the message on</param>
        /// <param name="message">message the IPipeMessage to send  </param>
        /// <returns>Boolean true if message was sent successfully</returns>
        public virtual bool SendMessage(string outputPipeName, IPipeMessage message)
        {
            bool success = false;
            if(HasOutputPipe(outputPipeName) && PipesMaps.TryGetValue(outputPipeName, out IPipeFitting pipe))
            {
                success = pipe.Write(message);
            }
            return success;
        }

        /// <summary>INPUT Pipe Type</summary>
        public const string INPUT = "input";

        /// <summary>OUTPUT Pipe Type</summary>
        public const string OUTPUT = "output";

        /// <summary>The names of the INPUT pipes</summary>
        protected List<string> InputPipes { get; set; } = new List<string>();

        /// <summary>The names of the OUTPUT pipes</summary>
        protected List<string> OutputPipes { get; set; } = new List<string>();

        /// <summary>The map of pipe names to their pipes</summary>
        protected ConcurrentDictionary<string, IPipeFitting> PipesMaps { get; set; } = new ConcurrentDictionary<string, IPipeFitting>();

        /// <summary>The map of pipe names to their types</summary>
        protected ConcurrentDictionary<string, string> PipeTypesMap { get; set; } = new ConcurrentDictionary<string, string>();
    }
}

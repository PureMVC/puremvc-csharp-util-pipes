//
//  PureMVC C# Multicore Utility - Pipes
//
//  Copyright(c) 2017-2027 Saad Shams <saad.shams@puremvc.org>
//  Your reuse is governed by the Creative Commons Attribution 3.0 License
//

namespace Pipes.Interfaces
{
    /// <summary>
    /// Pipe Aware interface.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Can be implemented by any PureMVC Core that wishes
    ///         to communicate with other Cores using the Pipes
    ///         utility.
    ///     </para>
    /// </remarks>
    public interface IPipeAware
    {
        /// <summary>
        /// Connect input Pipe Fitting.
        /// </summary>
        /// <param name="name">name of the input pipe</param>
        /// <param name="pipe">input Pipe Fitting</param>
        void AcceptInputPipe(string name, IPipeFitting pipe);

        /// <summary>
        /// Connect output Pipe Fitting.
        /// </summary>
        /// <param name="name">name of the input pipe</param>
        /// <param name="pipe">output Pipe Fitting</param>
        void AcceptOutputPipe(string name, IPipeFitting pipe);
    }
}

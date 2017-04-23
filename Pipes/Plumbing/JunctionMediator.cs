//
//  PureMVC C# Multicore Utility - Pipes
//
//  Copyright(c) 2017-2027 Saad Shams <saad.shams@puremvc.org>
//  Your reuse is governed by the Creative Commons Attribution 3.0 License
//

using Pipes.Interfaces;
using PureMVC.Interfaces;
using PureMVC.Patterns.Mediator;

namespace Pipes.Plumbing
{
    /// <summary>
    /// Junction Mediator.
    /// A base class for handling the Pipe Junction in an IPipeAware 
    /// Core.
    /// </summary>
    public class JunctionMediator: Mediator
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="mediatorName"></param>
        /// <param name="viewComponent"></param>
        public JunctionMediator(string mediatorName, object viewComponent): base(mediatorName, viewComponent)
        {
        }

        /// <summary>
        /// List Notification Interests.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Returns the notification interests for this base class.
        ///         Override in subclass and call <c>super.listNotificationInterests</c>
        ///         to get this list, then add any sublcass interests to 
        ///         the array before returning.
        ///     </para>
        /// </remarks>
        /// <returns>string array of notifications</returns>
        public override string[] ListNotificationInterests()
        {
            return new string[]
            {
                JunctionMediator.ACCEPT_INPUT_PIPE,
                JunctionMediator.ACCEPT_OUTPUT_PIPE
            };
        }

        /// <summary>
        /// Handle Notification.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         This provides the handling for common junction activities. It 
        ///         accepts input and output pipes in response to <c>IPipeAware</c>
        ///         interface calls.
        ///     </para>
        ///     <para>
        ///         Override in subclass, and call <c>super.handleNotification</c>
        ///         if none of the subclass-specific notification names are matched.
        ///     </para>
        /// </remarks>
        /// <param name="notification"></param>
        public override void HandleNotification(INotification notification)
        {
            switch(notification.Name)
            {
                // accept an input pipe
                // register the pipe and if successful 
                // set this mediator as its listener
                case JunctionMediator.ACCEPT_INPUT_PIPE:
                    string inputPipeName = notification.Type;
                    IPipeFitting inputPipe = (IPipeFitting)notification.Body;
                    if(Junction.RegisterPipe(inputPipeName, Junction.INPUT, inputPipe))
                    {
                        Junction.AddPipeListener(inputPipeName, this, HandlePipeMessage);
                    }
                    break;

                // accept an output pipe
                case JunctionMediator.ACCEPT_OUTPUT_PIPE:
                    string outputPipeName = notification.Type;
                    IPipeFitting outputPipe = (IPipeFitting)notification.Body;
                    Junction.RegisterPipe(outputPipeName, Junction.OUTPUT, outputPipe);
                    break;
            }
        }

        /// <summary>
        /// Handle incoming pipe messages.
        /// <para>
        ///     Override in subclass and handle messages appropriately for the module.
        /// </para>
        /// </summary>
        /// <param name="message"></param>
        public virtual void HandlePipeMessage(IPipeMessage message)
        {
        }

        /// <summary>
        /// The Junction for this Module.
        /// </summary>
        public Junction Junction { get { return (Junction)ViewComponent; } }

        /// <summary>Accept input pipe notification name constant.</summary>
        public const string ACCEPT_INPUT_PIPE = "acceptInputPipe";

        /// <summary>Accept output pipe notification name constant.</summary>
        public const string ACCEPT_OUTPUT_PIPE = "acceptOutputPipe";
    }
}

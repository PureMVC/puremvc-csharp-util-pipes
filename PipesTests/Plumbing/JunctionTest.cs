//
//  PureMVC C# Multicore Utility - Pipes
//
//  Copyright(c) 2017-2027 Saad Shams <saad.shams@puremvc.org>
//  Your reuse is governed by the Creative Commons Attribution 3.0 License
//

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pipes.Interfaces;
using Pipes.Messages;

namespace Pipes.Plumbing
{
    /// <summary>
    /// Test the Junction class.
    /// </summary>
    [TestClass]
    public class JunctionTest
    {
        /// <summary>
        /// Test registering an INPUT pipe to a junction.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Tests that the INPUT pipe is successfully registered and
        ///         that the hasPipe and hasInputPipe methods work. Then tests
        ///         that the pipe can be retrieved by name.
        ///     </para>
        ///     <para>
        ///         Finally, it removes the registered INPUT pipe and tests
        ///         that all the previous assertions about it's registration
        ///         and accessability via the Junction are no longer true.
        ///     </para>
        /// </remarks>
        [TestMethod]
        public void TestRegisterRetrieveAndRemoveInputPipe()
        {
            // create pipe connected to this test with a pipelistener
            IPipeFitting pipe = new Pipe();

            // create junction
            Junction junction = new Junction();

            // register the pipe with the junction, giving it a name and direction
            bool registered = junction.RegisterPipe("TestInputPipe", Junction.INPUT, pipe);

            // test assertions
            Assert.IsTrue(pipe is Pipe, "Expecting pipe is Pipe");
            Assert.IsTrue(junction is Junction, "Expecting junction is Junction");
            Assert.IsTrue(registered, "Expecting success registering pipe");

            // assertions about junction methods once input  pipe is registered
            Assert.IsTrue(junction.HasPipe("TestInputPipe"), "Expecting junction.HasPipe('TestInputPipe')");
            Assert.IsTrue(junction.HasInputPipe("TestInputPipe"), "Expecting junction.HasInputPipe('TestInputPipe')");
            Assert.IsTrue(junction.RetrievePipe("TestInputPipe") == pipe, "Expecting junction.RetrievePipe('TestInputPipe') == pipe");

            // now remove the pipe and be sure that it is no longer there (same assertions should be false)
            junction.RemovePipe("TestInputPipe");
            Assert.IsFalse(junction.HasPipe("TestInputPipe"), "Expecting junction has pipe");
            Assert.IsFalse(junction.HasInputPipe("TestInputPipe"), "Expecting junction has input pipe");
            Assert.IsFalse(junction.RetrievePipe("TestInputPipe") == pipe, "Expecting pipe retrieved from junction");
        }

        /// <summary>
        /// Test registering an OUTPUT pipe to a junction.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Tests that the OUTPUT pipe is successfully registered and
        ///         that the hasPipe and hasOutputPipe methods work. Then tests
        ///         that the pipe can be retrieved by name.
        ///     </para>
        ///     <para>
        ///         Finally, it removes the registered OUTPUT pipe and tests
        ///         that all the previous assertions about it's registration
        ///         and accessability via the Junction are no longer true.
        ///     </para>
        /// </remarks>
        [TestMethod]
        public void TestRegisterRetrieveAndRemoveOutputPipe()
        {
            // create pipe connected to this test with a pipelistener
            IPipeFitting pipe = new Pipe();

            // create junction
            Junction junction = new Junction();

            // register the pipe with the junction, giving it a name and direction
            bool registered = junction.RegisterPipe("TestOutputPipe", Junction.OUTPUT, pipe);

            // test assertions
            Assert.IsTrue(pipe is Pipe, "Expecting pipe is Pipe");
            Assert.IsTrue(junction is Junction, "Expecting junction is Junction");
            Assert.IsTrue(registered, "Expecting success registering pipe");

            // assertions about junction methods once output pipe is registered
            Assert.IsTrue(junction.HasPipe("TestOutputPipe"), "Expecting junction.HasPipe('TestOutputPipe')");
            Assert.IsTrue(junction.HasOutputPipe("TestOutputPipe"), "Expecting junction.HasInputPipe('TestOutputPipe')");
            Assert.IsTrue(junction.RetrievePipe("TestOutputPipe") == pipe, "Expecting junction.RetrievePipe('TestOutputPipe') == pipe");

            // now remove the pipe and be sure that it is no longer there (same assertions should be false)
            junction.RemovePipe("TestOutputPipe");
            Assert.IsFalse(junction.HasPipe("TestOutputPipe"), "Expecting junction has pipe");
            Assert.IsFalse(junction.HasOutputPipe("TestOutputPipe"), "Expecting junction has input pipe");
            Assert.IsFalse(junction.RetrievePipe("TestOutputPipe") == pipe, "Expecting pipe retrieved from junction");
        }

        /// <summary>
        /// Test adding a PipeListener to an Input Pipe.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Registers an INPUT Pipe with a Junction, then tests
        ///         the Junction's addPipeListener method, connecting
        ///         the output of the pipe back into to the test. If this
        ///         is successful, it sends a message down the pipe and 
        ///         checks to see that it was received.
        ///     </para>
        /// </remarks>
        [TestMethod]
        public void TestAddingPipeListenerToAnInputPipe()
        {
            // create pipe 
            IPipeFitting pipe = new Pipe();

            // create junction
            Junction junction = new Junction();

            // create test message
            IPipeMessage message = new Message(Message.NORMAL, new { testVal = 1 });

            // register the pipe with the junction, giving it a name and direction
            bool registered = junction.RegisterPipe("TestInputPipe", Junction.INPUT, pipe);

            // add the pipelistener using the junction method
            bool listenerAdded = junction.AddPipeListener("TestInputPipe", this, CallBackMethod);

            // send the message using our reference to the pipe, 
            // it should show up in messageReceived property via the pipeListener
            bool sent = pipe.Write(message);

            // test assertions
            Assert.IsTrue(pipe is Pipe, "Expecting pipe is Pipe");
            Assert.IsTrue(junction is Junction, "Expecting junction is Junction");
            Assert.IsTrue(registered, "Expecting success registering pipe");
            Assert.IsTrue(listenerAdded, "Expecting added pipeListener");
            Assert.IsTrue(sent, "Expecting successful write to pipe");
            Assert.IsTrue(messagesReceived.Count == 1, "Expecting 1 message received");
            Assert.IsTrue(messagesReceived[0] == message, "Expecting received message was same instance sent");
            messagesReceived.RemoveAt(0);
        }

        /// <summary>
        /// Test using sendMessage on an OUTPUT pipe.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Creates a Pipe, Junction and Message. 
        ///         Adds the PipeListener to the Pipe.
        ///         Adds the Pipe to the Junction as an OUTPUT pipe.
        ///         uses the Junction's sendMessage method to send
        ///         the Message, then checks that it was received.
        ///     </para>
        /// </remarks>
        [TestMethod]
        public void TestSendMessageOnAnOutputPipe()
        {
            // create pipe 
            IPipeFitting pipe = new Pipe();

            // add a PipeListener manually 
            bool listenerAdded = pipe.Connect(new PipeListener(this, CallBackMethod));

            // create junction
            Junction junction = new Junction();

            // create test message
            IPipeMessage message = new Message(Message.NORMAL, new { testVal = 1 });

            // register the pipe with the junction, giving it a name and direction
            bool registered = junction.RegisterPipe("TestOutputPipe", Junction.OUTPUT, pipe);

            // send the message using the Junction's method 
            // it should show up in messageReceived property via the pipeListener
            bool sent = junction.SendMessage("TestOutputPipe", message);

            // test assertions
            Assert.IsTrue(pipe is Pipe, "Expecting pipe is Pipe");
            Assert.IsTrue(junction is Junction, "Expecting junction is Junction");
            Assert.IsTrue(registered, "Expecting success registering pipe");
            Assert.IsTrue(listenerAdded, "Expecting added pipeListener");
            Assert.IsTrue(sent, "Expecting successful write to pipe");
            Assert.IsTrue(messagesReceived.Count == 1, "Expecting 1 message received");
            Assert.IsTrue(messagesReceived[0] == message, "Expecting received message was same instance sent");
            messagesReceived.RemoveAt(0);
        }

        /// <summary>
        /// Array of received messages.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Used by <code>callBackMedhod</code> as a place to store
        ///         the recieved messages.
        ///     </para>
        /// </remarks>
        private List<IPipeMessage> messagesReceived = new List<IPipeMessage>();

        /// <summary>
        /// Callback given to <code>PipeListener</code> for incoming message.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Used by <code>testReceiveMessageViaPipeListener</code> 
        ///         to get the output of pipe back into this  test to see 
        ///         that a message passes through the pipe.
        ///     </para>
        /// </remarks>
        /// <param name="message"></param>
        private void CallBackMethod(IPipeMessage message)
        {
            messagesReceived.Add(message);
        }
    }
}

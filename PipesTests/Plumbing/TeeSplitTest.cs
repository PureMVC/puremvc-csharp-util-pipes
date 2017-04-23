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
    /// Test the TeeSplit class.
    /// </summary>
    [TestClass]
    public class TeeSplitTest
    {
        /// <summary>
        /// Test connecting and disconnecting I/O Pipes.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Connect an input and several output pipes to a splitting tee. 
        ///         Then disconnect all outputs in LIFO order by calling disconnect 
        ///         repeatedly.
        ///     </para>
        /// </remarks>
        [TestMethod]
        public void TestConnectingAndDisconnectingIOPipes()
        {
            // create input pipe
            IPipeFitting input1 = new Pipe();

            // create output pipes 1, 2, 3 and 4
            IPipeFitting pipe1 = new Pipe();
            IPipeFitting pipe2 = new Pipe();
            IPipeFitting pipe3 = new Pipe();
            IPipeFitting pipe4 = new Pipe();

            // create splitting tee (args are first two output fittings of tee)
            TeeSplit teeSplit = new TeeSplit(pipe1, pipe2);

            // connect 2 extra outputs for a total of 4
            bool connectedExtra1 = teeSplit.Connect(pipe3);
            bool connectedExtra2 = teeSplit.Connect(pipe4);

            // connect the single input
            bool inputConnected = input1.Connect(teeSplit);

            // test assertions
            Assert.IsTrue(pipe1 is Pipe, "Expecting pipe1 is Pipe");
            Assert.IsTrue(pipe2 is Pipe, "Expecting pipe2 is Pipe");
            Assert.IsTrue(pipe3 is Pipe, "Expecting pipe3 is Pipe");
            Assert.IsTrue(pipe4 is Pipe, "Expecting pipe4 is Pipe");
            Assert.IsTrue(teeSplit is TeeSplit, "Expecting teeSplit is TeeSplit");
            Assert.IsTrue(connectedExtra1, "Expecting connected pipe 3");
            Assert.IsTrue(connectedExtra2, "Expecting connected pipe 4");

            // test LIFO order of output disconnection
            Assert.IsTrue(teeSplit.Disconnect() == pipe4, "Expecting disconnected pipe 4");
            Assert.IsTrue(teeSplit.Disconnect() == pipe3, "Expecting disconnected pipe 3");
            Assert.IsTrue(teeSplit.Disconnect() == pipe2, "Expecting disconnected pipe 2");
            Assert.IsTrue(teeSplit.Disconnect() == pipe1, "Expecting disconnected pipe 4");
            Assert.IsTrue(teeSplit.Disconnect() == null, "Expecting null");
        }

        /// <summary>
        /// Test disconnectFitting method.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Connect several output pipes to a splitting tee.
        ///         Then disconnect specific outputs, making sure that once
        ///         a fitting is disconnected using disconnectFitting, that
        ///         it isn't returned when disconnectFitting is called again. 
        ///         Finally, make sure that the when a message is sent to 
        ///         the tee that the correct number of output messages is
        ///         written.
        ///     </para>
        /// </remarks>
        [TestMethod]
        public void TestDisconnectFitting()
        {
            messagesReceived = new List<IPipeMessage>();

            // create input pipe
            IPipeFitting input1 = new Pipe();

            // create output pipes 1, 2, 3 and 4
            IPipeFitting pipe1 = new Pipe();
            IPipeFitting pipe2 = new Pipe();
            IPipeFitting pipe3 = new Pipe();
            IPipeFitting pipe4 = new Pipe();

            // setup pipelisteners 
            pipe1.Connect(new PipeListener(this, CallBackMethod));
            pipe2.Connect(new PipeListener(this, CallBackMethod));
            pipe3.Connect(new PipeListener(this, CallBackMethod));
            pipe4.Connect(new PipeListener(this, CallBackMethod));

            // create splitting tee
            TeeSplit teeSplit = new TeeSplit();

            // add outputs
            teeSplit.Connect(pipe1);
            teeSplit.Connect(pipe2);
            teeSplit.Connect(pipe3);
            teeSplit.Connect(pipe4);

            // test assertions
            Assert.IsTrue(teeSplit.DisconnectFitting(pipe4) == pipe4, "Expecting teeSplit.DisconnectFitting(pipe4) == pipe4");
            Assert.IsTrue(teeSplit.DisconnectFitting(pipe4) == null, "Expecting teeSplit.DisconnectFitting(pipe4) == null");

            // Write a message to the tee 
            teeSplit.Write(new Message(Message.NORMAL));

            // test assertions 	
            Assert.IsTrue(messagesReceived.Count == 3, "Expecting messagesReceived.Count == 3");
        }

        /// <summary>
        /// Test receiving messages from two pipes using a TeeMerge.
        /// </summary>
        [TestMethod]
        public void TestReceiveMessagesFromTwoTeeSplitOutputs()
        {
            messagesReceived = new List<IPipeMessage>();

            // create a message to send on pipe 1
            IPipeMessage message = new Message(Message.NORMAL, new { testProp = 1 });

            // create output pipes 1 and 2
            IPipeFitting pipe1 = new Pipe();
            IPipeFitting pipe2 = new Pipe();

            // create and connect anonymous listeners
            bool connected1 = pipe1.Connect(new PipeListener(this, CallBackMethod));
            bool connected2 = pipe2.Connect(new PipeListener(this, CallBackMethod));

            // create splitting tee (args are first two output fittings of tee)
            TeeSplit teeSplit = new TeeSplit(pipe1, pipe2);

            // write messages to their respective pipes
            bool written = teeSplit.Write(message);

            // test assertions
            Assert.IsTrue(message is IPipeMessage, "Expecting message is IPipeMessage");
            Assert.IsTrue(pipe1 is Pipe, "Expecting pipe1 is Pipe");
            Assert.IsTrue(pipe2 is Pipe, "Expecting pipe2 is Pipe");
            Assert.IsTrue(teeSplit is TeeSplit, "Expecting teeSplit is TeeSplit");
            Assert.IsTrue(connected1, "Expecting connected anonymous listener to pipe 1");
            Assert.IsTrue(connected2, "Expecting connected anonymous listener to pipe 2");
            Assert.IsTrue(written, "Expecting wrote single message to tee");

            // test that both messages were received, then test
            // FIFO order by inspecting the messages themselves
            Assert.IsTrue(messagesReceived.Count == 2, "Expecting received 2 messages");

            // test message 1 assertions 
            IPipeMessage message1 = messagesReceived[0];
            messagesReceived.RemoveAt(0);
            Assert.IsTrue(message1 is IPipeMessage, "Expecting message1 is IPipeMessage");
            Assert.IsTrue(message1 == message, "Expecting message1 == message");

            // test message 2 assertions 
            IPipeMessage message2 = messagesReceived[0];
            messagesReceived.RemoveAt(0);
            Assert.IsTrue(message2 is IPipeMessage, "Expecting message2 is IPipeMessage");
            Assert.IsTrue(message2 == message, "Expecting message2 == message");
        }

        /// <summary>
        /// Array of received messages.
        /// <para>
        ///     Used by <code>callBackMedhod</code> as a place to store
        ///     the recieved messages.
        /// </para>
        private List<IPipeMessage> messagesReceived = new List<IPipeMessage>();

        /// <summary>
        /// Callback given to <code>PipeListener</code> for incoming message.
        /// <para>
        ///     Used by <code>testReceiveMessageViaPipeListener</code> 
        ///     to get the output of pipe back into this  test to see 
        ///     that a message passes through the pipe.
        /// </para>
        /// </summary>
        /// <param name="message">message</param>
        private void CallBackMethod(IPipeMessage message)
        {
            messagesReceived.Add(message);
        }
    }
}

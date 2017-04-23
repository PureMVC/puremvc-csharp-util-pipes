//
//  PureMVC C# Multicore Utility - Pipes
//
//  Copyright(c) 2017-2027 Saad Shams <saad.shams@puremvc.org>
//  Your reuse is governed by the Creative Commons Attribution 3.0 License
//

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pipes.Interfaces;
using Pipes.Messages;
using System.Xml;
using System.Collections.Generic;

namespace Pipes.Plumbing
{
    /// <summary>
    /// Test the TeeMerge class.
    /// </summary>
    [TestClass]
    public class TeeMergeTest
    {
        XmlDocument xmlDocument = new XmlDocument();

        /// <summary>
        /// Constructor.
        /// </summary>
        public TeeMergeTest()
        {
            xmlDocument.LoadXml("<message testAtt='Pipe 1 Message'/>");
        }

        /// <summary>
        /// Test connecting an output and several input pipes to a merging tee. 
        /// </summary>
        [TestMethod]
        public void TestConnectingIOPipes()
        {
            // create input pipe
            IPipeFitting output1 = new Pipe();

            // create input pipes 1, 2, 3 and 4
            IPipeFitting pipe1 = new Pipe();
            IPipeFitting pipe2 = new Pipe();
            IPipeFitting pipe3 = new Pipe();
            IPipeFitting pipe4 = new Pipe();

            // create splitting tee (args are first two input fittings of tee)
            TeeMerge teeMerge = new TeeMerge(pipe1, pipe2);

            // connect 2 extra inputs for a total of 4
            bool connectedExtra1 = teeMerge.ConnectInput(pipe3);
            bool connectedExtra2 = teeMerge.ConnectInput(pipe4);

            // connect the single output
            bool connected = output1.Connect(teeMerge);

            // test assertions
            Assert.IsTrue(pipe1 is Pipe, "Expecting pipe1 is Pipe");
            Assert.IsTrue(pipe2 is Pipe, "Expecting pipe2 is Pipe");
            Assert.IsTrue(pipe3 is Pipe, "Expecting pipe3 is Pipe");
            Assert.IsTrue(pipe4 is Pipe, "Expecting pipe4 is Pipe");
            Assert.IsTrue(teeMerge is TeeMerge, "teeMerge is TeeMerge");
            Assert.IsTrue(connectedExtra1, "Expected connected extra input 1");
            Assert.IsTrue(connectedExtra2, "Expected connected extra input 2");
        }

        /// <summary>
        /// Test receiving messages from two pipes using a TeeMerge.
        /// </summary>
        [TestMethod]
        public void TestReceiveMessagesFromTwoPipesViaTeeMerge()
        {
            // create a message to send on pipe 1
            IPipeMessage pipe1Message = new Message(Message.NORMAL, new { testProp = 1 }, xmlDocument, Message.PRIORITY_LOW);

            // create a message to send on pipe 2
            IPipeMessage pipe2Message = new Message(Message.NORMAL, new { testProp = 2 }, xmlDocument, Message.PRIORITY_HIGH);

            // create pipes 1 and 2
            IPipeFitting pipe1 = new Pipe();
            IPipeFitting pipe2 = new Pipe();

            // create merging tee (args are first two input fittings of tee)
            TeeMerge teeMerge = new TeeMerge(pipe1, pipe2);

            // create listener
            PipeListener listener = new PipeListener(this, CallBackMethod);

            // connect the listener to the tee and write the messages
            bool connected = teeMerge.Connect(listener);

            // write messages to their respective pipes
            bool pipe1written = pipe1.Write(pipe1Message);
            bool pipe2written = pipe2.Write(pipe2Message);

            // test assertions
            Assert.IsTrue(pipe1Message is IPipeMessage, "Expecting pipe1Message is IPipeMessage");
            Assert.IsTrue(pipe2Message is IPipeMessage, "Expecting pipe2Message is IPipeMessage");
            Assert.IsTrue(pipe1 is Pipe, "Expecting pipe1 is Pipe");
            Assert.IsTrue(pipe2 is Pipe, "Expecting pipe2 is Pipe");
            Assert.IsTrue(teeMerge is TeeMerge, "Expecting teeMerge is TeeMerge");
            Assert.IsTrue(listener is PipeListener, "Expecting listener is PipeListener");
            Assert.IsTrue(connected, "Expecting connected listener to merging tee");
            Assert.IsTrue(pipe1written, "Expecting wrote message to pipe1");
            Assert.IsTrue(pipe2written, "Expecting wrote message to pipe2");

            // test that both messages were received, then test
            // FIFO order by inspecting the messages themselves
            Assert.IsTrue(messagesReceived.Count == 2, "Expecting received 2 messages");

            // test message 1 assertions 
            IPipeMessage message1 = messagesReceived[0];
            messagesReceived.RemoveAt(0);
            Assert.IsTrue(message1 is IPipeMessage, "Expecing message1 is IPipeMessage");
            Assert.IsTrue(message1 == pipe1Message, "Expecting message1 == pipe1Message");
            Assert.IsTrue(message1.Type == Message.NORMAL, "Expecting message1.Type == Message.NORMAL");
            Assert.IsTrue(((dynamic)message1.Header).testProp == 1, "Expecting ((dynamic)message1.Header).testProp");
            Assert.IsTrue(message1.Priority == Message.PRIORITY_LOW, "Expecting message1.Priority == Message.PRIORITY_LOW");

            // test message 2 assertions
            IPipeMessage message2 = messagesReceived[0];
            messagesReceived.RemoveAt(0);
            Assert.IsTrue(message2 is IPipeMessage, "Expecing message2 is IPipeMessage");
            Assert.IsTrue(message2 == pipe2Message, "Expecting message2 == pipe1Message");
            Assert.IsTrue(message2.Type == Message.NORMAL, "Expecting message2.Type == Message.NORMAL");
            Assert.IsTrue(((dynamic)message2.Header).testProp == 2, "Expecting ((dynamic)message2.Header).testProp");
            Assert.IsTrue(message2.Priority == Message.PRIORITY_HIGH, "Expecting message1.Priority == Message.PRIORITY_HIGH");
        }

        /// <summary>
        /// Test receiving messages from four pipes using a TeeMerge.
        /// </summary>
        [TestMethod]
        public void TestReceiveMessagesFromFourPipesViaTeeMerge()
        {
            // create a message to send on pipe 1
            IPipeMessage pipe1Message = new Message(Message.NORMAL, new { testProp = 1 });
            IPipeMessage pipe2Message = new Message(Message.NORMAL, new { testProp = 2 });
            IPipeMessage pipe3Message = new Message(Message.NORMAL, new { testProp = 3 });
            IPipeMessage pipe4Message = new Message(Message.NORMAL, new { testProp = 4 });

            // create pipes 1, 2, 3 and 4
            IPipeFitting pipe1 = new Pipe();
            IPipeFitting pipe2 = new Pipe();
            IPipeFitting pipe3 = new Pipe();
            IPipeFitting pipe4 = new Pipe();

            // create merging tee
            TeeMerge teeMerge = new TeeMerge(pipe1, pipe2);
            bool connectedExtraInput3 = teeMerge.ConnectInput(pipe3);
            bool connectedExtraInput4 = teeMerge.ConnectInput(pipe4);

            // create listener
            PipeListener listener = new PipeListener(this, CallBackMethod);

            // connect the listener to the tee and write the messages
            bool connected = teeMerge.Connect(listener);

            // write messages to their respective pipes
            bool pipe1written = pipe1.Write(pipe1Message);
            bool pipe2written = pipe2.Write(pipe2Message);
            bool pipe3written = pipe3.Write(pipe3Message);
            bool pipe4written = pipe4.Write(pipe4Message);

            // test assertions
            Assert.IsTrue(pipe1Message is IPipeMessage);
            Assert.IsTrue(pipe2Message is IPipeMessage);
            Assert.IsTrue(pipe3Message is IPipeMessage);
            Assert.IsTrue(pipe4Message is IPipeMessage);
            Assert.IsTrue(pipe1 is Pipe);
            Assert.IsTrue(pipe2 is Pipe);
            Assert.IsTrue(pipe3 is Pipe);
            Assert.IsTrue(pipe4 is Pipe);
            Assert.IsTrue(teeMerge is TeeMerge, "Expecting teeMerge is TeeMerge");
            Assert.IsTrue(listener is PipeListener, "Expecing listner is PipeListener");
            Assert.IsTrue(connected, "Expecting connected listener to merging tee");
            Assert.IsTrue(connectedExtraInput3, "Expecting connected extra input 3 to merging tee");
            Assert.IsTrue(connectedExtraInput4, "Expecting connected extra input 4 to merging tee");
            Assert.IsTrue(pipe1written, "Expecting wrote message to pipe 1");
            Assert.IsTrue(pipe2written, "Expecting wrote message to pipe 2");
            Assert.IsTrue(pipe3written, "Expecting wrote message to pipe 3");
            Assert.IsTrue(pipe4written, "Expecting wrote message to pipe 4");

            // test that both messages were received, then test
            // FIFO order by inspecting the messages themselves
            Assert.IsTrue(messagesReceived.Count == 4, "Expecting received 4 messages");

            // test message 1 assertions
            IPipeMessage message1 = messagesReceived[0];
            messagesReceived.RemoveAt(0);
            Assert.IsTrue(message1 is IPipeMessage, "Expecting message1 is IPipeMessage");
            Assert.IsTrue(message1 == pipe1Message, "Expecing message1 == pipe1Message");
            Assert.IsTrue(message1.Type == Message.NORMAL, "Expecting message1.Type == Message.NORMAL");
            Assert.IsTrue(((dynamic)message1.Header).testProp == 1, "Expecting ((dynamic)message1.Header).testProp == 1");

            // test message 2 assertions
            IPipeMessage message2 = messagesReceived[0];
            messagesReceived.RemoveAt(0);
            Assert.IsTrue(message2 is IPipeMessage, "Expecting message2 is IPipeMessage");
            Assert.IsTrue(message2 == pipe2Message, "Expecing message2 == pipe1Message");
            Assert.IsTrue(message2.Type == Message.NORMAL, "Expecting message2.Type == Message.NORMAL");
            Assert.IsTrue(((dynamic)message2.Header).testProp == 2, "Expecting ((dynamic)message2.Header).testProp == 2");

            // test message 3 assertions
            IPipeMessage message3 = messagesReceived[0];
            messagesReceived.RemoveAt(0);
            Assert.IsTrue(message3 is IPipeMessage, "Expecting message3 is IPipeMessage");
            Assert.IsTrue(message3 == pipe3Message, "Expecing message3 == pipe1Message");
            Assert.IsTrue(message3.Type == Message.NORMAL, "Expecting message3.Type == Message.NORMAL");
            Assert.IsTrue(((dynamic)message3.Header).testProp == 3, "Expecting ((dynamic)message3.Header).testProp == 3");

            // test message 4 assertions
            IPipeMessage message4 = messagesReceived[0];
            messagesReceived.RemoveAt(0);
            Assert.IsTrue(message4 is IPipeMessage, "Expecting message4 is IPipeMessage");
            Assert.IsTrue(message4 == pipe4Message, "Expecing message4 == pipe1Message");
            Assert.IsTrue(message4.Type == Message.NORMAL, "Expecting message4.Type == Message.NORMAL");
            Assert.IsTrue(((dynamic)message4.Header).testProp == 4, "Expecting ((dynamic)message4.Header).testProp == 4");
        }

        /// <summary>
        /// Array of received messages.
        /// <para>
        ///     Used by <code>callBackMedhod</code> as a place to store
        ///     the recieved messages.
        /// </para>
        /// </summary>
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

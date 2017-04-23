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

namespace Pipes.Plumbing
{
    /// <summary>
    /// Test the PipeListener class.
    /// </summary>
    [TestClass]
    public class PipeListenerTest
    {
        /// <summary>
        /// Test connecting a pipe listener to a pipe. 
        /// </summary>
        [TestMethod]
        public void TestConnectingToAPipe()
        {
            // create pipe and listener
            IPipeFitting pipe = new Pipe();
            PipeListener listener = new PipeListener(this, CallBackMethod);

            // connect the listener to the pipe
            bool success = pipe.Connect(listener);

            // test assertions
            Assert.IsTrue(pipe is Pipe, "pipe is Pipe");
            Assert.IsTrue(success, "Expecting successfully connected listener to pipe");
        }

        /// <summary>
        /// Test receiving a message from a pipe using a PipeListener.
        /// </summary>
        [TestMethod]
        public void TestReceiveMessageViaPipeListener()
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml("<message att='Hello'/>");

            // create a message
            IPipeMessage messageToSend = new Message(Message.NORMAL, new { testProp = "testVal" }, xmlDocument, Message.PRIORITY_HIGH);

            // create pipe and listener
            IPipeFitting pipe = new Pipe();
            PipeListener listener = new PipeListener(this, CallBackMethod);

            // connect the listener to the pipe and write the message
            bool connected = pipe.Connect(listener);
            bool written = pipe.Write(messageToSend);

            // test assertions
            Assert.IsTrue(pipe is Pipe, "pipe is Pipe");
            Assert.IsTrue(connected, "Expecting connected listener to pipe");
            Assert.IsTrue(written, "Expecting wrote message to pipe");
            Assert.IsTrue(messageReceived is Message, "Expecting messageReceived is Message");
            Assert.IsTrue(messageReceived.Type == Message.NORMAL, "Expecting messageReceived.Type == Message.NORMAL");
            Assert.IsTrue(((dynamic)messageReceived.Header).testProp == "testVal", "Expecting messageReceived.Header.testProp == 'testVal'");
            Assert.IsTrue(((XmlDocument)messageReceived.Body).DocumentElement.Attributes["att"].InnerText.Equals("Hello"), "Expecting ((XmlDocument)messageReceived.Body).DocumentElement.Attributes['att'].InnerText.Equals('Hello')");
            Assert.IsTrue(messageReceived.Priority == Message.PRIORITY_HIGH, "Expecting messageReceived.Priority == Message.PRIORITY_HIGH");
        }

        /// <summary>
        /// Recipient of message.
        /// <para>
        ///     Used by <code>callBackMedhod</code> as a place to store
        ///     the recieved message.
        /// </para>
        /// </summary>
        private IPipeMessage messageReceived;

        /// <summary>
        /// Callback given to <code>PipeListener</code> for incoming message.
        /// <para>
        ///     Used by <code>testReceiveMessageViaPipeListener</code> 
        ///     to get the output of pipe back into this  test to see 
        ///     that a message passes through the pipe.
        /// </para>
        /// </summary>
        /// <param name="message"></param>
        private void CallBackMethod(IPipeMessage message)
        {
            messageReceived = message;
        }

    }
}

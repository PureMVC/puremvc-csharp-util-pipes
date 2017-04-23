//
//  PureMVC C# Multicore Utility - Pipes
//
//  Copyright(c) 2017-2027 Saad Shams <saad.shams@puremvc.org>
//  Your reuse is governed by the Creative Commons Attribution 3.0 License
//

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pipes.Interfaces;
using Pipes.Messages;
using Pipes.Plumbing;

namespace PipesTest.Plumbing.PipeTest
{
    /// <summary>
    /// Test the Filter class.
    /// </summary>
    [TestClass]
    public class FilterTest
    {
        /// <summary>
        /// Test connecting input and output pipes to a filter as well as disconnecting the output.
        /// </summary>
        [TestMethod]
        public void TestConnectingAndDisconnectingIOPipes()
        {
            // create output pipes 1
            IPipeFitting pipe1 = new Pipe();
            IPipeFitting pipe2 = new Pipe();

            // create filter
            IPipeFitting filter = new Filter("TestFilter");

            // connect input fitting
            bool connectedInput = pipe1.Connect(filter);

            // connect output fitting
            bool connectedOutput = filter.Connect(pipe2);

            // test assertions
            Assert.IsTrue(pipe1 is Pipe, "pipe1 is Pipe");
            Assert.IsTrue(pipe2 is Pipe, "pipe2 is Pipe");
            Assert.IsTrue(filter is Filter, "Expecting filter is Filter");
            Assert.IsTrue(connectedInput, "Expecting connected input");
            Assert.IsTrue(connectedOutput, "Expected connected output");

            // disconnect pipe 2 from filter
            IPipeFitting disconnectedPipe = filter.Disconnect();
            Assert.IsTrue(disconnectedPipe == pipe2, "Expecting disconnected pipe2 from filter");
        }

        /// <summary>
        /// Test applying filter to a normal message.
        /// </summary>
        [TestMethod]
        public void TestFilteringNormalMessage()
        {
            // create messages to send to the queue
            IPipeMessage message = new Message(Message.NORMAL, new { width = 10, height = 2 });

            // create filter, attach an anonymous listener to the filter output to receive the message,
            // pass in an anonymous function an parameter object
            IPipeFitting filter = new Filter("scale", new PipeListener(this, CallBackMethod), (IPipeMessage msg, dynamic _params) => {
                msg.Header = new
                {
                    width = ((dynamic)msg.Header).width * ((dynamic)_params).factor,
                    height = ((dynamic)msg.Header).height * ((dynamic)_params).factor
                };
                return true;
            }, new { factor = 10 });

            // write messages to the filter
            bool written = filter.Write(message);

            // test assertions
            Assert.IsTrue(message is IPipeMessage, "Expecting message is IPipeMessage");
            Assert.IsTrue(filter is Filter, "Expecting filter is Filter");
            Assert.IsTrue(written, "Expecting wrote normal message to filter");
            Assert.IsTrue(messagesReceived.Count == 1, "Expecting received 1 messages");

            // test filtered message assertions 
            IPipeMessage received = messagesReceived[0];
            messagesReceived.RemoveAt(0);
            Assert.IsTrue(received is IPipeMessage, "Expecting received is IPipeMessage");
            Assert.IsTrue(received == message, "Expecting received == message");
            Assert.IsTrue(((dynamic)received.Header).width == 100, "Expecting ((dynamic)received.Header).width == 100");
            Assert.IsTrue(((dynamic)received.Header).height == 20, "Expecting ((dynamic)received.Header).height == 20");
        }

        /// <summary>
        /// Test setting filter to bypass mode, writing, then setting back to filter mode and writing. 
        /// </summary>
        [TestMethod]
        public void TestBypassAndFilterModeToggle()
        {
            // create messages to send to the queue
            IPipeMessage message = new Message(Message.NORMAL, new { width = 10, height = 2 });

            // create filter, attach an anonymous listener to the filter output to receive the message,
            // pass in an anonymous function an parameter object
            IPipeFitting filter = new Filter("scale", new PipeListener(this, CallBackMethod), (IPipeMessage msg, object _params) => {
                msg.Header = new
                {
                    width = ((dynamic)msg.Header).width * ((dynamic)_params).factor,
                    height = ((dynamic)msg.Header).height * ((dynamic)_params).factor
                };
                return true;
            }, new { factor = 10 });

            // create bypass control message
            IPipeMessage byPassMessage = new FilterControlMessage(FilterControlMessage.BYPASS, "scale");

            // write bypass control message to the filter
            bool bypassWritten = filter.Write(byPassMessage);

            // write normal message to the filter
            bool written1 = filter.Write(message);

            // test assertions
            Assert.IsTrue(message is IPipeMessage, "Expecting message is IPipeMessage");
            Assert.IsTrue(filter is Filter, "Expecting filter is Filter");
            Assert.IsTrue(bypassWritten, "Expecting wrote bypass message to filter");
            Assert.IsTrue(written1, "Expecting wrote normal message to filter");
            Assert.IsTrue(messagesReceived.Count == 1, "Expecting received 1 messages");

            // test filtered message assertions (no change to message)
            IPipeMessage received1 = messagesReceived[0];
            messagesReceived.RemoveAt(0);
            Assert.IsTrue(received1 == message, "Expecting received == message");
            Assert.IsTrue(((dynamic)received1.Header).width == 10, "Expecting ((dynamic)received1.Header).width == 10");
            Assert.IsTrue(((dynamic)received1.Header).height == 2, "((dynamic)received1.Header).height == 2");

            // create filter control message
            IPipeMessage filterMessage = new FilterControlMessage(FilterControlMessage.FILTER, "scale");

            // write bypass control message to the filter
            bool filterWritten = filter.Write(filterMessage);

            // write normal message to the filter again
            bool written2 = filter.Write(message);

            // test assertions  
            Assert.IsTrue(filterWritten, "Expecing worte filter message to filter");
            Assert.IsTrue(written2, "Expecting wrote normal message to filter");
            Assert.IsTrue(messagesReceived.Count == 1, "Expecing received 1 messages");

            // test filtered message assertions (message filtered)
            IPipeMessage received2 = messagesReceived[0];
            messagesReceived.RemoveAt(0);
            Assert.IsTrue(received2 is IPipeMessage, "Expecting received 2 is IPipeMessage");
            Assert.IsTrue(received2 == message, "received2 == message");
            Assert.IsTrue(((dynamic)received2.Header).width == 100, "Expecting ((dynamic)received2.Header).width == 100");
            Assert.IsTrue(((dynamic)received2.Header).height == 20, "Expecting ((dynamic)received2.Header).height == 20");
        }

        /// <summary>
        /// Test setting filter parameters by sending control message. 
        /// </summary>
        [TestMethod]
        public void TestSetParamsByControlMessage()
        {
            // create messages to send to the queue
            IPipeMessage message = new Message(Message.NORMAL, new { width = 10, height = 2 });

            // create filter, attach an anonymous listener to the filter output to receive the message,
            // pass in an anonymous function an parameter object
            IPipeFitting filter = new Filter("scale", new PipeListener(this, CallBackMethod), (IPipeMessage msg, object _params) => {
                msg.Header = new
                {
                    width = ((dynamic)msg.Header).width * ((dynamic)_params).factor,
                    height = ((dynamic)msg.Header).height * ((dynamic)_params).factor
                };
                return true;
            }, new { factor = 10 });

            // create setParams control message
            IPipeMessage setParamsMessage = new FilterControlMessage(FilterControlMessage.SET_PARAMS, "scale", null, new { factor = 5 });

            // write filter control message to the filter
            bool setParamsWritten = filter.Write(setParamsMessage);

            // write normal message to the filter
            bool written = filter.Write(message);

            // test assertions
            Assert.IsTrue(message is IPipeMessage, "Expecting message is IPipeMessage");
            Assert.IsTrue(filter is Filter, "Expecting filter is Filter");
            Assert.IsTrue(setParamsWritten, "Expecting wrote set_params message to filter");
            Assert.IsTrue(messagesReceived.Count == 1, "Expecting received 1 messages");

            // test filtered message assertions (message filtered with overridden parameters)
            IPipeMessage received1 = messagesReceived[0];
            messagesReceived.RemoveAt(0);
            Assert.IsTrue(received1 is IPipeMessage, "Expecting received 2 is IPipeMessage");
            Assert.IsTrue(received1 == message, "received2 == message");
            Assert.IsTrue(((dynamic)received1.Header).width == 50, "Expecting ((dynamic)received2.Header).width == 50");
            Assert.IsTrue(((dynamic)received1.Header).height == 10, "Expecting ((dynamic)received2.Header).height == 10");
        }

        /// <summary>
        /// Test setting filter function by sending control message. 
        /// </summary>
        [TestMethod]
        public void TestSetFilterByControlMessage()
        {
            // create messages to send to the queue
            IPipeMessage message = new Message(Message.NORMAL, new { width = 100, height = 20 });

            // create filter, attach an anonymous listener to the filter output to receive the message,
            // pass in an anonymous function and an anonymous parameter object
            IPipeFitting filter = new Filter("scale", new PipeListener(this, CallBackMethod), (IPipeMessage msg, object _params) => {
                msg.Header = new
                {
                    width = ((dynamic)msg.Header).width * ((dynamic)_params).factor,
                    height = ((dynamic)msg.Header).height * ((dynamic)_params).factor
                };
                return true;
            }, new { factor = 10 });

            // create setFilter control message	
            IPipeMessage setFilterMessage = new FilterControlMessage(FilterControlMessage.SET_FILTER, "scale", (IPipeMessage msg, object _params) =>
            {
                msg.Header = new
                {
                    width = ((dynamic)msg.Header).width / ((dynamic)_params).factor,
                    height = ((dynamic)msg.Header).height / ((dynamic)_params).factor
                };
                return true;
            });

            // write filter control message to the filter
            bool setFilterWritten = filter.Write(setFilterMessage);

            // write normal message to the filter
            bool written = filter.Write(message);

            // test assertions
            Assert.IsTrue(message is IPipeMessage, "Expecting message is IPipeMessage");
            Assert.IsTrue(filter is Filter, "Expecting filter is Filter");
            Assert.IsTrue(setFilterWritten, "Expecting wrote message to filter");
            Assert.IsTrue(written, "Expecting wrote normal message to filter");
            Assert.IsTrue(messagesReceived.Count == 1, "Expecting received 1 messages");

            // test filtered message assertions (message filtered with overridden filter function)
            IPipeMessage received = messagesReceived[0];
            messagesReceived.RemoveAt(0);
            Assert.IsTrue(received is Message, "Expecting received is Message");
            Assert.IsTrue(received == message, "Expecting received == message");
            Assert.IsTrue(((dynamic)received.Header).width == 10, "Expecting ((dynamic)received2.Header).width == 10");
            Assert.IsTrue(((dynamic)received.Header).height == 2, ((dynamic)received.Header).height + "Expecting ((dynamic)received2.Header).height == 2");
        }

        /// <summary>
        /// Test using a filter function to stop propagation of a message. 
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         The way to stop propagation of a message from within a filter
        ///         is to throw an error from the filter function. This test creates
        ///         two NORMAL messages, each with header objects that contain 
        ///         a <code>bozoLevel</code> property. One has this property set to 
        ///         10, the other to 3.
        ///     </para>
        ///     <para>
        ///         Creates a Filter, named 'bozoFilter' with an anonymous pipe listener
        ///         feeding the output back into this test. The filter funciton is an 
        ///         anonymous function that throws an error if the message's bozoLevel 
        ///         property is greater than the filter parameter <code>bozoThreshold</code>.
        ///         the anonymous filter parameters object has a <code>bozoThreshold</code>
        ///         value of 5.
        ///     </para>
        ///     <para>
        ///         The messages are written to the filter and it is shown that the 
        ///         message with the <code>bozoLevel</code> of 10 is not written, while
        ///         the message with the <code>bozoLevel</code> of 3 is.
        ///     </para>
        /// </remarks>
        [TestMethod]
        public void TestUseFilterToStopAMessage()
        {
            // create messages to send to the queue
            IPipeMessage message1 = new Message(Message.NORMAL, new { bozoLevel = 10, user = "Dastardly Dan" });
            IPipeMessage message2 = new Message(Message.NORMAL, new { bozoLevel = 3, user = "Dudley Doright" });

            // create filter, attach an anonymous listener to the filter output to receive the message,
            // pass in an anonymous function and an anonymous parameter object
            Filter filter = new Filter("bozoFilter", new PipeListener(this, CallBackMethod), (IPipeMessage msg, object _params) =>
            {
                if (((dynamic)msg.Header).bozoLevel > ((dynamic)_params).bozoThreshold)
                {
                    return false;
                } else {
                    return true;
                }
            }, new { bozoThreshold = 5 });

            // write normal message to the filter
            bool written1 = filter.Write(message1);
            bool written2 = filter.Write(message2);

            // test assertions
            Assert.IsTrue(message1 is IPipeMessage, "Expecting message is IPipeMessage");
            Assert.IsTrue(message2 is IPipeMessage, "Expecting message is IPipeMessage");
            Assert.IsTrue(filter is Filter, "Expecting filter is Filter");
            Assert.IsTrue(written1 == false, "Expecting written1 == false");
            Assert.IsTrue(written2 == true, "Expecting wrote good message");
            Assert.IsTrue(messagesReceived.Count == 1, "Expecting received 1 messages");

            // test filtered message assertions (message with good auth token passed
            IPipeMessage received = messagesReceived[0];
            messagesReceived.RemoveAt(0);
            Assert.IsTrue(received is Message, "Expecting received is Message");
            Assert.IsTrue(received == message2, "Expecting received == message2");
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
        ///     that a message passes through the pipe.</P>
        /// </para>
        /// </summary>
        /// <param name="message"></param>
        private void CallBackMethod(IPipeMessage message)
        {
            messagesReceived.Add(message);
        }
    }
}
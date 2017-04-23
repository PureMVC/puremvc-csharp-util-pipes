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
    /// Test the Queue class.
    /// </summary>
    [TestClass]
    public class QueueTest
    {
        /// <summary>
        /// Test connecting input and output pipes to a queue. 
        /// </summary>
        [TestMethod]
        public void TestConnectingIOPipes()
        {
            // create output pipes 1
            IPipeFitting pipe1 = new Pipe();
            IPipeFitting pipe2 = new Pipe();

            // create queue
            Queue queue = new Queue();

            // connect input fitting
            bool connectedInput = pipe1.Connect(queue);

            // connect output fitting
            bool connectedOutput = queue.Connect(pipe2);

            // test assertions
            Assert.IsTrue(pipe1 is Pipe, "Expecting pipe1 is Pipe");
            Assert.IsTrue(pipe2 is Pipe, "Expecting pipe2 is Pipe");
            Assert.IsTrue(queue is Queue, "Expecting queue is Queue");
            Assert.IsTrue(connectedInput, "Expecting connected input");
            Assert.IsTrue(connectedOutput, "Expecting connected output");
        }

        /// <summary>
        /// Test writing multiple messages to the Queue followed by a Flush message.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Creates messages to send to the queue. 
        ///         Creates queue, attaching an anonymous listener to its output.
        ///         Writes messages to the queue. Tests that no messages have been
        ///         received yet (they've been enqueued). Sends FLUSH message. Tests
        ///         that messages were receieved, and in the order sent (FIFO).
        ///     </para>
        /// </remarks>
        [TestMethod]
        public void TestWritingMultipleMessagesAndFlush()
        {
            // create messages to send to the queue
            IPipeMessage message1 = new Message(Message.NORMAL, new { testProp = 1 });
            IPipeMessage message2 = new Message(Message.NORMAL, new { testProp = 2 });
            IPipeMessage message3 = new Message(Message.NORMAL, new { testProp = 3 });

            // create queue control flush message
            IPipeMessage flush = new QueueControlMessage(QueueControlMessage.FLUSH);

            // create queue, attaching an anonymous listener to its output
            Queue queue = new Queue(new PipeListener(this, CallBackMethod));

            // write messages to the queue
            bool message1written = queue.Write(message1);
            bool message2written = queue.Write(message2);
            bool message3written = queue.Write(message3);

            // test assertions
            Assert.IsTrue(message1 is IPipeMessage, "Expecting message1 is IPipeMessage");
            Assert.IsTrue(message2 is IPipeMessage, "Expecting message2 is IPipeMessage");
            Assert.IsTrue(message3 is IPipeMessage, "Expecting message3 is IPipeMessage");
            Assert.IsTrue(flush is IPipeMessage, "Expecting flush is IPipeMessage");
            Assert.IsTrue(queue is Queue, "Expecting queue is Queue");

            Assert.IsTrue(message1written, "Expecting wrote message1 to queue");
            Assert.IsTrue(message2written, "Expecting wrote message2 to queue");
            Assert.IsTrue(message3written, "Expecting wrote message3 to queue");

            // test that no messages were received (they've been enqueued)
            Assert.IsTrue(messagesReceived.Count == 0, "Expecting received 0 messages");

            // write flush control message to the queue
            bool flushWritten = queue.Write(flush);

            // test that all messages were received, then test
            // FIFO order by inspecting the messages themselves
            Assert.IsTrue(messagesReceived.Count == 3, "Expected received 3 messages");

            // test message 1 assertions 
            IPipeMessage received1 = messagesReceived[0];
            messagesReceived.RemoveAt(0);
            Assert.IsTrue(received1 is IPipeMessage, "Expected received1 is IPipeMessage");
            Assert.IsTrue(received1 == message1, "Expected received1 == message1"); // object equality

            // test message 2 assertions 
            IPipeMessage received2 = messagesReceived[0];
            messagesReceived.RemoveAt(0);
            Assert.IsTrue(received2 is IPipeMessage, "Expected received2 is IPipeMessage");
            Assert.IsTrue(received2 == message2, "Expected received2 == message2"); // object equality

            // test message 3 assertions 
            IPipeMessage received3 = messagesReceived[0];
            messagesReceived.RemoveAt(0);
            Assert.IsTrue(received3 is IPipeMessage, "Expected received3 is IPipeMessage");
            Assert.IsTrue(received3 == message3, "Expected received3 == message3"); // object equality
        }

        /// <summary>
        /// Test the Sort-by-Priority and FIFO modes.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Creates messages to send to the queue, priorities unsorted. 
        ///         Creates queue, attaching an anonymous listener to its output.
        ///         Sends SORT message to start sort-by-priority order mode.
        ///         Writes messages to the queue. Sends FLUSH message, tests
        ///         that messages were receieved in order of priority, not how
        ///         they were sent.
        ///     </para>
        ///     <para>
        ///         Then sends a FIFO message to switch the queue back to
        ///         default FIFO behavior, sends messages again, flushes again,
        ///         tests that the messages were recieved and in the order they
        ///         were originally sent.
        ///     </para>
        /// </remarks>
        [TestMethod]
        public void TestSortByPriorityAndFIFO()
        {
            // create messages to send to the queue
            IPipeMessage message1 = new Message(Message.NORMAL, null, null, Message.PRIORITY_MED);
            IPipeMessage message2 = new Message(Message.NORMAL, null, null, Message.PRIORITY_LOW);
            IPipeMessage message3 = new Message(Message.NORMAL, null, null, Message.PRIORITY_HIGH);

            // create queue, attaching an anonymous listener to its output
            Queue queue = new Queue(new PipeListener(this, CallBackMethod));

            // begin sort-by-priority order mode
            bool sortWritten = queue.Write(new QueueControlMessage(QueueControlMessage.SORT));

            // write messages to the queue
            var message1written = queue.Write(message1);
            var message2written = queue.Write(message2);
            var message3written = queue.Write(message3);

            // flush the queue
            bool flushWritten = queue.Write(new QueueControlMessage(QueueControlMessage.FLUSH));

            // test assertions
            Assert.IsTrue(sortWritten, "Expecting wrote sort message to queue");
            Assert.IsTrue(message1written, "Expecting wrote message1 to queue");
            Assert.IsTrue(message2written, "Expecting wrote message2 to queue");
            Assert.IsTrue(message3written, "Expecting wrote message3 to queue");
            Assert.IsTrue(flushWritten, "Expecting wrote flush message to queue");

            // test that 3 messages were received
            Assert.IsTrue(messagesReceived.Count == 3, "Expecting received 3 messages");

            // get the messages
            IPipeMessage received1 = messagesReceived[0];
            messagesReceived.RemoveAt(0);
            IPipeMessage received2 = messagesReceived[0];
            messagesReceived.RemoveAt(0);
            IPipeMessage received3 = messagesReceived[0];
            messagesReceived.RemoveAt(0);

            // test that the message order is sorted 
            Assert.IsTrue(received1.Priority < received2.Priority, "Expecting received1 is higher priority than received 2");
            Assert.IsTrue(received2.Priority < received3.Priority, "Expecting received2 is higher priority than received 3");
            Assert.IsTrue(received1 == message3, "Expected received1 == message3"); // object equality
            Assert.IsTrue(received2 == message1, "Expected received2 == message1"); // object equality
            Assert.IsTrue(received3 == message2, "Expected received3 == message2"); // object equality

            // begin FIFO order mode
            bool fifowritten = queue.Write(new QueueControlMessage(QueueControlMessage.FIFO));

            // write messages to the queue
            bool message1writtenAgain = queue.Write(message1);
            bool message2writtenAgain = queue.Write(message2);
            bool message3writtenAgain = queue.Write(message3);

            // flush the queue
            bool flushwrittenAgain = queue.Write(new QueueControlMessage(QueueControlMessage.FLUSH));

            // test assertions
            Assert.IsTrue(fifowritten, "Expecting wrote fifo message to queue");
            Assert.IsTrue(message1writtenAgain, "Expecting worte message1 to queue again");
            Assert.IsTrue(message2writtenAgain, "Expecting worte message2 to queue again");
            Assert.IsTrue(message3writtenAgain, "Expecting worte message3 to queue again");

            // test that 3 messages were received 
            Assert.IsTrue(messagesReceived.Count == 3, "Expecting received 3 messages");

            // get the messages
            IPipeMessage received1Again = messagesReceived[0];
            messagesReceived.RemoveAt(0);
            IPipeMessage received2Again = messagesReceived[0];
            messagesReceived.RemoveAt(0);
            IPipeMessage received3Again = messagesReceived[0];
            messagesReceived.RemoveAt(0);

            // test message order is FIFO
            Assert.IsTrue(received1Again == message1, "Expecting received1Again == message1");
            Assert.IsTrue(received2Again == message2, "Expecting received2Again == message2");
            Assert.IsTrue(received3Again == message3, "Expecting received3Again == message3");
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

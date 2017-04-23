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
    /// Test the Message class.
    /// </summary>
    [TestClass]
    public class MessageTest
    {
        XmlDocument xmlDocument = new XmlDocument();

        /// <summary>
        /// Constructor.
        /// </summary>
        public MessageTest()
        {
            xmlDocument.LoadXml("<message att='Hello'/>");
        }

        /// <summary>
        /// Tests the constructor parameters and getters.
        /// </summary>
        [TestMethod]
        public void TestConstructorAndGetters()
        {
            // create a message with complete constructor args
            IPipeMessage message = new Message(Message.NORMAL, new { testProp = "testVal" }, xmlDocument, Message.PRIORITY_HIGH);

            // test assertions
            Assert.IsTrue(message is IPipeMessage);
            Assert.IsTrue(message.Type == Message.NORMAL, "Expecing message.Type == Message.Normal");
            Assert.IsTrue(((dynamic)message.Header).testProp == "testVal", "Expecting message.Header.testProp == 'testVal'");
            Assert.IsTrue(((XmlDocument)message.Body).DocumentElement.Attributes["att"].InnerText.Equals("Hello"), "Expecting ((XmlDocument)message.Body).DocumentElement.Attributes['att'].InnerText.Equals('Hello')");
            Assert.IsTrue(message.Priority == Message.PRIORITY_HIGH, "Expecting message.Priority == Message.PRIORITY_HIGH");
        }

        /// <summary>
        /// Tests message default priority.
        /// </summary>
        [TestMethod]
        public void TestDefaultPriority()
        {
            // Create a message with minimum constructor args
            IPipeMessage message = new Message(Message.NORMAL);

            // test assertions
            Assert.IsTrue(message.Priority == Message.PRIORITY_MED, "Expecting message.Priority == Message.PRIORITY_MED");
        }

        /// <summary>
        /// Tests the setters and getters.
        /// </summary>
        [TestMethod]
        public void TestSettersAndGetters()
        {
            // create a message with minimum constructor args
            IPipeMessage message = new Message(Message.NORMAL);

            // Set remainder via setters
            message.Header = new { testProp = "testVal" };
            message.Body = xmlDocument;
            message.Priority = Message.PRIORITY_LOW;

            // test assertions
            Assert.IsTrue(message is IPipeMessage);
            Assert.IsTrue(message.Type == Message.NORMAL, "Expecing message.Type == Message.Normal");
            Assert.IsTrue(((dynamic)message.Header).testProp == "testVal", "Expecting message.Header.testProp == 'testVal'");
            Assert.IsTrue(((XmlDocument)message.Body).DocumentElement.Attributes["att"].InnerText.Equals("Hello"), "Expecting ((XmlDocument)message.Body).DocumentElement.Attributes['att'].InnerText.Equals('Hello')");
            Assert.IsTrue(message.Priority == Message.PRIORITY_LOW, "Expecting message.Priority == Message.PRIORITY_LOW");
        }
    }
}

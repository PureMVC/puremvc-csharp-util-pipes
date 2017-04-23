//
//  PureMVC C# Multicore Utility - Pipes
//
//  Copyright(c) 2017-2027 Saad Shams <saad.shams@puremvc.org>
//  Your reuse is governed by the Creative Commons Attribution 3.0 License
//

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pipes.Interfaces;

namespace Pipes.Plumbing
{
    /// <summary>
    /// Test the Pipe class.
    /// </summary>
    [TestClass]
    public class PipeTest
    {
        /// <summary>
        /// Test the constructor.
        /// </summary>
        [TestMethod]
        public void TestConstructor()
        {
            IPipeFitting pipe = new Pipe();

            Assert.IsTrue(pipe is Pipe, "Expecing pipe is pipe");
        }

        /// <summary>
        /// Test connecting and disconnecting two pipes. 
        /// </summary>
        [TestMethod]
        public void TestConnectingAndDisconnectingTwoPipes()
        {
            // create two pipes
            IPipeFitting pipe1 = new Pipe();
            IPipeFitting pipe2 = new Pipe();
            // connect them
            bool success = pipe1.Connect(pipe2);

            // test assertions
            Assert.IsTrue(pipe1 is Pipe, "pipe1 is Pipe");
            Assert.IsTrue(pipe2 is Pipe, "pipe2 is Pipe");
            Assert.IsTrue(success, "Expecting connected pipe1 to pipe2");

            // disconnect pipe 2 from pipe 1
            IPipeFitting disconnectedPipe = pipe1.Disconnect();
            Assert.IsTrue(disconnectedPipe == pipe2, "Expecting disconnected pipe2 from pipe1");
        }

        /// <summary>
        /// Test attempting to connect a pipe to a pipe with an output already connected. 
        /// </summary>
        [TestMethod]
        public void TestConnectingToAConnectedPipe()
        {
            // create two pipes
            IPipeFitting pipe1 = new Pipe();
            IPipeFitting pipe2 = new Pipe();
            IPipeFitting pipe3 = new Pipe();

            // connect them
            bool success = pipe1.Connect(pipe2);

            // test assertions
            Assert.IsTrue(success, "Expecting connected pipe1 to pipe2");
            Assert.IsTrue(pipe1.Connect(pipe3) == false, "Expecting can't connect pipe3 to pipe1");
        }

    }
}

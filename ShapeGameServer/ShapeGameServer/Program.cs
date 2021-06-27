using System;
using System.IO;
using System.Threading;
namespace ShapeGameServer
{
    class Program
    {
        private static Thread consoleThread;
        static void Main(string[] args)
        {
            //To get the output in a file

            FileStream filestream = new FileStream("out.txt", FileMode.Create);
            var streamwriter = new StreamWriter(filestream);
            streamwriter.AutoFlush = true;
            Console.SetOut(streamwriter);
            Console.SetError(streamwriter);

            InitializeConsoleThread();
            ServerHandleData.InitializePacketListener();
            ServerTCP.InitializeServer();
        }
        private static void InitializeConsoleThread()
        {
            consoleThread = new Thread(ConsoleLoop);
            consoleThread.Name = "ConsoleThread";
            consoleThread.Start();
        }
        private static void ConsoleLoop()
        {
            while (true)
            {

            }
        }
    }
}

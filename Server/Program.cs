using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Threading;

namespace Server
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Game m_game = new Game();
            EventBasedNetListener m_listener = new EventBasedNetListener();
            NetDataWriter m_writer = new NetDataWriter();
            NetManager m_server = new NetManager(m_listener);

            m_server.Start(2310);

            Thread.Sleep(2000);
            if (m_server.IsRunning) Console.WriteLine("Server is running! Port: " + m_server.LocalPort);
            else Console.WriteLine("Server is NOT running!");

            m_listener.ConnectionRequestEvent += request =>
            {
                if (m_server.PeersCount < m_game.m_MaxPlayers) request.AcceptIfKey("amosdhhs9tnxtndb48fw");
                else request.Reject();

                m_game.m_CurPlayers = m_server.PeersCount;
            };

            m_listener.PeerConnectedEvent += peer =>
            {
                Console.WriteLine("We got a new connection from: {0}", peer.EndPoint);
                m_writer.Put("Connection successful!");
                peer.Send(m_writer, DeliveryMethod.ReliableOrdered);
            };

            do
            {
                while (!Console.KeyAvailable)
                {
                    m_server.PollEvents();
                    Thread.Sleep(20);
                }
            }
            while (Console.ReadKey(true).Key != ConsoleKey.Escape);

            m_server.Stop();
            Console.WriteLine("ESC key pressed, stopping server!");
        }
    }
}
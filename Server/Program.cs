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
            EventBasedNetListener m_listener = new EventBasedNetListener();
            NetDataWriter m_writer = new NetDataWriter();
            NetManager m_server = new NetManager(m_listener);
            Game m_game = new Game();

            Console.ForegroundColor = ConsoleColor.Green;

            m_server.Start(2310);
            Thread.Sleep(2000);
            if (m_server.IsRunning) Console.WriteLine("Server is running! Port: " + m_server.LocalPort);
            else Console.WriteLine("Server is NOT running!");

            m_listener.ConnectionRequestEvent += request =>
            {
                Console.WriteLine("New request from: " + request.RemoteEndPoint);

                if (m_server.PeersCount < m_game.m_MaxPlayers) request.AcceptIfKey("amosdhhs9tnxtndb48fw");
                else request.Reject();

                m_game.m_player = new NetworkPlayer[m_server.PeersCount];
                for (int i = 0; i < m_game.m_player.Length; i++)
                {
                    m_game.m_player[i] = new NetworkPlayer();
                    m_game.m_player[i].m_Id = i;
                }
            };

            m_listener.PeerConnectedEvent += peer =>
            {
                Console.WriteLine("We got a new connection from: " + peer.EndPoint);

                m_writer.Put("Connection to host successful! Your ID: " + peer.Id);
                peer.Send(m_writer, DeliveryMethod.ReliableOrdered);
                m_writer.Reset();
            };

            m_listener.NetworkReceiveEvent += (peer, reader, deliveryMethod) =>
            {
                string sChache = reader.GetString();
                if (sChache == "READY")
                {
                    m_game.m_player[peer.Id].m_Ready = true;
                    reader.Recycle();

                    //Debug
                    Console.WriteLine("Peer " + peer.Id + " is ready!");
                }
                else if (sChache == "INPUT")
                {
                    float x = reader.GetFloat();
                    float y = reader.GetFloat();
                    reader.Recycle();

                    m_game.m_player[peer.Id].m_Input = new Vector3(x, y);

                    //Debug
                    Console.WriteLine("Peer " + peer.Id + " X: " + x + "Y: " + y);
                }
            };

            do
            {
                while (!Console.KeyAvailable)
                {
                    if (!m_game.m_IsInProgress &&
                        m_game.m_player.Length > 0 &&
                        m_game.AllReady())
                    {
                        m_game.StartMatch(m_server, m_writer, m_game);
                    }

                    m_server.PollEvents();
                    m_game.NetworkGame(m_server, m_writer);
                    Thread.Sleep(20);
                }
            }
            while (Console.ReadKey(true).Key != ConsoleKey.Escape);
            Console.WriteLine("ESC key pressed, stopping server!");
            m_writer.Reset();
            m_writer.Put("Server shut down!");
            m_server.SendToAll(m_writer, DeliveryMethod.ReliableOrdered);
            m_server.Stop();
        }
    }
}
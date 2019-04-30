using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteNetLib;
using LiteNetLib.Utils;

namespace Server
{
    class Game
    {
        public bool m_IsInProgress;
        public int m_MaxPlayers = 4;
        public NetworkPlayer[] m_player = new NetworkPlayer[0];

        public void NetworkGame(NetManager server, NetDataWriter writer)
        {
            if (m_IsInProgress)
            {
                writer.Put("UPDATE");
                writer.Put(m_player.Length);
                for (int i = 0; i < m_player.Length; i++)
                {
                    writer.Put(m_player[i].m_Position.x);
                    writer.Put(m_player[i].m_Position.y);
                    writer.Put(m_player[i].m_Position.z);
                    writer.Put(m_player[i].m_Velocity.x);
                    writer.Put(m_player[i].m_Velocity.y);
                    writer.Put(m_player[i].m_Velocity.z);
                }
                server.SendToAll(writer, DeliveryMethod.ReliableOrdered);
                writer.Reset();
            }
        }

        public void StartMatch(NetManager server, NetDataWriter writer, Game game)
        {
            game.m_IsInProgress = true;
            writer.Put("START");
            writer.Put(m_player.Length);

            switch (m_player.Length)
            {
                case 1:
                    m_player[0].m_Position = new Vector3(0, 0.5f, 0);
                    break;
                case 2:
                    m_player[0].m_Position = new Vector3(-4, 0.5f, 0);
                    m_player[1].m_Position = new Vector3(4, 0.5f, 0);
                    break;
                case 3:
                    m_player[0].m_Position = new Vector3(-3.464f, 0.5f, -2f);
                    m_player[1].m_Position = new Vector3(3.464f, 0.5f, -2f);
                    m_player[2].m_Position = new Vector3(0, 0.5f, 4);
                    break;
                default:
                    writer.Reset();
                    writer.Put("START");
                    writer.Put(4);
                    m_player[0].m_Position = new Vector3(-2.828f, 0.5f, -2.828f);
                    m_player[1].m_Position = new Vector3(2.828f, 0.5f, -2.828f);
                    m_player[2].m_Position = new Vector3(-2.828f, 0.5f, 2.828f);
                    m_player[3].m_Position = new Vector3(2.828f, 0.5f, 2.828f);
                    break;
            }

            server.SendToAll(writer, DeliveryMethod.ReliableOrdered);
            writer.Reset();
        }

        public bool AllReady()
        {
            if (m_player.Length > 0)
            {
                for (int i = 0; i < m_player.Length; i++)
                {
                    if (!m_player[i].m_Ready) return false;
                }
                return true;
            }
            return false;
        }
    }
}

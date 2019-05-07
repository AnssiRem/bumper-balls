using LiteNetLib;
using LiteNetLib.Utils;
using System;

namespace Server
{
    internal class Game
    {
        public bool m_IsInProgress;
        public int m_MaxPlayers = 4;
        public float m_PlayerAcc = 0.0025f;
        public NetworkPlayer[] m_player = new NetworkPlayer[0];

        private int m_winnerId;

        public void NetworkGame(NetManager server, NetDataWriter writer)
        {
            if (m_IsInProgress)
            {
                //Calculate players positions and velocities and send to all
                CalcPlayers(m_player);

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

                //Check if players are off the platform
                for (int i = 0; i < m_player.Length; i++)
                {
                    if (m_player[i].m_Position.Length() > 6.133f && !m_player[i].m_Killed)
                    {
                        EliminatePlyer(i, server, writer);
                    }
                }

                //Check for winner
                if (OneAlive())
                {
                    writer.Put("WIN");
                    writer.Put(m_winnerId);
                    server.SendToAll(writer, DeliveryMethod.ReliableOrdered);
                    writer.Reset();

                    m_IsInProgress = false;

                    for (int i = 0; i < m_player.Length; i++)
                    {
                        m_player[i].m_Velocity = Vector3.zero;
                    }
                }
            }
        }

        /// <summary>
        /// Calculates the players' postitions and velocities. "A server tick"
        /// </summary>
        /// <param name="player">Game's player array</param>
        private void CalcPlayers(NetworkPlayer[] player)
        {
            for (int i = 0; i < player.Length; i++)
            {
                //Check if two player's are colliding
                if (!player[i].m_Colliding && !player[i].m_Killed)
                {
                    for (int j = 0; j < player.Length; j++)
                    {
                        if (i != j)
                        {
                            Vector3 dist2Player = player[j].m_Position - player[i].m_Position;
                            if (dist2Player.Length() <= 0.5f)
                            {
                                player[i].m_Colliding = true;
                                player[j].m_Colliding = true;
                                PlayersCollide(player[i], player[j]);
                            }
                        }
                    }
                }

                //Calculate next position using peer's input and previous velocity
                if (!player[i].m_Colliding && !player[i].m_Killed)
                {
                    if (player[i].m_Input.x != 0 ||
                        player[i].m_Input.y != 0)
                    {
                        player[i].m_Velocity.x += m_PlayerAcc * player[i].m_Input.Normalize().x;
                        player[i].m_Velocity.z += m_PlayerAcc * player[i].m_Input.Normalize().y;
                    }
                    else
                    {
                        player[i].m_Velocity *= 1 - (2 * m_PlayerAcc);
                    }

                    player[i].m_Position.x += player[i].m_Velocity.x;
                    player[i].m_Position.z += player[i].m_Velocity.z;
                }
            }
            //Set collision bool false for next function call
            for (int i = 0; i < player.Length; i++)
            {
                player[i].m_Colliding = false;
            }
        }

        /// <summary>
        /// Removes peer from play
        /// </summary>
        /// <param name="id">Peer id</param>
        /// <param name="server">Server object</param>
        /// <param name="writer">Writer object</param>
        public void EliminatePlyer(int id, NetManager server, NetDataWriter writer)
        {
            writer.Put("KILL");
            writer.Put(id);
            server.SendToAll(writer, DeliveryMethod.ReliableOrdered);
            writer.Reset();

            m_player[id].m_Killed = true;

            //Debug
            Console.WriteLine("Peer " + id + " eliminated!");
        }

        /// <summary>
        /// Initializes the game
        /// </summary>
        /// <param name="server">Server object</param>
        /// <param name="writer">Writer object</param>
        /// <param name="game">Game object</param>
        public void StartMatch(NetManager server, NetDataWriter writer, Game game)
        {
            game.m_IsInProgress = true;
            writer.Put("START");
            writer.Put(m_player.Length);//Player amount

            //Starting postitions
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

            //Set booleans false for continious play
            for (int i = 0; i < m_player.Length; i++)
            {
                m_player[i].m_Killed = false;
                m_player[i].m_Ready = false;
            }
        }

        /// <summary>
        /// Calculate the collision of two players
        /// </summary>
        /// <param name="player1">First player object</param>
        /// <param name="player2">Second player object</param>
        private void PlayersCollide(NetworkPlayer player1, NetworkPlayer player2)
        {
            Vector3 dist2Player = player2.m_Position - player1.m_Position;
            float apparentEnergy = player1.m_Velocity.Length() + player2.m_Velocity.Length();

            player1.m_Velocity = -dist2Player.Normalize() * apparentEnergy * 0.5f;
            player2.m_Velocity = -player1.m_Velocity;

            player1.m_Position.x += player1.m_Velocity.x;
            player1.m_Position.z += player1.m_Velocity.z;

            player2.m_Position.x += player2.m_Velocity.x;
            player2.m_Position.z += player2.m_Velocity.z;
        }

        /// <summary>
        /// Check if all players are ready
        /// </summary>
        /// <returns>True if all players are ready</returns>
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

        /// <summary>
        /// Check if only a single player is alive
        /// </summary>
        /// <returns>True if only one player remains alive</returns>
        public bool OneAlive()
        {
            if (m_player.Length > 1)
            {
                int alive = 0;
                for (int i = 0; i < m_player.Length; i++)
                {
                    if (!m_player[i].m_Killed)
                    {
                        m_winnerId = m_player[i].m_Id;
                        alive++;
                    }
                }
                if (alive == 1) return true;
                else return false;
            }
            return false;
        }
    }
}
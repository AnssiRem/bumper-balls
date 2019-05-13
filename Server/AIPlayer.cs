using System;

namespace Server.AI
{
    internal class AIPlayer : NetworkPlayer
    {
        private bool m_setInitTarget;
        private int m_stateUpdateRate = 1;
        private int m_targetChangeRate = 1000;
        private int m_targetId;
        private int m_update = 0;
        private AIPlayerFSM m_FSM;
        private Random rnd = new Random();

        public AIPlayer()
        {
            m_Position = new Vector3();
            m_Velocity = new Vector3();
            m_Input = new Vector3();

            m_FSM = new AIPlayerFSM();
        }

        public AIPlayer(int id)
        {
            m_Id = id;
            m_Position = new Vector3();
            m_Velocity = new Vector3();
            m_Input = new Vector3();

            m_FSM = new AIPlayerFSM();
        }

        public void UpdateInput(Game game)
        {
            if (!m_setInitTarget || game.m_player[m_targetId].m_Killed || m_update % m_targetChangeRate == 0)
            {
                PickTarget(game);
                m_setInitTarget = true;
            }

            if (m_update % m_stateUpdateRate == 0)
            {
                m_FSM.DecideState(m_Id, m_Position, m_Velocity, game.m_player[m_targetId].m_Position, game.m_player[m_targetId].m_Velocity);
            }
            m_update++;

            Vector3 temp;
            switch ((int)m_FSM.m_State)
            {
                case 0://Attack
                       //Move towards target
                    temp = game.m_player[m_targetId].m_Position - m_Position;
                    m_Input = new Vector3(temp.x, temp.z, 0).Normalize();
                    break;
                case 1://Dodge
                       //Move perpendicular to map center
                    temp = Vector3.zero - m_Position;
                    temp = new Vector3(temp.z, 0, -temp.x) * (float)Math.Pow(-1, (rnd.Next(1, 3)));
                    m_Input = new Vector3(temp.x, temp.z, 0).Normalize();
                    break;
                case 2://Survive
                       //Move towards center of the map
                    temp = Vector3.zero - m_Position;
                    m_Input = new Vector3(temp.x, temp.z, 0).Normalize();
                    break;
                default:
                    Console.WriteLine("AN UNHANDLED FSM STATE!");
                    break;
            }
        }

        private void PickTarget(Game game)
        {
            if (!game.OneAlive())
            {
                int[] possibleId = new int[0];
                int j = 0;
                for (int i = 0; i < game.m_player.Length; i++)
                {
                    if (game.m_player[i] != this && !game.m_player[i].m_Killed)
                    {
                        Array.Resize(ref possibleId, possibleId.Length + 1);
                        possibleId[j] = game.m_player[i].m_Id;
                        j++;
                    }
                }
                m_targetId = possibleId[rnd.Next(0, possibleId.Length)];
            }
        }
    }
}
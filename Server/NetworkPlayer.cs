using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class NetworkPlayer
    {
        public bool m_Ready;
        public int m_Id;
        public Vector3 m_Position;
        public Vector3 m_Velocity;
        public Vector3 m_Input;

        public NetworkPlayer()
        {
            m_Position = new Vector3();
            m_Velocity = new Vector3();
            m_Input = new Vector3();
        }
    }
}

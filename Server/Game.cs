using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Game
    {
        public bool m_IsInProgress;

        public int m_MaxPlayers;
        public int m_CurPlayers;

        public Game()
        {
            m_IsInProgress = false;

            m_MaxPlayers = 4;
            m_CurPlayers = 0;
        }
    }
}

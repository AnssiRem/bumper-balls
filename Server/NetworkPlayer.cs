namespace Server
{
    internal class NetworkPlayer
    {
        public bool m_Colliding;
        public bool m_Killed;
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

        public NetworkPlayer(int id)
        {
            m_Id = id;
            m_Position = new Vector3();
            m_Velocity = new Vector3();
            m_Input = new Vector3();
        }
    }
}
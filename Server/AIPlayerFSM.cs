using System;

namespace Server.AI
{
    internal enum FSMState
    {
        Attack,
        Dodge,
        Survive
    };

    internal class AIPlayerFSM
    {
        public FSMState m_State;

        private float m_edgeRadius = 3f;
        private float m_targettingRadius = 1.5f;
        private float m_targetApproachAngle = 15f;
        private float m_origRecedeAngle = 90f;

        public AIPlayerFSM()
        {
            m_State = FSMState.Attack;
        }

        public void DecideState(int id, Vector3 position, Vector3 velocity, Vector3 targetPosition, Vector3 targetVelocity)
        {
            if (position.Length() > m_edgeRadius)
            {
                //Calculate the angle between vector from position to orig and own velocity
                float cosAngle = (-position.x * velocity.x + -position.y * velocity.y + -position.z * velocity.z) / (-position.Length() * velocity.Length());
                if (Math.Acos(cosAngle) > m_origRecedeAngle * 3.1416f / 180f)
                {
                    if (m_State != FSMState.Survive)
                    {
                        Console.WriteLine("Surviving! Regards player: " + id);
                    }
                    m_State = FSMState.Survive;
                }
                else
                {
                    //Calculate the angle between vector from target to position and target's velociy
                    Vector3 target2pos = position - targetPosition;
                    cosAngle = (target2pos.x * targetVelocity.x + target2pos.y * targetVelocity.y + target2pos.z * targetVelocity.z) / (target2pos.Length() * targetVelocity.Length());
                    if (Math.Acos(cosAngle) < m_targetApproachAngle * 3.1416f / 180f)
                    {
                        if(m_State != FSMState.Dodge)
                        {
                            Console.WriteLine("Dodging! Regards player: " + id);
                        }
                        m_State = FSMState.Dodge;
                    }
                    else
                    {
                        if (m_State != FSMState.Attack)
                        {
                            Console.WriteLine("Attacking! Regards player: " + id);
                        }
                        m_State = FSMState.Attack;
                    }
                }
            }
            else
            {
                if ((targetPosition - position).Length() > m_targettingRadius)
                {
                    if (m_State != FSMState.Attack)
                    {
                        Console.WriteLine("Attacking! Regards player: " + id);
                    }
                    m_State = FSMState.Attack;
                }
                else
                {
                    if (m_State != FSMState.Survive)
                    {
                        Console.WriteLine("Surviving! Regards player: " + id);
                    }
                    m_State = FSMState.Survive;
                }
            }
        }
    }
}
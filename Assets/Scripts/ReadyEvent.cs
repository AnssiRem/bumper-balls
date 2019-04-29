using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BumberBalls;

namespace BumberBalls
{
    public class ReadyEvent : MonoBehaviour
    {
        private Client m_client;

        public void FireEvent()
        {
            if (!m_client)
            {
                m_client = GameObject.Find("Client Object").GetComponent<Client>();
            }

            m_client.Ready();
        }
    }
}
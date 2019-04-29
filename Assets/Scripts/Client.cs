using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using BumberBalls.CustomDebug;

namespace BumberBalls
{

    public class Client : MonoBehaviour
    {
        private EventBasedNetListener m_listener;
        private NetDataWriter m_writer;
        private NetManager m_client;

        private DebugUI m_debugUI = new DebugUI();

        private bool m_IsInProgress;

        private void Awake()
        {
            DontDestroyOnLoad(this);
        }

        private void Update()
        {
            UpdateScene();

            NetworkInput(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        }

        public void NetworkConnect(Text inputText)
        {
            string temp = inputText.text;

            m_listener = new EventBasedNetListener();
            m_client = new NetManager(m_listener);
            m_writer = new NetDataWriter();

            m_client.Start();
            m_client.Connect(temp, 2310, "amosdhhs9tnxtndb48fw");

            m_debugUI.Write("Connecting to IPv4: " + temp);

#if UNITY_EDITOR
            Debug.Log("Starting client... Port: " + m_client.LocalPort);
            Debug.Log("Connecting... IPv4: " + temp);
#endif

            m_listener.NetworkReceiveEvent += (fromPeer, dataReader, deliveryMethod) =>
            {
                string sCahce = dataReader.GetString();
                dataReader.Recycle();

                if (sCahce == "START")
                {
                    StartMatch();
                }

                m_debugUI.Write("Connection successful! IPv4: " + temp);

                //Debug
                Debug.Log("Recieved: " + sCahce);
            };

            InvokeRepeating("PollEvents", 0, 0.02f);
        }

        private void NetworkInput(float x, float y)
        {
            if (m_IsInProgress)
            {
                m_writer.Put(x);
                m_writer.Put(y);
                m_client.FirstPeer.Send(m_writer, DeliveryMethod.ReliableOrdered);
                m_writer.Reset();
            }
        }

        private void UpdateScene()
        {
            if (m_client != null)
            {
                if (m_client.PeersCount != 0 && SceneManager.GetActiveScene().name != "Game")
                {
                    SceneManager.LoadScene("Game");
                }
            }
        }

        public void Ready()
        {
            m_writer.Put("READY");

            m_client.FirstPeer.Send(m_writer, DeliveryMethod.ReliableOrdered);
        }

        private void PollEvents()
        {
            m_client.PollEvents();
        }

        private void StartMatch()
        {
            m_IsInProgress = true;
            GameObject.Find("Canvas").SetActive(false);
        }
    }
}
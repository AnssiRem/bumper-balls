﻿using BumberBalls.CustomDebug;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BumberBalls
{
    [RequireComponent(typeof(Game))]
    public class Client : MonoBehaviour
    {
        private EventBasedNetListener m_listener;
        private NetDataWriter m_writer;
        private NetManager m_client;
        private Game m_game;
        //Debug
        private DebugUI m_debugUI = new DebugUI();

        private void Awake()
        {
            DontDestroyOnLoad(this);

            m_game = gameObject.GetComponent<Game>();
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

                if (sCahce == "START")
                {
                    int iCahce = dataReader.GetInt();
                    dataReader.Recycle();
                    m_game.StartMatch(iCahce);

                    //Debug
                    Debug.Log("Recieved integer: " + iCahce);
                }
                else if (sCahce == "UPDATE")
                {
                    int iCahce = dataReader.GetInt();
                    for (int i = 0; i < iCahce; i++)
                    {
                        //Position floats
                        float fCache = dataReader.GetFloat();
                        m_game.m_playerObject[i].transform.position = new Vector3
                        (
                            fCache,
                            m_game.m_playerObject[i].transform.position.y,
                            m_game.m_playerObject[i].transform.position.z
                        );
                        fCache = dataReader.GetFloat();
                        m_game.m_playerObject[i].transform.position = new Vector3
                        (
                            m_game.m_playerObject[i].transform.position.x,
                            fCache,
                            m_game.m_playerObject[i].transform.position.z
                        );
                        fCache = dataReader.GetFloat();
                        m_game.m_playerObject[i].transform.position = new Vector3
                        (
                            m_game.m_playerObject[i].transform.position.x,
                            m_game.m_playerObject[i].transform.position.y,
                            fCache
                        );
                        //TODO: Velocity floats
                        fCache = dataReader.GetFloat();
                        fCache = dataReader.GetFloat();
                        fCache = dataReader.GetFloat();
                    }
                    dataReader.Recycle();
                }
                else
                {
                    m_debugUI.Write("Message from the host: " + sCahce);
                }
            };

            InvokeRepeating("PollEvents", 0, 0.02f);
        }

        private void NetworkInput(float x, float y)
        {
            if (m_game.m_IsInProgress)
            {
                m_writer.Put("INPUT");
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
            m_writer.Reset();
        }

        private void PollEvents()
        {
            m_client.PollEvents();
        }
    }
}
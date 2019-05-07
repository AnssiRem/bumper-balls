using BumberBalls.CustomDebug;
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

        /// <summary>
        /// Connects to host, initializes client
        /// </summary>
        /// <param name="inputText">The IPv4 of host</param>
        public void NetworkConnect(Text inputText)
        {
            string temp = inputText.text;

            m_listener = new EventBasedNetListener();
            m_client = new NetManager(m_listener);
            m_writer = new NetDataWriter();

            m_client.Start();
            m_client.Connect(temp, 2310, "amosdhhs9tnxtndb48fw");

            m_debugUI.Write("Starting client... Port: " + m_client.LocalPort + "\nConnecting to IPv4: " + temp);

            m_listener.NetworkReceiveEvent += (fromPeer, dataReader, deliveryMethod) =>
            {
                string sCahce = dataReader.GetString();

                if (sCahce == "START")
                {
                    int iCahce = dataReader.GetInt();
                    dataReader.Recycle();
                    m_game.StartMatch(iCahce);

                    //Debug
                    m_debugUI.Write("Recieved integer: " + iCahce);
                }
                else if (sCahce == "UPDATE")
                {
                    int iCahce = dataReader.GetInt();
                    for (int i = 0; i < iCahce; i++)
                    {
                        //Position floats
                        if (!m_game.m_playerObject[i].GetComponent<Player>().m_IsKilled)
                        {
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
                        //TODO: Velocity floats/lerp?
                        dataReader.GetFloat();
                        dataReader.GetFloat();
                        dataReader.GetFloat();
                        }
                        else
                        {
                            for (int j = 0; j < 6; j++)
                            {
                                dataReader.GetFloat();
                            }
                        }
                    }
                    dataReader.Recycle();
                }
                else if (sCahce == "KILL")
                {
                    int iCahce = dataReader.GetInt();
                    GameObject[] playerObject = GameObject.FindGameObjectsWithTag("Player");
                    for (int i = 0; i < playerObject.Length; i++)
                    {
                        Player player = playerObject[i].GetComponent<Player>();
                        if (player.m_Id == iCahce + 1)
                        {
                            player.Kill();

                            //Debug
                            m_debugUI.Write(string.Format("Killed player {0}", iCahce + 1));
                        }
                    }
                    dataReader.Recycle();
                }
                else if(sCahce == "WIN")
                {
                    int iCahce = dataReader.GetInt();
                    m_game.EndMatch(iCahce);
                }
                else
                {
                    m_debugUI.Write("Message from the host: " + sCahce);
                }
            };

            InvokeRepeating("PollEvents", 0, 0.02f);
        }

        /// <summary>
        /// Send client's inputs to the host
        /// </summary>
        /// <param name="x">Horizontal axis</param>
        /// <param name="y">Vertical axis</param>
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

        /// <summary>
        /// Change Unity scene when client has connected
        /// </summary>
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

        /// <summary>
        /// Send ready message to host
        /// </summary>
        public void Ready()
        {
            m_writer.Put("READY");
            m_client.FirstPeer.Send(m_writer, DeliveryMethod.ReliableOrdered);
            m_writer.Reset();
        }

        /// <summary>
        /// Invokeable client PollEvents method
        /// </summary>
        private void PollEvents()
        {
            m_client.PollEvents();
        }
    }
}
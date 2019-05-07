using UnityEngine;
using UnityEngine.UI;

namespace BumberBalls
{
    internal class Game : MonoBehaviour
    {
        public bool m_IsInProgress;
        public GameObject[] m_playerObject;

        private Color[] m_color = { Color.blue, Color.red, Color.yellow, Color.green, Color.cyan, Color.magenta, new Color(255, 165, 0), new Color(50, 255, 50) };

        [SerializeField] private GameObject m_playerPrefab;

        /// <summary>
        /// Initialize the game locally
        /// </summary>
        /// <param name="playerCount">The number of players</param>
        public void StartMatch(int playerCount)
        {
            //"Hide" UI elements
            GameObject.Find("Canvas/Vertical Layout/Ready Button").SetActive(false);
            GameObject.Find("Canvas/Vertical Layout/Winner Text").GetComponent<Text>().text = "";

            //Initialize player objects in hardcoded positions and give them id's
            m_playerObject = new GameObject[playerCount];
            switch (playerCount)
            {
                case 1:
                    m_playerObject[0] = Instantiate(m_playerPrefab, new Vector3(0, 0.5f, 0), Quaternion.identity);
                    for (int i = 0; i < m_playerObject.Length; i++) m_playerObject[i].GetComponent<Player>().m_Id = i + 1;
                    //Player colors
                    for (int i = 0; i < playerCount; i++)m_playerObject[i].GetComponent<Renderer>().material.color = m_color[i];
                    break;

                case 2:
                    m_playerObject[0] = Instantiate(m_playerPrefab, new Vector3(-4, 0.5f, 0), Quaternion.identity);
                    m_playerObject[1] = Instantiate(m_playerPrefab, new Vector3(4, 0.5f, 0), Quaternion.identity);
                    for (int i = 0; i < m_playerObject.Length; i++) m_playerObject[i].GetComponent<Player>().m_Id = i + 1;
                    //Player colors
                    for (int i = 0; i < playerCount; i++) m_playerObject[i].GetComponent<Renderer>().material.color = m_color[i];
                    break;

                case 3:
                    m_playerObject[0] = Instantiate(m_playerPrefab, new Vector3(-3.464f, 0.5f, -2f), Quaternion.identity);
                    m_playerObject[1] = Instantiate(m_playerPrefab, new Vector3(3.464f, 0.5f, -2f), Quaternion.identity);
                    m_playerObject[2] = Instantiate(m_playerPrefab, new Vector3(0, 0.5f, 4), Quaternion.identity);
                    for (int i = 0; i < m_playerObject.Length; i++) m_playerObject[i].GetComponent<Player>().m_Id = i + 1;
                    //Player colors
                    for (int i = 0; i < playerCount; i++) m_playerObject[i].GetComponent<Renderer>().material.color = m_color[i];
                    break;

                default:
                    m_playerObject[0] = Instantiate(m_playerPrefab, new Vector3(-2.828f, 0.5f, -2.828f), Quaternion.identity);
                    m_playerObject[1] = Instantiate(m_playerPrefab, new Vector3(2.828f, 0.5f, -2.828f), Quaternion.identity);
                    m_playerObject[2] = Instantiate(m_playerPrefab, new Vector3(-2.828f, 0.5f, 2.828f), Quaternion.identity);
                    m_playerObject[3] = Instantiate(m_playerPrefab, new Vector3(2.828f, 0.5f, 2.828f), Quaternion.identity);
                    for (int i = 0; i < m_playerObject.Length; i++) m_playerObject[i].GetComponent<Player>().m_Id = i + 1;
                    //Player colors
                    for (int i = 0; i < playerCount; i++) m_playerObject[i].GetComponent<Renderer>().material.color = m_color[i];
                    break;
            }
            m_IsInProgress = true;
        }

        /// <summary>
        /// End the match
        /// </summary>
        /// <param name="id">Id of the winning player</param>
        public void EndMatch(int id)
        {
            //Destroy player objects
            for (int i = 0; i < m_playerObject.Length; i++)
            {
                Destroy(m_playerObject[i]);
            }

            //"Show" UI elements
            GameObject.Find("Canvas/Vertical Layout/Ready Button").SetActive(true);
            GameObject.Find("Canvas/Vertical Layout/Winner Text").GetComponent<Text>().text = string.Format("Player {0} wins!", id + 1);
        }
    }
}
using UnityEngine;

namespace BumberBalls
{
    internal class Game : MonoBehaviour
    {
        public bool m_IsInProgress;
        public GameObject[] m_playerObject;

        [SerializeField] private GameObject m_playerPrefab;

        public void StartMatch(int playerCount)
        {
            GameObject.Find("Canvas/Vertical Layout/Ready Button").SetActive(false);

            m_playerObject = new GameObject[playerCount];
            switch (playerCount)
            {
                case 1:
                    m_playerObject[0] = Instantiate(m_playerPrefab, new Vector3(0, 0.5f, 0), Quaternion.identity);
                    for (int i = 0; i < m_playerObject.Length; i++) m_playerObject[i].GetComponent<Player>().m_Id = i + 1;

                    break;

                case 2:
                    m_playerObject[0] = Instantiate(m_playerPrefab, new Vector3(-4, 0.5f, 0), Quaternion.identity);
                    m_playerObject[1] = Instantiate(m_playerPrefab, new Vector3(4, 0.5f, 0), Quaternion.identity);
                    for (int i = 0; i < m_playerObject.Length; i++) m_playerObject[i].GetComponent<Player>().m_Id = i + 1;
                    break;

                case 3:
                    m_playerObject[0] = Instantiate(m_playerPrefab, new Vector3(-3.464f, 0.5f, -2f), Quaternion.identity);
                    m_playerObject[1] = Instantiate(m_playerPrefab, new Vector3(3.464f, 0.5f, -2f), Quaternion.identity);
                    m_playerObject[2] = Instantiate(m_playerPrefab, new Vector3(0, 0.5f, 4), Quaternion.identity);
                    for (int i = 0; i < m_playerObject.Length; i++) m_playerObject[i].GetComponent<Player>().m_Id = i + 1;
                    break;

                default:
                    m_playerObject[0] = Instantiate(m_playerPrefab, new Vector3(-2.828f, 0.5f, -2.828f), Quaternion.identity);
                    m_playerObject[1] = Instantiate(m_playerPrefab, new Vector3(2.828f, 0.5f, -2.828f), Quaternion.identity);
                    m_playerObject[2] = Instantiate(m_playerPrefab, new Vector3(-2.828f, 0.5f, 2.828f), Quaternion.identity);
                    m_playerObject[3] = Instantiate(m_playerPrefab, new Vector3(2.828f, 0.5f, 2.828f), Quaternion.identity);
                    for (int i = 0; i < m_playerObject.Length; i++) m_playerObject[i].GetComponent<Player>().m_Id = i + 1;
                    break;
            }
            m_IsInProgress = true;
        }
    }
}
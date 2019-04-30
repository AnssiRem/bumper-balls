using UnityEngine;
using UnityEngine.UI;

namespace BumberBalls.CustomDebug
{
    public class DebugUI
    {
        private Text m_text;

        public void Write(string s)
        {
            if (!m_text) m_text = GameObject.Find("Debug Text").GetComponent<Text>();

            m_text.text = s;
        }
    }
}
using UnityEngine;

namespace BumberBalls
{
    [RequireComponent(typeof(Rigidbody))]
    public class Player : MonoBehaviour
    {
        public bool m_IsKilled;
        public float[] m_Input = new float[2];
        public int m_Id;

        [SerializeField] private bool m_locallyControlled;
        [SerializeField] private float m_movementForce;

        private void FixedUpdate()
        {
            //Debug
            if (m_locallyControlled)
            {
                m_Input[0] = Input.GetAxis("Horizontal");
                m_Input[1] = Input.GetAxis("Vertical");

                if (m_Input[0] != 0 || m_Input[1] != 0)
                {
                    Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
                    gameObject.GetComponent<Rigidbody>().AddForceAtPosition(movement.normalized * m_movementForce, transform.position + new Vector3(0, 0.5f, 0));
                }
            }
        }

        /// <summary>
        /// Make player lose
        /// </summary>
        public void Kill()
        {
            m_IsKilled = true;
            gameObject.GetComponent<Rigidbody>().useGravity = true;
            gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        }
    }
}
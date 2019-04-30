using UnityEngine;

namespace BumberBalls
{
    public class Player : MonoBehaviour
    {
        public int m_Id;
        public float[] m_Input = new float[2];

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
    }
}
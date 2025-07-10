using UnityEngine;

public class PlayerNormalSkill : MonoBehaviour
{
    private Rigidbody m_rigidBody;

    public void Initialize(float speed, float lifetime, Vector3 direction)
    {
        m_rigidBody = GetComponent<Rigidbody>();
        if (m_rigidBody != null)
        {
            m_rigidBody.linearVelocity = direction.normalized * speed;
        }

        Destroy(gameObject, lifetime);
    }
}

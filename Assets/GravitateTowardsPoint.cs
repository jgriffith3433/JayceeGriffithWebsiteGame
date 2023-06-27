using UnityEngine;

public class GravitateTowardsPoint : MonoBehaviour
{
    [SerializeField] private GravPoint m_PointTransform = null;
    [SerializeField] private float m_PullForce = 1f;
    [SerializeField] private Rigidbody m_Rigidbody = null;

    private void FixedUpdate()
    {
        var forceDirection = m_PointTransform.transform.position - transform.position;
        m_Rigidbody.AddForce(forceDirection.normalized * m_PullForce * 1000 * Time.fixedDeltaTime, ForceMode.Acceleration);
    }
}

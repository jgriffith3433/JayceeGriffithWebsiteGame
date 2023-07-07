using System.Collections;
using UnityEngine;

public class ExplosionForceTimeline : MonoBehaviour
{
    [SerializeField]
    private float m_Radius = 5.0F;
    [SerializeField]
    private float m_Power = 10.0F;

    private void OnEnable()
    {
        var explosionPos = transform.position;
        var colliders = Physics.OverlapSphere(explosionPos, m_Radius, (1 << 6));
        foreach (var hit in colliders)
        {
            var gooba = hit.GetComponentInParent<GoobaPickADirectionAndRun>();

            if (gooba != null)
            {
                gooba.Ragdoll(m_Power, explosionPos, m_Radius, 100.0f * Random.Range(0f, 10f), ForceMode.Force);
            }

            var rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = false;
                rb.isKinematic = true;
                rb.AddExplosionForce(m_Power, explosionPos, m_Radius, 1000.0f, ForceMode.Impulse);
            }
        }
    }
}

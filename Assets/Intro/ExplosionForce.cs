using System.Collections;
using UnityEngine;

public class ExplosionForce : MonoBehaviour
{
    public LayerMask targetLayers;
    [SerializeField]
    private float m_Radius = 5.0F;
    [SerializeField]
    private float m_Power = 10.0F;

    private void OnEnable()
    {
        var explosionPos = transform.position + (transform.up * -2);
        var colliders = Physics.OverlapSphere(explosionPos, m_Radius, targetLayers);
        foreach (var hit in colliders)
        {
            var gooba = hit.GetComponentInParent<TestPlayer>();

            if (gooba != null)
            {
                gooba.Ragdoll();
                var rbs = gooba.PuppetMaster.GetComponentsInChildren<Rigidbody>();
                foreach(var rb in rbs)
                {
                    rb.AddExplosionForce(Random.Range(1, m_Power), explosionPos, m_Radius, 100.0f, ForceMode.Impulse);
                }
            }

            var aiPlayer = hit.GetComponentInParent<AIPlayer>();
            if (aiPlayer != null)
            {
                aiPlayer.Ragdoll();
                var rbs = aiPlayer.PuppetMaster.GetComponentsInChildren<Rigidbody>();
                foreach (var rb in rbs)
                {
                    rb.AddExplosionForce(Random.Range(1, m_Power), explosionPos, m_Radius, 100.0f, ForceMode.Impulse);
                }
            }

            //var rb = hit.GetComponent<Rigidbody>();
            //if (rb != null)
            //{
            //    rb.AddExplosionForce(m_Power, explosionPos, m_Radius, 1000.0f, ForceMode.Impulse);
            //}
        }
    }
}

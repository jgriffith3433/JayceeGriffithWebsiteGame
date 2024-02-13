using RootMotion.Dynamics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingGoobas : MonoBehaviour
{
    public GameObject FallingGoobaPrefab;
    public int AmountOfGoobas = 100;
    public int RespawnZ = 0;
    public int RespawnY = -75;
    public int Width = 50;
    public int Depth = 100;
    public int Spin = 2;
    public float SpawnWaitTime = .25f;

    private List<GameObject> m_SpawnedGoobas = new List<GameObject>();
    private List<CapsuleCollider> m_SpawnedGoobasController = new List<CapsuleCollider>();

    IEnumerator Start()
    {
        for (var i = 0; i < AmountOfGoobas; i++)
        {
            yield return new WaitForSeconds(SpawnWaitTime);
            var newGooba = Instantiate(FallingGoobaPrefab, transform.position + new Vector3(Random.Range(-Width, Width), Random.Range(50, 100), Random.Range(RespawnZ - Depth, RespawnZ + Depth)), Quaternion.identity, transform);
            var cc = newGooba.GetComponentInChildren<CapsuleCollider>();
            var rb = cc.GetComponent<Rigidbody>();
            rb.angularVelocity = new Vector3(Random.Range(-Spin, Spin), Random.Range(-Spin, Spin), Random.Range(-Spin, Spin));
            m_SpawnedGoobas.Add(newGooba);
            m_SpawnedGoobasController.Add(cc);
        }
    }

    void Update()
    {
        for (var i = 0; i < m_SpawnedGoobas.Count; i++)
        {
            var spawnedGooba = m_SpawnedGoobas[i];
            var spawnedGoobaCC = m_SpawnedGoobasController[i];
            if (spawnedGoobaCC.transform.position.y < RespawnY)
            {
                var rb = spawnedGoobaCC.GetComponent<Rigidbody>();
                if (rb)
                {
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = new Vector3(Random.Range(-Spin, Spin), Random.Range(-Spin, Spin), Random.Range(-Spin, Spin));
                }
                spawnedGoobaCC.transform.position = transform.position + new Vector3(Random.Range(-Width, Width), Random.Range(50, 100), Random.Range(RespawnZ - Depth, RespawnZ + Depth));
            }
        }
    }
}

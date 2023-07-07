using GNet;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    [SerializeField]
    private float m_Speed = 30f;

    [SerializeField]
    private GameObject m_ExplosionPrefab = null;

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, transform.position + transform.forward * m_Speed, Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 1 || other.gameObject.layer == 6)
        {
            Instantiate(m_ExplosionPrefab, new Vector3(transform.position.x, other.transform.position.y, transform.position.z), Quaternion.identity);
            gameObject.DestroySelf();
        }
    }
}

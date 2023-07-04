using RootMotion.Dynamics;
using System.Collections;
using UnityEngine;

public class GoobaPickADirectionAndRun : MonoBehaviour
{
    public float TimeBeforeRunning = 10f;
    public float Speed = 10f;
    private bool m_StartRunning = true;
    private float m_X = 0;
    private float m_Z = 0;
    private Vector3 m_LastPosition;
    [SerializeField]
    private PuppetMaster m_PuppetMaster = null;
    [SerializeField]
    private Animator m_Animator = null;
    [SerializeField]
    private Rigidbody m_Ridigbody = null;

    void OnEnable()
    {
        m_LastPosition = transform.position;
        m_X = Random.Range(-0.5f, 0.5f);
        m_Z = Random.Range(-0.5f, 0.5f);
    }

    public void Ragdoll(float explosionForce, Vector3 explosionPosition, float explosionRadius, float upwardsModifier = 0.0f, ForceMode mode = ForceMode.Force)
    {
        m_StartRunning = false;
        m_PuppetMaster.mode = PuppetMaster.Mode.Active;
        m_PuppetMaster.state = PuppetMaster.State.Dead;
    }

    private void Update()
    {
        if (m_StartRunning)
        {
            m_Animator.SetFloat("Forward", 1f);
            transform.position = Vector3.Lerp(transform.position, transform.position + (transform.forward + (new Vector3(m_X, 0, m_Z) * Speed)), Time.deltaTime);
            var dir = transform.position - m_LastPosition;
            dir = new Vector3(dir.x, 0, dir.z);
            transform.rotation = Quaternion.LookRotation(dir, transform.up);
            m_LastPosition = transform.position;
        }
    }

}

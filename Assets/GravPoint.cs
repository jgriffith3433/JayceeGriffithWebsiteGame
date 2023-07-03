using UnityEngine;

public class GravPoint : MonoBehaviour
{
    [SerializeField] private float m_Speed = 5f;
    [SerializeField] private Transform m_Target = null;
    [SerializeField] private TestPlayer m_Player = null;

    private bool m_SlowlyMoveTowardsTarget = true;

    public void SetTarget(Transform target)
    {
        m_Target = target;
    }

    private void Update()
    {
        if (!m_Player.tno.isMine)
        {
            gameObject.SetActive(false);
            return;
        }
        if (m_SlowlyMoveTowardsTarget)
        {
            if (m_Target != null)
            {
                transform.position = Vector3.MoveTowards(transform.position, m_Target.position, m_Speed * Time.deltaTime);
            }
        }
    }

    public void ChangeSlowlyMoveTowardsTarget(bool value)
    {
        m_SlowlyMoveTowardsTarget = value;
    }
}
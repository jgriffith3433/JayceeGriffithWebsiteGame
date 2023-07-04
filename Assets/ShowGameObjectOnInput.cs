using System.Collections;
using UnityEngine;

public class ShowGameObjectOnInput : MonoBehaviour
{
    [SerializeField]
    private GameObject m_GO = null;
    [SerializeField]
    private float m_HideAgainAfterTime = 3f;

    private Coroutine m_Coroutine = null;

    private void Update()
    {
        if (Input.touchCount > 0 || Input.GetMouseButtonDown(0))
        {
            m_GO.SetActive(true);
            if (m_Coroutine != null)
            {
                StopCoroutine(m_Coroutine);
                m_Coroutine = null;
            }
            m_Coroutine = StartCoroutine(HideAfterTime(m_HideAgainAfterTime));
        }
    }

    private IEnumerator HideAfterTime(float t)
    {
        yield return new WaitForSeconds(t);
        m_GO.SetActive(false);
    }
}

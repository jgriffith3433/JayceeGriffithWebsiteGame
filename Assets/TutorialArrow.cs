using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialArrow : MonoBehaviour
{
    public Vector2 m_BounceVector = Vector2.zero;

    private float t = 0f;
    private Vector2 m_StartingPosition;
    private int m_CurrentDirection = 1;
    private RectTransform m_RectTransform = null;
    
    private void Awake()
    {
        m_RectTransform = GetComponent<RectTransform>();
        if (m_RectTransform != null)
        {
            m_StartingPosition = m_RectTransform.localPosition;
        }
        else
        {
            m_StartingPosition = transform.localPosition;
        }
    }

    void Update()
    {
        if (m_RectTransform != null)
        {
            m_RectTransform.localPosition = Vector2.Lerp(m_StartingPosition, m_StartingPosition + m_BounceVector, t);
        }
        else
        {
            transform.localPosition = Vector2.Lerp(m_StartingPosition, m_StartingPosition + m_BounceVector, t);
        }
        t += m_CurrentDirection * Time.deltaTime;

        if (t >= 1.0f)
        {
            m_CurrentDirection = -1;
            t = 1;
        }
        else if (t <= 0)
        {
            m_CurrentDirection = 1;
            t = 0;
        }
    }
}

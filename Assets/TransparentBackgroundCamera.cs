using System.Collections.Generic;
using UnityEngine;

public class TransparentBackgroundCamera : MonoBehaviour
{
    private static List<TransparentBackgroundCamera> m_Instances = null;

    private void Awake()
    {
        if (m_Instances == null)
        {
            m_Instances = new List<TransparentBackgroundCamera>();
        }
        foreach(var camInstance in m_Instances)
        {
            camInstance.gameObject.SetActive(false);
        }
        m_Instances.Add(this);
    }

    private void OnDestroy()
    {
        m_Instances.Remove(this);
        if (m_Instances.Count > 0)
        {
            m_Instances[m_Instances.Count - 1].gameObject.SetActive(true);
        }
    }

    void OnPreRender()
    {
        TransparentBackground.ClearCanvas();
    }
}

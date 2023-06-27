using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveWhenCameraAtZero : MonoBehaviour
{
    public GameObject m_GameObjectToActivate = null;

    private void Update()
    {
        if (m_GameObjectToActivate == null)
        {
            return;
        }
        m_GameObjectToActivate.SetActive(Camera.main.transform.position.x == 0);
    }
}

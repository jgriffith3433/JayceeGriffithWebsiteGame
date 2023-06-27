using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnDestroyDebug : MonoBehaviour
{
    void OnDestroy()
    {
        Debug.Log("Destroyed");
    }
}

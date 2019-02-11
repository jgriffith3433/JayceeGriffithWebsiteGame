using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatchFallTrigger : MonoBehaviour
{
    public GameObject GameCharacter;
    public GameObject SpawnPos;

    private void OnTriggerEnter(Collider other)
    {
        NavigationController.Instance.OnFall();
    }
}

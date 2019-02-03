using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatchFallTrigger : MonoBehaviour
{
    public GameObject GameCharacter;
    private void OnTriggerEnter(Collider other)
    {
        GameCharacter.transform.position = new Vector3(0, 20, 0);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageController : MonoBehaviour
{
    public GameObject SpawnPos;

    void Awake()
    {
        GameController.Instance.SpawnPos = SpawnPos;
        GameController.Instance.SpawnCharacter();
        GameController.Instance.CallGetDimensions();
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NavigationController : MonoBehaviour
{
    public static NavigationController Instance;
    void Awake()
    {
        Instance = this;
    }

    public TextMeshPro LeftText;
    public TextMeshPro RightText;

    public void GoPrevious()
    {
        GameController.Instance.NavigateToPage(LeftText.text);
    }

    public void GoNext()
    {
        GameController.Instance.NavigateToPage(RightText.text);
    }

    public void OnFall()
    {
        GameController.Instance.SpawnCharacter(1);
    }
}

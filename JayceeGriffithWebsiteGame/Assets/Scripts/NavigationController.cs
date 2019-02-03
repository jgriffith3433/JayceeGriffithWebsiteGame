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
    public TextMeshPro MiddleText;
    public TextMeshPro RightText;

    public string CurrentPageName { get; set; }
    public string PreviousPageName { get; set; }
    public string NextPageName { get; set; }

    public void UpdateNavigation()
    {
        LeftText.SetText(PreviousPageName);
        MiddleText.SetText(CurrentPageName);
        RightText.SetText(NextPageName);
    }

    public void GoPrevious()
    {
        GameController.Instance.NavigateToPage(PreviousPageName);
    }

    public void GoNext()
    {
        GameController.Instance.NavigateToPage(NextPageName);
    }
}

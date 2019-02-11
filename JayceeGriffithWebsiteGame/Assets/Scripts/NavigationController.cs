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

    public NextTrigger Next;
    public PreviousTrigger Previous;

    public string CurrentPageName { get; set; }
    public string PreviousPageName { get; set; }
    public string NextPageName { get; set; }
    public bool PreviousDropDown { get; set; }
    public bool NextDropDown { get; set; }

    public void UpdateNavigation()
    {
        LeftText.SetText(PreviousPageName);
        MiddleText.SetText(CurrentPageName);
        RightText.SetText(NextPageName);
        if (NextDropDown)
        {
            AdjustableSceneController.Instance.SetNextDropDown(true);
            Next.DisableTrigger();
        }
        else
        {
            AdjustableSceneController.Instance.SetNextDropDown(false);
            Next.EnableTrigger();
        }
        if (PreviousDropDown)
        {
            AdjustableSceneController.Instance.SetPreviousDropDown(true);
            Previous.DisableTrigger();
        }
        else
        {
            AdjustableSceneController.Instance.SetPreviousDropDown(false);
            Previous.EnableTrigger();
        }
    }

    public void GoPrevious()
    {
        GameController.Instance.NavigateToPage(PreviousPageName);
    }

    public void GoNext()
    {
        GameController.Instance.NavigateToPage(NextPageName);
    }

    public void OnFall()
    {
        if (NextDropDown)
        {
            GameController.Instance.NavigateToPage(NextPageName);
        }
        else if (PreviousDropDown)
        {
            GameController.Instance.NavigateToPage(PreviousPageName);
        }
        GameController.Instance.SpawnCharacter(1);
    }
}

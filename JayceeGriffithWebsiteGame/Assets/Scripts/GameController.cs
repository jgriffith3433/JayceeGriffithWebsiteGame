using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void SetPage(string pageName);

    [DllImport("__Internal")]
    private static extern void OnReady();

    [DllImport("__Internal")]
    private static extern void GetPageNavigation();

    public static GameController Instance;

    private Dictionary<string, string> PageNav;

    public void Awake()
    {
        Instance = this;
        PageNav = new Dictionary<string, string>();
        PageNav.Add("", "");
        PageNav.Add("/", "Home");
        PageNav.Add("/portfolio", "Portfolio");
        PageNav.Add("/reading", "Reading");
    }

    public IEnumerator Start()
    {
        yield return new WaitForSeconds(1);
        try
        {
            OnReady();
        }
        catch
        {

        }
    }
    #region ToReact
    public void NavigateToPage(string pageName)
    {
        foreach (var pageNavKey in PageNav.Keys)
        {
            if (PageNav[pageNavKey] == pageName)
            {
                try
                {
                    SetPage(pageNavKey);
                }
                catch
                {

                }
                break;
            }
        }
    }
    #endregion

    #region FromReact
    public void Connected()
    {
        try
        {
            GetPageNavigation();
        }
        catch
        {

        }
    }

    public void UpdatePageNavigation(string pageNavJson)
    {
        var jsonObj = JSON.Parse(pageNavJson);
        NavigationController.Instance.CurrentPageName = PageNav[jsonObj["currentLocation"].Value];
        NavigationController.Instance.NextPageName = PageNav[jsonObj["nextLocation"].Value];
        NavigationController.Instance.PreviousPageName = PageNav[jsonObj["prevLocation"].Value];
        NavigationController.Instance.UpdateNavigation();
    }
    #endregion
}
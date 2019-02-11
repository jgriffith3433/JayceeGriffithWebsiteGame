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

    [DllImport("__Internal")]
    private static extern void GetPageWidth();

    public static GameController Instance;

    private Dictionary<string, string> PageNav;
    public BasicBehaviour GameCharacter;
    public GameObject SpawnPos;

    public void Awake()
    {
        Instance = this;
        PageNav = new Dictionary<string, string>();
        PageNav.Add("", "");
        PageNav.Add("/", "Home");
        PageNav.Add("/portfolio", "Portfolio");
        PageNav.Add("/about/me", "About Me");
        PageNav.Add("/about/music", "Music");
        PageNav.Add("/about/reading", "Reading");
    }

    public IEnumerator Start()
    {
        yield return new WaitForSeconds(1);
        try
        {
            OnReady();
            GetPageWidth();
        }
        catch
        {
            Debug.Log("Error in calling: OnReady");
            Debug.Log("Error in calling: GetPageWidth");
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
                    Debug.Log("Error in calling: SetPage");
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
            Debug.Log("Error in calling: GetPageNavigation");
        }
    }

    public void SpawnCharacter(float waitTime = 0.0f)
    {
        StartCoroutine(DoSpawnCharacter(waitTime));
    }

    private IEnumerator DoSpawnCharacter(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        GameCharacter.transform.position = SpawnPos.transform.position;
        var rb = GameCharacter.GetRigidBody;
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
        }
    }

    public void UpdatePageNavigation(string pageNavJson)
    {
        var jsonObj = JSON.Parse(pageNavJson);
        NavigationController.Instance.CurrentPageName = PageNav[jsonObj["currentLocation"].Value];
        NavigationController.Instance.NextPageName = PageNav[jsonObj["nextLocation"].Value];
        NavigationController.Instance.PreviousPageName = PageNav[jsonObj["prevLocation"].Value];
        NavigationController.Instance.NextDropDown = jsonObj["nextDropDown"].AsBool;
        NavigationController.Instance.PreviousDropDown = jsonObj["prevDropDown"].AsBool;
        NavigationController.Instance.UpdateNavigation();
        AdjustableSceneController.Instance.Adjust();
    }

    public void UpdatePageWidth(string width)
    {
        AdjustableSceneController.Instance.SetPageWidth(float.Parse(width));
        AdjustableSceneController.Instance.Adjust();
        SpawnCharacter();
    }
    #endregion
}
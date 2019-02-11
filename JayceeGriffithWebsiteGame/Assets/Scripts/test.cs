using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    [TextArea(5, 5)]
    public string pagenav;
    [TextArea(5, 5)]
    public string nextpagenav;
    public static test Instance;
    private void Awake()
    {
        Instance = this;
        Debug.Log("Viewport Width: " + Screen.width);
    }
    IEnumerator Start()
    {
        yield return new WaitForSeconds(1);
        GameController.Instance.UpdatePageNavigation(pagenav);
        GameController.Instance.UpdatePageWidth("600");
    }

    public void NavigateToPage(string NextPageName)
    {
        GameController.Instance.UpdatePageNavigation(nextpagenav);
    }
}

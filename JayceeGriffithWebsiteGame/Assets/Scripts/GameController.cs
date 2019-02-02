using System.Collections;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void SetPage(string pageName);

    [DllImport("__Internal")]
    private static extern void OnReady();

    public static GameController Instance;

    public TextMeshPro PageNameText;
    private string PageName;

    public void Awake()
    {
        Instance = this;
    }

    public IEnumerator Start()
    {
        yield return new WaitForSeconds(1);
        OnReady();
    }

    public void ToReactSetPage(string pageName)
    {
        SetPage(pageName);
    }

    public void FromReactUpdatePage(string pageName)
    {
        PageName = pageName;
        PageNameText.SetText(PageName);
    }
}
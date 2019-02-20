using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    public string page;
    public static test Instance;
    public float[] testwidths;
    private void Awake()
    {
        Instance = this;
    }
    IEnumerator Start()
    {
        yield return new WaitForSeconds(1);
        GameController.Instance.LoadPage(page);
        yield return new WaitForSeconds(1);
        StartCoroutine(TestWidths());
    }

    IEnumerator TestWidths()
    {
        foreach (var w in testwidths)
        {
            AdjustableSceneController.Instance.SetPageWidth(w);
            AdjustableSceneController.Instance.Adjust();
            yield return new WaitForSeconds(2);
        }
    }
}

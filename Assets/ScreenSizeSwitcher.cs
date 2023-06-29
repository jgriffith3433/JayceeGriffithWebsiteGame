using UnityEngine;
using System.Collections.Generic;

public class ScreenSizeSwitcher : MonoBehaviour
{
    [SerializeField]
    private Dictionary<float, float> m_SwitchSizes = new Dictionary<float, float>
    {
        { 0, 350 },
        { 350, 950 },
        { 950, 99999 },
    };

    [SerializeField]
    private GameObject[] m_SwitchObjects = null;

    private void Update()
    {
        var foundSize = false;
        var currentObjectIndex = 0;
        foreach(var switchSizeKvp in m_SwitchSizes)
        {
            var inRange = Screen.width >= switchSizeKvp.Key && Screen.width <= switchSizeKvp.Value;
            m_SwitchObjects[currentObjectIndex].SetActive(inRange && !foundSize);
            if (inRange)
            {
                foundSize = true;
            }
            currentObjectIndex++;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustableSceneController : MonoBehaviour
{
    public static AdjustableSceneController Instance;
    private void Awake()
    {
        Instance = this;
    }

    public GameObject Floor;
    public GameObject PreviousTrigger;
    public GameObject NextTrigger;
    public RectTransform Previous;
    public RectTransform PreviousArrow;
    public RectTransform Next;
    public RectTransform NextArrow;


    public void AdjustSceneToWidth(float width)
    {
        var sceneAdjustmentRatio = .9f;
        Floor.transform.localScale = new Vector3(width / 50 * sceneAdjustmentRatio, Floor.transform.localScale.y, Floor.transform.localScale.z);
        PreviousTrigger.transform.position = new Vector3(-width / 125 * sceneAdjustmentRatio, PreviousTrigger.transform.position.y, PreviousTrigger.transform.position.z);
        NextTrigger.transform.position = new Vector3(width / 125 * sceneAdjustmentRatio, NextTrigger.transform.position.y, NextTrigger.transform.position.z);
        Previous.sizeDelta = new Vector2(width / 35 * sceneAdjustmentRatio, Previous.sizeDelta.y);
        PreviousArrow.sizeDelta = new Vector2(width / 35 * sceneAdjustmentRatio, PreviousArrow.sizeDelta.y);
        Next.sizeDelta = new Vector2(width / 35 * sceneAdjustmentRatio, Next.sizeDelta.y);
        NextArrow.sizeDelta = new Vector2(width / 35 * sceneAdjustmentRatio, NextArrow.sizeDelta.y);
    }
}

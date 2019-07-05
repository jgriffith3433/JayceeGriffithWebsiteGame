using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AdjustableSceneController : MonoBehaviour
{
    public static AdjustableSceneController Instance;
    private void Awake()
    {
        Instance = this;
    }

    public GameObject SpawnPos;

    public GameObject Floor;
    public GameObject PreviousTrigger;
    public GameObject NextTrigger;
    public RectTransform Previous;
    public TextMeshPro PreviousText;
    public RectTransform PreviousArrow;
    public TextMeshPro PreviousArrowText;
    public RectTransform Next;
    public TextMeshPro NextText;
    public RectTransform NextArrow;
    public TextMeshPro NextArrowText;

    private float PageWidth = 960.0f;
    private float SceneAdjustmentRatio = .9f;

    public float NextOffset = 0.0f;

    public void SetPageWidth(float width)
    {
        PageWidth = width;
    }

    public void Adjust()
    {
        AdjustToWidth();
    }

    private void AdjustToWidth()
    {
        var fontSizeClamped = Mathf.Clamp(PageWidth / 75, 8, 12);
        var gcOldPos = GameController.Instance.GameCharacter.transform.position;
        Floor.transform.localScale = new Vector3(PageWidth / 100 * SceneAdjustmentRatio, Floor.transform.localScale.y, Floor.transform.localScale.z);
        PreviousTrigger.transform.position = new Vector3(-PageWidth / 200 * SceneAdjustmentRatio, PreviousTrigger.transform.position.y, PreviousTrigger.transform.position.z);
        NextTrigger.transform.position = new Vector3((PageWidth / 200 * SceneAdjustmentRatio) + NextOffset, NextTrigger.transform.position.y, NextTrigger.transform.position.z);
        Previous.sizeDelta = new Vector2(PageWidth / 35 * SceneAdjustmentRatio, Previous.sizeDelta.y);
        PreviousText.fontSize = fontSizeClamped;
        PreviousArrow.sizeDelta = new Vector2(PageWidth / 35 * SceneAdjustmentRatio, PreviousArrow.sizeDelta.y);
        PreviousArrowText.fontSize = fontSizeClamped;
        Next.sizeDelta = new Vector2(PageWidth / 35 * SceneAdjustmentRatio, Next.sizeDelta.y);
        NextText.fontSize = fontSizeClamped;
        NextArrow.sizeDelta = new Vector2(PageWidth / 35 * SceneAdjustmentRatio, NextArrow.sizeDelta.y);
        NextArrowText.fontSize = fontSizeClamped;
        GameController.Instance.GameCharacter.transform.position = gcOldPos;
    }
}

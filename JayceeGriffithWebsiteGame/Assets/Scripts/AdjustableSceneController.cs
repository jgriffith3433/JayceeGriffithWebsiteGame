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

    public GameObject SpawnPos;

    public GameObject Floor;
    public GameObject PreviousTrigger;
    public GameObject NextTrigger;
    public RectTransform Previous;
    public RectTransform PreviousArrow;
    public RectTransform Next;
    public RectTransform NextArrow;

    private bool NextDropDown;
    private bool PreviousDropDown;
    private float PageWidth = 960.0f;
    private float SceneAdjustmentRatio = .9f;

    public void SetNextDropDown(bool nextDropDown)
    {
        NextDropDown = nextDropDown;
    }

    public void SetPreviousDropDown(bool previousDropDown)
    {
        PreviousDropDown = previousDropDown;
    }

    public void SetPageWidth(float width)
    {
        SceneAdjustmentRatio = .9f;
        PageWidth = width;
    }

    public void Adjust()
    {
        AdjustToWidth();
        AdjustForDropDowns();
    }

    private void AdjustToWidth()
    {
        Floor.transform.localScale = new Vector3(PageWidth / 50 * SceneAdjustmentRatio, Floor.transform.localScale.y, Floor.transform.localScale.z);
        PreviousTrigger.transform.position = new Vector3(-PageWidth / 125 * SceneAdjustmentRatio, PreviousTrigger.transform.position.y, PreviousTrigger.transform.position.z);
        NextTrigger.transform.position = new Vector3(PageWidth / 125 * SceneAdjustmentRatio, NextTrigger.transform.position.y, NextTrigger.transform.position.z);
        Previous.sizeDelta = new Vector2(PageWidth / 35 * SceneAdjustmentRatio, Previous.sizeDelta.y);
        PreviousArrow.sizeDelta = new Vector2(PageWidth / 35 * SceneAdjustmentRatio, PreviousArrow.sizeDelta.y);
        Next.sizeDelta = new Vector2(PageWidth / 35 * SceneAdjustmentRatio, Next.sizeDelta.y);
        NextArrow.sizeDelta = new Vector2(PageWidth / 35 * SceneAdjustmentRatio, NextArrow.sizeDelta.y);
    }

    private void AdjustForDropDowns()
    {
        if (NextDropDown)
        {
            SpawnPos.transform.position = new Vector3(0, SpawnPos.transform.position.y, SpawnPos.transform.position.z);
            //SpawnPos.transform.position = new Vector3(-PageWidth / 200 * SceneAdjustmentRatio, SpawnPos.transform.position.y, SpawnPos.transform.position.z);
            Floor.transform.position = new Vector3(-PageWidth / 150, Floor.transform.position.y, Floor.transform.position.z);
            NextTrigger.transform.position = new Vector3((PageWidth / 2) / 125 * SceneAdjustmentRatio, NextTrigger.transform.position.y, NextTrigger.transform.position.z);
            Next.sizeDelta = new Vector2((PageWidth / 2) / 35 * SceneAdjustmentRatio, Next.sizeDelta.y);
            NextArrow.sizeDelta = new Vector2((PageWidth / 2) / 35 * SceneAdjustmentRatio, NextArrow.sizeDelta.y);
        }
        else if (PreviousDropDown)
        {
            SpawnPos.transform.position = new Vector3(0, SpawnPos.transform.position.y, SpawnPos.transform.position.z);
            //SpawnPos.transform.position = new Vector3(PageWidth / 200 * SceneAdjustmentRatio, SpawnPos.transform.position.y, SpawnPos.transform.position.z);
            Floor.transform.position = new Vector3(PageWidth / 150, Floor.transform.position.y, Floor.transform.position.z);
            PreviousTrigger.transform.position = new Vector3((PageWidth / 2) / 125 * SceneAdjustmentRatio, PreviousTrigger.transform.position.y, PreviousTrigger.transform.position.z);
            Previous.sizeDelta = new Vector2((PageWidth / 2) / 35 * SceneAdjustmentRatio, Previous.sizeDelta.y);
            PreviousArrow.sizeDelta = new Vector2((PageWidth / 2) / 35 * SceneAdjustmentRatio, PreviousArrow.sizeDelta.y);
        }
        else
        {
            SpawnPos.transform.position = new Vector3(0, SpawnPos.transform.position.y, SpawnPos.transform.position.z);
            Floor.transform.position = new Vector3(0, Floor.transform.position.y, Floor.transform.position.z);
            Floor.transform.localScale = new Vector3(PageWidth / 50 * SceneAdjustmentRatio, Floor.transform.localScale.y, Floor.transform.localScale.z);

            PreviousTrigger.transform.position = new Vector3(-PageWidth / 125 * SceneAdjustmentRatio, PreviousTrigger.transform.position.y, PreviousTrigger.transform.position.z);
            NextTrigger.transform.position = new Vector3(PageWidth / 125 * SceneAdjustmentRatio, NextTrigger.transform.position.y, NextTrigger.transform.position.z);
            Previous.sizeDelta = new Vector2(PageWidth / 35 * SceneAdjustmentRatio, Previous.sizeDelta.y);
            PreviousArrow.sizeDelta = new Vector2(PageWidth / 35 * SceneAdjustmentRatio, PreviousArrow.sizeDelta.y);
            Next.sizeDelta = new Vector2(PageWidth / 35 * SceneAdjustmentRatio, Next.sizeDelta.y);
            NextArrow.sizeDelta = new Vector2(PageWidth / 35 * SceneAdjustmentRatio, NextArrow.sizeDelta.y);
        }
    }
}

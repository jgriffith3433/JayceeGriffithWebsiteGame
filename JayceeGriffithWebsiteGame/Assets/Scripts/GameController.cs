using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void MouseEvent(string mouseEvent);

    [DllImport("__Internal")]
    private static extern void SetPage(string pageName);

    [DllImport("__Internal")]
    private static extern void OnReady();

    [DllImport("__Internal")]
    private static extern void GetDimensions();

    public static GameController Instance;

    private Dictionary<string, string> PageNav;
    public BasicBehaviour GameCharacter;
    public Quaternion OriginalLookRot;
    public GameObject SpawnPos;
    public float PageWidth;
    public float PageHeight;

    public GameObject ParticleAttractorTarget;
    public ParticleSystem ParticleAttractor_Spark;
    public GameObject ParticleAttractor;
    public GameObject ParticleAttractor_Click;
    public Coroutine ClickParticleCoroutine;
    public Coroutine DragParticleCoroutine;
    private Vector3 MoveToVector;

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
        OriginalLookRot = GameCharacter.transform.rotation;
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
            Debug.Log("Error in calling: OnReady");
        }

        yield return new WaitForSeconds(5);
        StartCoroutine(DoSpark());
    }

    private IEnumerator DoSpark()
    {
        if (!ParticleAttractor.activeSelf && !ParticleAttractor_Click.activeSelf)
        {
            ParticleAttractor_Spark.Play();
        }
        yield return new WaitForSeconds(Random.Range(2.5f, 15.0f));
        StartCoroutine(DoSpark());
    }

    public void Update()
    {
        HandleMouseEvents();
        HandleTouchEvents();
        HandleExternalMovement();
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
        GameCharacter.transform.rotation = OriginalLookRot;
        GameCharacter.AddExternalInput(new Vector2(0, 1), 1);
    }

    public void ShowClickParticle(Vector3 pos, float initWaitTime = 0.25f, float waitTime = 1.0f)
    {
        ParticleAttractor_Click.transform.position = pos;
        if (ClickParticleCoroutine != null)
        {
            StopCoroutine(ClickParticleCoroutine);
        }
        ClickParticleCoroutine = StartCoroutine(DoShowClickParticle(initWaitTime, waitTime));
    }

    private IEnumerator DoShowClickParticle(float initWaitTime, float waitTime)
    {
        yield return new WaitForSeconds(initWaitTime);
        ParticleAttractor_Click.SetActive(true);
        yield return new WaitForSeconds(waitTime);
        ParticleAttractor_Click.SetActive(false);
        ClickParticleCoroutine = null;
    }

    public void HideDragParticle(float waitTime = 0.25f)
    {
        if (DragParticleCoroutine != null)
        {
            StopCoroutine(DragParticleCoroutine);
        }
        DragParticleCoroutine = StartCoroutine(DoHideDragParticle(waitTime));
    }

    private IEnumerator DoHideDragParticle(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        ParticleAttractor.SetActive(false);
        DragParticleCoroutine = null;
    }

    private void HandleExternalMovement()
    {
        if (MoveToVector != Vector3.zero)
        {
            var moveVector = (MoveToVector - GameCharacter.transform.position);
            var moveDir = moveVector.normalized;

            if (moveVector.magnitude > 1.5f)
            {
                var moveInput = Vector2.zero;
                if (moveDir.x > 0)
                {
                    moveInput = new Vector2(0.5f, 0);
                }
                else if (moveDir.x < 0)
                {
                    moveInput = new Vector2(-0.5f, 0);
                }
                GameCharacter.SetMoveInput(moveInput);
            }
            else
            {
                MoveToVector = Vector3.zero;
                GameCharacter.SetMoveInput(Vector2.zero);
            }
        }
        else
        {
            MoveToVector = Vector3.zero;
            GameCharacter.SetMoveInput(Vector2.zero);
        }
    }

    private void CheckMoveToTouch(Vector3 movePosition)
    {
        var checkDir = (movePosition - Camera.main.transform.position);
        int layerMask = 1 << 8;
        RaycastHit info;
        Debug.DrawRay(Camera.main.transform.position, checkDir, Color.red, 5);
        if (Physics.Raycast(new Ray(Camera.main.transform.position, checkDir), out info, 20.0f, layerMask, QueryTriggerInteraction.Collide))
        {
            MoveToVector = new Vector3(info.point.x, -2, 0);
        }
        else
        {
            MoveToVector = Vector3.zero;
        }
    }

    #region ToReact
    private void HandleMouseEvents()
    {
        try
        {
            var screenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
            screenPoint.z = 10.0f;
            ParticleAttractorTarget.transform.position = Camera.main.ScreenToWorldPoint(screenPoint);
            ParticleAttractor_Spark.transform.position = Camera.main.ScreenToWorldPoint(screenPoint);

            var mouseEvent = new JSONObject();
            mouseEvent.Add("type", new JSONString("mousemove"));
            mouseEvent.Add("pointerX", new JSONNumber(Input.mousePosition.x));
            mouseEvent.Add("pointerY", new JSONNumber(Input.mousePosition.y));
            MouseEvent(mouseEvent.ToString());
        }
        catch
        {
            //Debug.Log("Error in calling: MouseEvent");
        }
        if (Input.GetMouseButtonDown(0))
        {
            try
            {
                ParticleAttractor.SetActive(true);
                var mouseEvent = new JSONObject();
                mouseEvent.Add("type", new JSONString("mousedown"));
                mouseEvent.Add("pointerX", new JSONNumber(Input.mousePosition.x));
                mouseEvent.Add("pointerY", new JSONNumber(Input.mousePosition.y));
                MouseEvent(mouseEvent.ToString());
            }
            catch
            {
                Debug.Log("Error in calling: MouseEvent");
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            try
            {
                HideDragParticle();
                var screenP = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
                screenP.z = 10.0f;
                ShowClickParticle(Camera.main.ScreenToWorldPoint(screenP));
                CheckMoveToTouch(Camera.main.ScreenToWorldPoint(screenP));

                var mouseEvent = new JSONObject();
                mouseEvent.Add("type", new JSONString("mouseup"));
                mouseEvent.Add("pointerX", new JSONNumber(Input.mousePosition.x));
                mouseEvent.Add("pointerY", new JSONNumber(Input.mousePosition.y));
                MouseEvent(mouseEvent.ToString());
            }
            catch
            {
                Debug.Log("Error in calling: MouseEvent");
            }
            try
            {
                var mouseEvent = new JSONObject();
                mouseEvent.Add("type", new JSONString("click"));
                mouseEvent.Add("pointerX", new JSONNumber(Input.mousePosition.x));
                mouseEvent.Add("pointerY", new JSONNumber(Input.mousePosition.y));
                MouseEvent(mouseEvent.ToString());
            }
            catch
            {
                Debug.Log("Error in calling: MouseEvent");
            }
        }
        var mouseScroll = Input.GetAxis("Mouse ScrollWheel");
        if (mouseScroll != 0)
        {
            try
            {
                var mouseEvent = new JSONObject();
                mouseEvent.Add("type", new JSONString("scroll"));
                mouseEvent.Add("scroll", new JSONNumber(mouseScroll));
                mouseEvent.Add("pointerX", new JSONNumber(Input.mousePosition.x));
                mouseEvent.Add("pointerY", new JSONNumber(Input.mousePosition.y));
                MouseEvent(mouseEvent.ToString());
            }
            catch
            {
                Debug.Log("Error in calling: MouseEvent");
            }
        }
    }

    private void HandleTouchEvents()
    {
        if (Input.touchCount > 0)
        {
            var firstTouch = Input.GetTouch(0);

            var screenPoint = new Vector3(firstTouch.position.x, firstTouch.position.y, 0);
            screenPoint.z = 10.0f;
            ParticleAttractorTarget.transform.position = Camera.main.ScreenToWorldPoint(screenPoint);
            ParticleAttractor_Spark.transform.position = Camera.main.ScreenToWorldPoint(screenPoint);

            if (Input.touchCount == 1)
            {
                if (firstTouch.phase == TouchPhase.Began)
                {
                    try
                    {
                        ParticleAttractor.SetActive(true);

                        var mouseEvent = new JSONObject();
                        mouseEvent.Add("type", new JSONString("mousedown"));
                        mouseEvent.Add("pointerX", new JSONNumber(firstTouch.position.x));
                        mouseEvent.Add("pointerY", new JSONNumber(firstTouch.position.y));
                        MouseEvent(mouseEvent.ToString());
                    }
                    catch
                    {
                        Debug.Log("Error in calling: MouseEvent");
                    }
                }
                if (firstTouch.phase == TouchPhase.Moved)
                {
                    var swipeSpeed = firstTouch.deltaPosition.magnitude / firstTouch.deltaTime;
                    var swipeVector = firstTouch.deltaPosition * swipeSpeed;
                    if (swipeSpeed > 0.1f)
                    {
                        try
                        {
                            var mouseEvent = new JSONObject();
                            mouseEvent.Add("type", new JSONString("scroll"));
                            mouseEvent.Add("scroll", new JSONNumber(swipeVector.y));
                            mouseEvent.Add("pointerX", new JSONNumber(firstTouch.position.x));
                            mouseEvent.Add("pointerY", new JSONNumber(firstTouch.position.y));
                            MouseEvent(mouseEvent.ToString());
                        }
                        catch
                        {
                            Debug.Log("Error in calling: MouseEvent");
                        }
                    }
                }
                if (firstTouch.phase == TouchPhase.Ended)
                {
                    try
                    {
                        HideDragParticle();
                        var screenP = new Vector3(firstTouch.position.x, firstTouch.position.y, 0);
                        screenP.z = 10.0f;
                        ShowClickParticle(Camera.main.ScreenToWorldPoint(screenP));
                        CheckMoveToTouch(Camera.main.ScreenToWorldPoint(screenP));

                        var mouseEvent = new JSONObject();
                        mouseEvent.Add("type", new JSONString("mouseup"));
                        mouseEvent.Add("pointerX", new JSONNumber(firstTouch.position.x));
                        mouseEvent.Add("pointerY", new JSONNumber(firstTouch.position.y));
                        MouseEvent(mouseEvent.ToString());
                    }
                    catch
                    {
                        Debug.Log("Error in calling: MouseEvent");
                    }
                    try
                    {
                        var mouseEvent = new JSONObject();
                        mouseEvent.Add("type", new JSONString("click"));
                        mouseEvent.Add("pointerX", new JSONNumber(firstTouch.position.x));
                        mouseEvent.Add("pointerY", new JSONNumber(firstTouch.position.y));
                        MouseEvent(mouseEvent.ToString());
                    }
                    catch
                    {
                        Debug.Log("Error in calling: MouseEvent");
                    }
                }
            }
            if (Input.touchCount == 2)
            {
            }
        }
    }

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

    public void CallGetDimensions()
    {
        try
        {
            GetDimensions();
        }
        catch
        {
            Debug.Log("Error in calling: GetDimensions");
        }
    }
    #endregion

    #region FromReact
    public void Connected()
    {
    }

    public void LoadPage(string page)
    {
        var pageGameObject = GameObject.Find("Page");
        if (pageGameObject != null)
        {
            Destroy(pageGameObject);
        }
        SceneManager.LoadScene(PageNav[page], LoadSceneMode.Additive);
    }

    public void UpdateDimensions(string dimensions)
    {
        var jsonDimensions = JSON.Parse(dimensions);
        PageWidth = jsonDimensions["width"].AsFloat;
        PageHeight = jsonDimensions["height"].AsFloat;
        if (AdjustableSceneController.Instance)
        {
            AdjustableSceneController.Instance.SetPageWidth(PageWidth);
            AdjustableSceneController.Instance.Adjust();
        }
    }
    #endregion
}
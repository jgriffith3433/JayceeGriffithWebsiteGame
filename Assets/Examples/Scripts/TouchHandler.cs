//-------------------------------------------------
//                    GNet 3
// Copyright © 2012-2018 Tasharen Entertainment Inc
//-------------------------------------------------

using UnityEngine;
using GNet;

/// <summary>
/// Very simple event manager script that sends out basic touch and mouse-based notifications using NGUI's syntax.
/// </summary>

[RequireComponent(typeof(Camera))]
public class TouchHandler : MonoBehaviour
{
    static public Vector3 worldPos;
    static public Vector2 screenPos;

    public LayerMask eventReceiverMask = -1;

    Camera mCam;
    GameObject mGo;
    public Vector3 m_PlaneNormal = new Vector3(0, 1, 0);
    void Awake() { mCam = GetComponent<Camera>(); }

    /// <summary>
    /// Update the touch and mouse position and send out appropriate events.
    /// </summary>

    void Update()
    {
        // Touch notifications
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            screenPos = touch.position;

            if (touch.phase == TouchPhase.Began)
            {
                SendPress(touch.position);
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                if (mGo != null)
                {
                    SendDrag(touch.position);
                }
            }
            else if (touch.phase != TouchPhase.Stationary)
            {
                SendRelease(touch.position);
            }
        }
        else
        {
            screenPos = Input.mousePosition;
            // Mouse notifications
            if (Input.GetMouseButtonDown(0))
            {
                SendPress(Input.mousePosition);
            }
            if (Input.GetMouseButtonUp(0)) SendRelease(Input.mousePosition);
            if (mGo != null && Input.GetMouseButton(0)) SendDrag(Input.mousePosition);
        }
    }

    /// <summary>
    /// Send out a press notification.
    /// </summary>

    void SendPress(Vector2 pos)
    {
        worldPos = pos;
        mGo = Raycast(pos, out worldPos);
        if (mGo != null)
        {
            mGo.SendMessage("OnPress", true, SendMessageOptions.DontRequireReceiver);
        }
    }

    /// <summary>
    /// Send out a release notification.
    /// </summary>

    void SendRelease(Vector2 pos)
    {
        worldPos = pos;

        if (mGo != null)
        {
            var go = Raycast(pos, out worldPos);
            if (mGo == go)
            {
                mGo.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
            }
            mGo.SendMessage("OnPress", false, SendMessageOptions.DontRequireReceiver);
            mGo = null;
        }
    }

    /// <summary>
    /// Send out a drag notification.
    /// </summary>

    void SendDrag(Vector2 pos)
    {
        Raycast(pos, out worldPos);
        var delta = pos - screenPos;
        //if (delta.sqrMagnitude > .001f)
        {
            mGo.SendMessage("OnDrag", delta, SendMessageOptions.DontRequireReceiver);
        }
        screenPos = pos;
        // ////http://wiki.unity3d.com/index.php/3d_Math_functions?_ga=2.185523936.899254785.1589669299-1860711680.1587336081
        // var currentProjectedOnPlaneNormal = Vector3.Dot(m_PlaneNormal, mGo.transform.position);
        // var cameraProjectedOnPlaneNormal = Vector3.Dot(m_PlaneNormal, mCam.transform.position);
        // var wantedDistanceFromCamera = cameraProjectedOnPlaneNormal - currentProjectedOnPlaneNormal;
        // var p = mCam.ScreenPointToRay(pos).direction;
        // var pOnPlaneNormal = Vector3.Dot(m_PlaneNormal, p);
        // p *= wantedDistanceFromCamera / -pOnPlaneNormal;
        // p += mCam.transform.position;

        // worldPos = p;
        // var delta = pos - screenPos;
        // //if (delta.sqrMagnitude > 0.001f)
        // //{
        // mGo.SendMessage("OnDrag", delta, SendMessageOptions.DontRequireReceiver);
        // screenPos = pos;
        //}
    }

    /// <summary>
    /// Helper function that raycasts into the screen to determine what's underneath the specified position.
    /// </summary>

    GameObject Raycast(Vector2 pos, out Vector3 hitPoint)
    {
        RaycastHit hit;

        if (Physics.Raycast(mCam.ScreenPointToRay(pos), out hit, 300f, eventReceiverMask))
        {
            hitPoint = hit.point;
            return hit.collider.gameObject;
        }
        hitPoint = Vector3.zero;
        return null;
    }
}

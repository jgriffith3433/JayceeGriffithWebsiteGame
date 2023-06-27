//-------------------------------------------------
//                    GNet 3
// Copyright © 2012-2018 Tasharen Entertainment Inc
//-------------------------------------------------

using UnityEngine;
using GNet;

/// <summary>
/// This script shows how it's possible to associate objects with players.
/// You can see it used on draggable cubes in Example 3.
/// </summary>

public class GravDraggedObject : TNBehaviour
{
    [SerializeField] private GravPoint m_GravPoint = null;
    [SerializeField] private Transform mTrans = null;
    [SerializeField] private TestPlayer m_Player = null;
	public bool zeroZAxis = false;


    private int m_OriginalLayer;
    Player mOwner;
    Vector3 mTarget;

    public void SetGravPointTarget(GravPoint gravPoint)
    {
        m_GravPoint = gravPoint;
        mTrans = gravPoint.transform;
    }

    protected override void Awake()
    {
        base.Awake();
        m_OriginalLayer = gameObject.layer;
    }

    /// <summary>
    /// Press / release event handler.
    /// </summary>

    void OnPress(bool isPressed)
    {
        if (!m_Player.tno.isMine)
        {
            return;
        }
        // When pressed on an object, claim it for the player (unless it was already claimed).
        if (isPressed)
        {
            ChangeSlowlyMoveTowardsTarget(false);
        }
        else
        {
            ChangeSlowlyMoveTowardsTarget(true);
        }
    }

    public void ChangeSlowlyMoveTowardsTarget(bool value)
    {
        m_GravPoint.ChangeSlowlyMoveTowardsTarget(value);
        m_Player.SendChangeSlowlyMoveTowardsTarget(value);
    }

    /// <summary>
    /// When the player is dragging the object around, update the target position for everyone.
    /// </summary>

    void OnDrag(Vector2 delta)
    {
        if (!m_Player.tno.isMine)
        {
            return;
        }
        mTarget = TouchHandler.worldPos;
        if (zeroZAxis)
        {
            mTarget = new Vector3(mTarget.x, mTarget.y + 2, 0);
        }
        mTrans.position = Vector3.Lerp(mTrans.position, mTarget, Time.deltaTime * 20f);
    }

    void OnUpdate()
    {
        if (zeroZAxis)
        {
            mTrans.position = new Vector3(mTrans.position.x, mTrans.position.y, 0);
        }
    }
}

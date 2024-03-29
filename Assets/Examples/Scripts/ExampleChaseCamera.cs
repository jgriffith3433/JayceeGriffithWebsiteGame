//-------------------------------------------------
//                    GNet 3
// Copyright © 2012-2018 Tasharen Entertainment Inc
//-------------------------------------------------

using UnityEngine;
using GNet;

/// <summary>
/// Very simple chase camera used on the Car example.
/// This script is attached to the "Chase Camera" object underneath the Car prefab.
/// It takes care of smoothly tweening the position and rotation of the chase camera.
/// Note that in order for this script to work properly the car's rigidbody must be set to "Interpolate".
/// </summary>

public class ExampleChaseCamera : TNBehaviour
{
	public Vector3 RotationOffset = new Vector3(0, 0, 0);
	public Vector3 PositionOffset = new Vector3(0, 0, 0);
	static public Transform target;
	public bool usePosition = true;
	public bool useRotation = true;
	public Transform mTrans;

	Vector3 mPos;
	Quaternion mRot;

	public override void OnStart ()
	{
		base.OnStart();

		if (tno == null || tno.isMine)
		{
			//mTrans = transform;
			mPos = mTrans.position;
			mRot = mTrans.rotation;
		}
		else Destroy(this);
	}

	void Update ()
	{
		if (target)
		{
			Transform t = mTrans;

			Vector3 pos = t.position;
			pos += PositionOffset;

			Vector3 forward = t.forward;
			forward.y = 0f;
			forward += RotationOffset;
			forward.Normalize();

			Quaternion rot = Quaternion.LookRotation(forward);

			float delta = Time.deltaTime;
			mPos = Vector3.Lerp(mPos, pos, delta * 8f);
			mRot = Quaternion.Slerp(mRot, rot, delta * 4f);

			if (usePosition)
			{
				target.position = mPos;
			}
			if (useRotation)
			{
				target.rotation = mRot;
			}
		}
	}
}

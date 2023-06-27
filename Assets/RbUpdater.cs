using GNet;
using UnityEngine;

[RequireComponent(typeof(TNObject))]
public class RbUpdater : MonoBehaviour
{
	/// <summary>
	/// Maximum number of updates per second when synchronizing the rigidbody.
	/// </summary>

	[Range(0.25f, 5f)]
	public float rigidbodyUpdates = 1f;

	/// <summary>
	/// We want to cache the network object (TNObject) we'll use for network communication.
	/// If the script was derived from TNBehaviour, this wouldn't have been necessary.
	/// </summary>

	[System.NonSerialized]
	public TNObject tno;

	protected Vector2 mLastInput;
	protected float mLastInputSend = 0f;
	protected float mNextRB = 0f;
	protected Rigidbody mRb = null;
	private Vector3 m_PosLastSent;
	private Quaternion m_RotLastSent;
	private Vector3 m_VelocityLastSent;
	private Vector3 m_AngularVelocityLastSent;
	private Player m_Owner = null;

	protected void Awake()
	{
		tno = GetComponent<TNObject>();
		mRb = GetComponent<Rigidbody>();
	}

	/// <summary>
	/// Only the car's owner should be updating the movement axes, and the result should be sync'd with other players.
	/// </summary>

	protected void Update()
	{
		if (mRb.IsSleeping())
		{
			if (m_Owner == TNManager.player)
			{
				ClaimObject(0, mRb.position, mRb.rotation, mRb.velocity, mRb.angularVelocity);
			}
			return;
		}
		float time = Time.time;
		// Since the input is sent frequently, rigidbody only needs to be corrected every couple of seconds.
		// Faster-paced games will require more frequent updates.
		if (mNextRB < time)
		{
			mNextRB = time + 1f / rigidbodyUpdates;
			if (m_PosLastSent != mRb.position || m_RotLastSent != mRb.rotation || m_VelocityLastSent != mRb.velocity || m_AngularVelocityLastSent != mRb.angularVelocity)
			{
				if (m_Owner == null)
				{
					ClaimObject(TNManager.playerID, mRb.position, mRb.rotation, mRb.velocity, mRb.angularVelocity);
					tno.Send(5, ForwardType.OthersSaved, TNManager.playerID, mRb.position, mRb.rotation, mRb.velocity, mRb.angularVelocity);
				}
				if (m_Owner == TNManager.player)
				{
					m_PosLastSent = mRb.position;
					m_RotLastSent = mRb.rotation;
					m_VelocityLastSent = mRb.velocity;
					m_AngularVelocityLastSent = mRb.angularVelocity;
					tno.Send("SetRB", ForwardType.OthersSaved, mRb.position, mRb.rotation, mRb.velocity, mRb.angularVelocity);
				}
			}
		}
	}

	/// <summary>
	/// RFC for the rigidbody will be called once per second by default.
	/// </summary>

	[RFC]
	protected void SetRB(Vector3 pos, Quaternion rot, Vector3 vel, Vector3 angVel)
	{
		mRb.position = pos;
		mRb.rotation = rot;
		mRb.velocity = vel;
		mRb.angularVelocity = angVel;

		m_PosLastSent = pos;
		m_RotLastSent = rot;
		m_VelocityLastSent = vel;
		m_AngularVelocityLastSent = angVel;
	}

	/// <summary>
	/// Remember the last player who claimed control of this object.
	/// </summary>

	[RFC(5)]
	void ClaimObject(int playerID, Vector3 pos, Quaternion rot, Vector3 vel, Vector3 angVel)
	{
		m_Owner = TNManager.GetPlayer(playerID);

		mRb.position = pos;
		mRb.rotation = rot;
		mRb.velocity = vel;
		mRb.angularVelocity = angVel;

		m_PosLastSent = pos;
		m_RotLastSent = rot;
		m_VelocityLastSent = vel;
		m_AngularVelocityLastSent = angVel;
	}
}

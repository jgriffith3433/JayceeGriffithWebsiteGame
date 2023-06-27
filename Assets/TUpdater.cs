using GNet;
using UnityEngine;

[RequireComponent(typeof(TNObject))]
public class TUpdater : MonoBehaviour
{
	[SerializeField] private Transform m_T = null;
	/// <summary>
	/// Maximum number of updates per second when synchronizing the rigidbody.
	/// </summary>

	[Range(0.25f, 5f)]
	public float tUpdates = 1f;

	/// <summary>
	/// We want to cache the network object (TNObject) we'll use for network communication.
	/// If the script was derived from TNBehaviour, this wouldn't have been necessary.
	/// </summary>

	[System.NonSerialized]
	public TNObject tno;

	protected Vector2 mLastInput;
	protected float mLastInputSend = 0f;
	protected float mNextT = 0f;

	protected void Awake()
	{
		tno = GetComponent<TNObject>();
		if (m_T == null)
		{
			m_T = transform;
		}
	}

	/// <summary>
	/// Only the car's owner should be updating the movement axes, and the result should be sync'd with other players.
	/// </summary>

	protected void Update()
	{
		if (!tno.isMine)
		{
			return;
		}
		float time = Time.time;
		// Since the input is sent frequently, rigidbody only needs to be corrected every couple of seconds.
		// Faster-paced games will require more frequent updates.
		if (mNextT < time)
		{
			mNextT = time + 1f / tUpdates;
			tno.Send("SetT", ForwardType.OthersSaved, m_T.position, m_T.rotation);
		}
	}

	/// <summary>
	/// RFC for the rigidbody will be called once per second by default.
	/// </summary>

	[RFC]
	protected void SetT(Vector3 pos, Quaternion rot)
	{
		m_T.position = pos;
		m_T.rotation = rot;
	}
}

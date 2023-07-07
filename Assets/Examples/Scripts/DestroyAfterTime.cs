using UnityEngine;
using GNet;
using System.Collections;

/// <summary>
/// This script shows how to destroy an object after time.
/// </summary>

public class DestroyAfterTime : TNBehaviour
{
	[SerializeField] private float m_TimeToWait = 1f;
	IEnumerator Start()
	{
		yield return new WaitForSeconds(m_TimeToWait);
		DestroySelf();
	}
}

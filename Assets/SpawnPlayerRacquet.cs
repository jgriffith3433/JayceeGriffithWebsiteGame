using GNet;
using RootMotion.Demos;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPlayerRacquet : MonoBehaviour
{
	/// <summary>
	/// ID of the channel where the prefab should be created. '0' means "last used channel" (not recommended).
	/// </summary>

	public int channelID = 0;

	/// <summary>
	/// Prefab to instantiate.
	/// </summary>

	public string prefabPath;

	/// <summary>
	/// Whether the instantiated object will remain in the game when the player that created it leaves.
	/// Set this to 'false' for the player's avatar.
	/// </summary>

	public bool persistent = false;
	private static int pid;
	private static GameObject p;

	IEnumerator Start()
	{
		if (channelID < 1) channelID = TNManager.lastChannelID;
		while(TNManager.instance == null)
		{
			yield return new WaitForSeconds(1);
		}
		while (TNManager.isJoiningChannel || !TNManager.IsInChannel(channelID) || Application.isLoadingLevel) yield return null;
		TNManager.Instantiate(channelID, "CreatePlayerAtPosition", prefabPath, persistent, transform.position + new Vector3(0, 10, 0), transform.rotation, TNManager.playerID);
		Destroy(gameObject);
	}

	[RCC]
	static GameObject CreatePlayerAtPosition(GameObject prefab, Vector3 pos, Quaternion rot, int playerId)
	{
		// Instantiate the prefab
		p = prefab;
		GameObject go = p.Instantiate();
		var player = TNManager.GetPlayer(playerId);
		go.name = "Player_" + player.name;
		pid = playerId;

		//var player1Go = GameObject.Find("Player_1");
		//GameObject.Find("BotGravPoint").GetComponent<GravPoint>().SetTarget(go.GetComponentInChildren<GravDraggedObject>(true).transform);
		// Set the position and rotation based on the passed values
		Transform t = go.transform;
		t.position = pos;
		t.rotation = rot;
		return go;
	}
}
using GNet;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sandbox : MonoBehaviour
{
	public static Transform SandboxTransform;
    private void Awake()
    {
		SandboxTransform = transform;
	}

    [RCC]
	static GameObject CreateSandboxObject(GameObject prefab, int playerId, float offsetX, float offsetZ)
	{
		var player = TNManager.GetPlayer(playerId);
		var playerCharacter = GameObject.Find("Player_" + player.name);
		TestPlayer testPlayerComponent = null;
		if (playerCharacter != null)
		{
			testPlayerComponent = playerCharacter.GetComponent<TestPlayer>();
		}
		// Instantiate the prefab
		var go = Instantiate(prefab);
		if (testPlayerComponent != null)
		{
			go.transform.position = testPlayerComponent.UserControlAI.transform.position + (testPlayerComponent.UserControlAI.transform.forward * 5) + new Vector3(offsetX, 0, offsetZ);
			go.transform.rotation = testPlayerComponent.UserControlAI.transform.transform.rotation;
		}
		go.name += "_" +player.name;
		return go;
	}
}

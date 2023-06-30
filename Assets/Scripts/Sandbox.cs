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
	static GameObject CreateSandboxObject(GameObject prefab, int playerId)
	{
		// Instantiate the prefab
		var go = Instantiate(prefab);
		var player = TNManager.GetPlayer(playerId);
		var playerCharacter = GameObject.Find("Player_" + player.name);
		if (playerCharacter != null)
        {
			var testPlayerComponent = playerCharacter.GetComponent<TestPlayer>();
			if (testPlayerComponent != null)
            {
				go.transform.position = testPlayerComponent.UserControlAI.transform.position + (testPlayerComponent.UserControlAI.transform.forward * 5);
				go.transform.rotation = testPlayerComponent.UserControlAI.transform.transform.rotation;
			}
		}
		go.name += "_" + player.name;
		return go;
	}
}

using GNet;
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

    [RCC]
	static GameObject CreateRocket(GameObject prefab, int playerId, int playerIdToBomb)
	{
		var playerOnwer = TNManager.GetPlayer(playerId);
		var playerToBomb = TNManager.GetPlayer(playerIdToBomb);
		var playerToBombCharacter = GameObject.Find("Player_" + playerToBomb.name);

		GameObject go = null;
		// Instantiate the prefab
		if (playerToBombCharacter != null)
		{
			var testPlayerComponent = playerToBombCharacter.GetComponent<TestPlayer>();
			if (testPlayerComponent != null)
			{
				go = Instantiate(prefab);
				go.transform.position = testPlayerComponent.UserControlAI.transform.position + new Vector3(0, 200, 0);
				go.transform.rotation = Quaternion.LookRotation((testPlayerComponent.UserControlAI.transform.position - go.transform.position).normalized);
			}
		}
		go.name += "_" + playerOnwer.name;
		return go;
	}
}

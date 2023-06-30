using GNet;
using UnityEngine;

public class Portfolio : MonoBehaviour
{
    [SerializeField] private Sidescroller m_Sidescroller = null;

    private void Awake()
    {
        var player = TNManager.GetPlayer(TNManager.player.id);
        var playerCharacter = GameObject.Find("Player_" + player.name);
		if (playerCharacter != null)
        {
			var testPlayer = playerCharacter.GetComponent<TestPlayer>();
			if (testPlayer != null)
            {
                m_Sidescroller.SetPlayer(testPlayer);
				testPlayer.Teleport(transform.position);

			}
        }
	}
}

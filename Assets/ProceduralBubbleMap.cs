using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralBubbleMap : MonoBehaviour
{
    public List<GameObject> Bubbles = null;
    public TestPlayer Player = null;
    public SpawnPlayer Spawn = null;
    public int SpreadOfBubblesX = 10;
    public int SpreadOfBubblesY = 10;
    public int GenerateDampener = 100;

    private int Count = 0;

    public void Start()
    {
        GenerateMap();
    }

    public void Update()
    {
        Count++;
        if (Count > GenerateDampener)
        {
            Count = 0;
            if (Player == null)
            {
                //Player = SpawnPlayer.TestPlayers.Find(p => p.tno.isMine);
            }
            if (Player != null && Player.tno.isMine)
            {
                if (Player.UserControlAI.transform.position.y < -5)
                {
                    //Spawn.RecreatePlayerAtPosition(new Vector3(
			            Player.Teleport(new Vector3(
                            transform.position.x + Mathf.Lerp(-SpreadOfBubblesX, SpreadOfBubblesX, Random.Range(0, 1f)),
                            transform.position.y + 5,
                            0));
                    GenerateMap();
                }
            }
        }
    }

    private void GenerateMap()
    {
        if (Player == null)
        {
            return;
        }
        foreach(var bubble in Bubbles)
        {
            bubble.transform.position = new Vector3(
                transform.position.x + Mathf.Lerp(-SpreadOfBubblesX, SpreadOfBubblesX, Random.Range(0, 1f)),
                transform.position.y + Mathf.Lerp(-SpreadOfBubblesY, SpreadOfBubblesY, Random.Range(0, 1f)),
                .5f);
            bubble.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }
}

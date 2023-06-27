using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GNet;
using System.Linq;

public class TableTennisClient : TNEventReceiver
{
	[SerializeField] private int channelID = 0;
    [SerializeField] private TableTennis m_TableTennisGame = null;
    public List<Player> Players = null;

    private bool m_Starting = true;

    private IEnumerator Start()
    {
        m_Starting = true;
        while (TNManager.isJoiningChannel || !TNManager.IsInChannel(channelID) || Application.isLoadingLevel)
        {
            yield return new WaitForSeconds(0.1f);
        }
        Players = new List<Player>();
        var playerList = TNManager.GetPlayers(channelID);
        foreach(var player in playerList)
        {
            Players.Add(player);
        }
        Players = Players.OrderBy(p => p.id).ToList();
        m_Starting = false;
    }

    protected override void OnPlayerJoin(int channelID, Player player)
    {
        if (m_Starting)
        {
            return;
        }
        if (!Players.Contains(player))
        {
            Players.Add(player);
            Players = Players.OrderBy(p => p.id).ToList();
        }
    }

    protected override void OnPlayerLeave(int channelID, Player player)
    {
        Debug.Log("Starting: " + m_Starting);
        if (m_Starting)
        {
            return;
        }
        Debug.Log("looking for player: " + player.id);
        if (Players.Contains(player))
        {
            Players.Remove(player);
            m_TableTennisGame.RemovePlayerRacquet(player);
            Players = Players.OrderBy(p => p.id).ToList();
        }
    }
}

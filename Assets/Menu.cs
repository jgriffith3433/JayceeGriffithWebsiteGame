using UnityEngine;
using UnityEngine.UI;
using GNet;
using UnityTools = GNet.UnityTools;

public class Menu : TNEventReceiver
{
    long mNextSend = 0;
    bool mReEnable = false;
    [SerializeField]
    private BestHTTP.Logger.Loglevels m_LogLevel = BestHTTP.Logger.Loglevels.None;

    public static Menu m_Instance = null;
    
    protected override void OnEnable()
    {
        base.OnEnable();
        if (Application.isPlaying)
        {
            if (m_Instance == null)
            {
                m_Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if (Application.isPlaying)
        {
            if (m_Instance == this)
            {
                m_Instance = null;
            }
        }
    }

    private void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        TNManager.ConnectToHub(GNetConfig.HubUrl, MHLab.ReactUI.Core.BrowserBridge.GamerTag);
        TNManager.client.m_ClientPlayer.ReceiveResponseServerListPacket += OnReceivedServerList;
        BestHTTP.HTTPManager.Logger.Level = m_LogLevel;
    }

    private void OnDestroy()
    {
        if (TNManager.client != null && TNManager.client.m_ClientPlayer != null)
        {
            TNManager.client.m_ClientPlayer.ReceiveResponseServerListPacket -= OnReceivedServerList;
        }
        TNManager.StartDisconnectingFromHub();
    }

    protected override void OnConnectedToHub(ClientPlayer clientPlayer)
    {
        Debug.Log("Connected to hub: " + clientPlayer.ConnectionId);
        TNManager.SendPacket(new RequestServerListPacket(TNSrcLobbyClient.GameId));
    }

    void OnReceivedServerList(CommandPacket commandPacket)
    {
        var responseServerListPacket = commandPacket as ResponseServerListPacket;
        long time = System.DateTime.UtcNow.Ticks / 10000;
        LobbyClient.knownServers.ReadFrom(responseServerListPacket, time);
        if (LobbyClient.knownServers.list.size > 0)
        {
            TNManager.ConnectToGameServer(LobbyClient.knownServers.list.buffer[0].serverId);
        }
        else
        {
            TNManager.RequestNewGameServer();
        }
    }

    protected override void OnDisconnectedFromHub(ClientPlayer clientPlayer)
    {
        Debug.Log("Disconnected from hub: " + clientPlayer.ConnectionId);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
    }

    protected override void OnReceiveResponseNewGameServerPacket(ResponseNewGameServerPacket responseNewGameServerPacket)
    {
        Debug.Log("Created new game server: " + responseNewGameServerPacket.ServerId);
        TNManager.SendPacket(new RequestServerListPacket(TNSrcLobbyClient.GameId));
        //TNManager.ConnectToGameServer(responseNewGameServerPacket.ServerId);
    }

    protected override void OnConnectedToGameServer(ClientPlayer clientPlayer)
    {
        Debug.Log("Connected to game server: " + clientPlayer.GameServerId);
        TNManager.JoinChannel(7, "NavMeshAgent", true, 255, null);
    }

    protected override void OnDisconnectedFromGameServer(ClientPlayer clientPlayer)
    {
        Debug.Log("Disconnected from game server: " + clientPlayer.GameServerId);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
        TNManager.ConnectToGameServer(clientPlayer.GameServerId);
    }

    protected override void OnJoinChannel(int channelID, bool success, string msg)
    {
        Debug.Log("Joined channel #" + channelID + " " + success + " " + msg);
    }

    protected override void OnLeaveChannel(int channelID)
    {
        Debug.Log("Left channel #" + channelID);

        if (TNManager.channels.size == 0)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
        }
    }
}

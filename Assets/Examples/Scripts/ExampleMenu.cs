//-------------------------------------------------
//                    GNet 3
// Copyright © 2012-2018 Tasharen Entertainment Inc
//-------------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using GNet;
using UnityTools = GNet.UnityTools;
using MHLab.ReactUI.Core;
using System.Collections;

/// <summary>
/// This script provides a main menu for all examples.
/// The menu is created in Unity's built-in Immediate Mode GUI system.
/// The menu makes use of the following GNet functions:
/// - TNManager.Connect
/// - TNManager.JoinChannel
/// - TNManager.LeaveChannel
/// - TNManager.Disconnect
/// - TNServerInstance.Start
/// - TNServerInstance.Stop
/// </summary>


public class ExampleMenu : TNEventReceiver
{
    public BestHTTP.Logger.Loglevels m_LogLevel = BestHTTP.Logger.Loglevels.None;
    public static ExampleMenu m_Instance = null;

    const float buttonWidth = 400f;
    const float buttonHeight = 40f;

    public enum IPType
    {
        IP_v_4,
        IP_v_6,
    }
    public int serverTcpPort = 5127;
    public IPType addressFamily = IPType.IP_v_4;
    public string[] examples;
    public ServerList ServerList = null;
    public GUIStyle button;
    public GUIStyle text;
    public GUIStyle textLeft;
    public GUIStyle input;
    string m_SelectedGameServerId = "";
    string m_SelectedServerOrChannelName = "";
    string connectionId = "";
    float mGameServerListAlpha = 0f;
    float mChannelListAlpha = 0f;
    long mLobbyNextSend = 0;
    public int LobbyRequestsPerMinute = 3;
    public static ushort GameId = 1;
    public Button m_StartStopServerButton = null;
    public Text m_StartStopServerButtonText = null;
    public Button m_JoinLeaveServerButton = null;
    public Text m_JoinLeaveServerButtonText = null;
    public Button m_SoundOnButton;
    public Button m_SoundOffButton;
    public AudioSource m_AudioSource;
    [SerializeField]
    private float m_AudioVolume = 0.25f;
    public Button[] m_ServerOrChannelButtonList = null;
    public Text[] m_ServerOrChannelButtonTextList = null;
    public Button m_JoinLeaveChannelButton = null;
    public Text m_JoinLeaveChannelButtonText = null;
    public GameObject m_MenuCanvas = null;
    public GameObject m_GameCanvas = null;
    public EventSystem m_EventSystem = null;
    public GameObject m_TutorialText = null;
    public bool m_Skip = true;
    public string m_SkipLevel = "Table Tennis";

    public string GetSelectedServerOrChannelName()
    {
        return m_SelectedServerOrChannelName;
    }

    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        AudioListener.volume = m_AudioVolume;
        m_AudioSource.Play();
        BestHTTP.HTTPManager.Logger.Level = m_LogLevel;
        Debug.Log("Connecting to hub: " + GNetConfig.HubUrl);
        TNManager.ConnectToHub(GNetConfig.HubUrl, BrowserBridge.GamerTag);
        DontDestroyOnLoad(m_AudioSource.gameObject);
        DontDestroyOnLoad(m_MenuCanvas);
        DontDestroyOnLoad(m_EventSystem);
        DontDestroyOnLoad(m_GameCanvas);
        StartCoroutine(HideText());
        if (m_Skip)
        {
            StartCoroutine(Skip());
        }
    }

    private IEnumerator Skip()
    {
        yield return new WaitForSeconds(0.1f);
        while (!TNManager.isConnectedToHub)
        {
            yield return new WaitForSeconds(0.1f);

        }

        TNManager.client.player.SendPacket(new RequestServerListPacket(GameId));
        while (ServerList == null)
        {
            yield return new WaitForSeconds(0.1f);
        }

        if (ServerList.list.Count == 0)
        {
            TNManager.RequestNewGameServer();
        }

        while (ServerList.list.Count == 0)
        {
            yield return new WaitForSeconds(0.1f);
        }

        var ent = ServerList.list.buffer[0];
        var serverId = ent.serverId.ToString();

        TNManager.ConnectToGameServer(serverId);

        while (!TNManager.isConnectedToGameServer)
        {
            yield return new WaitForSeconds(0.1f);
        }
        if (!string.IsNullOrEmpty(m_SkipLevel))
        {
            TNManager.JoinChannel(2, m_SkipLevel, true);
        }
    }

    private IEnumerator HideText()
    {
        yield return new WaitForSeconds(10);
        m_TutorialText.SetActive(false);
    }

    public void StartStopGameServer()
    {
        // stop any game server we are connected to
        if (TNManager.isConnectedToGameServer)
        {
            TNManager.RequestStopGameServer(TNManager.client.player.GameServerId);
        }
        else if (!string.IsNullOrEmpty(m_SelectedGameServerId))
        {
            TNManager.RequestStopGameServer(m_SelectedGameServerId);
            TNManager.client.player.SendPacket(new RequestServerListPacket(GameId));
        }
        else
        {
            TNManager.RequestNewGameServer();
        }
        //TODO: Implement ResponseStopGameServerPacket in GNet. For now just empty the string
        m_SelectedGameServerId = "";
        m_SelectedServerOrChannelName = "";
    }

    public void JoinLeaveServer()
    {
        if (TNManager.isConnectedToGameServer)
        {
            TNManager.StartDisconnectingFromGameServer();
        }
        else if (!string.IsNullOrEmpty(m_SelectedServerOrChannelName))
        {
            TNManager.ConnectToGameServer(m_SelectedServerOrChannelName);
        }
        m_SelectedServerOrChannelName = "";
        m_SelectedGameServerId = "";
    }

    public void SelectedChannelOrServer()
    {
        m_SelectedServerOrChannelName = EventSystem.current.currentSelectedGameObject.name;
        if (EventSystem.current.currentSelectedGameObject.GetComponent<ChannelOrServer>().IsServer)
        {
            m_SelectedGameServerId = m_SelectedServerOrChannelName;
        }
        else
        {
            m_SelectedGameServerId = "";
        }
    }

    public void JoinLeaveChannel()
    {
        if (TNManager.isInChannel)
        {
            TNManager.LeaveChannel();
        }
        else
        {
            var channelId = System.Array.IndexOf(examples, m_SelectedServerOrChannelName) + 1;
            if (channelId != -1)
            {
                TNManager.JoinChannel(channelId, m_SelectedServerOrChannelName, true);
            }
        }
        m_SelectedServerOrChannelName = "";
    }

    public void ShowHideMenu()
    {
        m_MenuCanvas.SetActive(!m_MenuCanvas.activeSelf);
    }

    void Update()
    {
        if (Application.isPlaying)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ShowHideMenu();
            }
            m_StartStopServerButton.interactable = TNManager.isConnectedToHub;
            m_StartStopServerButtonText.text = TNManager.isConnectedToHub ? !string.IsNullOrEmpty(m_SelectedGameServerId) || TNManager.isConnectedToGameServer ? "Stop Server" : "Create Server" : "Connecting To Hub";
            // if (!TNManager.isConnectedToHub && TNManager.client != null && TNManager.client.hubStage != GNet.NetworkPlayer.Stage.Connecting)
            // {
            //     //TODO: Show how to connect to hub manually?
            //     TNManager.ConnectToHub(hubUrlToConnectTo, BrowserBridge.GamerTag);
            //     return;
            // }
            // if (!TNSrcLobbyClient.Instance.IsConnectedToHub && !TNSrcLobbyClient.Instance.IsConnectingToHub)
            // {
            //     TNSrcLobbyClient.Instance.HubUrlToConnectTo = hubUrlToConnectTo;
            //     TNSrcLobbyClient.Instance.ConnectToHub();
            //     return;
            // }
            //lobby
            long time = System.DateTime.UtcNow.Ticks / 10000;
            if (mLobbyNextSend < time && TNManager.isConnectedToHub)
            {
                mLobbyNextSend = time + (60 / LobbyRequestsPerMinute * 1000);
                TNManager.client.player.SendPacket(new RequestServerListPacket(GameId));
            }
            m_JoinLeaveServerButton.interactable = TNManager.isConnectedToHub && (!string.IsNullOrEmpty(m_SelectedGameServerId) || TNManager.isConnectedToGameServer);
            m_JoinLeaveServerButtonText.text = TNManager.isConnectedToGameServer ? "Leave Server" : "Join Server";
            m_JoinLeaveChannelButton.interactable = (TNManager.isConnectedToGameServer && !string.IsNullOrEmpty(m_SelectedServerOrChannelName)) || (TNManager.isConnectedToGameServer && TNManager.isInChannel);
            m_JoinLeaveChannelButtonText.text = TNManager.isConnectedToGameServer && TNManager.isInChannel ? "Leave Game" : "Join Game";

            if (TNManager.isConnectedToGameServer)
            {
                UpdateChannelList();
                //hide servers
                mGameServerListAlpha = UnityTools.SpringLerp(mGameServerListAlpha, 0, 8f, Time.deltaTime);
                //show channels
                mChannelListAlpha = UnityTools.SpringLerp(mChannelListAlpha, 1, 8f, Time.deltaTime);
            }
            else
            {
                UpdateServerList();
                //show servers
                mGameServerListAlpha = UnityTools.SpringLerp(mGameServerListAlpha, 1, 8f, Time.deltaTime);
                //hide channels
                mChannelListAlpha = UnityTools.SpringLerp(mChannelListAlpha, 0, 8f, Time.deltaTime);
            }
        }
    }

    void OnGUI()
    {
        DrawDebugInfo();
    }

    void UpdateChannelList()
    {
        for (int i = 0; i < m_ServerOrChannelButtonList.Length; ++i)
        {
            m_ServerOrChannelButtonList[i].GetComponent<ChannelOrServer>().IsServer = false;
        }
        for (int i = 0; i < m_ServerOrChannelButtonList.Length; ++i)
        {
            if (i < examples.Length)
            {
                m_ServerOrChannelButtonList[i].gameObject.SetActive(true);
                var channelName = examples[i];
                m_ServerOrChannelButtonList[i].gameObject.name = channelName;
                m_ServerOrChannelButtonTextList[i].text = channelName;
                //m_ServerOrChannelButtonList[i].text.color = gameServerId == serverId ? black : red;

            }
            else
            {
                m_ServerOrChannelButtonList[i].gameObject.SetActive(false);
                m_ServerOrChannelButtonList[i].gameObject.name = "";
                m_ServerOrChannelButtonTextList[i].text = "";
            }
        }
    }


    void OnReceivedServerList(CommandPacket commandPacket)
    {
        var responseServerListPacket = commandPacket as ResponseServerListPacket;
        long time = System.DateTime.UtcNow.Ticks / 10000;
        ServerList = new ServerList();
        ServerList.ReadFrom(responseServerListPacket, time);
    }

    void UpdateServerList()
    {
        for (int i = 0; i < m_ServerOrChannelButtonList.Length; ++i)
        {
            m_ServerOrChannelButtonList[i].GetComponent<ChannelOrServer>().IsServer = true;
            if (ServerList != null && i < ServerList.list.size)
            {
                m_ServerOrChannelButtonList[i].gameObject.SetActive(true);
                var ent = ServerList.list.buffer[i];
                var serverId = ent.serverId.ToString();
                m_ServerOrChannelButtonList[i].gameObject.name = serverId;
                m_ServerOrChannelButtonTextList[i].text = serverId;
                //m_ServerOrChannelButtonList[i].text.color = gameServerId == serverId ? black : red;
            }
            else
            {
                m_ServerOrChannelButtonList[i].gameObject.SetActive(false);
                m_ServerOrChannelButtonList[i].gameObject.name = "";
                m_ServerOrChannelButtonTextList[i].text = "";
            }
        }
    }

    public void Mute()
    {
        m_SoundOnButton.interactable = true;
        m_SoundOffButton.interactable = false;
        AudioListener.volume = 0;
    }

    public void Unmute()
    {
        m_SoundOnButton.interactable = false;
        m_SoundOffButton.interactable = true;
        AudioListener.volume = m_AudioSource.volume;
    }

    public void QuitGame()
    {
#if !UNITY_EDITOR
			Application.Quit();
#endif

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

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

    protected override void OnConnectedToHub(ClientPlayer clientPlayer)
    {
        Debug.Log("Connected to hub: " + clientPlayer.ConnectionId);
        connectionId = clientPlayer.ConnectionId;
        TNManager.client.player.ReceiveResponseServerListPacket += OnReceivedServerList;
#if UNITY_EDITOR
#endif
    }

    protected override void OnDisconnectedFromHub(ClientPlayer clientPlayer)
    {
        Debug.Log("Disconnected from hub: " + clientPlayer.ConnectionId);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Empty");
        connectionId = "";
        m_MenuCanvas.SetActive(true);
        if (TNManager.client != null && TNManager.client.player != null)
        {
            TNManager.client.player.ReceiveResponseServerListPacket -= OnReceivedServerList;
        }
    }

    protected override void OnReceiveResponseNewGameServerPacket(ResponseNewGameServerPacket responseNewGameServerPacket)
    {
        TNManager.client.player.SendPacket(new RequestServerListPacket(GameId));
#if UNITY_EDITOR
#endif
    }

    protected override void OnConnectedToGameServer(ClientPlayer clientPlayer)
    {
        Debug.Log("Connected to game server: " + clientPlayer.GameServerId);
#if UNITY_EDITOR
#endif
    }

    protected override void OnDisconnectedFromGameServer(ClientPlayer clientPlayer)
    {
        Debug.Log("Disconnected from game server");
        UnityEngine.SceneManagement.SceneManager.LoadScene("Empty");
        m_MenuCanvas.SetActive(true);
        TNManager.client.player.SendPacket(new RequestServerListPacket(GameId));
    }

    protected override void OnJoinChannel(int channelID, bool success, string msg)
    {
        Debug.Log("Joined channel #" + channelID + " " + success + " " + msg);
        m_MenuCanvas.SetActive(false);
    }

    protected override void OnLeaveChannel(int channelID)
    {
        Debug.Log("Left channel #" + channelID);
        if (TNManager.channels.size == 0)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Empty");
            m_MenuCanvas.SetActive(true);
        }
    }

    void DrawDebugInfo()
    {
        //GUILayout.Label("LAN: " + Tools.localAddress.ToString(), textLeft);

        if (Application.isPlaying)
        {
            //if (Tools.isExternalIPReliable)
            //	GUILayout.Label("WAN: " + Tools.externalAddress, textLeft);
            //else GUILayout.Label("WAN: Resolving...", textLeft);

            //if (TNManager.isConnected)
            //	GUILayout.Label("Ping: " + TNManager.ping + " (" + (TNManager.canUseUDP ? "TCP+UDP" : "TCP") + ")", textLeft);
        }
    }
}

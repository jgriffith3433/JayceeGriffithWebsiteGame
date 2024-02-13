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
    [SerializeField] string m_SelectedServerOrChannelName = "";
    string connectionId = "";
    float mGameServerListAlpha = 0f;
    float mChannelListAlpha = 0f;
    long mLobbyNextSend = 0;
    long mLastClickTime = 0;
    public int LobbyRequestsPerMinute = 3;
    public int DoubleClickTime = 500;
    public static ushort GameId = 1;
    public AudioSource m_AudioSource;
    [SerializeField]
    private float m_AudioVolume = 0.25f;
    public ScreenSizeSwitcher m_CanvasSwitcher = null;
    public EventSystem m_EventSystem = null;
    public bool m_Skip = true;
    public string m_SkipLevel = "Table Tennis";
    public MenuObjects[] MenuObjects;
    public GameObject[] NewGameButtons;
    public GameObject[] BackButtons;
    public bool MenuVisible = false;

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
        DontDestroyOnLoad(m_AudioSource.gameObject);
        DontDestroyOnLoad(m_CanvasSwitcher);
        DontDestroyOnLoad(m_EventSystem);
        if (m_Skip)
        {
            StartCoroutine(Skip());
        }
        Debug.Log("Connecting to hub: " + GNetConfig.HubUrl);
        TNManager.ConnectToHub(GNetConfig.HubUrl, BrowserBridge.GamerTag);
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

        //while (!TNManager.isConnectedToGameServer)
        //{
        //    yield return new WaitForSeconds(0.1f);
        //}
        //if (!string.IsNullOrEmpty(m_SkipLevel))
        //{
        //    TNManager.JoinChannel(8, m_SkipLevel, false, 255, "", false);
        //}
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

        DeselectAllButtons();
    }

    private void DeselectAllButtons()
    {
        foreach (var menuObject in MenuObjects)
        {
            if (menuObject.gameObject.activeSelf)
            {
                for (int i = 0; i < menuObject.m_ServerOrChannelButtonList.Length; ++i)
                {
                    var buttonComponent = menuObject.m_ServerOrChannelButtonList[i].GetComponent<Button>();
                    var imageComponent = menuObject.m_ServerOrChannelButtonList[i].GetComponent<Image>();
                    //imageComponent.color = buttonComponent.colors.normalColor;
                }
            }
        }
    }

    public void SelectedChannelOrServer(GameObject button)
    {
        var buttonComponent = button.GetComponent<Button>();
        var imageComponent = button.GetComponent<Image>();
        var isServer = button.GetComponent<ChannelOrServer>().IsServer;
        DeselectAllButtons();
        m_SelectedServerOrChannelName = button.name;
        m_SelectedGameServerId = isServer ? m_SelectedServerOrChannelName : "";
        if (isServer)
        {
            JoinLeaveServer();
        }
        else
        {
            JoinLeaveChannel();
        }
        foreach (var backButton in BackButtons)
        {
            backButton.SetActive(true);
        }
        foreach (var menuObject in MenuObjects)
        {
            menuObject.m_SoundOffButton.gameObject.SetActive(false);
            menuObject.m_SoundOnButton.gameObject.SetActive(false);
        }
        //long time = System.DateTime.UtcNow.Ticks / 10000;
        //if (time - mLastClickTime <= DoubleClickTime)
        //{
        //    //double click
        //    mLastClickTime = time;
        //    if (!string.IsNullOrEmpty(m_SelectedServerOrChannelName))
        //    {
        //        DeselectAllButtons();
        //        if (isServer)
        //        {
        //            JoinLeaveServer();
        //        }
        //        else
        //        {
        //            JoinLeaveChannel();
        //        }
        //    }
        //}
        //else
        //{
        //    //single click
        //    if (m_SelectedServerOrChannelName == button.name)
        //    {
        //        //click off
        //        DeselectAllButtons();
        //        imageComponent.color = buttonComponent.colors.normalColor;
        //        m_SelectedServerOrChannelName = "";
        //        m_SelectedGameServerId = "";
        //    }
        //    else
        //    {
        //        //click on
        //        mLastClickTime = time;
        //        DeselectAllButtons();
        //        imageComponent.color = buttonComponent.colors.selectedColor;
        //        m_SelectedServerOrChannelName = button.name;
        //        m_SelectedGameServerId = isServer ? m_SelectedServerOrChannelName : "";
        //    }
        //}
    }

    public void JoinLeaveChannel()
    {
        var inChannelOtherThanChat = TNManager.channels.size > 1 || (TNManager.channels.size == 1 && !TNManager.IsInChannel(1));
        if (inChannelOtherThanChat)
        {
            for (var i = 0; i < TNManager.channels.size; i++)
            {
                var channel = TNManager.channels.buffer[i];
                if (channel.id != 1)
                {
                    TNManager.LeaveChannel(channel.id);
                    //ShowHideMenu();

                    foreach (var menuObject in MenuObjects)
                    {
                        menuObject.m_SoundOffButton.gameObject.SetActive(AudioListener.volume != 0);
                        menuObject.m_SoundOnButton.gameObject.SetActive(AudioListener.volume == 0);
                    }
                    foreach (var newGameButton in NewGameButtons)
                    {
                        newGameButton.SetActive(true);
                    }
                    foreach (var backButton in BackButtons)
                    {
                        backButton.SetActive(false);
                    }
                }
            }
            if (!string.IsNullOrEmpty(m_SelectedServerOrChannelName))
            {
                var channelId = System.Array.IndexOf(examples, m_SelectedServerOrChannelName) + 2;
                if (channelId != 1)
                {
                    TNManager.JoinChannel(channelId, m_SelectedServerOrChannelName, false);
                    foreach (var backButton in BackButtons)
                    {
                        backButton.SetActive(false);
                    }
                }
            }
        }
        else
        {
            var channelId = System.Array.IndexOf(examples, m_SelectedServerOrChannelName) + 2;
            if (channelId != 1)
            {
                TNManager.JoinChannel(channelId, m_SelectedServerOrChannelName, false);
                foreach (var backButton in BackButtons)
                {
                    backButton.SetActive(false);
                }
            }
        }
        m_SelectedServerOrChannelName = "";
        DeselectAllButtons();
    }

    public void NewGamePressed()
    {
        foreach (var newGameButton in NewGameButtons)
        {
            newGameButton.SetActive(false);
        }
        MenuVisible = false;
        ShowHideMenu();
    }

    public void ShowHideMenu()
    {
        MenuVisible = !MenuVisible;
        foreach (var menuObject in MenuObjects)
        {
            menuObject.m_LeftMenu.SetActive(MenuVisible);
            menuObject.m_ServerOrChannelListMenu.SetActive(MenuVisible);
        }

        long time = System.DateTime.UtcNow.Ticks / 10000;
        mLobbyNextSend = time + (60 / LobbyRequestsPerMinute * 1000);
        TNManager.client.player.SendPacket(new RequestServerListPacket(GameId));
    }

    void Update()
    {
        if (Application.isPlaying && TNManager.isConnectedToHub)
        {
            var inChannelOtherThanChat = TNManager.channels.size > 1 || (TNManager.channels.size == 1 && !TNManager.IsInChannel(1));
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ShowHideMenu();
            }
            foreach (var menuObject in MenuObjects)
            {
                //menuObject.m_Information.SetActive(!TNManager.isConnectedToGameServer);
                menuObject.m_StartStopServerButton.interactable = TNManager.isConnectedToHub;
                menuObject.m_StartStopServerButtonText.text = TNManager.isConnectedToHub ? !string.IsNullOrEmpty(m_SelectedGameServerId) || TNManager.isConnectedToGameServer ? "Stop Server" : "Create Server" : "Connecting To Hub";
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
                menuObject.m_JoinLeaveServerButton.interactable = TNManager.isConnectedToHub && (!string.IsNullOrEmpty(m_SelectedGameServerId) || TNManager.isConnectedToGameServer);
                menuObject.m_JoinLeaveServerButtonText.text = TNManager.isConnectedToGameServer ? "Leave Server" : "Join Server";
                menuObject.m_JoinLeaveChannelButton.interactable = (TNManager.isConnectedToGameServer && !string.IsNullOrEmpty(m_SelectedServerOrChannelName)) || (TNManager.isConnectedToGameServer && inChannelOtherThanChat);
                menuObject.m_JoinLeaveChannelButtonText.text = TNManager.isConnectedToGameServer && inChannelOtherThanChat && string.IsNullOrEmpty(m_SelectedServerOrChannelName) ? "Leave Game" : "Join Game";

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
    }

    void OnGUI()
    {
        DrawDebugInfo();
    }

    void UpdateChannelList()
    {
        var inChannelOtherThanChat = TNManager.channels.size > 1 || (TNManager.channels.size == 1 && !TNManager.IsInChannel(1));
        foreach (var menuObject in MenuObjects)
        {
            for (int i = 0; i < menuObject.m_ServerOrChannelButtonList.Length; ++i)
            {
                menuObject.m_ServerOrChannelButtonList[i].GetComponent<ChannelOrServer>().IsServer = false;
            }
            for (int i = 0; i < menuObject.m_ServerOrChannelButtonList.Length; ++i)
            {
                if (i < examples.Length)
                {
                    //menuObject.m_ServerOrChannelButtonList[i].gameObject.SetActive(!inChannelOtherThanChat);
                    menuObject.m_ServerOrChannelButtonList[i].gameObject.SetActive(true);
                    var channelName = examples[i];
                    menuObject.m_ServerOrChannelButtonList[i].gameObject.name = channelName;
                    menuObject.m_ServerOrChannelButtonTextList[i].text = channelName;
                    //m_ServerOrChannelButtonList[i].text.color = gameServerId == serverId ? black : red;

                }
                else
                {
                    menuObject.m_ServerOrChannelButtonList[i].gameObject.SetActive(false);
                    menuObject.m_ServerOrChannelButtonList[i].gameObject.name = "";
                    menuObject.m_ServerOrChannelButtonTextList[i].text = "";
                }
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
        foreach (var menuObject in MenuObjects)
        {
            for (int i = 0; i < menuObject.m_ServerOrChannelButtonList.Length; ++i)
            {
                menuObject.m_ServerOrChannelButtonList[i].GetComponent<ChannelOrServer>().IsServer = true;
                if (ServerList != null && i < ServerList.list.size)
                {
                    menuObject.m_ServerOrChannelButtonList[i].gameObject.SetActive(true);
                    var ent = ServerList.list.buffer[i];
                    var serverId = ent.serverId.ToString();
                    menuObject.m_ServerOrChannelButtonList[i].gameObject.name = serverId;
                    menuObject.m_ServerOrChannelButtonTextList[i].text = serverId;
                    //MenuObjects.m_ServerOrChannelButtonList[i].text.color = gameServerId == serverId ? black : red;
                }
                else
                {
                    menuObject.m_ServerOrChannelButtonList[i].gameObject.SetActive(false);
                    menuObject.m_ServerOrChannelButtonList[i].gameObject.name = "";
                    menuObject.m_ServerOrChannelButtonTextList[i].text = "";
                }
            }
        }
    }

    public void Mute()
    {
        AudioListener.volume = 0;
        foreach (var menuObject in MenuObjects)
        {
            menuObject.m_SoundOnButton.interactable = true;
            menuObject.m_SoundOffButton.interactable = false;
            menuObject.m_SoundOffButton.gameObject.SetActive(false);
            menuObject.m_SoundOnButton.gameObject.SetActive(true);
        }
    }

    public void Unmute()
    {
        AudioListener.volume = m_AudioSource.volume;
        foreach (var menuObject in MenuObjects)
        {
            menuObject.m_SoundOnButton.interactable = false;
            menuObject.m_SoundOffButton.interactable = true;
            menuObject.m_SoundOnButton.gameObject.SetActive(false);
            menuObject.m_SoundOffButton.gameObject.SetActive(true);
        }
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
        StartCoroutine(LoadEmpty());
        connectionId = clientPlayer.ConnectionId;
        TNManager.client.player.ReceiveResponseServerListPacket += OnReceivedServerList;
        long time = System.DateTime.UtcNow.Ticks / 10000;
        mLobbyNextSend = time + (60 / LobbyRequestsPerMinute * 1000);
        TNManager.client.player.SendPacket(new RequestServerListPacket(GameId));

#if UNITY_EDITOR
#endif
    }

    private IEnumerator LoadEmpty()
    {
        yield return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Empty");
        m_CanvasSwitcher.gameObject.SetActive(true);
    }

    protected override void OnDisconnectedFromHub(ClientPlayer clientPlayer)
    {
        Debug.Log("Disconnected from hub: " + clientPlayer.ConnectionId);
        StartCoroutine(LoadEmpty());
        connectionId = "";
        MenuVisible = true;
        foreach (var menuObject in MenuObjects)
        {
            menuObject.m_LeftMenu.SetActive(true);
            menuObject.m_ServerOrChannelListMenu.SetActive(true);
        }
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
        TNManager.JoinChannel(1, "Chat", false, 255, "", false);
#if UNITY_EDITOR
#endif
    }

    protected override void OnDisconnectedFromGameServer(ClientPlayer clientPlayer)
    {
        Debug.Log("Disconnected from game server");
        StartCoroutine(LoadEmpty());
        MenuVisible = true;
        foreach (var menuObject in MenuObjects)
        {
            menuObject.m_LeftMenu.SetActive(true);
            menuObject.m_ServerOrChannelListMenu.SetActive(true);
        }
        TNManager.client.player.SendPacket(new RequestServerListPacket(GameId));
    }

    protected override void OnJoinChannel(int channelID, bool success, string msg)
    {
        Debug.Log("Joined channel #" + channelID + " " + success + " " + msg);
        if (channelID != 1)
        {
            MenuVisible = false;
            foreach (var menuObject in MenuObjects)
            {
                menuObject.m_LeftMenu.SetActive(false);
                menuObject.m_ServerOrChannelListMenu.SetActive(false);
            }
        }
    }

    protected override void OnLeaveChannel(int channelID)
    {
        Debug.Log("Left channel #" + channelID);
        if (TNManager.channels.size == 0)
        {
            if (channelID != 1)
            {
                MenuVisible = true;
                foreach (var menuObject in MenuObjects)
                {
                    menuObject.m_LeftMenu.SetActive(true);
                    //menuObject.m_ServerOrChannelListMenu.SetActive(true);
                }
            }
        }
        else
        {
            if (channelID != 1)
            {
                MenuVisible = true;
                foreach (var menuObject in MenuObjects)
                {
                    menuObject.m_LeftMenu.SetActive(true);
                    //menuObject.m_ServerOrChannelListMenu.SetActive(true);
                }
            }

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

//-------------------------------------------------
//                    GNet 3
// Copyright © 2012-2018 Tasharen Entertainment Inc
//-------------------------------------------------

using UnityEngine;
using GNet;
using UnityEngine.UI;

/// <summary>
/// This example script shows how to create a chat window powered by the Tasharen Network framework.
/// You can see it used in Example Chat.
/// </summary>

public class ExampleChat : TNBehaviour
{
	Rect mRect;
	string mName = "Guest";
	string mInput = "";
	int mChannelID;
	[SerializeField] private Transform m_ChatMessageParent = null;
	[SerializeField] private GameObject m_ChatMessagePrefab = null;
	[SerializeField] private GameObject m_ChatCanvas = null;

	struct ChatEntry
	{
		public string text;
		public Color color;
	}

	/// <summary>
	/// Add a new chat entry.
	/// </summary>

	void AddToChat (string text, Color color)
	{
		ChatEntry ent = new ChatEntry();
		ent.text = text;
		ent.color = color;

		var chatMessageInstance = Instantiate(m_ChatMessagePrefab, m_ChatMessageParent);
		var textComponent = chatMessageInstance.GetComponentInChildren<Text>();
		textComponent.text = ent.text;
		textComponent.color = ent.color;
	}

	public void ShowHideChat()
	{
		m_ChatCanvas.SetActive(!m_ChatCanvas.activeSelf);
	}

	public void PressChat()
	{
		ShowHideChat();
	}

	/// <summary>
	/// Register event delegates.
	/// </summary>

	void OnEnable ()
	{
		TNManager.onJoinChannel += OnJoinChannel;
		TNManager.onPlayerJoin += OnPlayerJoin;
		TNManager.onPlayerLeave += OnPlayerLeave;
		TNManager.onRenamePlayer += OnRenamePlayer;
		TNManager.onSetServerData += OnSetServerData;
		TNManager.onSetChannelData += OnSetChannelData;
	}

	/// <summary>
	/// Unregister event delegates.
	/// </summary>

	void OnDisable ()
	{
		TNManager.onJoinChannel -= OnJoinChannel;
		TNManager.onPlayerJoin -= OnPlayerJoin;
		TNManager.onPlayerLeave -= OnPlayerLeave;
		TNManager.onRenamePlayer -= OnRenamePlayer;
		TNManager.onSetServerData -= OnSetServerData;
		TNManager.onSetChannelData -= OnSetChannelData;
	}

	void OnSetServerData (string path, DataNode node) { PrintServerData(path); }
	void OnSetChannelData (Channel ch, string path, DataNode node) { PrintChannelData(path); }

	/// <summary>
	/// The list of players in the channel is immediately available upon joining a room.
	/// </summary>

	void OnJoinChannel (int channelID, bool success, string error)
	{
		mChannelID = channelID;
		mName = TNManager.playerName;

		// Show the current configuration
		PrintServerData();
		PrintChannelData();

		var text = "Other players here: ";
		var players = TNManager.GetPlayers(channelID);

		for (int i = 0; i < players.size; ++i)
		{
			if (i > 0) text += ", ";
			text += players.buffer[i].name;
			if (players.buffer[i].id == TNManager.playerID) text += " (you)";
		}
		AddToChat(text, Color.black);
	}

	/// <summary>
	/// Notification of a new player joining the channel.
	/// </summary>

	void OnPlayerJoin (int channelID, Player p)
	{
		AddToChat(p.name + " has joined channel " + channelID, Color.black);
	}

	/// <summary>
	/// Notification of another player leaving the channel.
	/// </summary>

	void OnPlayerLeave (int channelID, Player p)
	{
		AddToChat(p.name + " has left channel " + channelID, Color.black);
	}

	/// <summary>
	/// Notification of a player changing their name.
	/// </summary>

	void OnRenamePlayer (Player p, string previous)
	{
		AddToChat(previous + " is now known as " + p.name, Color.black);
	}

	/// <summary>
	/// This is our chat callback. As messages arrive, they simply get added to the list.
	/// </summary>

	[RFC] void OnChat (int playerID, string text)
	{
		// Figure out who sent the message and add their name to the text
		Player player = TNManager.GetPlayer(playerID);
		Color color = (player.id == TNManager.playerID) ? Color.green : Color.white;
		AddToChat("[" + player.name + "]: " + text, color);
	}

	void SetChannelData (int channelID, string text)
	{
		if (!string.IsNullOrEmpty(text))
		{
			var parts = text.Split(new char[] { '=' }, 2);

			if (parts.Length == 2)
			{
				var key = parts[0].Trim();
				var val = parts[1].Trim();
				var node = new DataNode(key, val);
				if (node.ResolveValue()) TNManager.SetChannelData(channelID, node.name, node.value);
			}
			else Debug.LogWarning("Invalid syntax [" + text + "]. Expected [key = value].");
		}
	}

	/// <summary>
	/// Send the typed message to the server and clear the text.
	/// </summary>

	void Send ()
	{
		if (!string.IsNullOrEmpty(mInput))
		{
			mInput = mInput.Trim();

			if (mInput == "/getServer") PrintServerData();
			else if (mInput.StartsWith("/getServer ")) PrintServerData(mInput.Substring(5));
			else if (mInput.StartsWith("/setServer "))
			{
				if (TNManager.isAdmin) TNManager.SetServerData(mInput.Substring(5));
				else AddToChat("Only server administrators can set server data", Color.red);
			}
			else if (mInput == "/get") PrintChannelData();
			else if (mInput.StartsWith("/get ")) PrintChannelData(mInput.Substring(5));
			else if (mInput.StartsWith("/set ")) SetChannelData(mChannelID, mInput.Substring(5));
			else if (mInput.StartsWith("/exe "))
			{
				// Longer version, won't cause compile errors if RuntimeCode is not imported
				var type = System.Type.GetType("GNet.RuntimeCode");
				if (type != null) type.Invoke("Execute", mInput.Substring(5));
				else Debug.LogError("You need to import the RuntimeCode package first");

				// Shorter version:
				//RuntimeCode.Execute(mInput.Substring(5));
			}
			else tno.Send("OnChat", ForwardType.All, TNManager.playerID, mInput);

			mInput = "";
		}
	}

    private void Update()
    {
		if (Event.current != null && Event.current.type == EventType.KeyUp)
		{
			var keyCode = Event.current.keyCode;
			string ctrl = GUI.GetNameOfFocusedControl();
			Debug.Log(ctrl);
			if (ctrl == "Name")
			{
				if (keyCode == KeyCode.Return)
				{
					// Enter key pressed on the input field for the player's nickname -- change the player's name.
					TNManager.playerName = mName;
					if (Application.isPlaying) GUI.FocusControl("Chat Window");
				}
			}
			else if (ctrl == "Chat Input")
			{
				if (keyCode == KeyCode.Return)
				{
					Send();
					if (Application.isPlaying) GUI.FocusControl("Chat Window");
				}
			}
			else if (keyCode == KeyCode.Return)
			{
				// Enter key pressed -- give focus to the chat input
				if (Application.isPlaying) GUI.FocusControl("Chat Input");
			}
			else if (keyCode == KeyCode.Slash)
			{
				mInput = "/";
				if (Application.isPlaying) GUI.FocusControl("Chat Input");
			}
		}
	}

	public void PressedChange()
	{
		TNManager.playerName = mName;
	}

	public void PressedSend()
    {
		Send();
	}

	/// <summary>
	/// Helper function that prints the specified config node and its children.
	/// </summary>

	void PrintConfig (string path, DataNode node, Color color)
	{
		if (!string.IsNullOrEmpty(path)) node = node.GetHierarchy(path);

		if (node != null)
		{
			var lines = node.ToString().Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
			foreach (string s in lines) AddToChat(s.Replace("\t", "    "), color);
		}
		else AddToChat("[" + path + "] has not been set", color);
	}

	void PrintServerData (string path = "")
	{
		AddToChat("Server Data (" + path + "):", Color.yellow);
		PrintConfig(path, TNManager.serverData, Color.yellow);
	}

	void PrintChannelData (string path = "")
	{
		var ch = TNManager.GetChannel(mChannelID);
		AddToChat("Channel #" + ch.id + " Data (" + path + "):", Color.green);
		PrintConfig(path, ch.dataNode, Color.green);
	}
}

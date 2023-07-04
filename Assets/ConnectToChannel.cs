using System;
using GNet;
using MHLab.ReactUI.Core;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MHLab.ReactUI.Scripts
{
    [Serializable]
    public class ConnectToChannelPayload : BrowserPayload
    {
        public int ChannelId;
    }

    public class ConnectToChannel : MonoBehaviour
    {
        protected void OnEnable()
        {
            BrowserBridge.RegisterHandler<ConnectToChannelPayload>(ConnectToChannelHandler);
        }
        protected void OnDisable()
        {
            BrowserBridge.UnregisterHandler<ConnectToChannelPayload>();
        }

        private void ConnectToChannelHandler(ConnectToChannelPayload e)
        {
            var levelName = "";
            switch(e.ChannelId)
            {
                case 1:
                    levelName = "Chat";
                    break;
                case 2:
                    levelName = "Portfolio";
                    break;
                case 3:
                    levelName = "Sandbox";
                    break;
            }
            var inChannelOtherThanChat = TNManager.channels.size > 1 || (TNManager.channels.size == 1 && !TNManager.IsInChannel(1));
            if (inChannelOtherThanChat)
            {
                for (var i = 0; i < TNManager.channels.size; i++)
                {
                    //Leave everything but chat
                    if (TNManager.channels.buffer[i].id != 1)
                    {
                        TNManager.LeaveChannel(TNManager.channels.buffer[i].id);
                    }
                }
            }
            TNManager.JoinChannel(e.ChannelId, levelName, false, 255, "", false);
        }
    }
}
using GNet;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace MHLab.ReactUI.Core
{
    public class BrowserBridgeCommand
    {
        public Type CommandType { get; private set; }
        public Action<BrowserPayload> Command { get; private set; }


        public BrowserBridgeCommand(Type commandType, Action<BrowserPayload> command)
        {
            CommandType = commandType;
            Command = command;
        }
    }

    public class BrowserBridge : MonoBehaviour
    {
        private static readonly Dictionary<string, BrowserBridgeCommand> m_CommandBindings = new Dictionary<string, BrowserBridgeCommand>();
        private static BrowserBridge m_Instance = null;
        public static string GamerTag = "Guest";

        private void Awake()
        {
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

        //Called from Javascript like this
        //unityInstance.SendMessage('MyGameObject', 'MyFunction', 'MyString');
        //that's why this is a monobehaviour
        public void ProcessPacket(string serializedPacket)
        {
            var browserPacket = JsonUtility.FromJson<BrowserPacket<BrowserEventPayload>>(serializedPacket);
            if (m_CommandBindings.ContainsKey(browserPacket.payloadType))
            {
                var command = m_CommandBindings[browserPacket.payloadType];
                command.Command.DynamicInvoke(JsonUtility.FromJson(serializedPacket, command.CommandType));
                return;
            }
            Debug.LogError("Did not process packet: " + serializedPacket);
        }

        public void SetGamerTag(string gamerTag)
        {
            GamerTag = gamerTag;
        }
        
        public static void Dispatch<T>(BrowserPacket<T> browserPacket) where T : BrowserPayload
        {
            DispatchPacket(JsonUtility.ToJson(browserPacket));
        }

        private static Action<BrowserPayload> Convert<T>(Action<T> function) where T : BrowserPayload
        {
            return (command) => function(command as T);
        }

        public static void RegisterHandler<T>(Action<T> handler) where T : BrowserPayload
        {
            var payloadType = BrowserPacket<T>.GetPayloadType();
            if (m_CommandBindings.ContainsKey(payloadType))
            {
                m_CommandBindings[payloadType] = new BrowserBridgeCommand(typeof(T), Convert(handler));
            }
            else
            {
                m_CommandBindings.Add(payloadType, new BrowserBridgeCommand(typeof(T), Convert(handler)));
            }
        }

        public static void UnregisterHandler<T>() where T : BrowserPayload
        {
            var payloadType = BrowserPacket<T>.GetPayloadType();
            if (m_CommandBindings.ContainsKey(payloadType))
            {
                m_CommandBindings.Remove(payloadType);
            }
        }

#if UNITY_EDITOR
        public static void DispatchPacket(string browserPacket)
        {
            //Debug.Log(browserPacket);
        }
#else
    [DllImport("__Internal")]
    public static extern void DispatchPacket(string browserPacket);
#endif

    }
}
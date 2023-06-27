using System;

namespace MHLab.ReactUI.Core
{
    [Serializable]
    public class BrowserPacket<T> where T : BrowserPayload
    {
        public string payloadType;
        public T payload;

        public BrowserPacket()
        {

        }

        public BrowserPacket(T p)
        {
            payloadType = GetPayloadType();
            payload = p;
        }

        public static string GetPayloadType()
        {
            return typeof(T).Name;
        }
    }
}
using System;

namespace MHLab.ReactUI.Core
{
    [Serializable]
    public struct BrowserPacketHeader
    {
        public string eventName;

        public BrowserPacketHeader(string eventName)
        {
            this.eventName = eventName;
        }
    }
}
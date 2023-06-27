using System;

namespace MHLab.ReactUI.Core
{
    [Serializable]
    public class BrowserClosePayload : BrowserEventPayload
    {
        public string source;
    }
}
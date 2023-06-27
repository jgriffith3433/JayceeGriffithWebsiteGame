using System;

namespace MHLab.ReactUI.Core
{
    [Serializable]
    public class BrowserInitPayload : BrowserEventPayload
    {
        public string hubUrl;
    }
}
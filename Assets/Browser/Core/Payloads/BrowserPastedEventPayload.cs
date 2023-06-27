using System;

namespace MHLab.ReactUI.Core
{
    [Serializable]
    public class BrowserPastedEventPayload : BrowserEventPayload
    {
        public string pastedText;
    }
}
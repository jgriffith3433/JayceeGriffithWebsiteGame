using System;
using MHLab.ReactUI.Core;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MHLab.ReactUI.Scripts
{
    [Serializable]
    public class BrowserInputSwitched : BrowserPayload
    {
        public bool isUnity;
    }

    public class BrowserInputSwitcher : MonoBehaviour
    {
        protected void OnEnable()
        {
            BrowserBridge.RegisterHandler<BrowserInputSwitched>(BrowserInputSwitched);
        }
        protected void OnDisable()
        {
            BrowserBridge.UnregisterHandler<BrowserInputSwitched>();
        }

        private void BrowserInputSwitched(BrowserInputSwitched e)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
    WebGLInput.captureAllKeyboardInput = e.isUnity;
#endif
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2) || (Input.touchCount > 0 && Input.GetTouch(0).phase != TouchPhase.Ended))
            {
                if (MouseOverUILayerObject.IsPointerOverUIObject())
                {
                    var browserPacket = new BrowserPacket<BrowserInputSwitched>(new BrowserInputSwitched
                    {
                        isUnity = true,
                    });
                    BrowserBridge.Dispatch(browserPacket);
                }
                else
                {
                    var browserPacket = new BrowserPacket<BrowserInputSwitched>(new BrowserInputSwitched
                    {
                        isUnity = false,
                    });
                    BrowserBridge.Dispatch(browserPacket);
                }
            }
        }
    }
}
using GNet;
using MHLab.ReactUI.Core;
using UnityEngine;

public class TurnOnGameObjectAfterBrowserInit : MonoBehaviour
{
    [SerializeField] private GameObject[] m_GameObjectsToTurnOn = null;
    [SerializeField] private string m_HubUrl = GNetConfig.HubUrl;

    private static TurnOnGameObjectAfterBrowserInit m_Instance = null;

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

    private void OnEnable()
    {
#if UNITY_EDITOR
        BrowserInit(new BrowserInitPayload
        {
            hubUrl = m_HubUrl
        });
#else
        BrowserBridge.RegisterHandler<BrowserInitPayload>(BrowserInit);
        BrowserBridge.RegisterHandler<BrowserClosePayload>(BrowserClose);
#endif
    }

    private void OnDisable()
    {
        BrowserBridge.UnregisterHandler<BrowserInitPayload>();
        BrowserBridge.UnregisterHandler<BrowserClosePayload>();
    }

    private void BrowserInit(BrowserInitPayload browserInitPayload)
    {
        GNetConfig.HubUrl = browserInitPayload.hubUrl;
        foreach (var gameObjectToTurnOn in m_GameObjectsToTurnOn)
        {
            gameObjectToTurnOn.SetActive(true);
        }
    }

    private void BrowserClose(BrowserClosePayload browserClosePayload)
    {
        Application.Quit();
    }
}

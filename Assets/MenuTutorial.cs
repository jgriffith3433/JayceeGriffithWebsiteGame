using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GNet;

public class MenuTutorial : MonoBehaviour
{
    [SerializeField] private ExampleMenu m_ExampleMenu = null;
    [SerializeField] private GameObject m_CreateServerArrow = null;
    [SerializeField] private GameObject m_ChooseServerOrChannelArrow = null;
    [SerializeField] private GameObject m_JoinServerArrow = null;
    [SerializeField] private GameObject m_JoinGameArrow = null;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private void Update()
    {
        if (!TNManager.isConnectedToHub)
        {
            return;
        }

        m_CreateServerArrow.SetActive(!TNManager.isConnectedToGameServer && m_ExampleMenu.ServerList != null && m_ExampleMenu.ServerList.list.size == 0);
        m_ChooseServerOrChannelArrow.SetActive(
            !TNManager.isInChannel &&
            ((!TNManager.isConnectedToGameServer && m_ExampleMenu.ServerList != null && m_ExampleMenu.ServerList.list.size > 0 && string.IsNullOrEmpty(m_ExampleMenu.GetSelectedServerOrChannelName())) ||
            (TNManager.isConnectedToGameServer && string.IsNullOrEmpty(m_ExampleMenu.GetSelectedServerOrChannelName())))
        );
        m_JoinServerArrow.SetActive(
            !TNManager.isConnectedToGameServer && !string.IsNullOrEmpty(m_ExampleMenu.GetSelectedServerOrChannelName())
        );
        m_JoinGameArrow.SetActive(
            TNManager.isConnectedToGameServer && !TNManager.isInChannel && !string.IsNullOrEmpty(m_ExampleMenu.GetSelectedServerOrChannelName())
        );
    }
}

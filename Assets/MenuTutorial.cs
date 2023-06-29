using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GNet;

public class MenuTutorial : MonoBehaviour
{
    [SerializeField] private ExampleMenu m_ExampleMenu = null;
    [SerializeField] private GameObject m_CreateServerArrow = null;
    [SerializeField] private GameObject m_PressMenuArrow = null;
    [SerializeField] private GameObject m_JoinServerArrow = null;
    [SerializeField] private GameObject m_JoinGameArrow = null;
    [SerializeField] private GameObject m_Menu = null;

    private bool m_PressedMenu = false;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (!TNManager.isConnectedToHub)
        {
            return;
        }

        if (m_Menu.activeSelf)
        {
            m_PressedMenu = true;
        }

        m_PressMenuArrow.gameObject.SetActive(!m_PressedMenu && !TNManager.isConnectedToGameServer);

        m_CreateServerArrow.SetActive(!TNManager.isConnectedToGameServer && m_ExampleMenu.ServerList != null && m_ExampleMenu.ServerList.list.size == 0);
        
        m_JoinServerArrow.SetActive(
            !TNManager.isConnectedToGameServer && !string.IsNullOrEmpty(m_ExampleMenu.GetSelectedServerOrChannelName())
        );
        m_JoinGameArrow.SetActive(
            TNManager.isConnectedToGameServer && !TNManager.isInChannel && !string.IsNullOrEmpty(m_ExampleMenu.GetSelectedServerOrChannelName())
        );
    }
}

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

        if (m_PressedMenu)
        {
            m_PressMenuArrow.gameObject.SetActive(false);
        }
        else
        {
            m_PressMenuArrow.SetActive(
            !TNManager.isInChannel &&
            ((!TNManager.isConnectedToGameServer && m_ExampleMenu.ServerList != null && m_ExampleMenu.ServerList.list.size > 0 && string.IsNullOrEmpty(m_ExampleMenu.GetSelectedServerOrChannelName())) ||
            (TNManager.isConnectedToGameServer && string.IsNullOrEmpty(m_ExampleMenu.GetSelectedServerOrChannelName())))
        );
        }

        m_CreateServerArrow.SetActive(!TNManager.isConnectedToGameServer && m_ExampleMenu.ServerList != null && m_ExampleMenu.ServerList.list.size == 0);
        
        m_JoinServerArrow.SetActive(
            !TNManager.isConnectedToGameServer && !string.IsNullOrEmpty(m_ExampleMenu.GetSelectedServerOrChannelName())
        );
        m_JoinGameArrow.SetActive(
            TNManager.isConnectedToGameServer && !TNManager.isInChannel && !string.IsNullOrEmpty(m_ExampleMenu.GetSelectedServerOrChannelName())
        );
    }
}

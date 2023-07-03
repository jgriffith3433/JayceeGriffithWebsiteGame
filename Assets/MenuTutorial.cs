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
    [SerializeField] private GameObject m_ChooseGameArrow = null;
    [SerializeField] private GameObject m_JoinGameArrow = null;
    [SerializeField] private ExampleMenu m_Menu = null;

    private bool m_PressedMenu = false;

    private void Update()
    {
        if (!TNManager.isConnectedToHub)
        {
            return;
        }
        
        var inChannelOtherThanChat = TNManager.channels.size > 1 || (TNManager.channels.size == 1 && !TNManager.IsInChannel(1));
        if (m_Menu.MenuVisible)
        {
            m_PressedMenu = true;
        }
        m_PressMenuArrow.gameObject.SetActive(!m_PressedMenu && !inChannelOtherThanChat);

        m_CreateServerArrow.SetActive(!TNManager.isConnectedToGameServer && m_ExampleMenu.ServerList != null && m_ExampleMenu.ServerList.list.size == 0);
        
        m_JoinServerArrow.SetActive(
            !TNManager.isConnectedToGameServer && !string.IsNullOrEmpty(m_ExampleMenu.GetSelectedServerOrChannelName())
        );
        m_ChooseGameArrow.SetActive(
            (TNManager.isConnectedToGameServer && m_Menu.MenuVisible && !inChannelOtherThanChat && string.IsNullOrEmpty(m_ExampleMenu.GetSelectedServerOrChannelName())) ||
            (!TNManager.isConnectedToGameServer && m_Menu.MenuVisible && string.IsNullOrEmpty(m_ExampleMenu.GetSelectedServerOrChannelName()) && m_ExampleMenu.ServerList != null && m_ExampleMenu.ServerList.list.size > 0)
        );
        m_JoinGameArrow.SetActive(
            TNManager.isConnectedToGameServer && !inChannelOtherThanChat && !string.IsNullOrEmpty(m_ExampleMenu.GetSelectedServerOrChannelName())
        );
    }
}

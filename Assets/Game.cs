using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] private ExampleMenu m_Menu = null;

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void PressMenu()
    {
        m_Menu.ShowHideMenu();
    }
}

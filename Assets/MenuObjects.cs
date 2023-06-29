using UnityEngine;
using UnityEngine.UI;

public class MenuObjects : MonoBehaviour
{
    public Button m_StartStopServerButton = null;
    public Text m_StartStopServerButtonText = null;
    public Button m_JoinLeaveServerButton = null;
    public Text m_JoinLeaveServerButtonText = null;
    public Button m_SoundOnButton;
    public Button m_SoundOffButton;
    public Button[] m_ServerOrChannelButtonList = null;
    public Text[] m_ServerOrChannelButtonTextList = null;
    public Button m_JoinLeaveChannelButton = null;
    public Text m_JoinLeaveChannelButtonText = null;
    public GameObject m_LeftMenu = null;
    public GameObject m_ServerOrChannelListMenu = null;
    public GameObject m_Information = null;
}

using Cinemachine;
using GNet;
using UnityEngine;

public class FollowGoobaUnderwaterCamera : MonoBehaviour
{
    [SerializeField] private CinemachineTargetGroup m_TargetGroup = null;
    [SerializeField] private GameObject m_Gooba = null;

    private TestPlayer m_Player = null;

    private void Awake()
    {
        if (TNManager.instance != null && TNManager.player == null)
        {
            return;
        }
        var player = TNManager.GetPlayer(TNManager.player.id);
        if (player != null)
        {
            var playerCharacter = GameObject.Find("Player_" + player.name);
            if (playerCharacter != null)
            {
                m_Player = playerCharacter.GetComponent<TestPlayer>();
                if (m_Player != null)
                {
                    //m_TargetGroup.AddMember(m_Gooba.transform, 0.75f, 1f);
                    //m_TargetGroup.AddMember(m_Player.GravDraggedObject.transform, 0.25f, 1f);
                }
            }
        }
    }

    private void LateUpdate()
    {
        if (m_Player != null && m_TargetGroup != null)
        {
            //m_TargetGroup.transform.rotation = m_Gooba.transform.rotation;
        }
        else
        {
            if (TNManager.instance != null && TNManager.player != null)
            {
                var player = TNManager.GetPlayer(TNManager.player.id);
                if (player != null)
                {
                    var playerCharacter = GameObject.Find("Player_" + player.name);
                    if (playerCharacter != null)
                    {
                        m_Player = playerCharacter.GetComponent<TestPlayer>();
                    }
                }
            }
        }
    }
}

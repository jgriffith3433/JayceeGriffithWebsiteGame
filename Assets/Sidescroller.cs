using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sidescroller : MonoBehaviour
{
    public CameraPositions CameraPositions = null;
    public Transform m_BackgroundLayer = null;
    public Transform m_ForegroundLayer = null;
    public Transform m_ForegroundLayerStartHelper = null;
    public Transform m_ForegroundLayerEndHelper = null;
    public Transform m_BackgroundLayerStartHelper = null;
    public Transform m_BackgroundLayerEndHelper = null;
    public Transform m_PlayerStartHelper = null;
    public Transform m_PlayerEndHelper = null;
    public Transform m_CameraStartHelper = null;
    public Transform m_CameraEndHelper = null;
    public float m_SimulatedPlayerXSpeed = 5f;
    private float m_SidescrollT = 0f;
    private TestPlayer m_Player = null;
    private bool m_Scroll = false;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(2);
        m_Scroll = true;
    }

    public void SetPlayer(TestPlayer player)
    {
        m_Player = player;
    }

    private void Update()
    {
        if (!m_Scroll)
        {
            return;
        }
        var playerX = 0f;
        if (m_Player == null)
        {
#if UNITY_EDITOR
            playerX = SimulatePlayer();
#endif
        }
        else
        {
            playerX = m_Player.UserControlAI.transform.position.x;
        }
        m_SidescrollT = Mathf.InverseLerp(m_PlayerStartHelper.position.x, m_PlayerEndHelper.position.x, playerX);
        m_ForegroundLayer.position = Vector3.Lerp(m_ForegroundLayerStartHelper.position, m_ForegroundLayerEndHelper.position, 1 - m_SidescrollT);
        m_BackgroundLayer.position = Vector3.Lerp(m_BackgroundLayerStartHelper.position, m_BackgroundLayerEndHelper.position, 1 - m_SidescrollT);
        Camera.main.transform.position = Vector3.Lerp(m_CameraStartHelper.position, m_CameraEndHelper.position, m_SidescrollT);
    }

    private float SimulatePlayer()
    {
        return Mathf.PingPong(Time.time * m_SimulatedPlayerXSpeed, m_PlayerEndHelper.position.x);
    }
}

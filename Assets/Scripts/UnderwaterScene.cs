using GNet;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class UnderwaterScene : MonoBehaviour
{
    [SerializeField] private float m_RespawnY = -100f;
    [SerializeField] private PlayableDirector m_PlayableDirector = null;

    private TestPlayer m_Player = null;

    private void Awake()
    {
        m_PlayableDirector.stopped += PlayableDirectorStopped;
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
                    m_Player.SetRespawnY(m_RespawnY);
                }
            }
        }
    }

    private void Start()
    {
        for (var i = 0; i < SceneManager.sceneCount; i++)
        {
            var s = SceneManager.GetSceneAt(i);
            if (s.name == "Underwater")
            {
                SceneManager.SetActiveScene(s);
                break;
            }
        }
    }

    private void PlayableDirectorStopped(PlayableDirector obj)
    {
        if (TNManager.instance != null)
        {
            TNManager.LeaveChannel(4);
        }
    }
}

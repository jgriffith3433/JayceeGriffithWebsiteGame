using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class Intro : MonoBehaviour
{
    public MenuObjects[] MenuObjects;
    public PlayableDirector IntroTimeline;
    private float m_PreviousVolume = 1;

    [SerializeField]
    private ShowGameObjectOnInput m_AudioControls = null;

    [SerializeField]
    private GameObject m_ClickToPlay = null;

    private void Awake()
    {
        m_PreviousVolume = AudioListener.volume;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.touchCount > 0)
        {
            if (IntroTimeline.state != PlayState.Playing)
            {
                IntroTimeline.Play();
                m_AudioControls.gameObject.SetActive(true);
                m_ClickToPlay.SetActive(false);
            }
        }
    }

    private void OnEnable()
    {
        IntroTimeline.stopped += IntroTimeline_stopped;
    }

    private void OnDisable()
    {
        IntroTimeline.stopped -= IntroTimeline_stopped;
    }

    public void Skip()
    {
        IntroTimeline.Stop();
    }

    private void IntroTimeline_stopped(PlayableDirector obj)
    {
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }

    public void Mute()
    {
        m_PreviousVolume = AudioListener.volume;
        AudioListener.volume = 0;
        foreach (var menuObject in MenuObjects)
        {
            menuObject.m_SoundOnButton.interactable = true;
            menuObject.m_SoundOffButton.interactable = false;
        }
    }

    public void Unmute()
    {
        AudioListener.volume = m_PreviousVolume;
        foreach (var menuObject in MenuObjects)
        {
            menuObject.m_SoundOnButton.interactable = false;
            menuObject.m_SoundOffButton.interactable = true;
        }
    }
}

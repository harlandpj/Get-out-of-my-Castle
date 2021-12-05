using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MenuHandler : MonoBehaviour
{
    AudioSource audio;
    public AudioClip chime;

    [SerializeField]
    private GameObject[] HeroBackgrounds;

    private void Awake()
    {
        audio = GetComponent<AudioSource>();
        DisableHeroBackgrounds();
    }

    private void DisableHeroBackgrounds()
    {
       foreach (GameObject background in HeroBackgrounds)
        {
            background.SetActive(false);
        }
    }

    public void HeroButtonAClicked()
    {
        MainManager.HeroSelected = "A";
        PlayButtonNoise();
        SelectHeroBackground();
    }

    private void SelectHeroBackground()
    {
        // disable all hero backgrounds, then set current one active
        DisableHeroBackgrounds();
        string heroToFind = "HeroBackground" + MainManager.HeroSelected;

        // the background must be a child under the canvas NOT in any other container
        transform.FindChild(heroToFind).gameObject.SetActive(true);
    }

    private void PlayButtonNoise()
    {
        audio.PlayOneShot(chime, 1f);
    }

    public void HeroButtonBClicked()
    {
        MainManager.HeroSelected = "B";
        PlayButtonNoise();
        SelectHeroBackground();
    }

    public void HeroButtonCClicked()
    {
        MainManager.HeroSelected = "C";
        PlayButtonNoise();
        SelectHeroBackground();
    }

    public void HeroButtonDClicked()
    {
        MainManager.HeroSelected = "D";
        PlayButtonNoise();
        SelectHeroBackground();
    }

    public void StartButtonClicked()
    {
        // load the main scene
        SceneManager.LoadScene(1); // defined in index in build settings window
    }

    public void ExitButtonClicked()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
}

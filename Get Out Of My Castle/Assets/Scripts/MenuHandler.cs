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
    AudioSource theAudio;
    public AudioClip chime;

    [SerializeField]
    private GameObject[] HeroBackgrounds;

    private void Awake()
    {
        theAudio = GetComponent<AudioSource>();
        DisableHeroBackgrounds();
        HighlightPreviousHero(); // check and highlight previously selected hero from earlier session
    }

    private void HighlightPreviousHero()
    {
        // check if we have run the game before and highlight any previously used
        if (MainManager.HeroSelected.Length > 0)
        {
            SelectHeroBackground();
            MainManager.Instance.LoadUserData();
        }
    }

    private void DisableHeroBackgrounds()
    {
       foreach (GameObject background in HeroBackgrounds)
        {
            background.SetActive(false);
        }
    }

    // ABSTRACTION
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
        HighlightSelectedHero();
        MainManager.Instance.SaveUserData();
    }

    private void HighlightSelectedHero()
    {
        string heroToFind = "HeroBackground" + MainManager.HeroSelected;

        // the background must be a child under the canvas NOT in any other container
        transform.Find(heroToFind).gameObject.SetActive(true);
    }

    private void PlayButtonNoise()
    {
        theAudio.PlayOneShot(chime, 1f);
    }

    // ABSTRACTION
    public void HeroButtonBClicked()
    {
        MainManager.HeroSelected = "B";
        PlayButtonNoise();
        SelectHeroBackground();
    }

    // ABSTRACTION
    public void HeroButtonCClicked()
    {
        MainManager.HeroSelected = "C";
        PlayButtonNoise();
        SelectHeroBackground();
    }

    // ABSTRACTION
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

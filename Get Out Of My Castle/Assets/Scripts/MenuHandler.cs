using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using TMPro;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class MenuHandler : MonoBehaviour
{
    AudioSource theAudio;

    [Header("Selection Noise")]
    [SerializeField]
    public AudioClip chime;

    [Header("Backgrounds Displayed on Selection")]
    [SerializeField]
    private GameObject[] HeroBackgrounds;

    [Header("Skybox Used")]
    [SerializeField]
    private Material defaultSkybox;

    private void Awake()
    {
        theAudio = GetComponent<AudioSource>();
        
        UnityEngine.RenderSettings.skybox = defaultSkybox;
        SceneLoader.SetLightIntensity(1); // reset to day in case came from main game

        DisableHeroBackgrounds();

        if (MainManager.HeroSelected != null)
        {
            HighlightPreviousHero(); // check and highlight previously selected hero from earlier session
        }
        
    }

    private void Start()
    {
        MainManager.Instance.LoadUserData();
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
        MainManager.Instance.bGameOver = false;

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

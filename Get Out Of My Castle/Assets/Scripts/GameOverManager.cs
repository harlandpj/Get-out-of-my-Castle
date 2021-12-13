using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using TMPro;


#if UNITY_EDITOR
using UnityEditor;
#endif



public class GameOverManager : MonoBehaviour
{
    [SerializeField]
    Material defaultSkybox;

    public GameObject nameInput;


    private void Awake()
    {
        UnityEngine.RenderSettings.skybox = defaultSkybox;
        SceneLoader.SetLightIntensity(1);
    }
    
    void Start()
    {
        
    }

    public void RestartButtonPressed()
    {
        try
        {
            // Save user data
            MainManager.Instance.SaveUserData();
        }
        catch (NullReferenceException e)
        {
            Debug.LogError($"Exception in GameOVerManager, Main Manager doesn't exist yet! {e.Data}");
        }

        // reset health and lives (in this order, may change setters later)
        MainManager.Lives = 3; // set this before health!
        MainManager.Health = 100; // set this after Lives

        // load the main menu scene
        SceneManager.LoadScene(0); // defined in index in build settings window
    }
    
    public void NameEntered()
    {
        string theName = nameInput.GetComponent<Text>().text.ToString();

        MainManager.PlayerName = theName;
    }

    public void ExitButtonClicked()
    {
        // Save user data
        MainManager.Instance.SaveUserData();

#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
}

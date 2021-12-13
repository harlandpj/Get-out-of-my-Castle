using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ScoreDisplayPanel : MonoBehaviour
{
    [Header("Player Data")]
    [SerializeField]
    private TMP_Text highScore;
    
    [SerializeField]
    private TMP_Text lives;

    [SerializeField]
    private TMP_Text health;

    [SerializeField]
    private TMP_Text score;
    
    [SerializeField]
    private TMP_Text lastPlayerName;

    [Header("Quit Button")]

    [SerializeField]
    GameObject quitButton;

    // Start is called before the first frame update
    void Start()
    {
        InitialiseScores();
    }

    private void InitialiseScores()
    {
        MainManager.Instance.LoadUserData();

        highScore.SetText(MainManager.HighScore.ToString());
        score.SetText("0");
        lives.SetText("3");
        health.SetText(MainManager.Health.ToString());
        lastPlayerName.SetText(MainManager.PlayerName.ToString());
    }

    private void FixedUpdate()
    {
        // inefficient - change to a ui event instead later on
        UpdateScores(); 
    }

    private void UpdateScores()
    {
        // better to update as UI events but don't have time today!
        highScore.SetText(MainManager.HighScore.ToString());
        score.SetText(MainManager.Score.ToString());
        lives.SetText(MainManager.Lives.ToString());
        health.SetText(MainManager.Health.ToString());
    }

    public void QuitButtonPressed()
    {
        // reset health and lives (in this order, may change setters later)
        MainManager.Lives = 3; // set this before health!
        MainManager.Health = 100; // set this after Lives

        // load the main menu scene
        SceneManager.LoadScene(0); // defined in index in build settings window
    }

    public void ToggleQuitButtoon(bool onOff)
    {
        quitButton.SetActive(onOff);
    }
}

using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using UnityEngine.SceneManagement;

public class MainManager : MonoBehaviour
{
    // Start / update() not needed for now

    public static MainManager Instance { get; private set; } // shared by all instances
    
    public static string HeroSelected; // which hero has been selected/previously selected by user
    public static string PlayerName;
    public static int HighScore;
    public static int Score;

    [Header("Life Lost Sound")]
    [SerializeField]
    private AudioClip lifeLost;

    // dual function setter which reduces lives when health is zero
    // and won't allow health to be reset when lives are zero
    // so always set lives first when using this!
    private static int m_Health;
    public static int Health
    {
        get => m_Health;
        set
        {
            if (value >= 0)
            {
                if (value <= 100)
                {
                    m_Health = value; // increase health
                }
                else
                {
                    m_Health =100;
                }
            }
            else
            {
                if (value <=0)
                {
                    m_Health =0;
                    Lives -= 1;
                 
                    if (Lives > 0)
                    {
                        Health = 100;
                    }
                }
                else
                {
                    m_Health = value; // adding a negative
                }
            }
        }
    }

    private static int maxLives = 3;
    
    private static int m_Lives;
    public static int Lives
    {
        get => m_Lives;
        set
        {
            if (value >= 0)
            {
                if (value <= maxLives)
                {
                    m_Lives = value;
                }
                else
                {
                    m_Lives = maxLives;
                }
            }
            else
            {
                // deduct a life
                if (value <= 0)
                {
                    m_Lives = 0;
                }
                else
                {
                    m_Lives = value; // add a negative value
                }
            }
        }
    }


    public void AddDamageToPlayerHealth(int amountDamage)
    {
        if (Health - amountDamage ==0)
        {
            // life lost
            PlayLifeLost();
        }

        Health += amountDamage;

        CheckGameOver();
    }

    private void PlayLifeLost() 
    {
        GetComponent<AudioSource>().PlayOneShot(lifeLost,1f);
    }

    public bool bGameOver = false;

    public void CheckGameOver()
    {
        if (!bGameOver)
        {
            // check lives left
            if (Lives <= 0)
            {
                bGameOver = true;
                SaveUserData();
                //SceneManager.LoadScene(2, LoadSceneMode.Additive);
                SceneManager.LoadScene(2);
            }
        }
    }

    public void IncreasePlayerHealth(int amountDamage)
    {
        // remember, when adding health it should be a positive amount
        Health += amountDamage;
    }

    // setup the static object and retain data on loading
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadUserData();

        // MAY Change this back, not sure!
        ///////////////// ALWAYS SET LIVES FIRST BEFORE SETTING HEALTH \\\\\\\\\\\\\
        Lives = 3;
        Health = 100;
        ///////////////// ALWAYS SET LIVES FIRST BEFORE SETTING HEALTH \\\\\\\\\\\\\
    }

    private void Start()
    {
        // load again
        LoadUserData();
    }

    [System.Serializable]
    class SaveData
    {
        public string HeroName; // A or B or C or D
        public string PlayName; // players name
        public int Score; // players high score
    }

    public void SaveUserData()
    {
        SaveData data = new SaveData();

        data.HeroName = HeroSelected;

        if (Score > HighScore)
        {
            // new high score
            data.Score = Score;
            
            if (PlayerName.Length != 0)
            {
                data.PlayName = PlayerName;
            }
        }
        else
        {
            data.Score = HighScore;
            data.PlayName = PlayerName;
        }

        // convert to JSON format and save to file
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
        Debug.Log($"Saving Data to: {Application.persistentDataPath} in savefile.json");
    }

    public void LoadUserData()
    {
        string path = Application.persistentDataPath + "/savefile.json";

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            HeroSelected = data.HeroName;
            PlayerName = data.PlayName;
            HighScore = data.Score;
            Debug.Log($"Loading Data from: {Application.persistentDataPath} in savefile.json");
        }
    }
}
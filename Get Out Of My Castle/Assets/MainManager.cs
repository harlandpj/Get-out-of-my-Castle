using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

public class MainManager : MonoBehaviour
{
    // Start / update() not needed for now

    public static MainManager Instance { get; private set; } // shared by all instances
    
    public static string HeroSelected; // which hero has been selected/previously selected by user
    public static string PlayerName;
    public static int HighScore;

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
    }

    [System.Serializable]
    class SaveData
    {
        public string HeroName;
        public string PlayName;
        public int Score;
    }

    public void SaveUserData()
    {
        SaveData data = new SaveData();

        data.HeroName = HeroSelected;

        // convert to JSON format and save to file
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
    }

    public void LoadUserData()
    {
        string path = Application.persistentDataPath + "/savefile.json";

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            HeroSelected = data.HeroName;
        }
    }
}
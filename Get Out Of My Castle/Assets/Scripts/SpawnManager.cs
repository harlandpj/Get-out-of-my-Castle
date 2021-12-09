using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    private bool playerChecked = false;
    GameObject player1, player2, player3, player4;
    
    [SerializeField]
    private GameObject[] enemies;

    [SerializeField]
    private GameObject[] pickups;

    // Start is called before the first frame update
    void Start()
    {
        player1 = GameObject.Find("PlayerA");
        player2 = GameObject.Find("PlayerB");
        player3 = GameObject.Find("PlayerC");
        player4 = GameObject.Find("PlayerD");

        CheckPlayerCreated();
        InvokeRepeating("SpawnRandomEnemy", 5f, 15f);
        InvokeRepeating("SpawnRandomPickup", 5f, 15f);
    }

    private int totalAllowed = 100;
    private int totalPickupsAllowed = 100;

    private int FindTotalEnemies()
    {
        int maxNowOnScreen = GameObject.FindGameObjectsWithTag("ChiefOrc").Length +
            GameObject.FindGameObjectsWithTag("Troll").Length +
            GameObject.FindGameObjectsWithTag("Wolf").Length;

        return maxNowOnScreen;
    }

    private int FindTotalPickups()
    {
        int maxNowOnScreen = GameObject.FindGameObjectsWithTag("Pickup1").Length +
            GameObject.FindGameObjectsWithTag("Pickup2").Length +
            GameObject.FindGameObjectsWithTag("Pickup3").Length;
        return maxNowOnScreen;
    }

    private void SpawnRandomPickup()
    {
        // not taking any notice of potential obstacles in way
        // just spawning for this game

        int maxOnScreen = FindTotalPickups();
        int randNumber = UnityEngine.Random.Range(0, 3); // select a random enemy

        if (randNumber >2)
        {
            // put in as never seemed to be two!
            randNumber = 2;
        }

        float randX = UnityEngine.Random.Range(-300, 400);
        float randZ = UnityEngine.Random.Range(-300, 400);

        Vector3 randomPos = new Vector3(randX, 0.6f, randZ);

        // only spawn a maximum amount at any point
        if (maxOnScreen < totalPickupsAllowed)
        {
            Instantiate(pickups[randNumber], randomPos, Quaternion.identity);
        }
    }

    private void SpawnRandomEnemy()
    {
        // not taking any notice of potential obstacles in way
        // just spawning for this game
        
        int maxOnScreen = FindTotalEnemies();
        int randNumber = UnityEngine.Random.Range(0, 3); // select a random enemy

        if (randNumber >2)
        {
            // put in as never seemed to be two!
            randNumber = 2;
        }

        float randX = UnityEngine.Random.Range(-200, 400);
        float randZ = UnityEngine.Random.Range(-300, 200);

        Vector3 randomPos = new Vector3(randX, -0.239f, randZ);

        // only spawn a maximum amount at any point
        if (maxOnScreen < totalAllowed)
        {
            Instantiate(enemies[randNumber], randomPos, Quaternion.identity);
        }
    }

    private void CheckPlayerCreated()
    {
        // we need to instantiate the Player selected in the menu
        if (!playerChecked)
        {
            playerChecked = true;

            // set spawn position for new Player
            Vector3 spawnPos = new Vector3(127, 0.02f, -278.6f);

            // turn off all players
            TurnOffPlayers();

            // now enable the correct hero
            switch (MainManager.HeroSelected)
            {
                case "A":
                    {
                        TurnOffPlayers();
                        player1.SetActive(true);
                        break;
                    }

                case "B":
                    {
                        TurnOffPlayers();
                        player2.SetActive(true);
                        break;
                    }

                case "C":
                    {
                        TurnOffPlayers();
                        player3.SetActive(true);
                        break;
                    }

                case "D":
                    {
                        TurnOffPlayers();
                        player4.SetActive(true);
                        break;
                    }
            }
        }
    }

    private void TurnOffPlayers()
    {
        player1.SetActive(false);
        player2.SetActive(false);
        player3.SetActive(false);
        player4.SetActive(false);
    }

    // reset enemies to new player
    protected virtual void ResetEnemies()
    {
        BaseEnemy[] enemies = GameObject.FindObjectsOfType<BaseEnemy>();

        foreach (BaseEnemy enemy in enemies)
        {
            enemy.ChangePlayerReference();
        }
    }
}


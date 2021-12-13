using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    private bool playerChecked = false;
    GameObject player1, player2, player3, player4;

    [Header("Enemy Prefabs (Set from Scene)")]
    [SerializeField]
    private GameObject[] enemies;

    [Header("Collectable Items")]
    [SerializeField]
    private GameObject[] pickups;

    [Header("Max Number of Enemies & Pickups Allowed")]
    [SerializeField]
    private int totalAllowed = 100;

    [SerializeField]
    private int totalPickupsAllowed = 150;

    // Start is called before the first frame update
    void Start()
    {
        player1 = GameObject.Find("PlayerA");
        player2 = GameObject.Find("PlayerB");
        player3 = GameObject.Find("PlayerC");
        player4 = GameObject.Find("PlayerD");

        CheckPlayerCreated();
        InvokeRepeating("SpawnRandomEnemy", 5f, 30f);
        InvokeRepeating("SpawnRandomPickup", 5f, 15f);

        SpawnInitialPickups();
        SpawnInitialWanderingEnemies(); 
    }

    private void SpawnInitialPickups()
    {
        for (int i=0; i<30; i++)
        {
            SpawnRandomPickup();
        }
    }

    private void SpawnInitialWanderingEnemies()
    {
        // Spawn enemies at random positions, with random patrol points
        // just enough to make it interesting, but not overly difficult!
        for (int i = 0; i < 12; i++)
        {
            SpawnRandomEnemy();
        }
    }

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
        int randNumber = UnityEngine.Random.Range(1, 11); // select a random enemy

        // make wolves less likely as faster
        if (randNumber <5)
        {
            // chief orc
            randNumber = 0;
        }
        if (randNumber >=5 && randNumber <=8)
        {
            randNumber = 1;
        }
        else
        {
            randNumber = 0;
        }

        // spawn inside castle grounds
        float randX = UnityEngine.Random.Range(10, 250);
        float randZ = UnityEngine.Random.Range(-150, 150);

        Vector3 randomPos = new Vector3(randX, 1.15f, randZ);

        // only spawn a maximum amount at any point
        if (maxOnScreen < totalPickupsAllowed)
        {
            Instantiate(pickups[randNumber], randomPos, pickups[randNumber].transform.rotation);
        }
    }

    private void SpawnRandomEnemy()
    {
        // not taking any notice of potential obstacles in way
        // just spawning for this game
        
        int maxOnScreen = FindTotalEnemies(); // how many currently on screen
        int randNumber = UnityEngine.Random.Range(0, 3); // select a random enemy

        if (randNumber >2)
        {
            // put in as never seemed to be two!
            randNumber = 2;
        }

        float randX = UnityEngine.Random.Range(10,250);
        float randZ = UnityEngine.Random.Range(-150, 150);

        // FOR TESTING - spawn in front of castle area
        //float randX = UnityEngine.Random.Range(100, 250);
        //float randZ = UnityEngine.Random.Range(-100, 0);

        Vector3 randomPos = new Vector3(randX, -0.239f, randZ);

        // only allow a maximum number of enemies on screen at any point
        if (maxOnScreen < totalAllowed)
        {
            GameObject enemy;
            enemy = Instantiate(enemies[randNumber], randomPos, Quaternion.identity);

            // now set random patrol positions
            // (change later to spawn zones (which must be clear of obstacles)
            // but for now, just inside castle area)
         
            for (int i = 0; i < 3; i++)
            {
                GameObject toCreate;

                // spawn patrol points inside castle grounds
                float randx = UnityEngine.Random.Range(10, 250);
                float randz = UnityEngine.Random.Range(-150, 150);
                
                Vector3 pos = new Vector3(randx, 0f, randz);

                toCreate = Instantiate(new GameObject(), pos, Quaternion.identity);

                // now set patrol point in correct script
                switch (randNumber)
                {
                    case 0: 
                        {
                             OrcChief myScriptReference = enemy.GetComponent<OrcChief>();
                            myScriptReference.SetupDynamicPatrolPoint(i,toCreate);
                            myScriptReference.SetToFirstPatrolPosition();
                            break; 
                        }
                    case 1: 
                        {
                            Troll myScriptReference = enemy.GetComponent<Troll>();
                            myScriptReference.SetupDynamicPatrolPoint(i, toCreate);
                            myScriptReference.SetToFirstPatrolPosition();
                            break; 
                        }
                    case 2: 
                        {
                            Wolf myScriptReference = enemy.GetComponent<Wolf>();
                            myScriptReference.SetupDynamicPatrolPoint(i, toCreate);
                            myScriptReference.SetToFirstPatrolPosition();
                            break; 
                        }
                }
            }

            Debug.Log($"Finished Setting Patrol points in dynamic enemy: {enemy.tag.ToString()}!");
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


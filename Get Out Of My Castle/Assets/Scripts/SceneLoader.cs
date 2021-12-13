using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField]
    Material[] skyBoxes;

    // Start is called before the first frame update
    void Start()
    {
        GenerateSkybox();    
    }

    private void GenerateSkybox()
    {
        // selects a random skybox to use this time around
        int boxNumber = Random.Range(0, skyBoxes.Length);

        if (boxNumber > 3)
        {
            boxNumber = 3;
        }

        if (skyBoxes.Length > 0)
        {
            UnityEngine.RenderSettings.skybox = skyBoxes[boxNumber];
            SetLightIntensity(boxNumber);
        }
    }

    public static void SetLightIntensity(int boxNumber)
    {
        // setup new light rotation and intensity to simulate different times of day
        //
        // main directional light angle
        //
        // dawn -40 degrees, intensity 0.9
        // day 50 degrees, intensity 1
        // dusk -73 degrees, intensity 0.6
        // night -88 deg, intensity 0.1

        
        Quaternion newRotation = new Quaternion();
        float newIntensity = new float();

        // standard rotation of a directional light at time of writing is (50,-30,0)
        switch (boxNumber)
        {
            case 0:
                {
                    // dawn
                    newRotation = Quaternion.Euler(new Vector3(-40f, -30f, 0f));
                    newIntensity = 0.9f;
                    break;
                }
            case 1:
                {
                    // day
                    newRotation = Quaternion.Euler(new Vector3(50f, -30f, 0f));
                    newIntensity = 1f;
                    break;
                }
            case 2:
                {
                    // dusk
                    newRotation = Quaternion.Euler(new Vector3(073f, -30f, 0f));
                    newIntensity = 0.6f;
                    break;
                }
            case 3:
                {
                    // night
                    newRotation = Quaternion.Euler(new Vector3(-88f, -30f, 0f));
                    newIntensity = 0.1f;
                    break;
                }
        }

        GameObject mainLight = GameObject.FindGameObjectWithTag("MainLight");

        // find main light in active scene and reset it
        mainLight.GetComponent<Light>().intensity = newIntensity;
        mainLight.GetComponent<Transform>().rotation =  newRotation;
    }
}

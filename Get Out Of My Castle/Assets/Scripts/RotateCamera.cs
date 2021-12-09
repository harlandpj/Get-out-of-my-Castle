using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RotateCamera : MonoBehaviour
{
    private float rotationSpeed = 12f;
    public GameObject focusPoint = null;

    // Start is called before the first frame update
    void Start()
    {
        //focusPoint = GameObject.Find("Camera Focus Point");

        // setup focus point again
        //GameObject parentObj = gameObject.GetComponentInChildren<Camera>().transform.parent.gameObject;
        //GameObject parentObj = gameObject.GetComponentInChildren<Camera>().transform.parent.gameObject;
        focusPoint = transform.parent.gameObject;

        if (focusPoint == null)
        {
            // no focus point found
            UnityEngine.Debug.Log("no focus point found!");
        }
    }

    private void LateUpdate()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        transform.Rotate(Vector3.up, horizontalInput * Time.deltaTime * rotationSpeed);
        transform.LookAt(focusPoint.transform.position);
    }
}

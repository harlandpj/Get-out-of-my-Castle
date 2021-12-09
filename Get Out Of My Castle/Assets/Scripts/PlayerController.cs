using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent( typeof(Rigidbody), typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody playerRB;
    private Animator playerAnim;
    private CharacterController playerController;
    private AudioSource audioSource;

    // audio clips to use for swordplay
    [SerializeField]
    private AudioClip swordInAir;

    [SerializeField]
    private AudioClip swordHit;

    [SerializeField]
    private float playerSpeed = 20.0f;

    [SerializeField]
    private float rotationSpeed = 180f;
    private Vector3 rotation;

    // Start is called before the first frame update
    void Start()
    {
        SetupPlayer();
    }

    private void SetupPlayer()
    {
        playerRB = GetComponent<Rigidbody>();
        playerController = gameObject.GetComponent<CharacterController>();
        playerAnim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        // setup focus point again
        GameObject parentObj = gameObject.GetComponentInChildren<Camera>().transform.parent.gameObject;
        
    }

    // Update is called once per frame
    void Update()
    {
        // get player input
        float horizontalInput = Input.GetAxis("Horizontal"); // -1 to 1 input from key press (left/right X Axis)
        float verticalInput = Input.GetAxis("Vertical");   // -1 to 1 input from key press (down/up Z Axis)
        
        rotation = new Vector3(0, Input.GetAxisRaw("Horizontal") * rotationSpeed * Time.deltaTime * 0.23f, 0);
        Vector3 move = new Vector3(0, 0, Input.GetAxisRaw("Vertical") * Time.deltaTime);

        move = transform.TransformDirection(move);
        playerController.Move(move * playerSpeed);

        Vector3 newPosition;

        newPosition = new Vector3(transform.position.x, 0f, transform.position.z) ; // always on ground

        // rotate player
        transform.Rotate(rotation);

        CheckKeyPressed();
    }

    private void CheckKeyPressed()
    {
        // check key pressed
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // attack
            AttackWithSword();
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            // stop attack
            StopAttackWithSword();
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            // run
            StartRunning();
        }
        else if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            // stop running
            StopRunning();
        }
    }

    private bool bAttackEnabled = true;

    private void AttackWithSword()
    {
        if (bAttackEnabled)
        {
            // sword attack
            playerAnim.SetBool("Attack", true);
            bAttackEnabled = false;
            CheckProximity();
        }

        StartCoroutine(StopSpaceSpamming());
    }

    IEnumerator StopSpaceSpamming()
    {
        yield return new WaitForSeconds(1f);
        bAttackEnabled = true;
    }

    private void CheckProximity()
    {
        // check if we are facing an enemy and within range to do damge
        
        RaycastHit pointHit;

        // Shoot the ray to see if we hit something!
        Vector3 shootPoint = new Vector3(transform.position.x, 0.5f, transform.position.z); // shoot from 0.5f above ground


        if (Physics.Raycast(shootPoint, transform.forward, out pointHit, 1.75f))
        {
            // ok... we hit something in range with the raycast
            Debug.Log("Laser hit :  " + pointHit.transform.name);
            Debug.Log("Position of Ray cast Hit (x,y,z) is: x=   " + pointHit.point.x + ", y=   " + pointHit.point.y + ", z=   " + pointHit.point.z + ".");
            Debug.DrawRay(shootPoint, transform.TransformDirection(Vector3.forward) * pointHit.distance, Color.white, 2.0f);

            // check who collided with us - if player update game manager with score        
            if (pointHit.transform.CompareTag("ChiefOrc") ||
                pointHit.transform.CompareTag("Troll") ||
                pointHit.transform.CompareTag("Wolf"))
            {
                // hit an enemy, make a noise
                Debug.Log($"Hit a {pointHit.transform.tag}");
                PlaySwordHitSound();

                // add damage to enemy
                pointHit.transform.gameObject.GetComponent<BaseEnemy>().AddDamage();
            }
        }
        else
        {
            PlaySwordInAirSound();
        }
    }

    private void PlaySwordHitSound()
    {
        StartCoroutine(SwordHittingSound());
    }

    IEnumerator SwordHittingSound()
    {
        audioSource.PlayOneShot(swordHit, 1f);
        yield return new WaitForSeconds(swordHit.length);
    }

    private void PlaySwordInAirSound()
    {
        StartCoroutine(SwordInAirSound());
    }

    IEnumerator SwordInAirSound()
    {
        audioSource.PlayOneShot(swordInAir, 1f);
        yield return new WaitForSeconds(swordInAir.length);
    }

    private void StopAttackWithSword()
    {
        // sword attack
        playerAnim.SetBool("Attack", false);
    }

    private void StartRunning()
    {
        playerAnim.SetFloat("Speed", 1.5f);
    }

    private void StopRunning()
    {
        playerAnim.SetFloat("Speed", 0f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("ChiefOrc") ||
            collision.gameObject.CompareTag("Troll") ||
            collision.gameObject.CompareTag("Wolf"))
        {
            // added this to try to solve collisions pushing Player up
            // in air despite it being restricted in Y position in editor
            // something to do with Overlapping colliders and Physics engine pushing it up!
            // may change back to just translate/move manually!

            //Vector3 currentTransform = transform.position;
            //Vector3 newTransform = new Vector3(currentTransform.x, 0f, currentTransform.z);
            //transform.position = newTransform;
            //playerController.Move(newTransform);

            // set velocity to zero - hack (didn't work)!
            Vector3 zeroVelocity = new Vector3(0f,0f,0f);
            playerRB.velocity = zeroVelocity;
        }
    }
}

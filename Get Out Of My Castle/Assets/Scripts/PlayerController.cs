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
        // check input
        CheckKeyPressed();
    }

    private void MoveRotatePlayer()
    {
        // get player input
        rotation = new Vector3(0, Input.GetAxisRaw("Horizontal") * rotationSpeed*Time.deltaTime * 0.23f, 0);
        Vector3 moveDirection = Vector3.zero;
        float speed = 1000;
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        moveDirection += transform.forward * moveZ * speed * Time.deltaTime;

        playerController.Move(moveDirection * Time.deltaTime);
        moveDirection = Vector3.zero;

        // rotate player
        transform.Rotate(rotation);
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
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            // run (must always use GetKey here rather than Keydown otherwise buffers
            // input if still holding move forward (UP arrow) & stuck inbetween 2 or more enemies!)
            MoveRotatePlayer();
            StartRunning();
        }
        else if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            StopRunning();
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            MoveRotatePlayer();
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            // run (must always use GetKey here rather than Keydown otherwise buffers
            // input if still holding move forward (UP arrow) & stuck inbetween 2 or more enemies!)
            MoveRotatePlayer();
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            // run (must always use GetKey here rather than Keydown otherwise buffers
            // input if still holding move forward (UP arrow) & stuck inbetween 2 or more enemies!)
            MoveRotatePlayer();
        }
        else
        {
            // prevent it randomly running!
            Vector3 moveDirection = Vector3.zero;
            playerController.Move(moveDirection * Time.deltaTime);
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
            StartCoroutine(StopSpaceSpamming());
        }
    }

    IEnumerator StopSpaceSpamming()
    {
        yield return new WaitForSeconds(swordHit.length);
    }

    private void CheckProximity()
    {
        // check if we are facing an enemy and within range to do damge
        
        RaycastHit pointHit;
        RaycastHit pointHit2;

        // Shoot the ray to see if we hit something!
        Vector3 shootPoint = new Vector3(transform.position.x, 0.5f, transform.position.z); // shoot from 0.5f above ground
        Vector3 shootPoint2 = new Vector3(transform.position.x, 1.5f, transform.position.z); // shoot from 1.5f above ground

        if (Physics.Raycast(shootPoint, transform.forward, out pointHit, 2f /*1.75f*/) || Physics.Raycast(shootPoint, transform.forward, out pointHit2, 2f /*1.75f*/))
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
        bAttackEnabled = true;
    }

    private void PlaySwordInAirSound()
    {
        StartCoroutine(SwordInAirSound());
    }

    IEnumerator SwordInAirSound()
    {
        audioSource.PlayOneShot(swordInAir, 1f);
        yield return new WaitForSeconds(swordInAir.length);
        bAttackEnabled = true;
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

        // prevent keyboard buffer from continuing movement
        // which sometimes happens
        playerController.Move(new Vector3(0,0,0));
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

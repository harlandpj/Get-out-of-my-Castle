using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


// base class for all enemies, this is never instantiated
// enemies have an array of patrol points as they will not be stationary
// except in specific circumstances later on (wandering enemies)

[RequireComponent(typeof(NavMeshAgent), typeof(Animator))]
public abstract class BaseEnemy : MonoBehaviour
{    
    protected Animator m_Anim; // animator component
    protected NavMeshAgent navAgent; // navigation mesh
    protected Transform playerTransform; // transform of player
    protected GameObject Player;

    [SerializeField]
    protected AudioClip attackNoise;

    protected AudioSource audioSource;

    
    [SerializeField] 
    protected int m_Health; // current health

    // ENCAPSULATION
    public int Health
    {
        get => m_Health;
        protected set
        {
            if (value < 100)
            {
                if (value <= 0)
                {
                    m_Health = 0;
                }
                else
                {
                    m_Health = value;
                }
            }
            else
            {
                m_Health = 100;
            }
        }
    }

    // ENCAPSULATION
    protected int m_CurrentPatrolPoint = 0; // current patrol point number (importantly not a GameObject) of the enemy
    public int CurrentPatrolPoint 
    {
        get => m_CurrentPatrolPoint;
        protected set 
        { 
            if (value < 0)
            {
                // invalid point
                m_CurrentPatrolPoint = 0;
            }
            else if (value > m_PatrolPoints.Length-1)
            {
                // invalid patrol point set (zero based array)
                m_CurrentPatrolPoint = m_PatrolPoints.Length - 1;
            }
            else 
            {
                // anything else
                m_CurrentPatrolPoint = value;
            }
        }
    }

    protected GameObject m_DestinationPoint; // destination is either Player or next Patrol Point

    protected int m_Strength; // some enemies are stronger than others (will take longer to kill)
    protected int m_DamageDealt; // damage dealt to player when attacking
    protected float m_Speed; // basic speed of enemy
    protected float m_EyesightDistance; // some enemies are btter at seeing the player (for raycast distance)

    // will use a simple set of bools for now - maybe change to a state machine later on
    protected bool m_Attacking; // true if currently attacking player
    protected bool m_Patrolling = true; // always starts off patrolling between points (unless overridden)
    private bool m_Reversing = false; // sets to true at end of patrol, reset to false when start position reached again
    
    protected string m_EnemyName; // name of enemy for UI display (later on in dev)

    // serialized to enable setting private field thru inspector
    [SerializeField]
    protected GameObject[] m_PatrolPoints; // array of patrol points

    private void Awake()
    {
        SetupEnemy();
    }

    // POLYMORPHISM
    protected virtual void Start()
    {
        ChangePlayerReference(); // as player needs resetting
    }

    // change Player info on changing player
    public virtual void ChangePlayerReference()
    {
        // this has been added to allow changing the player character
        // dynamically after selection from the main menu
        Player = GameObject.FindGameObjectWithTag("Player");
        playerTransform = Player.transform;
    }


    // POLYMORPHISM - override in derived & always call base
    public virtual void SetupEnemy()
    {
        navAgent = GetComponent<NavMeshAgent>();
        m_Anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        // customiseable by derived classes
        m_Health = 100;
        m_Speed = 5f;
        m_DamageDealt = 5;
        m_EyesightDistance = 30f; // default vision distance 
        
        navAgent.speed = m_Speed; // set speed of character

        Player = GameObject.FindGameObjectWithTag("Player");
        playerTransform = Player.GetComponent<Transform>();

        // set initial destination to be first patrol point
        if (m_PatrolPoints.Length == 0)
        {
            m_DestinationPoint = Player;
        }
        else
        {
            m_CurrentPatrolPoint = 0;
            m_DestinationPoint = m_PatrolPoints[0];
        }
        
    }

    // POLYMORPHISM, override in derived class to provide any specific attack functionality
    public virtual void AttackPlayer()
    {
        m_Attacking = true;
        m_Anim.SetBool("Attack", true);
        MakeAttackNoise();
    }

    // POLYMORPHISM
    public virtual void OnCollisionEnter(Collision collision)
    {
        // check if we collided with player
        if (collision.gameObject.CompareTag("Player"))
        {
            AttackPlayer();
        }
    }

    // POLYMORPHISM
    protected virtual void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StopAttacking();
        }
    }

    // POLYMORPHISM
    protected virtual void StopAttacking()
    {
        m_Attacking = false;
        m_Anim.SetBool("Attack", false);
    }

    // POLYMORPHISM
    // adds damage to health of enemy
    public virtual void AddDamage()
    {
        AmIDead();
    }

    // POLYMORPHISM
    // move the enemy
    public virtual void MoveEnemy()
    {
        m_Anim.SetFloat("Speed", 1.5f);

        if (PlayerSeenOrInRange())
        {
            MoveTowardsPlayer();
        }
        else
        {
            Patrol();
        }
    }

    protected bool PlayerSeenOrInRange()
    {
        // checks if player is visible, i.e. within eyesight range in plain view,
        // and not hidden behind something in scene
        if (Vector3.Distance(transform.position, 
                             Player.transform.position) <= m_EyesightDistance)
        {
            // player IS in range to be attacked!
            //

            // *REMOVED FOR NOW*
            // but only attack if visible (this bit removed for now for simplicity, but WILL use in future!)
            // need to change to Cone shape ray(?) and cast in all 4 directions while moving

            //RaycastHit pointHit = new RaycastHit();

            //Vector3 direction = new Vector3(Player.transform.position.x - transform.position.x,
            //                                Player.transform.position.y - transform.position.y,
            //                                Player.transform.position.z - transform.position.z);
            //Ray shootRay = new Ray();

            //shootRay.direction = direction;
            //shootRay.origin = transform.position;

            // shoot a ray and see if we can hit Player (i.e. with range AND visible)
            //Vector3 shootPoint = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);

            //if (Physics.Raycast(shootPoint, transform.forward, out pointHit, m_EyesightDistance))
            //if (Physics.Raycast(shootRay.direction, pointHit.transform.position, m_EyesightDistance))
            //{
            //// we hit something in range with the raycast
            //Debug.Log("Ray hit :  " + pointHit.transform.name);
            //Debug.Log("Position of Ray cast Hit (x,y,z) is: x=   " + pointHit.point.x + ", y=   " + pointHit.point.y + ", z=   " + pointHit.point.z + ".");
            //Debug.DrawRay(shootRay, transform.TransformDirection(Vector3.forward) * pointHit.distance, Color.white, 2.0f);
            //Debug.DrawRay(transform.position, shootRay.direction, Color.white, 2.0f);

            // check if Player
            //    if (pointHit.transform.CompareTag("Player"))
            //    {
            //        Debug.Log("Player is VISIBLE!");
            //        m_Patrolling = false; // starts move towards player
            //        return true;
            //    }
            //    else
            //    {
            //        m_Patrolling = true; // end move towards player
            //        return false;
            //    }
            //}
            //else
            //{
            //    m_Patrolling = true;
            //    return false;
            //}

            //m_Anim.SetFloat("Speed", 0); // stop and attack
            //m_Anim.SetBool("Attack", true);

            m_Patrolling = false;
            return true;
        }

        m_Patrolling = true;
        m_Anim.SetFloat("Speed", 1.5f); // continue
        m_Anim.SetBool("Attack", false);
        return false;
    }

    protected void MoveTowardsPlayer()
    {
        // just chase the player
        navAgent.destination = playerTransform.position;

        // check if in attack range
        CloseEnoughToAttack();
    }

    private void CloseEnoughToAttack()
    {
        if (Vector3.Distance(transform.position, Player.transform.position) <= 2f)
        {
            AttackPlayer();
        }
        else
        {
            StopAttacking();
        }
    }

    // enemy dies
    public virtual void Die() 
    {
        AmIDead();
    }

    private void AmIDead()
    {
        // checks if have died
        if (m_Health <= 0)
        {
            Death();
        }
    }

    private void Death()
    {
        // destroy with slight delay (would use a Pool Manager in a longer project to avoid GC)
        PlayDeathSound();
    }


    private void PlayDeathSound()
    {
        // start a coroutine here to play a sound
        StartCoroutine("PlayDeath");
    }

    IEnumerator PlayDeath()
    {
        // just for now do this!
        Debug.Log("Enemy has Died");

        yield return new WaitForSeconds(2);
        Destroy(gameObject, 0.5f);
    }

    // enemy patrol function
    public virtual void Patrol()
    {
        if (m_PatrolPoints != null)
        {
            // check if we have any patrol points set
            if (m_PatrolPoints.Length == 0)
            {
                // this enemy just goes towards the player
                navAgent.destination = Player.transform.position;
            }
            else
            {
                // just patrol from our current position to next patrol point
                // reversing at end to go back to original position
                if (Vector3.Distance(transform.position,
                                     m_DestinationPoint.transform.position) <= 1.5f)
                {
                    // check if we were reversing and reached beginning
                    if (m_Reversing)
                    {
                        if (m_CurrentPatrolPoint == 0)
                        {
                            // we are at start of patrol
                            m_Reversing = false;
                            m_CurrentPatrolPoint++;
                        }
                        else
                        {
                            // go to next point
                            m_CurrentPatrolPoint--;
                        }
                    }
                    else
                    {
                        // not currently reversing, check have reached end of patrol
                        if (m_CurrentPatrolPoint >= m_PatrolPoints.Length - 1)
                        {
                            // go in reverse
                            m_Reversing = true;
                            m_CurrentPatrolPoint--;
                        }
                        else
                        {
                            // continue on to next patrol point
                            m_CurrentPatrolPoint++;
                        }
                    }

                    // set next destination
                    m_DestinationPoint = m_PatrolPoints[m_CurrentPatrolPoint];
                    navAgent.destination = m_DestinationPoint.transform.position;
                }
                else
                {
                    // set destination
                    navAgent.destination = m_DestinationPoint.transform.position;
                }
            }
        }
        else
        {
            // this enemy just goes towards the player
            navAgent.destination = Player.transform.position;
        }
    }

    // will make a noise specific to the type of enemy
    protected virtual void MakeAttackNoise()
    {
        // implement different noises in derived classes
        StartCoroutine(PlayingAttack());
    }

    protected virtual IEnumerator PlayingAttack()
    {
        // must have something here as can't override audioClips!
        //audioSource.PlayOneShot(attackNoise, 1f);
        yield return new WaitForSeconds(0.1f);
    }
}

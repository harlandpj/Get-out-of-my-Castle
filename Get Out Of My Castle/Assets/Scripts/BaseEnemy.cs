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

    [SerializeField]
    protected Transform playerTransform; // transform of player

    [SerializeField]
    protected GameObject Player; // our player

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
    protected int m_CurrentPatrolPoint = 0; // current patrol point number
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

    protected GameObject m_DestinationPoint; // destination is either towards Player or next Patrol Point

    protected int m_Strength; // some enemies are stronger than others (and will take longer to kill)
    protected int m_DamageDealt; // damage dealt to player when attacking
    protected float m_Speed; // basic speed of enemy
    protected float m_EyesightDistance; // some enemies are btter at seeing the player (for raycast distance)

    // will use a simple set of bools for now - maybe change to a state machine later on
    [Header ("Enemy States")]
    [SerializeField]
    protected bool m_Attacking; // true if currently attacking player

    [SerializeField]
    protected bool m_Patrolling = true; // always starts off patrolling between points (unless overridden)

    [SerializeField]
    private bool m_Reversing = false; // sets to true at end of patrol, reset to false when start position reached again
    
    protected string m_EnemyName; // name of enemy for UI display (later on in dev)

    [Space]
    [SerializeField]
    protected GameObject[] m_PatrolPoints; // array of patrol points

    protected BaseEnemy()
    {
        //m_PatrolPoints = new GameObject[3];
    }

    private void Awake()
    {
        SetupEnemy();
    }

    // set to first position
    public virtual void SetToFirstPatrolPosition()
    {
        CurrentPatrolPoint = 0;
        m_DestinationPoint = m_PatrolPoints[0];
    }

    // POLYMORPHISM
    protected virtual void Start()
    {
        // as player always needs resetting due to having 4 on screen with same name at start
        ChangePlayerReference(); 
    }

    // change Player info on changing player
    public virtual void ChangePlayerReference()
    {
        // has to be added here when game objects exist, to allow changing the player character
        // dynamically after selection from the main menu
        Player = GameObject.FindGameObjectWithTag("Player");
        playerTransform = Player.transform;
    }

    //
    public virtual void SetupDynamicPatrolPoint(int arrayNum, GameObject obj)
    {
        // set with dynamic object created in spawn manager
        m_PatrolPoints[arrayNum] = obj;
    }

    // POLYMORPHISM - override in derived & always call base
    public virtual void SetupEnemy()
    {
        // always call base in derived class

        navAgent = GetComponent<NavMeshAgent>();
        m_Anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        
        // customiseable in derived classes
        Health = 100; // max health
        m_Speed = 5f; // default speed
        m_DamageDealt = 5;  // default attack damage
        m_EyesightDistance = 30f; // default vision distance 
        
        navAgent.speed = m_Speed; // set speed of character

        Player = GameObject.FindGameObjectWithTag("Player");
        playerTransform = Player.GetComponent<Transform>();

        // removed dynamic points for now as probably have to override whole function
        // set initial destination to be first patrol point
        if (m_PatrolPoints !=null)
        {
            CurrentPatrolPoint = 0;
            m_DestinationPoint = m_PatrolPoints[0];
        }
        
        // removed dynamic patrol points for now
        // just set to go towards Player
        if (m_PatrolPoints == null)
        {
            Debug.LogError("BaseEnemy.SetupEnemy Patrol Points Array is NULL!");
        }

        if (m_PatrolPoints.Length == 0)
        {
            //SetupDynamicPatrolPoints();
            //m_DestinationPoint = m_PatrolPoints[0];
            m_DestinationPoint = Player;
        }
        else
        {
            // set to start position of patrol points
            CurrentPatrolPoint = 0;
            m_DestinationPoint = m_PatrolPoints[0];
        }
    }

    // POLYMORPHISM, override in derived class to provide any specific attack functionality
    public virtual void AttackPlayer()
    {
        Debug.Log($"Now in AttackPlayer: m_Attacking = {m_Attacking}");

        if (!m_Attacking)
        {
            Debug.Log($"Now in AttackPlayer: {0} {m_Attacking}");
            // ok to attack
            Debug.Log("Now in AttackPlayer starting attack!");
            m_Attacking = true;
            m_Anim.SetBool("Attack", true);
            m_Anim.SetFloat("Speed", 0f);
            MakeAttackNoise(); // this also resets m_Attacking flag
            StartCoroutine(DontAddDamage());
        }
    }

    IEnumerator DontAddDamage()
    {
        yield return new WaitForSeconds(2f);
        m_Anim.SetBool("Attack", false);
        AddDamage();
    }

    // POLYMORPHISM
    protected virtual void StopAttacking()
    {
        m_Attacking = false;
        m_Anim.SetBool("Attack", false);
        stopMovingForNow = false;
    }

    // POLYMORPHISM
    // adds damage to health of enemy
    public virtual void AddDamage()
    {
        // die if health is zero, derived must call this base method
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

            // *REMOVED FOR NOW*
            // Only attack if visible (this bit removed for now for simplicity, but WILL use in future!)
            // need to change to raycast in 3 forward directions (at -45,0,45 degrees) while moving to see if Player found

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

            if (!bMakeApproachNoise)
            {
                // make approach noise
                MakeApproachNoise();
            }

            m_Patrolling = false;
            return true;
        }
        else
        {
            if (bMakeApproachNoise)
            {
                StopApproachNoise();
            }

            m_Patrolling = true;
            m_Anim.SetFloat("Speed", 1.5f); // continue
            m_Anim.SetBool("Attack", false);
            approachingThePlayer = false;
            return false;
        }
    }

    protected bool approachingThePlayer = false;
    protected bool bMakeApproachNoise = false;
    protected bool bPlayingApproach = false;
    protected bool bPlayingAttack = false;
    protected bool stopMovingForNow = false;

    //POLYMORPHISM
    protected virtual void MakeApproachNoise()
    {
    }
    
    //POLYMORPHISM
    protected virtual void StopApproachNoise()
    {
    }

    protected void MoveTowardsPlayer()
    {
       
        if (!stopMovingForNow)
        {
            // just chase the player
            navAgent.destination = playerTransform.position;
        }
        
        // check if in attack range
        CloseEnoughToAttack();
    }

    private void CloseEnoughToAttack()
    {
        if (Vector3.Distance(transform.position, Player.transform.position) <= 3f)
        {
            // in attack range, stop moving when attacking
            stopMovingForNow = true;

            // set navAgent destination to be my current transform to avoid repeated movement
            // towards player during attack
            navAgent.SetDestination(transform.position);
            Debug.Log("Stopped, now Attacking Player!");
            AttackPlayer();
        }
        else
        {
            // stop attack and restart movement
            StopAttacking();
            navAgent.SetDestination(Player.transform.position);
            stopMovingForNow = false;
            m_Attacking = false;
        }
    }

    // enemy dies (horribly :o) )
    protected virtual void Die() 
    {
        AmIDead();
    }

    protected void AmIDead()
    {
        // checks if have died
        if (Health <= 0)
        {
            Death();
        }
    }

    private void Death()
    {
        // destroy with slight delay (use a Pool Manager in a longer project to avoid GC overheads)
        PlayDeathSound();
    }

    protected virtual void PlayDeathSound()
    {
        // start a coroutine here to play a sound
        StartCoroutine("PlayDeath");
    }

    // never called
    protected virtual IEnumerator PlayDeath()
    {
        yield return new WaitForSeconds(1);
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


                if (m_DestinationPoint == null)
                {
                    Debug.Log("Destination doesnt exist!");
                    m_DestinationPoint = Player;
                }

                if (Vector3.Distance(transform.position,
                                     m_DestinationPoint.transform.position) <= 1.5f)
                {
                    // check if we were reversing and reached beginning
                    if (m_Reversing)
                    {
                        if (CurrentPatrolPoint == 0)
                        {
                            // we are at start of patrol
                            m_Reversing = false;
                            CurrentPatrolPoint++;
                        }
                        else
                        {
                            // go to next point
                            CurrentPatrolPoint--;
                        }
                    }
                    else
                    {
                        // not currently reversing, check have reached end of patrol
                        if (CurrentPatrolPoint >= m_PatrolPoints.Length - 1)
                        {
                            // go in reverse
                            m_Reversing = true;
                            CurrentPatrolPoint--;
                        }
                        else
                        {
                            // continue on to next patrol point
                            CurrentPatrolPoint++;
                        }
                    }

                    // set next destination
                    m_DestinationPoint = m_PatrolPoints[CurrentPatrolPoint];
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
            //SetupDynamicPatrolPoints();
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
        yield return new WaitForSeconds(0.1f);
    }

    protected virtual void StopPlayingAttack() { }
}

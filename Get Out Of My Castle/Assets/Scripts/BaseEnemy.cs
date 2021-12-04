using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// base class for all enemies in game, this is never instantiated
[RequireComponent(typeof(NavMeshAgent), typeof(Animator))]
public abstract class BaseEnemy : MonoBehaviour
{    
    protected Animator m_Anim; // animator component
    protected NavMeshAgent navAgent; // navigation mesh
    protected Transform playerTransform; // transform of player
        
    // override in derived class to provide any specific attack functionality
    public abstract void AttackPlayer();

    protected int m_Health; // current health
    public int Health
    {
        get => m_Health;

        private set
        {
            if (value < 100)
            {
                m_Health = value;
            }
            else
            {
                m_Health = 100;
            }
        }
    }

    protected int m_Strength; // some enemies are stronger than others (will take longer to kill)
    protected int m_DamageDealt; // damage dealt to player when attacking

    protected float m_Speed; // speed of enemy
    protected float m_recoveryTime; // recovery time after being attacked (if not dead)
    protected float m_EyesightDistance; // some enemies are btter at seeing the player (for raycast distance)

    // will use a simple set of bools for now - maybe change to a state machine later on
    protected bool m_Attacking; // true if currently attacking player
    protected bool m_Patrolling; // true if patrolling between points
    protected string m_EnemyName; // name of enemy for UI display

    // serialized to set private field thru inspector
    [SerializeField]
    protected Vector3[] m_PatrolPoints; // array of patrol points


    private void Awake()
    {
        navAgent = GetComponent<NavMeshAgent>();
        m_Anim = GetComponent<Animator>();

        playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    private void FixedUpdate()
    {
        // just set to go towards player for now
        Move();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // check if we collided with player
        if (collision.gameObject.CompareTag("Player"))
        {
            AttackPlayer();
        }
    }
    // call to add damage to enemy
    public virtual void AddDamage()
    {

    }

    // move the enemy
    public virtual void Move()
    {
        navAgent.destination = playerTransform.position;
    }

    // enemy dies
    public virtual void Die() 
    { 

    }

    // enemy patrol function
    public virtual void Patrol()
    {

    }

    public virtual bool CanSeePlayer()
    {
        return false;
    }
    
    protected virtual void MakeNoise()
    {

    }
}

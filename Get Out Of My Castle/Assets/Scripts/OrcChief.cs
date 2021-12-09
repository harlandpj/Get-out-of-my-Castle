using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// INHERITANCE
public class OrcChief : BaseEnemy
{
    public OrcChief()
    {
        m_EnemyName = "Orc Chief";
    }

    // POLYMORPHISM
    public override void SetupEnemy()
    {
        base.SetupEnemy();
        m_Speed = 3.5f; // Orcs are slower
        m_DamageDealt = 10; // deals more damage than base enemy
        m_EyesightDistance = 25f; // default vision distance lower
        navAgent.speed = m_Speed; // set speed of character
        m_Anim = GetComponent<Animator>();
    }

    // POLYMORPHISM
    public override void AddDamage()
    {
        // harder to kill - so add less damage
        Health -= 3;
        base.AddDamage();
    }

    // POLYMORPHISM
    public override void AttackPlayer()
    {
        // attack the player
        m_Anim.SetBool("Attack", true);
        Debug.Log("Orc Attacking Player!");
    }
    // POLYMORPHISM
    protected override void StopAttacking()
    {
        // stop attacking only when away from player
        if (Vector3.Distance(transform.position, Player.transform.position) > 2f)
        {
            m_Attacking = false;
            m_Anim.SetBool("Attack", false);
        }
    }

    // POLYMORPHISM
    public override void Patrol()
    {
        base.Patrol(); // added - but may not be necessary in this example project
    }

    // POLYMORPHISM
    public override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);
    }

    // POLYMORPHISM
    public override void MoveEnemy()
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

    private void FixedUpdate()
    {
        // move the enemy
        MoveEnemy();       
    }

    protected override void Start()
    {
        base.Start();
        m_Anim = GetComponent<Animator>();
    }

    // POLYMORPHISM: make a noise specific to the type of enemy
    protected override void MakeAttackNoise()
    {
        StartCoroutine(PlayingAttack());
    }

    protected override IEnumerator PlayingAttack()
    {
        // must have something here as can't override audioClips!
        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(attackNoise, 1f);
        yield return new WaitForSeconds(attackNoise.length);
    }
}

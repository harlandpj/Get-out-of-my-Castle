using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// INHERITANCE
public class Troll : BaseEnemy
{
    public Troll()
    {
        m_EnemyName = "Troll";
    }
    
    // POLYMORPHISM
    public override void SetupEnemy()
    {
        base.SetupEnemy();
        
        m_Anim = GetComponent<Animator>();
        m_Speed = 4.5f; // trolls are faster
        m_DamageDealt = 10; // deals more damage than base enemy
        m_EyesightDistance = 50f; // vision distance lower
        navAgent.speed = m_Speed; // set speed of character
    }

    // POLYMORPHISM
    public override void AddDamage()
    {
        // easier to kill
        Health -=7; // reduce health by 7%
        base.AddDamage();
    }

    // POLYMORPHISM
    public override void AttackPlayer()
    {
        // attack the player
        m_Anim.SetBool("Attack", true);
        Debug.Log("Troll Attacking Player!");
        MakeAttackNoise();
    }

    // POLYMORPHISM
    public override void Patrol()
    {
        base.Patrol();
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
        m_Anim.SetBool("Attack", false);

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

    // POLYMORPHISM
    protected override void Start()
    {
        base.Start();
        m_Anim = GetComponent<Animator>();
    }
}

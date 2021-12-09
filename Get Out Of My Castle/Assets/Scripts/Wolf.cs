using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// INHERITANCE
public class Wolf : BaseEnemy
{
    public Wolf()
    {
        m_EnemyName = "Wolf";
    }

    // POLYMORPHISM
    public override void SetupEnemy()
    {
        base.SetupEnemy();

        m_Speed = 8f; // wolves are much faster
        m_DamageDealt = 6; // deals more damage than base enemy
        m_EyesightDistance = 70f; // vision distance higher
        navAgent.speed = m_Speed; // set speed of character
    }

    // POLYMORPHISM
    public override void AddDamage()
    {
        // easier to kill
        Health -= 6; // reduce health by 6%
        base.AddDamage();
    }

    // POLYMORPHISM
    public override void AttackPlayer()
    {
        // attack the player
        m_Anim.SetBool("Attack", true);
        Debug.Log("Wolf Attacking Player!");
    }

    // POLYMORPHISM
    protected override void StopAttacking()
    {
        m_Attacking = false;
        m_Anim.SetBool("Attack", false);
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
        base.MoveEnemy();
    }

    protected override void Start()
    {
        base.Start();
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
}

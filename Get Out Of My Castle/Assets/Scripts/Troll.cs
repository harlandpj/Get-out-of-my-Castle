using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// INHERITANCE
public class Troll : BaseEnemy
{
    [Header("Animal Noises")]
    [SerializeField]
    private AudioClip approachNoise;

    [SerializeField]
    private AudioClip trollAttackNoise;
    
    [SerializeField]
    private AudioClip trollDeath;

    public Troll()
    {
        m_EnemyName = "Troll";
        m_PatrolPoints = new GameObject[3];
    }
    
    // POLYMORPHISM
    public override void SetupEnemy()
    {
        base.SetupEnemy();
        
        m_Anim = GetComponent<Animator>();
        m_Speed = 4.5f; // trolls are faster
        m_DamageDealt = 15; // deals more damage than base enemy
        m_EyesightDistance = 40f; // vision distance lower
        navAgent.speed = m_Speed; // set speed of character
    }

    // POLYMORPHISM
    public override void SetToFirstPatrolPosition()
    {
        base.SetToFirstPatrolPosition();
    }

    // POLYMORPHISM
    public override void AddDamage()
    {
        Health -=34; // reduce health
        base.AddDamage();
    }

    // POLYMORPHISM
    public override void AttackPlayer()
    {
        if (!m_Attacking)
        {
            Debug.Log($"Now in Troll::AttackPlayer: m_Attacking = {m_Attacking}");

            m_Anim.SetBool("Attack", true);
            m_Anim.SetFloat("Speed", 0f);
            MakeAttackNoise(); // this also starts a repeating "attacking" coroutine
        }
    }

    // POLYMORPHISM: make a noise specific to the type of enemy
    protected override void MakeAttackNoise()
    {
        if (!m_Attacking)
        {
            m_Attacking = true;
            Debug.Log($"Now in Troll::MakeAttackNoise: m_Attacking = {m_Attacking}");

            // start attack sequence
            StartCoroutine(PlayingAttack());
        }
    }

    // POLYMORPHISM
    protected override IEnumerator PlayingAttack()
    {
        // give animator time to start attack again
        // if we have just come back in here as no loop option apparent
        yield return new WaitForSeconds(1f);

        // must have something here as can't override audioClips!
        audioSource = GetComponent<AudioSource>();

        while (m_Attacking)
        {
            audioSource.PlayOneShot(trollAttackNoise, 1f);
            m_Anim.SetBool("Attack", true);
            yield return new WaitForSeconds(trollAttackNoise.length);
            MainManager.Instance.AddDamageToPlayerHealth(-5);
            Debug.Log("Adding Damage of 5 to Player");
            m_Anim.SetBool("Attack", false);
            yield return new WaitForSeconds(2f); // give animator chance to start idle anim and stop too much damage adding
        }
    }

    // POLYMORPHISM
    public override void Patrol()
    {
        base.Patrol();
    }

    // POLYMORPHISM
    public override void MoveEnemy()
    {
        if (!m_Attacking)
        {
            m_Anim.SetFloat("Speed", 1.5f);
        }
        
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
        if (!MainManager.Instance.bGameOver)
        {
            // move the enemy
            MoveEnemy();
        }
        else
        {
            StopPlayingAttack();
            StopApproachNoise();
        }
    }

    protected override void PlayDeathSound()
    {
        // start a coroutine here to play a sound
        StartCoroutine("PlayDeath");
    }

    protected override void MakeApproachNoise()
    {
        if (!bMakeApproachNoise)
        {
            if (!bPlayingApproach)
            {
                bPlayingApproach = true;

                // approach noise
                audioSource.clip = approachNoise;
                audioSource.loop = true;
                audioSource.Play();
            }
        }
    }

    protected override void StopApproachNoise()
    {
        audioSource.loop = false;
        audioSource.Stop();
        bPlayingApproach = false;
    }

    bool DeathPlaying = false;

    protected override IEnumerator PlayDeath()
    {
        Debug.Log("Troll has Died");
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = 1f;
        audioSource.PlayOneShot(trollDeath, 1f);
        if (!DeathPlaying)
        {
            DeathPlaying = true;
            m_Anim.SetBool("Dead", true);
        }
        
        MainManager.Score += 75;

        yield return new WaitForSeconds(trollDeath.length + 0.5f);
        if (DeathPlaying)
        {
            m_Anim.SetBool("Death", false);
        }

        audioSource.volume = 0.35f;
        Destroy(gameObject);
    }
    
    // POLYMORPHISM
    protected override void Start()
    {
        base.Start();
        m_Anim = GetComponent<Animator>();
    }

    public override void SetupDynamicPatrolPoint(int arrayNum, GameObject obj)
    {
        base.SetupDynamicPatrolPoint(arrayNum, obj);
    }
}

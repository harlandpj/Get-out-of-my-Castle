using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// INHERITANCE
public class Wolf : BaseEnemy
{
    [Header("Animal Noises")]
    [SerializeField]
    private AudioClip approachNoise;

    [SerializeField]
    private AudioClip wolfAttackNoise;

    [SerializeField]
    private AudioClip wolfDeath;

    public Wolf()
    {
        m_EnemyName = "Wolf";
        m_PatrolPoints = new GameObject[3];
    }

    // POLYMORPHISM
    public override void SetupEnemy()
    {
        base.SetupEnemy();

        m_Speed = 6f; // wolves are much faster
        m_DamageDealt = 1; 
        m_EyesightDistance = 50f; // vision distance higher
        navAgent.speed = m_Speed; // set speed of character
    }
    
    public override void SetToFirstPatrolPosition()
    {
        base.SetToFirstPatrolPosition();
    }
    
    // POLYMORPHISM
    public override void AddDamage()
    {
        // easier to kill
        Health -= 50; // reduce health by 50%
        base.AddDamage();
    }

    // POLYMORPHISM
    public override void AttackPlayer()
    {
        if (!m_Attacking)
        {
            Debug.Log($"Now in Wolf::AttackPlayer: m_Attacking = {m_Attacking}");

            m_Anim.SetBool("Attack", true);
            m_Anim.SetFloat("Speed", 0f);
            MakeAttackNoise(); // this also starts a repeating "attacking" coroutine
        }
    }

    // maybe a case to change this to base class function and have a SetAudioClip() instead here
    // called to set audio clip as can't set clip in base class as it's abstract and needs to change
    // in all derived classes
    //
    //POLYMORPHISM
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

    protected override IEnumerator PlayingAttack()
    {
        // give animator time to start attack again
        // if we have just come back in here
        yield return new WaitForSeconds(1f);

        // here as can't set audioClips declared in abstract class
        audioSource = GetComponent<AudioSource>();

        while (m_Attacking)
        {
            m_Anim.SetBool("Attack", true);
            yield return new WaitForSeconds(wolfAttackNoise.length +2f);

            audioSource.PlayOneShot(wolfAttackNoise, 1f);
            MainManager.Instance.AddDamageToPlayerHealth(-5);
            Debug.Log("Adding Damage to Player");
            m_Anim.SetBool("Attack", false);

            yield return new WaitForSeconds(0.5f); // give animator chance to start idle anim
        }
    }
    
    // POLYMORPHISM: make a noise specific to the type of enemy
    protected override void MakeAttackNoise()
    {
        if (!m_Attacking)
        {
            m_Attacking = true;
            Debug.Log($"Now in Wolf::MakeAttackNoise: m_Attacking = {m_Attacking}");

            // start attack sequence
            StartCoroutine(PlayingAttack());
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
        // enemies should ALWAYS be moving
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

    protected override void Start()
    {
        base.Start();
        m_Anim = GetComponent<Animator>();

        // set up the dynamic patrol points if we don't have any
        if (m_PatrolPoints == null)
        {
            //SetupDynamicPatrolPoint();
            SetupEnemy();
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


    bool DeathPlaying = false;

    protected override IEnumerator PlayDeath()
    {
        Debug.Log("Wolf has Died");
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = 1f;
        audioSource.PlayOneShot(wolfDeath, 1f);
        if (!DeathPlaying)
        {
            DeathPlaying = true;
            m_Anim.SetBool("Death", true);
        }
        yield return new WaitForSeconds(wolfDeath.length + 0.5f);
        MainManager.Score += 50;

        if (DeathPlaying)
        {
            m_Anim.SetBool("Death", false);
        }

        audioSource.volume = 0.35f;
        Destroy(gameObject);
    }

    public override void SetupDynamicPatrolPoint(int arrayNum, GameObject obj)
    {
        base.SetupDynamicPatrolPoint(arrayNum, obj);
    }
}

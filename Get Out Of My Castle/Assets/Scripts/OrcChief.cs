using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// INHERITANCE
public class OrcChief : BaseEnemy
{
    [Header("Animal Noises")]
    [SerializeField]
    private AudioClip approachNoise;

    [SerializeField]
    private AudioClip chiefOrcAttackNoise;

    [SerializeField]
    private AudioClip orcDeath;

    public OrcChief()
    {
        m_EnemyName = "Orc Chief";
        m_PatrolPoints = new GameObject[3];
    }

    public override void SetToFirstPatrolPosition()
    {
        base.SetToFirstPatrolPosition();
    }
    
    // POLYMORPHISM
    public override void SetupEnemy()
    {
        base.SetupEnemy();
        m_Speed = 3.5f; // Orcs are slower
        m_DamageDealt = 10; // deals more damage than base enemy
        m_EyesightDistance = 30f; // default vision distance lower
        navAgent.speed = m_Speed; // set speed of character
        m_Anim = GetComponent<Animator>();
    }
    
    // POLYMORPHISM
    public override void AddDamage()
    {
        // Adds damage to enemy
        Health -= 20;
        base.AddDamage();
    }

    // POLYMORPHISM
    public override void AttackPlayer()
    {
        if (!m_Attacking)
        {
            Debug.Log($"Now in OrcChief::AttackPlayer: m_Attacking = {m_Attacking}");
            
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
            Debug.Log($"Now in OrcChief::MakeAttackNoise: m_Attacking = {m_Attacking}");

            StopApproachNoise();

            // start attack sequence
            StartCoroutine(PlayingAttack());
        }
    }

    protected override IEnumerator PlayingAttack()
    {
        // give animator time to start attack again
        // if we have just come back in here
        yield return new WaitForSeconds(1f);

        while (m_Attacking)
        {
            if (!bPlayingAttack)
            {
                bPlayingAttack = true;
                audioSource.clip = chiefOrcAttackNoise;
                audioSource.loop = true;
                audioSource.Play();
            }

            m_Anim.SetBool("Attack", true);
            yield return new WaitForSeconds(chiefOrcAttackNoise.length);
            MainManager.Instance.AddDamageToPlayerHealth(-10);
            Debug.Log("Adding Damage of 10 to Player");
            m_Anim.SetBool("Attack", false);
            
            
        
            yield return new WaitForSeconds(0.5f); // give animator chance to start idle anim
            if (!m_Attacking)
            {
                StopPlayingAttack();
            }
        }
    }

    protected override void StopPlayingAttack()
    {
        base.StopPlayingAttack();

        if (MainManager.Instance.bGameOver)
        {
            StopCoroutine("PlayingAttack");
        }
    }

    // POLYMORPHISM
    public override void Patrol()
    {
        base.Patrol(); // added - but may not be necessary in this example project
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

    // POLYMORPHISM
    protected override void Start()
    {
        base.Start();

        m_Anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        //// set up the dynamic patrol points if we don't have any
        //if (m_PatrolPoints == null)
        //{
        //    SetupDynamicPatrolPoints();
        //}

        SetupEnemy();
        Player = GameObject.FindGameObjectWithTag("Player");
        playerTransform = Player.GetComponent<Transform>();
    }

    protected override void MakeApproachNoise()
    {
        if (!bMakeApproachNoise)
        {
            bMakeApproachNoise = true;

            if (!bPlayingApproach)
            {
                bPlayingApproach = true;
         
                // approach noise
                audioSource.clip = approachNoise;
                audioSource.volume = 0.5f;
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
        // just for now do this!
        Debug.Log("Orc Chief has Died");
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = 1f;
        audioSource.PlayOneShot(orcDeath, 1f);

        MainManager.Score += 100;

        if (!DeathPlaying)
        {
            DeathPlaying = true;
            m_Anim.SetBool("Dead", true);
        }

        yield return new WaitForSeconds(orcDeath.length +1.5f);

        if (DeathPlaying)
        {
            m_Anim.SetBool("Dead", false);
        }

        audioSource.volume = 0.35f;
        Destroy(gameObject);
    }

    public override void SetupDynamicPatrolPoint(int arrayNum, GameObject obj)
    {
        base.SetupDynamicPatrolPoint(arrayNum,obj);
    }
}

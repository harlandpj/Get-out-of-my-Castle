using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrcChief : BaseEnemy
{
    public OrcChief()
    {
        m_EnemyName = "Orc Chief";
    }

    public override void AddDamage()
    {
        base.AddDamage();
    }

    public override void AttackPlayer()
    {
        // attack the player
        m_Anim.SetBool("Attack", true);
    }

    public override void Patrol()
    {
        base.Patrol();
    }
}

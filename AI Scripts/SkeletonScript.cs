using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonScript : EnemyAIScript
{
    public AudioSource audio;

    void Awake() //change our runspeeds 
    {
        walkSpeed = 500f;
        runSpeed = 700f;
    }

    void Update()
    {
        Animate();
    }

    new void Animate()
    {
        float xDir = rbody.velocity.x;
        if (xDir < 0 && !srender.flipX)
        {
            srender.flipX = true;
        }

        if (xDir > 0 && srender.flipX)
        {
            srender.flipX = false;
        }
    }

    //take damage
    public new void TakeDamage(int damage)
    {
        if (!isDead)
        {
            currentHealth -= damage;
            HealthBar.SetHealth(currentHealth);

            if (currentHealth <= 0)
            {
                isDead = true;
                animator.SetBool("isDead", true);
                Invoke("killSkeleton", 1.2f);
            }
        }
    }

    public void killSkeleton()
    {
        CoOpMSMScript.Instance.KillEnemy(gameObject);
    }
}

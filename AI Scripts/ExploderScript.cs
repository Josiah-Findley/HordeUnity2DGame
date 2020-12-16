using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExploderScript : EnemyAIScript
{
    public GameObject explosion;
    public int damage = 30;
    public bool hasntExploded = true; //so we dont instantiate a ton of explosions

    public AudioSource audio;
    void Awake() //change our runspeeds 
    {
        walkSpeed = 500f;
        runSpeed = 700f;
        maxHealth = 50;

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
                animator.SetBool("isDead", true);
                Invoke("killExploder", 1.1f);
                isDead = true;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player1" || collision.gameObject.tag == "Player2")
            killExploder();
    }

    //adapted from Stack Overflow
    //Does damage to Area around it
    void AreaDamage(Vector3 location, float radius)
    {
        Collider2D[] objectsInRange = Physics2D.OverlapCircleAll(location, radius);
        bool incrementGuardP1 = true;
        bool incrementGuardP2 = true;

        foreach (Collider2D col in objectsInRange)
        {
            // linear falloff of effect
            float proximity = (location - col.transform.position).magnitude;
            float effect = 1 - (proximity / radius);

            //damage (exploders will not do damage to other exploders btw)
            int totalDamage = (int)(damage * effect);

            EnemyAIScript enemy = col.gameObject.GetComponent<EnemyAIScript>();
            monsterScript monster = col.gameObject.GetComponent<monsterScript>();
            EnemyLaserAIScript laser = col.gameObject.GetComponent<EnemyLaserAIScript>();
            SkeletonScript skeleton = col.gameObject.GetComponent<SkeletonScript>();
            AOEScript aoe = col.gameObject.GetComponent<AOEScript>();
            Player1Script player1 = col.gameObject.GetComponent<Player1Script>();
            Player2Script player2 = col.gameObject.GetComponent<Player2Script>();

            if (enemy != null)
                enemy.TakeDamage(totalDamage);
            else if (laser != null)
                laser.TakeDamage(totalDamage);
            else if (monster != null)
                monster.TakeDamage(totalDamage);
            else if (skeleton != null)
                skeleton.TakeDamage(totalDamage);
            else if (aoe != null)
                aoe.TakeDamage(totalDamage);

            else if (player1 != null && incrementGuardP1)
            {
                incrementGuardP1 = false;
                player1.TakeDamage(totalDamage, false);
            }
            else if (player2 != null && incrementGuardP2)
            {
                incrementGuardP2 = false;
                player2.TakeDamage(totalDamage, false);
            }

            //Damage PopUp
            if (totalDamage > 100)
                PopUpScript.Create(col.transform.position, 100, "damage");
            else if (totalDamage > 0)
                PopUpScript.Create(col.transform.position, totalDamage, "damage");
        }
    }

    public void killExploder()
    {
        AreaDamage(rbody.transform.position, 4);
        //make explosion
        if(hasntExploded)
        {
            Instantiate(explosion, rbody.transform.position, Quaternion.identity);
            hasntExploded = false;
        }


        CoOpMSMScript.Instance.KillEnemy(gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOEScript : EnemyAIScript
{
    public AudioSource audio;

    private void Awake() //moves slower than other monsters
    {
        walkSpeed = 150f;
        runSpeed = 300f; 
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
                Invoke("killAOE", 1f);
                isDead = true;
            }
        }
    }

    //When Collides with bullet take damage
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Vector2 direction = (transform.position - collision.transform.position);

        if (collision.gameObject.CompareTag("ForceField"))
        {
            rbody.AddForce(direction.normalized * 20000 * Time.deltaTime);
            TakeDamage(1);
        }
    }

    public void killAOE()
    {
        CoOpMSMScript.Instance.KillEnemy(gameObject);
    }
}

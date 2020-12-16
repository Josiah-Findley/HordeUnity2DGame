using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLaserAIScript : EnemyAIScript
{
    //audio
    public AudioSource audio;
    public AudioClip regShot;

    public GameObject laserPrefab;
    public Transform firePoint;

    //have line of sight
    public LayerMask targetLayers;

    //delay
    float timer;
    float waitingTime = 0.5f;


    void Awake()
    {
        maxHealth = 200;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDead)
        {


            timer += Time.deltaTime;


                Transform shootAt = target;

                if (((target.position - transform.position).magnitude < 7 || (target2.position - transform.position).magnitude < 7))
                {


                    if ((target.position - transform.position).magnitude < (target2.position - transform.position).magnitude)
                    {
                        shootAt = target;

                    }

                    else
                    {
                        shootAt = target2;
                    }


                float rotateSpeed = 10 * Time.deltaTime;
                float angle = AngleBetweenTwoPoints(shootAt.position, firePoint.position);

                Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                firePoint.rotation = Quaternion.Slerp(firePoint.rotation, rotation, rotateSpeed);

                if (timer > waitingTime)
                {
                    ShootBullet();
                    timer = 0;
                }
            }


        }

        Animate();

    }



    //shoots laser
    public void ShootBullet()
    {
        audio.PlayOneShot(regShot);
        Instantiate(laserPrefab, firePoint.position, firePoint.rotation);
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


    //follow player
    protected float AngleBetweenTwoPoints(Vector2 a, Vector2 b)
    {
        return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
    }

    //take damage
    public new void TakeDamage(int damage)
    {
        currentHealth -= damage;
        HealthBar.SetHealth(currentHealth);

        if (currentHealth <= 0)
        {
            animator.SetBool("isDead", true);
            isDead = true;
            Invoke("killLaserEnemy", 1.5f);

        }
    }

    public void killLaserEnemy()
    {
        CoOpMSMScript.Instance.KillLaserEnemy(gameObject);
    }
}




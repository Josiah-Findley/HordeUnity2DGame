using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class OrangeyScript : MonoBehaviour
{
    //Animation
    public Animator animator;
        //RigidBody
    protected Rigidbody2D rbody;
    protected SpriteRenderer srender;
    //Health
    public int maxHealth = 75;
    public int currentHealth;
    public HealthBarScript HealthBar;
    //Speed
    public float speed = 550f;
    float followDistance = 7;
    public bool isDead = false;
    //Patrol Points
    int patrolIndex;

    //Pathfinding
    public float nextWayPointDistance = 2f;
    Path path;
    int currentWayPoint = 0;
    bool reachedEndOfPath = false;
    Seeker seeker;

    private void Start()
    {
        seeker = GetComponent<Seeker>();
        rbody = GetComponent<Rigidbody2D>();
        srender = GetComponent<SpriteRenderer>();

        //random path update time
        float updateTime = Random.Range(0.1f, 0.3f);

        //Update Path
        InvokeRepeating("UpdatePath", 0f, updateTime);

        patrolIndex = Random.Range(0, CoOpMSMScript.Instance.patrolScript.numPatrolPoints);

        currentHealth = maxHealth;
        HealthBar.SetMaxHealth(maxHealth);
    }

    void FixedUpdate()
    {
        //PathFinder Stuff
        if (path == null)
            return;
        if (currentWayPoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            return;
        }
        else
        {
            reachedEndOfPath = false;
        }

        Vector2 direction = ((Vector2)path.vectorPath[currentWayPoint] - rbody.position).normalized;
        Vector2 force = direction * speed * Time.deltaTime;
        rbody.AddForce(force);

        //Change WayPoint
        float distance = Vector2.Distance(rbody.position, path.vectorPath[currentWayPoint]);

        if (distance < nextWayPointDistance)
        {
            currentWayPoint++;
        }
        Animate();

    }

    void Animate()
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

    //Path Methods
    void UpdatePath()
    {
        if (((Vector2)transform.position - CoOpMSMScript.Instance.patrolScript.patrols[patrolIndex]).magnitude < 1)
        {
            patrolIndex = Random.Range(0, CoOpMSMScript.Instance.patrolScript.numPatrolPoints);
        }
        seeker.StartPath(rbody.position, CoOpMSMScript.Instance.patrolScript.patrols[patrolIndex], OnPathComplete);
    }

    //Path completed
    void OnPathComplete(Path p)
    {

        if (!p.error)
        {
            path = p;
            currentWayPoint = 0;
        }
    }

    //take damage
    public void TakeDamage(int damage)
    {
        if (!isDead)
        {
            currentHealth -= damage;
            HealthBar.SetHealth(currentHealth);

            if (currentHealth <= 0)
            {
                isDead = true;
                animator.SetBool("isDead", true);
                Invoke("killOrangey", 0.5f);
            }
        }
    }

    public void killOrangey()
    {
        CoOpMSMScript.Instance.KillOrangey(gameObject);
    }
}

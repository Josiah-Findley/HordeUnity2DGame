using UnityEngine;
using Pathfinding;

public class EnemyAIScript : MonoBehaviour
{
    //Animation
    public Animator animator;

    //Target to track
    public Transform target;
    public Transform target2;
    //Health
    public int maxHealth = 100;
    public int currentHealth;
    public HealthBarScript HealthBar;
    public bool isDead = false;

    //Patrol Points
    int patrolIndex;

    //Speed
    public float speed = 300f;
    protected float walkSpeed = 200f;
    protected float runSpeed = 400f; //this is modifiable for our other
    float followDistance = 7;


    //Pathfinding
    public float nextWayPointDistance = 2f;
    Path path;
    int currentWayPoint = 0;
    bool reachedEndOfPath = false;
    Seeker seeker;

    bool isCoOpMode;


    //RigidBody
    protected Rigidbody2D rbody;
    protected SpriteRenderer srender;

    void Start()
    {
        //which mode
        if (GameObject.Find("CoOpGameManager") !=null)
        {
            isCoOpMode = true;

        }

        //Find the Players
        target = GameObject.Find("Player").transform;
        target2 = GameObject.Find("Player2").transform;
        seeker = GetComponent<Seeker>();
        rbody = GetComponent<Rigidbody2D>();
        srender = GetComponent<SpriteRenderer>();

        //random path update time
        float updateTime = Random.Range(0.1f, 0.3f);

        //Update Path
        InvokeRepeating("UpdatePath", 0f, updateTime);

        //which mode
        if(isCoOpMode)
            patrolIndex = Random.Range(0, CoOpMSMScript.Instance.patrolScript.numPatrolPoints);
        else
            patrolIndex = Random.Range(0, MSMScript.Instance.patrolScript.numPatrolPoints);

        currentHealth = maxHealth;
        HealthBar.SetMaxHealth(maxHealth);

    }

    //Path Methods
    void UpdatePath() {

        bool shouldPathFind;

        if (isCoOpMode)
            shouldPathFind = seeker.IsDone();
        else
            shouldPathFind= seeker.IsDone() && (((target.position - transform.position).magnitude < followDistance && target.position.x > MSMScript.Instance.p1SpawnPointEntrance.position.x) || (target2.position - transform.position).magnitude < followDistance && target2.position.x < MSMScript.Instance.p2SpawnPointEntrance.position.x);

        //Creates path to Player when in range
        if (shouldPathFind)
        {
            speed = runSpeed;//faster speed
            //Follows Whatever Player is closer
            if ((target.position - transform.position).magnitude < (target2.position - transform.position).magnitude)
            {
                seeker.StartPath(rbody.position, target.position, OnPathComplete);
            }
            else {
                seeker.StartPath(rbody.position, target2.position, OnPathComplete);
            }
            
            
        }

        //else creates path to randomized patrol point
        else {

            if (!isCoOpMode)
            {
                if (((Vector2)transform.position - MSMScript.Instance.patrolScript.patrols[patrolIndex]).magnitude < 1)
                {
                    patrolIndex = Random.Range(0, MSMScript.Instance.patrolScript.numPatrolPoints);
                }
                speed = walkSpeed;//slower patrol speed
                seeker.StartPath(rbody.position, MSMScript.Instance.patrolScript.patrols[patrolIndex], OnPathComplete);
            }

            /*else {
                if (((Vector2)transform.position - CoOpMSMScript.Instance.patrolScript.patrols[patrolIndex]).magnitude < 1)
                {
                    patrolIndex = Random.Range(0, CoOpMSMScript.Instance.patrolScript.numPatrolPoints);
                }
                speed = walkSpeed;//slower patrol speed
                seeker.StartPath(rbody.position, CoOpMSMScript.Instance.patrolScript.patrols[patrolIndex], OnPathComplete);
            }*/
        }


    }

    //Path completed
    void OnPathComplete(Path p){

        if (!p.error) {
            path = p;
            currentWayPoint = 0;
        }
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


        //Following Player
        Vector2 direction = ((Vector2)path.vectorPath[currentWayPoint] - rbody.position).normalized;
        Vector2 force = direction * speed*Time.deltaTime;
        rbody.AddForce(force);

        

        //Change WayPoint
        float distance = Vector2.Distance(rbody.position, path.vectorPath[currentWayPoint]);

        if (distance < nextWayPointDistance) {
            currentWayPoint++;
        }
        Animate();

    }

    public void Animate() {

        //Set Animation
        Vector2 movement = rbody.velocity;
        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Magnitude", movement.magnitude);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Vector2 direction = (transform.position - collision.transform.position);

        if (collision.gameObject.CompareTag("Player1"))
        {

            Player1Script player = collision.gameObject.GetComponent<Player1Script>();
            PopUpScript.Create(collision.transform.position, 5, "damage");
            player.TakeDamage(5, false);
            rbody.AddForce(direction.normalized * 20000 * Time.deltaTime);

        }

        if (collision.gameObject.CompareTag("Player2") )
        {
            Player2Script player = collision.gameObject.GetComponent<Player2Script>();
            PopUpScript.Create(collision.transform.position, 5, "damage");
            player.TakeDamage(5, false);
            rbody.AddForce(direction.normalized*20000*Time.deltaTime);

        }

        if (collision.gameObject.CompareTag("ForceField"))
        {
            rbody.AddForce(direction.normalized * 20000 * Time.deltaTime);
            TakeDamage(1);
        }
    }


    public void TakeDamage(int damage)
    {
        if (!isDead)
        {
            currentHealth -= damage;
            HealthBar.SetHealth(currentHealth);

            if (!isCoOpMode)
            {
                if (currentHealth <= 0)
                {
                    MSMScript.Instance.KillEnemy(gameObject);
                    isDead = true;
                }
            }
            else
            {
                if (currentHealth <= 0)
                {
                    CoOpMSMScript.Instance.KillEnemy(gameObject);
                    isDead = true;
                }
            }
        }
    }

}
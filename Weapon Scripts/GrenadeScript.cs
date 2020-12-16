using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeScript : MonoBehaviour
{
    Rigidbody2D rbody;
    public GameObject explosion;




    public float speed = 15f;
    public int damage = 60;
    public int enemyDamage = 130;
    public int monsterDamage = 60;

    bool isCoOpMode;

    // Start is called before the first frame update
    void Start()
    {
        rbody = gameObject.GetComponent<Rigidbody2D>();
        rbody.velocity = transform.right * speed;

        //which mode
        if (GameObject.Find("CoOpGameManager") != null)
        {
            isCoOpMode = true;

        }


    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 0, -500 * Time.deltaTime); //rotates 50 degrees per second around z axis
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        AreaDamage(rbody.transform.position, 4);
        //make explosion
        Instantiate(explosion, rbody.transform.position, Quaternion.identity);
        gameObject.SetActive(false);
    }

    //adapted from Stack Overflow
    //Does damage to Area around it
    void AreaDamage(Vector3 location, float radius)
    {
        Collider2D[] objectsInRange = Physics2D.OverlapCircleAll(location, radius);
        bool incrementGuardP1 = true;
        bool incrementGuardP2 = true;
        int totalDamageCount =0;

        foreach (Collider2D col in objectsInRange)
        {
            //damage
            int totalDamage = 0;


            // linear falloff of effect
            float proximity = (location - col.transform.position).magnitude;
            float effect = 1 - (proximity / radius);

           EnemyAIScript enemy = col.gameObject.GetComponent<EnemyAIScript>();
            monsterScript monster = col.gameObject.GetComponent<monsterScript>();
            EnemyLaserAIScript laser = col.gameObject.GetComponent<EnemyLaserAIScript>();
            SkeletonScript skeleton = col.gameObject.GetComponent<SkeletonScript>();
            ExploderScript exploder = col.gameObject.GetComponent<ExploderScript>();
            AOEScript aoe = col.gameObject.GetComponent<AOEScript>();
            OrangeyScript orangey = col.gameObject.GetComponent<OrangeyScript>();
            Player1Script player1 = col.gameObject.GetComponent<Player1Script>();
            Player2Script player2 = col.gameObject.GetComponent<Player2Script>();


            if (totalDamageCount<2000) {

                if (enemy != null)
                {

                    totalDamage = (int)(enemyDamage * 1.5f * effect);
                    enemy.TakeDamage(totalDamage);
                }
                else if (laser != null)
                {
                    totalDamage = (int)(enemyDamage * 2 * effect);
                    laser.TakeDamage(totalDamage);
                }
                else if (monster != null)
                {

                    totalDamage = (int)(monsterDamage * effect);
                    monster.TakeDamage(totalDamage);
                }
                else if (skeleton != null)
                {
                    totalDamage = (int)(enemyDamage * 2 * effect);
                    skeleton.TakeDamage(totalDamage);
                }
                else if (exploder != null)
                {
                    totalDamage = (int)(enemyDamage * 2 * effect);
                    exploder.TakeDamage(totalDamage);
                }
                else if (aoe != null)
                {
                    totalDamage = (int)(enemyDamage * 2 * effect);
                    aoe.TakeDamage(totalDamage);
                }
                else if (orangey != null)
                {
                    totalDamage = (int)(enemyDamage * 2 * effect);
                    orangey.TakeDamage(totalDamage);
                }

                else if (player1 != null && incrementGuardP1)
                {
                    if (!isCoOpMode)
                    {
                        if (player1.transform.position.x > MSMScript.Instance.p1SpawnPointEntrance.position.x)
                        {
                            incrementGuardP1 = false;
                            totalDamage = (int)(damage * effect);
                            player1.TakeDamage(totalDamage, false);
                        }


                    }

                    else {
                        incrementGuardP1 = false;
                        totalDamage = (int)(damage * effect);
                        player1.TakeDamage(totalDamage, false);

                    }

                }


                else if (player2 != null && incrementGuardP2)
                {


                    if (!isCoOpMode)
                    {
                        if (player2.transform.position.x < MSMScript.Instance.p2SpawnPointEntrance.position.x)
                        {
                            incrementGuardP2 = false;
                            totalDamage = (int)(damage * effect);
                            player2.TakeDamage(totalDamage, false);
                        }


                    }
                    else
                    {
                        incrementGuardP2 = false;
                        totalDamage = (int)(damage * effect);
                        player2.TakeDamage(totalDamage, false);

                    }
                }

                //Damage PopUp
                if (totalDamage > 100)
                    PopUpScript.Create(col.transform.position, 100, "damage");
                else if (totalDamage > 0)
                    PopUpScript.Create(col.transform.position, totalDamage, "damage");

                totalDamageCount += totalDamage;
            }
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    Rigidbody2D rbody;

    public float speed = 20f;
    public int damage = 10;
    public int enemyDamage = 25;
    public int monsterDamage = 2;

    bool isCoOpMode;

    // Start is called before the first frame update
    void Start()
    {
        //which mode
        if (GameObject.Find("CoOpGameManager") != null)
        {
            isCoOpMode = true;

        }

        rbody = gameObject.GetComponent<Rigidbody2D>();
        rbody.velocity = transform.right * speed;

     
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.tag == "Player1")
        {
            if (!isCoOpMode) {
                PopUpScript.Create(collision.transform.position, damage, "damage");
                collision.gameObject.GetComponent<Player1Script>().TakeDamage(damage, true);
            }
        }
        else if (collision.gameObject.tag == "Player2")
        {
            if (!isCoOpMode)
            {
                PopUpScript.Create(collision.transform.position, damage, "damage");
                collision.gameObject.GetComponent<Player2Script>().TakeDamage(damage, true);
            }
        }
        else if (collision.gameObject.tag == "Enemy")
        {
            PopUpScript.Create(collision.transform.position, enemyDamage, "damage");
            collision.gameObject.GetComponent<EnemyAIScript>().TakeDamage(enemyDamage);
        }
        else if (collision.gameObject.tag == "LaserEnemy")
        {
            PopUpScript.Create(collision.transform.position, enemyDamage, "damage");
            collision.gameObject.GetComponent<EnemyLaserAIScript>().TakeDamage(enemyDamage);
        }
        else if (collision.gameObject.tag == "Skeleton")
        {
            PopUpScript.Create(collision.transform.position, enemyDamage, "damage");
            collision.gameObject.GetComponent<SkeletonScript>().TakeDamage(enemyDamage);
        }
        else if (collision.gameObject.tag == "Exploder")
        {
            PopUpScript.Create(collision.transform.position, enemyDamage, "damage");
            collision.gameObject.GetComponent<ExploderScript>().TakeDamage(enemyDamage);
        }
        else if (collision.gameObject.tag == "AOE")
        {
            PopUpScript.Create(collision.transform.position, enemyDamage, "damage");
            collision.gameObject.GetComponent<AOEScript>().TakeDamage(enemyDamage);
        }
        else if(collision.gameObject.tag == "Orangey")
        {
            PopUpScript.Create(collision.transform.position, enemyDamage, "damage");
            collision.gameObject.GetComponent<OrangeyScript>().TakeDamage(enemyDamage);
        }

        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
         if (collision.gameObject.tag == "Monster")
        {
            rbody.mass = 0;
            Invoke("resetMass",0.5f);
            PopUpScript.Create(collision.transform.position, monsterDamage, "damage");
            collision.gameObject.GetComponent<monsterScript>().TakeDamage(monsterDamage);
            gameObject.SetActive(false);
        }

    }

    void resetMass() {
        rbody.mass = 0.075f;
    }


}

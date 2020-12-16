using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class laserScript : MonoBehaviour
{
    Rigidbody2D rbody;

    public float speed = 20f;
    public int damage = 10;
    public int enemyDamage = 25;
    public int monsterDamage = 2;

    // Start is called before the first frame update
    void Start()
    {
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
            PopUpScript.Create(collision.transform.position, damage, "damage");
            collision.gameObject.GetComponent<Player1Script>().TakeDamage(damage, false);
        }
        else if (collision.gameObject.tag == "Player2")
        {
            PopUpScript.Create(collision.transform.position, damage, "damage");
            collision.gameObject.GetComponent<Player2Script>().TakeDamage(damage, false);
        }


        Destroy(gameObject);
    }

    //When Collides with bullet take damage
    private void OnTriggerEnter2D(Collider2D collision)
    {


        if (collision.gameObject.CompareTag("ForceField"))
        {
            Destroy(gameObject);
        }

    }

}
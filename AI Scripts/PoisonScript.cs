using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonScript : MonoBehaviour
{
    public int damagePerSecond = 5;
    private float timerP1;
    private float timerP2;

    private void Start()
    {
        timerP1 = Time.time;
        timerP2 = Time.time;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player1")
        {
            if (Time.time > timerP1)
            {
                PopUpScript.Create(collision.transform.position, damagePerSecond, "damage");
                collision.gameObject.GetComponent<Player1Script>().TakeDamage(damagePerSecond, false);
                timerP1 = Time.time + 1;
            }
        }

        if (collision.gameObject.tag == "Player2")
        {
            if (Time.time > timerP2)
            {
                PopUpScript.Create(collision.transform.position, damagePerSecond, "damage");
                collision.gameObject.GetComponent<Player2Script>().TakeDamage(damagePerSecond, false);
                timerP2 = Time.time + 1;
            }
        }
    }
}

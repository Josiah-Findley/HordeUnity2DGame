using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnScript : MonoBehaviour
{
    public string allowedPlayer; //the tag of player who has this spawn point 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //if anything but the correct player tries to come through, it will block them 
    private void OnCollisionEnter2D(Collision2D collision)
    {
       if(collision.gameObject.tag == allowedPlayer)
        {
            Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>());
        }
    }


}

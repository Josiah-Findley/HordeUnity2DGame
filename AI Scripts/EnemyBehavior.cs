using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    public Animator animator;
    Rigidbody2D rbody;
    public int cash;

    // Start is called before the first frame update
    void Start()
    {
        rbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame


    void Update()
    {

        int maxSpeed = 3;
        rbody.velocity = Vector2.zero;

        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0.0f)* maxSpeed;
        transform.position = transform.position + movement * Time.deltaTime;
        animator.SetFloat("Horizontal",  movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Magnitude", movement.magnitude);
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        //grab coin
        if (collision.gameObject.tag == "coin")
        {
            cash += 100;
            Destroy(collision.gameObject);
        }

    }



}

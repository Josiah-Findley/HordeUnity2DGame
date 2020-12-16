using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmScript : MonoBehaviour
{
    public Transform weapon; //the weapon (child of the arm object)
    public Transform crate; //child of the arm object (only enabled when crate is being held)

    bool facingRight = true; //is our arm sprite oriented right?
    int facingDirection = 4;  //1 = up, 2 = down, 3 = left, 4 = right    --used to make sure the arms are in the correct position when the player is idling 
    public bool isPlayerOne;
    public bool crateEnabled = false; //is this player holding a crate? - will be changed in the player scripts

    // Start is called before the first frame update
    void Start()
    {
        weapon = transform.GetChild(0);
        crate = transform.GetChild(1);
    }

    // Update is called once per frame
    void Update()
    {
        OrientArms();

        if (crateEnabled)
        {
            OrientCrate();
        }
        else
            crate.gameObject.SetActive(false);
    }

    void OrientArms()
    {
        float moveX;
        float moveY;

        if (isPlayerOne)
        {
            Vector2 look = new Vector2(Input.GetAxis("Player1_LookH"), Input.GetAxis("Player1_LookV"));
            Vector2 movement = new Vector2(Input.GetAxis("Player1_Horizontal"), Input.GetAxis("Player1_Vertical"));
            if (look.magnitude < 0.2f)
            {
                moveX = movement.x;
                moveY = movement.y;
            }
            else
            {
                moveX = look.x;
                moveY = look.y;
            }
        }
        else
        {
            Vector2 look = new Vector2(Input.GetAxis("Player2_LookH"), Input.GetAxis("Player2_LookV"));
            Vector2 movement = new Vector2(Input.GetAxis("Player2_Horizontal"), Input.GetAxis("Player2_Vertical"));
            if (look.magnitude < 0.2f)
            {
                moveX = movement.x;
                moveY = movement.y;
            }
            else
            {
                moveX = look.x;
                moveY = look.y;
            }
        }

        if (moveX == 0 && moveY == 0) //ARE WE IDLING - should only need to handle up and down here, as TurnLeftOrRight() should handle L/R idling
        {
            if (facingDirection == 1)
            {
                gameObject.GetComponent<SpriteRenderer>().enabled = false;
                weapon.GetComponent<SpriteRenderer>().sortingLayerName = "Default"; //making sure the arms and gun is in the correct layer/visibility

                weapon.transform.rotation = Quaternion.Euler(0, 0, 90);
            }

            if (facingDirection == 2)
            {
                gameObject.GetComponent<SpriteRenderer>().enabled = true;
                weapon.GetComponent<SpriteRenderer>().sortingLayerName = "Gun"; //making sure the arms and gun is in the correct layer/visibility

                weapon.transform.rotation = Quaternion.Euler(0, 0, -90);
            }

            if (facingDirection == 3)
            {
                gameObject.GetComponent<SpriteRenderer>().enabled = true;
                weapon.GetComponent<SpriteRenderer>().sortingLayerName = "Gun"; //making sure the arms and gun is in the correct layer/visibility

                weapon.transform.rotation = Quaternion.Euler(0, 180, 0);
            }

            if (facingDirection == 4)
            {
                gameObject.GetComponent<SpriteRenderer>().enabled = true;
                weapon.GetComponent<SpriteRenderer>().sortingLayerName = "Gun"; //making sure the arms and gun is in the correct layer/visibility

                weapon.transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }
        else //WE ARE NOT IDLING 
        {

            if (moveY > 0 && moveX == 0) //ARE WE MOVING UP
            {
                gameObject.GetComponent<SpriteRenderer>().enabled = false;
                weapon.GetComponent<SpriteRenderer>().sortingLayerName = "Default"; //making sure the arms and gun is in the correct layer/visibility

                facingDirection = 1;

                weapon.transform.rotation = Quaternion.Euler(0, 0, 90);

                
            }
            else if (moveY < 0 && moveX == 0) //ARE WE MOVING DOWN (moving down right or down left will make use of the left and right orientation)
            {
                gameObject.GetComponent<SpriteRenderer>().enabled = true;
                weapon.GetComponent<SpriteRenderer>().sortingLayerName = "Gun"; //making sure the arms and gun is in the correct layer/visibility

                facingDirection = 2;

                weapon.transform.rotation = Quaternion.Euler(0, 0, -90);
            }
            else  //ARE WE MOVING LEFT OR RIGHT
            {
                TurnLeftOrRight(moveX);
            }
        }
    }


    //handles movement orientation to the left and right 
    //Will also check to see if it needs reorienting before it turns left or right 
    void TurnLeftOrRight(float moveX)
    {
        //rotation checking
        if (facingDirection == 1)
        {
            gameObject.GetComponent<SpriteRenderer>().enabled = true;
            weapon.GetComponent<SpriteRenderer>().sortingLayerName = "Gun"; //making sure the arms and gun is in the correct layer/visibility
            weapon.transform.rotation = Quaternion.Euler(0, 0, 0); //if the weapon was previously facing up, turn it back to the right 
        }
        else if (facingDirection == 2)
        {
            weapon.transform.rotation = Quaternion.Euler(0, 0, 0); //if the weapon was previously facing down, turn it back to the right 
        }

        //MOVING LEFT
        if (moveX < 0)
        {
            facingDirection = 3;
            weapon.transform.rotation = Quaternion.Euler(0, 180, 0); //turn the weapon left
            if (facingRight)
                FlipX();
        }

        //MOVING RIGHT
        if (moveX > 0)
        {
            facingDirection = 4;
            weapon.transform.rotation = Quaternion.Euler(0, 0, 0); //turn the weapon right
            if (!facingRight)
                FlipX();
        }
    }

    void FlipX()
    {
        facingRight = !facingRight;
        transform.Rotate(Vector3.up * 180);
    }

    void OrientCrate()
    {
        if (facingDirection == 1)
            crate.gameObject.SetActive(false);
        else
            crate.gameObject.SetActive(true);
    }
}


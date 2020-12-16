using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player1Script : MonoBehaviour
{
    Rigidbody2D rbody;

    public float speed = 5f;
    public Animator animator;
    public GameObject gun; //gun in hand
    public GameObject playerArm; //the players arm, (used for changing out items in hand
    Vector2 idleMovement;//idle vector
    public int currentGun;

    public PlayerStats playerStats; //players stats (gold, guns, health)

    //ForceField Variables
    public GameObject forceField;
    public GameObject forceFieldUI;
    private int timeBtwnField = 8; //timer to stop us from using forcefield more than once at a time
    private int forceFieldTimeActive = 3; //how long will the field be active
    private float fieldInactive = 0; //timer for having the field inactive 
    private float fieldActive = 0; //timer for having the field active
    

    // boolean for carrying the crate
    public bool isCarryingCrate = false;
    public bool droppedCrate = false; //used so we dont pick up the crate as soon as we drop it 
    float timeSinceCrateDropped; //how long since we dropped the crate (so we dont immediately pick it back up)
    public bool atShop = false;

    bool isCoOpMode;

    private bool p1_dropcrate = false;

    public AudioClip forceFieldEffect;

    // Start is called before the first frame update
    void Start()
    {
        //which mode
        if (GameObject.Find("CoOpGameManager") != null)
        {
            isCoOpMode = true;

        }

        rbody = gameObject.GetComponent<Rigidbody2D>();
        Application.targetFrameRate = 60;
        idleMovement = new Vector2();//idle vector
        currentGun = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //PlayerMovement
         float x = speed * Input.GetAxis("Player1_Horizontal"); //maximum speed * an axis somewhere between +1 and -1. horizontal
         float y = speed * Input.GetAxis("Player1_Vertical"); //maximum speed * an axis somewhere between +1 and -1. vertical
         Vector2 movement = new Vector2(x, y);
        

        rbody.velocity = movement;

        Vector2 look = new Vector2(Input.GetAxis("Player1_LookH"), Input.GetAxis("Player1_LookV"));
        AnimatePlayer(movement, look);

        ForceField();

        //switch gun
        if (Input.GetButtonDown("P1_SwapWeapon") && !atShop)
        {
            switchGun();
        }

        //Consume Health potion
        if (Input.GetButtonDown("P1_ConsumePotion"))
        {
            playerStats.TryConsumeHealthPotion();
            playerStats.HealthBar.SetHealth(playerStats.currentHealth);
        }

        //if you try to shoot while holding the crate, you drop the crate
        //allows you to drop the crate if need be
        if ((Input.GetButtonDown("P1_DropCrate") || (Input.GetAxisRaw("P1_DropCrate") > 0 && !p1_dropcrate)) && isCarryingCrate)
        {
            p1_dropcrate = true;
            isCarryingCrate = false;
            MSMScript.Instance.UpdateQueue("CRATE DROPPED!");
            speed = 5f; //back to normal speed
            gun.SetActive(true); //you can now shoot
            playerArm.GetComponent<ArmScript>().crateEnabled = false; //crate gone
            MSMScript.Instance.SpawnCrate(gameObject.transform.position);
            droppedCrate = true;
            timeSinceCrateDropped = Time.time;
        }
        if (Input.GetAxisRaw("P1_DropCrate") <= 0)
        {
            p1_dropcrate = false;
        }
        //lets us pick the crate back up again if we purposefully dropped it 
        if (Time.time - timeSinceCrateDropped > 1)
            droppedCrate = false;
    }

    //Switching between weapons in inventory 
    public void switchGun()
    {
            currentGun = (currentGun + 1) % 5;

            switch (currentGun)
            {
                case 0: playerStats.weaponslot.GetComponent<SpriteRenderer>().sprite = Item.GetSprite(Item.ItemType.gun); break;
                case 1: if (playerStats.itemOwned(Item.ItemType.gun1)) { playerStats.weaponslot.GetComponent<SpriteRenderer>().sprite = Item.GetSprite(Item.ItemType.gun1); } else { switchGun(); } break;
                case 2: if (playerStats.itemOwned(Item.ItemType.gun2)) { playerStats.weaponslot.GetComponent<SpriteRenderer>().sprite = Item.GetSprite(Item.ItemType.gun2); } else { switchGun(); } break;
                case 3: if (playerStats.itemOwned(Item.ItemType.gun3)) { playerStats.weaponslot.GetComponent<SpriteRenderer>().sprite = Item.GetSprite(Item.ItemType.gun3); } else { switchGun(); } break;
                case 4: if (playerStats.itemOwned(Item.ItemType.gun4)) { playerStats.weaponslot.GetComponent<SpriteRenderer>().sprite = Item.GetSprite(Item.ItemType.gun4); } else { switchGun(); } break;
        }
    }

    //Do a specific amount of damage to player, handles death as well
    public void TakeDamage(int damage, bool wasBullet)
    {
        //take damage
        playerStats.currentHealth -= damage;
        playerStats.HealthBar.SetHealth(playerStats.currentHealth);

        if (playerStats.currentHealth <= 0)
        {
            //are we carrying the crate?
            if(isCarryingCrate)
            {
                //drop the crate at death point from a method in MSM
                isCarryingCrate = false;
                MSMScript.Instance.SpawnCrate(gameObject.transform.position);
                MSMScript.Instance.UpdateQueue("CRATE DROPPED");
                speed = 5f;
                gun.SetActive(true); //you can now shoot
                playerArm.GetComponent<ArmScript>().crateEnabled = false; //crate gone
            }
            if (!isCoOpMode)
            {
                MSMScript.Instance.KillPlayer(gameObject, wasBullet); //kill/respawn player
            }
            else
            {
                CoOpMSMScript.Instance.KillPlayer(gameObject, wasBullet); //kill/respawn player
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //at Shop
        if (collision.gameObject.tag == "Shop")
        {
            atShop = true;
        }

        //grab coin
        if (collision.gameObject.tag == "coin") 
        {
            if (!isCoOpMode)
            {
                collision.gameObject.SetActive(false);
                playerStats.AddGoldAmount(MSMScript.Instance.ENEMY_SCORE);
                PopUpScript.Create(collision.transform.position, MSMScript.Instance.ENEMY_SCORE, "coin");
                PopUpScript.Create(collision.transform.position, MSMScript.Instance.ENEMY_SCORE, "score");
                MSMScript.Instance.AddOrSubPoints(1, "Enemy");
            }
            else {
                collision.gameObject.SetActive(false);
                playerStats.AddGoldAmount(CoOpMSMScript.Instance.ENEMY_SCORE);
                PopUpScript.Create(collision.transform.position, CoOpMSMScript.Instance.ENEMY_SCORE, "coin");
                PopUpScript.Create(collision.transform.position, CoOpMSMScript.Instance.ENEMY_SCORE, "score");
                CoOpMSMScript.Instance.AddOrSubPoints(1, "Enemy");
            }
        }

        //grab purpleCoin
        if (collision.gameObject.tag == "purpleCoin")
        {
            if (!isCoOpMode)
            {
                collision.gameObject.SetActive(false);
                playerStats.AddGoldAmount(MSMScript.Instance.LASER_ENEMY_SCORE);
                PopUpScript.Create(collision.transform.position, MSMScript.Instance.LASER_ENEMY_SCORE, "coin");
                PopUpScript.Create(collision.transform.position, MSMScript.Instance.LASER_ENEMY_SCORE, "score");
                MSMScript.Instance.AddOrSubPoints(1, "LaserEnemy");
            }
            else
            {
                collision.gameObject.SetActive(false);
                playerStats.AddGoldAmount(CoOpMSMScript.Instance.LASER_ENEMY_SCORE);
                PopUpScript.Create(collision.transform.position, CoOpMSMScript.Instance.LASER_ENEMY_SCORE, "coin");
                PopUpScript.Create(collision.transform.position, CoOpMSMScript.Instance.LASER_ENEMY_SCORE, "score");
                CoOpMSMScript.Instance.AddOrSubPoints(1, "LaserEnemy");
            }
        }

        //grab chest from HVT
        if (collision.gameObject.tag == "Chest")
        {
            if (!isCoOpMode)
            {
                Destroy(collision.gameObject);
                playerStats.AddGoldAmount(MSMScript.Instance.HVT_SCORE); //10 coins 
                PopUpScript.Create(collision.transform.position, MSMScript.Instance.HVT_SCORE, "coin");
                PopUpScript.Create(collision.transform.position, MSMScript.Instance.HVT_SCORE, "score");
                MSMScript.Instance.AddOrSubPoints(1, "HVT");
            }
            else {
                Destroy(collision.gameObject);
                playerStats.AddGoldAmount(CoOpMSMScript.Instance.HVT_SCORE); //10 coins 
                PopUpScript.Create(collision.transform.position, CoOpMSMScript.Instance.HVT_SCORE, "coin");
                PopUpScript.Create(collision.transform.position, CoOpMSMScript.Instance.HVT_SCORE, "score");
                CoOpMSMScript.Instance.AddOrSubPoints(1, "HVT");

            }
        }

        //pick up health orange
        if (collision.gameObject.tag == "HealthOrange")
        {
            Destroy(collision.gameObject);
            PopUpScript.Create(collision.transform.position, CoOpMSMScript.Instance.ENEMY_SCORE, "score");
            CoOpMSMScript.Instance.AddLife("p1");
        }

        //grab crate
        if (collision.gameObject.tag == "Crate" && !droppedCrate)
        {
            isCarryingCrate = true;
            Destroy(collision.gameObject);
            MSMScript.Instance.UpdateQueue("CRATE STOLEN!");
            speed = 3f; //the crate is heavy! You move slow 
            gun.SetActive(false); //no shooting while holding the crate!
            playerArm.GetComponent<ArmScript>().crateEnabled = true; // show crate in arms
        }

        //cash crate
        if(collision.gameObject.tag == "CrateTrigger" && isCarryingCrate)
        {
            isCarryingCrate = false;
            playerStats.AddGoldAmount(MSMScript.Instance.CRATE_SCORE); //15 coins
            MSMScript.Instance.AddOrSubPoints(1, "Crate");
            MSMScript.Instance.UpdateQueue("P1 CACHED IN!");
            PopUpScript.Create(this.transform.position, MSMScript.Instance.CRATE_SCORE, "coin");
            PopUpScript.Create(this.transform.position, MSMScript.Instance.CRATE_SCORE, "score");
            speed = 5f;
            gun.SetActive(true); //you can now shoot
            playerArm.GetComponent<ArmScript>().crateEnabled = false; //crate gone
        }
    }

    public void OnTriggerExit2D(Collider2D collision) {
        //at Shop
        if (collision.gameObject.tag == "Shop")
        {
            atShop = false;
        }
    }

    //Gets our player to face properly and animate 
    void AnimatePlayer(Vector2 movement, Vector2 look)
    {
        Vector2 direction = look;
        if (look.magnitude < 0.2f)
        {
            direction = movement;
        }

        //Sets Player Animation
        animator.SetFloat("Horizontal", direction.x);
        animator.SetFloat("Vertical", direction.y);
        animator.SetFloat("Magnitude", movement.magnitude);

        //Stores velocity right before zero
        if (movement.magnitude > 0.2f)
        {
            idleMovement = movement;
        }
        //Sets Idle Animation
        animator.SetFloat("LastMoveX", idleMovement.x);
        animator.SetFloat("LastMoveY", idleMovement.y);
    }

    //ForceField Functionality 
    void ForceField()
    {
        if (fieldInactive <= 0)
        {
            forceFieldUI.SetActive(true);

            if (Input.GetButtonDown("P1_ForceField"))
            {
                //play our sound
                if (!isCoOpMode)
                    MSMScript.Instance.audio.PlayOneShot(forceFieldEffect);
                else
                    CoOpMSMScript.Instance.audio.PlayOneShot(forceFieldEffect);

                //make our sprite appear
                forceField.SetActive(true);
                forceFieldUI.SetActive(false);

                //reset timers
                fieldInactive = timeBtwnField;
                fieldActive = forceFieldTimeActive;
            }
        }
        else {
            fieldInactive -= Time.deltaTime;
        }


        if (fieldActive <= 0) {
            forceField.SetActive(false); //deactivate field 
        }

        else
            fieldActive -= Time.deltaTime;
    }

    public IEnumerator Knockback(float duration, float power, Vector3 direction)
    {
        float timer = 0; //time since we first called this function

        while(duration > timer)
        {
            timer += Time.deltaTime;
            rbody.AddForce(new Vector2(direction.x * power, direction.y * power));
        }

        yield return 0;
    }

}

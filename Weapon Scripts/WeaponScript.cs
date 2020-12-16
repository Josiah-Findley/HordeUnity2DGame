using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponScript : MonoBehaviour
{
    public Transform firePoint;
    public bool isPlayer1;
    public GameObject bulletPrefab;
    public GameObject RapidFirebulletPrefab;
    public GameObject doubleDamageBulletPrefab;
    public GameObject bigBoyBulletPrefab;
    public GameObject GrenadePrefab;
    private float rapidControlP1 = 0; //controls our rapidfire speed
    private float rapidControlP2 = 0; //controls our rapidfire speed

    //ObjectPooling
    public ObjectPool bulletPool;
    public ObjectPool RapidFirebulletPool;
    public ObjectPool doubleDamagePool;
    public ObjectPool bigBoyBulletPool;
    public ObjectPool GrenadePool;


    public AudioSource audio;
    public AudioClip regShot;
    public AudioClip bigShot;

    public int numBulletsRapid = 0;

    //Grenade Delay
    float timer;
    float waitingTime = 0.5f;

    private bool p1_shoot = false;
    private bool p2_shoot = false;

    private void Start()
    {
        bulletPool = new ObjectPool(bulletPrefab, true, 20);
        RapidFirebulletPool = new ObjectPool(RapidFirebulletPrefab, true, 20);
        doubleDamagePool = new ObjectPool(doubleDamageBulletPrefab, true, 20);
        bigBoyBulletPool = new ObjectPool(bigBoyBulletPrefab, true, 20);
        GrenadePool = new ObjectPool(GrenadePrefab, true, 20);


    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        //Shooting rapidFire
        if ((Input.GetButton("P1_Shoot") || Input.GetAxisRaw("P1_Shoot") > 0) && isPlayer1 && GetComponent<SpriteRenderer>().sprite == Item.GetSprite(Item.ItemType.gun2)) //for rapid fire functionality P1
        {
            p2_shoot = true;
            if(Time.time > rapidControlP1 + 0.1)
            {
                audio.PlayOneShot(regShot);
                //ObjPulling
                GameObject bullet = RapidFirebulletPool.GetObject();
                if (!bullet)
                    return;
                bullet.transform.rotation = firePoint.rotation;
                bullet.transform.position = firePoint.position;
                bullet.GetComponent<Rigidbody2D>().velocity = transform.right * bullet.GetComponent<BulletScript>().speed;
                // Instantiate(RapidFirebulletPrefab, firePoint.position, firePoint.rotation);
                rapidControlP1 = Time.time;
            }
        }
        else if ((Input.GetButton("P2_Shoot") || Input.GetAxisRaw("P2_Shoot") > 0) && !isPlayer1 && GetComponent<SpriteRenderer>().sprite == Item.GetSprite(Item.ItemType.gun2)) //for rapid fire functionality P2
        {
            p2_shoot = true;
            if (Time.time > rapidControlP2 + 0.1)
            {
                audio.PlayOneShot(regShot);
                //ObjPulling
                GameObject bullet = RapidFirebulletPool.GetObject();
                if (!bullet)
                    return;
                bullet.transform.rotation = firePoint.rotation;
                bullet.transform.position = firePoint.position;
                bullet.GetComponent<Rigidbody2D>().velocity = transform.right * bullet.GetComponent<BulletScript>().speed;
                // Instantiate(RapidFirebulletPrefab, firePoint.position, firePoint.rotation);
                rapidControlP2 = Time.time;
            }
        }
        //NormalShooting
        else if ((Input.GetButtonDown("P1_Shoot") || (Input.GetAxisRaw("P1_Shoot") > 0 && !p1_shoot)) && isPlayer1)
        {
            p1_shoot = true;
            Shoot();
        }
        else if ((Input.GetButtonDown("P2_Shoot") || (Input.GetAxisRaw("P2_Shoot") > 0 && !p2_shoot)) && !isPlayer1)
        {
            p2_shoot = true;
            Shoot();
        }
        
        if(Input.GetAxisRaw("P1_Shoot") <= 0)
        {
            p1_shoot = false;
        }
        if (Input.GetAxisRaw("P2_Shoot") <= 0)
        {
            p2_shoot = false;
        }
    }

    
    void Shoot()
    {
        if (GetComponent<SpriteRenderer>().sprite == Item.GetSprite(Item.ItemType.gun))
        {
            audio.PlayOneShot(regShot);
            //ObjPulling
            GameObject bullet = bulletPool.GetObject();
            if (!bullet)
                return;
            bullet.transform.rotation = firePoint.rotation;
            bullet.transform.position = firePoint.position;
            bullet.GetComponent<Rigidbody2D>().velocity = transform.right * bullet.GetComponent<BulletScript>().speed;
            // Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        }
        else if (GetComponent<SpriteRenderer>().sprite == Item.GetSprite(Item.ItemType.gun1)) 
        {
            audio.PlayOneShot(bigShot);
            //ObjPulling
            GameObject bullet = bigBoyBulletPool.GetObject();
            if (!bullet)
                return;
            bullet.transform.rotation = firePoint.rotation;
            bullet.transform.position = firePoint.position;
            bullet.GetComponent<Rigidbody2D>().velocity = transform.right * bullet.GetComponent<BulletScript>().speed;

            //Instantiate(bigBoyBulletPrefab, firePoint.position, firePoint.rotation);
        }
        else if (GetComponent<SpriteRenderer>().sprite == Item.GetSprite(Item.ItemType.gun3))
        {
            audio.PlayOneShot(regShot);
            //ObjPulling
            GameObject bullet = doubleDamagePool.GetObject();
            if (!bullet)
                return;
            bullet.transform.rotation = firePoint.rotation;
            bullet.transform.position = firePoint.position;
            bullet.GetComponent<Rigidbody2D>().velocity = transform.right * bullet.GetComponent<BulletScript>().speed;

            //Instantiate(doubleDamageBulletPrefab, firePoint.position, firePoint.rotation);
        }
        else if (GetComponent<SpriteRenderer>().sprite == Item.GetSprite(Item.ItemType.gun4))
        {
            if (timer > waitingTime)
            {
                audio.PlayOneShot(regShot);
                //ObjPulling
                GameObject bullet = GrenadePool.GetObject();
                if (!bullet)
                    return;
                //bullet reset
                bullet.transform.rotation = firePoint.rotation;
                bullet.transform.position = firePoint.position;
                bullet.GetComponent<Rigidbody2D>().velocity = transform.right * bullet.GetComponent<GrenadeScript>().speed;

                //reset Delay
                timer = 0;
            }

            //Instantiate(GrenadePrefab, firePoint.position, firePoint.rotation);
        }
    }

    public void RapidFire()
    {
        //ObjPulling
        GameObject bullet = RapidFirebulletPool.GetObject();
        if (!bullet)
            return;
        bullet.transform.rotation = firePoint.rotation;
        bullet.transform.position = firePoint.position;
        bullet.GetComponent<Rigidbody2D>().velocity = transform.right * bullet.GetComponent<BulletScript>().speed;

        //Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }

    /*public void rapidFire() {
        numBulletsRapid++;
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        if (numBulletsRapid < 3)
            Invoke("rapidFire", 0.3f);
        else
            numBulletsRapid = 0;
    }*/
}

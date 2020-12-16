using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionScript : MonoBehaviour
{

    //AudioFiles
    public AudioSource audio; //our game audiosource
    public AudioClip explosionSound;

    // Start is called before the first frame update
    void Start()
    {
        audio.PlayOneShot(explosionSound);
        Invoke("DestroySelf", 0.68f);
    }

    // Update is called once per frame
    void DestroySelf()
    {
        Destroy(gameObject);
    }
}

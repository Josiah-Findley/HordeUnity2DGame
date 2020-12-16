/* 
    Code adapted from CodeMonkey
 */
 
using UnityEngine;
using System.Reflection;
using System.Runtime.InteropServices;
using TMPro;

public class GameAssets : MonoBehaviour {

    private static GameAssets _i = null;

    public static GameAssets ij
    {
        get
        {
            return _i;
        }
    }

    private void Awake()
    {
        if (_i != null && _i != this)
        {
            Destroy(this.gameObject);
            return;

        }
        _i = this;
    }

    public Sprite gun;
    public Sprite gun1;
    public Sprite gun2;
    public Sprite gun3;
    public Sprite gun4;
    public Transform damagePopUp;
    public Transform coinPopUp;
    public Transform scorePopUp;
    // public Sprite sheild;
    public Sprite HealthPotion;
    //public Sprite sword1;
    //public Sprite sword2;



}

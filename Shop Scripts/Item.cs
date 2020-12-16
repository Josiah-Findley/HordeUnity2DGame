/* 
   Code Adapted from Code Monkey
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{

    public enum ItemType {
        gun,
        gun1,
        gun2,
        gun3,
        gun4,
        HealthPotion,
    //    Sword_1,
    //    Sword_2
    }
    public Sprite gun;
    public Sprite gun1;
    public Sprite gun2;
    public Sprite gun3;
    public Sprite gun4;
    public Sprite sheild;
    public Sprite HealthPotion;
    //  public Sprite sword1;
    //  public Sprite sword2;


    //cost of item
    public static int GetCost(ItemType itemType)
    {
        bool isCoOpMode;

        //which mode
        if (GameObject.Find("CoOpGameManager") != null)
        {
            isCoOpMode = true;

        }
        else {
            isCoOpMode = false;
        }

        if (!isCoOpMode)
            switch (itemType)
            {
                default:
                case ItemType.gun1: return 15;
                case ItemType.gun2: return 25;
                case ItemType.gun3: return 35;
                case ItemType.gun4: return 40;
                case ItemType.HealthPotion: return 5;
                    //  case ItemType.Sword_1: return 75;
                    //  case ItemType.Sword_2: return 150;
            }
        else
            switch (itemType)
            {
                default:
                case ItemType.gun1: return 50;
                case ItemType.gun2: return 100;
                case ItemType.gun3: return 150;
                case ItemType.gun4: return 200;
                case ItemType.HealthPotion: return 15;
                    //  case ItemType.Sword_1: return 75;
                    //  case ItemType.Sword_2: return 150;
            }

    }

    //sprite of item
    public static Sprite GetSprite(ItemType itemType) {
        switch (itemType) {
        default:
        case ItemType.gun:     return GameAssets.ij.gun;
        case ItemType.gun1:      return GameAssets.ij.gun1;
        case ItemType.gun2:      return GameAssets.ij.gun2;
        case ItemType.gun3:         return GameAssets.ij.gun3;
        case ItemType.gun4:          return GameAssets.ij.gun4;
        case ItemType.HealthPotion: return GameAssets.ij.HealthPotion;
       // case ItemType.Sword_1:      return GameAssets.ij.sword1;
       // case ItemType.Sword_2:      return GameAssets.ij.sword2;
        }
    }

}

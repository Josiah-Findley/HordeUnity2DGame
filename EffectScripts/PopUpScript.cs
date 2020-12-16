/* 
    Code adapted from CodeMonkey
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PopUpScript : MonoBehaviour
{

    public TextMeshPro textMesh;
    private const float DISAPPEAR_TIMER_MAX = 0.4f;
    private float disappearTimer;
    private Color textColor;
    private Vector3 moveVector;
    private static int sortingOrder;


    /************************Creates PopUP*****************************/
    public static PopUpScript Create(Vector3 position, int amount, string type)
    {
        Transform popType;

        //Object Based on type
        switch (type)
        {
            default:
            case "damage": popType = GameAssets.ij.damagePopUp; break;
            case "coin": popType = GameAssets.ij.coinPopUp; break;
            case "score": popType = GameAssets.ij.scorePopUp; break;
            case "negScore": popType = GameAssets.ij.scorePopUp; break;
        }

        Transform popUpTransform = Instantiate(popType, position, Quaternion.identity);

        PopUpScript PopUp = popUpTransform.GetComponent<PopUpScript>();
        PopUp.PopUpInit(amount, type);
        return PopUp;
    }

    //Grabs text on Awake
    private void Awake()
    {
        textMesh = transform.GetComponent<TextMeshPro>();
    }

    /************************Initializes PopUp*****************************/
    private void PopUpInit(int amount, string type)
    {

        //Color Based on type
        switch (type)
        {
            default:
            case "damage": textColor = Color.red; textMesh.SetText("-"+amount.ToString()); break;
            case "coin": textColor = Color.yellow; textMesh.SetText(amount.ToString()); break;
            case "score": textColor = Color.white; textMesh.SetText(amount.ToString()); break;
            case "negScore": textColor = Color.red; textMesh.SetText(amount.ToString()); break;
        }

        textMesh.color = textColor;
        textMesh.fontSize = 7;
        disappearTimer = DISAPPEAR_TIMER_MAX;


        if (type.Equals("negScore"))
            transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().color = Color.red;


        //move over score popUp
        if (type.Equals("score"))
            moveVector = new Vector3(4.7f, 1) * 3f;
        else
            moveVector = new Vector3(.7f, 1) * 3f;

        //keeps track of drawing order
        sortingOrder++;
        textMesh.sortingOrder = sortingOrder;
    }


    /************************Animates and then Destroys PopUp*****************************/
    private void Update()
    {
        transform.position += moveVector * Time.deltaTime;
        moveVector -= moveVector * 8f * Time.deltaTime;

        if (disappearTimer > DISAPPEAR_TIMER_MAX * .5f)
        {
            // First half of the popup lifetime
            float increaseScaleAmount = 1f;
            transform.localScale += Vector3.one * increaseScaleAmount * Time.deltaTime;
        }
        else
        {
            // Second half of the popup lifetime
            float decreaseScaleAmount = 1;
            transform.localScale -= Vector3.one * decreaseScaleAmount * Time.deltaTime;
        }

        disappearTimer -= Time.deltaTime;
        if (disappearTimer < 0)
        {
            // Start disappearing
            float disappearSpeed = 3f;
            textColor.a -= disappearSpeed * Time.deltaTime;
            textMesh.color = textColor;
            if (textColor.a < 0)
            {
                Destroy(gameObject);
            }
        }
    }

}







   
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/* 
    Code adapted from CodeMonkey
 */


public class UI_Player : MonoBehaviour {

    public TextMeshProUGUI goldText;
    public TextMeshProUGUI healthPotionText;

    public PlayerStats player;

    public bool isPlayer2;

    bool isCoOpMode;


    private void Start() {
        //which mode
        if (GameObject.Find("CoOpGameManager") != null)
        {
            isCoOpMode = true;

        }
        else {
            isCoOpMode = false;
        }

        UpdateText();

        player.OnGoldAmountChanged += Instance_OnGoldAmountChanged;
        player.OnHealthPotionAmountChanged += Instance_OnHealthPotionAmountChanged;
    }

    private void Instance_OnHealthPotionAmountChanged(object sender, System.EventArgs e) {
        UpdateText();
    }

    private void Instance_OnGoldAmountChanged(object sender, System.EventArgs e) {
        UpdateText();
    }

    private void UpdateText() {
    
        
        if (isCoOpMode && isPlayer2)
        {
            if (player.GetGoldAmount() > 9 && player.GetGoldAmount() < 100)
                goldText.text = " " + player.GetGoldAmount().ToString();
            else if (player.GetGoldAmount() < 9)
                goldText.text = "  " + player.GetGoldAmount().ToString();
            else
                goldText.text = player.GetGoldAmount().ToString();


            if (player.GetHealthPotionAmount() > 9 && player.GetHealthPotionAmount() < 100)
                healthPotionText.text = " " + player.GetGoldAmount().ToString();
            else if (player.GetHealthPotionAmount() < 9)
                healthPotionText.text = "  " + player.GetHealthPotionAmount().ToString();
            else
                healthPotionText.text = player.GetHealthPotionAmount().ToString();

        }
        else {
            goldText.text = player.GetGoldAmount().ToString();
            healthPotionText.text = player.GetHealthPotionAmount().ToString();
        }


    }

}

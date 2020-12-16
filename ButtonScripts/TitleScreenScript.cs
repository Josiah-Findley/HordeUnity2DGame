using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScreenScript : MonoBehaviour
{
    public GameObject controlPanel;
    public GameObject xboxControlPanel;

    public GameObject gameModePanel;
    public GameObject selectMapPanel;
    public GameObject coOpMapPanel;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    //if controls button is pressed
    public void OpenControls()
    {
        controlPanel.SetActive(true);
    }

    public void CloseControls()
    {
        controlPanel.SetActive(false);
    }
    
    //xbox panel stuff
    public void OpenXboxControls()
    {
        xboxControlPanel.SetActive(true);
    }

    public void CloseXboxControls()
    {
        xboxControlPanel.SetActive(false);
    }

    //mode select panel stuff
    public void OpenModeSelect()
    {
        gameModePanel.SetActive(true);
    }

    public void CloseModeSelect()
    {
        gameModePanel.SetActive(false);
    }

    //PVP map select stuff
    public void OpenPVPMapSelect()
    {
        selectMapPanel.SetActive(true);
    }

    public void ClosePVPMapSelect()
    {
        selectMapPanel.SetActive(false);
    }

    //co op panel stuff
    public void OpenCoOpMapSelect()
    {
        coOpMapPanel.SetActive(true);    
    }

    public void CloseCoOpMapSelect()
    {
        coOpMapPanel.SetActive(false);
    }


    //quit game
    public void QuitButton()
    {
        Application.Quit();
    }
    
    //when dungeon button is pressed
    public void SelectDungeon()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("The Dungeon");
    }

    //when forest button is pressed
    public void SelectForest()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Forest");
    }

    public void SelectCoOpMap()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("CollabMode");
    }
}

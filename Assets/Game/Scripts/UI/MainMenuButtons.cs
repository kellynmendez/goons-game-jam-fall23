using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtons : MonoBehaviour
{
    // Enable the play button
    public void PressPlayButton()
    {
        SceneManager.LoadScene("MainLevel");
    }
    
    // Enable the quit button
    public void PressQuitButton()
    {
        Application.Quit();
    }

}

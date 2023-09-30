using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtons : MonoBehaviour
{
    [SerializeField] AudioClip levelClip;

    // Enable the play button
    public void PressPlayButton()
    {
        SceneManager.LoadScene("MainLevel");
        AudioManager.Instance.PlayGameScore(levelClip);
    }
    
    // Enable the quit button
    public void PressQuitButton()
    {
        Application.Quit();
    }

}

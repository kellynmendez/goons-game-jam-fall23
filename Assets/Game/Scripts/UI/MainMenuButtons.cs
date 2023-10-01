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
        SceneManager.LoadScene("Controls");
        AudioManager.Instance.PlayGameScore(levelClip);
    }
    
    // Enable the quit button
    public void PressQuitButton()
    {
        Application.Quit();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

}

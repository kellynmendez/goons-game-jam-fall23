using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchScreens : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (SceneManager.GetActiveScene().name == "Controls")
            {
                SceneManager.LoadScene("MainLevel");
            }
            else if (SceneManager.GetActiveScene().name == "Credits")
            {
                SceneManager.LoadScene("MainMenu");
            }
            else
            {
                    // Do nothing
            }
        }
    }
}

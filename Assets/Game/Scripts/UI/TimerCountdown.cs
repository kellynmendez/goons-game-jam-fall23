using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public class TimerCountdown : MonoBehaviour
{
    public Text timerText;
    bool visible = true;
    [SerializeField] private float secondsLeft;
    WaitForSeconds delay = new WaitForSeconds(1f);
    WaitForSeconds delayFlash = new WaitForSeconds(0.5f);

    // Start is called before the first frame update
    void Start()
    {
        TimerSwitchColor(0);                        // Set the initial color
        StartCoroutine("TimerCount");               // Start the timer
    }

    // Update is called once per frame
    void Update()
    {
  
    }

    IEnumerator TimerCount()
    {
        while (secondsLeft > 0)     // Iterate every second until no time left
        {
            // Decrement time left
            secondsLeft -= 1;

            // Switching colors:
            if (secondsLeft == 30)
            {
                TimerSwitchColor(1);
            }
            if (secondsLeft == 10)
            {
                TimerSwitchColor(2);

            }

            // Display time
            TimerDisplay();
            
            // Flash the time text for the last 10 seconds
            if (secondsLeft < 11)
            {
                yield return StartCoroutine("TimerFlash", visible);     // OFF
                yield return StartCoroutine("TimerFlash", visible);     // ON
            }
            else
            {
                yield return delay;     // Wait for a second
            }
        }
        SceneManager.LoadScene("Credits");      // Load the credits scene afterwards
    }

    IEnumerator TimerFlash(bool visible)
    {
        // Disable/enable the text
        this.visible = !visible;    
        timerText.gameObject.SetActive(visible);
        yield return delayFlash;        // Wait half the second
    }

    void TimerDisplay()
    {
        // Update the timer text object
        timerText.text = (string.Format("00:{0:00}", secondsLeft));
    }

    void TimerSwitchColor(int opt)
    {
        switch (opt)
        {
            // Under 30 seonds - Yellow
            case 1:     timerText.color = Color.yellow;
                        break;
            // Under 10 seconds - Green
            case 2:     timerText.color = Color.green;
                        break;
            // Otherwise - White
            default:    timerText.color = Color.white;
                        break;
        }
    }
}

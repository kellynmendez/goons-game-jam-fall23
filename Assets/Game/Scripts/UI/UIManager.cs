using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] List<GameObject> playerHearts;

    private void Awake()
    {
        for (int i = 0; i < playerHearts.Count; i++)
        {
            playerHearts[i].SetActive(true);
        }
    }

    public void UpdateScore()
    {
        
    }

    public void PlayerHurt()
    {
        if (playerHearts.Count > 0)
        {
            Debug.Log("HERE");
            GameObject heart = playerHearts[0];
            heart.SetActive(false);
            playerHearts.Remove(heart);
        }
        
    }
}

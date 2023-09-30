using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance = null;

    [SerializeField] AudioClip mainMenuClip;
    [SerializeField] AudioClip levelClip;

    AudioSource _audioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            _audioSource = GetComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayGameScore(AudioClip clip)
    {
        _audioSource.clip = clip;
        _audioSource.Play();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (SceneManager.GetActiveScene().name == "Controls")
            {
                
                SceneManager.LoadScene("MainLevel");
                Debug.Log("here");
                PlayGameScore(levelClip);
            }
            if (SceneManager.GetActiveScene().name == "Credits")
            {
                SceneManager.LoadScene("MainMenu");
                PlayGameScore(mainMenuClip);
            }
        }
    }
}
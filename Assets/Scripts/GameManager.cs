using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public Player player;
    public AudioClip gameoverAudioClip;
    public Timer timer;

    private AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player.isDead)
        {
            player.enabled = false;
            audioSource.clip = gameoverAudioClip;
            audioSource.loop = false;
            audioSource.playOnAwake = false;
            timer.enabled = false;
            audioSource.Play();
            audioSource.PlayDelayed(gameoverAudioClip.length);
        }
        
    }
}

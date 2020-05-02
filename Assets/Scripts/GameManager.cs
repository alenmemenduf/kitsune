using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public Player player;
    public AudioClip gameoverAudioClip;

    public GameObject losePanel;
    public GameObject winPanel;
    public Timer timer;
    public Image winPanelImage;
    public Sprite[] starSprites;

    private AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player.isDead || player.isWinner)
        {
            OnGameover();
        }
  
        
    }

    void ShowLoseScreen()
    {
        losePanel.SetActive(true);
    }

    void ShowWinPanel()
    {
        float minutes = (Mathf.Floor((timer.time % 3600) / 60));
        float seconds = (timer.time % 60);


        if (minutes <= 3 && seconds < 30)
        {
            winPanelImage.sprite = starSprites[2];
        }
        else if (minutes == 3 && seconds >= 30)
        {
            winPanelImage.sprite = starSprites[1];
        }
        else if (minutes >= 4)
        {
            winPanelImage.sprite = starSprites[0];
        }
        winPanelImage.gameObject.SetActive(true);
        winPanel.SetActive(true);
    }

    void OnGameover()
    {
        player.enabled = false;
        audioSource.clip = gameoverAudioClip;
        audioSource.loop = false;
        audioSource.playOnAwake = false;
        timer.enabled = false;
        audioSource.Play();
        audioSource.PlayDelayed(gameoverAudioClip.length);

        if (player.isDead)
        {
            ShowLoseScreen();
        }
        if (player.isWinner)
        {
            ShowWinPanel();
        }
        
    }
}

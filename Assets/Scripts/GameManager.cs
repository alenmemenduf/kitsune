using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/*
 * The game manager is responsible for checking all the present instances and modifing the state of game if necessary.
 */
public class GameManager : MonoBehaviour
{

    public Player player;
    public AudioClip gameoverAudioClip;

    public GameObject losePanel;
    public GameObject winPanel;
    public Image winPanelImage;
    public Sprite[] starSprites;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

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
        winPanelImage.gameObject.SetActive(true);
        winPanel.SetActive(true);
    }

    void OnGameover()
    {
        player.enabled = false;
        audioSource.loop = false;
        audioSource.playOnAwake = false;


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

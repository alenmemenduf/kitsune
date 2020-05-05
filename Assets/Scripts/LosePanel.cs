using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LosePanel : MonoBehaviour
{

    public GameObject losePanel;

    public void Retry()
    {
        losePanel.SetActive(false);
        SceneManager.LoadScene("Game");
    }
}

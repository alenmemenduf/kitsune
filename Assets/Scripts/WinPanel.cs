using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class WinPanel : MonoBehaviour
{
    public GameObject winPanel;

    public void Retry()
    {
        winPanel.SetActive(false);
        SceneManager.LoadScene("TestScene");
    }
}

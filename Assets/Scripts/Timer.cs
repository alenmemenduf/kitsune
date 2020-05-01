using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{

    Text text;
    float time;
    public float timerSpeed = 1;
    public float colorChangeSpeed = 2;

    float minutes;
    // Start is called before the first frame update1
    void Start()
    {
        text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {

        if (minutes <= 4)
        {
            time += Time.deltaTime * timerSpeed;

            minutes = Mathf.Floor((time % 3600) / 60);
            string minutesText = (Mathf.Floor((time % 3600) / 60)).ToString("00");
            string seconds = (time % 60).ToString("00");


            text.text = minutes + ":" + seconds;
        }else
        {
            text.text = "5:00";
        }

        if(5 - minutes <= 1.5f)
        {
            float t = (Mathf.Sin(Time.time * colorChangeSpeed) + 1) / 2;
            text.color = Color.Lerp(Color.white, Color.red, t);
        }
    }
}

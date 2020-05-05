using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinningElement : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            print("Works");
            if (!collision.GetComponent<Player>().isDead)
            {
                collision.GetComponent<Player>().isWinner = true;
            }
        }
    }
}

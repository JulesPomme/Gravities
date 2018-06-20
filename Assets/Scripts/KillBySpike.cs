using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillBySpike : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == GameManager.Instance().player)
        {
            GameManager.Instance().RestartAtLastCheckPoint();
        }
    }
}

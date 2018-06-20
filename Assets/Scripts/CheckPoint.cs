using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{

    public Vector2 respawnPoint;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == GameManager.Instance().player)
        {
            GameManager.Instance().UpdateRespawnPoint(respawnPoint);
        }
    }
}

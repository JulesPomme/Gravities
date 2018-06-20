using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    public GameObject player;
    /// <summary>
    /// A rectangle size around the player inside which he moves without being followed.
    /// </summary>
    public Vector2 toleranceRectangleSize = new Vector2(0, 0);
    /// <summary>
    /// The horizontal bounds of the level
    /// </summary>
    public Vector2 hBounds = new Vector2(0, 0);
    /// <summary>
    /// The vertical bounds of the level
    /// </summary>
    public Vector2 vBounds = new Vector2(0, 0);

    private void Start()
    {
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);
    }

    void Update()
    {

        float xPosition = transform.position.x;
        float yPosition = transform.position.y;

        float xDiff = transform.position.x - player.transform.position.x;
        //Debug.Log("cam x = " + transform.position.x + ", player x = " + player.transform.position.x + ", diff " + xDiff);
        if (Mathf.Abs(xDiff) > toleranceRectangleSize.x)
        {
            xPosition = player.transform.position.x + (Mathf.Sign(xDiff) * toleranceRectangleSize.x);
        }

        float yDiff = transform.position.y - player.transform.position.y;
        //Debug.Log("cam y = " + transform.position.y + ", player y = " + player.transform.position.y + ", diff " + yDiff);
        if (Mathf.Abs(yDiff) > toleranceRectangleSize.y)
        {
            yPosition = player.transform.position.y + (Mathf.Sign(yDiff) * toleranceRectangleSize.y);
        }

        xPosition = Mathf.Max(hBounds.x, Mathf.Min(hBounds.y, xPosition));
        yPosition = Mathf.Max(vBounds.x, Mathf.Min(vBounds.y, yPosition));

        transform.position = new Vector3(xPosition, yPosition, transform.position.z);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tests_Tasks : MonoBehaviour
{

    public GameObject player;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Tasks.NewTask(1f, PlayerMoveX);
            Tasks.NewTask(3.5f, PlayerMoveY);
            Tasks.NewTask(5.5f, PlayerMoveXY);
        }
    }

    private void PlayerMoveX()
    {
        player.transform.position += new Vector3(1f, 0f, 0f);
        Debug.Log("Se ha movido en X");
    }

    private void PlayerMoveY()
    {
        player.transform.position += new Vector3(0f, 1f, 0f);
        Debug.Log("Se ha movido en Y");
    }

    private void PlayerMoveXY()
    {
        player.transform.position += new Vector3(1f, 1f, 0f);
        Debug.Log("Se ha movido en X e Y");
    }

}

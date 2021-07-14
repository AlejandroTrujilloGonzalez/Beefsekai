using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapController : MonoBehaviour
{
    [SerializeField]private float speedToMove = 4f;
    public GameObject player;
    public Transform[] placesPositions;

    private Vector3 positions;

    //bool canMoveToPlace01 = false;


    private void Update()
    {

        Vector3 position = positions;
        player.transform.position = Vector3.Lerp(player.transform.position, positions, speedToMove * Time.deltaTime);

        //DetectGameObj();

    }


    #region Métodos que cambian la posicion del player
    public void Place01()
    {
        positions = placesPositions[0].transform.position;

    }

    public void Place02()
    {
        positions = placesPositions[1].transform.position;
        Debug.Log("place02");
    }

    public void Place03()
    {
        positions = placesPositions[2].transform.position;
    }

    public void Place04()
    {
        positions = placesPositions[3].transform.position;
    }

    public void Place05()
    {
        positions = placesPositions[4].transform.position;
    }
    #endregion


    public void PlaceTeleport(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }



}

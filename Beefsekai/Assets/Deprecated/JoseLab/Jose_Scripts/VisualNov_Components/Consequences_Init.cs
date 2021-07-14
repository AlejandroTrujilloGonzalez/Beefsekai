using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Consequences_Init : MonoBehaviour
{
    public ElectionsPanel electionsPanel;


    #region Singleton Chusterillo Temporal
    public static Consequences_Init consequences;

    private void Awake()
    {
        if (consequences == null)
        {
            consequences = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        electionsPanel = FindObjectOfType<ElectionsPanel>().GetComponent<ElectionsPanel>();

    }
    #endregion 


    private void Update()
    {
        ElectionsBranches();
    }

    public void ElectionsBranches()
    {
        if (electionsPanel.canChoice == true)
        {
            switch (electionsPanel.idElection)
            {
                case 1:
                    Debug.Log("Boton 0 presionado");
                    break;
                case 2:
                    Debug.Log("Boton 1 presionado");
                    break;
                case 3:
                    Debug.Log("boton 2 presionado");
                    break;
                case 4:
                    Debug.Log("Boton 3 presionado");
                    break;

                default:
                    break;
            }
        }

    }

    public void NextMessage()
    {

    }

    public void PreviousMessage()
    {

    }





}

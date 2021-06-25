using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager_Init : MonoBehaviour
{
    #region MortalSingletonOfDarkness
    private static GameManager_Init _instance;

    public static GameManager_Init Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager_Init>();
                if (_instance == null)
                {
                    _instance = new GameObject().AddComponent<GameManager_Init>();
                }
            }

            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(this);
        }

        DontDestroyOnLoad(this);

    }
    #endregion


    public int messageDisplayed;
    //Variable consequenceToLoad me imagino que vendrá del tipo Consequences que crearemos
    

    public void Consequence()
    {
        int previousMessage = messageDisplayed - 1;
        //DoConsequence(ConsequenceToLoad)
    }

    // void ConsequenceToLoad()

    public int ChangeMessageDisplayed(int num)//Temporal
    {
        int newMessageDisplayed = messageDisplayed - num;
        //Método para organizar el Parser


        return newMessageDisplayed;
    }

    //Los controles del menú igual es mejor que estén en otro Manager? que se encarge de los sonidos opciones, salir etc


}

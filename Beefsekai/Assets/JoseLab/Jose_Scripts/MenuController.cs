using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject optionPanel;
    public void ChargeFirstScene()//Igual la escena que va cuando pulsamos no es la de juego si no una cinematica o presentacion
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LoadEspecificScene(string nameScene)//Por si necesitamos cargar una escena en especifico en algún momento
    {
        SceneManager.LoadScene(nameScene);
    }

    public void LoadScene()
    {
        Debug.Log("Cargando Escena");//Ya veremos como meter el sistema de guardado
    }

    public void ExitGame()
    {
        Debug.Log("saliendo");
        Application.Quit();
    }

    public void ShowOptions()//He creado un método para cada acción para más adelante partir de aquí a la hora de incluir animaciones o lo que fuera
    {
        startPanel.SetActive(false);
        optionPanel.SetActive(true);
    }

    public void HideOptions()
    {
        startPanel.SetActive(true);
        optionPanel.SetActive(false);
    }


}

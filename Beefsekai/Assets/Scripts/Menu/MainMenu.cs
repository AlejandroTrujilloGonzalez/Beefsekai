using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public GameObject startPanel;
    public GameObject optionPanel;
    public GameObject loadedPanel;

    public string selectedGameFile = "";

    public GameSavePanel saveLoadPanel;
    int currentSavageLoadPage
    {
        get { return saveLoadPanel.currentSaveLoadPage; }
    }


    public static MainMenu instance;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        CloseSettingPanel();
        CloseLoadGamePanel();
    }

    public void ClickLoadGame()
    {
        loadedPanel.SetActive(true);
        startPanel.SetActive(false);
        optionPanel.SetActive(false);

        saveLoadPanel.LoadFilesOntoScreen(currentSavageLoadPage);

    }

    public void ClickSettings()
    {
        optionPanel.SetActive(true);
        startPanel.SetActive(false);
        loadedPanel.SetActive(false);
    }

    public void BackButton()
    {
        startPanel.SetActive(true);
        CloseSettingPanel();
        CloseLoadGamePanel();
    }

    public void CloseLoadGamePanel()
    {
        loadedPanel.SetActive(false);
    }

    public void CloseSettingPanel()
    {
        optionPanel.SetActive(false);
    }

    public void StartNewGame()
    {
        selectedGameFile = "new file";
        //Guarda el nombre del archivo que vamos a cargar en el juego
        FileManager.SaveFile(FileManager.savPath + "savData/file", selectedGameFile);

        SceneManager.LoadScene("Main");
    }


    public void ExitGame()
    {
        Application.Quit();
    }

}

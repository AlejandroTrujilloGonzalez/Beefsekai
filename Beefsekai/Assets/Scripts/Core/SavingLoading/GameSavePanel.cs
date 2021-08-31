using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameSavePanel : MonoBehaviour
{

    public List<BUTTON> buttons = new List<BUTTON>();

    public int currentSaveLoadPage = 1;

    public CanvasGroup canvasGroup;

    public Button loadButton;
    public Button saveButton;
    public Button deleteButton;

    public enum TASK
    {
        saveToSlot,
        loadFromSlot,
        deleteSlot
    }

    public TASK slotTask = TASK.loadFromSlot;

    public void LoadFilesOntoScreen(int page = 1)
    {
        currentSaveLoadPage = page;

        string directory = FileManager.savPath + "savData/GameFiles/" + page.ToString() + "/";

        if (System.IO.Directory.Exists(directory))
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                BUTTON b = buttons[i];
                string expectedFile = directory + (i + 1).ToString() + ".txt";
                if (System.IO.File.Exists(expectedFile))
                {
                    if (NovelController.instance.encryptGameFile)
                    {
                        GAMEFILE file = FileManager.LoadEncryptedJSON<GAMEFILE>(expectedFile, FileManager.keys);
                        b.button.interactable = true;
                        byte[] previewImageData = FileManager.LoadComposingBytes(directory + (i + 1).ToString() + ".png");
                        Texture2D previewImage = new Texture2D(2, 2);
                        ImageConversion.LoadImage(previewImage, previewImageData);
                        file.previewImagePath = previewImage;
                        b.previewDisplay.texture = file.previewImagePath;
                        b.dateTimeText.text = page.ToString() + file.modificationDate;
                    }
                    else
                    {
                        GAMEFILE file = FileManager.LoadJSON<GAMEFILE>(expectedFile);
                        b.button.interactable = true;
                        byte[] previewImageData = FileManager.LoadComposingBytes(directory + (i + 1).ToString() + ".png");
                        Texture2D previewImage = new Texture2D(2, 2);
                        ImageConversion.LoadImage(previewImage, previewImageData);
                        file.previewImagePath = previewImage;
                        b.previewDisplay.texture = file.previewImagePath;
                        b.dateTimeText.text = page.ToString() + file.modificationDate;
                    }


                }
                else
                {
                    b.button.interactable = allowSavingFromThisScreen;
                    b.previewDisplay.texture = Resources.Load<Texture2D>("Art/Images/UI/EmptyGameFile");
                    b.dateTimeText.text = page.ToString() + "\n" + "empty file...";
                }
            }
        }
        else
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                BUTTON b = buttons[i];
                b.button.interactable = allowSavingFromThisScreen;
                b.previewDisplay.texture = Resources.Load<Texture2D>("Art/Images/UI/EmptyGameFile");
                b.dateTimeText.text = page.ToString() + "\n" + "empty file...";
            }
        }
    }

    public BUTTON selectedButton = null;
    string selectedGameFile = "";
    string selectedFilePath = "";
    public bool allowLoadFromThisScreen = true;
    public bool allowSavingFromThisScreen = true;
    public bool allowDeletingFromThisScreen = true;

    public void ClickOnSaveSlot(Button button)
    {
        foreach (BUTTON B in buttons)
        {
            if (B.button == button)
            {
                selectedButton = B;
            }
        }

        selectedGameFile = currentSaveLoadPage.ToString() + "/" + (buttons.IndexOf(selectedButton) + 1).ToString();
        selectedFilePath = FileManager.savPath + "savData/GameFiles/" + selectedGameFile + ".txt";

        if (System.IO.File.Exists(selectedFilePath))
        {
            loadButton.interactable = allowLoadFromThisScreen;
            saveButton.interactable = allowSavingFromThisScreen;
            deleteButton.interactable = allowDeletingFromThisScreen;
        }
        else
        {
            selectedButton.dateTimeText.text = "<color=red>FILE NOT FOUND";
            loadButton.interactable = false;
            saveButton.interactable = allowSavingFromThisScreen;
            deleteButton.interactable = true;
        }

    }

    public void LoadFromSelectedSlot()
    {
        if (NovelController.instance.encryptGameFile)
        {
            GAMEFILE file = FileManager.LoadEncryptedJSON<GAMEFILE>(selectedFilePath, FileManager.keys);
        }
        else
        {
            GAMEFILE file = FileManager.LoadJSON<GAMEFILE>(selectedFilePath);
        }

        FileManager.SaveFile(FileManager.savPath + "savData/file", selectedGameFile);

        UnityEngine.SceneManagement.SceneManager.LoadScene("Main");

        gameObject.SetActive(false);
    }

    public void ClosePanel()
    {

    }

    public void SaveToSelectedSlot()
    {
        if (NovelController.instance != null)
        {
            savingFile = StartCoroutine(SavingFile());
        }
    }

    Coroutine savingFile = null;
    IEnumerator SavingFile()
    {
        NovelController.instance.activeGameFileName = selectedGameFile;
        canvasGroup.alpha = 0;
        yield return new WaitForEndOfFrame();
        NovelController.instance.SaveGameFile();

        selectedButton.dateTimeText.text = currentSaveLoadPage.ToString() + "\n" + GAMEFILE.activeFile.modificationDate;
        selectedButton.previewDisplay.texture = GAMEFILE.activeFile.previewImagePath;
        yield return new WaitForEndOfFrame();
        canvasGroup.alpha = 1;

        savingFile = null;
    }

    public void DeleteSlot()
    {

    }

    [System.Serializable]
    public class BUTTON
    {
        public Button button;
        public RawImage previewDisplay;
        public TextMeshProUGUI dateTimeText;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ElectionsPanel : MonoBehaviour
{
    public int idElection;

    #region UIComponentes
    public GameObject electionPanel;
    public Image panelImage;
    public bool canChoice = false;

    public int buttonsOn;//Cuantos botones habrá en la pantalla de elección
    public GameObject[] buttonsObj;
    //public Button[] buttonsArray;
    public string[] buttonsText;
    private GridLayoutGroup electionsGridLayout;

    #endregion


    private void Start()
    {
        buttonsText = new string[buttonsOn];

        //SelectNumbersOfButtons(4);

        for (int i = 0; i < buttonsOn; i++)
        {
            buttonsObj = GameObject.FindGameObjectsWithTag("ChoiceButton");
        }

        foreach (GameObject buttonsActive in buttonsObj)
        {
            buttonsActive.SetActive(true);
        }

        electionsGridLayout = electionPanel.GetComponent<GridLayoutGroup>();


    }


    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < buttonsText.Length; i++)
        {
            buttonsObj[i].GetComponentInChildren<TMP_Text>().text = buttonsText[i];
        }

        ChangeNumbersOfOptions();//Muestra de 2 a 4 opciones según el valor de buttonsOn
        TestInputs();//Temporal




        if (canChoice == true)
        {
            panelImage.rectTransform.position = new Vector2(panelImage.rectTransform.position.x, 230);
        }
        else
        {
            panelImage.rectTransform.position = new Vector2(panelImage.rectTransform.position.x, -270);
        }
    }

    public void SelectNumbersOfButtons(int buttonsToDisplay)//Si llamamos a este método muestra los botones que le pasamos por parámetro, tambien modificando directamente la var
    {
        buttonsOn = buttonsToDisplay;
    }


    public void ChangeNumbersOfOptions()//Método que de momento se encarga de mostrar 2 3 o 4 botones según elijamos 
    {
        switch (buttonsOn)//Perdón pero funciona xd
        {
            case 2:
                buttonsObj[buttonsObj.Length - 1].SetActive(false);
                buttonsObj[buttonsObj.Length - 2].SetActive(false);
                electionsGridLayout.cellSize = new Vector2(500f, 40f);
                break;
            case 3:
                foreach (GameObject buttonsActive in buttonsObj)
                {
                    buttonsActive.SetActive(true);
                }
                buttonsObj[buttonsObj.Length - 1].SetActive(false);
                electionsGridLayout.cellSize = new Vector2(500f, 33f);
                break;
            case 4:
                foreach (GameObject buttonsActive in buttonsObj)
                {
                    buttonsActive.SetActive(true);
                }
                electionsGridLayout.cellSize = new Vector2(500f, 25f);
                break;

            default:
                break;

        }
    }

    public void SelectChoice(int idChoice)//Temporal, podemos usar como dice Javi un event o delegado
    {
        idElection = idChoice;
        Debug.Log("Has elegido: " + idElection);
    }

    //public void ChoiseSelected()
    //{
    //    if (canChoice == true)
    //    {
    //        switch (idElection)
    //        {
    //            case 0:
    //                break;
    //            case 1:
    //                break;
    //            case 2:
    //                break;
    //            case 3:
    //                break;
    //            default:
    //                break;
    //        }
    //    }
    //}



    private void TestInputs()//Temporal
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            canChoice = true;
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            canChoice = false;
        }
    }


}

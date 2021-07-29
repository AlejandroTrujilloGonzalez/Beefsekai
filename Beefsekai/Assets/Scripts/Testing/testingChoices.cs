using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testingChoices : MonoBehaviour
{

    public string tittle = "I Like...";
    public string[] choices;


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            ChoiceScreen.Show(tittle, choices);
        }
    }


    IEnumerator DynamycStoryExample()
    {

        //Copio lo que tiene el pavo para tener un ejemplo de como usa las elecciones
        //Load story part 1
        NovelController.instance.LoadChapterFile("story_1"); yield return new WaitForEndOfFrame();
        while (NovelController.instance.isHandlingChapterFile)
        {
            yield return new WaitForEndOfFrame();
        }

        ChoiceScreen.Show("blabla", "elecion 1", "elecion 2");
        while (ChoiceScreen.isWaitingForChoiceToBeMade)
        {
            yield return new WaitForEndOfFrame();
        }

        //eleccion 1
        if (ChoiceScreen.lastChoiceMade.index == 0)
        {
            NovelController.instance.LoadChapterFile("story_a1");
        }
        else
        {
            NovelController.instance.LoadChapterFile("story_a2");
        }

        yield return new WaitForEndOfFrame();
        NovelController.instance.Next();

        while (NovelController.instance.isHandlingChapterFile)
            yield return new WaitForEndOfFrame();


    }

}

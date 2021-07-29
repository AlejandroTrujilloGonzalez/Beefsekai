using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputScreenTest : MonoBehaviour
{
    public string displayTittle = "";

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            InputScreen.Show(displayTittle);
        }

        if (Input.GetKeyDown(KeyCode.F2) && InputScreen.isWaitingForUserInput)
        {
            InputScreen.instance.Accept();
            print("you entered the value of" + InputScreen.currentInput);
        }
    }
}

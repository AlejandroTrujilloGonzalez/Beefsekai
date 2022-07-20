using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkipUI : MonoBehaviour
{

    [SerializeField] GameObject speech;

    [SerializeField] GameObject choice;

    [SerializeField] GameObject showBut;

    public void ToggleUI()
    {
        speech.SetActive(!speech.activeInHierarchy);
        choice.SetActive(!choice.activeInHierarchy);
        showBut.SetActive(!showBut.activeInHierarchy);
    }

}

using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OldDialogueSystem : MonoBehaviour
{
    public static OldDialogueSystem instance;

    public ELEMENTS elements;

    public float showCharactersSpeed = 0.04f;

    private void Awake()
    {
        instance = this;
    }

    public void Say(string speech, string speakerName, bool additive = false) //Additive a true en caso de que sea un mensaje añadido al actual
    {
        StopSpeaking();
        speechText.text = targetSpeech;
        speaking = StartCoroutine(Speaking(speech, additive, speakerName));
    }

    public void StopSpeaking()
    {
        if (IsSpeaking)
        {
            StopCoroutine(speaking);
        }
        speaking = null;
    }

    public bool IsSpeaking { get { return speaking != null; } }
    public bool IsWaitingForUserInput = false;

    string targetSpeech = "";
    Coroutine speaking = null;
    IEnumerator Speaking(string speech, bool additive, string speakerName = "")
    {
        speechPanel.SetActive(true);
        targetSpeech = speech;

        //Para añadir un mensaje al que ya esta mostrandose en el panel (Puntos suspensivos, etc)
        if (!additive)
        {
            speechText.text = "";
        } else
        {
            targetSpeech = speechText.text + targetSpeech;
        }
        
        speakerNameText.text = DetermineSpeaker(speakerName);
        IsWaitingForUserInput = false;

        while (speechText.text != targetSpeech)
        {
            speechText.text += targetSpeech[speechText.text.Length];
            yield return new WaitForSeconds(showCharactersSpeed); //Velocidad escritura de los caracteres
        }

        //Texto terminado
        IsWaitingForUserInput = true;
        while (IsWaitingForUserInput)
        {
            yield return new WaitForEndOfFrame();
        }
        StopSpeaking();
    }

    public string DetermineSpeaker(string s)
    {
        string retVal = speakerNameText.text;
        if (s != speakerNameText.text && s != "")
        {
            retVal = (s.ToLower().Contains("narrador") ? "" : s);
        }

        return retVal;
    }

    [Serializable]
    public class ELEMENTS
    {
        //Main panel
        public GameObject speechPanel;
        public TextMeshProUGUI speakerNameText;
        public TextMeshProUGUI speechText;
    }
    public GameObject speechPanel { get { return elements.speechPanel; } }
    public TextMeshProUGUI speakerNameText { get { return elements.speakerNameText; } }
    public TextMeshProUGUI speechText { get { return elements.speechText; } }
}

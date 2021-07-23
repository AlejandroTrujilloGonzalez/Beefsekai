using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueSyst : MonoBehaviour
{
    #region Objeto Elements con gets
    [System.Serializable]
    public class ELEMENTS
    {
        public GameObject speechPanel;
        public TMP_Text speakerNameText;
        public TMP_Text speechText;
    }
    public ELEMENTS elements;
    public GameObject speechPanel { get { return elements.speechPanel; } }
    public TMP_Text speakerNameText { get { return elements.speakerNameText; } }
    public TMP_Text speechText { get { return elements.speechText; } }
    #endregion


    #region MiniSingleton
    public static DialogueSyst instance;

    private void Awake()
    {
        instance = this;
    }
    #endregion

    public bool isSpeaking { get { return speaking != null; } }
    Coroutine speaking = null;
    public bool isWaitingForUserInput = false;

    private string targetSpeech = "";

    public void Say(string speech, string speaker)
    {
        StopSpeaking();

        speaking = StartCoroutine(Speaking(speech, false, speaker));
    }

    public void SayAdd(string speech, string speaker = "")
    {
        StopSpeaking();
        speechText.text = targetSpeech;
        speaking = StartCoroutine(Speaking(speech, true, speaker));
    }

    public void StopSpeaking()
    {
        if (isSpeaking)
        {
            StopCoroutine(speaking);
        }
        speaking = null;
    }

    IEnumerator Speaking(string speech, bool additive, string speaker)
    {
        speechPanel.SetActive(true);
        targetSpeech = speech;
        if (!additive)
        {
            speechText.text = "";
        }
        else
        {
            targetSpeech = speechText.text + targetSpeech;
        }


        speakerNameText.text = DetermineSpeaker(speaker);
        isWaitingForUserInput = false;

        while (speechText.text != targetSpeech)
        {
            speechText.text += targetSpeech[speechText.text.Length];
            yield return new WaitForEndOfFrame();
        }

        isWaitingForUserInput = true;

        while (isWaitingForUserInput)
        {
            yield return new WaitForEndOfFrame();
        }
        StopSpeaking();

    }

    private string DetermineSpeaker(string s)
    {
        string retValue = speakerNameText.text;
        if (s != speakerNameText.text && s != "")
        {
            retValue = (s.ToLower().Contains("narrator")) ? "" : s;
        }

        return retValue;

    }

    public void Close()
    {
        StopSpeaking();
        speechPanel.SetActive(false);
    }

}

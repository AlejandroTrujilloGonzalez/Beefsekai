using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueSystem : MonoBehaviour
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
    public static DialogueSystem instance;

    private void Awake()
    {
        instance = this;
    }
    #endregion

    public bool isSpeaking { get { return speaking != null; } }
    Coroutine speaking = null;

    TextArchitect textArchitect = null;

    public bool isWaitingForUserInput = false;

    private string targetSpeech = "";

    public void Say(string speech, string speaker = "", bool additive = false)
    {
        StopSpeaking();

        if (additive)
            speechText.text = targetSpeech;

        speaking = StartCoroutine(Speaking(speech, false, speaker));
    }

    public void StopSpeaking()
    {
        if (isSpeaking)
        {
            StopCoroutine(speaking);
        }

        if (textArchitect != null && textArchitect.isConstructing)
        {
            textArchitect.Stop();
        }

        speaking = null;
    }

    IEnumerator Speaking(string speech, bool additive, string speaker)
    {
        speechPanel.SetActive(true);

        string additiveSpeech = additive ? speechText.text : "";

        targetSpeech = additiveSpeech + speech;

        textArchitect = new TextArchitect(speech, additiveSpeech);

        speakerNameText.text = DetermineSpeaker(speaker);
        isWaitingForUserInput = false;

        while (textArchitect.isConstructing)
        {
            if (Input.GetKey(KeyCode.Space))
                textArchitect.skip = true;

            speechText.text = textArchitect.currentText;

            yield return new WaitForEndOfFrame();
        }

        speechText.text = textArchitect.currentText;

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

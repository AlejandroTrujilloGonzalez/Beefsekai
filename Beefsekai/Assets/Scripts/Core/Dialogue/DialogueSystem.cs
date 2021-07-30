using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueSystem : MonoBehaviour
{
    public TextArchitect currentArchitect { get { return textArchitect; } }

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

    public GameObject speakerNamePane;
    public TMP_Text speakerNameText { get { return elements.speakerNameText; } }
    public TMP_Text speechText { get { return elements.speechText; } }

    //public GameObject speakerNamePane { get { return elements.speakerNamePane; } }
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

    public string targetSpeech = "";

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

        if (textArchitect == null)
            textArchitect = new TextArchitect(speechText, speech, additiveSpeech);
        else
            textArchitect.Renew(speech, additiveSpeech);

        speakerNameText.text = DetermineSpeaker(speaker);

        speakerNamePane.SetActive(speakerNameText.text != "");
        isWaitingForUserInput = false;

        while (textArchitect.isConstructing)
        {
            if (Input.GetKey(KeyCode.Space))
                textArchitect.skip = true;

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

        if (retValue.Contains("*"))
            retValue = retValue.Remove(0, 1);

        return retValue;

    }

    public void Close()
    {
        StopSpeaking();
        speechPanel.SetActive(false);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueSystem : MonoBehaviour
{
    public static DialogueSystem instance;

    public ELEMENTS elements;

    void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// Say something and show it on the speech box.
    /// </summary>
    public void Say(string speech, string speaker = "", bool additive = false)
    {
        StopSpeaking();

        if (additive)
            speechText.text = targetSpeech;

        speaking = StartCoroutine(Speaking(speech, additive, speaker));
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

    public bool isSpeaking { get { return speaking != null; } }
     public bool isWaitingForUserInput = false;

    public string targetSpeech = "";
    Coroutine speaking = null;
    TextArchitect textArchitect = null;
    public TextArchitect currentArchitect { get { return textArchitect; } }

    IEnumerator Speaking(string speech, bool additive, string speaker = "")
    {
        speechPanel.SetActive(true);

        string additiveSpeech = additive ? speechText.text : "";
        targetSpeech = additiveSpeech + speech;

        //create a new architect the very first time. Any time other than that and we renew the architect.
        if (textArchitect == null)
            textArchitect = new TextArchitect(speechText, speech, additiveSpeech);
        else
            textArchitect.Renew(speech, additiveSpeech);

        speakerNameText.text = DetermineSpeaker(speaker);//temporary
        speakerNamePane.SetActive(speakerNameText.text != "");

        isWaitingForUserInput = false;

        if (isClosed)
            OpenAllRequirementsForDialogueSystemVisibility(true);

        while (textArchitect.isConstructing)
        {
            if (Input.GetKey(KeyCode.Space))
                textArchitect.skip = true;

            yield return new WaitForEndOfFrame();
        }

        //text finished
        isWaitingForUserInput = true;
        while (isWaitingForUserInput)
            yield return new WaitForEndOfFrame();

        StopSpeaking();
    }

    string DetermineSpeaker(string s)
    {
        string retVal = speakerNameText.text;//default return is the current name
        if (s != speakerNameText.text && s != "")
            retVal = (s.ToLower().Contains("narrator")) ? "" : s;

        if (retVal.Contains("*"))
            retVal = retVal.Remove(0, 1);

        return retVal;
    }

    /// <summary>
    /// Close the entire speech panel. Stop all dialogue.
    /// </summary>
    public void Close()
    {
        print("Close");
        StopSpeaking();

        for (int i = 0; i < SpeechPanelRequirements.Length; i++)
        {
            SpeechPanelRequirements[i].SetActive(false);
        }
    }

    public void OpenAllRequirementsForDialogueSystemVisibility(bool v)
    {
        for (int i = 0; i < SpeechPanelRequirements.Length; i++)
        {
            SpeechPanelRequirements[i].SetActive(v);
        }
    }

    public void Open(string speakerName = "", string speech = "")
    {
        if (speakerName == "" && speech == "")
        {
            OpenAllRequirementsForDialogueSystemVisibility(false);
            return;
        }

        OpenAllRequirementsForDialogueSystemVisibility(true);

        speakerNameText.text = speakerName;

        speakerNamePane.SetActive(speakerName != "");

        speechText.text = speech;
    }

    public bool isClosed
    {
        get { return !speechBox.activeInHierarchy; }
    }

    [System.Serializable]
    public class ELEMENTS
    {
        /// <summary>
        /// The main panel containing all dialogue related elements on the UI
        /// </summary>
        public GameObject speechPanel;
        public GameObject speakerNamePane;
        public TMP_Text speakerNameText;
        public TMP_Text speechText;
    }
    public GameObject speechPanel { get { return elements.speechPanel; } }
    public TMP_Text speakerNameText { get { return elements.speakerNameText; } }
    public TMP_Text speechText { get { return elements.speechText; } }
    public GameObject speakerNamePane { get { return elements.speakerNamePane; } }

    /// <summary>
    /// All objects of this array must be enabled or disabled depending on the status of the dialogue system
    /// </summary>
    public GameObject[] SpeechPanelRequirements;
    public GameObject speechBox;

    ///////////////////BUTTONS TESTING///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///
    [HideInInspector]public bool autoOn = false;
    public void AutonOn()
    {
        Debug.Log("Activando auto");
        autoOn = true;
    }

    public void AutoOff()
    {
        Debug.Log("Desactivando auto");
        autoOn = false;
    }

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShowDialogue : MonoBehaviour
{
    DialogueSyst dialogue;

    public string[] s = new string[]
    {
        "Sabes porqué Miguel Bosé no come hamburguesas?: Bartowolo",
        "...",
        "Porque la carne es vacuna"
    };
    private int index = 0;

    // Start is called before the first frame update
    void Start()
    {
        dialogue = DialogueSyst.instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!dialogue.isSpeaking || dialogue.isWaitingForUserInput)
            {
                if (index >= s.Length)
                {
                    return;
                }
                Say(s[index]);
                index++;
            }
        }
    }

    public void Say(string s)
    {
        string[] parts = s.Split(':');
        string speech = parts[0];
        string speaker = (parts.Length >= 2) ? parts[1] : "";

        dialogue.Say(speech, speaker);
    }

}

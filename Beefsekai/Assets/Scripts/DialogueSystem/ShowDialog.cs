using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowDialog : MonoBehaviour
{
    //Clase basica para mostrar mensajes

    DialogueSystem dialogue;
    public Message[] messages;
    public int messageIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        dialogue = DialogueSystem.instance;
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            if (!dialogue.IsSpeaking || dialogue.IsWaitingForUserInput)
            {
                if (messageIndex >= messages.Length)
                {
                    return;
                }

                dialogue.Say(messages[messageIndex].Contenido, messages[messageIndex].NombreDePersonaje);
                messageIndex++;
            } 
        }
    }
}

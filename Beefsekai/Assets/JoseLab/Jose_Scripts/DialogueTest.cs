using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueTest : MonoBehaviour
{

    public Message messageTest;

    [SerializeField] private messageType messageType;
    [SerializeField] private TMP_Text pnjName;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private Sprite sprite;

    Coroutine letterCoroutine;
    public bool waitForInput = true;

    // Start is called before the first frame update
    void Start()
    {
        this.messageType = messageTest.TipoDeMensaje;
        pnjName.text = messageTest.NombreDePersonaje;
        dialogueText.text = messageTest.Contenido;
        sprite = messageTest.SpriteResaltado;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

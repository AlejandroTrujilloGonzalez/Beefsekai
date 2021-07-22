using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class characterTest : MonoBehaviour
{

    public Character Bartowolo;

    // Start is called before the first frame update
    void Start()
    {
        Bartowolo = CharacterManager.instance.GetCharacters("Bartowolo", enableCreatedCharacterOnStart: false);
    }

    public string[] speech;
    int i = 0;


    public Vector2 moveTarget;
    public float moveSpeed;
    public bool smooth;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (i < speech.Length)
            {
                Bartowolo.Say(speech[i]);
            }
            else
            {
                DialogueSyst.instance.Close();
            }

            i++;
        }


        if (Input.GetKey(KeyCode.M))
        {
            Bartowolo.MoveTo(moveTarget, moveSpeed, smooth);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            Bartowolo.StopMoving();
        }

    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class architectTesting : MonoBehaviour
{

    public TMP_Text tmprotext;

    TextArchitect architect;

    [TextArea(5, 10)]
    public string say;

    public int charactersPerFrame = 1;

    public float speed = 1f;

    public bool useEncap = true;

    void Start()
    {
        architect = new TextArchitect(say);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            architect = new TextArchitect(say, "", charactersPerFrame, speed, useEncap);
        }

        tmprotext.text = architect.currentText;
    }

}

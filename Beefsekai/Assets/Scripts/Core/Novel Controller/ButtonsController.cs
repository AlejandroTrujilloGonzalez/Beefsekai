using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonsController : MonoBehaviour
{
    [Header("GO Buttons")]
    public GameObject skipButton;
    public GameObject autoButton;
    public GameObject nextButton;
    public GameObject backButton;
    public GameObject menuButton;

    [HideInInspector]public bool autoTxt;


    public TextArchitect textArchitect;
    public TextArchitect currentArchitect { get { return textArchitect; } }

    private void Awake()
    {
        autoTxt = false;
    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //AutoTool();
    }

    public void PutAutoOn()
    {
        autoTxt = true;
    }

    public void PutAutoOff()
    {
        autoTxt = false;
    }

    public void AutoTool()
    {
        if (autoTxt == true)
        {
            currentArchitect.skip = true;
        }
        else if (autoTxt == false)
        {
            currentArchitect.skip = false;
        }
    }

}

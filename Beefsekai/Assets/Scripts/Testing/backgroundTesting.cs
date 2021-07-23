using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class backgroundTesting : MonoBehaviour
{

    public BackGround wood;
    public BackGround city;
    public BackGround bridge;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            wood = BackGroundController.instance.GetBackground("Wood");
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            city = BackGroundController.instance.GetBackground("Fantasy_City");
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            bridge = BackGroundController.instance.GetBackground("Bridge");
        }

    }
}

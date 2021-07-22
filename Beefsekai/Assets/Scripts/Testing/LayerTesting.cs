using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerTesting : MonoBehaviour
{
    BGController bgcontroller;

    public Texture texture;
    public float speed;
    public bool smooth;

    // Start is called before the first frame update
    void Start()
    {
        bgcontroller = BGController.instance;
    }

    // Update is called once per frame
    void Update()
    {
        BGController.Layer layer = null;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("aparecen cosas");
            layer = bgcontroller.background;
            Debug.Log(bgcontroller.background.ToString());
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            layer = bgcontroller.foreground;
        }


        if (Input.GetKey(KeyCode.T))
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                layer.TransitionToTexture(texture, speed,smooth);
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                layer.SetTexture(texture);
            }
        }

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerTesting : MonoBehaviour
{
    BCFC bgcontroller;
    BCFC.LAYER test;

    public Texture texture;
    //public MovieTexture mov;
    public float speed;
    public bool smooth;

    BCFC.LAYER layer;

    // Start is called before the first frame update
    void Start()
    {
        bgcontroller = BCFC.instance;
        layer = null;
    }

    // Update is called once per frame
    void Update()
    {
        //BCFC.LAYER layer = null; HAY QUE SER HIJODEPUTA

        if (Input.GetKeyDown(KeyCode.Q))
        {
            //layer.CreateNewActiveImage();
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
            if (Input.GetKeyDown(KeyCode.B))
            {
                Debug.Log(texture.name);
                layer.SetTexture(texture);
            }
        }

    }
}

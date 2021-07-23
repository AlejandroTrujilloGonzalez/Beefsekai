using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class BackGround
{

    public string bgName;
    [HideInInspector]public RectTransform root;

    public bool isMultiLayerBG { get { return bgRenderers.renderer == null; } }


    public BackGround(string _name)
    {
        BackGroundController bg = BackGroundController.instance;
        GameObject prefab = Resources.Load("Art/Temporal/BGContenedor[" + _name + "]") as GameObject;//Hay que organizar esto si cambiamos las carpetas
        GameObject ob = GameObject.Instantiate(prefab, bg.backgroundPanel);

        root = ob.GetComponent<RectTransform>();
        bgName = _name;


        bgRenderers.renderer = ob.GetComponentInChildren<RawImage>();
        if (isMultiLayerBG)
        {
            bgRenderers.bgRenderer = ob.transform.Find("BGImage").GetComponent<Image>();
        }

    }

    [System.Serializable]
    public class BGRenderers
    {
        public RawImage renderer;

        public Image bgRenderer;
    }

    public BGRenderers bgRenderers = new BGRenderers();



}

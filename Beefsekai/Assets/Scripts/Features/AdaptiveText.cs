using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AdaptiveText : MonoBehaviour
{

    TMP_Text txt;

    public bool continualUpdate = true;

    public int fontSizeAtDefaultResolution = 30;

    public static float defaultResolution = 2525f;
    
    // Start is called before the first frame update
    void Start()
    {
        /*print(Screen.height + Screen.width); Esto se utiliza para averiguar el numero sumatorio de la reso9lucion en la que estamos desarrollando, luego puede eliminarse, sustituyendola en el codigo en el valor de defaultResolution */
        
        txt = GetComponent<TMP_Text>();

        if(continualUpdate)
        {
            InvokeRepeating("Adjust", 0f, Random.Range(0.3f, 1f));
        }
        else
        {
            Adjust();
            enabled = false;
        }
    }

    // Update is called once per frame
void Adjust()
    {
        if (!enabled || !gameObject.activeInHierarchy)
            return;

        float totalCurrentRes = Screen.height + Screen.width;

        float perc = totalCurrentRes / defaultResolution;

        int fontSize = Mathf.RoundToInt((float)fontSizeAtDefaultResolution * perc);

        txt.fontSize = fontSize;
    }
}

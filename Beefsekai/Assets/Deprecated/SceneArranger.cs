using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneArranger : MonoBehaviour
{
    [SerializeField]
    List<Image> pnjs;
    int activos;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < pnjs.Count; i++)
        {
            if (pnjs[i].isActiveAndEnabled)
            {
                activos++;
            }
        }
        Setter(activos);
    }

    void Setter(int actores)
    {
        switch (actores)
        {
            case 0:
                break;

            case 1:
                pnjs[0].rectTransform.position = new Vector3(0,pnjs[0].rectTransform.rect.height / 2, 0);
                break;

            case 2:
                pnjs[0].rectTransform.position = new Vector3(Screen.width * 0.33f, pnjs[0].rectTransform.rect.height / 2, 0);
                pnjs[1].rectTransform.position = new Vector3(Screen.width * 0.66f, pnjs[0].rectTransform.rect.height / 2, 0);
                break;

            case 3:
                pnjs[0].rectTransform.position = new Vector3(Screen.width * 0.25f, pnjs[0].rectTransform.rect.height / 2, 0);
                pnjs[1].rectTransform.position = new Vector3(Screen.width * 0.5f, pnjs[0].rectTransform.rect.height / 2, 0);
                pnjs[2].rectTransform.position = new Vector3(Screen.width * 0.75f, pnjs[0].rectTransform.rect.height / 2, 0);
                break;

            case 4:
                for (int i = 0; i < actores; i++)
                {
                    pnjs[i].rectTransform.position = new Vector3(Screen.width * (0.2f * (i + 1)), pnjs[i].rectTransform.rect.height / 2, 0);
                }
                break;

        }
    }
}

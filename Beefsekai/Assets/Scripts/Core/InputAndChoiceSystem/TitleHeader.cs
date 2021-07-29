using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TitleHeader : MonoBehaviour
{
    public Image banner;
    public TMP_Text tittleText;
    public string tittle { get { return tittleText.text; } set { tittleText.text = value; } }



    public enum DISPLAY_METHOD
    {
        instant,
        slowFade,
        typeWriter,
        floatingSlowFade
    }

    public DISPLAY_METHOD displayMethod = DISPLAY_METHOD.instant;
    public float fadeSpeed = 1;

    public void Show(string displayTittle)
    {
        tittle = displayTittle;

        if (isRevealing)
        {
            StopCoroutine(revealing);
        }

        if (!cachedBannerPos)
        {
            cachedBannerOriginalPosition = banner.transform.position;
        }

        revealing = StartCoroutine(Revealing());

    }

    public void Hide()
    {
        if (isRevealing)
        {
            StopCoroutine(revealing);
        }
        revealing = null;

        banner.enabled = false;
        tittleText.enabled = false;

        if (cachedBannerPos)
        {
            banner.transform.position = cachedBannerOriginalPosition;
        }
    }

    public bool isRevealing { get { return revealing != null; } }
    Coroutine revealing = null;

    IEnumerator Revealing()
    {
        banner.enabled = true;
        tittleText.enabled = true;

        switch (displayMethod)
        {
            case DISPLAY_METHOD.instant:
                banner.color = GlobalFunctions.SetAlpha(banner.color, 1);
                tittleText.color = GlobalFunctions.SetAlpha(banner.color, 1);
                break;
            case DISPLAY_METHOD.slowFade:
                yield return SlowFade();
                break;
            case DISPLAY_METHOD.floatingSlowFade:
                yield return FloatingSlowFade();
                break;
            case DISPLAY_METHOD.typeWriter:
                yield return TypeWriter();
                break;
            default:
                break;
        }


        revealing = null;

    }


    IEnumerator SlowFade()
    {
        banner.color = GlobalFunctions.SetAlpha(banner.color, 0);
        tittleText.color = GlobalFunctions.SetAlpha(banner.color, 0);
        while (banner.color.a < 1)
        {
            banner.color = GlobalFunctions.SetAlpha(banner.color, Mathf.MoveTowards(banner.color.a, 1, fadeSpeed * Time.unscaledDeltaTime));
            tittleText.color = GlobalFunctions.SetAlpha(banner.color, banner.color.a);
            yield return new WaitForEndOfFrame();
        }

    }

    bool cachedBannerPos = false;
    Vector3 cachedBannerOriginalPosition = Vector3.zero;
    IEnumerator FloatingSlowFade()
    {
        banner.color = GlobalFunctions.SetAlpha(banner.color, 0);
        tittleText.color = GlobalFunctions.SetAlpha(tittleText.color, 0);

        float amount = 25f * ((float)Screen.height / 720f);
        Vector3 downPos = new Vector3(0, amount, 0);
        banner.transform.position = cachedBannerOriginalPosition - downPos;

        while (banner.color.a < 1 || banner.transform.position != cachedBannerOriginalPosition)
        {
            banner.color = GlobalFunctions.SetAlpha(banner.color, Mathf.MoveTowards(banner.color.a, 1, fadeSpeed * Time.unscaledDeltaTime));
            tittleText.color = GlobalFunctions.SetAlpha(banner.color, banner.color.a);

            banner.transform.position = Vector3.MoveTowards(banner.transform.position, cachedBannerOriginalPosition, 55 * fadeSpeed * Time.unscaledDeltaTime);
            yield return new WaitForEndOfFrame();
        }


    }

    IEnumerator TypeWriter()
    {
        banner.color = GlobalFunctions.SetAlpha(banner.color, 1);
        tittleText.color = GlobalFunctions.SetAlpha(tittleText.color, 1);
        TextArchitect architect = new TextArchitect(tittleText, tittle);
        while (architect.isConstructing)
        {
            yield return new WaitForEndOfFrame();
        }
    }


}

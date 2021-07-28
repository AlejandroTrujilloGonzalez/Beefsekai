using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagManager : MonoBehaviour
{

    public static void Inject(ref string s)
    {
        if (!s.Contains("["))
            return;

        //Replace the mainCharName tag with the actual name of the main character
        s = s.Replace("[mainCharName]", "Furro"); //Temporal, se debe cargar el nombre del guardado

        s = s.Replace("[curHolyRelic]", "Divine Arc");
    }

    public static string[] SplitByTags(string targetText)
    {
        return targetText.Split(new char[2] { '<','>' });
    }
}

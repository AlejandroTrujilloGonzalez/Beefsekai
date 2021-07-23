using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackGroundController : MonoBehaviour
{

    public static BackGroundController instance;

    public RectTransform backgroundPanel;
    public List<BackGround> backGrounds = new List<BackGround>();
    public Dictionary<string, int> backgroundDictionary = new Dictionary<string, int>();


    private void Awake()
    {
        instance = this;
    }

    public BackGround GetBackground(string backGroundName, bool createBackgroundIfDoesNotExist = true)
    {
        int index = -1;
        if (backgroundDictionary.TryGetValue(backGroundName, out index))
        {
            return backGrounds[index];
        }
        else if (createBackgroundIfDoesNotExist)
        {
            return CreateBackground(backGroundName);
        }

        return null;

    }

    public BackGround CreateBackground(string backgroundName)
    {
        BackGround newBG = new BackGround(backgroundName);
        backgroundDictionary.Add(backgroundName, backGrounds.Count);
        backGrounds.Add(newBG);

        return newBG;
    }


}

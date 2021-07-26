using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NovelController : MonoBehaviour
{

    List<string> data = new List<string>();

    int progress = 0;
    // Start is called before the first frame update
    void Start()
    {
        LoadChapterFile("chapter0_start");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            HandLeLine(data[progress]);
            progress++;
        }
    }

    public void LoadChapterFile(string fileName)
    {
        data = FileManager.LoadFile(FileManager.savPath + "Resources/Story/" + fileName);
        progress = 0;
        cachedLastSpeaker = "";
    }

   // string cachedLastSpeaker = "";
    void HandLeLine(string line)
    {
        string[] dialogueAndActions = line.Split('"');

        if (dialogueAndActions.Length == 3)
        {
            HandleDialogue(dialogueAndActions[0], dialogueAndActions[1]);

            HandleEventsFromLine(dialogueAndActions[2]);
        }
        else
        {
            HandleEventsFromLine(dialogueAndActions[0]);
        }
    }

    string cachedLastSpeaker = "";

    void HandleDialogue(string dialogueDetails, string dialogue)
    {
        string speaker = cachedLastSpeaker;

        bool additive = dialogueDetails.Contains("+");

        if (additive)
            dialogueDetails = dialogueDetails.Remove(dialogueDetails.Length - 1);

        if(dialogueDetails.Length > 0)
        {
            if (dialogueDetails[dialogueDetails.Length - 1] == ' ')
                dialogueDetails = dialogueDetails.Remove(dialogueDetails.Length - 1);

            speaker = dialogueDetails;

            cachedLastSpeaker = speaker;
        }

        if (speaker != "narrator")
        {
            Character character = CharacterManager.instance.GetCharacters(speaker);

            character.Say(dialogue, additive);
        }
        else
        {
            DialogueSystem.instance.Say(dialogue, speaker, additive);
        }
    }

    void HandleEventsFromLine(string events)
    {

        string actions = events.Split(' ');

        foreach(string action in actions)
        {
            HandleAction(action);
        }
    }
    
    HandleAction(string action)
    {
        print("Handle events [" + action + "]");

        string[] data = action.Split('(', ')');

        if (data[0] == "setBackground")
        {
            Command_SetLayerImage(data[1], BFCF.instance.background);
            return;
        }

        if (data[0] == "setCinematic")
        {
            Command_SetLayerImage(data[1], BFCF.instance.cinematic);
            return;
        }

        if (data[0] == "setForeground")
        {
            Command_SetLayerImage(data[1], BFCF.instance.foreground);
            return;
        }

        if(data[0] == "playSound")
        {
            Command_PlaySound(data[1]);
            return;
        }

        if (data[0] == "playMusic")
        {
            Command_PlayMusic(data[1]);
            return;
        }

        if (data[0] == "move")
        {
            Command_MoveCharacter(data[1]);
            return;
        }

        if (data[0] == "setPosition")
        {
            Command_SetPosition(data[1]);
            return;
        }

        if (data[0] == "changeExpression")
        {
            Command_ChangeExpression(data[1]);
            return;
        }

    }

    void Command_SetLayerImage(string data, BCFC.LAYER layer)
    {
        string texName = data.Contains(",") ? data.Split(',')[0] : data;

        Texture2D tex = texName == "null" ? null : Resources.Load("Images/UI/Backdrops/" + texName) as Texture2D;

        float spd = 2f;

        bool smooth = false;

        if(data.Contains(","))
        {
            string[] parameters = data.Split(',');

            foreach(string p in parameters)
            {
                float fVal = 0;

                bool bVal = false;

                if (float.TryParse(p, out fVal))
                {
                    spd = fVal; continue;
                }


                if (bool.TryParse(p, out bVal))
                {
                    smooth = bVal; continue;
                }

            }
        }

        layer.TransitionToTexture(tex, spd, smooth);
    }

    void Command_PlaySound()
    {
        AudioClip clip = Resources.Load("Audio/SFX/" + data) as AudioClip;

        if (clip != null)
            AudioManager.instance.PlaySFX(clip);

        else
            Debug.LogError("clip does not exist - " + data);
    }

    void Command_PlayMusic()
    {
        AudioClip clip = Resources.Load("Audio/Music/" + data) as AudioClip;

        if (clip != null)
            AudioManager.instance.PlaySong(clip);

        else
            Debug.LogError("clip does not exist - " + data);
    }

   void Command_MoveCharacter(string data)
    {
        string[] parameters = data.Split(',');

        string character = parameters[0];

        float locationX = float.Parse(parameters[1]);

        float locationY = float.Parse(parameters[2]);

        float speed = parameters.Length >= 4 ? float.Parse(parameters[3]) : 1f;

        bool smooth = parameters.Length == 5 ? bool.Parse(parameters[4]) : false;

        Character c = CharacterManager.instance.GetCharacters(character);

        c.MoveTo(new Vector2(locationX, locationY), , smooth);
    }

    void Command_SetPosition(string data)
    {
        string[] parameters = data.Split(',');

        string character = parameters[0];

        float locationX = float.Parse(parameters[1]);

        float locationY = float.Parse(parameters[2]);

        Character c = CharacterManager.instance.GetCharacters(character);

        c.SetPosition(new Vector2(locationX, locationY));
    }

    void Command_ChangeExpression(string data)
    {
        string[] parameters = data.Split(',');

        string character = parameters[0];

        string region = parameters[1];

        string expression = parameters[2];

        float speed = parameters.Length == 4 ? float.Parse(parameters[3]) : 1f;

        Character c = CharacterManager.instance.GetCharacters(character);

        Sprite sprite = c.GetSprite(expression);

        if (region.ToLower() == "body")
        {

            c.TransitionBody(sprite, speed, false);
        }

        if(region.ToLower() == "body")
        {

            c.TransitionExpression(sprite, speed, false);
        }

    }
}

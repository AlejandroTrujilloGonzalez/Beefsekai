using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CharacterDialogueDetails : MonoBehaviour
{
    public static CharacterDialogueDetails instance;
    public static string dataFile = "Characters/CharacterDialogueDetails";

    public Dictionary<string, CDD> characterVariables = new Dictionary<string, CDD>();
    public Dictionary<string, string> characterNicknameDictionary = new Dictionary<string, string>();

    void Awake()
    {
        instance = this;

        Reload();
    }

    // Start is called before the first frame update
    public void Reload()
    {
        List<string> data = FileManager.ReadTextAsset(Resources.Load<TextAsset>(dataFile));

        for(int i = 1; i < data.Count; i++)
        {
            string line = data[i];
            //each element is split by a double space.
            string[] parts = line.Split(new string[1] { "  " }, System.StringSplitOptions.RemoveEmptyEntries);
            CDD cd = new CDD("");

            for(int a = 0; a < parts.Length; a++)
            {
                string[] titleAndValue = parts[a].Split('=');
                string title = titleAndValue[0];
                string value = titleAndValue[1];

                switch(title)
                {
                    case "name":
                        cd.name = value;
                        break;
                    case "nickname":
                        cd.nickName = value;
                        break;
                    case "name color":
                        cd.nameColor = string.Format("<color={0}>", value);
                        break;
                    case "dialogue font":
                        cd.speechFont = Resources.Load<TMP_FontAsset>("Fonts & Materials/" + value);
                        if (cd.speechFont == null)
                            return;
                            //cd.speechFont = TMP_FontAsset.defaultFontAsset;
                        break;
                }
            }

            //all values are set, add the character to the data lists so it can be accessed quickly.
            characterNicknameDictionary.Add(cd.nickName, cd.name);
            characterVariables.Add(cd.name, cd);
        }
    }

    /// <summary>
    /// Get the details for the character by this name or nickname.
    /// </summary>
    /// <param name="characterName"></param>
    /// <returns></returns>
    public CDD GetDetailsForCharacter(string characterName)
    {
        string actualCharacterName = "";

        //if this is not a nickname, this is the actual name
        if (!characterNicknameDictionary.TryGetValue(characterName, out actualCharacterName))
            actualCharacterName = characterName;

        CDD retval = null;
        if (characterVariables.TryGetValue(actualCharacterName, out retval))
        {
            return retval;
        }
        else
        {
            retval = new CDD(actualCharacterName);
            return retval;
        }
    }

    /// <summary>
    /// Character Dialogue Details. Information to be used when a character speaks.
    /// </summary>
    [System.Serializable]
    public class CDD
    {
        public string name = "";
        public string nickName = "";
        public string nameColor = "<color=#FFFFFFFF>";

        public TMP_FontAsset speechFont = null;

        public CDD(string characterName)
        {
            this.name = characterName;

            //get the font for this character or use the default.
            //if this is the main character whose name will be different on different game files, it needs a specific font file.
            if (GAMEFILE.activeFile != null)
            {
                if (this.name == GAMEFILE.activeFile.playerName)
                {
                    nameColor = "<color=#00FFFFFF>";//some color for the main character. Blue green
                    speechFont = Resources.Load<TMP_FontAsset>("Fonts & Materials/_Player");//load the player's font.
                }
            }

            if (speechFont == null)
                return;
                //speechFont = TMP_FontAsset.defaultFontAsset;
        }
    }
}

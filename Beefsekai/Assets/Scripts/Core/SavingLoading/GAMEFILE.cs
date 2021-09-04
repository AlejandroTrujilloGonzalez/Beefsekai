using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GAMEFILE
{

    public static GAMEFILE activeFile = new GAMEFILE();

    public string chapterName;
    public int chapterProgress = 0;

    public string playerName = "";

    public string cachedLastSpeaker = "";

    public string currentTextSystemSpeakerDisplayText = "";
    public string currentTestSystemDisplayText = "";

    public List<CHARACTERDATA> charactersInScene = new List<CHARACTERDATA>();

    public Texture background = null;
    public Texture cinematic = null;
    public Texture foreground = null;

    public AudioClip music = null;

    public string modificationDate = "";
    public Texture2D previewImagePath = null;

    public string[] tempVals = new string[9];

    public int affFeliodora = 0;
    public int affGallahim = 0;
    public int affAsshimilos = 0;

    public GAMEFILE()//Faltan cosas a diferencia con el video
    {
        this.chapterName = "Chapter0_Start";
        this.chapterProgress = 0;
        this.cachedLastSpeaker = "";

        this.background = null;//OJO
        this.foreground = null;

        this.music = null;

        charactersInScene = new List<CHARACTERDATA>();

        tempVals = new string[9];

        this.affFeliodora = 0;
        this.affGallahim = 0;
        this.affAsshimilos = 0;
    }

    //Datos de los personajes. Entiendo que aqui deberiamos meter tambien el nivel de afinidad
    [System.Serializable]
    public class CHARACTERDATA
    {
        public string characterName = "";
        public string displayName = "";
        public bool enabled = true;
        public string facialExpression = "";
        public string bodyExpression = "";
        public bool facingLeft = true;
        public Vector2 position = Vector2.zero;



        public CHARACTERDATA(Character character)
        {
            this.characterName = character.characterName;
            this.displayName = character.displayName;
            this.enabled = character.enabled;
            this.facialExpression = character.renderers.expressionRenderer.sprite.name;
            this.bodyExpression = character.renderers.bodyRender.sprite.name;
            this.facingLeft = character.isFacingLeft;
            this.position = character._targetPosition;
        }
    }
}

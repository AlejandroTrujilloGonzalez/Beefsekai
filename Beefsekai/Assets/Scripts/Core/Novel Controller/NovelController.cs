using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NovelController : MonoBehaviour
{

    public static NovelController instance;

    List<string> data = new List<string>();

    int progress = 0;

    void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        LoadChapterFile("Chapter0_Start");
    }

    // Update is called once per frame
    void Update()
    {
        //testing
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Next();
        }
    }

    public void LoadChapterFile(string fileName)
    {
        data = FileManager.LoadFile(FileManager.savPath + "Resources/Story/" + fileName);
        cachedLastSpeaker = "";

        if (handlingChapterFile != null)
            StopCoroutine(handlingChapterFile);
        handlingChapterFile = StartCoroutine(HandlingChapterFile());        
    }

    //Trigger que avanza el progreso por el chapter file
    bool _next = false;
    //Procede con la siguiente linea del capitulo o termina la linea
    public void Next()
    {
        _next = true;
    }

    Coroutine handlingChapterFile = null;
    IEnumerator HandlingChapterFile()
    {
        //progreso de las lineas en este capitulo
        int progress = 0;

        while (progress < data.Count)
        {
            if (_next)
            {
                HandLeLine(data[progress]);
                progress++;
                while (isHandlingLine)
                {
                    yield return new WaitForEndOfFrame();
                }
            }
            yield return new WaitForEndOfFrame();
        }
    }

   // string cachedLastSpeaker = "";
    void HandLeLine(string rawLine)
    {
        ChapterLineManager.LINE line = ChapterLineManager.Interpret(rawLine);

        //Tratamos la linea. Esto requiere un bucle lleno de esperas para los inputs ya que la linea consiste en multiple segmentos que deben tratarse individualmente
        StopHandlingLine();
        handlingLine = StartCoroutine(HandlingLine(line));
    }

    void StopHandlingLine()
    {
        if (isHandlingLine)
            StopCoroutine(handlingLine);
        handlingLine = null;
    }

    [HideInInspector]
    public string cachedLastSpeaker = "";

    public bool isHandlingLine {get { return handlingLine != null; } }
    Coroutine handlingLine = null;
    IEnumerator HandlingLine(ChapterLineManager.LINE line)
    {
        //el trigger "Next" controla el flujo del capitulo moviendose por la lineas del mismo, 
        //tambien controla el progreso de una linea por sus segmentos, por lo que debe reiniciarse.
        _next = false;
        int lineProgress = 0;

        while (lineProgress < line.segments.Count)
        {
            _next = false; //Se reinicia al principio de cada bucle
            ChapterLineManager.LINE.SEGMENT segment = line.segments[lineProgress];

            //Siempre corre el primer segmento automaticamente pero espera al trigger para procesar todos los segmentos.
            if (lineProgress > 0)
            {
                if (segment.trigger == ChapterLineManager.LINE.SEGMENT.TRIGGER.autoDelay)
                {
                    for (float timer = segment.autoDelay; timer >= 0; timer -= Time.deltaTime)
                    {
                        yield return new WaitForEndOfFrame();
                        if (_next)
                            break; //permite la terminacion de un delay cuando "next" es triggereao. Previene esperas no skipeables
                    }
                }
                else
                {
                    while (!_next)                    
                        yield return new WaitForEndOfFrame(); //espera hasta que el jugador diga de mover al siguiente segmento                    
                }
            }
            _next = false;

            //El segmento ya esta listo para ejecutarse.
            segment.Run();

            while (segment.isRunning)
            {
                yield return new WaitForEndOfFrame();
                //allow for autocompletion of the current segment for skipping purposes
                if (_next)
                {
                    //Completa el texto del primer avance y fuerza a terminar el segundo
                    if (!segment.architect.skip)
                        segment.architect.skip = true;
                    else
                        segment.ForceFinish();
                    _next = false;
                }
            }

            lineProgress++;

            yield return new WaitForEndOfFrame();
        }

        //La linea ha terminado. Maneja todas las acciones al final de la linea
        for (int i = 0; i < line.actions.Count; i++)
        {
            HandleAction(line.actions[i]);
        }
        handlingLine = null;
    }

    //ACTIONS

    public void HandleAction(string action)
    {
        print("execute command - " + action);
        string[] data = action.Split('(', ')');

        switch (data[0])
        {
            case "enter":
                Command_Enter(data[1]);
                break;

            case "exit":
                Command_Exit(data[1]);
                break;

            case ("setBackground"):
                Command_SetLayerImage(data[1], BCFC.instance.background);
                break;

         /*   case ("setCinematic"):
                Command_SetLayerImage(data[1], BCFC.instance.cinematic);
                break;                        */

            case ("setForeground"):
                Command_SetLayerImage(data[1], BCFC.instance.foreground);
                break;

            case ("playSound"):
                Command_PlaySound(data[1]);
                break;

            case ("playMusic"):
                Command_PlayMusic(data[1]);
                break;

            case ("move"):
                Command_MoveCharacter(data[1]);
                break;

            case ("setPosition"):
                Command_SetPosition(data[1]);
                break;

            case ("setFace"):
                Command_SetFace(data[1]);
                break;

            case ("setBody"):
                Command_SetBody(data[1]);
                break;

            case ("flip"):
                Command_Flip(data[1]);
                break;

            case ("faceLeft"):
                Command_FaceLeft(data[1]);
                break;

            case ("faceRight"):
                Command_FaceRight(data[1]);
                break;
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

    void Command_PlaySound(string data)
    {
        AudioClip clip = Resources.Load("Audio/SFX/" + data) as AudioClip;

        if (clip != null)
        {
            //AudioManager.instance.PlaySFX(clip);
            Debug.Log("PlaySoundMetodo");
        }
        else
            Debug.LogError("clip does not exist - " + data);

    }

    void Command_PlayMusic(string data)
    {
        AudioClip clip = Resources.Load("Audio/Music/" + data) as AudioClip;

        if (clip != null)
        {
            Debug.Log("PlayMusic metodo");
            //AudioManager.instance.PlaySong(clip);//Las cargas de música las podemos dejar para más adelante, estos errores se subsanarán en principio cuando implementemos el código del video de audiomanager
        }
        else
            Debug.LogError("clip does not exist - " + data);

    }

    void Command_MoveCharacter(string data)
    {
        string[] parameters = data.Split(',');
        string character = parameters[0];
        float locationX = float.Parse(parameters[1]);
        float locationY = parameters.Length >= 3 ? float.Parse(parameters[2]) : 0;
        float speed = parameters.Length >= 4 ? float.Parse(parameters[3]) : 7f;
        bool smooth = parameters.Length == 5 ? bool.Parse(parameters[4]) : true;

        Character c = CharacterManager.instance.GetCharacters(character);
        c.MoveTo(new Vector2(locationX, locationY), speed, smooth);
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

    void Command_SetFace(string data)
    {
        string[] parameters = data.Split(',');
        string character = parameters[0];
        string expression = parameters[1];
        float speed = parameters.Length == 3 ? float.Parse(parameters[2]) : 3f;

        Character c = CharacterManager.instance.GetCharacters(character);
        Sprite sprite = c.GetSprite(expression);

        c.TransitionExpression(sprite, speed, false);
    }

    void Command_SetBody(string data)
    {
        string[] parameters = data.Split(',');
        string character = parameters[0];
        string expression = parameters[1];
        float speed = parameters.Length == 3 ? float.Parse(parameters[2]) : 3f;

        Character c = CharacterManager.instance.GetCharacters(character);
        Sprite sprite = c.GetSprite(expression);

        c.TransitionBody(sprite, speed, false);
    }

    void Command_Flip(string data)
    {
        string[] characters = data.Split(';');

        foreach (string s in characters)
        {
            Character c = CharacterManager.instance.GetCharacters(s);
            c.Flip();
        }
    }

    void Command_FaceLeft(string data)
    {
        string[] characters = data.Split(';');

        foreach (string s in characters)
        {
            Character c = CharacterManager.instance.GetCharacters(s);
            c.FaceLeft();
        }
    }

    void Command_FaceRight(string data)
    {
        string[] characters = data.Split(';');

        foreach (string s in characters)
        {
            Character c = CharacterManager.instance.GetCharacters(s);
            c.FaceRight();
        }
    }

    void Command_Exit(string data)
    {
        string[] parameters = data.Split(',');
        string[] characters = parameters[0].Split(';');
        float speed = 3;
        bool smooth = false;
        for (int i = 1; i < parameters.Length; i++)
        {
            float fVal = 0; bool bVal = false;
            if (float.TryParse(parameters[i], out fVal))
            { speed = fVal; continue; }
            if (bool.TryParse(parameters[i], out bVal))
            { smooth = bVal; continue; }
        }

        foreach (string s in characters)
        {
            Character c = CharacterManager.instance.GetCharacters(s);
            c.FadeOut(speed, smooth);
        }
    }

    void Command_Enter(string data)
    {
        string[] parameters = data.Split(',');
        string[] characters = parameters[0].Split(';');
        float speed = 3;
        bool smooth = false;
        for (int i = 1; i < parameters.Length; i++)
        {
            float fVal = 0; bool bVal = false;
            if (float.TryParse(parameters[i], out fVal))
            { speed = fVal; continue; }
            if (bool.TryParse(parameters[i], out bVal))
            { smooth = bVal; continue; }
        }

        foreach (string s in characters)
        {
            Character c = CharacterManager.instance.GetCharacters(s, true, false);
            if (!c.enabled)
            {
                c.renderers.bodyRender.color = new Color(1, 1, 1, 0);
                c.renderers.expressionRenderer.color = new Color(1, 1, 1, 0);
                c.enabled = true;

                c.TransitionBody(c.renderers.bodyRender.sprite, speed, smooth);
                c.TransitionExpression(c.renderers.expressionRenderer.sprite, speed, smooth);
            }
            else
                c.FadeIn(speed, smooth);
        }
    }
}

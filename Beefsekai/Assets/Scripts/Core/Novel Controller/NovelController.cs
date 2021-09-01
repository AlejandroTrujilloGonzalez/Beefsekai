using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NovelController : MonoBehaviour
{
	public static NovelController instance;

	public GameSavePanel saveLoadPanel;

	/// <summary> The lines of data loaded directly from a chapter file.	/// </summary>
	List<string> data = new List<string>();

	void Awake()
	{
		instance = this;
	}

    public string activeGameFileName = "";

    GAMEFILE activeGameFile
    {
        get
        {
			return GAMEFILE.activeFile;
        }
        set
        {
			GAMEFILE.activeFile = value;
        }
    }

    string activeChapterFile = "";

    // Use this for initialization
    void Start()
	{
		saveLoadPanel.gameObject.SetActive(false);
        LoadGamefile(FileManager.LoadFile(FileManager.savPath + "savData/file.txt")[0]);

   //     foreach (var stringcosas in data)
   //     {
			//Debug.Log(stringcosas);
   //     }

	}

	//public bool encryptGameFile = false;

    public void LoadGamefile(string gameFileName)
    {
		activeGameFileName = gameFileName;

        string filePath = FileManager.savPath + "savData/GameFiles/" + activeGameFileName + ".txt";

        //En caso de no encontrar gameFiles, carga uno en blanco
        if (!System.IO.File.Exists(filePath))
        {
			activeGameFile = new GAMEFILE();
        }
        else
        {
			if (Encrpyterizador.instance.encryptedOn)
				activeGameFile = FileManager.LoadEncryptedJSON<GAMEFILE>(filePath, keys);
			else
				activeGameFile = FileManager.LoadJSON<GAMEFILE>(filePath);
		}

		//activeGameFile = FileManager.LoadJSON<GAMEFILE>(filePath);
		//activeGameFile = FileManager.LoadEncryptedJSON<GAMEFILE>(filePath, keys);

		//Carga el fichero
		TextAsset chapterAsset = Resources.Load<TextAsset>("Story/" + activeGameFile.chapterName);
		//data = FileManager.LoadFile(FileManager.savPath + "Resources/Story/" + activeGameFile.chapterName);//OJO ESTO FUNCIONABA
		data = FileManager.ReadTextAsset(chapterAsset);
		activeChapterFile = activeGameFile.chapterName;
        cachedLastSpeaker = activeGameFile.cachedLastSpeaker;      

        DialogueSystem.instance.Open(activeGameFile.currentTextSystemSpeakerDisplayText, activeGameFile.currentTestSystemDisplayText);

        //Carga todos los personajes en la escena
        for (int i = 0; i < activeGameFile.charactersInScene.Count; i++)
        {
            GAMEFILE.CHARACTERDATA data = activeGameFile.charactersInScene[i];
            Character character = CharacterManager.instance.CreateCharacter(data.characterName, data.enabled);
			//character.displayName = data.displayName;
			//character.canvasGroup.alpha = 1;//	COSAS QUE EL TIENE POR LA CARA Y NO ENSEÑA EN LOS VIDEOS
            character.SetBody(data.bodyExpression);
            character.SetExpression(data.facialExpression);
            if (data.facingLeft)
                character.FaceLeft();
            else
                character.FaceRight();
            character.SetPosition(data.position);
        }

        //Carga el background
        if (activeGameFile.background != null)
            BCFC.instance.background.SetTexture(activeGameFile.background);
        //if (activeGameFile.cinematic != null)
        //  BCFC.instance.background.SetTexture(activeGameFile.cinematic);
        if (activeGameFile.foreground != null)
            BCFC.instance.background.SetTexture(activeGameFile.foreground);//////////creo que intenta cargar esto y petardea a veces

		//Carga la musica
		// if (activeGameFile.music != null)
		//     AudioManager.instance.PlaySong(activeGameFile.music);

		//load the temporary variables back
		//CACHE.tempVals = activeGameFile.tempVals;///////////////Mas cosas que tiene por la cara
		CACHE.tempVals = activeGameFile.tempVals;

        if (handlingChapterFile != null)
            StopCoroutine(handlingChapterFile);
        handlingChapterFile = StartCoroutine(HandlingChapterFile());

        chapterProgress = activeGameFile.chapterProgress;
    }

    public void SaveGameFile()
    {
        string filePath = FileManager.savPath + "savData/GameFiles/" + activeGameFileName + ".txt";//sin el txt funcionaba TT

        activeGameFile.chapterName = activeChapterFile;
        activeGameFile.chapterProgress = chapterProgress;
        activeGameFile.cachedLastSpeaker = cachedLastSpeaker;

        activeGameFile.currentTextSystemSpeakerDisplayText = DialogueSystem.instance.speakerNameText.text;
        activeGameFile.currentTestSystemDisplayText = DialogueSystem.instance.speechText.text;

        //Reune todos los personajes y guarda sus stats
        activeGameFile.charactersInScene.Clear();
        for (int i = 0; i < CharacterManager.instance.characters.Count; i++)
        {
            Character character = CharacterManager.instance.characters[i];
            GAMEFILE.CHARACTERDATA data = new GAMEFILE.CHARACTERDATA(character);
			Debug.Log(character.renderers.bodyRender.name);
            activeGameFile.charactersInScene.Add(data);
        }

        //guarda el background
        BCFC b = BCFC.instance;
        activeGameFile.background = b.background.activeImage != null ? b.background.activeImage.texture : null;
        //activeGameFile.cinematic = b.cinematic.activeImage != null ? b.cinematic.activeImage.texture : null;
        activeGameFile.foreground = b.foreground.activeImage != null ? b.foreground.activeImage.texture : null;//PUEDE QUE ESTO PETARDEE

		//guarda la musica
		//activeGameFile.music = AudioManager.activeSong != null ? AudioManager.activeSong.clip : null;

		//Guardado de una imagen para el tema de guardar y cargar partida
		//string screenShotDirectory = "Assets/Resources/savData/previewImages/";
		string screenShotPath = FileManager.savPath + "savData/GameFiles/" + activeGameFileName + ".png";
		if (FileManager.TryCreateDirectoryFromPath(screenShotPath + ".png"))
        {
			GAMEFILE.activeFile.previewImagePath = ScreenCapture.CaptureScreenshotAsTexture();
			byte[] textureData = activeGameFile.previewImagePath.EncodeToPNG();
			FileManager.SaveComposingBytes(screenShotPath, textureData);
			//ScreenCapture.CaptureScreenshot(screenShotPath + ".png");
			//activeGameFile.previewImagePath = Resources.Load<Texture2D>("savData/previewImages/" + activeGameFileName + ".png");
        }

		//Guardado datos/fecha
		activeGameFile.modificationDate = System.DateTime.Now.ToString();

		//FileManager.SaveJSON(filePath, activeGameFile);
		//FileManager.SaveEncryptedJSON(filePath, activeGameFile, keys);//Faltan cosas a diferencia con el video
		if (Encrpyterizador.instance.encryptedOn)
			FileManager.SaveEncryptedJSON(filePath, activeGameFile, keys);
		else
			FileManager.SaveJSON(filePath, activeGameFile);
	}
	//Temporary
	//byte[] keys = new byte[3] { 23, 70, 194 };
	byte[] keys
    {
        get { return FileManager.keys; }
    }

	// Update is called once per frame
	void Update()
	{
		//Next
		if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			Next();
		}

        //save testing
        if (Input.GetKeyDown(KeyCode.S))
        {
            SaveGameFile();
        }

		//Back chustero
        if (chapterProgress >= 2)
        {
			if (Input.GetKeyDown(KeyCode.LeftArrow) && chapterProgress >= 2)
			{
				chapterProgress -= 2;
				Next();
			}
		}

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (ChoiceScreen.isWaitingForChoiceToBeMade)
            {
				return;
            }
            if (!saveLoadPanel.gameObject.activeInHierarchy)
            {
				saveLoadPanel.gameObject.SetActive(true);
				//animacion
				saveLoadPanel.LoadFilesOntoScreen(saveLoadPanel.currentSaveLoadPage);
			}
			else
            {
				//fadeoff
            }

        }


    }

	//COSAS DE AUTO//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public bool autoOn = false;
	public void StartAuto()
    {
		autoOn = true;
		StartCoroutine(Auto());
    }

	public void AutoOff()
    {
		autoOn = false;
    }

	IEnumerator Auto()
    {
        while (autoOn == true)
        {
			Next();
			yield return new WaitForSeconds(2f);
        }
    }
	//COSAS DE AUTO////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public void LoadChapterFile(string fileName)
	{
        activeChapterFile = fileName;

		//data = FileManager.LoadFile(FileManager.savPath + "Resources/Story/" + fileName);
		data = FileManager.ReadTextAsset(Resources.Load<TextAsset>($"Story/{fileName}"));
		cachedLastSpeaker = "";

		if (handlingChapterFile != null)
			StopCoroutine(handlingChapterFile);
		handlingChapterFile = StartCoroutine(HandlingChapterFile());

		//auto start the chapter.
		Next();
	}

	/// <summary>
	/// Trigger that advances the progress through a chapter file.
	/// </summary>
	bool _next = false;
	/// <summary>
	/// Procede to the next line of a chapter or finish the line right now.
	/// </summary>
	public void Next()
	{
		_next = true;
	}

	public bool isHandlingChapterFile { get { return handlingChapterFile != null; } }
	Coroutine handlingChapterFile = null;
	[HideInInspector] public int chapterProgress = 0;
	IEnumerator HandlingChapterFile()
	{
		//the progress through the lines in this chapter.
		chapterProgress = 0;

		while (chapterProgress < data.Count)
		{
			//we need a way of knowing when the player wants to advance. We need a "next" trigger. Not just a keypress. But something that can be triggerd
			//by a click or a keypress
			if (_next)
			{
				string line = data[chapterProgress];
				//this is a choice
				if (line.StartsWith("choice"))
				{
					yield return HandlingChoiceLine(line);
					chapterProgress++;
				}
				//this is a normal line of dialogue and actions.
				else
				{
					HandleLine(line);
					chapterProgress++;
					while (isHandlingLine)
					{
						yield return new WaitForEndOfFrame();
					}
				}
			}
			yield return new WaitForEndOfFrame();
		}

		handlingChapterFile = null;
	}

	IEnumerator HandlingChoiceLine(string line)
	{
		string title = line.Split('"')[1];
		List<string> choices = new List<string>();
		List<string> actions = new List<string>();

		bool gatheringChoices = true;
		while (gatheringChoices)
		{
			chapterProgress++;
			line = data[chapterProgress];

			if (line == "{")
				continue;

			line = line.Trim();//remove the tabs that have become quad spaces.

			if (line != "}")
			{
				choices.Add(line.Split('"')[1]);
				actions.Add(data[chapterProgress + 1].Replace("    ", ""));
				chapterProgress++;
			}
			else
			{
				gatheringChoices = false;
			}
		}

		//display choices
		if (choices.Count > 0)
		{
			ChoiceScreen.Show(title, choices.ToArray()); yield return new WaitForEndOfFrame(); 
			while (ChoiceScreen.isWaitingForChoiceToBeMade)
				yield return new WaitForEndOfFrame();

			//choice is made. execute the paired action.
			string action = actions[ChoiceScreen.lastChoiceMade.index];
			HandleLine(action);

			while (isHandlingLine)
				yield return new WaitForEndOfFrame();
		}
		else
		{
			Debug.LogError("Invalid choice operation. No choices were found.");
		}


	}

	void HandleLine(string rawLine)
	{
		ChapterLineManager.LINE line = ChapterLineManager.Interpret(rawLine);

		//now we need to handle the line. This requires a loop full of waiting for input since the line consists of multiple segments that have to be
		//handled individually.
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
	/// <summary> Used as a fallback when no speaker is given.</summary>
	public string cachedLastSpeaker = "";

	public bool isHandlingLine { get { return handlingLine != null; } }
	Coroutine handlingLine = null;
	IEnumerator HandlingLine(ChapterLineManager.LINE line)
	{
		//since the "next" trigger controls the flow of a chapter by moving through lines and yet also controls the progression through a line by
		//its segments, it must be reset.
		_next = false;
		int lineProgress = 0;//progress through the segments of a line.

		while (lineProgress < line.segments.Count)
		{
			_next = false;//reset at the start of each loop.
			ChapterLineManager.LINE.SEGMENT segment = line.segments[lineProgress];

			//always run the first segment automatically. But wait for the trigger on all proceding segments.
			if (lineProgress > 0)
			{
				if (segment.trigger == ChapterLineManager.LINE.SEGMENT.TRIGGER.autoDelay)
				{
					for (float timer = segment.autoDelay; timer >= 0; timer -= Time.deltaTime)
					{
						yield return new WaitForEndOfFrame();
						if (_next)
							break;//allow the termination of a delay when "next" is triggered. Prevents unskippable wait timers.
					}
				}
				else
				{
					while (!_next)
						yield return new WaitForEndOfFrame();//wait until the player says move to the next segment.
				}
			}
			_next = false;//next could have been triggered during an event above.

			//the segment now needs to build and run.
			segment.Run();

			while (segment.isRunning)
			{
				yield return new WaitForEndOfFrame();
				//allow for auto completion of the current segment for skipping purposes.
				if (_next)
				{
					//rapidly complete the text on first advance, force it to finish on the second.
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

		//Line is finished. Handle all the actions set at the end of the line.
		for (int i = 0; i < line.actions.Count; i++)
		{
			HandleAction(line.actions[i]);
		}

		handlingLine = null;
	}

	//ACTIONS
	//\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
	public void HandleAction(string action)
	{
		//print("execute command - " + action);
		string[] data = action.Split('(', ')');
		//int affinityNecesary = 0;
		switch (data[0])
		{
			case "enter":
				Command_Enter(data[1]);
				break;

			case "exit":
				Command_Exit(data[1]);
				break;

			case "setBackground":
				Command_SetLayerImage(data[1], BCFC.instance.background);
				break;

			//case "setCinematic":
			//	Command_SetLayerImage(data[1], BCFC.instance.cinematic);
			//	break;

			case "setForeground":
				Command_SetLayerImage(data[1], BCFC.instance.foreground);
				break;

			case "playSound":
				Command_PlaySound(data[1]);
				break;

			case "playMusic":
				Command_PlayMusic(data[1]);
				break;

			case "move":
				Command_MoveCharacter(data[1]);
				break;

			case "setPosition":
				Command_SetPosition(data[1]);
				break;

			case "setFace":
				Command_SetFace(data[1]);
				break;

			case "setBody":
				Command_SetBody(data[1]);
				break;

			case "flip":
				Command_Flip(data[1]);
				break;

			case "faceLeft":
				Command_FaceLeft(data[1]);
				break;

			case "faceRight":
				Command_FaceRight(data[1]);
				break;

			case "transBackground":
				Command_TransLayer(BCFC.instance.background, data[1]);
				break;

			//case "transCinematic":
			//	Command_TransLayer(BCFC.instance.cinematic, data[1]);
			//	break;

			case "transForeground":
				Command_TransLayer(BCFC.instance.foreground, data[1]);
				break;

			case "showScene":
				Command_ShowScene(data[1]);
				break;

			case "Load":
				Command_Load(data[1]);
				break;

			case "debug":
				Command_Debug();
				break;

			case "changeFeliodoraAffinity"://changeFeliodoraAffinity(sum)---> el string que va dentro suma o resta 1 dependiendo de lo que pongamos, "sum" para sumar "rest" para restar
				Command_SetAffinityValuesOfFeliodora(data[1]);
				break;

			case "changeGallimAffinity"://changeGallimAffinity(sum)--> En cuanto tengamos más personajes con afinidad siempre será igual, cambiando el nombre ---> changeNombreDePersonajeAffinity(Aqui dentro "sum" o "rest" para sumar o restar afinidad)
				Command_SetAffinityValuesOfGallim(data[1]);
				break;

			case "changeAffinityOf"://Otra forma de cambiar afinidad, sería por ejemplo(dentro del txt) changeAffinityOf(Feliodora,sum)--> Esto sumaria 1 a la afinidad con Feliodira, en contra --> changeAffinityOf(NombreDePersonaje,rest), esto restaria 1
				Command_ChangeAffinityOf(data[1]);
				break;

			case "getAffinity"://Solo devuelve afinidad
				Command_GetAffinityValues(data[1]);
				break;

			case "checkAffinity"://Dentro del txt para comparar tiene que ser el siguiente orden y sin separaciones entre las comillas-- > checkAffinity(NombreDePersonaje,NombredelArchivo,AfinidadNecesariaParaContinuar(int))
				Command_CheckAffinityNecesaryToContinue(data[1]);
				break;
		}

	}


	//AQUI EMPIEZAN LOS COMANDOS PARA EL INTERCAMBIO DE AFINIDADES////////////////////////////////////////////////////////////////////////////////////////
	//int valueAffinity = 0;
	//private int feliodoraAff = 0;
	//private int gallimAff = 0;

	void Command_ChangeAffinityOf(string data)//Con este método se pone el nombre del personaje("tiene que existir previamente") y añades sum o rest para cambiar su afinidad, creo que es más intuitivo que tener muchos métodos
    {
		string[] parameters = data.Split(',');
		string chacaName = parameters[0];
		string calc = parameters[1];

        switch (chacaName)
        {
			case "Feliodora":
                if (calc == "sum")
                {
					GAMEFILE.activeFile.affFeliodora += 1;
				}
                else if (calc == "rest")
                {
					GAMEFILE.activeFile.affFeliodora -= 1;
					//feliodoraAff -= 1;
					//PlayerPrefs.SetInt("Feliodora", feliodoraAff);
				}
				break;

			case "Gallahim":
				if (calc == "sum")
				{
					GAMEFILE.activeFile.affGallahim += 1;
				}
				else if (calc == "rest")
				{
					GAMEFILE.activeFile.affGallahim -= 1;
				}
				break;


            default:
                break;
        }

    }

	void Command_SetAffinityValuesOfFeliodora(string calc)
	{
		if (calc == "sum")
		{
			GAMEFILE.activeFile.affFeliodora += 1;
		}
		else if (calc == "rest")
		{
			GAMEFILE.activeFile.affFeliodora -= 1;
		}

	}
	void Command_SetAffinityValuesOfGallim(string calc)
	{
		if (calc == "sum")
		{
			GAMEFILE.activeFile.affGallahim += 1;
		}
		else if (calc == "rest")
		{
			GAMEFILE.activeFile.affGallahim -= 1;
		}

	}

	void Command_CheckAffinityNecesaryToContinue(string data)
	{
		string[] parameters = data.Split(',');
		string characName = parameters[0];
		string txtName = parameters[1];
		int affinityNecesary = int.Parse(parameters[2]);

		Debug.Log(characName);
		Debug.Log(txtName);
		Debug.Log(affinityNecesary);

		switch (characName)
        {
			case "Feliodora":
                if (affinityNecesary <= GAMEFILE.activeFile.affFeliodora)
                {
					Command_Load(txtName);
                }
                else
                {
					return;
                }
				break;

			case "Gallahim":
                if (affinityNecesary <= GAMEFILE.activeFile.affGallahim)
                {
					Command_Load(txtName);
                }
                else
                {
					return;
                }
				break;

            default:
				Debug.Log("No tienes la afinidad suficiente con" + characName + " Necesitas: " + affinityNecesary);
                break;
        }
    }

	void Command_GetAffinityValues(string characName)
	{
        switch (characName)
        {
			case ("Feliodora"):
				Debug.Log("La afinidad de " + characName + "es" + GAMEFILE.activeFile.affFeliodora);
				break;

			case ("Gallahim"):
				Debug.Log("La afinidad de " + characName + "es" + GAMEFILE.activeFile.affGallahim);
				break;
            default:
                break;
        }
    }

	void Command_Debug()
	{
		Debug.Log("DebugTool");
	}

	//PARRIBA FUNCIONES DE PRUEBA PARA TEMA AFINIDAD DE PERSONAJES/////////////////////////////////////////////////////////////////////////////////////////////////

	void Command_Load(string chapterName)
	{
		NovelController.instance.LoadChapterFile(chapterName);
	}

	void Command_SetLayerImage(string data, BCFC.LAYER layer)
	{
		string texName = data.Contains(",") ? data.Split(',')[0] : data;
		Texture2D tex = texName == "null" ? null : Resources.Load("Art/Images/UI/Backdrops/" + texName) as Texture2D;
		float spd = 2f;
		bool smooth = false;

		if (data.Contains(","))
		{
			string[] parameters = data.Split(',');
			foreach (string p in parameters)
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
			Debug.Log("cpsas de audio");
		}
		else
			Debug.LogError("Clip does not exist - " + data);
	}

	void Command_PlayMusic(string data)
	{
		AudioClip clip = Resources.Load("Audio/Music/" + data) as AudioClip;

		//AudioManager.instance.PlaySong(clip);
	}

	void Command_MoveCharacter(string data)
	{
		string[] parameters = data.Split(',');
		string character = parameters[0];
		float locationX = float.Parse(parameters[1], System.Globalization.NumberStyles.Float, new System.Globalization.CultureInfo("en-US"));
		float locationY = parameters.Length >= 3 ? float.Parse(parameters[2], System.Globalization.NumberStyles.Float, new System.Globalization.CultureInfo("en-US")) : 0;
		float speed = parameters.Length >= 4 ? float.Parse(parameters[3], System.Globalization.NumberStyles.Float, new System.Globalization.CultureInfo("en-US")) : 7f;
		bool smooth = parameters.Length == 5 ? bool.Parse(parameters[4]) : true;

		Character c = CharacterManager.instance.GetCharacters(character);
		c.MoveTo(new Vector2(locationX, locationY), speed, smooth);
	}

	void Command_SetPosition(string data)
	{
		string[] parameters = data.Split(',');
		string character = parameters[0];
		float locationX = float.Parse(parameters[1], System.Globalization.NumberStyles.Float, new System.Globalization.CultureInfo("en-US"));
		float locationY = parameters.Length == 3 ? float.Parse(parameters[2], System.Globalization.NumberStyles.Float, new System.Globalization.CultureInfo("en-US")) : 0;

		Character c = CharacterManager.instance.GetCharacters(character);
		c.SetPosition(new Vector2(locationX, locationY));

		print("set " + c.characterName + " position to " + locationX + "," + locationY);
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

	void Command_TransLayer(BCFC.LAYER layer, string data)
	{
		string[] parameters = data.Split(',');

		string texName = parameters[0];
		string transTexName = parameters[1];
		Texture2D tex = texName == "null" ? null : Resources.Load("Images/UI/Backdrops/" + texName) as Texture2D;
		Texture2D transTex = Resources.Load("Images/TransitionEffects/" + transTexName) as Texture2D;

		float spd = 2f;
		bool smooth = false;

		for (int i = 2; i < parameters.Length; i++)
		{
			string p = parameters[i];
			float fVal = 0;
			bool bVal = false;
			if (float.TryParse(p, out fVal))
			{ spd = fVal; continue; }
			if (bool.TryParse(p, out bVal))
			{ smooth = bVal; continue; }
		}

		TransitionMaster.TransitionLayer(layer, tex, transTex, spd, smooth);
	}

	void Command_ShowScene(string data)
	{
		string[] parameters = data.Split(',');
		bool show = bool.Parse(parameters[0]);
		string texName = parameters[1];
		Texture2D transTex = Resources.Load("Images/TransitionEffects/" + texName) as Texture2D;
		float spd = 2f;
		bool smooth = false;

		for (int i = 2; i < parameters.Length; i++)
		{
			string p = parameters[i];
			float fVal = 0;
			bool bVal = false;
			if (float.TryParse(p, out fVal))
			{ spd = fVal; continue; }
			if (bool.TryParse(p, out bVal))
			{ smooth = bVal; continue; }
		}

		TransitionMaster.ShowScene(show, spd, smooth, transTex);
	}
}

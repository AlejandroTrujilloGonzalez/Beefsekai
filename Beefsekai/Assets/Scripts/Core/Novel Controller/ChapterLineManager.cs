using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChapterLineManager : MonoBehaviour
{
    //Clase para tratar las lineas de cada capitulo

    //Convierte un string en una linea interpretable
    public static LINE Interpret(string rawLine)
    {
        return new LINE(rawLine);
    }

    //Cada linea esta compuesta por segmentos
    public class LINE
    {
        public string speaker = "";

        public List<SEGMENT> segments = new List<SEGMENT>();
        public List<string> actions = new List<string>();

        public string lastSegmentsWholeDialog = "";

        public LINE(string rawLine)
        {
            //Separar dialogo de acciones
            string[] dialogueAndActions = rawLine.Split('"');
            char actionSplitter = ' ';
            string[] actionsArr = dialogueAndActions.Length == 3 ? dialogueAndActions[2].Split(actionSplitter) : dialogueAndActions[0].Split(actionSplitter);

            if (dialogueAndActions.Length == 3) //Contiene dialogo
            {
                speaker = dialogueAndActions[0] == "" ? NovelController.instance.cachedLastSpeaker : dialogueAndActions[0];
                if (speaker[speaker.Length - 1] == ' ')
                    speaker = speaker.Remove(speaker.Length - 1);

                NovelController.instance.cachedLastSpeaker = speaker;

                //segmenta el dialogo
                SegmentDialogue(dialogueAndActions[1]);
            }
            //se capturan las acciones. se tratan mas tarde
            for (int i = 0; i < actionsArr.Length; i++)
            {
                actions.Add(actionsArr[i]);
            }

            //linea segmentada y acciones cargadas, lista para ser usada


        }

        //Segmenta el dialogo en una lista de partes individuales seperadas por input delays
        void SegmentDialogue(string dialogue)
        {
            segments.Clear();
            string[] parts = dialogue.Split('{', '}');

            for (int i = 0; i < parts.Length; i++)
            {
                SEGMENT segment = new SEGMENT();
                bool isOdd = i % 2 != 0;

                //Los comandos tienen indices impares, el dialogo siempre par
                if (isOdd)
                {
                    //Los comandos y los datos se separan por espacios
                    string[] commandData = parts[i].Split(' ');
                    switch (commandData[0])
                    {
                        case "c": //espera un input y limpia
                            segment.trigger = SEGMENT.TRIGGER.waitForClick;
                            break;
                        case "a": //espera un input y se añade al texto
                            segment.trigger = SEGMENT.TRIGGER.waitForClick;
                            //añadir al texto requiere buscar el texto del segmento previo para que sea el texto previo
                            segment.pretext = segments.Count > 0 ? segments[segments.Count - 1].dialogue : "";
                            break;
                        case "w": //espera un tiempo y luego limpia
                            segment.trigger = SEGMENT.TRIGGER.autoDelay;
                            segment.autoDelay = float.Parse(commandData[1]);
                            break;
                        case "wa": //espera un tiempo y se añade al texto
                            segment.trigger = SEGMENT.TRIGGER.autoDelay;
                            segment.autoDelay = float.Parse(commandData[1]);
                            //añadir al texto requiere buscar el texto del segmento previo para que sea el texto previo
                            segment.pretext = segments.Count > 0 ? segments[segments.Count - 1].dialogue : "";
                            break;
                    }
                    i++; //se incrementa para pasar el comando y salta al siguiente trozo de dialogo
                }

                segment.dialogue = parts[i];
                segment.line = this;

                segments.Add(segment);
            }
        }

        public class SEGMENT
        {
            public LINE line;
            public string dialogue = "";
            public string pretext = "";
            //public enum TRIGGER {waitClick, waitClickClear, autoDelay};
            public enum TRIGGER { waitForClick, autoDelay }

            //public TRIGGER trigger = TRIGGER.waitClickClear;
            public TRIGGER trigger = TRIGGER.waitForClick;

            public float autoDelay = 0f;

            //Ejecuta el segmento
            public void Run()
            {
                if (running != null)
                    NovelController.instance.StopCoroutine(running);
                running = NovelController.instance.StartCoroutine(Running());
            }

            //Corrutina para controlar la ejecucion del segmento
            public bool isRunning { get { return running != null; } }
            Coroutine running = null;
            public TextArchitect architect = null;
            List<string> allCurrentlyExecutedEvents = new List<string>();
            IEnumerator Running()
            {

                allCurrentlyExecutedEvents.Clear();
                //Take care of any tags that must be injected into the dialogue before we worry about events
                TagManager.Inject(ref dialogue);

                //split the dialogue by the event characters
                string[] parts = dialogue.Split('[', ']');

                for (int i = 0; i < parts.Length; i++)
                {
                    //events will always be odd indexed. Execute an event
                    bool isOdd = i % 2 != 0;
                    if (isOdd)
                    {
                        DialogueEvents.HandleEvent(parts[i], this);
                        allCurrentlyExecutedEvents.Add(parts[i]);
                        i++;
                    }

                    string targDialogue = parts[i];

                    if (line.speaker != "narrator")
                    {
                        Character character = CharacterManager.instance.GetCharacters(line.speaker);
                        character.Say(targDialogue, i > 0 ? true : pretext != "");
                    }
                    else
                    {
                        DialogueSystem.instance.Say(targDialogue, line.speaker, i > 0 ? true : pretext != "");
                    }

                    //espera mientras que el dialogueSystem contruye el dialogo
                    architect = DialogueSystem.instance.currentArchitect;
                    while (architect.isConstructing)
                        yield return new WaitForEndOfFrame();
                }
                
                running = null;
            }

            public void ForceFinish()
            {
                if (running != null)
                    NovelController.instance.StopCoroutine(running);
                running = null;

                if (architect != null)
                {
                    architect.ForceFinish();

                    if (pretext == "")
                        line.lastSegmentsWholeDialog = "";

                    //execute all actions that havent been made yet
                    string[] parts = dialogue.Split('[', ']');
                    for (int i = 0; i < parts.Length; i++)
                    {
                        bool isOdd = i % 2 != 0;
                        if (isOdd)
                        {
                            string e = parts[i];
                            if (allCurrentlyExecutedEvents.Contains(e))
                            {
                                allCurrentlyExecutedEvents.Remove(e);
                            }
                            else
                            {
                                DialogueEvents.HandleEvent(e, this);
                            }
                            i++;
                        }
                        //append only the dialogue to the whole dialogue and make the architect show it
                        line.lastSegmentsWholeDialog += parts[i];
                    }

                    //show the whole dialogue on the architect
                    architect.ShowText(line.lastSegmentsWholeDialog);
                }                    
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager instance;

    public RectTransform characterPanel;//Todos los personajes tienen que estar enlazados al panel de personaje
    public List<Character> characters = new List<Character>();//Lista de personajes que habrá en escena
    public Dictionary<string, int> charactersDictionary = new Dictionary<string, int>();//Forma para buscar nuestros personajes

    private void Awake()
    {
        instance = this;
    }

    public Character GetCharacters(string characterName, bool createCharacterIfDoesNotExist = true, bool enableCreatedCharacterOnStart = true)//Intenta pillar un personaje por el nombre dado desde la lista de personajes
    {
        int index = -1;
        if (charactersDictionary.TryGetValue(characterName, out index))
        {
            return characters[index];
        }
        else if (createCharacterIfDoesNotExist)
        {
            if (Resources.Load("Characters/Character[" + characterName + "]") != null)
                return CreateCharacter(characterName, enableCreatedCharacterOnStart);
            return null;
        }

        return null;
    }

    public Character CreateCharacter(string characterName, bool enableOnStart = true)
    {
        Character newCharacter = new Character(characterName, enableOnStart);

        charactersDictionary.Add(characterName, characters.Count);
        characters.Add(newCharacter);

        return newCharacter;
    }

    public class CHARACTERPOSITIONS//Esto es para llamar a estos vectores a la hora de introducir una posicion para el personaje, pero se puede introducir a mano////////////
    {
        public Vector2 bottomLeft = new Vector2(0, 0);
        public Vector2 topRight = new Vector2(1f, 1f);
        public Vector2 center = new Vector2(0.5f, 0.5f);
        public Vector2 bottomRight = new Vector2(1f, 0);
        public Vector2 topLeft = new Vector2(0, 1f);
    }
    public static CHARACTERPOSITIONS characterPositions = new CHARACTERPOSITIONS();

    public class CHARACTEREXPRESSION//Esto es concreto del código del tipo del video, usa esto para diferenciar las distintas poses de sus personajes, nosotros podemos usarlo o no/////
    {
        public int normal = 0;
        public int shy = 1;
        public int normalAngle = 2;
        public int cojoinedFingers = 3;
    }

    public static CHARACTEREXPRESSION characterExpression = new CHARACTEREXPRESSION();
}

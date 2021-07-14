using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum messageType
{
    Normal,
    Choice,
    Event
};

[CreateAssetMenu(fileName = "Mensajes", menuName = "Visual Novel/Mensajes", order = 1)]

public class Message : ScriptableObject
{
    public messageType TipoDeMensaje;
    public string NombreDePersonaje;
    [TextArea]
    public string Contenido;
    public Sprite SpriteResaltado;
    public string[] Decisiones;
}

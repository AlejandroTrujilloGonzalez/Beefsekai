using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Encrpyterizador : MonoBehaviour
{
    public static Encrpyterizador instance;


    private void Awake()
    {
        instance = this;
    }
    
    public bool encryptedOn;
}

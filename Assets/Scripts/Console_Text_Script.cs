﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Console_Text_Script : Photon.MonoBehaviour
{

    private TextMesh textMesh;
    public bool isTyping = false;

    // Use this for initialization
    void Start ()
    {
        textMesh = GetComponent<TextMesh>();
        textMesh.text = "";
    }

    [PunRPC]
    public void RpcTypeText(string message)
    {
        StartCoroutine(TypeText(message));
    }

    //Type out the text that is loaded into the "message" variable
    public IEnumerator TypeText(string message)
    {
        isTyping = true;
        textMesh = GetComponent<TextMesh>();
        textMesh.text = "";
        foreach (char letter in message.ToCharArray())
        {
            textMesh.text += letter;
            yield return new WaitForSeconds(0.05f);
        }
        isTyping = false;
    }

}

﻿using UnityEngine;
using System.Collections;

public class Spawn_Abort_Reset_Rotate : Photon.MonoBehaviour
{
    [SerializeField]
    int playerNum;
    [SerializeField]
    Transform AbortButton;
    [SerializeField]
    Transform ResetButton;
    [SerializeField]
    Transform RotateButton1;
    [SerializeField]
    Transform RotateButton2;

    // Use this for initialization
    void Start()
    {
        if (PhotonNetwork.isMasterClient)
        {
            object[] data = new object[1];
            data[0] = playerNum;
            PhotonNetwork.InstantiateSceneObject("Abort Button", AbortButton.position, AbortButton.rotation, 0, data);
            PhotonNetwork.InstantiateSceneObject("Reset Button", ResetButton.position, ResetButton.rotation, 0, data);
            StartCoroutine(SpawnRotateButtons(data));
            
        }
    }

    IEnumerator SpawnRotateButtons(object[] data)
    {
        yield return new WaitForSeconds(4);
        PhotonNetwork.InstantiateSceneObject("Rotate Button", RotateButton1.position, RotateButton1.rotation, 0, data);
        PhotonNetwork.InstantiateSceneObject("Rotate Button", RotateButton2.position, RotateButton2.rotation, 0, data);
    }

    // Update is called once per frame
    void Update()
    {

    }
}

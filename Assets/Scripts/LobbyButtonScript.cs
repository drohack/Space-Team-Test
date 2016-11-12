﻿using UnityEngine;
using System.Collections;

public class LobbyButtonScript : Photon.MonoBehaviour
{

    private Highlight_Handle_Top_Script handleScript;
    Animator anim;
    private bool isButtonDown = false;
    private bool isAnimating = false;
    private bool isLocked = false;
    [SerializeField] public PhotonMainMenu photonLobby_VR_Script;
    public GameObject gameName;
    

    //Network variables


    // Use this for initialization
    void Start()
    {
        //Load Network data

        handleScript = transform.Find("Handle").GetComponent<Highlight_Handle_Top_Script>();
        anim = transform.Find("Handle").GetComponent<Animator>();
        isButtonDown = false;
        isAnimating = false;
        isLocked = false;
    }
    // Update is called once per frame
    void Update()
    {
        if (!isAnimating && isButtonDown && isLocked && !handleScript.isGrabbing && !handleScript.isColliding)
        {
            isLocked = false;
            isButtonDown = false;
            photonLobby_VR_Script.JoinRoom(gameName.GetComponent<TextMesh>().text);
            StartCoroutine(WaitForAnimation(anim, "Button_Up_Anim"));
        }

        if (!isAnimating && !isLocked && !isButtonDown && (handleScript.isGrabbing || handleScript.isColliding))
        {
            isLocked = true;
            isButtonDown = true;
            Debug.Log("Pushed Button");
            StartCoroutine(WaitForAnimation(anim, "Button_Down_Anim"));
        }
    }

    private IEnumerator WaitForAnimation(Animator animation, string animationName)
    {
        isAnimating = true;
        animation.Play(animationName);
        do
        {
            yield return null;
        } while (animation.GetCurrentAnimatorStateInfo(0).IsName(animationName) && !animation.IsInTransition(0));

        isAnimating = false;
    }
}

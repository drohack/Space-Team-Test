﻿using UnityEngine;
using System.Collections;

public class Mastermind_Script : MonoBehaviour {

    /** SINGLE VARIABLES **/
    private         int numPlayers = 2;
    private         int score = 0;
    private         int numOfDiffGameObjects = 3; // The number of different type of game objects total to be used for random rolling of said game objects
    private const   int commandTimeoutSeconds = 10;
    public const    int buttonCommand = 0;
    public const    int lLeverCommand = 1;
    public const    int wLeverCommand = 2;
    private ArrayList buttonCommandArray;
    private ArrayList lLeverCommandArray;
    private ArrayList wLeverCommandArray;
    private GameObject[] rObjList; // The list of all random game objects get placed in current round
    private bool  isTapped = false;    // Variables for the custom WaitForSeconds function

    // Player Objects
    GameObject          p1_PlayerControlDeck;
    GameObject          p2_PlayerControlDeck;
    GameObject          p3_PlayerControlDeck;
    GameObject          p4_PlayerControlDeck;
    Score_Text_Script   p1_scoreTextScript;
    Score_Text_Script   p2_scoreTextScript;
    Score_Text_Script   p3_scoreTextScript;
    Score_Text_Script   p4_scoreTextScript;
    Console_Text_Script p1_consoleTextScript;
    Console_Text_Script p2_consoleTextScript;
    Console_Text_Script p3_consoleTextScript;
    Console_Text_Script p4_consoleTextScript;

    /** P1 VARIABLES **/
    public  int     p1_rCommand = -1;
    private bool    p1_isDisplayStart = true;
    public  bool    p1_isDisplayingCommand = false;
    private float   p1_gWaitSystem;         // Variables for the custom WaitForSeconds function
    private int     p1_gridX = 3;           // The grid which the random game objects get placed
    private int     p1_gridY = 3;           // The grid which the random game objects get placed

    /** P2 VARIABLES **/
    public int      p2_rCommand = -1;
    private bool    p2_isDisplayStart = true;
    public bool     p2_isDisplayingCommand = false;
    private float   p2_gWaitSystem;         // Variables for the custom WaitForSeconds function
    private int     p2_gridX = 3;           // The grid which the random game objects get placed
    private int     p2_gridY = 3;           // The grid which the random game objects get placed

    /** P3 VARIABLES **/
    public int      p3_rCommand = -1;
    private bool    p3_isDisplayStart = true;
    public bool     p3_isDisplayingCommand = false;
    private float   p3_gWaitSystem;         // Variables for the custom WaitForSeconds function
    private int     p3_gridX = 3;           // The grid which the random game objects get placed
    private int     p3_gridY = 3;           // The grid which the random game objects get placed

    /** P4 VARIABLES **/
    public int      p4_rCommand = -1;
    private bool    p4_isDisplayStart = true;
    public bool     p4_isDisplayingCommand = false;
    private float   p4_gWaitSystem;         // Variables for the custom WaitForSeconds function
    private int     p4_gridX = 3;           // The grid which the random game objects get placed
    private int     p4_gridY = 3;           // The grid which the random game objects get placed

    // Use this for initialization
    void Start () {
        Initialize();

        GenerateRandomObjects();

        StartCoroutine(DisplayStartText());
    }

    void Initialize()
    {
        score = 0;

        // Set command arrays from Command_Array.cs
        Command_Array commandArray = GetComponent<Command_Array>();
        buttonCommandArray = commandArray.buttonCommandArray;
        lLeverCommandArray = commandArray.lLeverCommandArray;
        wLeverCommandArray = commandArray.wLeverCommandArray;

        // Set player objects
        p1_PlayerControlDeck = GameObject.Find("Player Control Deck 1");
        p1_scoreTextScript = p1_PlayerControlDeck.transform.Find("Score_Text").GetComponent<Score_Text_Script>();
        p1_consoleTextScript = p1_PlayerControlDeck.transform.Find("Console/Console_Text").GetComponent<Console_Text_Script>();
        if (numPlayers > 1)
        {
            p2_PlayerControlDeck = GameObject.Find("Player Control Deck 2");
            p2_scoreTextScript = p2_PlayerControlDeck.transform.Find("Score_Text").GetComponent<Score_Text_Script>();
            p2_consoleTextScript = p2_PlayerControlDeck.transform.Find("Console/Console_Text").GetComponent<Console_Text_Script>();
        }
        if (numPlayers > 2)
        {
            p3_PlayerControlDeck = GameObject.Find("Player Control Deck 3");
            p3_scoreTextScript = p3_PlayerControlDeck.transform.Find("Score_Text").GetComponent<Score_Text_Script>();
            p3_consoleTextScript = p3_PlayerControlDeck.transform.Find("Console/Console_Text").GetComponent<Console_Text_Script>();
        }
        if (numPlayers > 3)
        {
            p4_PlayerControlDeck = GameObject.Find("Player Control Deck 4");
            p4_scoreTextScript = p4_PlayerControlDeck.transform.Find("Score_Text").GetComponent<Score_Text_Script>();
            p4_consoleTextScript = p4_PlayerControlDeck.transform.Find("Console/Console_Text").GetComponent<Console_Text_Script>();
        }
    }

    //Create new list of random objects
    void GenerateRandomObjects ()
    {
        //Create a list to hold all of the random game objects
        int rObjListSize = (p1_gridX * p1_gridY);
        if (numPlayers > 1)
            rObjListSize += (p2_gridX * p2_gridY);
        if (numPlayers > 2)
            rObjListSize += (p3_gridX * p3_gridY);
        if (numPlayers > 3)
            rObjListSize += (p4_gridX * p4_gridY);
        rObjList = new GameObject[rObjListSize];

        //Generate rObjList objects for PLAYER 1
        rObjList = GenerateRandomObjects(rObjList, 0, p1_PlayerControlDeck, p1_gridX, p1_gridY, 1);

        if (numPlayers > 1)
        {
            //Generate rObjList objects for PLAYER 2
            rObjList = GenerateRandomObjects(rObjList, (p2_gridX * p2_gridY), p2_PlayerControlDeck, p2_gridX, p2_gridY, 2);
        }
        if (numPlayers > 2)
        {
             //Generate rObjList objects for PLAYER 3
             rObjList = GenerateRandomObjects(rObjList, (p3_gridX * p3_gridY) + (p2_gridX * p2_gridY), p3_PlayerControlDeck, p3_gridX, p3_gridY, 3);
        }
        if (numPlayers > 3)
        {
            //Generate rObjList objects for PLAYER 4
            rObjList = GenerateRandomObjects(rObjList, (p4_gridX * p4_gridY) + (p2_gridX * p2_gridY) + (p3_gridX * p3_gridY), p4_PlayerControlDeck, p4_gridX, p4_gridY, 4);
        }
    }

    GameObject[] GenerateRandomObjects(GameObject[] inRObjList, int intRObjListSize, GameObject playerControlDeck, int gridX, int gridY, int playerNum)
    {
        GameObject[] outRObjList = inRObjList;

        int commandIndex;
        string newCommandText;

        GameObject buttonInstance = (GameObject)Resources.Load("Prefabs/Button");
        GameObject lLeverInstance = (GameObject)Resources.Load("Prefabs/L_Lever");
        GameObject wLeverInstance = (GameObject)Resources.Load("Prefabs/W_Lever");

        float xBQuaternion = buttonInstance.transform.rotation.eulerAngles.x + playerControlDeck.transform.rotation.eulerAngles.x;
        float yBQuaternion = buttonInstance.transform.rotation.eulerAngles.y + playerControlDeck.transform.rotation.eulerAngles.y;
        float zBQuaternion = buttonInstance.transform.rotation.eulerAngles.z + playerControlDeck.transform.rotation.eulerAngles.z;
        float xLQuaternion = lLeverInstance.transform.rotation.eulerAngles.x + playerControlDeck.transform.rotation.eulerAngles.x;
        float yLQuaternion = lLeverInstance.transform.rotation.eulerAngles.y + playerControlDeck.transform.rotation.eulerAngles.y;
        float zLQuaternion = lLeverInstance.transform.rotation.eulerAngles.z + playerControlDeck.transform.rotation.eulerAngles.z;
        float xWQuaternion = wLeverInstance.transform.rotation.eulerAngles.x + playerControlDeck.transform.rotation.eulerAngles.x;
        float yWQuaternion = wLeverInstance.transform.rotation.eulerAngles.y + playerControlDeck.transform.rotation.eulerAngles.y;
        float zWQuaternion = wLeverInstance.transform.rotation.eulerAngles.z + playerControlDeck.transform.rotation.eulerAngles.z;

        //for each grid position generate a random object and add it to the random object list
        for (int x = 0; x < gridX; x++)
        {
            for (int y = 0; y < gridY; y++)
            {
                GameObject randObject;
                //roll for a random game object
                int objNum = Random.Range(0, numOfDiffGameObjects);

                Vector3 buttonVector3 = new Vector3();
                Vector3 lLeverVector3 = new Vector3();
                Vector3 wLeverVector3 = new Vector3();
                //for the given random game object create a copy of it to randObject
                switch (objNum)
                {
                    case buttonCommand:
                        //roll for a random Button command from the buttonCommandArray
                        commandIndex = Random.Range(0, buttonCommandArray.Count);
                        newCommandText = (string)buttonCommandArray[commandIndex];
                        //remove selected button command from buttonCommandArray so it won't be used again
                        buttonCommandArray.RemoveAt(commandIndex);
                        //copy randomObject from the default wLever
                        if (playerNum == 1)
                            buttonVector3 = new Vector3(playerControlDeck.transform.position.x - 3 + (3 * x), 3 + (4 * y), playerControlDeck.transform.position.z - 0.7f);
                        if (playerNum == 2)
                            buttonVector3 = new Vector3(playerControlDeck.transform.position.x + 0.7f, 3 + (4 * y), playerControlDeck.transform.position.z - 3 + (3 * x));
                        if (playerNum == 3)
                            buttonVector3 = new Vector3(playerControlDeck.transform.position.x - 3 + (3 * x), 3 + (4 * y), playerControlDeck.transform.position.z + 0.7f);
                        if (playerNum == 4)
                            buttonVector3 = new Vector3(playerControlDeck.transform.position.x - 0.7f, 3 + (4 * y), playerControlDeck.transform.position.z - 3 + (3 * x));
                        randObject = (GameObject)Instantiate(Resources.Load("Prefabs/Button"),
                            buttonVector3,
                            Quaternion.Euler(new Vector3(xBQuaternion, yBQuaternion, zBQuaternion)));
                        randObject.transform.parent = playerControlDeck.transform;
                        //add new command text to the new randomObject
                        randObject.transform.GetChild(0).transform.GetChild(0).GetComponent<TextMesh>().text = newCommandText;
                        //set randObject's rCommand in it's Script
                        randObject.GetComponent<Button_Press_Script>().rCommand = intRObjListSize + ((x * gridX) + y);
                        break;
                    case lLeverCommand:
                        //roll for a random Button command from the lLeverCommandArray
                        commandIndex = Random.Range(0, lLeverCommandArray.Count);
                        newCommandText = (string)lLeverCommandArray[commandIndex];
                        //remove selected button command from lLeverCommandArray so it won't be used again
                        lLeverCommandArray.RemoveAt(commandIndex);
                        //copy randomObject from the default wLever
                        if (playerNum == 1)
                            lLeverVector3 = new Vector3(playerControlDeck.transform.position.x - 3 + (3 * x), 3 + (4 * y), playerControlDeck.transform.position.z - 1.47f);
                        if (playerNum == 2)
                            lLeverVector3 = new Vector3(playerControlDeck.transform.position.x + 1.47f, 3 + (4 * y), playerControlDeck.transform.position.z - 3 + (3 * x));
                        if (playerNum == 3)
                            lLeverVector3 = new Vector3(playerControlDeck.transform.position.x - 3 + (3 * x), 3 + (4 * y), playerControlDeck.transform.position.z + 1.47f);
                        if (playerNum == 4)
                            lLeverVector3 = new Vector3(playerControlDeck.transform.position.x - 1.47f, 3 + (4 * y), playerControlDeck.transform.position.z - 3 + (3 * x));
                        randObject = (GameObject)Instantiate(Resources.Load("Prefabs/L_Lever"),
                            lLeverVector3,
                            Quaternion.Euler(new Vector3(xLQuaternion, yLQuaternion, zLQuaternion)));
                        randObject.transform.parent = playerControlDeck.transform;
                        //add new command text to the new randomObject
                        randObject.transform.GetChild(0).transform.GetChild(0).GetComponent<TextMesh>().text = newCommandText;
                        //set randObject's rCommand in it's Script
                        randObject.transform.GetChild(2).GetComponent<L_Lever_Handle_Script>().rCommand = intRObjListSize + (p3_gridX * p3_gridY) + ((x * gridX) + y);
                        break;
                    case wLeverCommand:
                        //roll for a random Button command from the wLeverCommandArray
                        commandIndex = Random.Range(0, wLeverCommandArray.Count);
                        newCommandText = (string)wLeverCommandArray[commandIndex];
                        //remove selected button command from wLeverCommandArray so it won't be used again
                        wLeverCommandArray.RemoveAt(commandIndex);
                        //copy randomObject from the default wLever
                        if (playerNum == 1)
                            wLeverVector3 = new Vector3(playerControlDeck.transform.position.x - 3 + (3 * x), 3 + (4 * y), playerControlDeck.transform.position.z - 1.52f);
                        if (playerNum == 2)
                            wLeverVector3 = new Vector3(playerControlDeck.transform.position.x + 1.52f, 3 + (4 * y), playerControlDeck.transform.position.z - 3 + (3 * x));
                        if (playerNum == 3)
                            wLeverVector3 = new Vector3(playerControlDeck.transform.position.x - 3 + (3 * x), 3 + (4 * y), playerControlDeck.transform.position.z + 1.52f);
                        if (playerNum == 4)
                            wLeverVector3 = new Vector3(playerControlDeck.transform.position.x - 1.52f, 3 + (4 * y), playerControlDeck.transform.position.z - 3 + (3 * x));
                        randObject = (GameObject)Instantiate(Resources.Load("Prefabs/W_Lever"),
                            wLeverVector3,
                            Quaternion.Euler(new Vector3(xWQuaternion, yWQuaternion, zWQuaternion)));
                        randObject.transform.parent = playerControlDeck.transform;
                        //add new command text to the new randomObject
                        randObject.transform.GetChild(0).transform.GetChild(0).GetComponent<TextMesh>().text = newCommandText;
                        //set randObject's rCommand in it's Script
                        randObject.transform.GetChild(2).GetComponent<W_Lever_Handle_Script>().rCommand = intRObjListSize + (p3_gridX * p3_gridY) + ((x * gridX) + y);
                        break;
                    default:
                        //roll for a random Button command from the buttonCommandArray
                        commandIndex = Random.Range(0, buttonCommandArray.Count);
                        newCommandText = (string)buttonCommandArray[commandIndex];
                        //remove selected button command from buttonCommandArray so it won't be used again
                        buttonCommandArray.RemoveAt(commandIndex);
                        //copy randomObject from the default wLever
                        if (playerNum == 1)
                            buttonVector3 = new Vector3(playerControlDeck.transform.position.x - 3 + (3 * x), 3 + (4 * y), playerControlDeck.transform.position.z - 0.7f);
                        if (playerNum == 2)
                            buttonVector3 = new Vector3(playerControlDeck.transform.position.x + 0.7f, 3 + (4 * y), playerControlDeck.transform.position.z - 3 + (3 * x));
                        if (playerNum == 3)
                            buttonVector3 = new Vector3(playerControlDeck.transform.position.x - 3 + (3 * x), 3 + (4 * y), playerControlDeck.transform.position.z + 0.7f);
                        if (playerNum == 4)
                            buttonVector3 = new Vector3(playerControlDeck.transform.position.x - 0.7f, 3 + (4 * y), playerControlDeck.transform.position.z - 3 + (3 * x));
                        randObject = (GameObject)Instantiate(Resources.Load("Prefabs/Button"),
                            buttonVector3,
                            Quaternion.Euler(new Vector3(xBQuaternion, yBQuaternion, zBQuaternion)));
                        randObject.transform.parent = playerControlDeck.transform;
                        //add new command text to the new randomObject
                        randObject.transform.GetChild(0).transform.GetChild(0).GetComponent<TextMesh>().text = newCommandText;
                        //set randObject's rCommand in it's Script
                        randObject.GetComponent<Button_Press_Script>().rCommand = intRObjListSize + (p3_gridX * p3_gridY) + ((x * gridX) + y);
                        break;
                }

                //add randomObject to grid
                outRObjList[intRObjListSize + ((x * gridX) + y)] = randObject;
            }
        }

        return outRObjList;
    }

    // Update is called once per frame
    void Update () {
        //Quit if the user presses the esc key
        if (Input.GetKey("escape"))
            Application.Quit();

        //If the user scores 10 or more, change text to Green and to say "YOU WIN~"
        if (score >= 10)
        {
            p1_scoreTextScript.Win();
            if (numPlayers > 1)
                p2_scoreTextScript.Win();
            if (numPlayers > 2)
                p3_scoreTextScript.Win();
            if (numPlayers > 3)
                p4_scoreTextScript.Win();
        }

        //if we are NOT typing "START!" including waiting the 2 seconds
        //AND if we are NOT currently typing a command
        //AND if we are NOT currently waiting the 10 seconds for a command to pass
        //generate and display a new random command
        if (!p1_isDisplayStart && !p1_consoleTextScript.isTyping && !p1_isDisplayingCommand)
            StartCoroutine(P1_DisplayRandomCommand());
        if (numPlayers > 1)
        {
            if (!p2_isDisplayStart && !p2_consoleTextScript.isTyping && !p2_isDisplayingCommand)
                StartCoroutine(P2_DisplayRandomCommand());
        }
        if (numPlayers > 2)
        {
            if (!p3_isDisplayStart && !p3_consoleTextScript.isTyping && !p3_isDisplayingCommand)
                StartCoroutine(P3_DisplayRandomCommand());
        }
        if (numPlayers > 3)
        {
            if (!p4_isDisplayStart && !p4_consoleTextScript.isTyping && !p4_isDisplayingCommand)
                StartCoroutine(P4_DisplayRandomCommand());
        }
    }

    public void ScoreUp()
    {
        score++;
        p1_scoreTextScript.ScoreUp(score);
        if (numPlayers > 1)
            p2_scoreTextScript.ScoreUp(score);
        if (numPlayers > 2)
            p3_scoreTextScript.ScoreUp(score);
        if (numPlayers > 3)
            p4_scoreTextScript.ScoreUp(score);
    }

    public void ScoreDown()
    {
        score--;
        p1_scoreTextScript.ScoreDown(score);
        if (numPlayers > 1)
            p2_scoreTextScript.ScoreDown(score);
        if (numPlayers > 2)
            p3_scoreTextScript.ScoreDown(score);
        if (numPlayers > 3)
            p4_scoreTextScript.ScoreDown(score);
    }

    //Display "START!" for 2 seconds
    IEnumerator DisplayStartText()
    {
        p1_isDisplayStart = true;
        StartCoroutine(p1_consoleTextScript.TypeText(" START!"));
        if (numPlayers > 1)
        {
            p2_isDisplayStart = true;
            StartCoroutine(p2_consoleTextScript.TypeText(" START!"));
        }
        if (numPlayers > 2)
        {
            p3_isDisplayStart = true;
            StartCoroutine(p3_consoleTextScript.TypeText(" START!"));
        }
        if (numPlayers > 3)
        {
            p4_isDisplayStart = true;
            StartCoroutine(p4_consoleTextScript.TypeText(" START!"));
        }
        yield return new WaitForSeconds(2);
        p1_isDisplayStart = false;
        p2_isDisplayStart = false;
        p3_isDisplayStart = false;
        p4_isDisplayStart = false;
    }

    // Custom WaitForSeconds
    // This will either wait for the given seconds, or until the isTapped boolean is set to TRUE
    IEnumerator WaitForSecondsOrTap(int player, float seconds)
    {
        if (player == 1)
        {
            p1_gWaitSystem = seconds;
            while (p1_gWaitSystem > 0.0)
            {
                p1_gWaitSystem -= Time.deltaTime;
                yield return 0;
            }
        }
        else if (player == 2)
        {
            p2_gWaitSystem = seconds;
            while (p2_gWaitSystem > 0.0)
            {
                p2_gWaitSystem -= Time.deltaTime;
                yield return 0;
            }
        }
        else if (player == 3)
        {
            p3_gWaitSystem = seconds;
            while (p3_gWaitSystem > 0.0)
            {
                p3_gWaitSystem -= Time.deltaTime;
                yield return 0;
            }
        }
        else if (player == 4)
        {
            p4_gWaitSystem = seconds;
            while (p4_gWaitSystem > 0.0)
            {
                p4_gWaitSystem -= Time.deltaTime;
                yield return 0;
            }
        }

        //lower score if time reached (button was not tapped)
        if (!isTapped)
        {
            ScoreDown();
        }

        //reset isTapped
        isTapped = false;
    }

    // End the waitForSeconds by setting the timer to zero AND signal that a button was tapped (isTapped = true)
    public void TappedWaitForSecondsOrTap(int inputCommand)
    {
        isTapped = true;
        bool foundCommand = false;
        
        //Check to see if the current command is the correct button pressed. Update score accordingly
        if (p1_rCommand == inputCommand)
        {
            ScoreUp();
            //Set timer for that player to 0 to get next command
            p1_gWaitSystem = 0.0f;
            foundCommand = true;
        }
        if (p2_rCommand == inputCommand)
        {
            ScoreUp();
            //Set timer for that player to 0 to get next command
            p2_gWaitSystem = 0.0f;
            foundCommand = true;
        }
        if (p3_rCommand == inputCommand)
        {
            ScoreUp();
            //Set timer for that player to 0 to get next command
            p3_gWaitSystem = 0.0f;
            foundCommand = true;
        }
        if (p4_rCommand == inputCommand)
        {
            ScoreUp();
            //Set timer for that player to 0 to get next command
            p4_gWaitSystem = 0.0f;
            foundCommand = true;
        }

        // If no command matched lower score
        if (!foundCommand)
        {
            ScoreDown();
        }
    }

    IEnumerator P1_DisplayRandomCommand()
    {
        p1_isDisplayingCommand = true;

        //Clear text
        p1_consoleTextScript.GetComponent<TextMesh>().text = "";
        yield return 0;
        yield return new WaitForSeconds(1);

        //Roll random number to decide new command from the random object list
        p1_rCommand = Random.Range(0, rObjList.Length);

        //get random game object from random object list
        GameObject rObj = rObjList[p1_rCommand];

        string[] rCommandList = GetRandomCommand(rObj, p1_rCommand);
        p1_rCommand = int.Parse(rCommandList[0]);
        string message = rCommandList[1];

        //Type out new command to console
        StartCoroutine(p1_consoleTextScript.TypeText(message));

        //Wait for the commandTimeoutSeconds or if a button gets tapped
        yield return WaitForSecondsOrTap(1, commandTimeoutSeconds);
        p1_isDisplayingCommand = false;
    }

    IEnumerator P2_DisplayRandomCommand()
    {
        p2_isDisplayingCommand = true;

        //Clear text
        p2_consoleTextScript.GetComponent<TextMesh>().text = "";
        yield return 0;
        yield return new WaitForSeconds(1);

        //Roll random number to decide new command from the random object list
        p2_rCommand = Random.Range(0, rObjList.Length);

        //get random game object from random object list
        GameObject rObj = rObjList[p2_rCommand];

        string[] rCommandList = GetRandomCommand(rObj, p2_rCommand);
        p2_rCommand = int.Parse(rCommandList[0]);
        string message = rCommandList[1];

        //Type out new command to console
        StartCoroutine(p2_consoleTextScript.TypeText(message));

        //Wait for the commandTimeoutSeconds or if a button gets tapped
        yield return WaitForSecondsOrTap(2, commandTimeoutSeconds);
        p2_isDisplayingCommand = false;
    }

    IEnumerator P3_DisplayRandomCommand()
    {
        p3_isDisplayingCommand = true;

        //Clear text
        p3_consoleTextScript.GetComponent<TextMesh>().text = "";
        yield return 0;
        yield return new WaitForSeconds(1);

        //Roll random number to decide new command from the random object list
        p3_rCommand = Random.Range(0, rObjList.Length);

        //get random game object from random object list
        GameObject rObj = rObjList[p3_rCommand];

        string[] rCommandList = GetRandomCommand(rObj, p3_rCommand);
        p3_rCommand = int.Parse(rCommandList[0]);
        string message = rCommandList[1];

        //Type out new command to console
        StartCoroutine(p3_consoleTextScript.TypeText(message));

        //Wait for the commandTimeoutSeconds or if a button gets tapped
        yield return WaitForSecondsOrTap(3, commandTimeoutSeconds);
        p3_isDisplayingCommand = false;
    }

    IEnumerator P4_DisplayRandomCommand()
    {
        p4_isDisplayingCommand = true;

        //Clear text
        p4_consoleTextScript.GetComponent<TextMesh>().text = "";
        yield return 0;
        yield return new WaitForSeconds(1);

        //Roll random number to decide new command from the random object list
        p4_rCommand = Random.Range(0, rObjList.Length);

        //get random game object from random object list
        GameObject rObj = rObjList[p4_rCommand];

        string[] rCommandList = GetRandomCommand(rObj, p4_rCommand);
        p4_rCommand = int.Parse(rCommandList[0]);
        string message = rCommandList[1];

        //Type out new command to console
        StartCoroutine(p4_consoleTextScript.TypeText(message));

        //Wait for the commandTimeoutSeconds or if a button gets tapped
        yield return WaitForSecondsOrTap(4, commandTimeoutSeconds);
        p4_isDisplayingCommand = false;
    }

    string[] GetRandomCommand (GameObject rObj, int rCommand)
    {
        int commandType = buttonCommand;
        if (rObj.name.Contains("Button"))
            commandType = buttonCommand;
        else if (rObj.name.Contains("L_Lever"))
            commandType = lLeverCommand;
        else if (rObj.name.Contains("W_Lever"))
            commandType = wLeverCommand;


        string message = "";
        //Get new command
        switch (commandType)
        {
            case buttonCommand:
                //Button
                string buttonText = rObj.transform.GetChild(0).transform.GetChild(0).GetComponent<TextMesh>().text;
                message = "Engage " + buttonText;
                break;
            case lLeverCommand:
                //L_Lever
                string lLeverText = rObj.transform.GetChild(0).transform.GetChild(0).GetComponent<TextMesh>().text;
                L_Lever_Handle_Script lLeverHandleScript = rObj.transform.GetChild(2).GetComponent<L_Lever_Handle_Script>();
                message = "Turn ";
                if (lLeverHandleScript.isLLeverUp)
                {
                    message += "OFF ";
                    rCommand = (rCommand * 100) + 2;
                }
                else
                {
                    message += "ON ";
                    rCommand = (rCommand * 100) + 1;
                }
                message += lLeverText;
                break;
            case wLeverCommand:
                //W_Lever
                string wLeverText = rObj.transform.GetChild(0).transform.GetChild(0).GetComponent<TextMesh>().text;
                W_Lever_Handle_Script wLeverHandleScript = rObj.transform.GetChild(2).GetComponent<W_Lever_Handle_Script>();
                message = "";
                if (wLeverHandleScript.isWLeverUp)
                {
                    message += "Lower ";
                    rCommand = (rCommand * 100) + 2;
                }
                else
                {
                    message += "Raise ";
                    rCommand = (rCommand * 100) + 1;
                }
                message += wLeverText;
                break;
            default:
                break;
        }

        return new string[2]{ rCommand.ToString() , message };
    }
}

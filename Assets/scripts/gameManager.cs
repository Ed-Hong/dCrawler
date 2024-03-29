﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;       //Allows us to use Lists. 
    
public class gameManager : MonoBehaviour
{
    public static gameManager instance = null;		//makes singleton of gameManager, allowing to be accessed anywhere in any script
    public const int xTileSize = 27;
    public const int yTileSize = 21;

    public delegate void StartTurnAction();
    public static event StartTurnAction OnStartTurn;

    public delegate void EndTurnAction();
    public static event EndTurnAction OnEndTurn;

    private bool isTurnInProgress = false;
    private bool isPlayerMoving = false;
    private bool isPlayerKnockedBack = false;

    private int minimumTurnFrames = 5;
    private int turnFrameCounter = 0;

    //Awake is always called before any Start functions
    void Awake()
    {
        //Check if instance already exists
        if (instance == null){
			instance = this;
		} else if (instance != this){
			Destroy(gameObject);
		}
                
        DontDestroyOnLoad(gameObject);
        InitGame();
    }

    public void StartTurn()
    {
        if(!isTurnInProgress && OnStartTurn != null)
        {
            isTurnInProgress = true;
            OnStartTurn();
            print("------START------");
            EnemyMovement.Go();
        }
    }

    public IEnumerator EndTurn()
    {
        if (isTurnInProgress && OnEndTurn != null)
        {
            yield return StartCoroutine(WaitForFrames(minimumTurnFrames - turnFrameCounter));
            isTurnInProgress = false;
            turnFrameCounter = 0;
            OnEndTurn();
            print("------END------");
            EnemyMovement.Stop();
        }
    }

    public static IEnumerator WaitForFrames(int frameCount)
    {
        while (frameCount > 0)
        {
            frameCount--;
            print("waiting");
            yield return null;
        }
    }

    public bool IsTurnInProgress()
    {
        return isTurnInProgress;
    }

    public void CountFrame()
    {
        turnFrameCounter++;
        //print(turnFrameCounter);
    }

    public void SetPlayerIsKnockedBack(bool val)
    {
        isPlayerKnockedBack = val;
    }

    public bool IsPlayerKnockedBack()
    {
        return isPlayerKnockedBack;
    }

    public void SetPlayerIsMoving(bool val)
    {
        isPlayerMoving = val;
    }

    public bool IsPlayerMoving()
    {
        return isPlayerMoving;
    }

    //Initializes the game for each level.
    void InitGame()
    {
		//Initalize stuff idk
    }
}
        
        

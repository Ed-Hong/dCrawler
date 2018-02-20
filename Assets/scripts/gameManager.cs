using UnityEngine;
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

    public delegate void KnockBackAction();
    public static event KnockBackAction OnKnockBack;

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
            //print("START");
        }
    }

    public IEnumerator EndTurn()
    {
        yield return StartCoroutine(WaitForFrames(minimumTurnFrames - turnFrameCounter));
        if (isTurnInProgress && OnEndTurn != null)
        {
            isTurnInProgress = false;
            turnFrameCounter = 0;
            OnEndTurn();
            //print("END");
        }
    }

    public static IEnumerator WaitForFrames(int frameCount)
    {
        while (frameCount > 0)
        {
            frameCount--;
            yield return null;
            print("waiting");
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

    public void KnockBackPlayer()
    {
        if(OnKnockBack != null)
        {
            isPlayerKnockedBack = true;
            OnKnockBack();
            isPlayerKnockedBack = false;
        }
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
        
        

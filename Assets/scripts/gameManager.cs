using UnityEngine;
using System.Collections;
using System.Collections.Generic;       //Allows us to use Lists. 
    
public class gameManager : MonoBehaviour
{
    public static gameManager instance = null;		//makes singleton of gameManager, allowing to be accessed anywhere in any script
    private bool canMove = true;
    public const int xTileSize = 27;
    public const int yTileSize = 21;                  //Current level number, expressed in game as "Day 1".

    public delegate void StartTurnAction();
    public static event StartTurnAction OnStartTurn;

    public delegate void EndTurnAction();
    public static event EndTurnAction OnEndTurn;

    private bool IsTurnInProgress = false;

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
        if(!IsTurnInProgress && OnStartTurn != null)
        {
            IsTurnInProgress = true;
            OnStartTurn();
        }
    }

    public void EndTurn()
    {
        if (IsTurnInProgress && OnEndTurn != null)
        {
            IsTurnInProgress = false;
            OnEndTurn();
        }
    }

    public void SetCanMove(bool val)
    {
        canMove = val;
    }

    public bool GetCanMove()
    {
        return canMove;
    }

    //Initializes the game for each level.
    void InitGame()
    {
		//Initalize stuff idk
    }
}
        
        

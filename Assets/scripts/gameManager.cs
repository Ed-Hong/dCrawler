using UnityEngine;
using System.Collections;
using System.Collections.Generic;       //Allows us to use Lists. 
    
public class gameManager : MonoBehaviour
{
    public static gameManager instance = null;		//makes singleton of gameManager, allowing to be accessed anywhere in any script
    private bool canMove = true;
    private bool moveTurn = false;
    public const int xTileSize = 27;
    public const int yTileSize = 21;                


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

    public void SetCanMove(bool val)
    {
        canMove = val;
        //print("Setting canMove to " + val);
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
        
        

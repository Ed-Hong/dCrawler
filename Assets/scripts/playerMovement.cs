using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;      //Allows us to use SceneManager
using Util;
using Weapons;

//Player inherits from MovingObject, our base class for objects that can move, Enemy also inherits from this.
public class playerMovement : movingObject
{
    public  float       restartLevelDelay   = 1f;                   //Delay time in seconds to restart level.
    private Animator    animator;                                  //store a reference to the Player's animator component.
    public  Direction   direction           = Direction.NORTH;    //enum for direction facing
    public  Weapon      currentWeapon       = new BaseSword();   //
    
    //Start overrides the Start function of MovingObject
    protected override void Start ()
    {
        animator = GetComponent<Animator>();
        //Start function of the movingObject base class.
        base.Start ();
    }
    
    
    //This function is called when the behaviour becomes disabled or inactive.
    private void OnDisable ()
    {
        //When Player object is disabled, store the current local food total in the GameManager so it can be re-loaded in next level.
        //GameManager.instance.playerFoodPoints = food;
    }
    
    
    private void Update ()
    {
        if(!gameManager.instance.canMove) return;

        //variables
        int horizontal = 0;     
        int vertical = 0;
        
        if(Input.GetKeyDown("w")){
            vertical += 1;
            Turn(Direction.NORTH);
        }else if(Input.GetKeyDown("s")){
            vertical -= 1;
            Turn(Direction.SOUTH);
        }else if(Input.GetKeyDown("a")){
            horizontal -= 1;

            Turn(Direction.WEST);
        }else if(Input.GetKeyDown("d")){
            horizontal += 1;
            Turn(Direction.EAST);
        }

        if (Input.GetKeyDown("k"))
        {
            if(currentWeapon.GetType() == typeof(BaseSword))
            {
                currentWeapon = new TSword();
            }
            else
            {
                currentWeapon = new BaseSword();
            }

            print("Equipped " + currentWeapon.GetType().ToString());
        }

        //prevent diagonal movements
        if (horizontal != 0)
        {
            vertical = 0;
        }
        
        //see if input in h or v is not zero
        if(horizontal != 0 || vertical != 0)
        {

            if (!AttemptAttack(direction) && AttemptMove<BoxCollider>(horizontal, vertical))
            {
                gameManager.instance.canMove = false; //disables input until player is done changin tiles
            }

        }
    }
    
    //AttemptMove overrides the AttemptMove function in the base class MovingObject
    //AttemptMove takes a generic parameter T which for Player will be of the type Wall, it also takes integers for x and y direction to move in.
    protected override bool AttemptMove <T> (int xDir, int yDir)
    {
        //Every time player moves, subtract from food points total.
        
        //Call the AttemptMove method of the base class, passing in the component T (in this case Wall) and x and y direction to move.
        base.AttemptMove <T> (xDir, yDir);
        
        //Hit allows us to reference the result of the Linecast done in Move.
        RaycastHit2D hit;

        bool didMove = Move(xDir, yDir, out hit);

        //If Move returns true, meaning Player was able to move into an empty space.
        if (didMove) 
        {
            //Call RandomizeSfx of SoundManager to play the move sound, passing in two audio clips to choose from.
        }

        return didMove;
    }
    
    
    //OnCantMove overrides the abstract function OnCantMove in MovingObject.
    //It takes a generic parameter T which in the case of Player is a Wall which the player can attack and destroy.
    protected override void OnCantMove <T> (T component)
    {
        //print("CANT MOVE");
    }

    protected void Turn(Direction dir) 
    {
        direction = dir;
    }

    protected bool AttemptAttack(Direction attackDir)
    {
        var attackRange = currentWeapon.GetHitsForPositionAndDirection(transform.position, attackDir, base.pixelsPerTile);
        if (attackRange.Any(h => h.collider != null)) {
            //checkHit(hit);
            print("HIT");
            return true;
        }
        return false;
    }
    
    
    //OnTriggerEnter2D is sent when another object enters a trigger collider attached to this object (2D physics only).
    private void OnTriggerEnter2D (Collider2D other)
    {
        if(other.tag == "Exit")
        {
            //Invoke the Restart function to start the next level with a delay of restartLevelDelay (default 1 second).
            Invoke ("Restart", restartLevelDelay);
            //Disable the player object since level is over.
            enabled = false;
        }
    }
    
    
    //Restart reloads the scene when called.
    private void Restart ()
    {
        SceneManager.LoadScene (0);
    }
    
    
}
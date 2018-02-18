using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;      //Allows us to use SceneManager
using Util;

//Player inherits from MovingObject, our base class for objects that can move, Enemy also inherits from this.
public class playerMovement : movingObject
{
    public float restartLevelDelay = 1f;        //Delay time in seconds to restart level.
    public int pointsPerFood = 10;              //Number of points to add to player food points when picking up a food object.
    public int pointsPerSoda = 20;              //Number of points to add to player food points when picking up a soda object.
    public int wallDamage = 1;                  //How much damage a player does to a wall when chopping it.
    private Animator animator;                  //Used to store a reference to the Player's animator component.
    public Direction direction = Direction.NORTH;
    
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

        if (Input.GetKeyDown("j"))
        {
            print(direction);
        }

        //prevent diagonal movements
        if (horizontal != 0)
        {
            vertical = 0;
        }
        
        //see if input in h or v is not zero
        if(horizontal != 0 || vertical != 0)
        {
            if(AttemptMove<BoxCollider> (horizontal, vertical))
            {
                gameManager.instance.canMove = false; //disables input until player is done changin tiles
            }
            AttemptAttack(direction);
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
        print("CANT MOVE");
    }

    protected void Turn(Direction dir) 
    {
        direction = dir;
    }

    protected void AttemptAttack(Direction attackDir)
    {
        var playerPos = transform.position;
        Vector2 attackPos1;
        Vector2 attackPos2;
        Vector2 attackPos3;
        Vector2 attackPos4;

        RaycastHit2D hit1;
        RaycastHit2D hit2;
        RaycastHit2D hit3;
        RaycastHit2D hit4;

        var attackRange = new List<RaycastHit2D>();

        switch (attackDir)
        {
            case (Direction.NORTH):
            // bottom of T
            attackPos1 = new Vector2(playerPos.x, playerPos.y + base.pixelsPerTile);
            hit1 = Physics2D.Raycast(attackPos1, Vector2.zero);
            attackRange.Add(hit1);

            // center of T
            attackPos2 = new Vector2(playerPos.x, playerPos.y + 2 * base.pixelsPerTile);
            hit2 = Physics2D.Raycast(attackPos2, Vector2.zero);
            attackRange.Add(hit2);

            // topleft of T
            attackPos3 = new Vector2(playerPos.x - base.pixelsPerTile, playerPos.y + 2 * base.pixelsPerTile);
            hit3 = Physics2D.Raycast(attackPos3, Vector2.zero);
            attackRange.Add(hit3);

            // topright of T
            attackPos4 = new Vector2(playerPos.x + base.pixelsPerTile, playerPos.y + 2 * base.pixelsPerTile);
            hit4 = Physics2D.Raycast(attackPos4, Vector2.zero);
            attackRange.Add(hit4);
            break;

            case (Direction.EAST):
            // bottom of T
            attackPos1 = new Vector2(playerPos.x + base.pixelsPerTile, playerPos.y);
            hit1 = Physics2D.Raycast(attackPos1, Vector2.zero);
            attackRange.Add(hit1);
                
            // center of T
            attackPos2 = new Vector2(playerPos.x + 2*base.pixelsPerTile, playerPos.y);
            hit2 = Physics2D.Raycast(attackPos2, Vector2.zero);
            attackRange.Add(hit2);

            // topleft of T
            attackPos3 = new Vector2(playerPos.x + 2*base.pixelsPerTile, playerPos.y + base.pixelsPerTile);
            hit3 = Physics2D.Raycast(attackPos3, Vector2.zero);
            attackRange.Add(hit3);

            // topright of T
            attackPos4 = new Vector2(playerPos.x + 2*base.pixelsPerTile, playerPos.y - base.pixelsPerTile);
            hit4 = Physics2D.Raycast(attackPos4, Vector2.zero);
            attackRange.Add(hit4);
            break;

            case (Direction.SOUTH):
            // bottom of T
            attackPos1 = new Vector2(playerPos.x, playerPos.y - base.pixelsPerTile);
            hit1 = Physics2D.Raycast(attackPos1, Vector2.zero);
            attackRange.Add(hit1);

            // center of T
            attackPos2 = new Vector2(playerPos.x, playerPos.y - 2*base.pixelsPerTile);
            hit2 = Physics2D.Raycast(attackPos2, Vector2.zero);
            attackRange.Add(hit2);

            // topleft of T
            attackPos3 = new Vector2(playerPos.x - base.pixelsPerTile + base.pixelsPerTile, playerPos.y - 2*base.pixelsPerTile);
            hit3 = Physics2D.Raycast(attackPos3, Vector2.zero);
            attackRange.Add(hit3);

            // topright of T
            attackPos4 = new Vector2(playerPos.x + base.pixelsPerTile - base.pixelsPerTile, playerPos.y - 2*base.pixelsPerTile);
            hit4 = Physics2D.Raycast(attackPos4, Vector2.zero);
            attackRange.Add(hit4);
            break;

            case (Direction.WEST):
            // bottom of T
            attackPos1 = new Vector2(playerPos.x - base.pixelsPerTile, playerPos.y);
            hit1 = Physics2D.Raycast(attackPos1, Vector2.zero);
            attackRange.Add(hit1);

            // center of T
            attackPos2 = new Vector2(playerPos.x - 2 * base.pixelsPerTile, playerPos.y);
            hit2 = Physics2D.Raycast(attackPos2, Vector2.zero);
            attackRange.Add(hit2);

            // topleft of T
            attackPos3 = new Vector2(playerPos.x - 2 * base.pixelsPerTile, playerPos.y + base.pixelsPerTile);
            hit3 = Physics2D.Raycast(attackPos3, Vector2.zero);
            attackRange.Add(hit3);

            // topright of T
            attackPos4 = new Vector2(playerPos.x - 2 * base.pixelsPerTile, playerPos.y - base.pixelsPerTile);
            hit4 = Physics2D.Raycast(attackPos4, Vector2.zero);
            attackRange.Add(hit4);
            break;
        }
        if (attackRange.Any(h => h.collider != null)) {
            //checkHit(hit);
            print("HIT");
        }
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
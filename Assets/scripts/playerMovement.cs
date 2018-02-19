using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;      //Allows us to use SceneManager
using Util;
using Weapons;

//Player inherits from MovingObject, our base class for objects that can move, Enemy also inherits from this.
public class playerMovement : movingObject
{
    public  float       restartLevelDelay   = 1f;                  //Delay time in seconds to restart level.
    private Animator    animator;                                  //store a reference to the Player's animator component.
    public  Animator    weaponAnimator;                           //store a reference to the weapon animator component.

    public  Direction   direction           = Direction.NORTH;    //enum for direction facing
    public  Weapon      currentWeapon       = new BaseSword();    //

    //Weapons Testing
    private List<Weapon> weaponsTest = new List<Weapon>{ new BaseSword(), new BroadSword(), new TSword()};
    private int weaponNum = 0;


    //Start overrides the Start function of MovingObject
    protected override void Start ()
    {
        animator = GetComponent<Animator>();
        //Start function of the movingObject base class.
        base.Start ();
    }

    private void OnEnable()
    {
        gameManager.OnStartTurn += OnStart;
        gameManager.OnEndTurn += OnEnd;
    }

    private void OnStart()
    {
        // Event for when a Turn starts
        //print("PLAYER START");

    }

    private void OnEnd()
    {
        // Event for when a Turn ends
        //print("PLAYER END");
    }

    //This function is called when the behaviour becomes disabled or inactive.
    private void OnDisable ()
    {
        gameManager.OnStartTurn -= OnStart;
        gameManager.OnEndTurn -= OnEnd;
    }
    
    private void Update ()
    {
        if (gameManager.instance.IsTurnInProgress())
        {
            gameManager.instance.CountFrame();
            return;
        }

        //variables
        int horizontal = 0;
        int vertical = 0;
        bool madeMove = false;

        if (Input.GetKeyDown("w"))
        {
            vertical += 1;
            Turn(Direction.NORTH);
            madeMove = true;
        }
        else if (Input.GetKeyDown("s"))
        {
            vertical -= 1;
            Turn(Direction.SOUTH);
            madeMove = true;
        }
        else if (Input.GetKeyDown("a"))
        {
            horizontal -= 1;
            Turn(Direction.WEST);
            madeMove = true;
        }
        else if (Input.GetKeyDown("d"))
        {
            horizontal += 1;
            Turn(Direction.EAST);
            madeMove = true;
        }

        if (Input.GetKeyDown("k"))
        {
            if (weaponNum >= weaponsTest.Count) weaponNum = 0;
            currentWeapon = weaponsTest.ElementAt(weaponNum);
            print("Equipped " + currentWeapon.GetType().ToString());
            weaponNum++;
        }

        if(madeMove)
        {
            //prevent diagonal movements
            if (horizontal != 0)
            {
                vertical = 0;
            }

            if(CanAttack(direction) || CanMove(horizontal, vertical))
            {
                gameManager.instance.StartTurn();
                if(!AttemptAttack(direction))
                {
                    gameManager.instance.SetPlayerIsMoving(true);
                    AttemptMove<BoxCollider>(horizontal, vertical);
                }
            }
        }
    }

    protected override bool CanMove(int xDir, int yDir)
    {
        return base.CanMove(xDir, yDir);
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
            print("Move");
            if(direction == Direction.SOUTH){
                animator.SetTrigger("MoveDown");
            }else if(direction == Direction.NORTH){
                animator.SetTrigger("MoveUp");
            }else{
                animator.SetTrigger("MoveRight");
            }
        //If Move returns true, meaning Player was able to move into an empty space.
        if (didMove) 
        {

            
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

    protected bool CanAttack(Direction attackDir)
    {
        var attackRange = currentWeapon.GetHitsForPositionAndDirection(transform.position, attackDir);
        if (attackRange.Any(h => {
            //print(h.transform == null ? null : h.transform.name);
            return h.transform == null ? false : h.transform.CompareTag("Enemy");
        }))
        {
            return true;
        }
        return false;
    }

    protected bool AttemptAttack(Direction attackDir)
    {
        var attackRange = currentWeapon.GetHitsForPositionAndDirection(transform.position, attackDir);
        if (attackRange.Any(h => {
            return h.transform == null ? false : h.transform.CompareTag("Enemy");
        }))
        {
            //On an attack, we make the player "move up" just so that the timing for TurnEnd() is exactly synchronized with a player actually moving
            gameManager.instance.SetPlayerIsMoving(false);
            AttemptMove<BoxCollider>(0, 1);
            weaponAnimator.SetTrigger(currentWeapon.GetType().Name);
            print("Hit");
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
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

    public  Direction   currentDirection    = Direction.NORTH;    //enum for direction facing
    public  Weapon      currentWeapon       = new BaseSword();    //

    //Weapons Testing
    private List<Weapon> weaponsTest = new List<Weapon>{ new BaseSword(), new BroadSword(), new TSword()};
    private int weaponNum = 0;

    private Vector2 lastPosRelative = new Vector2(0, 0);

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
        CheckIfEnemyOverlap();
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

        if (gameManager.instance.IsPlayerKnockedBack())
        {
            return;
        }

        //variables
        int horizontal = 0;
        int vertical = 0;
        bool madeMove = false;

        if (Input.GetKeyDown("w"))
        {
            vertical += 1;
            currentDirection = Direction.NORTH;
            madeMove = true;
        }
        else if (Input.GetKeyDown("s"))
        {
            vertical -= 1;
            currentDirection = Direction.SOUTH;
            madeMove = true;
        }
        else if (Input.GetKeyDown("a"))
        {
            horizontal -= 1;
            currentDirection = Direction.WEST;
            GetComponent<SpriteRenderer>().flipX = true;
            madeMove = true;
        }
        else if (Input.GetKeyDown("d"))
        {
            horizontal += 1;
            currentDirection = Direction.EAST;
            GetComponent<SpriteRenderer>().flipX = false;
            madeMove = true;
        }

        //debug
        if (Input.GetKeyDown("k"))
        {
            if (weaponNum >= weaponsTest.Count) weaponNum = 0;
            currentWeapon = weaponsTest.ElementAt(weaponNum);
            print("Equipped " + currentWeapon.GetType().ToString());
            weaponNum++;
        }

        if (Input.GetKeyDown("q"))
        {
            print("FORCE TURN");
            gameManager.instance.StartTurn();
            StartCoroutine(gameManager.instance.EndTurn());
        }
        //---

        if (madeMove)
        {
            //prevent diagonal movements
            if (horizontal != 0)
            {
                vertical = 0;
            }

            if(CanAttack() || CanMove(horizontal, vertical))
            {
                gameManager.instance.StartTurn();
                if(!AttemptAttack())
                {
                    PlayerMove(horizontal, vertical);
                }
            }
        }
    }

    private void PlayerMove(int horizontal, int vertical)
    {
        gameManager.instance.SetPlayerIsMoving(true);
        AttemptMove<BoxCollider>(horizontal, vertical);
        lastPosRelative = new Vector2(horizontal, vertical);
    }

    //AttemptMove overrides the AttemptMove function in the base class MovingObject
    //AttemptMove takes a generic parameter T which for Player will be of the type Wall, it also takes integers for x and y direction to move in.
    protected override bool AttemptMove <T> (int xDir, int yDir)
    {
        //Every time player moves, subtract from food points total.
        
        //Call the AttemptMove method of the base class, passing in the component T (in this case Wall) and x and y direction to move.
        bool moved = base.AttemptMove <T> (xDir, yDir);

        print("Move");
        if(currentDirection == Direction.SOUTH)
        {
            animator.SetTrigger("MoveDown");
        }
        else if(currentDirection == Direction.NORTH)
        {
            animator.SetTrigger("MoveUp");
        }else
        {
            animator.SetTrigger("MoveRight");
        }

        if (moved) 
        {

            
        }

        return moved;
    }
    
    
    //OnCantMove overrides the abstract function OnCantMove in MovingObject.
    //It takes a generic parameter T which in the case of Player is a Wall which the player can attack and destroy.
    protected override void OnCantMove <T> (T component)
    {
        //print("CANT MOVE");
    }

    protected bool CanAttack()
    {
        var attackRange = currentWeapon.GetHitsForPositionAndDirection(transform.position, currentDirection);
        if (attackRange.Any(h => {
            //print(h.transform == null ? null : h.transform.name);
            return h.transform == null ? false : h.transform.CompareTag("Enemy");
        }))
        {
            return true;
        }
        return false;
    }

    protected bool AttemptAttack()
    {
        var attackRange = currentWeapon.GetHitsForPositionAndDirection(transform.position, currentDirection);
        if (attackRange.Any(h => {
            return h.transform == null ? false : h.transform.CompareTag("Enemy");
        }))
        {
            //On an attack, we make the player "move up" just so that the timing for TurnEnd() is exactly synchronized with a player actually moving
            gameManager.instance.SetPlayerIsMoving(false);
            AttemptMove<BoxCollider>(0, 1);

            RotateAttackAnimation();

            print("Hit");
            return true;
        }
        return false;
    }

    private void RotateAttackAnimation()
    {
        int rotations = 0;
        weaponAnimator.transform.rotation = new Quaternion();
        switch (currentDirection)
        {
            case (Direction.NORTH):
                weaponAnimator.transform.position = new Vector2(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y + gameManager.yTileSize));
                rotations = 1;
                break;
            case (Direction.EAST):
                weaponAnimator.transform.position = new Vector2(Mathf.RoundToInt(transform.position.x + gameManager.xTileSize), Mathf.RoundToInt(transform.position.y));
                rotations = 0;
                break;
            case (Direction.SOUTH):
                weaponAnimator.transform.position = new Vector2(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y - gameManager.yTileSize));
                rotations = 3;
                break;
            case (Direction.WEST):
                weaponAnimator.transform.position = new Vector2(Mathf.RoundToInt(transform.position.x - gameManager.xTileSize), Mathf.RoundToInt(transform.position.y));
                rotations = 2;
                break;
        }
        weaponAnimator.transform.RotateAround(weaponAnimator.transform.position, new Vector3(0, 0, 1), 90f * rotations);
        weaponAnimator.SetTrigger(currentWeapon.GetType().Name);
    }

    private void CheckIfEnemyOverlap()
    {
        //Disable the boxCollider so that linecast doesn't hit this object's own collider.
        GetComponent<BoxCollider2D>().enabled = false;

        var hit = Physics2D.Raycast(transform.position, Vector2.zero);
        if (hit.collider != null)
        {
            print("OVERLAP");
            KnockBackPlayer();
        }

        GetComponent<BoxCollider2D>().enabled = true;
    }

    // Player is knocked back to their last position whenever they occupy the same space as an enemy.
    private void KnockBackPlayer()
    {
        gameManager.instance.SetPlayerIsKnockedBack(true);
        PlayerMove(-Mathf.RoundToInt(lastPosRelative.x), -Mathf.RoundToInt(lastPosRelative.y));
        // OnHit()
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
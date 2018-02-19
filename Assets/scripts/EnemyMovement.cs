using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;      //Allows us to use SceneManager
using Util;

public class EnemyMovement : movingObject
{
    public float restartLevelDelay = 1f;        //Delay time in seconds to restart level.
    public int wallDamage = 1;                  //How much damage a player does to a wall when chopping it.
    private Animator animator;                  //Used to store a reference to the Player's animator component.
    public Direction direction = Direction.NORTH;
    private bool debounce = true;
    private bool stunned = false;
    private bool hitAnim = true;

    //public Weapon currentWeapon = new BaseSword();

    //Start overrides the Start function of MovingObject
    protected override void Start()
    {
        animator = GetComponent<Animator>();
        base.Start();
    }


    private void Update()
    {
        //if the player starts to move and not being debounced
        if (!gameManager.instance.GetCanMove() && debounce)
        {
            debounce = false;
            if(!stunned)
            {
                AttemptMove<BoxCollider>(0, -1);
            }
        }
        //if the player is done moving and being debounced
        else if (gameManager.instance.GetCanMove() && !debounce)
        {
            debounce = true;
            stunned = false;
        }
    }

    public void OnHit()
    {
        if (hitAnim)
        {
            StartCoroutine(FlashRed());
        }
    }

    private IEnumerator FlashRed()
    {
        hitAnim = false;
        GetComponent<SpriteRenderer>().color = Color.red;
        yield return new WaitForSeconds(0.1f);
        GetComponent<SpriteRenderer>().color = Color.white;
        hitAnim = true;
    }

    public void Stun()
    {
        stunned = true;
    }


    protected override bool AttemptMove<T>(int xDir, int yDir)
    {
        base.AttemptMove<T>(xDir, yDir);
        RaycastHit2D hit;
        bool didMove = Move(xDir, yDir, out hit);
        if (didMove)
        {
            //Call RandomizeSfx of SoundManager to play the move sound, passing in two audio clips to choose from.
        }
        return didMove;
    }


    //OnCantMove overrides the abstract function OnCantMove in MovingObject.
    //It takes a generic parameter T which in the case of Player is a Wall which the player can attack and destroy.
    protected override void OnCantMove<T>(T component)
    {
        print("CANT MOVE");
    }

    protected void Turn(Direction dir)
    {
        direction = dir;
    }

    protected bool AttemptAttack(Direction attackDir)
    {
        //var attackRange = currentWeapon.GetHitsForPositionAndDirection(transform.position, attackDir, base.pixelsPerTile);
        //if (attackRange.Any(h => h.collider != null))
        //{
        //    //checkHit(hit);
        //    print("HIT");
        //    return true;
        //}
        //return false;
        return false;
    }


    //OnTriggerEnter2D is sent when another object enters a trigger collider attached to this object (2D physics only).
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Exit")
        {
            //Invoke the Restart function to start the next level with a delay of restartLevelDelay (default 1 second).
            Invoke("Restart", restartLevelDelay);
            //Disable the player object since level is over.
            enabled = false;
        }
    }


    //Restart reloads the scene when called.
    private void Restart()
    {
        SceneManager.LoadScene(0);
    }


}
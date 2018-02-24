using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;      //Allows us to use SceneManager
using Util;

public class EnemyMovement : movingObject
{
    public float restartLevelDelay = 1f;        //Delay time in seconds to restart level.
    public int wallDamage = 1;                  //How much damage a player does to a wall when chopping it.
    private Animator animator;                  //Used to store a reference to the Player's animator component.
    public Direction direction = Direction.NORTH;
    private Transform target;

    private bool stunned = false;
    private bool hitAnim = true;

    private static List<EnemyMovement> activeEnemies = null;
    private static List<Vector2> selectedMoves = new List<Vector2>();
    public int Priority = 0;                   // TODO make this an enum or something?

    //public Weapon currentWeapon = new BaseSword();

    protected override void Start()
    {
        animator = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        base.Start();
    }


    private void OnEnable()
    {
        gameManager.OnStartTurn += OnEnemyTurnStart;
        gameManager.OnEndTurn += OnEnemyTurnEnd;
    }

    public static void Go()
    {
        foreach (var enemy in activeEnemies.OrderByDescending(e => e.Priority))
        {
            Vector2 nextMove = enemy.GetNextMove();
            int xDir = Mathf.RoundToInt(nextMove.x);
            int yDir = Mathf.RoundToInt(nextMove.y);

            Vector2 start = enemy.transform.position;
            Vector2 end = start + new Vector2(xDir * gameManager.xTileSize, yDir * gameManager.yTileSize);

            bool duplicateMove = false;

            if (selectedMoves.Any(p => 
            Mathf.Abs(p.x - end.x) < float.Epsilon && 
            Mathf.Abs(p.y - end.y) < float.Epsilon))
            {
                print("DUPLICATE MOVE");
                duplicateMove = true;
            }

            if (enemy.CanMove(xDir, yDir) 
                && !duplicateMove
                && !enemy.stunned)
            {
                selectedMoves.Add(end);
                enemy.AttemptMove<BoxCollider>(xDir, yDir);
                print("Enemy " + enemy.Priority + " moved!");
            }
        }
    }

    public static void Stop()
    {
        selectedMoves = new List<Vector2>();
        foreach(var enemy in activeEnemies)
        {
            enemy.stunned = false;
        }
    }

    private void OnEnemyTurnStart()
    {

    }

    private void OnEnemyTurnEnd()
    {

    }

    //This function is called when the behaviour becomes disabled or inactive.
    private void OnDisable()
    {
        //When Player object is disabled, store the current local food total in the GameManager so it can be re-loaded in next level.
        //GameManager.instance.playerFoodPoints = food;
    }


    private void Update()
    {
        //Enemies move on TurnStart() event, so no movement logic is necessary on Update()
    }

    public static IEnumerable<EnemyMovement> GetActiveEnemies()
    {
        if(activeEnemies == null)
        {
            activeEnemies = new List<EnemyMovement>();
        }

        return activeEnemies;
    }

    public static IEnumerable<EnemyMovement> AddActiveEnemy(EnemyMovement newEnemy)
    {
        if (activeEnemies == null)
        {
            activeEnemies = new List<EnemyMovement>();
        }

        activeEnemies.Add(newEnemy);
        return activeEnemies;
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

    //TODO improve enemy AI
    protected Vector2 GetNextMove()
    {
        int xDir = target.position.x > transform.position.x ? 1 : -1;
        int yDir = target.position.y > transform.position.y ? 1 : -1;

        // if it doesn't matter whether the enemy moves horizontally or vertically then flip a coin
        if (Mathf.Abs(target.position.x - transform.position.x) > float.Epsilon 
            && Mathf.Abs(target.position.y - transform.position.y) > float.Epsilon)
        {
            int rng = Random.Range(0, 2);
            if (rng > 0)
            {
                return new Vector2(xDir, 0);
            }
            else
            {
                return new Vector2(0, yDir);
            }
        }
        else if(Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon)
        {
            return new Vector2(0, yDir);
        }

        return new Vector2(xDir, 0);
    }

    protected override bool CanMove(int xDir, int yDir)
    {
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(xDir * gameManager.xTileSize, yDir * gameManager.yTileSize);

        //Disable the boxCollider so that linecast doesn't hit this object's own collider.
        GetComponent<BoxCollider2D>().enabled = false;

        //Cast a line from start point to end point checking collision on blockingLayer.

        RaycastHit2D hit = Physics2D.Linecast(start, end, blockingLayer);

        //Re-enable boxCollider after linecast
        GetComponent<BoxCollider2D>().enabled = true;

        //Check if anything was hit
        if (hit.transform == null)
        {
            return true;
        } 
        else if(hit.transform.tag == "Player")
        {
            //player is infront of enemy --- attack player
            print("PLAYER HIT");
        }
        return false;
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
        else
        {
            // didnt move
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
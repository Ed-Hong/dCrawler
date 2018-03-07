using UnityEngine;
using System.Collections;

    //The abstract keyword enables you to create classes and class members that are incomplete and must be implemented in a derived class.
    public abstract class movingObject : MonoBehaviour
    {
        protected int           speed               = 35;              //nebulous int that effects the speed of the move transition
        private float           moveTime            = 0.1f;             //???
        private float           inverseMoveTime;                        //Used to make movement more efficient.

        private BoxCollider2D   boxCollider;                           //The BoxCollider2D component attached to this object.
        private Rigidbody2D     rb2D;                                  //The Rigidbody2D component attached to this object.
        public LayerMask        blockingLayer;                         //Layer on which collision will be checked.
        public int              pixelsPerTile = 27;                          //eddy make this into x and y 

        //Protected, virtual functions can be overridden by inheriting classes.
        protected virtual void Start ()
        {
            //Get a component reference to this object's BoxCollider2D
            boxCollider = GetComponent <BoxCollider2D> ();
            //Get a component reference to this object's Rigidbody2D
            rb2D = GetComponent <Rigidbody2D> ();
            
            //By storing the reciprocal of the move time we can use it by multiplying instead of dividing, this is more efficient.
            inverseMoveTime = 1f / moveTime;
        }

        // only returns if it can or can't move
        protected virtual bool CanMove(int xDir, int yDir)
        {
            Vector2 start = transform.position;
            Vector2 end = start + new Vector2(xDir * gameManager.xTileSize, yDir * gameManager.yTileSize);

            //Disable the boxCollider so that linecast doesn't hit this object's own collider.
            boxCollider.enabled = false;

            //Cast a line from start point to end point checking collision on blockingLayer.
            RaycastHit2D hit = Physics2D.Linecast(start, end, blockingLayer);

            //Re-enable boxCollider after linecast
            boxCollider.enabled = true;

            //Check if anything was hit
            if (hit.transform == null)
            {
                return true;
            }
            return false;
        }

        // will move if possible and return true. if can't move, returns false
        protected bool Move (int xDir, int yDir, out RaycastHit2D hit)
        {
            Vector2 start = transform.position;
            Vector2 end = start + new Vector2 (xDir * gameManager.xTileSize, yDir * gameManager.yTileSize); 
            boxCollider.enabled = false;

            hit = Physics2D.Linecast(start, end, blockingLayer);
            if (!gameManager.instance.IsPlayerMoving() || gameManager.instance.IsPlayerKnockedBack())
            {
                //don't check for collision in cases where colliders would overlap
                hit = Physics2D.Linecast(start, end, 0);
            }

            boxCollider.enabled = true;
            
            if(hit.transform == null)
            {
                StartCoroutine (SmoothMovement (end));
                GetComponent<SpriteRenderer>().sortingOrder = -(int)(end.y / gameManager.yTileSize);
                return true;
            }
            return false;
        }
        
        
        //Co-routine for moving units from one space to next, takes a parameter end to specify where to move to.

        protected IEnumerator SmoothMovement (Vector3 end)
        {
            //Calculate the remaining distance to move based on the square magnitude of the difference between current position and end parameter. 
            //Square magnitude is used instead of magnitude because it's computationally cheaper.
            float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

            //While that distance is greater than a very small amount (Epsilon, almost zero):
            Vector2 tempPos = new Vector2(rb2D.position.x, rb2D.position.y);
            while (sqrRemainingDistance > float.Epsilon)
            {
                //Find a new position proportionally closer to the end, based on the moveTime
                Vector3 newPostion = Vector3.MoveTowards(tempPos, end, inverseMoveTime * ((1.5f*speed) * Time.deltaTime));
                //Call MovePosition on attached Rigidbody2D and move it to the calculated position.
                if (GetComponent<BoxCollider2D>().transform.tag == "Player")
                {
                    if (gameManager.instance.IsPlayerMoving())
                    {
                        rb2D.MovePosition(newPostion);
                        tempPos = new Vector2(rb2D.position.x, rb2D.position.y);
                    }
                    else
                    {
                        tempPos = newPostion;
                    }
                }
                else
                {
                    rb2D.MovePosition(newPostion);
                    tempPos = new Vector2(rb2D.position.x, rb2D.position.y);
                }

                //Recalculate the remaining distance after moving.
                sqrRemainingDistance = (newPostion - end).sqrMagnitude;
                
                //Return and loop until sqrRemainingDistance is close enough to zero to end the function
                yield return null;
            }
            if (GetComponent<BoxCollider2D>().transform.tag == "Player")
            {
                StartCoroutine(gameManager.instance.EndTurn());
            }
        }
        
        
        //The virtual keyword means AttemptMove can be overridden by inheriting classes using the override keyword.
        //AttemptMove takes a generic parameter T to specify the type of component we expect our unit to interact with if blocked (Player for Enemies, Wall for Player).
        protected virtual bool AttemptMove <T> (int xDir, int yDir) 
        where T : Component
        {
            //Hit will store whatever our linecast hits when Move is called.
            RaycastHit2D hit;
            
            //Set canMove to true if Move was successful, false if failed.
            bool canMove = Move (xDir, yDir, out hit);
            
            //Check if nothing was hit by linecast
            if(hit.transform == null)
                return false;
            
            //Get a component reference to the component of type T attached to the object that was hit
            T hitComponent = hit.transform.GetComponent<T> ();
            
            //if hitComponent is not equal to null, meaning MovingObject is blocked and has hit something it can interact with.
            if(hitComponent != null)
            {
                //Call the OnCantMove function and pass it hitComponent as a parameter.
                OnCantMove(hitComponent);
            }
            return canMove;
        }
        
        
        //The abstract modifier indicates that the thing being modified has a missing or incomplete implementation.
        //OnCantMove will be overriden by functions in the inheriting classes.
        protected abstract void OnCantMove <T> (T component)
            where T : Component;
    }
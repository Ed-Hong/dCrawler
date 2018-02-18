using System.Collections.Generic;
using UnityEngine;

namespace Util
{
    public abstract class Weapon
    {
        // base atk positions assuming player is facing NORTH
        protected IEnumerable<Vector2> AtkPositions { get; set; }

        public virtual IEnumerable<RaycastHit2D> GetHitsForPositionAndDirection(Vector2 playerPos, Direction direction)
        {
            var hits = new List<RaycastHit2D>();
            foreach (Vector2 pos in AtkPositions)
            {
                hits.Add(Physics2D.Raycast(TransformAtkPoint(playerPos, pos, direction), Vector2.zero));
            }
            return hits;
        }

        private Vector2 TransformAtkPoint(Vector2 playerPos, Vector2 orig, Direction direction)
        {
            Vector2 newAtkPos;
            switch (direction)
            {
                case (Direction.EAST):
                    newAtkPos = new Vector2(orig.y * gameManager.xTileSize, -orig.x * gameManager.yTileSize);
                    break;
                case (Direction.SOUTH):
                    newAtkPos = new Vector2(-orig.x * gameManager.xTileSize, -orig.y * gameManager.yTileSize);
                    break;
                case (Direction.WEST):
                    newAtkPos = new Vector2(-orig.y * gameManager.xTileSize, orig.x * gameManager.yTileSize);
                    break;
                default:
                    newAtkPos = new Vector2(orig.x * gameManager.xTileSize, orig.y * gameManager.yTileSize);
                    break;
            }

            return (playerPos + newAtkPos);
        }
    }
}

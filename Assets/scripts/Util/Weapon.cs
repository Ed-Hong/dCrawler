using System.Collections.Generic;
using UnityEngine;

namespace Util
{
    public abstract class Weapon
    {
        // base atk positions assuming player is facing NORTH
        protected IEnumerable<Vector2> AtkPositions { get; set; }

        public virtual IEnumerable<RaycastHit2D> GetHitsForPositionAndDirection(Vector2 playerPos, Direction direction, int pixelsPerTile)
        {
            var hits = new List<RaycastHit2D>();
            foreach (Vector2 pos in AtkPositions)
            {
                hits.Add(Physics2D.Raycast(TransformAtkPoint(playerPos, pos, direction, pixelsPerTile), Vector2.zero));
            }
            return hits;
        }

        private Vector2 TransformAtkPoint(Vector2 playerPos, Vector2 orig, Direction direction, int pixelsPerTile)
        {
            Vector2 newAtkPos;
            switch (direction)
            {
                case (Direction.EAST):
                    newAtkPos = new Vector2(orig.y, -orig.x);
                    break;
                case (Direction.SOUTH):
                    newAtkPos = new Vector2(-orig.x, -orig.y);
                    break;
                case (Direction.WEST):
                    newAtkPos = new Vector2(-orig.y, orig.x);
                    break;
                default:
                    newAtkPos = orig;
                    break;
            }

            return (playerPos + newAtkPos * pixelsPerTile);
        }
    }
}

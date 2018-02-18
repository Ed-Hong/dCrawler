using System.Collections.Generic;
using UnityEngine;
using Util;

namespace Weapons
{
    public class BroadSword : Weapon
    {
        public BroadSword()
        {
            AtkPositions = new List<Vector2>
            {
                new Vector2(0,1),
                new Vector2(-1,1),
                new Vector2(1,1)
            };
        }
    }
}

using System.Collections.Generic;
using UnityEngine;
using Util;

namespace Weapons
{
    public class TSword : Weapon
    {
        public TSword()
        {
            AtkPositions = new List<Vector2>
            {
                new Vector2(0,1),
                new Vector2(0,2),
                new Vector2(-1,2),
                new Vector2(1,2)
            };
        }
    }
}

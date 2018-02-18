using System.Collections.Generic;
using UnityEngine;
using Util;

namespace Weapons
{
    public class BaseSword : Weapon
    {
        public BaseSword()
        {
            AtkPositions = new List<Vector2>
            {
                new Vector2(0,1)
            };
        }
    }
}

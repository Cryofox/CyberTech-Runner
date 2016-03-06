using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
namespace RTS_Moba.Characters
{
    class Crawler : Character
    {
        //Average unit.
        public Crawler()
        {
            hp = 40;
            attackDamage = 5;
            moveSpeed = 6;
            attackRange = 1;
            attackSpeed = 1;
            sightRange  = 10;
            hp_Current = hp;
        }

        //Choose Random Location, Hunt for Players.
        protected override void Update_AILogic(float timeDelta)
        {
            if ( (targetLocation == Vector3.zero || Vector3.Distance(targetLocation, location) < 2) )
            {
                System.Random r = new System.Random();
                targetLocation = new Vector3(r.Next(0, 100), 0.5f, r.Next(0, 100));
                currentOrder = order_Types.huntMove;

            }
        }



    }
}

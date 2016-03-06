using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS_Moba.Characters
{
    class Soldier : Character
    {
        public Soldier():base()
        {
            faction = Faction.player;
            moveSpeed = 5;
            attackSpeed =  0.5f;
            attackDamage = 10;
            hp = 85;
            hp_Current = hp;
        }





    }
}

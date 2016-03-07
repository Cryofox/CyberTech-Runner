using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RTS_Moba.Characters.Enemies
{
    //Extends Soldier
    class Guard : Soldier
    {

        public Guard():base()
        {
            faction = Faction.ai;
        }

        protected override void Update_AILogic(float timeDelta)
        {
            //Guard will...do nothing.

        }

    }
}

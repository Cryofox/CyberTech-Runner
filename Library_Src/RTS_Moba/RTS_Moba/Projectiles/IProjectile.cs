using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

using RTS_Moba.Characters;
namespace RTS_Moba.Projectiles
{
    interface IProjectile
    {

        void Update(float timeDelta);
        void Disable();

        void SetFireDirection(Vector3 start, Vector3 direction, Faction shooterFaction);

        bool GetActive();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RTS_Moba.Characters;
namespace RTS_Moba.Projectiles
{
    class ProjectileManager
    {
        #region Singleton Constructor
        //Singleton Constructor
        private static readonly ProjectileManager _instance = new ProjectileManager();

        public static ProjectileManager instance
        {
            get
            {
                return _instance;
            }
        }
        #endregion

        //Bullet Projectiles
        List<IProjectile> projectiles;
        private ProjectileManager()
        {
            projectiles = new List<IProjectile>();
            for (int i = 0; i < 10; i++)
            {
                projectiles.Add(new Bullet());
            }

        }

        public void Update(float timeDelta)
        {
            for(int i=0;i< projectiles.Count;i++)
            {
                projectiles[i].Update(timeDelta);
            }
        }

        public void SpawnProjectile(Vector3 start, Vector3 normal, Faction shooterFaction)
        {
            for (int i = 0; i < projectiles.Count; i++)
                if (!projectiles[i].GetActive())
                {
                    projectiles[i].SetFireDirection(start, normal, shooterFaction);
                    return;
                }
        }


    }
}

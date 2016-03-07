using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;


using RTS_Moba.Pathfind;
using RTS_Moba.Characters;
namespace RTS_Moba.Projectiles
{
    class Bullet : IProjectile
    {

        GameObject go;
        Vector3 fireDirection = Vector3.zero;

        int damage = 20;
        float speed = 20;

        Vector3 location;

        Faction shooterFaction = Faction.ai;
        public Bullet()
        {
            go = ResourceManager.Get_Prefab("Bullet");
            go = GameObject.Instantiate(go, Vector3.zero, Quaternion.identity) as GameObject;
            go.SetActive(false);

        }

        void IProjectile.Disable()
        {
            throw new NotImplementedException();
        }

        void IProjectile.SetFireDirection(Vector3 start, Vector3 direction, Faction shooterFaction)
        {
            this.shooterFaction = shooterFaction;
            location = start + direction*2;
            fireDirection = direction;
            go.SetActive(true);
            go.transform.rotation = Quaternion.LookRotation(fireDirection);
        }

        void IProjectile.Update(float timeDelta)
        {
            if (!go.activeSelf)
                return;

            Vector3 newLocation = Vector3.Normalize(fireDirection) * speed * timeDelta;


            // 1 << 9 = Ignore all but 9
            // ~(1<<9) = Ignore only 9, ~ = Complement.
            int mask = (1 << 9); //Ignore all Layers except the CollisionPlane Layer

            //Theres a small chance this Projectile may crash into something between Location and new Location.
            //Use Unity's Colliders for this.
            Ray ray = new Ray(location, fireDirection);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, (speed * timeDelta), mask))
            {
                //The Object now...
                Character character = CharacterManager.instance.Get_CharAt((int)hit.transform.position.x, (int)hit.transform.position.z);
                if(character!= null)
                { 
                    character.TakeDamage(damage);

                }
                go.SetActive(false);
                return;
            }


            location += newLocation;
            go.transform.position = location;
        }

        bool IProjectile.GetActive()
        {
            return go.activeSelf;
        }
    }
}

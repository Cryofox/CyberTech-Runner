using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


using RTS_Moba.Pathfind;
using RTS_Moba.Controllers;
using RTS_Moba.UI;
namespace RTS_Moba.Controllers
{
    class SquadController
    {
        //Initialize a ShieldManager.
        static ShieldManager shieldMan = new ShieldManager();

        public static List<Vector3> Get_SquadPoint(int squadNum, Vector3 targetLocation)
        {
            List<Vector3> takenSpots = new List<Vector3>();

            for (int i = 0; i < squadNum; i++)
            {
                Vector3 spot = targetLocation;
                spot = GetBestPoint(takenSpots, spot);

                takenSpots.Add(spot);
            }



            //Find Nearest Available Spot that is (A) Not Taken, and (B) Not a Wall


            //Show the shields for this Location.
            ShowShields(takenSpots);
            return takenSpots;
        }
        static Vector3 GetBestPoint(List<Vector3> takenSpots, Vector3 wantedSpot)
        {
            Vector3 idealSpot = wantedSpot;
            bool spotFound = false;

            int oX = (int)wantedSpot.x;
            int oY = (int)wantedSpot.z;

            //Growing Outwars is the Plan
            for(int i=0; !spotFound && i< Pathfinder.instance.width; i++)
            {
                //I = Counter
                for (int x = oX-i; x <= oX+i && !spotFound; x++)
                    for (int y = oY-i; y <= oY+i && !spotFound; i++)
                    {
                        if(Pathfinder.instance.isLocationWalkable(x, y))
                        {
                            idealSpot = new Vector3(x, 1, y);
                            spotFound = true;
                            //Check if this spot exists
                            for (int s = 0; s < takenSpots.Count; s++)
                            {
                                if (takenSpots[s] == idealSpot)
                                {
                                    spotFound = false;
                                    break;
                                }
                            }

                            
                        }
                    }
            }

            return idealSpot;
        }


        static void ShowShields(List<Vector3> points)
        {
            Vector3 shieldLoc = Vector3.zero;
            Quaternion shieldRot = Quaternion.identity;
            //First Toggle all shields off.
            shieldMan.Clear();
            for (int i = 0; i < points.Count; i++)
            {
                int x = (int)points[i].x;
                int y = (int)points[i].z;

                int newX = x;
                int newY = y;
                //To Do: Draw 4 Shields one per valid Wall side.
                newX= x + 1;
                if (!Pathfinder.instance.isLocationWalkable(newX, newY))
                {
                    //Need a shield here.
                    shieldLoc.x = x + 0.8f;
                    shieldLoc.y = 1;
                    shieldLoc.z = y +0.5f;
                    shieldRot = Quaternion.Euler(90, 90, 0);

                    shieldMan.ShowShield(shieldLoc, shieldRot);
                }
                newX = x - 1;
                if (!Pathfinder.instance.isLocationWalkable(newX, newY))
                {
                    //Need a shield here.
                    shieldLoc.x = x +0.2f;
                    shieldLoc.y = 1;
                    shieldLoc.z = y + 0.5f;
                    shieldRot = Quaternion.Euler(90, -90, 0);

                    shieldMan.ShowShield(shieldLoc, shieldRot);
                }
                newX = x;
                newY = y + 1;
                if (!Pathfinder.instance.isLocationWalkable(newX, newY))
                {
                    //Need a shield here.
                    shieldLoc.x = x + 0.5f;
                    shieldLoc.y = 1;
                    shieldLoc.z = y + 0.8f;
                    shieldRot = Quaternion.Euler(90, 0, 0);

                    shieldMan.ShowShield(shieldLoc, shieldRot);
                }
                newY = y - 1;
                if (!Pathfinder.instance.isLocationWalkable(newX, newY))
                {
                    //Need a shield here.
                    shieldLoc.x = x + 0.5f;
                    shieldLoc.y = 1;
                    shieldLoc.z = y + 0.2f;
                    shieldRot = Quaternion.Euler(90, 0, 0);

                    shieldMan.ShowShield(shieldLoc, shieldRot);
                }




            }
        }
    }
}

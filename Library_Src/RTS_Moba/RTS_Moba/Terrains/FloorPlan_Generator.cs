using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
namespace RTS_Moba.Terrains
{
    class FloorPlan_Generator
    {
        static System.Random random;
        public static bool[][] Generate_Floor(int width, int height, int seed, int generator)
        {
            random = new System.Random();

            //Create the Bool Map for Pathfinding
            bool[][] map = new bool[width][];

            for (int x = 0; x < width; x++)
            {
                map[x] = new bool[height];
                for (int y = 0; y < height; y++)
                        map[x][y] = true;
            }

            //if 1
            if (generator==2)
                return Generator_2(map);
        
            else
                return Generator_1(map);
        }

        //Gen 1-> 10%, each tile has a 20% chance of being a wall.
        static bool[][] Generator_1(bool[][]map)
        {
            int width = map.Length;
            int height = map[0].Length;

            for (int x = 0; x < width; x++)
            {

                for (int y = 0; y < height; y++)
                {
                    //Load/Create Boolean Map
                    //20% Chance to be false
                    if (random.NextDouble() < 0.1)
                        map[x][y] = false;
                    else
                        map[x][y] = true;

                }
            }

            return map;
        }

        #region Gen2

        //Random Square Placement
        static bool[][] Generator_2(bool[][] map)
        {
            int w = map.Length;
            int h = map[0].Length;

            //Choose a Random Point and Expand untill 20% of the walls are Placed
            int minWallCount = (int) ( (float)w * (float)h * 0.2f);

            int count = 0;

            int px = 0;
            int py = 0;

            int attemptCount = 0;

            
            while (count < minWallCount && attemptCount < 10)
            {
                //Choose a Random Point
                px = random.Next(0, w);
                py = random.Next(0, h);

                //Calc Max Distance to nearest edge
                int minWithinBordersX = Math.Min(px, w - px);
                int minWithinBordersY = Math.Min(py, h - py);

                //Create Square Outline at this location

                //Create the Half distances from this value
                int halfW = Math.Min(random.Next(5, 10), minWithinBordersX - 1);
                int halfH = Math.Min(random.Next(5, 10), minWithinBordersY - 1);

                int minX = px - halfW;
                int maxX = px + halfW;
                int minY = py - halfH;
                int maxY = py + halfH;


                for (int x = minX; x < maxX; x++)
                    for (int y = minY; y < maxY; y++)
                        if ((x == minX || x == maxX - 1) || (y == minY || y == maxY - 1))
                        {
                            map[x][y] = false;
                            count++;
                        }


            }

            return map;
        }

        #endregion
        #region Gen3
        //Gen 1-> 20%, each tile has a 20% chance of being a wall.
        static bool[][] Generator_3(bool[][] map)
        {

            //Step 1: Create Rooms
            //Rooms must be placed Adjacent to one Another
            //There sizes can vary
            //Their outer Ring is a (default) wall.


            return map;
        }



        void Create_Rooms()
        {
            //Create rooms untill x rooms are created

            int minRoomSize = 8; //8x8 room, to allow for item placement





        }



        #endregion


    }
}

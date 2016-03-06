using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RTS_Moba.Pathfind;
namespace RTS_Moba
{
    class Terrain
    {
        private int _width  = 0;
        private int _height = 0;

        bool[][] map;
        TextMesh[][] lbl_Map;

        //
        public static System.Random random;
        public Terrain(int width, int height)
        {
            //If no Seed is specified...
            if (random == null)
                random = new System.Random();

            Setup_Terrain(width, height);
            //Setup the Pathfinder with our World.
            Pathfinder.instance.Setup(map, this);
        }


        void Setup_Terrain(int width, int height)
        {

            //Tiles are Offset by 0.5 to correct for Rounding Issues.
            //Center is placed To the Right and Up of (x,y) coordinate so doing x(int), y(int) of Character Position = Instant Grid Cell.
            float offset = 0.5f;

            //Create the Bool Map for Pathfinding
            map = new bool[width][];

            if (Debugging.Debugger.isDebugMode)
                lbl_Map = new TextMesh[width][];

            for (int x = 0; x < width; x++)
            {
                map[x] = new bool[height];
                if (Debugging.Debugger.isDebugMode)
                    lbl_Map[x] = new TextMesh[height];
                for (int y = 0; y < height; y++)
                {
                    //Instantiate the Tile
                    GameObject go = ResourceManager.Get_Tile();
                    go = GameObject.Instantiate(go, new Vector3((float)x + offset, 0, (float)y + offset), Quaternion.identity) as GameObject;

                    if (Debugging.Debugger.isDebugMode)
                    {
                        //Instantiate the Debug Tile Info
                        //Note this is a unity 3d Text as all world Text shouldbe
                        GameObject debug_Temp = ResourceManager.Get_UI("Debug_HUD_JPDist");
                        debug_Temp = GameObject.Instantiate(debug_Temp, Vector3.zero, Quaternion.identity) as GameObject;
                        debug_Temp.transform.rotation = Quaternion.Euler(90, 0, 0);
                        //UIFollow setup
                        debug_Temp.transform.position = go.transform.position + new Vector3(0, 2, 0);

                        lbl_Map[x][y] = debug_Temp.GetComponent<TextMesh>();

                        go.SetActive(false);
                    }
                    //Load/Create Boolean Map
                    //20% Chance to be false
                    if (random.NextDouble() < 0.2)
                    {
                        map[x][y] = false;

                        //Instantiate Wall at location
                        GameObject wall = ResourceManager.Get_Wall();
                        wall = GameObject.Instantiate(wall, new Vector3((float)x + offset, 0, (float)y + offset), Quaternion.identity) as GameObject;

                        //Debug Feature
                        if (Debugging.Debugger.isDebugMode)
                            lbl_Map[x][y].color = Color.red;

                    }
                    else
                    {
                        map[x][y] = true;

                    }
                }
            }

        }


        //Once Pathfinder and The Terrain is setup, wall should be erected.














        public void Debug_SetText(int x, int y, string text)
        { Debug_SetText(x, y, text, Color.white); }

        public void Debug_SetText(int x, int y, string text, Color color)
        {
            lbl_Map[x][y].text = text;
            lbl_Map[x][y].color = color;
        }



    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace RTS_Moba.Pathfind
{
    /*
        This class functions as a Singleton Wrapper for any Pathfind algorithm I choose.
        Atm I'm leaning on JPS but in case I wish to swap it out later, the process should be simple.

    */
    class Pathfinder
    {
        #region Singleton Constructor
        //Singleton Constructor
        private static readonly Pathfinder _instance = new Pathfinder();
        private Pathfinder() { }
        public static Pathfinder instance
        {
            get
            {
                return _instance;
            }
        }
        #endregion

        private JPS_Plus pf_Searcher;

       // private FlowField flowField;
        Terrain terrain;
        //Pass a Map for the Pathfinder to Process
        public void Setup(bool[][] map, Terrain terrain)
        {
            pf_Searcher = new JPS_Plus(map);
            //flowField = new FlowField(map);

            //For Reference
            this.terrain = terrain;
        }


        public List<Vector3> Get_Path(Vector3 start, Vector3 end)
        {
            return pf_Searcher.Find_Path(start, end);
        }

        public bool isLocationWalkable(Vector3 location)
        {
            return isLocationWalkable((int)location.x, (int)location.z);
        }
        public bool isLocationWalkable(int x,int y)
        {

            return pf_Searcher.isWalkable(x, y);
        }

        public void Debug_Draw()
        {
           // pf_Searcher.Debug_Draw(terrain);

        }

        public int width {  get { return terrain._width; } }
        public int height { get { return terrain._height; } }

    }
}

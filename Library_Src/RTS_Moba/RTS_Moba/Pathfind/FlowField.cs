using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;

using System.Diagnostics;
namespace RTS_Moba.Pathfind
{
    //Singleton (One Pathfinder per map!)
    class FlowField
    {
        Hashtable lookUpMap;

        int width;
        int height;

        Node[][] nodeMap;
        public FlowField(bool[][] map)
        {

            //For each Point on the Map setup a Pathfind Grid then store it in the lookupMap
            width = map.Length;
            height = map[0].Length;

            //Setup a Path FOR EVERY Potential Point
            nodeMap = new Node[width][];

            //Setup NodeMap
            for (int x = 0; x < width; x++)
            {
                nodeMap[x] = new Node[height];
                for (int y = 0; y < height; y++)
                    nodeMap[x][y] = new Node(this,x,y); 
            }
            Stopwatch swatch = new Stopwatch();
            swatch.Start();
        //Process Boolean Map into NodeMap
        // for (int x = 0; x < width; x++)
        //{
          //  for (int y = 0; y < 2; y++)
           //  Setup_Flow(map, 0, y);

            /// }

            swatch.Stop();

            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = swatch.Elapsed;

            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);

            UnityEngine.Debug.LogError("Time Elapsed:" + elapsedTime);
        }

        void Setup_Flow(bool[][] map, int targetX, int targetY)
        {
            //Given this Boolean Map, we'll Need to create a flowfield for it.

            //Step one Create the WaveFront

            List<Node> openList = new List<Node>();
            List<Node> closedList = new List<Node>();

            Node currentNode = nodeMap[targetX][targetY];
            openList.Add(currentNode);
            currentNode.waveFrontNum = 0;

            int waveCounter = 0;

            //Populate Grid with Index

            int attemptCount =  width* height  +1;
            while (openList.Count > 0 && waveCounter < attemptCount)
            {
                currentNode = openList[0];
                openList.RemoveAt(0);
                closedList.Add(currentNode);

                //Get Orthogonal Neighbours
                int origX = currentNode.x;
                int origY = currentNode.y;

                int newX = origX;
                int newY = origY;

                // Up
                newY = origY + 1;
                subroutine_SetupWaveFrontNode(map, openList, closedList, newX, newY, waveCounter);
                // Down
                newY = origY - 1;
                subroutine_SetupWaveFrontNode(map, openList, closedList, newX, newY, waveCounter);
                // Left
                newX = origX + 1;
                subroutine_SetupWaveFrontNode(map, openList, closedList, newX, newY, waveCounter);
                // Right
                newX = origX - 1;
                subroutine_SetupWaveFrontNode(map, openList, closedList, newX, newY,waveCounter);

                waveCounter++;
            }
            //Debug.LogError("Attempt:" + waveCounter);


            nodeMap[targetX][targetY].SetupTarget(new Vector3(targetX, 0, targetY), Vector3.zero); //We are here no need to Move.
        }

        //Helper function for populating openList, and setting up correct wavefront numbers.
        void subroutine_SetupWaveFrontNode(bool[][] map, List<Node> openList, List<Node> closedList, int x, int y, int waveCounter)
        {
            //If its within boundaries and in neithor open/closed lists
            if (isValid(x, y) && !openList.Contains(nodeMap[x][y]) && !closedList.Contains(nodeMap[x][y]))
            {
                openList.Add(nodeMap[x][y]);
                //if the node is a WALL set its waveCounter to a higher value than normal (100 in this case, it's arbirtray, but its higher than possible neighbours)
                if(map[x][y])
                    nodeMap[x][y].waveFrontNum = waveCounter + 1;
                else
                    nodeMap[x][y].waveFrontNum = waveCounter + 101;
            }
        }




        bool isValid(Vector3 vector) { return isValid(vector.x, vector.z); }
        bool isValid(Vector2 vector) { return isValid(vector.x, vector.y);   }
        bool isValid(float x, float y){ return isValid((int)x, (int)y);}
        bool isValid(int x, int y)
        {
            return (x >= 0 && x < width && y >= 0 & y < height);
        }



        class Node
        {
            Hashtable targetMap;
            FlowField owner;

            //This value is ONLY used for Setting up the flowfields as it's simpler to recycle an int than to constantly allocate more memory
            public int waveFrontNum;

            int _x;
            int _y;

            public int x
            {
                get { return _x;}
            }
            public int y
            {
                get { return _y; }
            }

            public Node(FlowField owner, int x, int y)
            {
                this.owner = owner;
                targetMap = new Hashtable();
                this._x = x;
                this._y = y;
            }

            public void SetupTarget(Vector3 goal, Vector3 direction)
            {
                //Zero out Goal
                goal.x = (int)goal.x;
                goal.y = 0;
                goal.z = (int)goal.z;

                targetMap[goal] = direction;
            }

            public Vector3 GetDirection(Vector3 goal)
            {
                //Zero out Goal
                goal.x = (int)goal.x;
                goal.y = 0;
                goal.z = (int)goal.z;

                if (goal.x < 0 || goal.x > owner.width || goal.z < 0 || goal.z > owner.height)
                {
                    UnityEngine.Debug.LogError("Goal[" + goal + "] is out of bounds returning Zero");
                    return Vector3.zero;
                }
                if (targetMap[goal] == null)
                { 
                    UnityEngine.Debug.LogError("Goal[" + goal + "] is null returning Zero");
                    return Vector3.zero;
                }
                return (Vector3)(targetMap[goal]);

            }

        }




    }
}

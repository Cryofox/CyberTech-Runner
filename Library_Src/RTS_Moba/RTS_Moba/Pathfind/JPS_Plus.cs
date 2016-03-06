using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RTS_Moba.Debugging;
using RTS_Moba.UI;
namespace RTS_Moba.Pathfind
{
    class JPS_Plus
    {
        int mapFloorLevel = 1;
        int width;
        int height;
        JumpPointNode[][] map;
        public JPS_Plus(bool[][] map)
        {
            width = map.Length;
            height = map[0].Length;
            //Process Map
            ProcessMap(map);

            //Test Pathfind
            /*
            System.Random r = new System.Random();

            int x = r.Next(0, width);
            int y = r.Next(0, width);

            while (!map[x][y])
            {
                 x = r.Next(0, width);
                 y = r.Next(0, width);
            }
            Vector3 start = new Vector3(x, mapFloorLevel, y);

            x = r.Next(0, width);
            y = r.Next(0, width);

            while (!map[x][y])
            {
                x = r.Next(0, width);
                y = r.Next(0, width);
            }
            Vector3 goal = new Vector3(x, mapFloorLevel, y);

            List<Vector3> path = Find_Path(start, goal);
            Debug_Path(path,Color.green);
            Debug_Path( OptimizePath(path), Color.yellow);
            */
        }

        public void Debug_Path(List<Vector3> nodes,Color color)
        {
            if (nodes.Count == 0)
                return;
            Debugging.Debugger.DrawRectangle(new Rect(nodes[0].x , nodes[0].z , 0.1f, 0.1f), Color.cyan, 10);
            Debugging.Debugger.DrawRectangle(new Rect(nodes[nodes.Count - 1].x , nodes[nodes.Count - 1].z, 0.1f, 0.1f), Color.yellow, 10);
            for (int i = 0; i < nodes.Count; i++)
            {
                if (i + 1 < nodes.Count)
                {
                    Debugger.DrawLine(nodes[i], nodes[i + 1], color, 3);
                    //Debugger.DrawLine(nodes[i], nodes[i] + Vector3.Normalize(nodes[i + 1]-nodes[i]), Colors.GREEN, 10);

                }
            }

            //Debug.Log("Nodes inPath =" + nodes.Count);
        }


        public void Debug_Draw(Terrain terrain=null)
        {
            /*
            Rect rectangle;
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                {
                    
                    rectangle = new Rect(x+0.05f, y + 0.05f, 0.9f, 0.9f);
                    if (!map[x][y].isWall)
                    {
                        JumpPointNode jpn = map[x][y];

                        //Purple == Process_PrimaryJumpPoints Jump Node
                        if (jpn.isPrimaryJumpDown || jpn.isPrimaryJumpLeft || jpn.isPrimaryJumpRight || jpn.isPrimaryJumpUp)
                        {
                            Debugging.Debugger.DrawRectangle(rectangle, Colors.VIOLET);
                            //Draw Rectangles associated with Which PJP it is
                            Rect temp;
                            if (jpn.isPrimaryJumpDown)
                            {
                                temp = new Rect(x + 0.45f, y, 0.1f, 0.1f);
                                Debugging.Debugger.DrawRectangle(temp, Colors.RED);
                            }
                            if (jpn.isPrimaryJumpUp)
                            {
                                temp = new Rect(x + 0.45f, y + 0.9f, 0.1f, 0.1f);
                                Debugging.Debugger.DrawRectangle(temp, Colors.RED);
                            }
                            if (jpn.isPrimaryJumpLeft)
                            {
                                temp = new Rect(x, y + 0.45f, 0.1f, 0.1f);
                                Debugging.Debugger.DrawRectangle(temp, Colors.RED);
                            }
                            if (jpn.isPrimaryJumpRight)
                            {
                                temp = new Rect(x + 0.9f, y + 0.45f, 0.1f, 0.1f);
                                Debugging.Debugger.DrawRectangle(temp, Colors.RED);
                            }
                        }
                        else
                        {
                            Debugging.Debugger.DrawRectangle(rectangle, Colors.BLUE);
                        }
                    }
                    else
                    {
                        Debugging.Debugger.DrawRectangle(rectangle, Colors.RED);
                    }
                    

                    if (terrain != null && (Debugging.Debugger.isDebugMode))
                    {
                        string debugInfo="";
                        Color color = Color.red;
                        if (!map[x][y].isWall)
                        {
                            debugInfo = "" + map[x][y].jDistUpLeft + " " + map[x][y].jDistUp + " " + map[x][y].jDistUpRight;
                            debugInfo += "\n" + map[x][y].jDistLeft + " x " + map[x][y].jDistRight;
                            debugInfo += "\n" + map[x][y].jDistDownLeft  + " " + map[x][y].jDistDown + " " + map[x][y].jDistDownRight;
                            //  debugInfo = debugInfo.Replace("0", "  ");
                            color= Color.white;
                        }
                        terrain.Debug_SetText(x,y,debugInfo, color);
                    }

               }
               */
        }
        void ProcessMap(bool[][] bmap)
        {
            map = new JumpPointNode[width][];
            for (int x = 0; x < width; x++)
            {
                map[x] = new JumpPointNode[height];
                for (int y = 0; y < height; y++)
                {
                    map[x][y] = new JumpPointNode();
                    map[x][y].isWall = !bmap[x][y]; //boolean map contains Walkable Tiles therefore !Walkable = Wall.
                    map[x][y].x = x;
                    map[x][y].y = y;
                }
            }

            //Process Primary JumpPoints
            Process_PrimaryJumpPoints();
            Process_StraightJumpPoints(); //Depends on Primary JP
            Process_DiagonalJumpPoints(); //Depends on PJP and Straight JP
            Process_Directions(); //Now that all directions are setup, Link them to lookuptable.
        }
        #region ProcessNodes Subroutine
        void Process_PrimaryJumpPoints()
        {
            //A Jumppoint is primary if it has a Forced Neighbour. This occurs if a diagonal tile iswalled while it's corresponding Cardinal neighbours are 
            //free ex:
            /*
                120 1y0
                yx  2x0
                000 000

                2's are forced Angles going from y -> x
            */
            //Find ForcedNeighbours
            #region Get Primary Jump Points via Forced Neighbours
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                {
                    //Initialize Primary JumpPoints
                    map[x][y].isPrimaryJumpRight    = false;
                    map[x][y].isPrimaryJumpLeft     = false;
                    map[x][y].isPrimaryJumpRight    = false;
                    map[x][y].isPrimaryJumpUp       = false;
                    map[x][y].isPrimaryJumpDown     = false;

                    map[x][y].jDistDown = 0;
                    map[x][y].jDistUp = 0;
                    map[x][y].jDistLeft = 0;
                    map[x][y].jDistRight = 0;
                    map[x][y].jDistDownLeft = 0;
                    map[x][y].jDistDownRight = 0;
                    map[x][y].jDistUpLeft = 0;
                    map[x][y].jDistUpRight = 0;
                    //If this node is not a wall. Check if it's a primary node.
                    if (!map[x][y].isWall)
                    { 
                        //If the Left Node is not a wall (nor out of bounds) check if we're a RightJPoint
                        if ( isValid(x-1, y) && !map[x-1][y].isWall)
                        {
                            if ( 
                                (
                                //Check if this is a Right PJump Node Top Left is Wall Top is Not
                                    (isValid(x - 1, y + 1) && (map[x - 1][y + 1].isWall)) &&
                                    (isValid(x , y + 1) && (!map[x][y + 1].isWall))
                                )
                                ||
                                (
                                    //Check if this is a Right PJump Node Bot Left is Wall Bot is Not
                                    (isValid(x - 1, y - 1) && (map[x - 1][y - 1].isWall)) &&
                                    (isValid(x, y - 1) && (!map[x][y - 1].isWall))
                                )
                                )
                                    map[x][y].isPrimaryJumpRight = true;
                        }

                        //If the Right Node is not a wall (nor out of bounds) check if we're a LeftJPoint
                        if (isValid(x + 1, y) && !map[x + 1][y].isWall)
                        {
                            if (
                                (
                                    //Check if this is a Left PJump Node Top Right is Wall Top is Not
                                    (isValid(x + 1, y + 1) && (map[x + 1][y + 1].isWall)) &&
                                    (isValid(x, y + 1) && (!map[x][y + 1].isWall))
                                )
                                ||
                                (
                                    //Check if this is a Left PJump Node Bot Right is Wall Bot is Not
                                    (isValid(x + 1, y - 1) && (map[x + 1][y - 1].isWall)) &&
                                    (isValid(x, y - 1) && (!map[x][y - 1].isWall))
                                )
                                )
                                map[x][y].isPrimaryJumpLeft = true;
                        }

                        //If the Up Node is not a wall (nor out of bounds) check if we're a DownJPoint
                        if (isValid(x, y+1) && !map[x][y + 1].isWall)
                        {
                            if (
                                (
                                    //Check if TopRight is a Wall, and Right is Free
                                    (isValid(x + 1, y + 1) && (map[x + 1][y + 1].isWall)) &&
                                    (isValid(x+1, y) && (!map[x+1][y].isWall))
                                )
                                ||
                                (
                                    //Check if TopLeft is a Wall, and Left is Free
                                    (isValid(x - 1, y + 1) && (map[x - 1][y + 1].isWall)) &&
                                    (isValid(x-1, y) && (!map[x-1][y].isWall))
                                )
                                )
                                map[x][y].isPrimaryJumpDown = true;
                        }

                        //If the Down Node is not a wall (nor out of bounds) check if we're an UpJPoint
                        if (isValid(x, y - 1) && !map[x][y - 1].isWall)
                        {
                            if (
                                (
                                    //Check if BotRight is a Wall, and Right is Free
                                    (isValid(x + 1, y - 1) && (map[x + 1][y - 1].isWall)) &&
                                    (isValid(x + 1, y) && (!map[x + 1][y].isWall))
                                )
                                ||
                                (
                                    //Check if BotLeft is a Wall, and Left is Free
                                    (isValid(x - 1, y - 1) && (map[x - 1][y - 1].isWall)) &&
                                    (isValid(x - 1, y) && (!map[x - 1][y].isWall))
                                )
                                )
                                map[x][y].isPrimaryJumpUp = true;
                        }
                    }
                }
            #endregion
        }
        void Process_StraightJumpPoints()
        {
            //Process all Straight Jump Points
            SweepLeftSJP();
            SweepRightSJP();
            SweepDownSJP();
            SweepUpSJP();
        }
        void Process_DiagonalJumpPoints()
        {
            //Process all Diagonal Jump Points
            SweepDownLeftDJP();
            SweepDownRightDJP();
            SweepUpLeftDJP();
            SweepUpRightDJP();
        }
        #endregion
        #region Sweep Diagonal Jump Points
        void SweepDownLeftDJP()
        {
            //Sweep for Left Jump Nodes
            //Default Setup, Out of bounds = Wall therefore isNegative =-1;
            int count = 0;
            int isNegative = -1;
            //Increase Count using Y
            for (int x = 0; x < height; x++)
                for (int y = 0; y < height; y++)
                {
                    //If both x and y are past the first line stop processing.
                    if (x > 0 && y > 0)
                        continue;

                    count = 0;
                    isNegative = -1;
                    int iX=0;
                    int iY=0;
                    for (int i = 0; i < height; i++) //A Bit wasteful as it'll iterate outside the array but w.e
                    {
                        iX = x + i;
                        iY = y + i;
                        //If the iterated node is a wall
                        if (isValid(iX,iY))
                        {

                            if (map[iX][iY].isWall)
                            {
                                isNegative = -1;
                                count = -1;
                            }
                            else if
                               (isValid(iX - 1, iY) && map[iX - 1][iY].isWall ||
                                isValid(iX, iY - 1) && map[iX][iY - 1].isWall)
                            {
                                isNegative = -1;
                                count = 0;
                            }
                            //Not a wall, so we proceed
                            else
                            {

                                //Check if our Parent has valid paths
                                if (isValid(iX - 1, iY - 1) &&
                                    (
                                        map[iX - 1][iY - 1].jDistDown > 0 ||
                                        map[iX - 1][iY - 1].jDistLeft > 0
                                    ))

                                {
                                    isNegative = 1;
                                    count = 1;
                                }
                                map[iX][iY].jDistDownLeft = count * isNegative;


                            }

                        }
                        count++;
                    }
                    
                }
        }
        void SweepDownRightDJP()
        {
            //Sweep for Left Jump Nodes
            //Default Setup, Out of bounds = Wall therefore isNegative =-1;
            int count = 0;
            int isNegative = -1;
            //Increase Count using Y
            for (int x = width-1; x >-1; x--)
                for (int y = 0; y < height; y++)
                {
                    //If both Row is not Bot, and x is not the Rightmost
                    if (x < width - 1 && y > 0)
                        continue;

                    count = 0;
                    isNegative = -1;
                    int iX = 0;
                    int iY = 0;
                    for (int i = 0; i < height; i++) //A Bit wasteful as it'll iterate outside the array but w.e
                    {
                        iX = x - i;
                        iY = y + i;
                        //If the iterated node is a wall
                        if (isValid(iX, iY))
                        {

                            if (map[iX][iY].isWall)
                            {
                                isNegative = -1;
                                count = -1;
                            }
                            else if
                               (isValid(iX + 1, iY) && map[iX + 1][iY].isWall ||
                                isValid(iX, iY - 1) && map[iX][iY - 1].isWall)
                            {
                                isNegative = -1;
                                count = 0;
                            }
                            //Not a wall, so we proceed
                            else
                            {

                                //Check if our Parent has valid paths
                                if (isValid(iX + 1, iY - 1) &&
                                    (
                                        map[iX + 1][iY - 1].jDistDown > 0 ||
                                        map[iX + 1][iY - 1].jDistRight > 0
                                    ))

                                {
                                    isNegative = 1;
                                    count = 1;
                                }
                                map[iX][iY].jDistDownRight = count * isNegative;


                            }

                        }
                        count++;
                    }

                }
        }

        void SweepUpLeftDJP()
        {
            //Sweep for Left Jump Nodes
            //Default Setup, Out of bounds = Wall therefore isNegative =-1;
            int count = 0;
            int isNegative = -1;
            //Increase Count using Y
            for (int x = 0; x<width;x++)
                for (int y = height-1; y >-1; y--)

                {
                    //If both x and y are past the first line stop processing.
                    if (x >0 && y < height-1)
                        continue;

                    count = 0;
                    isNegative = -1;
                    int iX = 0;
                    int iY = 0;
                    for (int i = 0; i < height; i++) //A Bit wasteful as it'll iterate outside the array but w.e
                    {
                        iX = x + i;
                        iY = y - i;
                        //If the iterated node is a wall
                        if (isValid(iX, iY))
                        {

                            if (map[iX][iY].isWall)
                            {
                                isNegative = -1;
                                count = -1;
                            }
                            else if
                               (isValid(iX - 1, iY) && map[iX - 1][iY].isWall ||
                                isValid(iX, iY + 1) && map[iX][iY + 1].isWall)
                            {
                                isNegative = -1;
                                count = 0;
                            }
                            //Not a wall, so we proceed
                            else
                            {

                                //Check if our Parent has valid paths
                                if (isValid(iX - 1, iY + 1) &&
                                    (
                                        map[iX - 1][iY + 1].jDistUp > 0 ||
                                        map[iX - 1][iY + 1].jDistLeft > 0
                                    ))

                                {
                                    isNegative = 1;
                                    count = 1;
                                }
                                map[iX][iY].jDistUpLeft = count * isNegative;


                            }

                        }
                        count++;
                    }

                }
        }
        void SweepUpRightDJP()
        {
            //Sweep for Left Jump Nodes
            //Default Setup, Out of bounds = Wall therefore isNegative =-1;
            int count = 0;
            int isNegative = -1;
            //Increase Count using Y
            for (int x = width - 1; x >-1; x--)
                for (int y = height - 1; y > -1; y--)

                {
                    //If both x and y are past the first line stop processing.
                    if (x < width - 1 && y < height - 1)
                        continue;

                    count = 0;
                    isNegative = -1;
                    int iX = 0;
                    int iY = 0;
                    for (int i = 0; i < height; i++) //A Bit wasteful as it'll iterate outside the array but w.e
                    {
                        iX = x - i;
                        iY = y - i;
                        //If the iterated node is a wall
                        if (isValid(iX, iY))
                        {

                            if (map[iX][iY].isWall)
                            {
                                isNegative = -1;
                                count = -1;
                            }
                            else if
                               (isValid(iX + 1, iY) && map[iX + 1][iY].isWall ||
                                isValid(iX, iY + 1) && map[iX][iY + 1].isWall)
                            {
                                isNegative = -1;
                                count = 0;
                            }
                            //Not a wall, so we proceed
                            else
                            {

                                //Check if our Parent has valid paths
                                if (isValid(iX + 1, iY + 1) &&
                                    (
                                        map[iX + 1][iY + 1].jDistUp > 0 ||
                                        map[iX + 1][iY + 1].jDistRight > 0
                                    ))
                                {
                                    isNegative = 1;
                                    count = 1;
                                }
                                map[iX][iY].jDistUpRight = count * isNegative;


                            }

                        }
                        count++;
                    }

                }
        }
        #endregion
        #region Sweep Straight Jump Points
        void SweepLeftSJP()
        {
            //Sweep for Left Jump Nodes
            //Default Setup, Out of bounds = Wall therefore isNegative =-1;
            int count = 0;
            int isNegative = -1;
            for (int y = 0; y < height; y++)
            {
                count = 0;
                isNegative = -1;
                for (int x = 0; x < width; x++)
                {
                    //jDistLeft is equal to the Count + whether the last node was PJPoint or Wall.
                    //Negative is used to Denote Distance to Dead End
                    if (!map[x][y].isWall)
                        map[x][y].jDistLeft = count * isNegative;
                    //Sweep for Right Nodes. Going Left to Right
                    if (map[x][y].isPrimaryJumpLeft)
                    {
                        isNegative = 1;
                        count = 0;
                    }
                    if (map[x][y].isWall)
                    {
                        isNegative = -1;
                        count = -1; //Start with Negative 1 due to a Wall being unable to get to. Therefore -1 ++ = 0.
                    }

                    count++;
                }
            }
        }
        void SweepRightSJP()
        {
            //Sweep for Left Jump Nodes
            //Default Setup, Out of bounds = Wall therefore isNegative =-1;
            int count = 0;
            int isNegative = -1;
            for (int y = 0; y < height; y++)
            {
                count = 0;
                isNegative = -1;
                for (int x = width - 1; x > -1; x--)
                {
                    //jDistLeft is equal to the Count + whether the last node was PJPoint or Wall.
                    //Negative is used to Denote Distance to Dead End
                    if (!map[x][y].isWall)
                        map[x][y].jDistRight = count * isNegative;

                    //Reset if our point is PJP or Wall
                    if (map[x][y].isPrimaryJumpRight)
                    {
                        isNegative = 1;
                        count = 0;
                    }
                    if (map[x][y].isWall)
                    {
                        isNegative = -1;
                        count = -1; //Start with Negative 1 due to a Wall being unable to get to. Therefore -1 ++ = 0.
                    }

                    count++;
                }
            }
        }
        void SweepDownSJP()
        {
            //Sweep for Down Jump Nodes
            //Default Setup, Out of bounds = Wall therefore isNegative =-1;
            int count = 0;
            int isNegative = -1;
            for (int x = 0; x < width; x++)
            {
                count = 0;
                isNegative = -1;
                for (int y = 0; y < height; y++)
                {
                    //jDistLeft is equal to the Count + whether the last node was PJPoint or Wall.
                    //Negative is used to Denote Distance to Dead End
                    if (!map[x][y].isWall)
                        map[x][y].jDistDown = count * isNegative;

                    //Reset if our point is PJP or Wall
                    if (map[x][y].isPrimaryJumpDown)
                    {
                        isNegative = 1;
                        count = 0;
                    }
                    if (map[x][y].isWall)
                    {
                        isNegative = -1;
                        count = -1; //Start with Negative 1 due to a Wall being unable to get to. Therefore -1 ++ = 0.
                    }

                    count++;
                }
            }
        }
        void SweepUpSJP()
        {
            //Sweep for Down Jump Nodes
            //Default Setup, Out of bounds = Wall therefore isNegative =-1;
            int count = 0;
            int isNegative = -1;

            for (int x = 0; x < width; x++)
            {
                count = 0;
                isNegative = -1;
                for (int y = height - 1; y > -1; y--)
                {
                    //jDistLeft is equal to the Count + whether the last node was PJPoint or Wall.
                    //Negative is used to Denote Distance to Dead End
                    if (!map[x][y].isWall)
                        map[x][y].jDistUp = count * isNegative;

                    //Reset if our point is PJP or Wall
                    if (map[x][y].isPrimaryJumpUp)
                    {
                        isNegative = 1;
                        count = 0;
                    }
                    if (map[x][y].isWall)
                    {
                        isNegative = -1;
                        count = -1; //Start with Negative 1 due to a Wall being unable to get to. Therefore -1 ++ = 0.
                    }

                    count++;
                }
            }
        }
        #endregion
        void Process_Directions()
        {
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                {
                    List<NodeDirection> directions = new List<NodeDirection>();

                    directions.Add(new NodeDirection(CompassDirections.North, map[x][y].jDistUp) );
                    directions.Add(new NodeDirection(CompassDirections.South, map[x][y].jDistDown));
                    directions.Add(new NodeDirection(CompassDirections.East, map[x][y].jDistRight));
                    directions.Add(new NodeDirection(CompassDirections.West, map[x][y].jDistLeft));

                    directions.Add(new NodeDirection(CompassDirections.NorthEast, map[x][y].jDistUpRight) );
                    directions.Add(new NodeDirection(CompassDirections.NorthWest, map[x][y].jDistUpLeft));
                    directions.Add(new NodeDirection(CompassDirections.SouthEast, map[x][y].jDistDownRight));
                    directions.Add(new NodeDirection(CompassDirections.SouthWest, map[x][y].jDistDownLeft));

                    map[x][y].jpsDirections = directions;
                }
        }
        bool isValid(int x, int y)
        {
            if (x < 0 || x >= width || y  < 0 || y >= height)
                return false;

            return true;
        }

        
        class JumpPointNode
        {
            public int x;
            public int y;
            //Jump Point Direction Relative Distances
            public int jDistLeft;
            public int jDistRight;
            public int jDistDown;
            public int jDistUp;

            public int jDistDownLeft;
            public int jDistUpLeft;
            public int jDistDownRight;
            public int jDistUpRight;

            public bool isWall;

            //Primary Jump Points can exist in 4 Cardinal Directions

                //The Direction is the Direction a Jump point can go in.
                //IE isPrimaryJumpLEft is a PrimaryJumppoint if going in the LEFT Direction from x->y where Y is this node.
            public bool isPrimaryJumpLeft;
            public bool isPrimaryJumpRight;
            public bool isPrimaryJumpUp;
            public bool isPrimaryJumpDown;

            public List<NodeDirection> jpsDirections;
        }

        struct NodeDirection
        {
            public CompassDirections direction;
            public int distance;
            public bool isCardinal;
            public NodeDirection(CompassDirections direction, int distance)
            {
                this.distance = distance;
                this.direction = direction;

                switch (this.direction)
                {
                    case CompassDirections.North:
                    case CompassDirections.South:
                    case CompassDirections.East:
                    case CompassDirections.West:
                        isCardinal = true;
                        break;
                    default:
                        isCardinal = false;
                        break;
                }
            }
        }

        enum CompassDirections
        {
            North=10,
            South=20,
            East=1,
            West=2,

            NorthEast=11,
            NorthWest=12,
            SouthEast=21,
            SouthWest=22,

            None=0
        }

        #region Berherms LineDraw (Line of Sight Optimization)
        List<Vector3> OptimizePath(List<Vector3> path)
        {
            List<Vector3> optimizedPath = new List<Vector3>();

            if(path.Count>0)
            { 
                //Add the First node
                optimizedPath.Insert(0, path[path.Count-1]);
                //Start at the End
                for (int i = path.Count-1; i > -1;)
                {
                    int newIndex = i;
                    for (int j = i - 2; j > -1; j--)
                    {
                        //Perform Line of Sight
                        if (hasLineofSight(path[j], path[i]))
                        {
                            //Line of sight exists set newIndex to J
                            newIndex = j;
                        }
                        else
                            break;
                    }
                    if (i == newIndex)
                        i--;
                    else
                    { 
                        i = newIndex;
                    }
                    if(i>-1)
                        optimizedPath.Insert(0, path[i]);
                }
            }

            return optimizedPath;
        }
        bool hasLineofSight(Vector3 p1, Vector3 p2)
        {
            int x1 = (int)p1.x;
            int x2 = (int)p2.x;
            int y1 = (int)p1.z;
            int y2 = (int)p2.z;

            int deltaX = Math.Abs(x2 - x1);
            int deltaY = Math.Abs(y2 - y1);

            int signX =1;
            int signY =1;

            if (x2 < x1)
                signX = -1;
            if (y2 < y1)
                signY = -1;

            int px = x1;
            int py = y1;
            bool hitsWall = false;
            for (int x = 0, y = 0; x < deltaX || y < deltaY;)
            {
                if ((1+2*x)*deltaY == (1+2* y)*deltaX)
                {
                    // next step is diagonal
                    px += signX;
                    py += signY;
                    x++;
                    y++;
                }
                else if ((1 + 2 * x) * deltaY < (1 + 2 * y) * deltaX)
                {
                    // next step is horizontal
                    px += signX;
                    x++;
                }
                else
                {
                    // next step is vertical
                    py += signY;
                    y++;
                }

                if (map[px][py].isWall)
                    hitsWall = true;
            }

            return !hitsWall;
        }
        #endregion
        #region A* Pathfinding
        class PathFindNode
        {
            public float heuristic;
            public float givenCost;
            public JumpPointNode current;
            public PathFindNode parent;
        }

        public bool isWalkable(int x, int y)
        {
            return (isValid(x, y) && !map[x][y].isWall);
        }


        public List<Vector3> Find_Path(Vector3 start, Vector3 goal)
        {
            List<Vector3> pathToGoal = new List<Vector3>();

            List<PathFindNode> open_List = new List<PathFindNode>();
            List<PathFindNode> closed_List = new List<PathFindNode>();

            int startX = (int)start.x;
            int startY = (int)start.z;

            int goalX = (int)goal.x;
            int goalY = (int)goal.z;

            //Return empty list if the path is not Valid
            //Not valid implises outside of border
            if (!isValid(startX, startY) || !isValid(goalX, goalY))
                return pathToGoal;

            //If Start == End then just return a list of one...
            if (startX == goalX && startY == goalY)
            {
                pathToGoal.Add(new Vector3(goalX, mapFloorLevel, goalY));
                return pathToGoal;
            }


            //If our start location is a wall...return null
            if (map[startX][startY].isWall)
            {
                return pathToGoal;
            }

            //Now we modify the GoalXY if it's a wall
            if (map[goalX][goalY].isWall)
            {
                int newX = 0;
                int newY = 0;
                bool solutionFound = false;
                for (int x = 0; x < width && solutionFound == false; x++)
                    for (int y = 0; y < height && solutionFound == false; y++)
                    {
                        newX = goalX + x;
                        newY = goalY + y;

                        //Right Up
                        if (isValid(newX, newY) && !map[newX][newY].isWall)
                        {
                            goalX = newX;
                            goalY = newY;
                            solutionFound = true;
                            break;
                        }

                        newX = goalX - x;
                        newY = goalY + y;

                        //Left up
                        if (isValid(newX, newY) && !map[newX][newY].isWall)
                        {
                            goalX = newX;
                            goalY = newY;
                            solutionFound = true;
                            break;
                        }

                        newX = goalX + x;
                        newY = goalY - y;

                        //Right Down
                        if (isValid(newX, newY) && !map[newX][newY].isWall)
                        {
                            goalX = newX;
                            goalY = newY;
                            solutionFound = true;
                            break;
                        }

                        newX = goalX - x;
                        newY = goalY - y;

                        //Left Down
                        if (isValid(newX, newY) && !map[newX][newY].isWall)
                        {
                            goalX = newX;
                            goalY = newY;
                            solutionFound = true;
                            break;
                        }
                    }
            }

            PathFindNode startNode = new PathFindNode();
            PathFindNode goalNode = new PathFindNode();


            //Setup Initial PathFindNode
            startNode.current = map[startX][startY];
            startNode.givenCost = 0;
            startNode.parent = null;

            //Setup GoalNode
            goalNode.current = map[goalX][goalY];
            goalNode.givenCost = 0;
            goalNode.parent = null;



            open_List.Add(startNode);
            PathFindNode curNode = startNode;

            int index = 0;
            CompassDirections goalDirection;

            int distanceGoalX = 0;
            int distanceGoalY = 0;
            int totalDistance = 0;


            while (open_List.Count > 0)
            {
                index = Get_BestIndex(open_List, goalX, goalY);
                //Pop Node at Index
                curNode = open_List[index];

                open_List.RemoveAt(index);
                closed_List.Add(curNode);

                //Calculate the Distance between Current & Goal
                distanceGoalX = Math.Abs(curNode.current.x - goalNode.current.x);
                distanceGoalY = Math.Abs(curNode.current.y - goalNode.current.y);
                totalDistance = distanceGoalX + distanceGoalY;


                if (totalDistance == 0)
                {
                    goalNode = curNode;
                    //Debug.Log("FOUND PATH");
                    break;
                }

                //For each Direction of this Node apply below Logic
                foreach (NodeDirection nDirection in curNode.current.jpsDirections)
                {
                    //Cardinal vs Diagonal
                    int minDistance = Math.Min(totalDistance, Math.Abs(nDirection.distance));
                    int maxDistance = Math.Abs(nDirection.distance);


                    //Add both the minimum and Maximum Distance if they are Different
                    JumpPointNode jpn;

                    
                    jpn = Get_Node(curNode, nDirection.direction, minDistance);
                    PathFindNode pfn = new PathFindNode();
                    pfn.current = jpn;
                    pfn.parent = curNode;
                    pfn.givenCost += minDistance;

                    if (!isInList(open_List, pfn) && !isInList(closed_List, pfn))
                        open_List.Add(pfn);


                    //Add Max Distance in the event minDistance!=max
                    if (minDistance != maxDistance)
                    {
                        jpn = Get_Node(curNode, nDirection.direction, maxDistance);
                        pfn = new PathFindNode();
                        pfn.current = jpn;
                        pfn.parent = curNode;
                        pfn.givenCost += maxDistance;

                        if (!isInList(open_List, pfn) && !isInList(closed_List, pfn))
                            open_List.Add(pfn);
                    }

                }
            }


            if (goalNode.parent == null)
            {
                Debug.LogError("PATH NOT FOUND [" + start + "]->[" + goal + "]");
                Debug.LogError("Start ["+startX+","+startY+"]");
                Debug.LogError("End [" + goalX + "," + goalY + "]");
            }
            else
            {
                //Insert Nodes into List
                PathFindNode pfn = goalNode;

                while (pfn.parent != null)
                {
                    pathToGoal.Insert(0, new Vector3(pfn.current.x + 0.5f, mapFloorLevel, pfn.current.y + 0.5f));
                    pfn = pfn.parent;
                }
            }
          //  Debug.Log("Open  Count=" + open_List.Count);
          //  Debug.Log("Closed Count =" + closed_List.Count);
           // Debug.Log("Total Node Count=" + (open_List.Count + closed_List.Count));
           // Debug.Log("P2G Count=" + pathToGoal.Count);

            //Optimize the Path now
            pathToGoal = OptimizePath(pathToGoal);

            return pathToGoal;
        }




            //This Algorithm is Copied almost 1v1 from the textbooks pseudocode and the results are not what they should be imo.
            /*
            List<Vector3> Find_Path_TextBook(Vector3 start, Vector3 goal)
        {
            List<Vector3> pathToGoal = new List<Vector3>();

            List<PathFindNode> open_List = new List<PathFindNode>();
            List<PathFindNode> closed_List = new List<PathFindNode>();

            int startX = (int)start.x;
            int startY = (int)start.z;

            int goalX  = (int)goal.x;
            int goalY  = (int)goal.z;

            //Return empty list if the path is not Valid
            if(!isValid(startX,startY) || !isValid(goalX,goalY))
                return pathToGoal;


            PathFindNode startNode = new PathFindNode();
            PathFindNode goalNode = new PathFindNode();


            //Setup Initial PathFindNode
            startNode.current = map[startX][startY];
            //          node.current.x = startX;
            //          node.current.y = startY;
            startNode.givenCost = 0;
            startNode.parent = null;

            //Setup GoalNode
            goalNode.current = map[goalX][goalY];

            goalNode.parent = null;
            goalNode.givenCost = 0;


            open_List.Add(startNode);
            PathFindNode node = startNode;

            int index = 0;
            int diffX = 0;
            int diffY = 0;
            CompassDirections goalDirection;


            while (open_List.Count > 0)
            {
                //Grab the node
                node = open_List[index];
                //Pop the item out of the list and into the Closed list
                open_List.RemoveAt(index);
                closed_List.Add(node);


                //Calculate GoalDirection
                goalDirection = CompassDirections.None;

                //Setup CompassDirection from Node -> Goal + CardinalDifferences
                diffX = goalX - node.current.x;
                diffY = goalY - node.current.y;

                //Set East or West 
                if (diffX > 0)
                    goalDirection = CompassDirections.East;
                else if (diffX < 0)
                    goalDirection = CompassDirections.West;

                //North
                if (diffY > 0)
                    goalDirection += (int)CompassDirections.North;

                //South
                else if (diffY < 0)
                    goalDirection += (int)CompassDirections.South;

                diffX = Math.Abs(diffX);
                diffY = Math.Abs(diffY);


                //Break Loop if currentNode = GoalNode
                if (node.current.x == goalX && node.current.y == goalY)
                {
                    Debug.LogError("PATH FOUND!"); 
                    break;
                }
                //This should not occur unless node = Goal and above line doesn't break
                if (goalDirection == CompassDirections.None)
                    Debug.LogError("Cardinal Direction= NONE!");

                //For Each direction 
                for(int i=0;i< node.current.jpsDirections.Count;i++)
                {
                    PathFindNode newSuccessor = null;

                    NodeDirection nDirection = node.current.jpsDirections[i];
                    float givenCost = 0;
                    //If the direction is Cardinal AND in the exact direction of the Goal
                    //AND Goal is closer than closest JumpPoint/Wall
                    if (
                         nDirection.isCardinal &&
                         nDirection.direction == goalDirection &&
                         (diffX + diffY)<= Math.Abs(nDirection.distance)
                    )
                    {
                        Debug.Log("Last Cardinal Node:");
                        Debug.Log("x:"+diffX +" y:"+diffY);
                        Debug.Log("gDirection:" + goalDirection);
                        Debug.Log("nDistance :" + Math.Abs(nDirection.distance));
                        Debug.Log("nDirection:"+nDirection.direction);

                        //Set the node to point to this new Node
                        newSuccessor = goalNode;
                        givenCost = node.givenCost + (diffX + diffY); //Due to the direction being cardinal eithor X or Y will be 0. saves an IF doing an addition
                    }
                    //If the direction is a diagonal AND in the general direction of the goal
                    else if (
                        !nDirection.isCardinal &&
                        isGeneralDirection(nDirection.direction, goalDirection) &&
                        (
                            (diffX <= Math.Abs(nDirection.distance)) ||
                            (diffY <= Math.Abs(nDirection.distance))
                        )

                        )
                    {
                        //Create a Target Node (the hidden 4th type)

                        //Goal is closer eithor Horiz or Vertical than Wall/Jump Point
                        //Grab mindifference
                        int minDiff = Math.Min(diffX, diffY);
                        //Get the Node at that difference in relation to the direction
                        JumpPointNode jpn = Get_Node(node, nDirection.direction, nDirection.distance);
                        PathFindNode pfn = new PathFindNode();
                        pfn.current = jpn;
                        givenCost = node.givenCost + (float)Math.Sqrt(minDiff);
                        newSuccessor = pfn;
                    }
                    //Finally if all else fails, check if this points to a Jump point as opposed to a wall
                    else if (nDirection.distance > 0)
                    {
                        //Get the Node at that difference in relation to the direction
                        JumpPointNode jpn = Get_Node(node, nDirection.direction, nDirection.distance);
                        PathFindNode pfn = new PathFindNode();
                        pfn.current = jpn;
                        givenCost = Math.Abs(jpn.x - node.current.x) + Math.Abs(jpn.y - node.current.y);
                        if (!nDirection.isCardinal)
                            givenCost = (float)Math.Sqrt(givenCost);
                        givenCost += node.givenCost;
                        newSuccessor = pfn;
                    }

                    //A* Portion of Algorithm (Notice how this step occurs INSIDE the Direction Check.
                    if (newSuccessor != null)
                    {
                        if (!isInList(open_List, newSuccessor) && !isInList(closed_List, newSuccessor))
                        {
                            newSuccessor.parent = node;
                            newSuccessor.givenCost = givenCost;
                            open_List.Add(newSuccessor);
                        }
                        //IF this parent is better than the original parent. Update
                        else if (givenCost < newSuccessor.givenCost)
                        {
                            newSuccessor.parent = node;
                            newSuccessor.givenCost = givenCost;
                            UpdateNodeInList(open_List, newSuccessor);
                        }
                    }
                    


                }

            }
            if (goalNode.parent == null)
            {
                Debug.LogError("Start Node =" + startX + "," + startY);
                Debug.LogError("Goal Node =" + goalX + "," + goalY);

                Debug.LogError("!!PATH NOT FOUND!");
                PathFindNode pfn = goalNode;
                pathToGoal.Insert(0, new Vector3(pfn.current.x + 0.5f, 1, pfn.current.y + 0.5f));
                pfn = startNode;
                pathToGoal.Insert(0, new Vector3(pfn.current.x + 0.5f, 1, pfn.current.y + 0.5f));

            }
            else
            {
                //Insert Nodes into List
                PathFindNode pfn = goalNode;

                while (pfn.parent != null)
                {
                    pathToGoal.Insert(0,new Vector3(pfn.current.x+0.5f, 1, pfn.current.y+0.5f));
                    pfn = pfn.parent;
                }


            }
            Debug.Log("Total Node Count=" + (open_List.Count + closed_List.Count));
            Debug.Log("P2G Count=" + pathToGoal.Count);
            return pathToGoal;
        }

            */
        //Helper Routines for pathfinding
        void UpdateNodeInList(List<PathFindNode> list, PathFindNode node)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].current == node.current)
                {
                    list[i].givenCost = node.givenCost;
                    list[i].parent = node.parent;
                }
            }
        }

            //Check if Pathfind Node is in List
        bool isInList(List<PathFindNode> list, PathFindNode node)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].current == node.current)
                    return true;
            }
            return false;
        }



        //SubRoutine:Get Node in Direction based on offset
        JumpPointNode Get_Node(PathFindNode currentNode, CompassDirections direction, int offset)
        {
            offset = Math.Abs(offset);
            int x = currentNode.current.x;
            int y = currentNode.current.y;
            JumpPointNode jpn;

            //    Debug.Log("EROR: DIR["+direction+"]OFFSET[" + offset + "] DOES NOT EXIST FROM ->" + currentNode.current.x + "," + currentNode.current.y + "!");

            switch (direction)
            {
                case CompassDirections.North:
                    jpn= map[x][y + offset];break;

                case CompassDirections.NorthEast:
                    jpn = map[x + offset][y+offset]; break;

                case CompassDirections.NorthWest:
                    jpn = map[x - offset][y+offset]; break;

                case CompassDirections.South:
                    jpn = map[x][y-offset]; break;

                case CompassDirections.SouthEast:
                    jpn = map[x + offset][y-offset];break;

                case CompassDirections.SouthWest:
                    jpn = map[x - offset][y-offset]; break;

                case CompassDirections.East:
                    jpn = map[x + offset][y]; break;

                case CompassDirections.West:
                    jpn = map[x - offset][y]; break;

                default:
                    jpn = null; break;
            }
           
            return jpn;
        }


        //General Direction Routine
        bool isGeneralDirection(CompassDirections currentDirection, CompassDirections goalDirection)
        {
            List<CompassDirections> generalDirection = new List<CompassDirections>();
            switch (goalDirection)
            {
                case CompassDirections.North:
                    generalDirection.Add(CompassDirections.North);
                    generalDirection.Add(CompassDirections.NorthEast);
                    generalDirection.Add(CompassDirections.NorthWest);
                    generalDirection.Add(CompassDirections.East);
                    generalDirection.Add(CompassDirections.West);
                    break;
                case CompassDirections.NorthEast:
                    generalDirection.Add(CompassDirections.North);
                    generalDirection.Add(CompassDirections.NorthEast);
                    generalDirection.Add(CompassDirections.East);
                    break;
                case CompassDirections.NorthWest:
                    generalDirection.Add(CompassDirections.North);
                    generalDirection.Add(CompassDirections.NorthWest);
                    generalDirection.Add(CompassDirections.West);
                    break;

                case CompassDirections.South:
                    generalDirection.Add(CompassDirections.South);
                    generalDirection.Add(CompassDirections.SouthEast);
                    generalDirection.Add(CompassDirections.SouthWest);
                    generalDirection.Add(CompassDirections.East);
                    generalDirection.Add(CompassDirections.West);
                    break;

                case CompassDirections.SouthEast:
                    generalDirection.Add(CompassDirections.South);
                    generalDirection.Add(CompassDirections.SouthEast);
                    generalDirection.Add(CompassDirections.East);
                    break;
                case CompassDirections.SouthWest:
                    generalDirection.Add(CompassDirections.South);
                    generalDirection.Add(CompassDirections.SouthWest);
                    generalDirection.Add(CompassDirections.West);
                    break;
                case CompassDirections.East:
                    generalDirection.Add(CompassDirections.South);
                    generalDirection.Add(CompassDirections.SouthEast);
                    generalDirection.Add(CompassDirections.East);
                    generalDirection.Add(CompassDirections.NorthEast);
                    break;
                case CompassDirections.West:
                    generalDirection.Add(CompassDirections.South);
                    generalDirection.Add(CompassDirections.SouthWest);
                    generalDirection.Add(CompassDirections.West);
                    generalDirection.Add(CompassDirections.NorthWest);
                    break;
            }
            for (int i = 0; i < generalDirection.Count; i++)
                if (generalDirection[i] == currentDirection)
                    return true;

            return false;
        }


        int Get_BestIndex(List<PathFindNode> nodeList, int goalX, int goalY)
        {
            int index = -1;
            float smallestDistance = 0;
            float temp;
            for (int i = 0; i < nodeList.Count; i++)
            {
                temp = nodeList[i].givenCost + CalculateDistance(nodeList[i].current.x, nodeList[i].current.y, goalX, goalY);
                if (index == -1 || temp < smallestDistance)
                {
                    smallestDistance = temp;
                    index = i;
                }
            }
            return index;
        }


        int CalculateDistance(int x1,int y1, int x2, int y2)
        {
            return Math.Abs(x2 - x1) + Math.Abs(y2 - y1);
        }



        #endregion

    }
}

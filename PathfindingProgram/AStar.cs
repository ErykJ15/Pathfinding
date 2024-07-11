using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PathfindingProgram
{
    public class AStar
    {
        public Vector2 mBotPosition;  //this is the current location of the bot

        public Texture2D mTexture; //variable for the texture of the bot
        public static bool mPathCalculated; //stores if the path has already been calculated
        private Vector2 mPlayerPosition; //store the position of the bot
        private int mGridsize;
        public static List<Vector2> mFullPath; //here is store all the nodes of the path calculated
        private static bool mWorking = false; //if working is true the create path function is working
        private List<Vector2> mRoute; //mroute manage the position of the bot in the path calculated 
        IDictionary<string, Node> mPathTraking; //all visited nodes are stored in path tracking, and from this dictionary the best calculated path is recreated

        public AStar(int pX, int pY)
        {
            //here the variables are initialized as the instance of the class is created
            mBotPosition = new Vector2(pX, pY);
            mRoute = new List<Vector2>();
            mFullPath = new List<Vector2>();
            mPathTraking = new Dictionary<string, Node>();
        }

        Vector2 mNextPosition;  //in this variable it is kept track of what should be the next position of the bot following the calculated path
        public void Update(Map pMap, Player pPlayer)
        {
            mGridsize = pMap.mGridSize;
            if (!mWorking)
            {
                if (mPlayerPosition != pPlayer.mGridPosition) //generate path
                {
                    mPlayerPosition = pPlayer.mGridPosition;
                    CreatePath(pMap, pPlayer);
                }
                if (mRoute.Count - 1 >= 0) //else move the bot following the path calculated
                {
                    mNextPosition = mRoute[mRoute.Count - 1];
                    mRoute.RemoveAt(mRoute.Count - 1);
                }
                mBotPosition = mNextPosition;
            }
        }


        public void CreatePath(Map pMap, Player pPlayer)//build the path with a star
        {
            mRoute = new List<Vector2>();  //initialize path as a new list
            mWorking = true; //set working to true
            List<Node> nodes = new List<Node>();  //initialize nodes as a new list
            mPathTraking = new Dictionary<string, Node>();  //initialize path tracking as a new dictionary

            //here we initialize the first node that is the starting one
            Node sNode = new Node();
            sNode.mParentIndex = -1; //parent is -1 as this is the starting one so it has no parent
            sNode.mGCost = 0; //the distance is of 0 cause it has no parent
            sNode.mGridPosition = mBotPosition; //set the grid position to the bot position
            sNode.mIndex = GetGridPosition(sNode.mGridPosition); //calculate the index
            sNode.mHCost = calculateHCost(sNode.mGridPosition, pPlayer.mGridPosition); //calculate the h cost
            sNode.mFCost = sNode.mGCost + sNode.mHCost; //calculate the f cost (the sum of f cost(the distance between this node and the previous one) and h cost (the distance between this node and the target))

            nodes.Add(sNode);
            //and we add it to our dictionary
            mPathTraking.Add(sNode.mIndex.ToString(), sNode);

            Node currentNode = sNode; //currentNode is the starting one before the while

            while (true)//the loop have to loop until the path reach the target position
            {
                if (mPlayerPosition != pPlayer.mGridPosition)  //this is used to manage error (like the bot is already in the target pos)
                {
                    return;
                }
                List<Node> nodesNear = getNodesNear(currentNode, pMap);
                // here we calculate the 8 near nodes of the current node and of each of these we calculate the f cost
                // and add all of them to the dictionary
                for (int i = 0; i < nodesNear.Count; i++)
                {
                    Node newNode;
                    newNode = new Node();
                    newNode.mParentIndex = currentNode.mIndex;
                    newNode.mGCost = currentNode.mGCost + nodesNear[i].mDistance;//g cost take trace of the distance traveled (1 for straight and 1.4 for corners)

                    newNode.mGridPosition = nodesNear[i].mGridPosition;
                    newNode.mIndex = GetGridPosition(newNode.mGridPosition);

                    newNode.mHCost = calculateHCost(newNode.mGridPosition, pPlayer.mGridPosition);

                    newNode.mFCost = newNode.mGCost + newNode.mHCost; //here we create a new node for each node founds around the currentNode and we initialize them as we did for the starting one node
                    if (!mPathTraking.ContainsKey(newNode.mIndex.ToString())) //if the node is not still on the dictionary we add it so we know that our algorithm passde by that node at a certain parent
                    {
                        mPathTraking.Add(newNode.mIndex.ToString(), newNode);
                        nodes.Add(newNode);
                    }
                }
                nodes.Remove(currentNode);
                //then we get the one with lower f cost and we use it as currentNode
                currentNode = LowestFCost(nodes);

                if (currentNode == null)
                    return;
                if (currentNode.mGridPosition == pPlayer.mGridPosition)//this means we reached the target
                    break;
            }

            do
            {
                //add from the dictionary each node found to the percorso array following the map variable
                //the map variable start from zero plus each pass from the start position to the target
                mRoute.Add(currentNode.mGridPosition);
                if (mPathTraking.ContainsKey(currentNode.mParentIndex.ToString())) //this is a sort of rewind while where we flow the dictionary to found all the nodes of our path, following the parent attributes and reaching the parent -1 (the starting one)
                {
                    Node tmp;
                    mPathTraking.TryGetValue(currentNode.mParentIndex.ToString(), out tmp);
                    currentNode = tmp;
                }
            } while (currentNode.mParentIndex != -1);

            mRoute.Add(currentNode.mGridPosition);
            mFullPath.Clear();
            mFullPath.AddRange(mRoute); //we had the final path to full path

            //this functions are used to print the colours of the nodes visited by the algorithm
            clearColour(pMap);
            colourAllVisited(pMap);
            colourBounds(pMap);
            colourPath(pMap);
            mPathCalculated = true;
            mWorking = false;
        }

        private void colourAllVisited(Map pMap) //this colours all the tile visited of blue
        {
            for (int i = 0; i < mPathTraking.Values.Count; i++)
            {
                pMap.mCells[(int)mPathTraking.Values.ToList()[i].mGridPosition.X, (int)mPathTraking.Values.ToList()[i].mGridPosition.Y] = 3;
            }
        }
        private void clearColour(Map pMap) //this clear all colours in the tiles setting them to white
        {
            for (int i = 0; i < pMap.mGridSize; i++)
            {
                for (int j = 0; j < pMap.mGridSize; j++)
                {
                    if (pMap.mCells[i, j] == 3 || pMap.mCells[i, j] == 4 || pMap.mCells[i, j] == 2)
                    {
                        pMap.mCells[i, j] = 0;
                    }
                }
            }
        }

        private void colourBounds(Map pMap) //this colours the bound visited tiles of green
        {
            for (int i = 0; i < mPathTraking.Values.Count; i++)
            {
                if (checkColourNearNodes(mPathTraking.Values.ToList()[i], pMap))
                    pMap.mCells[(int)mPathTraking.Values.ToList()[i].mGridPosition.X, (int)mPathTraking.Values.ToList()[i].mGridPosition.Y] = 4;
            }
        }

        bool checkColourNearNodes(Node pNode, Map pMap) // this is used in the previous function to check if one tiles is in a bound
        {
            List<Node> nodes = getNodesNear(pNode, pMap);
            for (int i = 0; i < nodes.Count; i++)
                if (pMap.mCells[(int)nodes[i].mGridPosition.X, (int)nodes[i].mGridPosition.Y] == 0)
                    return true;

            return false;
        }

        private void colourPath(Map pMap) // this just colours the right path
        {
            for (int i = 0; i < mRoute.Count; i++)
            {
                pMap.mCells[(int)mFullPath[i].X, (int)mFullPath[i].Y] = 2;
            }
        }

        Node LowestFCost(List<Node> pNodes) // this function give the node of a list with the lower f cost (the near one to the target)
        {
            if (pNodes.Count <= 0)
                return null;
            Node minimumNode = pNodes[0];
            double minimumValue = double.MaxValue;
            for (int i = 0; i < pNodes.Count; i++)
            {
                if (minimumValue > pNodes[i].mFCost)
                {
                    minimumNode = pNodes[i];
                    minimumValue = pNodes[i].mFCost;
                }
            }
            return minimumNode;
        }
        public Vector2 GridToScreenPosition()
        {// this function is used to draw the bot in the right position in the gird
            return (mBotPosition * 15) + ((mBotPosition * 15) - (mBotPosition * 15));
        }
        int GetGridPosition(Vector2 pPos)
        {//this is used to calculate the index of each node on the grid
            return (int)pPos.Y + (int)pPos.X * mGridsize;
        }
        float calculateHCost(Vector2 pPathfinder, Vector2 pPlayer)
        {//this is used to calculate the h cost (Pythagorean theorem to calculate the distance between node and target)
            return (float)Math.Sqrt((double)((pPathfinder.X - pPlayer.X) * (pPathfinder.X - pPlayer.X)) + ((pPathfinder.Y - pPlayer.Y) * (pPathfinder.Y - pPlayer.Y)));
        }

        public List<Node> getNodesNear(Node pCurrentNode, Map pMap)//return all the 8 nodes near one node
        {
            //Simply first check for each location (top bottom right left and corners) if that node is in a valid location (if it's not a wall and if it's not off the map), and then initialize the node by adding it to the list
            List<Node> possibleNodes = new List<Node>();

            //UP
            if (pMap.ValidatePosition(new Vector2(pCurrentNode.mGridPosition.X, pCurrentNode.mGridPosition.Y + 1)))
            {
                possibleNodes.Add(new Node());
                possibleNodes[possibleNodes.Count - 1].mDistance = 1;
                possibleNodes[possibleNodes.Count - 1].mGridPosition = new Vector2(pCurrentNode.mGridPosition.X, pCurrentNode.mGridPosition.Y + 1);
            }
            //DOWN
            if (pMap.ValidatePosition(new Vector2(pCurrentNode.mGridPosition.X, pCurrentNode.mGridPosition.Y - 1)))
            {
                possibleNodes.Add(new Node());
                possibleNodes[possibleNodes.Count - 1].mDistance = 1;
                possibleNodes[possibleNodes.Count - 1].mGridPosition = new Vector2(pCurrentNode.mGridPosition.X, pCurrentNode.mGridPosition.Y - 1);
            }
            //LEFT
            if (pMap.ValidatePosition(new Vector2(pCurrentNode.mGridPosition.X - 1, pCurrentNode.mGridPosition.Y)))
            {
                possibleNodes.Add(new Node());
                possibleNodes[possibleNodes.Count - 1].mDistance = 1;
                possibleNodes[possibleNodes.Count - 1].mGridPosition = new Vector2(pCurrentNode.mGridPosition.X - 1, pCurrentNode.mGridPosition.Y);
            }
            //RIGHT
            if (pMap.ValidatePosition(new Vector2(pCurrentNode.mGridPosition.X + 1, pCurrentNode.mGridPosition.Y)))
            {
                possibleNodes.Add(new Node());
                possibleNodes[possibleNodes.Count - 1].mDistance = 1;
                possibleNodes[possibleNodes.Count - 1].mGridPosition = new Vector2(pCurrentNode.mGridPosition.X + 1, pCurrentNode.mGridPosition.Y);
            }
            //UP RIGHT
            if (pMap.ValidatePosition(new Vector2(pCurrentNode.mGridPosition.X + 1, pCurrentNode.mGridPosition.Y + 1)))
            {
                possibleNodes.Add(new Node());
                possibleNodes[possibleNodes.Count - 1].mDistance = 1.4f;
                possibleNodes[possibleNodes.Count - 1].mGridPosition = new Vector2(pCurrentNode.mGridPosition.X + 1, pCurrentNode.mGridPosition.Y + 1);
            }
            //UP LEFT
            if (pMap.ValidatePosition(new Vector2(pCurrentNode.mGridPosition.X - 1, pCurrentNode.mGridPosition.Y + 1)))
            {
                possibleNodes.Add(new Node());
                possibleNodes[possibleNodes.Count - 1].mDistance = 1.4f;
                possibleNodes[possibleNodes.Count - 1].mGridPosition = new Vector2(pCurrentNode.mGridPosition.X - 1, pCurrentNode.mGridPosition.Y + 1);
            }
            //DOWN RIGHT
            if (pMap.ValidatePosition(new Vector2(pCurrentNode.mGridPosition.X + 1, pCurrentNode.mGridPosition.Y - 1)))
            {
                possibleNodes.Add(new Node());
                possibleNodes[possibleNodes.Count - 1].mDistance = 1.4f;
                possibleNodes[possibleNodes.Count - 1].mGridPosition = new Vector2(pCurrentNode.mGridPosition.X + 1, pCurrentNode.mGridPosition.Y - 1);
            }
            //DOWN LEFT
            if (pMap.ValidatePosition(new Vector2(pCurrentNode.mGridPosition.X - 1, pCurrentNode.mGridPosition.Y - 1)))
            {
                possibleNodes.Add(new Node());
                possibleNodes[possibleNodes.Count - 1].mDistance = 1.4f;
                possibleNodes[possibleNodes.Count - 1].mGridPosition = new Vector2(pCurrentNode.mGridPosition.X - 1, pCurrentNode.mGridPosition.Y - 1);
            }

            return possibleNodes;
        }

    }
}

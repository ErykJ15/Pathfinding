using Microsoft.Xna.Framework;

namespace PathfindingProgram
{
    public class Node
    {
        //Nodes position on the grid, x and y
        public Vector2 mGridPosition;

        //the nodes index on the graph/map
        public int mIndex;

        //used to indicate the distance between the node itself and the previous node:
        //if the node is horizontal or vertical to the previous one, the distance is only 1,
        //while if the node is in a corner of the previous node then the distance is 1.4
        public float mDistance;

        //Index of the current nodes parent, helps keep track of the path
        public int mParentIndex;

        //cost of nodes from the starting point to current node
        public float mGCost;

        //heuristic cost, an estimate value between the current node and the target node
        public float mHCost;

        //the final cost of the path
        public float mFCost;


    }
}

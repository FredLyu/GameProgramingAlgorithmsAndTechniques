/******************************************
 *	Title:
 *	Description:
 *	Date:
 *	Version:
 *	Modify Recoder:
 *****************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyAI
{
    /// <summary>
    /// 节点类
    /// </summary>
    public class ANode : System.IComparable
    {
        /// <summary>
        /// 行
        /// </summary>
        public int Row { get; set; }
        /// <summary>
        /// 列
        /// </summary>
        public int Col { get; set; }

        /// <summary>
        /// 父节点
        /// </summary>
        public ANode parent = null;
        /// <summary>
        /// 相邻节点
        /// </summary>
        public List<ANode> adjacent = new List<ANode>();
        /// <summary>
        /// 曼哈顿距离
        /// </summary>
        public int h = 0;

        public int g = 0;

        public int f = 0;

        public void F(ANode startNode,ANode endNode)
        {
            h = Mathf.Abs(endNode.Row - Row) + Mathf.Abs(endNode.Col - Col);
            g = Mathf.Abs(startNode.Row - Row) + Mathf.Abs(startNode.Col - Col);

            f = g + h;
        }

        /// <summary>
        /// 清除
        /// </summary>
        public void Clear()
        {
            parent = null;
            h = 0;
            g = 0;
            f = 0;
            //adjacent.Clear();
        }

        public int CompareTo(object obj)
        {
            ANode node = obj as ANode;

            if (f - node.f < 0)
                return -1;
            else if (f - node.f == 0)
                return 0;
            else
                return 1;
        }
    }

    /// <summary>
    /// 图
    /// </summary>
    public class AMap
    {
        /// <summary>
        /// 列数
        /// </summary>
        public int cols = 0;
        /// <summary>
        /// 行数
        /// </summary>
        public int rows = 0;
        /// <summary>
        /// 行优先节点数组
        /// </summary>
        public ANode[] aNodes;

        public AMap(int[,] mapArray)
        {
            rows = mapArray.GetLength(0);
            cols = mapArray.GetLength(1);

            aNodes = new ANode[mapArray.Length];

            //分配节点的行号和列号
            for (int i = 0; i < aNodes.Length; i++)
            {
                ANode node = new ANode();
                node.Row = i / cols;
                node.Col = i - node.Row * cols;

                aNodes[i] = node;
            }

            int row, col;
            //分配邻居节点
            for (int j = 0; j < aNodes.Length; j++)
            {
                row = aNodes[j].Row;
                col = aNodes[j].Col;

                //1表示有障碍物，不能通过
                //0表示无障碍物，可以通过
                if (mapArray[row, col] != 1)
                {
                    //相邻上方的一个节点
                    if (row > 0 && mapArray[row - 1, col] != 1)
                        aNodes[j].adjacent.Add(aNodes[(row - 1) * cols + col]);
                    //相邻右边的一个节点
                    if (col + 1 < cols && mapArray[row, col + 1] != 1)
                        aNodes[j].adjacent.Add(aNodes[row * cols + col + 1]);
                    //相邻下方的一个节点
                    if (row + 1 < rows && mapArray[row + 1, col] != 1)
                        aNodes[j].adjacent.Add(aNodes[(row + 1) * cols + col]);
                    //相邻左边的一个节点
                    if (col > 0 && mapArray[row, col - 1] != 1)
                        aNodes[j].adjacent.Add(aNodes[row * cols + col - 1]);
                }
            }
        }

        
    }

    public class AStarAlgorithm
    {
        private ANode startNode;
        private ANode destNode;

        private List<ANode> openSet = new List<ANode>();
        private List<ANode> closedSet = new List<ANode>();

        private AMap map;

        /// <summary>
        /// 初始化地图
        /// </summary>
        /// <param name="map"></param>
        public AStarAlgorithm(AMap map)
        {
            this.map = map;
        }

        /// <summary>
        /// 查找开放式集合中H值最小的节点
        /// </summary>
        /// <returns></returns>
        private ANode FindLowest()
        {
            openSet.Sort();
            Debug.Log(openSet[0].f);
            return openSet[0];

            float minF = openSet[0].f;
            ANode minNode = openSet[0];

            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].f < minF)
                {
                    minF = openSet[i].f;
                    minNode = openSet[i];
                }
            }

            return minNode;
        }

        /// <summary>
        /// 将节点的相邻节点添加到开放集合中
        /// </summary>
        /// <param name="node"></param>
        private void AddAdjacent(ANode node)
        {
            for (int i = 0; i < node.adjacent.Count; i++)
            {
                if (closedSet.Contains(node.adjacent[i]))
                    continue;
                else if(openSet.Contains(node.adjacent[i]))
                {
                    int newG = node.adjacent[i].g + (Mathf.Abs(node.Row - node.adjacent[i].Row) + Mathf.Abs(node.Col - node.adjacent[i].Col));
                    if(newG < node.adjacent[i].g)
                    {
                        node.adjacent[i].parent = node;
                        node.adjacent[i].g = newG;
                        node.adjacent[i].f = newG + node.adjacent[i].h;
                    }
                }
                else
                {
                    node.adjacent[i].parent = node;
                    node.adjacent[i].F(startNode, destNode);
                    openSet.Add(node.adjacent[i]);
                }
            }
        }

        /// <summary>
        /// 更新地图
        /// </summary>
        /// <param name="map"></param>
        public void UpdateMap(AMap map)
        {
            this.map = map;
        }

        public void Start(ANode startNode, ANode endNode)
        {
            openSet.Clear();
            closedSet.Clear();

            closedSet.Add(startNode);
            destNode = endNode;
            this.startNode = startNode;

            for (int i = 0; i < map.aNodes.Length; i++)
            {
                map.aNodes[i].Clear();
            }
        }

        public Stack<ANode> Find()
        {
            Stack<ANode> path = new Stack<ANode>();

            ANode currNode = closedSet[0];

            while (currNode != destNode)
            {
                AddAdjacent(currNode);
                if (openSet.Count == 0)
                    break;

                currNode = FindLowest();
                openSet.Remove(currNode);
                closedSet.Add(currNode);
            }

            if (currNode == destNode)
            {
                ANode node = destNode;
                while (node != null)
                {
                    path.Push(node);
                    node = node.parent;
                }
            }
            else
                return null;

            return path;
        }
    }
}

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
    public class Node
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
        public Node parent = null;
        /// <summary>
        /// 相邻节点
        /// </summary>
        public List<Node> adjacent = new List<Node>();
        /// <summary>
        /// 曼哈顿距离
        /// </summary>
        public float h = 0;

        /// <summary>
        /// 清除
        /// </summary>
        public void Clear()
        {
            parent = null;
            h = 0;
            //adjacent.Clear();
        }
    }

    /// <summary>
    /// 图
    /// </summary>
    public class Map
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
        public Node[] nodes;

        public Map(int[,] mapArray)
        {
            rows = mapArray.GetLength(0);
            cols = mapArray.GetLength(1);

            nodes = new Node[mapArray.Length];

            //分配节点的行号和列号
            for (int i = 0; i < nodes.Length; i++)
            {
                Node node = new Node();
                node.Row = i / cols;
                node.Col = i - node.Row * cols;

                nodes[i] = node;
            }

            int row , col;
            //分配邻居节点
            for (int j = 0; j < nodes.Length; j++)
            {
                row = nodes[j].Row;
                col = nodes[j].Col;

                //1表示有障碍物，不能通过
                //0表示无障碍物，可以通过
                if(mapArray[row,col] != 1)
                {
                    //相邻上方的一个节点
                    if (row > 0 && mapArray[row - 1, col] != 1)
                        nodes[j].adjacent.Add(nodes[(row - 1) * cols + col]);
                    //相邻右边的一个节点
                    if (col + 1 < cols && mapArray[row, col + 1] != 1)
                        nodes[j].adjacent.Add(nodes[row * cols + col + 1]);
                    //相邻下方的一个节点
                    if (row + 1 < rows && mapArray[row + 1, col] != 1)
                        nodes[j].adjacent.Add(nodes[(row + 1) * cols + col]);
                    //相邻左边的一个节点
                    if (col > 0 && mapArray[row, col - 1] != 1)
                        nodes[j].adjacent.Add(nodes[row * cols + col - 1]);
                }
            }
        }
    }

    public class GreedyBestPriorityAlgorithm
    {
        private Node destNode;

        private List<Node> openSet = new List<Node>();
        private List<Node> closedSet = new List<Node>();

        private Map map;

        /// <summary>
        /// 初始化地图
        /// </summary>
        /// <param name="map"></param>
        public GreedyBestPriorityAlgorithm(Map map)
        {
            this.map = map;
        }

        /// <summary>
        /// 曼哈顿距离
        /// </summary>
        /// <param name="node">当前节点</param>
        /// <returns></returns>
        private float H(Node node)
        {
            return Mathf.Abs(node.Row - destNode.Row) + Mathf.Abs(node.Col - destNode.Col);
        }

        /// <summary>
        /// 查找开放式集合中H值最小的节点
        /// </summary>
        /// <returns></returns>
        private Node FindLowestH()
        {
            float minH = openSet[0].h;
            Node minNode = openSet[0];

            for (int i = 1; i < openSet.Count; i++)
            {
                if(openSet[i].h < minH)
                {
                    minH = openSet[i].h;
                    minNode = openSet[i];
                }
            }

            return minNode;
        }

        /// <summary>
        /// 将节点的相邻节点添加到开放集合中
        /// </summary>
        /// <param name="node"></param>
        private void AddAdjacent(Node node)
        {
            for (int i = 0; i < node.adjacent.Count; i++)
            {
                if (closedSet.Contains(node.adjacent[i])/* || openSet.Contains(node.adjacent[i])*/)
                    continue;
                else
                {
                    node.adjacent[i].parent = node;
                    if(!openSet.Contains(node.adjacent[i]))
                    {
                        node.adjacent[i].h = H(node.adjacent[i]);
                        openSet.Add(node.adjacent[i]);
                    }
                }
            }
        }

        /// <summary>
        /// 更新地图
        /// </summary>
        /// <param name="map"></param>
        public void UpdateMap(Map map)
        {
            this.map = map;
        }

        public void Start(Node startNode, Node endNode)
        {
            openSet.Clear();
            closedSet.Clear();

            closedSet.Add(startNode);
            destNode = endNode;

            for (int i = 0; i < map.nodes.Length; i++)
            {
                map.nodes[i].Clear();
            }
        }

        public Stack<Node> Find()
        {
            Stack<Node> path = new Stack<Node>();

            Node currNode = closedSet[0];

            while (currNode != destNode)
            {     
                AddAdjacent(currNode);
                if (openSet.Count == 0)
                    break;

                currNode = FindLowestH();
                openSet.Remove(currNode);
                closedSet.Add(currNode);
            }

            if (currNode == destNode)
            {
                Node node = destNode;
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

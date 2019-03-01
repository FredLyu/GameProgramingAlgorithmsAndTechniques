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
using UnityEngine.UI;
using MyAI;

public class TestMap : MonoBehaviour 
{
    public int[,] map;
    public RawImage[] imgaes;

    public int startRow = 0;
    public int startCol = 0;
    public int endRow = 0;
    public int endCol = 0;

    int GetIndex(int row,int col,int[,] tempMap)
    {
        int cols = tempMap.GetLength(1);
        return row * cols + col;
    }

    void ResetMapColor(Map map)
    {
        for (int i = 0; i < map.nodes.Length; i++)
        {
            imgaes[i].color = map.nodes[i].adjacent.Count == 0 ? Color.gray : Color.white;
        }
    }

	void Start () 
	{
        map = new int[,] {
            {0, 0, 0, 1, 0 },
            {0, 0, 0, 1, 0 },
            {0, 0, 0, 1, 0 },
            {0, 1, 1, 1, 0 },
            {0, 0, 0, 0, 0 }
        };
        Debug.Log("Rows = " + map.GetLength(0) + ", Cols = " + map.GetLength(1));

        if(map[startRow,startCol] == 1)
        {
            Debug.Log("Error:起点不能是墙!!!");
            return;
        }

        if(map[endRow,endCol] == 1)
        {
            Debug.Log("Error:终点不能是墙!!!");
        }

        if(startRow < 0 || startRow >= map.GetLength(0) || startCol < 0 || startCol >= map.GetLength(1) ||
            endRow < 0 || endRow >= map.GetLength(0) || endCol < 0 || endCol >= map.GetLength(1))
        {
            Debug.Log("Error:非法的索引值!!!");
            return;
        }

        Map myMap = new Map(map);
        GreedyBestPriorityAlgorithm gbpa = new GreedyBestPriorityAlgorithm(myMap);
        gbpa.Start(myMap.nodes[GetIndex(startRow, startCol, map)], myMap.nodes[GetIndex(endRow, endCol, map)]);
        Stack<Node> path = gbpa.Find();

        ResetMapColor(myMap);

        if(path != null)
        {
            Debug.Log("Serach one path!");
            while(path.Count != 0)
            {
                Node node = path.Pop();
                Debug.Log("Node row = " + node.Row + ", col = " + node.Col);
                imgaes[GetIndex(node.Row, node.Col, map)].color = Color.blue;
            }
        }
        else
        {
            Debug.Log("Some Errors!!!");
        }
	}
}

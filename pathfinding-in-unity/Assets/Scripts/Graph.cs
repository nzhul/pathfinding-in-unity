﻿using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class Graph : MonoBehaviour
    {
        public Node[,] nodes;
        public List<Node> walls = new List<Node>();

        int[,] _mapData;
        int _width;
        int _height;

        public static readonly Vector2[] allDirections =
        {
            new Vector2(0,1),
            new Vector2(1,1),
            new Vector2(1,0),
            new Vector2(1,-1),
            new Vector2(0,-1),
            new Vector2(-1,-1),
            new Vector2(-1,0),
            new Vector2(-1,1)
        };

        public void Init(int[,] mapData)
        {
            _mapData = mapData;
            _width = mapData.GetLength(0);
            _height = mapData.GetLength(1);

            nodes = new Node[_width, _height];
            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    NodeType type = (NodeType)mapData[x, y];
                    Node newNode = new Node(x, y, type);
                    nodes[x, y] = newNode;

                    newNode.position = new Vector3(x, 0, y);

                    if (type == NodeType.Blocked)
                    {
                        walls.Add(newNode);
                    }
                }
            }

            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    if (nodes[x, y].nodeType != NodeType.Blocked)
                    {
                        nodes[x, y].neighbors = GetNeighbors(x, y);
                    }
                }
            }
        }

        public bool IsWithinBounds(int x, int y)
        {
            return (x >= 0 && x < _width && y >= 0 && y < _height);
        }

        List<Node> GetNeighbors(int x, int y, Node[,] nodeArray, Vector2[] directions)
        {
            List<Node> neighborNodes = new List<Node>();

            foreach (Vector2 dir in directions)
            {
                int newX = x + (int)dir.x;
                int newY = y + (int)dir.y;

                if (IsWithinBounds(newX, newY)
                    && nodeArray[newX, newY] != null
                    && nodeArray[newX, newY].nodeType != NodeType.Blocked)
                {
                    neighborNodes.Add(nodeArray[newX, newY]);
                }
            }

            return neighborNodes;
        }

        List<Node> GetNeighbors(int x, int y)
        {
            return this.GetNeighbors(x, y, nodes, allDirections);
        }


    }
}
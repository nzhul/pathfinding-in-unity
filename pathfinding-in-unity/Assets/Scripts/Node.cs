using System.Collections.Generic;
using UnityEngine;
using System;

public enum NodeType
{
    Open,
    Blocked
}

public class Node : IComparable<Node>
{

    public NodeType nodeType = NodeType.Open;

    public int xIndex = -1;
    public int yIndex = -1;

    public Vector3 position;

    public List<Node> neighbors = new List<Node>();

    public float distanceTraveled = Mathf.Infinity;
    public Node previous = null;

    public int priority;

    public Node(int xIndex, int yIndex, NodeType nodeType)
    {
        this.xIndex = xIndex;
        this.yIndex = yIndex;
        this.nodeType = nodeType;
    }

    public int CompareTo(Node other)
    {
        if (this.priority < other.priority)
        {
            return -1;
        }
        else if (this.priority > other.priority)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

    public void Reset()
    {
        previous = null;
    }

}

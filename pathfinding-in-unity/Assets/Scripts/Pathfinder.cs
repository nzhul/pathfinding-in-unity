using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public class Pathfinder : MonoBehaviour
    {
        Node _startNode;
        Node _goalNode;
        Graph _graph;
        GraphView _graphView;

        Queue<Node> _frontierNodes;
        List<Node> _exploredNodes;
        List<Node> _pathNodes;

        public Color startColor = Color.green;
        public Color goalColor = Color.red;
        public Color frontierColor = Color.magenta;
        public Color exploredColor = Color.gray;
        public Color pathColor = Color.cyan;

        public bool isComplete = false;
        int _iterations = 0;


        public void Init(Graph graph, GraphView graphView, Node start, Node goal)
        {
            if (start == null || goal == null || graph == null || graphView == null)
            {
                Debug.LogWarning("PATHFINDER init error: missing component(s)!");
                return;
            }

            if (start.nodeType == NodeType.Blocked || goal.nodeType == NodeType.Blocked)
            {
                Debug.LogWarning("PATHFINDER init error: start and end goal nodes must be unblocked!");
                return;
            }

            _graph = graph;
            _graphView = graphView;
            _startNode = start;
            _goalNode = goal;

            ShowColors(graphView, start, goal);

            _frontierNodes = new Queue<Node>();
            _frontierNodes.Enqueue(start);
            _exploredNodes = new List<Node>();
            _pathNodes = new List<Node>();

            for (int x = 0; x < _graph.Width; x++)
            {
                for (int y = 0; y < _graph.Height; y++)
                {
                    _graph.nodes[x, y].Reset();
                }
            }

            isComplete = false;
            _iterations = 0;
        }

        private void ShowColors(GraphView graphView, Node start, Node goal)
        {
            if (graphView == null || start == null || goal == null)
            {
                return;
            }

            if (_frontierNodes != null)
            {
                graphView.ColorNodes(_frontierNodes.ToList(), frontierColor);
            }

            if (_exploredNodes != null)
            {
                graphView.ColorNodes(_exploredNodes, exploredColor);
            }

            NodeView startNodeView = graphView.nodeViews[start.xIndex, start.yIndex];
            if (startNodeView != null)
            {
                startNodeView.ColorNode(startColor);
            }

            NodeView goalNodeView = graphView.nodeViews[goal.xIndex, goal.yIndex];
            if (goalNodeView != null)
            {
                goalNodeView.ColorNode(goalColor);
            }
        }

        void ShowColors()
        {
            ShowColors(_graphView, _startNode, _goalNode);
        }
    }
}

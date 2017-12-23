using System.Collections;
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
        public Color arrowColor = new Color32(216, 216, 216, 255);
        public Color highlightColor = new Color32(255, 255, 128, 255);

        public bool showIterations = true;
        public bool showColors = true;
        public bool showArrows = true;
        public bool exitOnGoal = true;

        public bool isComplete = false;
        int _iterations = 0;

        public enum Mode
        {
            BreathFirstSearch = 0,
            Dijkstra = 1
        }

        public Mode mode = Mode.BreathFirstSearch;

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
            _startNode.distanceTraveled = 0;
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

            if (_pathNodes != null && _pathNodes.Count > 0)
            {
                graphView.ColorNodes(_pathNodes, pathColor);
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

        public IEnumerator SearchRoutine(float timeStep = 0.1f)
        {
            float timeStart = Time.time;

            yield return null;

            while (!isComplete)
            {
                if (_frontierNodes.Count > 0)
                {
                    Node currentNode = _frontierNodes.Dequeue();
                    _iterations++;

                    if (!_exploredNodes.Contains(currentNode))
                    {
                        _exploredNodes.Add(currentNode);
                    }

                    if (mode == Mode.BreathFirstSearch)
                    {
                        ExpandFrontierBreathFirst(currentNode);
                    }
                    else if (mode == Mode.Dijkstra)
                    {
                        ExpandFrontierDijkstra(currentNode);
                    }

                    if (_frontierNodes.Contains(_goalNode))
                    {
                        _pathNodes = GetPathNodes(_goalNode);
                        if (exitOnGoal)
                        {
                            isComplete = true;
                            Debug.Log("PATHFINDER mode: " + mode.ToString() + "   path length = " + _goalNode.distanceTraveled.ToString());
                        }
                    }

                    if (showIterations)
                    {
                        ShowDiagnostics();

                        yield return new WaitForSeconds(timeStep);
                    }
                }
                else
                {
                    isComplete = true;
                }
            }

            ShowDiagnostics();
            Debug.Log("PATHFINDER Searchroutine: elapsed time = " + (Time.time - timeStart).ToString() + " seconds");
        }

        private void ShowDiagnostics()
        {
            if (showColors)
            {
                ShowColors();
            }

            if (_graphView != null && showArrows)
            {
                _graphView.ShowNodeArrows(_frontierNodes.ToList(), arrowColor);

                if (_frontierNodes.Contains(_goalNode))
                {
                    _graphView.ShowNodeArrows(_pathNodes, highlightColor);
                }
            }
        }

        void ExpandFrontierBreathFirst(Node node)
        {
            if (node != null)
            {
                for (int i = 0; i < node.neighbors.Count; i++)
                {
                    if (!_exploredNodes.Contains(node.neighbors[i])
                        && !_frontierNodes.Contains(node.neighbors[i]))
                    {
                        float distanceToNeighbor = _graph.GetNodeDistance(node, node.neighbors[i]);
                        float newDistanceTraveled = distanceToNeighbor + node.distanceTraveled;
                        node.neighbors[i].distanceTraveled = newDistanceTraveled;

                        node.neighbors[i].previous = node;
                        _frontierNodes.Enqueue(node.neighbors[i]);
                    }
                }
            }
        }

        void ExpandFrontierDijkstra(Node node)
        {
            if (node != null)
            {
                for (int i = 0; i < node.neighbors.Count; i++)
                {
                    if (!_exploredNodes.Contains(node.neighbors[i]))
                    {
                        float distanceToNeighbor = _graph.GetNodeDistance(node, node.neighbors[i]);
                        float newDistanceTraveled = distanceToNeighbor + node.distanceTraveled;

                        if (float.IsPositiveInfinity(node.neighbors[i].distanceTraveled) ||
                            newDistanceTraveled < node.neighbors[i].distanceTraveled)
                        {
                            node.neighbors[i].previous = node;
                            node.neighbors[i].distanceTraveled = newDistanceTraveled;
                        }

                        if (!_frontierNodes.Contains(node.neighbors[i]))
                        {
                            _frontierNodes.Enqueue(node.neighbors[i]);
                        }
                    }
                }
            }
        }

        List<Node> GetPathNodes(Node endNode)
        {
            List<Node> path = new List<Node>();
            if (endNode == null)
            {
                return path;
            }
            path.Add(endNode);

            Node currentNode = endNode.previous;

            while (currentNode != null)
            {
                path.Insert(0, currentNode);
                currentNode = currentNode.previous;
            }

            return path;
        }
    }
}

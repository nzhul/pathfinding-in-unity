﻿using UnityEngine;

namespace Assets.Scripts
{
    public class DemoController : MonoBehaviour
    {
        public MapData mapData;
        public Graph graph;

        private void Start()
        {
            if (mapData != null && graph != null)
            {
                int[,] mapInstance = mapData.MakeMap();
                graph.Init(mapInstance);

                GraphView graphView = graph.gameObject.GetComponent<GraphView>();

                if (graphView != null)
                {
                    graphView.Init(graph);
                }
            }
        }
    }
}
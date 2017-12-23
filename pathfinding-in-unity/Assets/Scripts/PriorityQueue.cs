using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
    // Priority queue works with binary heap
    public class PriorityQueue<T> where T : IComparable<T>
    {
        List<T> data;

        public PriorityQueue()
        {
            this.data = new List<T>();
        }


    }
}

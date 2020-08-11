using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manybits
{
    public struct Day: IComparable<Day>
    {
        public int id;
        public bool isComplited;

        public static readonly Day empty = new Day() { id = -1, isComplited = false };

        public Day(int id, bool isComplited = false)
        {
            this.id = id;
            this.isComplited = isComplited;
        }

        public override string ToString()
        {
            return $"{id} ({isComplited})";
        }

        public int CompareTo(Day other)
        {
            if (id > other.id)
                return 1;
            else if (id < other.id)
                return -1;
            else
                return 0;
        }
    }
}

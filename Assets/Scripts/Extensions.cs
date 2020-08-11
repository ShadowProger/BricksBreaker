using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manybits
{
    using Random = UnityEngine.Random;

    public static partial class Extentions
    {
        public static T GetRandom<T>(this List<T> list)
        {
            var result = list[UnityEngine.Random.Range(0, list.Count)];
            return result;
        }



        public static void Shuffle<T>(this List<T> list)
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                int k = Random.Range(0, i);
                T tmp = list[i];
                list[i] = list[k];
                list[k] = tmp;
            }
        }



        public static void DebugDraw(this Rect rect, Color color)
        {
            Debug.DrawLine(rect.min, new Vector3(rect.xMin, rect.yMax, 0), color);
            Debug.DrawLine(new Vector3(rect.xMin, rect.yMax, 0), rect.max, color);
            Debug.DrawLine(rect.max, new Vector3(rect.xMax, rect.yMin, 0), color);
            Debug.DrawLine(new Vector3(rect.xMax, rect.yMin, 0), rect.min, color);
        }



        public static int ToInt(this DateTime date)
        {
            return date.Year * 10000 + date.Month * 100 + date.Day;
        }
    }
}

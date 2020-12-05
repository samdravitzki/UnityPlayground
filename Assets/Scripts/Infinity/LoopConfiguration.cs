using System;
using UnityEngine;

namespace Infinity
{
    [CreateAssetMenu(fileName = "LoopConfiguration", menuName = "Loop Config", order = 1500)]
    public class LoopConfiguration: ScriptableObject
    {
        public Bounds bounds = new Bounds(Vector3.zero, new Vector3(20, 20, 20));
    }
}
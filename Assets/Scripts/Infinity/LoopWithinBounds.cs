using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Infinity
{
    public class LoopWithinBounds : MonoBehaviour
    {
        [SerializeField]
        private LoopConfiguration settings;
        
        public LoopConfiguration Settings =>
            settings
                ? settings
                : (settings = ScriptableObject.CreateInstance<LoopConfiguration>());

        private Bounds LoopBounds => Settings.bounds;

        // Update is called once per frame
        void Update()
        {
            // TODO: Problems 1. Only works in one direction (Downwards looping, but not up), 2. Still get jerking
            var y = transform.position.y;
            var loopedy = ((y-LoopBounds.extents.y) % -LoopBounds.size.y) + LoopBounds.extents.y;
            // var loopedPos = (transform.position - LoopBounds.extents).Mod(-LoopBounds.size) + LoopBounds.extents;
            transform.position = new Vector3(transform.position.x, loopedy, transform.position.z);
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(1, 0, 0);
            Gizmos.DrawWireCube(LoopBounds.center, new Vector3(LoopBounds.extents.x * 2, LoopBounds.extents.y * 2, LoopBounds.extents.z * 2));
        }
    }
}

static class Vector3Extensions
{
    public static Vector3 Mod(this Vector3 v, Vector3 mod)
    {
        return new Vector3(v.x % mod.x, v.y % mod.y, v.z % mod.z); 
    }
}


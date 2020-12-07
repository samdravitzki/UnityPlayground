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
            var pos = transform.position;
            if (pos.y >= LoopBounds.max.y)
                transform.position = ToBottom(transform.position);
            if (pos.y <= LoopBounds.min.y)
                transform.position = ToTop(transform.position);
            if (pos.z >= LoopBounds.max.z)
                transform.position = ToBackward(transform.position);
            if (pos.z <= LoopBounds.min.z)
                transform.position = ToForward(transform.position);
            if (pos.x >= LoopBounds.max.x)
                transform.position = ToLeft(transform.position);
            if (pos.x <= LoopBounds.min.x)
                transform.position = ToRight(transform.position);
        }

        Vector3 ToTop(Vector3 pos)
        {
            return new Vector3(pos.x, LoopBounds.max.y, pos.z); // + Vector3.down;
        }

        Vector3 ToBottom(Vector3 pos)
        {
            return new Vector3(pos.x, LoopBounds.min.y, pos.z); // + Vector3.up;
        }

        Vector3 ToForward(Vector3 pos)
        {
            return new Vector3(pos.x, pos.y, LoopBounds.max.z); // + Vector3.back;
        }

        Vector3 ToBackward(Vector3 pos)
        {
            return new Vector3(pos.x, pos.y, LoopBounds.min.z); // + Vector3.forward;
        }

        Vector3 ToRight(Vector3 pos)
        {
            return new Vector3(LoopBounds.max.x, pos.y, pos.z); // + Vector3.left;
        }

        Vector3 ToLeft(Vector3 pos)
        {
            return new Vector3(LoopBounds.min.x, pos.y, pos.z); // + Vector3.right;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(0, 1, 0);
            Gizmos.DrawWireCube(LoopBounds.center, new Vector3(LoopBounds.extents.x * 2, LoopBounds.extents.y * 2, LoopBounds.extents.z * 2));
        }
    }
}

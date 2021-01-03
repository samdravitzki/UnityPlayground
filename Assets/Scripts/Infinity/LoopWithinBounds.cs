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
            var loopedPos = new Vector3(
                InifinityUtilities.FlipClamp(pos.x, LoopBounds.min.x, LoopBounds.max.x),
                InifinityUtilities.FlipClamp(pos.y, LoopBounds.min.y, LoopBounds.max.y),
                InifinityUtilities.FlipClamp(pos.z, LoopBounds.min.z, LoopBounds.max.z)
                );
            transform.position = loopedPos;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(1, 0, 0);
            Gizmos.DrawWireCube(LoopBounds.center, new Vector3(LoopBounds.extents.x * 2, LoopBounds.extents.y * 2, LoopBounds.extents.z * 2));
        }
    }
}


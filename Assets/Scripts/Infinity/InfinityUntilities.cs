using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Infinity
{
    static class InifinityUtilities
    {
        // Flip clamp is a flipped version of a clamp operation
        // Where the where when a value reaches max it is set to min and vice versa
        // Standard clamp operation for reference:
        // var t = val < min ? min : val;
        // return t > max ? max : t;
        public static float FlipClamp(float val, float min, float max) {
            var t = val < min ? max : val;
            return t > max ? min : t;
        }
    }
    
}

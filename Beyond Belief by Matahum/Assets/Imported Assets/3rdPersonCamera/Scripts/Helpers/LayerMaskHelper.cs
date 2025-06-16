//----------------------------------------------
//            3rd Person Camera
// Copyright © 2015-2024 Thomas Enzenebner
//            Version 1.0.8.1
//         t.enzenebner@gmail.com
//----------------------------------------------
using UnityEngine;

namespace ThirdPersonCamera
{
    public static class LayerMaskHelper
    {

        public static bool IsInLayerMask(this LayerMask mask, int layer)
        {
            return ((mask.value & (1 << layer)) > 0);
        }

        public static bool IsInLayerMask(this LayerMask mask, GameObject obj)
        {
            return ((mask.value & (1 << obj.layer)) > 0);
        }
    }
}


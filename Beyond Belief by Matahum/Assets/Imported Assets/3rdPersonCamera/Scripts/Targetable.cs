//----------------------------------------------
//            3rd Person Camera
// Copyright © 2015-2024 Thomas Enzenebner
//            Version 1.0.8.1
//         t.enzenebner@gmail.com
//----------------------------------------------
using UnityEngine;
using UnityEngine.Serialization;

namespace ThirdPersonCamera
{
    public class Targetable : MonoBehaviour
    {
        [FormerlySerializedAs("offset")] 
        [Tooltip("Set an offset for the target when the transform.position is not fitting.")]
        public Vector3 Offset;
    }
}

//----------------------------------------------
//            3rd Person Camera
// Copyright © 2015-2024 Thomas Enzenebner
//            Version 1.0.8.1
//         t.enzenebner@gmail.com
//----------------------------------------------
using System;
using UnityEngine;

namespace ThirdPersonCamera
{
    [Serializable]
    public struct CameraControllerState
    {
        public Quaternion CameraRotation;   // do not directly write to cameraRotation, use cameraAngles instead
        public Quaternion SmartPivotRotation;
        public Quaternion CurrentPivotRotation;
        public Quaternion PivotRotation;

        public Vector3 CurrentCameraOffsetVector;
        public Vector3 DesiredPosition;
        public Vector3 DirToTarget;
        public Vector3 PrevPosition;
        
        public Vector3 CameraAngles;

        public float Distance;
        public float PresentationDistance;
        public float SmartPivotStartingX;
        public float CameraOffsetMagnitude;

        public bool CameraNormalMode;
        public bool GroundHit;
        public bool SmartPivotInitialized;
        public bool OcclusionHitHappened;
        public bool InvertCameraOffsetVector;
    }
}
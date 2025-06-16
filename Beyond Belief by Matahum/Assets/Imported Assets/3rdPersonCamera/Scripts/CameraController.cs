//----------------------------------------------
//            3rd Person Camera
// Copyright © 2015-2024 Thomas Enzenebner
//            Version 1.0.8.1
//         t.enzenebner@gmail.com
//----------------------------------------------

//#define TPC_DEBUG // uncomment for more options and visualizing debug raycasts

using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Serialization;

namespace ThirdPersonCamera
{
    [DefaultExecutionOrder(100)]
    public class CameraController : MonoBehaviour
    {
        [Header("Camera Shake")]
        private float shakeDuration;
        private float shakeAmount;
        private float shakeTimer;
        private Vector3 shakeOffset;
        [FormerlySerializedAs("target")] [Tooltip("Set this to the transform the camera should follow, can be changed at runtime")]
        public Transform Target;

        [FormerlySerializedAs("desiredDistance")] [Header("Basic settings")] [Tooltip("The distance how far away the camera should be away from the target")]
        public float DesiredDistance = 5.0f;

        [FormerlySerializedAs("collisionDistance")] [Tooltip("Offset for the camera to not clip in objects")]
        public float CollisionDistance = 0.25f;

        [FormerlySerializedAs("offsetVector")] [Tooltip("Change this vector to offset the pivot the camera is rotating around")]
        public Vector3 OffsetVector = new Vector3(0, 1.0f, 0);

        [FormerlySerializedAs("cameraOffsetVector")] [Tooltip("Offset the camera in any axis without changing the pivot point")]
        public Vector3 CameraOffsetVector = new Vector3(0, 0, 0);

        [Tooltip("Completely disable zooming in/out manually")]
        public bool DisableZooming;

        [FormerlySerializedAs("zoomOutStepValue")] [Tooltip("The distance how fast the player can zoom in/out with each mousewheel event.")]
        public float ZoomOutStepValue = 1.0f;

        [FormerlySerializedAs("occludedZoomInSpeed")] [Tooltip("The speed, how fast the camera can zoom in automatically to the desired distance.")]
        public float OccludedZoomInSpeed = 60.0f;

        [FormerlySerializedAs("unoccludedZoomOutSpeed")] [Tooltip("The speed, how fast the camera can zoom out automatically to the desired distance.")]
        public float UnoccludedZoomOutSpeed = 7.0f;

        [FormerlySerializedAs("cameraOffsetChangeSpeed")] [Tooltip("How fast the offset can change.")]
        public float CameraOffsetChangeSpeed = 3.0f;

        [FormerlySerializedAs("cameraOffsetMinDistance")] [Tooltip("The minimum distance the camera offset can decrease to.")]
        public float CameraOffsetMinDistance = 0.2f;

        [FormerlySerializedAs("hideSkinnedMeshRenderers")] [Tooltip("Automatically turns off a targets skinned mesh renderer when the camera collider hits the player collider")]
        public bool HideSkinnedMeshRenderers;

        [FormerlySerializedAs("collisionLayer")] [Header("Collision layer settings")] [Tooltip("Set this layermask to specify with which layers the camera should collide")]
        public LayerMask CollisionLayer;

        [FormerlySerializedAs("playerLayer")] [Tooltip("Set this to your player layer so ground checks will ignore the player collider")]
        public LayerMask PlayerLayer;

        [FormerlySerializedAs("occlusionCheck")] [Header("Features")] [Tooltip("Automatically reposition the camera when the character is blocked by an obstacle.")]
        public bool OcclusionCheck = true;

        [FormerlySerializedAs("smartPivot")] [Tooltip("Uses a pivot when the camera hits the ground and prevents moving the camera closer to the target when looking up.")]
        public bool SmartPivot = true;

        [FormerlySerializedAs("thicknessCheck")] [Tooltip("Thickness checking can be configured to ignore  smaller obstacles like sticks, trees, etc... to not reposition or zoom in the camera.")]
        public bool ThicknessCheck = true;

        [FormerlySerializedAs("smoothTargetMode")]
        [Header("Smooth target mode")]
        [Tooltip(
            "Enable to smooth out target following, to hide noisy position changes that should not be picked up by the camera immediately or to enable smooth transitions to other targets that are changed during runtime")]
        public bool SmoothTargetMode;

        [FormerlySerializedAs("smoothTargetValue")] [Tooltip("The speed at which the camera lerps to the actual target position for each axis. 0 means no smoothing will be applied!")]
        public Vector3 SmoothTargetValue = new Vector3(15.0f, 4.0f, 15.0f);

        [FormerlySerializedAs("maxThickness")]
        [Header("Thickness check settings")]
        [Tooltip(
            "Adjusts the thickness check. The higher, the thicker the objects can be and there will be no occlusion check.Warning: Very high values could make Occlusion checking obsolete and as a result the followed target can be occluded")]
        public float MaxThickness = 0.3f;

        [FormerlySerializedAs("smartPivotOnlyGround")] [Header("Smart Pivot settings")] [Tooltip("Smart pivot is only activated on stable grounds")]
        public bool SmartPivotOnlyGround = true;

        [FormerlySerializedAs("floorNormalUp")] [Tooltip("Default ground up for smart pivot")]
        public Vector3 FloorNormalUp = new Vector3(0, 1, 0);

        [FormerlySerializedAs("maxFloorDot")] [Tooltip("Only hit results from maxFloorDot to 1 will be valid")]
        public float MaxFloorDot = 0.85f;

        [FormerlySerializedAs("pivotRotationEnabled")]
        [Header("Static pivot rotation settings")]
        [Tooltip("Enable pivot rotation feature to rotate the camera in any direction without the camera moving or rotation around. Used in LockOn and Follow.")]
        public bool PivotRotationEnabled;

        [FormerlySerializedAs("smoothPivot")] [Tooltip("When enabled the pivot will smoothly slerp to the new pivot angles")]
        public bool SmoothPivot;

        [FormerlySerializedAs("customPivot")] [Tooltip("Enable overriding the pivot with the public variable 'pivotAngles'")]
        public bool CustomPivot;

        [FormerlySerializedAs("pivotAngles")] [Tooltip("Use to override pivot")]
        public Vector3 PivotAngles;

        [FormerlySerializedAs("maxRaycastHitCount")]
        [Header("Extra settings")]
        [Tooltip(
            "The maximum amount of raycast hits that are available for occlusion hits. Can be tweaked for performance. Don't set amount less than 2 when your geometry has many polygons as this can make repositioning unreliable.")]
        public int MaxRaycastHitCount = 6;

        [FormerlySerializedAs("maxThicknessRaycastHitCount")]
        [Tooltip(
            "The maximum amount of raycast hits that are available for thickness hits. Can be tweaked for performance. Use an even amount to account for back-front face and don't use less than 2.")]
        public int MaxThicknessRaycastHitCount = 6;

#if TPC_DEBUG
        [Header("Debugging")]
        public bool showDesiredPosition;
        public bool drawOcclusionHit;
        public bool drawDirToTarget;
        
        // offset boxtest debug
        public bool drawOffsetBoxcast;
        public bool drawBoxCastHits;
        public bool drawOffsetBoxCastResults;
        
        // thickness check debug
        public bool drawThicknessCubes;
        public bool drawThicknessEndHits;
        public bool drawThicknessStartHits;
#endif

#if !TPC_DEBUG
        [FormerlySerializedAs("playerCollision")] [HideInInspector]
#endif
        public bool PlayerCollision;
#if !TPC_DEBUG
        [FormerlySerializedAs("cameraNormalMode")] [HideInInspector]
#endif
        public bool CameraNormalMode;
#if !TPC_DEBUG
        [FormerlySerializedAs("bGroundHit")] [HideInInspector]
#endif
        public bool GroundHit;

        private CameraControllerState state;
        private bool initDone;

        private SkinnedMeshRenderer[] smrs;

        private SortDistance sortMethodHitDistance;

        private Vector3 targetPosition;
        private Vector3 smoothedTargetPosition;

        private RaycastHit[] startHits;
        private RaycastHit[] endHits;
        private RaycastHit[] occlusionHits;
        private RaycastHit[] boxCastHits;
        private List<RaycastHit> occlusionHitsCollector;
        private List<RaycastHit> startHitsCollector;
        private List<RaycastHit> endHitsCollector;
        private List<RaycastHit> boxCastHitsCollector;

#if TPC_DEBUG
        public bool increaseX = false;
        private bool internalIncreaseX = false;
        public Vector3 increaseVector;
        public Vector3 savedVector;
#endif

        private void Awake()
        {
            initDone = false;

            state = new CameraControllerState();

            startHits = new RaycastHit[MaxThicknessRaycastHitCount];
            endHits = new RaycastHit[MaxThicknessRaycastHitCount];
            occlusionHits = new RaycastHit[MaxRaycastHitCount];
            boxCastHits = new RaycastHit[MaxRaycastHitCount];

            occlusionHitsCollector = new List<RaycastHit>(MaxRaycastHitCount);
            startHitsCollector = new List<RaycastHit>(MaxRaycastHitCount);
            endHitsCollector = new List<RaycastHit>(MaxRaycastHitCount);
            boxCastHitsCollector = new List<RaycastHit>(MaxRaycastHitCount);

            state.CameraRotation = transform.rotation;
            state.SmartPivotRotation = Quaternion.identity;
            state.PivotRotation = Quaternion.identity;
            state.CurrentPivotRotation = Quaternion.identity;

            sortMethodHitDistance = new SortDistance();

            state.Distance = DesiredDistance;

            CameraNormalMode = true;
            PlayerCollision = false;

            UpdateOffsetVector(OffsetVector);
            UpdateCameraOffsetVector(CameraOffsetVector);

            InitFromTarget();
        }

        private void InitFromTarget()
        {
            InitFromTarget(Target);
        }

        /// <summary>
        /// Use this to re-init a target, for example, when switching through multiple characters
        /// </summary>
        /// <param name="newTarget"></param>
        public void InitFromTarget(Transform newTarget)
        {
            Target = newTarget;

            if (Target == null)
                return;

            var tmpTransform = transform;
            var tmpTargetPosition = Target.transform.position;

            state.CameraRotation = tmpTransform.rotation;
            state.PrevPosition = tmpTransform.position;
            targetPosition = tmpTargetPosition;
            smoothedTargetPosition = tmpTargetPosition;

            smrs = HideSkinnedMeshRenderers ? Target.GetComponentsInChildren<SkinnedMeshRenderer>() : null;

            // get colliders from target
            var colliders = Target.GetComponentsInChildren<Collider>();

            foreach (var col in colliders)
            {
                if (!PlayerLayer.IsInLayerMask(col.gameObject))
                {
                    var colliderName = col.gameObject.name;
                    Debug.LogWarning(
                        $"The target \"{colliderName}\" has a collider which is not in the player layer. To fix: Change the layer of {colliderName} to the layer referenced in CameraController->Player layer");
                }
            }

            var selfCollider = GetComponent<Collider>();

            if (selfCollider == null && HideSkinnedMeshRenderers)
            {
                Debug.LogWarning("HideSkinnedMeshRenderers is activated but there's no collider on the camera. Example to fix: Atach a BoxCollider to the camera and set it to trigger.");
            }

            if (selfCollider && CollisionLayer.IsInLayerMask(gameObject))
            {
                Debug.LogWarning(
                    "Camera is colliding with the collision layer. Consider changing the camera to a layer that isn't in the collision layer. Example to fix: Change the camera to the player layer.");
            }

            initDone = true;
        }

        private void LateUpdate()
        {
#if TPC_DEBUG
            ResetCubes();
#endif

            if (Target == null)
                return;

            if (!initDone)
                return;

            if (state.Distance < 0)
                state.Distance = 0;

            // disable player character when too close
            if (HideSkinnedMeshRenderers && smrs != null)
            {
                if (PlayerCollision || state.Distance <= CollisionDistance)
                {
                    foreach (var t in smrs)
                    {
                        t.enabled = false;
                    }
                }
                else
                {
                    foreach (var t in smrs)
                    {
                        t.enabled = true;
                    }
                }
            }

            Quaternion transformRotation = state.CameraRotation;

            Vector3 offsetVectorTransformed = Target.rotation * OffsetVector;
            Vector3 cameraOffsetVectorTransformed = transformRotation * state.CurrentCameraOffsetVector;
            Vector3 cameraOffsetVectorTransformedStable = transformRotation * (state.InvertCameraOffsetVector ? -CameraOffsetVector : CameraOffsetVector);

            if (SmoothTargetMode)
            {
                if (SmoothTargetValue.x != 0)
                    smoothedTargetPosition.x = Mathf.Lerp(smoothedTargetPosition.x, Target.position.x, SmoothTargetValue.x * Time.deltaTime);
                if (SmoothTargetValue.y != 0)
                    smoothedTargetPosition.y = Mathf.Lerp(smoothedTargetPosition.y, Target.position.y, SmoothTargetValue.y * Time.deltaTime);
                if (SmoothTargetValue.z != 0)
                    smoothedTargetPosition.z = Mathf.Lerp(smoothedTargetPosition.z, Target.position.z, SmoothTargetValue.z * Time.deltaTime);

                targetPosition = smoothedTargetPosition;
            }
            else
            {
                targetPosition = Target.position;
            }

            Vector3 targetPosWithOffset = (targetPosition + offsetVectorTransformed);
            Vector3 targetPosWithCameraOffset = targetPosWithOffset + cameraOffsetVectorTransformed;
            state.DesiredPosition = transformRotation * (new Vector3(0, 0, -DesiredDistance) + state.CurrentCameraOffsetVector) + offsetVectorTransformed + targetPosition;

            state.CameraOffsetMagnitude = CameraOffsetVector.magnitude;

            if (state.CameraOffsetMagnitude < 0.001f)
            {
                CameraOffsetVector = Vector3.zero;
                state.CameraOffsetMagnitude = 0;
            }

            var dirToTargeSweep2 = (state.DesiredPosition - targetPosWithCameraOffset).normalized;
            state.DirToTarget = dirToTargeSweep2;

#if TPC_DEBUG
            if (showDesiredPosition)
                ShowThicknessCube(state.DesiredPosition, "desiredPosition");
#endif
            // sweep from target to desiredPosition and check for collisions
            {
                int hitCount = Physics.SphereCastNonAlloc(targetPosWithCameraOffset, CollisionDistance, dirToTargeSweep2, occlusionHits, DesiredDistance, CollisionLayer);


                SortAndFilterHits(occlusionHitsCollector, occlusionHits, hitCount, dirToTargeSweep2);
            }

            float thickness = float.MaxValue;

            state.OcclusionHitHappened = OcclusionCheck && occlusionHitsCollector.Count > 0;

            if (state.OcclusionHitHappened)
            {
                if (Physics.Raycast(targetPosWithCameraOffset, state.DirToTarget, out var closesHit, state.Distance + CollisionDistance, CollisionLayer))
                {
                    occlusionHitsCollector.Insert(0, closesHit);
                }

                if (ThicknessCheck)
                {
                    thickness = GetThicknessFromCollider(targetPosWithCameraOffset, state.DesiredPosition);
                }
            }

            // evaluate ground hit
            if ((SmartPivot && state.OcclusionHitHappened) || state.GroundHit)
            {
                if (Physics.CheckSphere(state.PrevPosition, CollisionDistance * 1.5f, CollisionLayer))
                {
                    if (SmartPivotOnlyGround)
                    {
#if TPC_DEBUG
                        Debug.DrawLine(state.PrevPosition, state.PrevPosition - (target.up * collisionDistance * 2.0f), Color.red);
#endif

                        if (Physics.Raycast(state.PrevPosition, -Target.up, out var groundHit, CollisionDistance * 2.0f, CollisionLayer))
                        {
                            state.GroundHit = Vector3.Dot(FloorNormalUp, groundHit.normal) > MaxFloorDot;
                        }
                        else
                            state.GroundHit = false;
                    }
                    else
                        state.GroundHit = true;
                }
                else
                    state.GroundHit = false;
            }
            else if (!state.OcclusionHitHappened && state.CameraNormalMode)
                state.GroundHit = false;

            // Avoid that the character is not visible
            if (OcclusionCheck && state.OcclusionHitHappened)
            {
                Vector3 vecA = occlusionHitsCollector[0].point - targetPosWithCameraOffset;
                float vecAMagnitude = vecA.magnitude;
                float newDist = vecAMagnitude - CollisionDistance;

                if (thickness > MaxThickness)
                {
                    if (newDist > DesiredDistance)
                        newDist = DesiredDistance;

                    state.Distance = newDist;
                }
                else if (state.CameraNormalMode)
                {
                    LerpDistance();
                }
            }
            else if (!state.OcclusionHitHappened)
            {
                LerpDistance();
            }

            // camera offset path
            // this was previously running as first but it's important to have an up-to-date distance
            // so this was moved here

            var currentOffsetLength = state.CurrentCameraOffsetVector.magnitude;

            if (state.CameraOffsetMagnitude > 0 || currentOffsetLength > 0)
            {
                var targetPosWithCameraOffsetStable = targetPosition + offsetVectorTransformed + cameraOffsetVectorTransformedStable;
                var castVectorNormalized = (targetPosWithCameraOffsetStable - targetPosWithOffset).normalized;

                var maxLength = GetBoxCastMaxDistance(targetPosWithOffset, castVectorNormalized, transformRotation
                );

                maxLength = Math.Max(maxLength, CameraOffsetMinDistance);

                if (state.CameraOffsetMagnitude > 0)
                {
                    if (maxLength <= CameraOffsetMinDistance)
                    {
                        // camera offset is too close to a wall, test if other side has better results

                        cameraOffsetVectorTransformedStable = transformRotation * (!state.InvertCameraOffsetVector ? -CameraOffsetVector : CameraOffsetVector);
                        targetPosWithCameraOffsetStable = targetPosition + offsetVectorTransformed + cameraOffsetVectorTransformedStable;
                        castVectorNormalized = (targetPosWithCameraOffsetStable - targetPosWithOffset).normalized;

                        var maxLength2 = GetBoxCastMaxDistance(targetPosWithOffset, castVectorNormalized, transformRotation);
                        maxLength2 = Math.Max(maxLength2, CameraOffsetMinDistance);

                        if (maxLength2 > CameraOffsetMinDistance)
                        {
                            // other side has a bigger offset, so switch sides
                            state.InvertCameraOffsetVector = !state.InvertCameraOffsetVector;
                            state.CurrentCameraOffsetVector = Vector3.Lerp(state.CurrentCameraOffsetVector, state.InvertCameraOffsetVector ? -CameraOffsetVector : CameraOffsetVector,
                                Time.deltaTime * CameraOffsetChangeSpeed);
                        }
                    }
                    else
                    {
                        var largestAllowedOffsetPoint = (state.InvertCameraOffsetVector ? -CameraOffsetVector : CameraOffsetVector).normalized * maxLength;

                        if (currentOffsetLength > maxLength)
                        {
                            // don't slerp when the offset decreases
                            state.CurrentCameraOffsetVector = largestAllowedOffsetPoint;
                        }
                        else
                        {
                            // only slerp when the offset increases
                            state.CurrentCameraOffsetVector = Vector3.Lerp(state.CurrentCameraOffsetVector, largestAllowedOffsetPoint, Time.deltaTime * CameraOffsetChangeSpeed);
                        }
                    }
                }
                else
                {
                    state.CurrentCameraOffsetVector = Vector3.Lerp(state.CurrentCameraOffsetVector, Vector3.zero, Time.deltaTime * CameraOffsetChangeSpeed);
                }
            }

            LerpPresentationDistance();

            if (!state.CameraNormalMode)
            {
                transformRotation *= state.SmartPivotRotation;
            }

            if (PivotRotationEnabled)
            {
                var newPivot = !CustomPivot ? state.PivotRotation : Quaternion.Euler(PivotAngles);
                state.CurrentPivotRotation = SmoothPivot ? Quaternion.Slerp(state.CurrentPivotRotation, newPivot, Time.deltaTime) : newPivot;

                transformRotation *= state.CurrentPivotRotation;
            }

            transformRotation = StabilizeRotation(transformRotation);
            transform.rotation = transformRotation;

            var newPosition = GetCameraPosition(state, targetPosition, offsetVectorTransformed);
            transform.position = newPosition;
            state.PrevPosition = newPosition;

            // CAMERA SHAKE
            if (shakeTimer > 0)
            {
                shakeOffset = UnityEngine.Random.insideUnitSphere * shakeAmount;
                shakeTimer -= Time.deltaTime;
            }
            else
            {
                shakeOffset = Vector3.zero;
            }

            transform.position += shakeOffset;
        }

        private void LerpDistance()
        {
            float lerpFactor = Time.deltaTime * ((state.Distance < DesiredDistance) ? OccludedZoomInSpeed : UnoccludedZoomOutSpeed);
            state.Distance = Mathf.Lerp(state.Distance, DesiredDistance, lerpFactor);
        }

        private void LerpPresentationDistance()
        {
            float lerpFactor = Time.deltaTime * ((state.Distance < state.PresentationDistance) ? OccludedZoomInSpeed : UnoccludedZoomOutSpeed);
            state.PresentationDistance = Mathf.Lerp(state.PresentationDistance, state.Distance, lerpFactor);
        }

        private void SortAndFilterHits(List<RaycastHit> collector, RaycastHit[] raycastHits, int hitCount, Vector3 targetDir)
        {
            Array.Sort(raycastHits, 0, hitCount, sortMethodHitDistance);

            collector.Clear();

            for (int i = 0; i < hitCount && i < hitCount; i++)
            {
                var hit = raycastHits[i];

                if (IsValidRayCastHit(hit, targetDir))
                {
                    //Debug.Log("Added hit point: " + hit.point + " normal: " + hit.normal);
                    collector.Add(hit);
                }
            }
        }

        private static bool IsValidRayCastHit(RaycastHit hit, Vector3 targetDir)
        {
            return (hit.point != Vector3.zero || hit.normal != -targetDir) &&
                   (hit.point != Vector3.zero && hit.normal != Vector3.zero) &&
                   Vector3.Dot(hit.normal, targetDir) < 0.0f;
        }

        public static Vector3 GetCameraPosition(in CameraControllerState state, in Vector3 targetPosition, in Vector3 preComputedOffsetVectorTransformed)
        {
            return state.CameraRotation * (new Vector3(0, 0, -state.PresentationDistance) + state.CurrentCameraOffsetVector) + preComputedOffsetVectorTransformed + targetPosition;
        }

        public static Vector3 GetCameraPosition(in CameraControllerState state, in Vector3 targetPosition, in Quaternion targetRotation, in Vector3 offsetVector)
        {
            Vector3 offsetVectorTransformed = targetRotation * offsetVector;
            return state.CameraRotation * (new Vector3(0, 0, -state.PresentationDistance) + state.CurrentCameraOffsetVector) + offsetVectorTransformed + targetPosition;
        }

        public void OnTriggerEnter(Collider c)
        {
            if (c.transform == Target || PlayerLayer.IsInLayerMask(c.gameObject))
            {
                PlayerCollision = true;
            }
        }

        public void OnTriggerExit(Collider c)
        {
            if (c.transform == Target || PlayerLayer.IsInLayerMask(c.gameObject))
            {
                PlayerCollision = false;
            }
        }

#if TPC_DEBUG
        public Queue<GameObject> debugThicknessCubes = new Queue<GameObject>();
        public Queue<GameObject> activeDebugThicknessCubes = new Queue<GameObject>();

        public void ShowThicknessCube(Vector3 position, string name)
        {
            GameObject cube = null;

            if (debugThicknessCubes.Count > 0)
            {
                cube = debugThicknessCubes.Dequeue();
            }
            else
            {
                cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                Destroy(cube.GetComponent<BoxCollider>());
            }

            cube.SetActive(true);
            cube.transform.position = position;
            cube.name = name;
            activeDebugThicknessCubes.Enqueue(cube);
        }

        public void ResetCubes()
        {
            while (activeDebugThicknessCubes.Count > 0)
            {
                GameObject cube = activeDebugThicknessCubes.Dequeue();

                cube.name = "unused";
                cube.SetActive(false);

                debugThicknessCubes.Enqueue(cube);
            }
        }
#endif

        public class ThicknessCheckList
        {
            public int ColliderId;

            public bool Marked;

            //public Collider collider;
            public Vector3 Point;
            public int LinkedIndex;
        }

        private float GetThicknessFromCollider(Vector3 start, Vector3 end)
        {
            Vector3 dir = (end - start);
            float length = dir.magnitude;
            Vector3 dirToHit = dir.normalized;

            int startHitCount = Physics.SphereCastNonAlloc(start, CollisionDistance, dirToHit, startHits, length, CollisionLayer);
            SortAndFilterHits(startHitsCollector, startHits, startHitCount, dirToHit);

            var endHitCount = Physics.SphereCastNonAlloc(end, CollisionDistance, -dirToHit, endHits, length, CollisionLayer);
            SortAndFilterHits(endHitsCollector, endHits, endHitCount, -dirToHit);

            if (startHitsCollector.Count == 0 || endHitsCollector.Count == 0)
                return float.MaxValue;

            ThicknessCheckList[] startHitList = new ThicknessCheckList[startHitsCollector.Count];
            ThicknessCheckList[] endHitList = new ThicknessCheckList[endHitsCollector.Count];
            Dictionary<int, float> uniqueColliders = new Dictionary<int, float>(0);

            for (int i = 0; i < startHitsCollector.Count; i++)
            {
                var tmp = startHitsCollector[i];
                var colliderId = tmp.collider.GetHashCode();

#if TPC_DEBUG
                if (drawThicknessStartHits)
                    Debug.DrawLine(tmp.point, tmp.point + tmp.normal, Color.red);
#endif

                startHitList[i] = new ThicknessCheckList()
                {
                    ColliderId = colliderId,
                    Point = tmp.point,
                    LinkedIndex = -1,
                    Marked = false
                };

                if (!uniqueColliders.ContainsKey(colliderId))
                    uniqueColliders.Add(colliderId, 0);
            }

            for (int i = 0; i < endHitsCollector.Count; i++)
            {
                var tmp = endHitsCollector[i];
                var colliderId = tmp.collider.GetHashCode();
#if TPC_DEBUG
                if (drawThicknessEndHits)
                    Debug.DrawLine(tmp.point, tmp.point + tmp.normal, Color.green);
#endif

                endHitList[i] = new ThicknessCheckList()
                {
                    ColliderId = colliderId,
                    Point = tmp.point,
                    LinkedIndex = -1,
                    Marked = false
                };

                if (!uniqueColliders.ContainsKey(colliderId))
                    uniqueColliders.Add(colliderId, 0);
            }

            var keyArray = uniqueColliders.Keys.ToArray();
            float finalThickness = float.MaxValue;
            bool finalThicknessFound = false;

            foreach (var colliderId in keyArray)
            {
                for (int i = 0; i < startHitList.Length; i++)
                {
                    var tmpStart = startHitList[i];

                    if (tmpStart.ColliderId == 0)
                        continue;

                    if (tmpStart.Marked || tmpStart.ColliderId != colliderId)
                        continue;

                    for (int ii = endHitList.Length - 1; ii >= 0; ii--)
                    {
                        var tmpEnd = endHitList[ii];

                        if (tmpEnd.ColliderId == 0)
                            continue;

                        if (tmpEnd.Marked || tmpEnd.ColliderId != colliderId)
                            continue;

                        tmpStart.Marked = true;
                        tmpEnd.Marked = true;
                        tmpStart.LinkedIndex = ii;
                        tmpEnd.LinkedIndex = i;
                    }
                }

                for (int i = 0; i < startHitList.Length; i++)
                {
                    var tmpStart = startHitList[i];

                    if (tmpStart.ColliderId == 0)
                        continue;

                    if (!tmpStart.Marked || tmpStart.ColliderId != colliderId || tmpStart.LinkedIndex == -1)
                        continue;

                    var tmpEnd = endHitList[tmpStart.LinkedIndex];

                    if (!finalThicknessFound)
                    {
                        finalThicknessFound = true;
                        finalThickness = 0;
                    }

#if TPC_DEBUG
                    if (drawThicknessCubes)
                    {
                        Debug.DrawLine(tmpStart.Point, tmpEnd.Point, Color.blue);
                    }
#endif

                    float thicknessFraction = (tmpStart.Point - tmpEnd.Point).magnitude;

                    var currentEntityThickness = uniqueColliders[colliderId];
                    currentEntityThickness += thicknessFraction;

                    if (currentEntityThickness > finalThickness)
                        finalThickness = currentEntityThickness;

                    uniqueColliders[colliderId] = currentEntityThickness;
                }
            }

            return Math.Min(finalThickness, float.MaxValue);
        }

        private static Vector3 GetBoxExtents(float length, float collisionDistance)
        {
            Vector3 cameraOffsetVirtualZeroPoint = new Vector3
            {
                x = 0.001f,
                y = collisionDistance,
                z = Mathf.Abs(length)
            };

            return cameraOffsetVirtualZeroPoint;
        }

        private float GetBoxCastMaxDistance(
            Vector3 origin,
            Vector3 castDirection,
            Quaternion transformRotation)
        {
            var boxLength = state.CameraOffsetMagnitude;

            var originTranslated = origin
                                   + (-castDirection * (CollisionDistance)) // move slight away from the actual 0 point
                                   - transformRotation * new Vector3(0, 0, state.Distance * 0.5f); // move by half the distance for the box cast to be aligned

            float boxCastDistance = CameraOffsetVector.magnitude + CollisionDistance * 2;
            var extents = GetBoxExtents((state.Distance * 0.5f), CollisionDistance);

            int hitCount = Physics.BoxCastNonAlloc(originTranslated, extents, castDirection, boxCastHits, transformRotation, boxCastDistance, CollisionLayer);
            SortAndFilterHits(boxCastHitsCollector, boxCastHits, hitCount, castDirection);

            float maxLength = boxLength;

            if (boxCastHitsCollector.Count <= 0)
                return maxLength;

            for (int i = 0; i < boxCastHitsCollector.Count; i++)
            {
                var boxCastHit = boxCastHitsCollector[i];

                Plane p = new Plane(castDirection, origin);
                Vector3 hc = p.ClosestPointOnPlane(boxCastHit.point);

                var mag = (boxCastHit.point - hc).magnitude;
                if (mag < maxLength)
                    maxLength = mag;
            }

            return maxLength - CollisionDistance;
        }

        /// <summary>
        /// Use this method to update the offset vector during runtime.
        /// In case the offset vector is inside the collision sphere
        /// the offset vector gets moved to a position outside of it
        /// </summary>
        /// <param name="newOffset"></param>
        public void UpdateOffsetVector(Vector3 newOffset)
        {
            if (newOffset.sqrMagnitude > CollisionDistance)
                OffsetVector = newOffset;
            else
            {
                // when the offset vector ends up inside the collision sphere, add a small vector plus the collision distance to fix collision glitches
                OffsetVector += Vector3.up * (CollisionDistance + 0.01f);
            }
        }

        /// <summary>
        /// Use this method to update the camera offset vector during runtime.
        /// Updating it like that removes the need to calculate the sqrMagnitude of the vector
        /// every frame.
        /// </summary>
        /// <param name="newOffset"></param>
        public void UpdateCameraOffsetVector(Vector3 newOffset)
        {
            CameraOffsetVector = newOffset;
        }

        // Slerping to new rotation sometimes leads to wrong rotation in Z-axis        
        // so any Z rotation will be removed
        public static Quaternion StabilizeRotation(Quaternion rotation)
        {
            return Quaternion.Euler(rotation.eulerAngles.x, rotation.eulerAngles.y, 0);
        }

        public static Quaternion StabilizeSlerpRotation(Quaternion from, Quaternion to, float timeModifier)
        {
            var slerpedRotation = Quaternion.Slerp(from, to, timeModifier);
            return Quaternion.Euler(slerpedRotation.eulerAngles.x, slerpedRotation.eulerAngles.y, 0);
        }

        public ref CameraControllerState GetControllerState()
        {
            return ref state;
        }

        public Vector3 GetSmoothedTargetPosition()
        {
            return smoothedTargetPosition;
        }

        // CAMERA SHAKE
        public void ShakeCamera(float amount, float duration)
        {
            shakeAmount = amount;
            shakeDuration = duration;
            shakeTimer = duration;
        }
    }
}
using System.Collections.Generic;
using UnityEngine;

namespace LOS
{
    [AddComponentMenu("Line of Sight/LOS Culler")]
    public class LOSCuller : MonoBehaviour
    {
        #region Exposed Data Members

        [Tooltip("Selects which layers block raycasts used for visibility calculations")]
        [SerializeField]
        private LayerMask m_RaycastLayerMask = -1;


        [Tooltip("Select a Layer this Object belongs to. Determines whether an LOSSource will reveal it.")]
        [SerializeField]
        private LayerMask m_RevealMask = -1;
        #endregion Exposed Data Members

        #region Private Data Members

        private bool m_IsVisible = true;

        #endregion Private Data Members

        #region Public Properties

        public LayerMask RaycastLayerMask
        {
            get { return m_RaycastLayerMask; }
            set { m_RaycastLayerMask = value; }
        }

        public LayerMask RevealMask
        {
            get { return m_RevealMask; }
            set { m_RevealMask = value; }
        }

        public bool Visibile
        {
            get { return m_IsVisible; }
        }

        #endregion Public Properties

        #region MonoBehaviour Functions

        private void OnEnable()
        {
            enabled &= Assert.Verify(GetComponent<Renderer>() != null, "No renderer attached to this GameObject! LOS Culler component must be added to a GameObject containing a MeshRenderer or Skinned Mesh Renderer!");
        }

        private void Update()
        {
            if (gameObject.GetComponent<Renderer>() != null)
                m_IsVisible = CustomCull(gameObject.GetComponent<Renderer>().bounds, m_RaycastLayerMask.value, m_RevealMask);
        }

        #endregion MonoBehaviour Functions

        #region Private Functions

        /// <summary>
        /// Checks to see if object is inside the view frustum of any of the LOS cameras.
        /// Ideally should be called in OnWillRenderObject, but it's to late to disable renderer..
        /// Early outs when visible to one camera.
        /// </summary>
        private static bool CustomCull(Bounds meshBounds, int layerMask, LayerMask revealLayers)
        {
            //Get list of cube sources
            List<LOSSourceCube> losCubeSources = LOSManager.Instance.LOSSourcesCube;

            for (int i = 0; i < losCubeSources.Count; ++i)
            {
                LOSSourceCube losSourceCube = losCubeSources[i];
                Camera currentCamera = losSourceCube.SourceCamera;

                if (losSourceCube.IsVisible && currentCamera != null)
                {
                    float squaredDistance = currentCamera.farClipPlane * currentCamera.farClipPlane;
                    if (meshBounds.SqrDistance(losSourceCube.transform.position) <= squaredDistance)
                    {
                        if (CheckRayCast(currentCamera, meshBounds, layerMask))
                            return true;
                    }
                }
            }

            // Get list of sources.
            List<LOSSource> losSources = LOSManager.Instance.LOSSources;

            for (int i = 0; i < losSources.Count; ++i)
            {
                LOSSource losSource = losSources[i];
                Camera currentCamera = losSource.SourceCamera;

                if (losSource.IsVisible && currentCamera != null)
                {
                    Plane[] cameraPlanes = losSource.FrustumPlanes;

                    if (GeometryUtility.TestPlanesAABB(cameraPlanes, meshBounds))
                    {
                        //The Line of Sight Reveal Mask is or contains this cullers layer. Then Reveal it.
                        if (CheckRayCast(currentCamera, meshBounds, layerMask) && (losSource.RevealMask== (losSource.RevealMask | revealLayers)))
                            return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Checks to see if the view from the camera to the object is not obstructed by other objects with colliders.
        /// Early outs when one ray connects.
        /// </summary>
        private static bool CheckRayCast(Camera currentCamera, Bounds meshBounds, int layerMask)
        {
            bool rayConnect = false;
            Vector3 cameraPosition = currentCamera.transform.position;

            //Written like this to avoid memory allocation
            if (CheckRayConnect(meshBounds.center, cameraPosition, layerMask) || CheckRayConnect(meshBounds.max, cameraPosition, layerMask) || CheckRayConnect(meshBounds.min, cameraPosition, layerMask))
                rayConnect = true;

            return rayConnect;
        }

        /// <summary>
        /// Checks if ray is blocked or connects from one point to the other.
        /// </summary>
        private static bool CheckRayConnect(Vector3 origin, Vector3 cameraPosition, int layerMask)
        {
            const float RAY_THRESHOLD = 0.1f;
            const float MIN_RAY_LENGTH = 0.00001f;

            bool rayConnect = false;

            Vector3 rayDirection = origin - cameraPosition;
            Ray visibleRay = new Ray(cameraPosition, rayDirection);

            float rayLength = rayDirection.magnitude - RAY_THRESHOLD;
            rayLength = Mathf.Max(MIN_RAY_LENGTH, rayLength);

            if (!Physics.Raycast(visibleRay, rayLength, layerMask))
                rayConnect = true;

            return rayConnect;
        }

        #endregion Private Functions
    }
}
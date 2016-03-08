using System.Collections;
using UnityEngine;

namespace LOS
{
    /// <summary>
    /// Disables a gameobjects renderer if the object is outside the line of sight
    /// </summary>
    [RequireComponent(typeof(LOS.LOSCuller))]
    [AddComponentMenu("Line of Sight/LOS Object Hider")]
    public class LOSObjectHider : MonoBehaviour
    {
        private LOSCuller m_Culler;

        private void Awake()
        {
            m_Culler = GetComponent<LOSCuller>();
        }

        private void OnEnable()
        {
            enabled &= Assert.Verify(m_Culler != null, "LOS culler component missing.");
            enabled &= Assert.Verify(GetComponent<Renderer>() != null, "No renderer attached to this GameObject! LOS Culler component must be added to a GameObject containing a MeshRenderer or Skinned Mesh Renderer!");
        }

        private void LateUpdate()
        {
            GetComponent<Renderer>().enabled = m_Culler.Visibile;
        }
    }
}
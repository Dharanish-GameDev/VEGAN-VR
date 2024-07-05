using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace VeganVR.Player.Local
{
    public class XR_RigRef : MonoBehaviour
    {
        public static XR_RigRef instance { get; private set; }

        #region Private Variables

        [SerializeField] private Transform rootTransform;
        [SerializeField] private Transform headTransform;
        [SerializeField] private Transform leftHandTransform;
        [SerializeField] private Transform rightHandTransform;

        [SerializeField] private XRDirectInteractor rightHandDirectInteractor;
        [SerializeField] private XRDirectInteractor leftHandDirectInteractor;

        [SerializeField] private XRRayInteractor rightRayInteractor;
        [SerializeField] private XRRayInteractor leftRayInteractor;

        [SerializeField] private List<Renderer> handRenderers;

        #endregion

        #region Properties

        public Transform RootTransform => rootTransform;
        public Transform HeadTransform => headTransform;
        public Transform LeftHandTransform => leftHandTransform;
        public Transform RightHandTransform => rightHandTransform;
        public XRRayInteractor RightRayInteractor => rightRayInteractor;
        public XRRayInteractor LeftRayInteractor => leftRayInteractor;
        public XRDirectInteractor RightDirectInteractor => rightHandDirectInteractor;
        public XRDirectInteractor LeftDirectInteractor => leftHandDirectInteractor;
        #endregion

        #region LifeCycle Methods

        private void Awake()
        {
            instance = this;
        }
        private void Start()
        {

        }
        private void Update()
        {

        }

        #endregion

        #region Private Methods


        #endregion

        #region Public Methods

        public void ChangeRootPos(Vector3 pos, Quaternion rot)
        {
            rootTransform.position = pos;
            rootTransform.rotation = rot;
        }

        public void ChangeHandsColorLocally(Color color)
        {
            foreach (Renderer renderer in handRenderers)
            {
                renderer.material.color = color;
            }
        }

        #endregion
    }
}


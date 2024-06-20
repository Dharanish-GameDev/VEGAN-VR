using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VeganVR.VR_Hands
{
    public class HandData : MonoBehaviour
    {
        public enum HandModelType { LeftHand, RightHand }

        [SerializeField] private HandModelType handModelType;
        [SerializeField] private Animator animator;
        [SerializeField] private Transform root;
        [SerializeField] private Transform[] fingerBonesTransformArray;

        #region Private Variables

        public Transform[] FingerBonesTransformArray => fingerBonesTransformArray;
        public Transform Root => root;

        #endregion

        #region Properties

        public Animator Animator => animator;
        public HandModelType handType => handModelType;

        #endregion

        #region LifeCycle Methods

        private void Awake()
        {

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


        #endregion
    }

}

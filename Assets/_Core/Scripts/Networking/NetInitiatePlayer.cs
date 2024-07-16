using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using VeganVR.Player.Local;

namespace VeganVR.Player.Network
{
    public class NetInitiatePlayer : NetworkBehaviour
    {
        #region Private Variables

        [SerializeField] private List<Renderer> disableMeshList = new();

        [Header("Note")]
        [Tooltip("Its not the parent object,its the canvas in that UI Parent")]
        [SerializeField] private Canvas playerUiCanvas;


        #endregion

        #region Properties


        #endregion

        #region LifeCycle Methods

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            ChangePlayerColorOverNet();
            InitiatePlayer();
            GetComponent<PlayerDetailsUI>().ChangePlayerName();
        }

        

        #endregion

        #region Private Methods

        private void InitiatePlayer()
        {
            if (!IsOwner) return;
            DisableMesh();
            ChangeInitialPlayerPos();
            XR_RigRef.instance.ChangeHandsColorLocally(NetworkHelper.Instance.PlayerColorList[(int)OwnerClientId]);
        }

        private void ChangePlayerColorOverNet()
        {
            foreach (Renderer mesh in disableMeshList)
            {
                mesh.material.color = NetworkHelper.Instance.PlayerColorList[(int)OwnerClientId];
            }
        }

        

        private void DisableMesh()
        {
            foreach (Renderer mesh in disableMeshList)
            {
                mesh.enabled = false;
            }
            playerUiCanvas.enabled = false;
        }

        private void ChangeInitialPlayerPos()
        {
            XR_RigRef.instance.ChangeRootPos(NetworkHelper.Instance.SpawnPointList[(int)OwnerClientId]);
            GameflowManager.Instance.ChangeIsFirstSpawnNearSideSelectionUiCompletedToTrue();
        }

        #endregion

        #region Public Methods


        #endregion
    }
}


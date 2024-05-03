using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode.Components;

public class ClientNetworkTransform : NetworkTransform
{

    #region Public Methods

    protected override bool OnIsServerAuthoritative()
    {
        return false;
    }

    #endregion
}

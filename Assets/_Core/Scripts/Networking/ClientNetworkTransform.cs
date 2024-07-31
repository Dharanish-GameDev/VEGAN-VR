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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Slicable : NetworkBehaviour
{
	#region Private Variables


	#endregion

	#region Properties



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
    public void Destroyself()
    {
        Destroy(gameObject);
    }

    #endregion
}

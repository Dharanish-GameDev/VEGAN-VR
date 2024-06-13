using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkHelper : MonoBehaviour
{
	public static NetworkHelper instance { get; private set; }

	#region Private Variables

	[SerializeField] private List<Transform> spawnPointList;
	[SerializeField] private List<Color> playerColorList;

	#endregion

	#region Properties

	public List <Transform> SpawnPointList => spawnPointList;
	public List<Color> PlayerColorList => playerColorList;

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

	#endregion
}

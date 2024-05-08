using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkHelper : MonoBehaviour
{
	public static NetworkHelper instance { get; private set; }

	#region Private Variables

	[SerializeField] private List<Transform> spawnPointList;

	#endregion

	#region Properties

	public List <Transform> SpawnPointList => spawnPointList;

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

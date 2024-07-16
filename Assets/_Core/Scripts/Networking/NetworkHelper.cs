using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using VeganVR.UI;

public class NetworkHelper : MonoBehaviour
{
	public static NetworkHelper Instance { get; private set; }

	#region Private Variables

	[SerializeField] private List<Transform> spawnPointList;
    [SerializeField] private List<Color> playerColorList;

	[Header("Arena SpawnPoints")]
	[Space(10)]
    [SerializeField] private Transform attackingPoint;
	[SerializeField] private Transform defendingPoint;


	#endregion

	#region Properties

	public List <Transform> SpawnPointList => spawnPointList;
	public Transform AttackingPoint => attackingPoint;
	public Transform DefendingPoint => defendingPoint;
	public List<Color> PlayerColorList => playerColorList;

	#endregion

	#region LifeCycle Methods

	private void Awake()
	{
		Instance = this;
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

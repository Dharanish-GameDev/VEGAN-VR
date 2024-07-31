using System.Collections.Generic;
using UnityEngine;

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

	[SerializeField] private List<ToggleRayInteractor> ToggleRayinteractorList;


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

	public void SetCanActivateRayBoolean(bool value)
	{
		foreach (var toggleRay in ToggleRayinteractorList)
		{
			toggleRay.SetCanActivateRayBoolean(value);
		}
	}

	


	#endregion
}

using UnityEngine;

public class SwordEffect : MonoBehaviour
{
	#region Private Variables

	[SerializeField] private ParticleSystem swordCuttingEffect;

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

	public void TriggerSwordCuttingEffect(Vector3 pos,Quaternion rot)
	{
		swordCuttingEffect.transform.position = pos;
		swordCuttingEffect.transform.rotation = rot;
		swordCuttingEffect.Play();
	}
    #endregion
}

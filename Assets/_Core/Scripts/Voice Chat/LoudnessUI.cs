using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VeganVR.VoiceChat;
public class LoudnessUI : MonoBehaviour
{
	#region Private Variables

	[SerializeField] private AudioLoudnessDedection AudioLoudnessDedection;
	[SerializeField] private Image loudnessUi;
	[SerializeField] private TMP_Dropdown micSelectionDropDown;
	private float micLoudness;

	#endregion

	#region Properties



	#endregion

	#region LifeCycle Methods

	private void Awake()
	{

	}
	private void Start()
	{
		micSelectionDropDown.options.Clear();
		List<string> MicNamesList = new List<string>();
        for (int i = 0; i < Microphone.devices.Length ; i++)
        {
			MicNamesList.Add(Microphone.devices[i]);
        }
		micSelectionDropDown.AddOptions(MicNamesList);
		micSelectionDropDown.onValueChanged.AddListener(delegate { DropDownValueChanged(micSelectionDropDown); });
    }
	private void Update()
	{
		micLoudness = AudioLoudnessDedection.GetLoudnessFromMicrophone() * 1.5f;
		if (micLoudness < 0.1) micLoudness = 0;
        loudnessUi.fillAmount = Mathf.LerpUnclamped(0,1, micLoudness);

    }

	
	
	#endregion

	#region Private Methods

	private void DropDownValueChanged(TMP_Dropdown dropdown)
	{
		AudioLoudnessDedection.ChangeMicIndex(dropdown.value);
	}

	#endregion

	#region Public Methods


	#endregion
}

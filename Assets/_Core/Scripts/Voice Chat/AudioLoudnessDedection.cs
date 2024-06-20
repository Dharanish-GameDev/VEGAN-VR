using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VeganVR.VoiceChat
{
    public class AudioLoudnessDedection : MonoBehaviour
    {
        #region Private Variables

        [SerializeField] private int sampleWindow = 64;
        [SerializeField] private AudioClip microphoneClip;
        private int micIndex = 0;

        #endregion

        #region Properties



        #endregion

        #region LifeCycle Methods

        private void Awake()
        {

        }
        private void Start()
        {
            MicrophoneToAudioClip();
        }
        private void Update()
        {

        }

        #endregion

        #region Private Methods

        public void MicrophoneToAudioClip()
        {
            string micName = Microphone.devices[micIndex];
            microphoneClip = Microphone.Start(micName, true, 20, AudioSettings.outputSampleRate);
        }


        public float GetLoudnessFromMicrophone()
        {
            return GetLoudnessFromAudioClip(Microphone.GetPosition(Microphone.devices[micIndex]), microphoneClip);
        }
        public void ChangeMicIndex(int index)
        {
            Microphone.End(Microphone.devices[micIndex]);
            micIndex = index;
            MicrophoneToAudioClip();
        }
        private float GetLoudnessFromAudioClip(int clipPos,AudioClip audioClip)
        {
            int startPos = clipPos - sampleWindow;
            if(startPos < 0) return 0;

            float[] waveData = new float[sampleWindow];
            audioClip.GetData(waveData, startPos);


            // Get Loudness
            float totalLoudness = 0;
            for (int i = 0; i < sampleWindow; i++)
            {
                totalLoudness += Mathf.Abs(waveData[i]);
            }
            return totalLoudness/sampleWindow;
        }

        #endregion

        #region Public Methods



        #endregion
    }
}


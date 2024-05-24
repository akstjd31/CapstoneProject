using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Voice.Unity;

public class VoiceManager : MonoBehaviour
{
    public Recorder recorder;

    private void Start()
    {
        recorder = this.GetComponent<Recorder>();
    }
}

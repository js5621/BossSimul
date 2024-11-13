using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearControl : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] AudioSource ClearAudioSource;
    [SerializeField] AudioClip ClearAudioSound;
    void Start()
    {
      ClearAudioSource.PlayOneShot(ClearAudioSound);
    }

    // Update is called once per frame

}

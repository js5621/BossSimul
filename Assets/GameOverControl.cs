using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverControl : MonoBehaviour
{
    [SerializeField] AudioSource GameOverAudioSource;
    [SerializeField] AudioClip GameOverAudioSound;

    // Start is called before the first frame update
    void Start()
    {
        GameOverAudioSource.PlayOneShot(GameOverAudioSound);
    }

    // Update is called once per frame
    void Update()
    {

    }
}

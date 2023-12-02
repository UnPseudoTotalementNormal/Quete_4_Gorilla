using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    bool played = false;
    void Update()
    {
        if (GetComponent<AudioSource>().isPlaying == false && played)
        {
            Destroy(this.gameObject);
        }
    }

    public void PlayAudio(string Path, float volume = 1)
    {
        this.gameObject.AddComponent<AudioSource>();
        AudioSource source = this.gameObject.GetComponent<AudioSource>();
        source.volume = volume;
        source.clip = Resources.Load<AudioClip>(Path);
        source.Play();
        played = true;
    }
}

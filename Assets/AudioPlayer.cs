using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour {

    public AudioClip Chest;
    public AudioClip Explosion;
    public AudioClip HitPawn;
    public AudioClip WinSound;
    new AudioSource audio;

    // Use this for initialization
    void Start () {
        audio = GetComponent<AudioSource>();
	}
	
    public void ChestOpen()
    {
        audio.clip = Chest;
        audio.Play();
    }

    public void Explode()
    {
        audio.clip = Explosion;
        audio.Play();
    }

    public void Pawn()
    {
        audio.clip = HitPawn;
        audio.Play();
    }

    public void Win()
    {
        audio.clip = WinSound;
        audio.Play();
    }
}

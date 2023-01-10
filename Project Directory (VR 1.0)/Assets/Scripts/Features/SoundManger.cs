using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable] public struct Music
{
    public Phase trackPhase;
    public AudioClip trackClip;
}
public enum Phase
{
    Calm,
    Action
}
public class SoundManger : MonoBehaviour
{
    [TextArea]
    public string Notes = "Tracks must be unique;\nDefault current phase will initiate first;";
    [SerializeField] private Phase currentPhase;
    private AudioSource trackPlayer;
    [SerializeField] private Music[] trackList;

    private void Awake()
    {
        if (this.GetComponent<AudioSource>())
        {
            trackPlayer = this.gameObject.GetComponent<AudioSource>();
        }
        else
        {
            trackPlayer = this.gameObject.AddComponent<AudioSource>();
        }
        trackPlayer.loop = true;
        SetMusic();
    }

    public void ChangePhase(Phase toWhat)
    {
        currentPhase = toWhat;
        SetMusic();
    }

    public void SetMusic()
    {
        foreach (var track in trackList)
        {
            if(track.trackPhase == currentPhase)
            {
                trackPlayer.clip = track.trackClip;
                trackPlayer.Play();
            }
        }
    }
}

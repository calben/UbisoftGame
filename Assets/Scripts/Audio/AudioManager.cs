﻿using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour {
	public AudioClip[] _clips;
	public float _volume = 1.0f;

	private AudioSource _audioSource;
	private bool _isStoppingSFX = false;
	private bool _isPlayingSFX = false;
	private float _fadeTime = 0.0f;

	// Use this for initialization
	void Start () {
	}

	public void Initialize(){
		_audioSource = this.GetComponent<AudioSource>();
		_audioSource.Stop();
	}

	// Update is called once per frame
	void Update () {
		if(_isStoppingSFX){
			_audioSource.volume -= Time.deltaTime * _fadeTime;
			if(_audioSource.volume <= 0.0f){
				_audioSource.volume = 0.0f;
				_audioSource.Stop();
				_isStoppingSFX = false;
				_audioSource.volume = _volume;
			}
		}
		/*
		if(_isPlayingSFX){
			_audioSource.volume += Time.deltaTime * _fadeTime;
			if(_audioSource.volume >= _sfxVolume){
				_audioSource.volume = _sfxVolume;
				_isPlayingSFX = false;
			}
		}
		*/
	}

	public void PlaySFX(string clipName, float fadeTime, bool _isLooping){
		for(int i = 0; i < _clips.Length; ++i){
			if(_clips[i].name.Equals(clipName)){
				_audioSource.volume = _volume;
				_isStoppingSFX = false;
				_audioSource.loop = _isLooping;
				_audioSource.clip = _clips[i];
				_audioSource.Play();
			}
		}
	}

	public void StopSFX(string clipName, float fadeTime){
		for(int i = 0; i < _clips.Length; ++i){
			if(_clips[i].name.Equals(clipName)){
				_isStoppingSFX = true;
				_fadeTime = fadeTime;
			}
		}
	}
}

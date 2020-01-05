// Created by Carlos Arturo Rodriguez Silva (Legend)

using UnityEngine;
using UnityEngine.UI;
using System;
using Diagnostics = System.Diagnostics;

public class MetronomePro_Player : MonoBehaviour {

	[Header ("Variables")]
	public bool active;
	bool playing = false;

	[Space(5)]

	public AudioSource songAudioSource;
	public Sprite playSprite;
	public Sprite playStartedSprite;
	public Sprite pauseSprite;

	[Space (5)]
	public Text txtSongName;
	public Text actualPosition;
	public Text songTotalDuration;
	public Image playAndPauseButton;
	public Image songPlayerBar;
	public Image iconMusicPlayer;
	public Dropdown velocityScale;
	public Slider songPlayerSlider;

	[Header ("Song Data")]
	public AudioClip songClip;

	public string songName;
	public string songArtist;

	[Space(5)]

	public double Bpm = 128;
	public double OffsetMS = 100;

	public int Step = 4;
	public int Base = 4;

	float amount;

	/// <summary>
	/// Get a MetronomePro script, to avoid call "FindObjectOfType()" many times
	/// </summary>
	public MetronomePro MetronomeParent 
	{
		get
		{
			_metronomePro = _metronomePro != null ? _metronomePro : FindObjectOfType<MetronomePro>();
			return _metronomePro;
		}
		set
		{
			_metronomePro = value;
		}
	}

	private MetronomePro _metronomePro;


	void Start () {

		// Assign the clip to the AudioSource
		songAudioSource.clip = songClip;

		// Display Song Data
		txtSongName.text = songName + " - " + songArtist;
		DisplaySongDuration ();

		// Stop any song and reset values
		StopSong ();

		// Send Song Data to Metronome
		SendSongData();

		MobileElements();
	}

	[Diagnostics.Conditional("UNITY_ANDROID"), Diagnostics.Conditional("UNTY_IOS")]
	public virtual void MobileElements()
	{
		txtSongName.gameObject.SetActive(false);

		if (iconMusicPlayer != null)
		{
			iconMusicPlayer.gameObject.SetActive(false);
		}
	}
	// Sends Song Data to Metronome Pro script
	public void SendSongData () {
		MetronomeParent.GetSongData (Bpm, OffsetMS, Base, Step);
	}

	// Set New Song Name and Artist
	public void SetSongName (string SongName, string SongArtist) {
		txtSongName.text = SongName + " - " + SongArtist;
	}

	// Sets a New Song and Metronome Velocity using Velocity Scale Dropdown Value
	public void SetNewVelocity () {
		if (velocityScale.value == 4) {
			songAudioSource.pitch = 1;
		} else if (velocityScale.value == 5) {
			songAudioSource.pitch = 0.75f;
		} else if (velocityScale.value == 6) {
			songAudioSource.pitch = 0.50f;
		} else if (velocityScale.value == 7) {
			songAudioSource.pitch = 0.25f;
		} else if (velocityScale.value == 3) {
			songAudioSource.pitch = 1.25f;
		} else if (velocityScale.value == 2) {
			songAudioSource.pitch = 1.50f;
		} else if (velocityScale.value == 1) {
			songAudioSource.pitch = 1.75f;
		} else if (velocityScale.value == 0) {
			songAudioSource.pitch = 2.00f;
		} else {
			songAudioSource.pitch = 1;
		}
	}

	public void SetNewBPM(double bpm)
    {
		this.Bpm = bpm;
    }

	// Sets a New Song Position if the user clicked on Song Player Slider
	public void SetNewSongPosition () {
		active = false;
		if (songPlayerSlider.value * songAudioSource.clip.length < songAudioSource.clip.length) {
			songAudioSource.time = (songPlayerSlider.value * songAudioSource.clip.length);
		} else if ((songPlayerSlider.value * songAudioSource.clip.length >= songAudioSource.clip.length)) {
			StopSong ();
		}

		if (MetronomeParent.neverPlayed) {
			MetronomeParent.CalculateIntervals ();
		}

		MetronomeParent.CalculateActualStep ();

		actualPosition.text = UtilityMethods.FromSecondsToMinutesAndSeconds (songAudioSource.time);

		songPlayerBar.fillAmount = songPlayerSlider.value;
		active = true;
	}

	// Calculate Song Total Duration
	public void DisplaySongDuration () {
		try {
		songTotalDuration.text = UtilityMethods.FromSecondsToMinutesAndSeconds (songAudioSource.clip.length);
		} catch {
			MetronomeParent.txtState.text = "Please assign an Audio Clip to the Player!";
			Debug.LogWarning ("Please assign an Audio Clip to the Player!");
		}
	}


	// Play or Pause the Song and Metronome
	public void PlayOrPauseSong() {
		if (playing) {
			Debug.Log ("Song Paused");
			active = false;
			playing = false;
			songAudioSource.Pause ();
			MetronomeParent.Pause ();
			playAndPauseButton.sprite = playStartedSprite;

		} else {
			songAudioSource.Play ();
			MetronomeParent.Play ();
			Debug.Log ("Song Playing");
			playAndPauseButton.sprite = pauseSprite;
			playing = true;
			active = true;
		}
	}


	// Stop Song and Metronome, Resets all too.
	public void StopSong () {
		Debug.Log ("Song Stoped");
		StopAllCoroutines ();
		active = false;
		playing = false;

		songAudioSource.Stop ();
		songAudioSource.time = 0;
		playAndPauseButton.sprite = playSprite;
		amount = 0f;
		songPlayerSlider.value = 0f;
		songPlayerBar.fillAmount = 0f;
		actualPosition.text = "00:00";

		MetronomeParent.Stop ();
	}

	// Next Song
	public void NextSong () {
		StopSong ();

		// Load next song data
		// //

		// songAudioSource.clip = songClip;
		// SendSongData ();
		// PlayOrPauseSong();
	}

	// Previous Song
	public void PreviousSong () {
		StopSong ();

		// Load previous song data
		// //

		// songAudioSource.clip = songClip;
		// SendSongData ();
		// PlayOrPauseSong();
	}

	// Update function is used to Update the Song Player Bar and Actual Position Text every frame and Player quick key buttons
	void Update () {

		if (active) {
			if (playing) {
				if (songAudioSource.isPlaying) {
					amount = (songAudioSource.time) / (songAudioSource.clip.length);
					songPlayerBar.fillAmount = amount;
					actualPosition.text = UtilityMethods.FromSecondsToMinutesAndSeconds(songAudioSource.time);
				} else {
					StopSong();
				}
			}
		}

		KeyboardController();
	}

	[Diagnostics.Conditional("UNITY_EDITOR"), 
	Diagnostics.Conditional("UNITY_STANDALONE"),
	Diagnostics.Conditional("DEVELOPMENT_BUILD")]
	public virtual void KeyboardController() {
		// Play song when user press Space button (PC only)
		if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.P))
		{
			PlayOrPauseSong();
		}
	}
}
	
public static class UtilityMethods {
	public static string FromSecondsToMinutesAndSeconds (float seconds) {
		int sec = (int)(seconds % 60f); 
		int min = (int)((seconds / 60f) % 60f);

		string minSec = min.ToString ("D2") + ":" + sec.ToString ("D2");
		return minSec;
	}
}

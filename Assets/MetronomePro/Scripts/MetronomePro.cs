// Created by Carlos Arturo Rodriguez Silva (Legend)

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class MetronomePro : MonoBehaviour {

	[Serializable]
	public class MetronomeEvent : UnityEvent<MetronomePro, MetronomePro_Player> {}

	[Serializable]
	public class MetronomeEvents
	{
		public MetronomeEvent OnBeat = new MetronomeEvent();
		public MetronomeEvent OnPlay = new MetronomeEvent();
		public MetronomeEvent OnPause = new MetronomeEvent();
		public MetronomeEvent OnStop = new MetronomeEvent();
	}

	[Header("Variables")]
	public bool active = false;
	[Tooltip("Plays the song and metronome when the game starts")]
	public bool autoPlay = true;
	public bool hideInputFields = false;

	[Space(5)]

	public AudioSource metronomeAudioSource;
	public AudioClip highClip;
	public AudioClip lowClip;

	[Range(0.0f, 1.0f)]
	public float metronomeVolume = 1f;

	[Space(5)]

	public Image imgBeat1;
	public Image imgBeat2;
	public Image imgBeat3;
	public Image imgBeat4;

	[Space(5)]
	public Text txtBPM;
	public Text txtOffsetMS;
	public InputField BPMInputField;
	public InputField OffsetInputField;
	public Text txtState;

	[Space(5)]

	public AudioSource songAudioSource;

	[Header("Metronome (this values will be overrided by Song Data)")]

	public double Bpm = 140.0f;
	public double OffsetMS = 0;

	public int Step = 4;
	public int Base = 4;

	public int CurrentMeasure = 0;
	public int CurrentStep = 0;
	public int CurrentTick;

	public List<double> songTickTimes;

	double interval;

	public bool neverPlayed = true;

	[Tooltip("Register events when something happens on Metronome (each beat, on play, on stop...)")]
	public MetronomeEvents events;

	protected MetronomePro_Player metronomePlayer;

	void Awake() {
		metronomePlayer = GetComponentInChildren<MetronomePro_Player>();
		metronomePlayer.MetronomeParent = this;
	}

	void Start() {

		imgBeat1.color = Color.gray;
		imgBeat2.color = Color.gray;
		imgBeat3.color = Color.gray;
		imgBeat4.color = Color.gray;
		txtBPM.text = "BPM: " + Bpm.ToString("F");
		txtState.text = "";

		if (autoPlay || metronomeAudioSource.playOnAwake || songAudioSource.playOnAwake)
		{
			metronomePlayer?.PlayOrPauseSong();
		}

		ShowOrHidePlayer();
	}

	void Update () {

		if (!metronomeVolume.Equals(metronomeAudioSource.volume))
		{
			metronomeAudioSource.volume = metronomeVolume;
		}
	}

	/// <summary>
	/// Show the Metronome/Music player, only for development and debugging
	/// </summary>
	/// <param name="enable">Enable/Disable the player UI</param>
	public virtual void ShowOrHidePlayer(bool enable = true) {

		#if UNITY_EDITOR || DEVELOPMENT_BUILD
			enable = true;
		#else
			enable = false;
		#endif

		Image bgPlayer = metronomePlayer?.GetComponent<Image>();
		var uiChild = metronomePlayer.transform.GetChild(0);
		var content = uiChild?.GetChild(0);

		var myIndex = transform.GetSiblingIndex();
		var bpmAndOffset = transform.parent.GetChild(myIndex - 1);

		bgPlayer.enabled = enable;

		if (content != null)
		{
			content.gameObject.SetActive(enable);
		}

		if (bpmAndOffset != null)
		{
			bpmAndOffset.gameObject.SetActive(hideInputFields ? false : enable);
		}

		txtState.gameObject.SetActive(enable);
	}

	public void GetSongData (double _bpm, double _offsetMS, int _base, int _step) {
		Bpm = _bpm;
		OffsetMS = _offsetMS;
		Base = _base;
		Step = _step;

		txtBPM.text = "BPM: " + Bpm.ToString("F");
		txtOffsetMS.text = "Offset: " + OffsetMS.ToString () + " MS";
	}

	// Set the new BPM when is playing
	public void UpdateBPM () {
		try {
		var newBPMFloat = float.Parse (BPMInputField.text);
		Bpm = (double)newBPMFloat;

		txtBPM.text = "BPM: " + Bpm.ToString("F");
		txtState.text = "";

		SetDelay ();
		} catch {
			txtState.text = "Please enter the new BPM value correctly.";
			Debug.Log ("Please enter the new BPM value correctly.");
		}
	}

	// Set the new Offset when is playing
	public void UpdateOffset () {
		try {
		var newOffsetFloat = int.Parse (OffsetInputField.text);
		OffsetMS = newOffsetFloat;

		txtOffsetMS.text = "Offset: " + OffsetMS.ToString () + " MS";
		txtState.text = "";

		SetDelay ();
		} catch {
			txtState.text = "Please enter the new Offset value correctly.";
			Debug.Log ("Please enter the new Offset value correctly.");
		}
	}


	void SetDelay () {
		bool isPlaying = false;

		if (songAudioSource.isPlaying) {
			isPlaying = true;
		}

		songAudioSource.Pause ();

		CalculateIntervals ();
		CalculateActualStep ();

		if (isPlaying) {
			songAudioSource.Play ();
		}
	}

	// Play Metronome
	public void Play () {
		if (neverPlayed) {
			CalculateIntervals ();
		}

		neverPlayed = false;
		active = true;

		events.OnPlay.Invoke(this, metronomePlayer);
	}

	// Pause Metronome
	public void Pause () {
		active = false;

		events.OnPause.Invoke(this, metronomePlayer);
	}

	// Stop Metronome
	public void Stop () {
		active = false;

		CurrentMeasure = 0;
		CurrentStep = 4;
		CurrentTick = 0;

		events.OnStop.Invoke(this, metronomePlayer);
	}

	// Calculate Time Intervals for the song
	public void CalculateIntervals () {
		try {
		active = false;
		var multiplier = Base / Step;
		var tmpInterval = 60f / Bpm;
		interval = tmpInterval / multiplier;

		int i = 0;

		songTickTimes.Clear ();

			while (interval * i <= songAudioSource.clip.length) {
				songTickTimes.Add ((interval * i) + (OffsetMS / 1000f));
				i++;
			}

			active = true;
		} catch {
			txtState.text = "There isn't an Audio Clip assigned in the Player.";
			Debug.LogWarning ("There isn't an Audio Clip assigned in the Player.");
		}
	}

	// Calculate Actual Step when the user changes song position in the UI
	public void CalculateActualStep () {
		active = false;

		// Get the Actual Step searching the closest Song Tick Time using the Actual Song Time
		for (int i = 0; i < songTickTimes.Count; i++) {
			if (songAudioSource.time < songTickTimes[i]) {
				CurrentMeasure = (i / Base);
				CurrentStep = (int)((((float)i / (float)Base) - (i / Base)) * 4);
				if (CurrentStep == 0) {
					CurrentMeasure = 0;
					CurrentStep = 4;
				} else {
					CurrentMeasure++;
				}

				CurrentTick = i;
				Debug.Log ("Metronome Synchronized at Tick: " + i + " Time: "+ songTickTimes[i]);
				break;
			}
		}
		active = true;
	}

	// Read Audio (this function executes from Unity Audio Thread)
	void OnAudioFilterRead (float[] data, int channels) {
		if (!active)
			return;

		// You can't execute any function of Unity here because this function is working on Unity Audio Thread (this ensure the Metronome Accuracy)
		// To Fix that you need to execute your function on Main Thread again, don't worry i created an easy way to do that :D
		// There are so much other fixes to do this, like Ninja Thread.
		ToMainThread.AssignNewAction ().ExecuteOnMainThread (CalculateTicks());
	}
		
	// Metronome Main function, this calculates the times to make a Tick, Step Count, Metronome Sounds, etc.
	IEnumerator CalculateTicks () {
		if (!active)
			yield return null;

		// Check if the song time is greater than the current tick Time
		if (songAudioSource.time >= songTickTimes [CurrentTick]) {

			CurrentTick++;

			if (CurrentTick >= songTickTimes.Count) {
				active = false;
			}

			// If the Current Step is greater than the Step, reset it and increment the Measure
			if (CurrentStep >= Step) {
				CurrentStep = 1;
				CurrentMeasure++;
				metronomeAudioSource.clip = highClip;
			} else {
				CurrentStep++;
				metronomeAudioSource.clip = lowClip;
			}

			// Call OnTick functions
			StartCoroutine (OnTick ());
		}

		yield return null;
	}

	// Tick Time (execute here all what you want)
	IEnumerator OnTick () {

		// Play Audio Tick
		#if UNITY_EDITOR || DEVELOPMENT_BUILD
			metronomeAudioSource.Play ();
		#endif

		// Change all colors in the UI to gray
		imgBeat1.color = Color.gray;
		imgBeat2.color = Color.gray;
		imgBeat3.color = Color.gray;
		imgBeat4.color = Color.gray;

		// Change the color from the Actual Step Image in the UI
		if (CurrentStep == 1) {
			imgBeat1.color = Color.yellow;
		} else if (CurrentStep == 2) {
			imgBeat2.color = Color.cyan;
		} else if (CurrentStep == 3) {
			imgBeat3.color = Color.cyan;
		} else if (CurrentStep == 4) {
			imgBeat4.color = Color.cyan;
		}


		// YOUR FUNCTIONS HERE (try add callbacks to "OnBeat" event)
		events.OnBeat.Invoke(this, metronomePlayer);

		// Example 1
		/*
		if (CurrentTick == 100) {
			Debug.Log ("OMG! IS THE TICK NUMBER 100!");
		}
		*/

		// Example 2
		// FindObjectOfType<MyAwesomeScript> ().MyFunction ();

		#if UNITY_EDITOR || DEVELOPMENT_BUILD
			Debug.Log ("Current Step: " + CurrentStep + "/" + Step);
		#endif
		yield return null;
	}
}
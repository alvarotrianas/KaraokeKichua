using UnityEngine;
using System.Collections;
using System.Collections.Generic; 
using System.Text.RegularExpressions; 
using UnityEngine.UI;

public class LyricSyncManager : MonoBehaviour {
	
	//Subtitle variables
	private string[] fileLines;
	private List<string> subtitleLines = new List<string>();
	public List<float> subtitleTimings = new List<float>();	
	public List<string> subtitleText = new List<string>();	
	private int nextSubtitle = 0;	
	private string displaySubtitle;	
	private AudioSource audio;
	private string subtitleFromLine;
	public Text lyricText;
	
	public static LyricSyncManager Instance { get; private set; }
	
	void Awake(){
		if(Instance != null && Instance != this)
			Destroy(gameObject);
		
		Instance = this;		
		gameObject.AddComponent<AudioSource>();
	}
	
	void Update () {
		//Increment nextSubtitle when we hit the associated time point
		if(nextSubtitle < subtitleText.Count){
			if(audio.time > subtitleTimings[nextSubtitle]){
				displaySubtitle = subtitleText[nextSubtitle];
				lyricText.text = displaySubtitle;
				nextSubtitle++;
			}
		}
	}
	
	public void BeginDialogue (List<string> songLyricSync, AudioSource clip) {
		lyricText.text = "";
		audio = clip;
		nextSubtitle = 0;
		ResetSubtitlesList ();
		subtitleLines = songLyricSync;
		SplitOutSubtitles ();
		//Set initial subtitle text
		if(subtitleText[0] != null)
			displaySubtitle = subtitleText[0];
	}
	
	private void ResetSubtitlesList(){
		//Reset all lists
		subtitleLines = new List<string>();
		subtitleTimings = new List<float>();
		subtitleText = new List<string>();		
	}

	private void SplitOutSubtitles (){
		//Split out our subtitle elements splitTemp[0]= timeNumber splitTemp[1]= Text
		for(int cnt = 0; cnt < subtitleLines.Count; cnt++){
			string[] splitTemp = subtitleLines[cnt].Split(',');
			if (splitTemp[9] != "") {
				subtitleTimings.Add(ParseTimeToSeconds(splitTemp[1]));
				for (int i = 9; i < splitTemp.Length; i++)
				{
					if (i>9)
						subtitleFromLine += ",";
					subtitleFromLine += splitTemp[i];
				}
				string cleanedSubtitle = CleanSubtitleString(subtitleFromLine);
				subtitleText.Add(cleanedSubtitle);
				subtitleFromLine = "";
			}
		}
	}

	//Set time to seconds
	private float ParseTimeToSeconds(string time){
		string[] timeArray = time.Split(':');
		float seconds = float.Parse(timeArray [0]) * 3600 + float.Parse(timeArray [1]) * 60 + float.Parse(timeArray [2]);
		return seconds;
	}
	
	//Remove all characters in brackets
	private string CleanSubtitleString(string subtitle)	{
		Regex digitsOnly = new Regex(@" ?\{.*?\}");
		if(subtitle.Contains("}")){
			subtitle = digitsOnly.Replace (subtitle, " ");
			return subtitle.Substring(1);
		}
		return subtitle;
	}
}
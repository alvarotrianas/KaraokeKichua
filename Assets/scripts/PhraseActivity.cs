﻿using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class PhraseActivity : Activity {

	public TextAsset json;
	private PhraseActivityData data = new PhraseActivityData ();
	private Phrase phrase = new Phrase();
	[SerializeField]
	private Text phraseTranslatedField;
	[SerializeField]
	private Text phraseBuiltField;
	[SerializeField]
	private RandomWordUI randomWordsPhrase;
	private int wordsOfPhraseCounter;
	private Regex regex = new Regex(@"[^\w\.@-]");

	void Awake(){
		if (level != null) {
			ReadDataFromJson ();
		}
		randomWordsPhrase.RandomWordOfPhraseSelected += HandleRandomWordOfPhraseSelected;
		ActivityStarted += HandleActivityStarted;
		ActivityDataReseted += ReadDataFromJson;
	}

	private void ReadDataFromJson (){
		JsonPhraseParser parser = new JsonPhraseParser();
		data = new PhraseActivityData();
		parser.SetLevelFilter (level);
		parser.JSONString = json.text;
		data = parser.Data;
		CheckIsDataFound ();
	}

	private void CheckIsDataFound() {
		if (data.phrases.Count == 0) {
			isDataFound = false;
			isCompleted = true;
		} else {
			isDataFound = true;
		}
	}

	private void HandleActivityStarted () {
		ClearActivity ();
		CreateActivity ();
		BeginActivity ();
	}

	private void ClearActivity () {
		wordsOfPhraseCounter = 0;
		phraseBuiltField.text = " ";
		randomWordsPhrase.DestroyAllButtons();
		resultsButton.gameObject.SetActive(false);
	}

	private void HandleRandomWordOfPhraseSelected (Button randomWordButton) {
		string nameButton = randomWordButton.transform.GetChild(0).GetComponent<Text>().text;
		string [] correctPhraseArray = randomWordsPhrase.phraseText.Split(' ');
		if (nameButton == correctPhraseArray [wordsOfPhraseCounter]) {
			wordAudio.SetWordToPlay (regex.Replace (nameButton, ""));
			wordAudio.PlayWord ();
			randomWordButton.transform.GetChild(0).GetComponent<Text>().color = new Color32 (74, 9, 92, 122);
			phraseBuiltField.text += (nameButton + " ");
			wordsOfPhraseCounter++;
			randomWordButton.interactable = false;
		}
		if ( (correctPhraseArray.Length-1) <= wordsOfPhraseCounter) {
			wordsOfPhraseCounter = 0;
			randomWordsPhrase.DestroyAllButtons();
			SetActivityAsFinished();
			FinishBuiltPhrase();
		}
	}

	private void FinishBuiltPhrase () {
		resultsButton.gameObject.SetActive(true);
	}

	private void CreateActivity () {
		randomWordsPhrase.DestroyAllButtons();
		phrase = GetRandomPhrase (data.phrases);
		phraseTranslatedField.text = phrase.phraseTranslated;
		randomWordsPhrase.DrawButtonsByWord (phrase.words);
		result.RetryActionExecuted = StartActivity;
	}

	private void BeginActivity ()	{
		gameStateBehaviour.GameState = GameState.PharseActivity;
	}

	private Phrase GetRandomPhrase(List<Phrase> phrases){
		if (phrases.Count != 0) {
			int randomIndex = UnityEngine.Random.Range(0, phrases.Count);
			return phrases[randomIndex];		
		}
		throw new Exception ("El nivel" + GameSettings.Instance.nameLevel [0] + "no tiene frases");
	}
}

﻿using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class WordActivityData {
	public string songName;
	public List<string> wordsList = new List<string> ();
	public List<string> wordsValidsList = new List<string> ();
}

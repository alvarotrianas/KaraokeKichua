﻿using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System;

public static class LevelDataPersistent {
	
	public static List<LevelData> dataLevels = new List<LevelData>();

	public static void Save(List<LevelData> data) {
		SetEnvironment ();
		BinaryFormatter buffer = new BinaryFormatter();
		FileStream file = File.Create (Application.persistentDataPath + "/LevelData.gd"); 
		LevelDataPersistent.dataLevels = data;
		buffer.Serialize(file, LevelDataPersistent.dataLevels);
		file.Close();
	}	
	
	public static void Load() {
		SetEnvironment ();
		if(File.Exists(Application.persistentDataPath + "/LevelData.gd")) {
			BinaryFormatter buffer = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/LevelData.gd", FileMode.Open);
			LevelDataPersistent.dataLevels = (List<LevelData>)buffer.Deserialize(file);
			file.Close();
		}
	}
	
	public static void ClearDataLevels (){		
		List<LevelData> levelData = new List<LevelData>();
		Save(levelData);
	}
	
	public static bool IsLevelUnlock(int level){
		foreach (LevelData levelData in dataLevels) {			
			if(levelData.level == level)
				return levelData.isUnlocked;			
		}
		
		return false;
	}
	
	public static void SaveLevelData(LevelData data){
		Load ();

		foreach (LevelData levelData in dataLevels) {						
			if(levelData.level == data.level){
				levelData.isUnlocked = data.isUnlocked;
				levelData.isIntroOpened = data.isIntroOpened;
				Save(dataLevels);
				return; 
			}			
		}
		
		dataLevels.Add (data);
		Save (dataLevels);
	}

	public static bool IsLevelIntroOpened(int level){
		Load ();
		foreach (LevelData levelData in dataLevels) {			
			if(levelData.level == level)
				return levelData.isIntroOpened;			
		}
		
		return false;
	}

	public static void log(){
		Load ();
		foreach (LevelData levelData in dataLevels) {			
			Debug.Log ("Guarde " + levelData.level+" - "+ levelData.isUnlocked + " - "+ levelData.isIntroOpened);			
		}

	}

	private static void SetEnvironment ()	{
		Environment.SetEnvironmentVariable ("MONO_REFLECTION_SERIALIZER", "yes");
	}
}

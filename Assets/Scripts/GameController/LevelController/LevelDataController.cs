using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using CodeStage.AntiCheat.ObscuredTypes;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class LevelDataController : MonoBehaviour
{

	// Use this for initialization
	//Unit data
	[System.Serializable]
	[XmlRoot ("LevelDataCollection")]
	public class LevelDataCollection
	{
		[XmlArray ("Levels")]
		[XmlArrayItem ("Level")]
		public List<LevelData> ListLevelData = new List<LevelData> ();
	}

	public LevelDataCollection levelDataCollection;
	public LevelDataCollection levelDataCollectionNormal;
	public LevelDataCollection levelDataCollectionHard;

	[System.Serializable]
	public class LevelData
	{
		public int LevelIndex = 0;
		public int NumberOfLanes;
		public int NumberOfPositionsCanBuildUnitInLane;
		public int NumberOfUnitsAllowedDead;
		public int InitialGold;
		[XmlArray ("Waves")]
		[XmlArrayItem ("Wave")]
		public List<Waves> ListWaves = new List<Waves> ();
	}

	[System.Serializable]
	public class Waves
	{
		//public int WaveIndex;
		[XmlArray ("Sequences")]
		[XmlArrayItem ("Sequence")]
		public List<Sequences> ListSequences = new List<Sequences> ();
	}

	[System.Serializable]
	public class Sequences
	{
		public float Time;
		[XmlArray ("Enemies")]
		[XmlArrayItem ("Enemy")]
		public List<EnemyAtSequence> ListEnemyAtSequence = new List<EnemyAtSequence> ();
	}


	[System.Serializable]
	public class EnemyAtSequence
	{
		public string EnemyID;
		public int Lane;
	}

	private bool startInitEnemy;

	public int totalLevel = 40;
	public int lastLevel = 1;

	public int lastLevelHard {
		get {
			return ObscuredPrefs.GetInt ("LastLevelHard", 0);
		}
		set {
			ObscuredPrefs.SetInt ("LastLevelHard", value);
			ObscuredPrefs.Save ();
		}
	}

	[HideInInspector]
	public int currentLevel = 1;
	public Dictionary<int, int> starAtLevel = new Dictionary<int, int> ();

	[System.Serializable]
	public class ScientistAtLevel
	{
		public int level;
		public int lane;
		public int positionInLane;
	}
	// public ScientistAtLevel scien
	public List<ScientistAtLevel> listScientistAtLevel = new List<ScientistAtLevel> ();
	public bool isUnlockedHardLevel = false;

	void Awake ()
	{
		if (Master.LevelData == null) {
			Master.LevelData = this;
		}
		LoadLevelData ();
	}

	void OnDestroy ()
	{
		StopAllCoroutines ();
		CancelInvoke ();
	}

	public void LoadLevelData ()
	{
		TextAsset textAsset = Resources.Load ("Data/Levels/LevelData") as TextAsset;
		var serializer = new System.Xml.Serialization.XmlSerializer (typeof(LevelDataCollection));
		using (var reader = new System.IO.StringReader (textAsset.text)) {
			this.levelDataCollection = (LevelDataCollection)serializer.Deserialize (reader);
		}

		for (int i = 0; i < levelDataCollection.ListLevelData.Count; i++) {
			levelDataCollection.ListLevelData [i].LevelIndex = i + 1;
		}

		lastLevel = ObscuredPrefs.GetInt ("LastLevel", 0);
		totalLevel = levelDataCollection.ListLevelData.Count;

		levelDataCollectionNormal = DeepClone (levelDataCollection);
		levelDataCollectionHard = DeepClone (levelDataCollection);

		GetLevelDataNormal ();
		GetLevelDataHard ();
		GetLevelInfo ();

		if (lastLevel >= totalLevel) {
			isUnlockedHardLevel = true;
		}
	}

	public void GetLevelDataNormal ()
	{
		float timePlus = 1f;
		for (int i = 0; i < levelDataCollectionNormal.ListLevelData.Count; i++) {
			if (levelDataCollectionNormal.ListLevelData [i].ListWaves.Count >= 5) {
				for (int y = 4; y < levelDataCollectionNormal.ListLevelData [i].ListWaves.Count; y++) {
					for (int x = 0; x < levelDataCollectionNormal.ListLevelData [i].ListWaves [y].ListSequences.Count; x++) {
						if (x == 0)
							continue;

						levelDataCollectionNormal.ListLevelData [i].ListWaves [y].ListSequences [x].Time += timePlus;
					}
				}
			}
		}
	}

	public void GetLevelDataHard ()
	{
		float timeMinus = 1.5f;
		for (int i = 0; i < levelDataCollectionHard.ListLevelData.Count; i++) {
			for (int y = 0; y < 4; y++) {
				if (y >= levelDataCollectionHard.ListLevelData [i].ListWaves.Count) {
					break;
				}
				for (int x = 0; x < levelDataCollectionHard.ListLevelData [i].ListWaves [y].ListSequences.Count; x++) {
					if (x == 0)
						continue;
					if (x <= timeMinus)
						continue;
					levelDataCollectionHard.ListLevelData [i].ListWaves [y].ListSequences [x].Time -= timeMinus;
				}
			}
		}
	}

	public void GetLevelInfo ()
	{
		float timePlus = 1.8f;
		for (int i = 0; i < levelDataCollectionNormal.ListLevelData.Count; i++) {
			string se = "";
			for (int y = 0; y < levelDataCollectionNormal.ListLevelData [i].ListWaves.Count; y++) {

				se += " | " + y + " : " + levelDataCollectionNormal.ListLevelData [i].ListWaves [y].ListSequences.Count;
			}
			//   Debug.Log("Level " + (i + 1) + " - Waves " + levelDataCollectionNormal.ListLevelData[i].ListWaves.Count + " | " + se);

		}
	}

	public LevelData GetLevelDataByIndex (int levelIndex, GameplayController.GameDifficult gameDifficult = GameplayController.GameDifficult.Normal)
	{
		if (gameDifficult == GameplayController.GameDifficult.Normal) {
			return levelDataCollectionNormal.ListLevelData [levelIndex - 1];
		} else {
			return levelDataCollectionHard.ListLevelData [levelIndex - 1];
		}
	}

	public int GetStarAtLevel (int level)
	{
		return ObscuredPrefs.GetInt ("StarAtLevel_" + level, 0);
	}

	public void SetStarAtLevel (int level, int star)
	{
		ObscuredPrefs.SetInt ("StarAtLevel_" + level, star);
		ObscuredPrefs.Save ();
	}

	public int GetStarAtLevelHard (int level)
	{
		return ObscuredPrefs.GetInt ("StarAtLevelHard_" + level, 0);
	}

	public void SetStarAtLevelHard (int level, int star)
	{
		ObscuredPrefs.SetInt ("StarAtLevelHard_" + level, star);
		ObscuredPrefs.Save ();
	}

	public void SetLastLevel (int level)
	{
		if (level > lastLevel) {
			lastLevel = level;
			ObscuredPrefs.SetInt ("LastLevel", level);
			ObscuredPrefs.Save ();
			Master.QuestData.SetProgressValue ("02", lastLevel);
		}
	}

	public int GetTotalStarAtLevelsGot ()
	{
		int totalStarAtLevels = 0;
		for (int i = 1; i <= lastLevel; i++) {
			totalStarAtLevels += GetStarAtLevel (i);
		}
		return totalStarAtLevels;
	}

	public bool isScientistAtLevel (int level)
	{
		foreach (ScientistAtLevel scientistAtLevel in listScientistAtLevel) {
			if (scientistAtLevel.level == level) {
				return true;
			}
		}
		return false;
	}

	public string[] GetEnemiesAtLevel (int level)
	{
		List<string> listEnemies = new List<string> ();
		LevelData levelData = GetLevelDataByIndex (level);
		foreach (Waves waves in levelData.ListWaves) {
			foreach (Sequences sequences in waves.ListSequences) {
				foreach (EnemyAtSequence enemyAtSequence in sequences.ListEnemyAtSequence) {
					if (!listEnemies.Contains (enemyAtSequence.EnemyID)) {
						listEnemies.Add (enemyAtSequence.EnemyID);
					}
				}
			}
		}
		return listEnemies.ToArray ();
	}

	public void Save (LevelDataCollection levelDataCollection)
	{
		string path = Application.dataPath + "/Resources/Data/Levels/LevelData.xml";

		var serializer = new XmlSerializer (typeof(LevelDataCollection));
		using (var stream = new FileStream (path, FileMode.Create)) {

			serializer.Serialize (stream, levelDataCollection);
			Debug.Log ("Saved XML to " + path);
		}
	}

	public static T DeepClone<T> (T obj)
	{
		using (var ms = new MemoryStream ()) {
			var formatter = new BinaryFormatter ();
			formatter.Serialize (ms, obj);
			ms.Position = 0;

			return (T)formatter.Deserialize (ms);
		}
	}

	//public bool CheckGotGemRewardAtLevelAndStarGot(int level, int starGotAtLevel)
	//{
	//    bool isGotReward = (ObscuredPrefs.GetInt("IsGotGemRewardAtLevelAndStar_" + level + "_" + starGotAtLevel, 0) == 0) ? false : true;
	//    return isGotReward;
	//}

	//public void SetGotGemRewardAtLevelAndStarGot(int level, int starGotAtLevel)
	//{
	//    ObscuredPrefs.SetInt("IsGotGemRewardAtLevelAndStar_" + level + "_" + starGotAtLevel, 1);
	//    ObscuredPrefs.Save();
	//}

}

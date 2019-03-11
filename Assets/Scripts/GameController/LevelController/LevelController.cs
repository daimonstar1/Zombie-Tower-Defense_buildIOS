using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelController : MonoBehaviour
{

	// Use this for initialization
	[HideInInspector]
	public LevelDataController.LevelData currentLevelData;
	[HideInInspector]
	public int totalWaves;
	[HideInInspector]
	public int currentWave;
	[HideInInspector]
	public int totalSequences;
	[HideInInspector]
	public int totalSequenceIndex;
	[HideInInspector]
	public bool isLastWave;
	[HideInInspector]
	public bool isLastSequence;
	bool isCheckingEnemyExistToStartNewWave;

	GameObject pf_scientist;

	public Dictionary<int, GameObject> scientists = new Dictionary<int, GameObject> ();

	// public List<LevelDataController.ScientistAtLevel> scientistsAtLevel = new List<LevelDataController.ScientistAtLevel>();

	void Awake ()
	{
		pf_scientist = Master.GetGameObjectInPrefabs ("Characters/Units/Scientist");
	}

	void Start ()
	{
		if (Master.Level == null) {
			Master.Level = this;
		}
		SetLevelInfo ();
	}

	void OnDestroy ()
	{
		StopAllCoroutines ();
		CancelInvoke ();
	}

	public void SetLevelInfo ()
	{
		int currentLevel = Master.LevelData.currentLevel;
		currentLevelData = Master.LevelData.GetLevelDataByIndex (Master.LevelData.currentLevel, GameplayController.gameDifficult);
		totalWaves = currentLevelData.ListWaves.Count;
		totalSequences = 0;
		//get total sequence
		for (int i = 0; i < currentLevelData.ListWaves.Count; i++) {
			totalSequences += currentLevelData.ListWaves [i].ListSequences.Count;
		}
		totalSequenceIndex = 0;
		Master.Lane.SetLane (currentLevelData.NumberOfLanes, currentLevelData.NumberOfPositionsCanBuildUnitInLane);
		Master.Gameplay.gold = currentLevelData.InitialGold;
		GetScientistAtLevel ();
	}

	void GetScientistAtLevel ()
	{
		int i = 1;
		foreach (LevelDataController.ScientistAtLevel scientistAtLevel in Master.LevelData.listScientistAtLevel) {
			if (scientistAtLevel.level == Master.LevelData.currentLevel) {
				GameObject scientist = NGUITools.AddChild (Master.Gameplay.unitsRoot, pf_scientist);
				scientist.GetComponent<ScientistController> ().id = i;
				scientist.GetComponent<ScientistController> ().currentLane = scientistAtLevel.lane;
				//Debug.Log(Master.Lane.positionsInLane[scientistAtLevel.lane][scientistAtLevel.positionInLane].gameObject.name + " | " + Master.Lane.positionsInLane[scientistAtLevel.lane][scientistAtLevel.positionInLane].localPosition);
				scientist.transform.position = Master.Lane.positionsInLane [scientistAtLevel.lane] [scientistAtLevel.positionInLane].gameObject.transform.position;
				Master.Lane.SetUnitAtPosition (Master.Lane.positionsInLane [scientistAtLevel.lane] [scientistAtLevel.positionInLane].gameObject, scientist);
				Master.Lane.SetCharacterAtLane (scientist, scientistAtLevel.lane);
				scientists.Add (i, scientist);
				i++;
			}
		}
	}

	public void StartInitEnenmy ()
	{
		InitEnemy (0, 0);
	}

	private void InitEnemy (int waveIndex, int sequenceIndexOfWave)
	{
		if (waveIndex >= currentLevelData.ListWaves.Count)
			return;

		LevelDataController.Waves wave = currentLevelData.ListWaves [waveIndex];

		if (sequenceIndexOfWave >= wave.ListSequences.Count)
			return;

		LevelDataController.Sequences sequence = wave.ListSequences [sequenceIndexOfWave];

		isCheckingEnemyExistToStartNewWave = false;
		currentWave = waveIndex;

		Master.WaitAndDo (sequence.Time, () => {
			totalSequenceIndex++;
			foreach (LevelDataController.EnemyAtSequence enemytAtSequence in sequence.ListEnemyAtSequence) {
				if (!enemytAtSequence.EnemyID.Equals ("10")) {
					GameObject pf_enemy = Master.GetEnemyPrefabByID (enemytAtSequence.EnemyID);
					GameObject go_enemy = NGUITools.AddChild (Master.Gameplay.enemiesRoot, pf_enemy);
					go_enemy.transform.localPosition = new Vector3 (Master.Gameplay.outOfScreenPos [2].localPosition.x, Master.Lane.positionOfLane [enemytAtSequence.Lane].localPosition.y - 30, 0);
					go_enemy.GetComponent<EnemyController> ().status.CurrentLane = enemytAtSequence.Lane;
					Master.Lane.SetCharacterAtLane (go_enemy, enemytAtSequence.Lane);
				}
			}
			sequenceIndexOfWave++;
			if (sequenceIndexOfWave >= wave.ListSequences.Count) {
				if (!isCheckingEnemyExistToStartNewWave) {
					CheckEnemyExistAndStartNewWave (waveIndex, sequenceIndexOfWave);
				}
			} else {
				InitEnemy (waveIndex, sequenceIndexOfWave);
			}
		}, this);
	}

	void CheckEnemyExistAndStartNewWave (int waveIndex, int sequenceIndex)
	{
		isCheckingEnemyExistToStartNewWave = true;
		Master.WaitAndDo (0.5f, () => {
			bool isExistEnemy = Master.Lane.isExistCharacterByTagInAllLane ("Enemy");
			if (!isExistEnemy) {
				sequenceIndex = 0;
				waveIndex++;
				InitEnemy (waveIndex, sequenceIndex);
			} else {
				CheckEnemyExistAndStartNewWave (waveIndex, sequenceIndex);
			}
		}, this);
	}

	public void ScientistDead (int id)
	{
		//scientists.Remove(id);
		//if (scientists.Count <= 0)
		//{
		//    Master.Gameplay.InstantLost();
		//}
		Master.Gameplay.InstantLost ();
	}

}

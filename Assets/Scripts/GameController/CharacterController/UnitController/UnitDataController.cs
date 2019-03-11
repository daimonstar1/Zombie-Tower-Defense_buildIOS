using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using CodeStage.AntiCheat.ObscuredTypes;

public class UnitDataController : MonoBehaviour
{

	//Unit data
	[XmlRoot ("UnitDataCollection")]
	public class UnitDataCollection
	{
		[XmlArray ("Units")]
		[XmlArrayItem ("Unit")]
		public List<UnitData> ListUnitsData = new List<UnitData> ();
	}

	public UnitDataCollection unitDataCollection;

	[System.Serializable]
	public class UnitData
	{
		public string UnitID = "";
		public string UnitName = "";
		public int Level = 1;
		public float Exp = 0;
		public float Health;
		public float Damage;
		public float AttackSpeed;
		public float Range;
		public float Critical;
		public int NumberOfAmmoToReload;
		//-1: no need reload
		public int NumberOfShootToUpgrade;
		public int Price;
		public float TimeCountdownSelect;
		public int UnlockAtLevel;
	}

	[HideInInspector]
	public UnitData unitData;
	public List<UnitData> listUnitData = new List<UnitData> ();

	[System.Serializable]
	public class UnitUpgradeData
	{
		public string UnitID = "";
		public int Level;
		public float Exp;
		public int Health;
		public int Damage;
		public int AttackSpeed;
		public int Range;
		public int Critical;
	}

	[HideInInspector]
	public UnitUpgradeData unitUpgradeData;
	[HideInInspector]
	public List<UnitUpgradeData> listUnitUpgradeData = new List<UnitUpgradeData> ();

	public class UnitUpgradeType
	{
		public static string Level = "Level";
		public static string Exp = "Exp";
		public static string Health = "Health";
		public static string Damage = "Damage";
		public static string AttackSpeed = "AttackSpeed";
		public static string Range = "Range";
		public static string Critical = "Critical";
	}

	[HideInInspector]
	public List<string> listStatsItem = new List<string> { "Damage", "Health", "AttackSpeed", "Range", "Critical" };

	[HideInInspector]
	public List<UnitData> listUnitChoose = new List<UnitData> ();
	//  [HideInInspector]
	public List<UnitData> listUnitAvailable = new List<UnitData> ();

	public int totalUnit;

	//for upgrade setting
	// private float increasePercentPerUpgrade = 15;

	private int firstStarRequireUpgrade = 150;
	private float increaseStarPercentPerUpgrade = 100;
	//%

	[HideInInspector]
	public float maxTimeToShoot = 7;
	//second
	[HideInInspector]
	public float minTimeToShoot = 0.2f;
	//second

	Dictionary<string, float> increaseValuePercentPerUpgrade = new Dictionary<string, float> () {
		{ UnitUpgradeType.Damage, 20 },
		{ UnitUpgradeType.Health, 20 },
		//    { UnitUpgradeType.AttackSpeed, 8}, get another logic
		{ UnitUpgradeType.Range, 8 },
		{ UnitUpgradeType.Critical, 10 },
	};

	// private florat increasePercentInvolUpgradeStats = 15;

	//for level
	private float firstMaxExp = 150;
	private float percentIncreaseMaxExpPerLevel = 200;
	private float expPerShoot = 5;
	// for 35 attack speed, if smaller, exp will be higher
	// private float percentIncreaseExpPerShootPerLevel = 5;
	void Awake ()
	{
		if (Master.UnitData == null) {
			Master.UnitData = this;
		}
	}

	void Start ()
	{
		LoadUnitData ();
		LoadUnitAvaiable ();
		LoadUnitUpgradeData ();
	}


	public void LoadUnitData ()
	{
		listUnitData.Clear ();
		TextAsset textAsset = Resources.Load ("Data/Characters/UnitData") as TextAsset;
		var serializer = new System.Xml.Serialization.XmlSerializer (typeof(UnitDataCollection));
		using (var reader = new System.IO.StringReader (textAsset.text)) {
			this.unitDataCollection = (UnitDataCollection)serializer.Deserialize (reader);
		}
		listUnitData = unitDataCollection.ListUnitsData;
		totalUnit = listUnitData.Count;
	}

	public void LoadUnitAvaiable ()
	{
		listUnitAvailable.Clear ();
		foreach (UnitData unitData in unitDataCollection.ListUnitsData) {
			if (unitData.UnlockAtLevel <= Master.LevelData.lastLevel + 1) {
				listUnitAvailable.Add (unitData);
			}
		}
	}

	public void LoadUnitUpgradeData ()
	{
		listUnitUpgradeData.Clear ();
		for (int i = 0; i < unitDataCollection.ListUnitsData.Count; i++) {
			string unitID = unitDataCollection.ListUnitsData [i].UnitID;
			UnitUpgradeData unitUpgradeData = new UnitUpgradeData ();
			string firstParam = "Unit_" + unitID + "_Upgrade_";
			unitUpgradeData.UnitID = unitID;
			unitUpgradeData.Level = ObscuredPrefs.GetInt (firstParam + UnitUpgradeType.Level, 1);
			unitUpgradeData.Exp = ObscuredPrefs.GetFloat (firstParam + UnitUpgradeType.Exp, 0);
			unitUpgradeData.Health = ObscuredPrefs.GetInt (firstParam + UnitUpgradeType.Health, 0);
			unitUpgradeData.Damage = ObscuredPrefs.GetInt (firstParam + UnitUpgradeType.Damage, 0);
			unitUpgradeData.AttackSpeed = ObscuredPrefs.GetInt (firstParam + UnitUpgradeType.AttackSpeed, 0);
			unitUpgradeData.Range = ObscuredPrefs.GetInt (firstParam + UnitUpgradeType.Range, 0);
			unitUpgradeData.Critical = ObscuredPrefs.GetInt (firstParam + UnitUpgradeType.Critical, 0);
			listUnitUpgradeData.Add (unitUpgradeData);
		}
	}

	public UnitData GetUnitDataByID (string id)
	{
		foreach (UnitData unitData in listUnitData) {
			if (id == unitData.UnitID) {
				return unitData;
			}
		}
		return null;
	}

	public UnitUpgradeData GetUnitUpgradeDataByID (string id)
	{
		foreach (UnitUpgradeData unitUpgradeData in listUnitUpgradeData) {
			if (id == unitUpgradeData.UnitID) {
				return unitUpgradeData;
			}
		}
		return null;
	}

	public UnitData GetUnitDataWithUpgradeByID (string unitID)
	{
		UnitData unitData = GetUnitDataByID (unitID);
		UnitData unitDataWithUpgrade = new UnitData ();
		// withUpgrade = unitData;
		if (unitData == null)
			return null;

		UnitUpgradeData unitUpgradeData = GetUnitUpgradeDataByID (unitID);
		unitDataWithUpgrade.UnitID = unitData.UnitID;
		unitDataWithUpgrade.UnitName = unitData.UnitName;
		unitDataWithUpgrade.Level = unitUpgradeData.Level;
		unitDataWithUpgrade.Exp = unitUpgradeData.Exp;
		unitDataWithUpgrade.Health = Master.IncreaseValues (unitData.Health, unitUpgradeData.Health, increaseValuePercentPerUpgrade [UnitUpgradeType.Health]);
		unitDataWithUpgrade.Damage = Master.IncreaseValues (unitData.Damage, unitUpgradeData.Damage, increaseValuePercentPerUpgrade [UnitUpgradeType.Damage]);
		unitDataWithUpgrade.AttackSpeed = unitData.AttackSpeed + unitUpgradeData.AttackSpeed * CalculateAttackSpeedPerUpgrade (unitData.AttackSpeed);
		//unitDataWithUpgrade.AttackSpeed = Master.IncreaseValues(unitData.AttackSpeed, unitUpgradeData.AttackSpeed, increaseValuePercentPerUpgrade[UnitUpgradeType.AttackSpeed]);
		unitDataWithUpgrade.Range = Master.IncreaseValues (unitData.Range, unitUpgradeData.Range, increaseValuePercentPerUpgrade [UnitUpgradeType.Range]);
		unitDataWithUpgrade.Critical = Master.IncreaseValues (unitData.Critical, unitUpgradeData.Critical, increaseValuePercentPerUpgrade [UnitUpgradeType.Critical]);
		unitDataWithUpgrade.NumberOfAmmoToReload = unitData.NumberOfAmmoToReload;
		unitDataWithUpgrade.NumberOfShootToUpgrade = unitData.NumberOfShootToUpgrade;
		unitDataWithUpgrade.Price = unitData.Price;
		unitDataWithUpgrade.TimeCountdownSelect = unitData.TimeCountdownSelect;
		unitDataWithUpgrade.UnlockAtLevel = unitData.UnlockAtLevel;

		//upgrade by level
		unitDataWithUpgrade = GetUnitDataAfterUpgradeLevel (unitDataWithUpgrade, unitUpgradeData.Level - 1);
		return unitDataWithUpgrade;
	}


	public UnitData GetUnitDataAfterUpgradeInGameplay (UnitData unitData, int times = 1)
	{
		unitData.Health = Master.IncreaseValues (unitData.Health, times, increaseValuePercentPerUpgrade [UnitUpgradeType.Health] * 0.6f);
		unitData.Damage = Master.IncreaseValues (unitData.Damage, times, increaseValuePercentPerUpgrade [UnitUpgradeType.Damage] * 0.6f);
		//unitData.AttackSpeed = unitData.AttackSpeed + CalculateAttackSpeedPerUpgrade(unitData.AttackSpeed);
		//unitData.AttackSpeed = Master.IncreaseValues(unitData.AttackSpeed, times, increaseValuePercentPerUpgrade[UnitUpgradeType.AttackSpeed]);
		unitData.Range = Master.IncreaseValues (unitData.Range, times, increaseValuePercentPerUpgrade [UnitUpgradeType.Range]);
		unitData.Critical = Master.IncreaseValues (unitData.Critical, times, increaseValuePercentPerUpgrade [UnitUpgradeType.Critical]);
		if (unitData.UnitID.Equals ("08")) {
			Debug.LogError ("Unit8 damge_upgrade=" + unitData.Damage);
			float damge_reduce = (unitData.Damage * 30f) / 100f;
			float value = unitData.Damage - damge_reduce;
			unitData.Damage = Mathf.Round (value * 10f) / 10f;
			Debug.LogError ("Unit8 damge_upgrade_reduce=" + unitData.Damage);
		}
		return unitData;
	}

	public UnitData GetUnitDataAfterUpgradeLevel (UnitData unitData, int level)
	{
		unitData.Health = Master.IncreaseValues (unitData.Health, level, increaseValuePercentPerUpgrade [UnitUpgradeType.Health] * 0.5f);
		unitData.Damage = Master.IncreaseValues (unitData.Damage, level, increaseValuePercentPerUpgrade [UnitUpgradeType.Damage] * 0.5f);
		unitData.AttackSpeed = unitData.AttackSpeed + CalculateAttackSpeedPerUpgrade (unitData.AttackSpeed);
		unitData.Range = Master.IncreaseValues (unitData.Range, level, increaseValuePercentPerUpgrade [UnitUpgradeType.Range] * 0.7f);
		unitData.Critical = Master.IncreaseValues (unitData.Critical, level, increaseValuePercentPerUpgrade [UnitUpgradeType.Critical] * 0.7f);
		return unitData;
	}

	public int GetCurrentUpgradedStats (string unitID, string upgradeType)
	{
		int currentUpgardeValue = 0;
		UnitUpgradeData currentUnitUpgradedData = GetUnitUpgradeDataByID (unitID);

		if (upgradeType == UnitUpgradeType.Health) {
			currentUpgardeValue = currentUnitUpgradedData.Health;
		}

		if (upgradeType == UnitUpgradeType.Damage) {
			currentUpgardeValue = currentUnitUpgradedData.Damage;
		}

		if (upgradeType == UnitUpgradeType.AttackSpeed) {
			currentUpgardeValue = currentUnitUpgradedData.AttackSpeed;
		}

		if (upgradeType == UnitUpgradeType.Range) {
			currentUpgardeValue = currentUnitUpgradedData.Range;
		}

		if (upgradeType == UnitUpgradeType.Critical) {
			currentUpgardeValue = currentUnitUpgradedData.Critical;
		}
		return currentUpgardeValue;
	}

	public float GetMaxExpAtLevel (int level)
	{
		return Master.IncreaseValues (firstMaxExp, (level - 1), percentIncreaseMaxExpPerLevel);
	}

	public float GetExpPerShootAtLevel (int level, float attackSpeed = 35)
	{
		//calc exp by attackspeed without level
		// float r = 35 / attackSpeed;
		//  float expPerShootByAttackSpeed = expPerShoot * r;
		//    return Master.IncreaseValues(expPerShootByAttackSpeed, level, percentIncreaseExpPerShootPerLevel);
		//return Master.IncreaseValues(expPerShoot, level, percentIncreaseExpPerShootPerLevel);
		return expPerShoot;
	}

	//float GetValueWithUpgrade(float value, int upgraded)
	//{
	//    for (int i = 0; i < upgraded; i++)
	//    {
	//        value += (float)(value * increasePercentPerUpgrade) / 100;
	//    }
	//    value = Mathf.Round(value * 10f) / 10f;
	//    return value;
	//}

	public UnitData doUpgradeUnitData (string unitID, string upgradeType)
	{
		string firstParam = "Unit_" + unitID + "_Upgrade_";
		int currentUpgardeValue = GetCurrentUpgradedStats (unitID, upgradeType) + 1;

		ObscuredPrefs.SetInt (firstParam + upgradeType, currentUpgardeValue);
		ObscuredPrefs.Save ();

		Master.Stats.Star -= GetStarRequireUpgrade (unitID, upgradeType);

		LoadUnitUpgradeData ();

		return GetUnitDataWithUpgradeByID (unitID);
	}

	public int GetStarRequireUpgrade (string unitID, string upgradeType)
	{
		//get current upgarde
		float value = (float)firstStarRequireUpgrade;
		int currentUpgradedValue = GetCurrentUpgradedStats (unitID, upgradeType);

		for (int i = 0; i < currentUpgradedValue; i++) {
			value += (float)(value * increaseStarPercentPerUpgrade) / 100;
		}
		return (int)value;
	}

	//public UnitData[] GetUnlockUnitAtLevel(int level)
	//{
	//    List<UnitData> listUnitUnlock = new List<UnitData>();
	//    UnitData[] unitsUnlock;
	//    foreach (UnitData unitData in unitDataCollection.ListUnitsData)
	//    {
	//        if (unitData.UnlockAtLevel == level)
	//        {
	//            listUnitUnlock.Add(unitData);
	//        }
	//    }
	//    if (listUnitUnlock.Count > 0)
	//    {
	//        unitsUnlock = new UnitData[listUnitUnlock.Count];
	//        for (int i = 0; i < listUnitUnlock.Count; i++)
	//        {
	//            unitsUnlock[i] = listUnitUnlock[i];
	//        }
	//        return unitsUnlock;
	//    }
	//    else
	//    {
	//        return null;
	//    }
	//}

	public int[] IncreaseExp (float exp, string unitID)
	{
		UnitData unitData = GetUnitDataWithUpgradeByID (unitID);
		float maxExp = GetMaxExpAtLevel (unitData.Level);
		int levelIncrease = 0;
		int[] result = new int[2]; //level bonus-exp

		while (unitData.Exp + exp >= maxExp) {
			levelIncrease++;
			unitData.Level++;
			exp = exp - (maxExp - unitData.Exp);
			unitData.Exp = 0;
			maxExp = GetMaxExpAtLevel (unitData.Level);
		}

		unitData.Exp += exp;

		string firstParam = "Unit_" + unitID + "_Upgrade_";

		ObscuredPrefs.SetInt (firstParam + UnitUpgradeType.Level, unitData.Level);
		ObscuredPrefs.SetFloat (firstParam + UnitUpgradeType.Exp, unitData.Exp);
		result [0] = levelIncrease;
		result [1] = (int)exp;

		//        Debug.Log(unitData.Level + " | " + levelIncrease + " | " + unitData.Exp + " | " + exp);
		LoadUnitUpgradeData ();
		return result;
	}

	public UnitData[] GetUnlockUnitsAtLevel (int level)
	{
		List<UnitData> listUnitUnlock = new List<UnitData> ();
		UnitData[] unitsUnlock;
		foreach (UnitData unitData in unitDataCollection.ListUnitsData) {
			if (unitData.UnlockAtLevel <= level) {
				listUnitUnlock.Add (unitData);
			}
		}
		if (listUnitUnlock.Count > 0) {
			unitsUnlock = new UnitData[listUnitUnlock.Count];
			for (int i = 0; i < listUnitUnlock.Count; i++) {
				unitsUnlock [i] = listUnitUnlock [i];
			}
			return unitsUnlock;
		} else {
			return null;
		}
	}

	public UnitData[] GetUnlockUnitAtLevel (int level)
	{
		List<UnitData> listUnitUnlock = new List<UnitData> ();
		UnitData[] unitsUnlock;
		foreach (UnitData unitData in unitDataCollection.ListUnitsData) {
			if (unitData.UnlockAtLevel == level) {
				listUnitUnlock.Add (unitData);
			}
		}
		if (listUnitUnlock.Count > 0) {
			unitsUnlock = new UnitData[listUnitUnlock.Count];
			for (int i = 0; i < listUnitUnlock.Count; i++) {
				unitsUnlock [i] = listUnitUnlock [i];
			}
			return unitsUnlock;
		} else {
			return null;
		}
	}

	public void CheckUnitUnlock (System.Action actionAfterClose = null)
	{
		UnitDataController.UnitData[] listUnitUnlock = Master.UnitData.GetUnlockUnitAtLevel (Master.LevelData.lastLevel + 1);
		if (listUnitUnlock == null) {
			if (actionAfterClose != null) {
				actionAfterClose ();
			}
		} else {
			string listUnitUnlockStr = "";
			foreach (UnitDataController.UnitData unitData in listUnitUnlock) {
				listUnitUnlockStr += unitData.UnitID + "-";
			}
			Master.UIGameplay.ShowDialog (UIController.Dialog.ListDialogs.NewUnitUnlockDialog, 0.3f, new string[] { listUnitUnlockStr }, null, actionAfterClose);
		}
	}


	public bool CheckUnitUnlocked (string unitID)
	{
		UnitData unitData = GetUnitDataByID (unitID);
		if (unitData.UnlockAtLevel <= (Master.LevelData.lastLevel + 1)) {
			return true;
		}

		return false;
	}

	public

    float CalculateAttackSpeedPerUpgrade (float currentAttackSpeed)
	{
		float valueNeedToMax = (maxTimeToShoot * 10) - currentAttackSpeed;
		return valueNeedToMax / 10;
	}

	public void Save (UnitDataCollection unitDataCollection)
	{
		string path = Application.dataPath + "/Resources/Data/Characters/UnitData.xml";
		var serializer = new XmlSerializer (typeof(UnitDataCollection));
		using (var stream = new FileStream (path, FileMode.Create)) {
			serializer.Serialize (stream, unitDataCollection);
			Debug.Log ("Saved XML to " + path);
		}
	}

}

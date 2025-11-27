using System.Collections.Generic;
using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{

	public static ObjectiveManager Instance;

    public List<ObjectiveData> ObjectiveAssetsList = new List<ObjectiveData>();

	[HideInInspector]
    public List<Objective> ObjectiveRuntimeList = new List<Objective>();

	public int CurrentObjectiveIndex = -1;

	void Awake(){

		if(Instance == null){
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}else{
			Destroy(gameObject);
			return;
		}

		foreach( var data in ObjectiveAssetsList){
			ObjectiveRuntimeList.Add(new Objective { data = data });
		}
	}

	public void StartNextObjective(){


	if( CurrentObjectiveIndex < ObjectiveRuntimeList.Count){
			CurrentObjectiveIndex ++;
			var CurrentObjective = ObjectiveRuntimeList[CurrentObjectiveIndex];
			Debug.Log("nouvel objectif : " + CurrentObjective.title);
			FindAnyObjectByType<DisplayObjScript>()?.Refresh();
	}else {
		Debug.Log("Plus aucun objectif");
	}
	}

	public void CurrentObjectiveCompleted(){
		ObjectiveRuntimeList[CurrentObjectiveIndex].isCompleted = true;
		StartNextObjective();
	}
}

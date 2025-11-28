using System.Collections.Generic;
using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{

	public static ObjectiveManager Instance;

    public List<ObjectiveData> ObjectiveAssetsList = new List<ObjectiveData>();

	[HideInInspector]
    public List<Objective> ObjectiveRuntimeList = new List<Objective>();

	public int CurrentObjectiveIndex = 0;

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
		CurrentObjectiveIndex++;
		
	}

	public void StartNextObjective(){


	if( CurrentObjectiveIndex < ObjectiveRuntimeList.Count){
			CurrentObjectiveIndex ++;
			var CurrentObjective = ObjectiveRuntimeList[CurrentObjectiveIndex];
			Debug.Log("nouvel objectif : " + CurrentObjective.title);
	}else {
		Debug.Log("Plus aucun objectif");
	}
	}

	public void CurrentObjectiveCompleted(){
		ObjectiveRuntimeList[CurrentObjectiveIndex].isCompleted = true;
		StartNextObjective();
	}
}

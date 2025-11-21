using UnityEngine;

[System.Serializable]
public class Objective
{

    public ObjectiveData data;
    public bool isCompleted = false;

    public string title => data != null ? data.title : "No Title";
    public string description => data != null ? data.description : "No description";
    
}

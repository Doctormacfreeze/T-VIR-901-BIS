using UnityEngine;

[CreateAssetMenu(menuName = "objectives/ObjectiveData")]
public class ObjectiveData : ScriptableObject
{
    public string title;
    [TextArea] public string description;
}

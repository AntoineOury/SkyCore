using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;

public class FilterSelectionByComponent : ScriptableWizard
{
    [SerializeField] private Component typeToFind = null;


    [MenuItem("Custom/Filter Selection By Component", priority = 11)]
    public static void SelectGameobjects()
    {
        DisplayWizard<FilterSelectionByComponent>("Filter Selected Gameobjects By Component", "Narrow Selection");
    }

    private void OnWizardCreate()
    {
        GameObject[] selections = Selection.gameObjects;
        if (selections.Length == 0)
        {
            Debug.Log("No selections.");
            return;
        }

        System.Type type = typeToFind.GetType();

        List<GameObject> toSelect = new List<GameObject>();
        foreach (GameObject g in selections)
        {
            if (g.GetComponent(type) != null)
                toSelect.Add(g.gameObject);
        }

        Selection.objects = toSelect.ToArray();
    }
}

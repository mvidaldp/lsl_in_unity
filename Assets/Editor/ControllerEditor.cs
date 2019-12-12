using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Controller))]
public class ControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var myScript = (Controller) target; // inspectorUI pointer object

        // store names and values of enumerator (dropdown list button)
        var options = Enum.GetNames(typeof(Controller.VSync)); // get names of enum as a str[]
        var values = Enum.GetValues(typeof(Controller.VSync)).Cast<int>().ToArray(); // get values as a int[]
        // set a header
        EditorGUILayout.LabelField("Colors", EditorStyles.boldLabel); // same as [Header("")] over class attributes
        myScript.colorOne = EditorGUILayout.ColorField("First", myScript.colorOne); // show color1 picker
        myScript.colorTwo = EditorGUILayout.ColorField("Second", myScript.colorTwo); // show color2 picker
        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("Duration", EditorStyles.boldLabel);
        myScript.durationOne = EditorGUILayout.FloatField("1st (ms)", myScript.durationOne);
        myScript.durationTwo = EditorGUILayout.FloatField("2nd (ms)", myScript.durationTwo);

        EditorGUILayout.Separator(); // adds a separator on the editor, similar to EditorGUILayout.Space(8);

        EditorGUILayout.LabelField("Frame Rate Options", EditorStyles.boldLabel); // another header

        myScript.vSyncEnabled = GUILayout.Toggle(myScript.vSyncEnabled, "VSync"); // show the check button
        using (new EditorGUI.DisabledScope(!myScript.vSyncEnabled)) // when checked, enable the dropdown, disable FPS
        {
            // set the dropdown list
            myScript.vSync =
                (Controller.VSync) EditorGUILayout.IntPopup("Performance", (int) myScript.vSync, options, values);
        }

        using (new EditorGUI.DisabledScope(myScript.vSyncEnabled)) // when unchecked, disable dropdown, enable FPS
        {
            // set the FPS limit box
            myScript.targetFrameRate = EditorGUILayout.IntField("Target FPS", myScript.targetFrameRate);
        }

        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("Debugger", EditorStyles.boldLabel);
        myScript.refreshTime = EditorGUILayout.FloatField("Refresh (ms)", myScript.refreshTime);
    }
}
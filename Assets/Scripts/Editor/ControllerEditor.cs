using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Controller))]
public class ControllerEditor : Editor
{
    public int[] octaves = {0, 1, 2, 3, 4, 5, 6, 7, 8};
    public Dictionary<string, decimal> frequencies = new Dictionary<string, decimal>();
    private double _a = Math.Pow(2, 1 / 12.0); // constant of the formula for calculating frequencies, aprox. 1.0594631
    private const int Tuning = 440; // standard tuning reference for A4: 440Hz
    // half steps from C0 from A4: 4 times 12 semitones (4 octaves) plus half steps from C to A (9) = 57
    private int halfSteps = - 12 * 4 - 9;
    public float frequency = 440.00f;

    void Awake()
    {
        // fill frequencies dictionary
        foreach (int o in octaves)
        {
            foreach (int k in Enum.GetValues(typeof(Controller.Key)))
            {
                var name = ((Controller.Key) k).ToString(); // store name of current k from its int value
                double f = Tuning * Math.Pow(_a, halfSteps);
                decimal rounded = decimal.Round((decimal) f, 2, MidpointRounding.AwayFromZero);
                frequencies.Add(name + o, rounded);
                halfSteps++;
            }
        }
    }

    public override void OnInspectorGUI()
    {
        var myScript = (Controller) target; // inspectorUI pointer object
        // store names and values of enumerator (dropdown list button)
        var options = Enum.GetNames(typeof(Controller.VSync)); // get names of enum as a str[]
        var values = Enum.GetValues(typeof(Controller.VSync)).Cast<int>().ToArray(); // get values as a int[]
        var keys = Enum.GetNames(typeof(Controller.Key));
        var kValues = Enum.GetValues(typeof(Controller.Key)).Cast<int>().ToArray();
        var octaves = myScript.octaves.Select(x => x.ToString()).ToArray();

        // set a header
        EditorGUILayout.LabelField("Colors", EditorStyles.boldLabel); // same as [Header("")] over class attributes
        myScript.colorOne = EditorGUILayout.ColorField("First", myScript.colorOne); // show color1 picker
        myScript.colorTwo = EditorGUILayout.ColorField("Second", myScript.colorTwo); // show color2 picker
        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("Duration", EditorStyles.boldLabel);
        myScript.durationOne = EditorGUILayout.IntField("1st (ms)", myScript.durationOne);
        myScript.durationTwo = EditorGUILayout.IntField("2nd (ms)", myScript.durationTwo);

        EditorGUILayout.Separator(); // adds a separator on the editor, similar to EditorGUILayout.Space(8);

        EditorGUILayout.LabelField("Audio", EditorStyles.boldLabel);
        myScript.key = (Controller.Key) EditorGUILayout.IntPopup("Key", (int) myScript.key, keys, kValues);
        myScript.octave = EditorGUILayout.IntPopup("Octave", myScript.octave, octaves, myScript.octaves);
        decimal freq = frequencies[myScript.key.ToString() + myScript.octave];
        using (new EditorGUI.DisabledScope(true))
        {
            frequency = EditorGUILayout.FloatField("Frequency (Hz)", (float) freq);
        }
        myScript.repeatToneAfter = EditorGUILayout.IntField("Repeat after (ms)", myScript.repeatToneAfter);
        EditorGUILayout.Separator();

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
        myScript.refreshTime = EditorGUILayout.IntField("Refresh (ms)", myScript.refreshTime);
    }
}
// using System;
// using System.Collections.Generic;
// using System.Linq;
// using UnityEditor;
// using UnityEngine;
//
// namespace Editor
// {
//     [CustomEditor(typeof(DataStreams))]
//     [CanEditMultipleObjects]
//     public class DataStreamsEditor : UnityEditor.Editor
//     {
//         private void Awake()
//         {
//
//         }
//
//         public override void OnInspectorGUI()
//         {
//             var myScript = (DataStreams) target; // inspectorUI pointer object
//             // store names and values of enumerator (dropdown list button)
//             var options = Enum.GetNames(typeof(DataStreams.StreamType)); // get names of enum as a str[]
//             var values = Enum.GetValues(typeof(DataStreams.StreamType)).Cast<int>().ToArray(); // get values as a int[]
//
//             // set a header
//             EditorGUILayout.LabelField("Outlets", EditorStyles.boldLabel); // same as [Header("")] over class attributes
//             myScript.nOutlets = EditorGUILayout.IntField("Total", myScript.nOutlets);
//             // myScript.outletStreams = EditorGUILayout.IntField("Total", myScript.nOutlets);
//             EditorGUILayout.Separator();
//             EditorGUILayout.LabelField("Inlets", EditorStyles.boldLabel); // same as [Header("")] over class attributes
//             myScript.nInlets = EditorGUILayout.IntField("Total", myScript.nInlets);
//         }
//     }
// }
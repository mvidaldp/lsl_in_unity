// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Reflection;
// using UnityEditor;
// using UnityEngine;
//
// namespace Editor
// {
//     // TODO:
//     // - Rewrite DataStreams and adapt DataStreamEditor to display StreamSources taken from the fields of the controller or the selected script
//     // - Add KeyDown/Keypress, MouseDown/MouseClick event detector (maybe also HTC controller button pressed)
//     // - Make possible to select one or multiple sources per outlet stream
//     // - Test project on HMDs
//     // - Make recordings enforcing right timestamps on Openvibe
//     
//         
//     // Custom serializable class
//     [Serializable]
//     public class Stream
//     {
//         public string name = "default";
//         public StreamType type = StreamType.Markers;
//         public KeyValuePair<string, object> streamSources = new KeyValuePair<string, object>("default", "default");
//         //
//         // public Stream(Dictionary<string, object> sources)
//         // {
//         //     var defaultSelected = new KeyValuePair<string, object>(sources.ElementAt(0).Key, sources.ElementAt(0).Value);
//         //     streamSources = new[] {defaultSelected};
//         // }
//     }
//     
//     [CustomEditor(typeof(DataStreams))]
//     [CanEditMultipleObjects]
//     public class DataStreamsEditor : UnityEditor.Editor
//     {
//         public List<string> sources = new List<string>();
//         public List<object> values = new List<object>();
//         public Stream[] outletStreams = new Stream[1];
//
//         private void Awake()
//         {
//             
//         }
//
//         public override void OnInspectorGUI()
//         {
//             sources.Clear();
//             values.Clear();
//             var controllerScript = target as Controller;
//             var dataStreamsScript = target as DataStreams;
//             Controller[] components = GameObject.Find("Controller").GetComponents<Controller>();
//             foreach (var component in components)
//             {
//                 foreach (FieldInfo fi in component.GetType().GetFields())
//                 {
//                     sources.Add(fi.Name);
//                     values.Add(fi.GetValue(component));
//                     // Debug.Log(fi.Name + ": " + fi.GetValue(component));
//                 }
//             }
//
//             foreach (var outletStream in dataStreamsScript.outletStreams)
//             {
//                 outletStream = EditorGUILayout.EnumPopup("Source", (int) outletStream.source, sources.ToArray(), values.ToArray());
//             }
//         }
//     }
// }
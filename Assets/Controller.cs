using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Controller : MonoBehaviour
{
    /* TODO:
        - Distinguish between FPS calculating and change delay
        - Upload on Github and tell EEG-VR-Group
        - Clean and comment code. Generate build and try it on windows
        - Try Unity4LSL on it
     */
    public Color colorOne = Color.black;
    public Color colorTwo = Color.white;

    bool _colorChanged; // so = false

    private Camera _cam; // where we actually change the colors

    // FPS related vars:
    // not enough with just enum declaration to be in the inspector, it needs an instantiation
    public enum VSync
    {
        Max = 1, // 1 frame per VSync
        Half = 2, // 1 frame each 2 VSync
        Third = 3, // 1 out of 3
        Fourth = 4 // 1 out of 4
    }

    public VSync vSync = VSync.Max;
    public bool vSyncEnabled;
    public int targetFrameRate = 60;

    public float updateInterval = 0.5F;
    private float _fps;
    private float _referenceTime;

    public float changeDelay = 1000;


    // Start is called before the first frame update
    void Start()
    {
        if (!vSyncEnabled)
            QualitySettings.vSyncCount = 0;
        else
            QualitySettings.vSyncCount = (int) vSync;
        Application.targetFrameRate = targetFrameRate;
        _cam = GetComponent<Camera>();
        _cam.clearFlags = CameraClearFlags.SolidColor;
        _referenceTime = Time.realtimeSinceStartup;
    }

    // Update is called once per frame
    void Update()
    {
        var timeSinceStart = Time.realtimeSinceStartup;
        var elapsedTime = timeSinceStart - _referenceTime;
        if (!vSyncEnabled)
            QualitySettings.vSyncCount = 0;
        else
            QualitySettings.vSyncCount = (int) vSync;
        Application.targetFrameRate = targetFrameRate;
        ++_fps;
        if (elapsedTime >= changeDelay / 1000)
        {
            Debug.Log("FPS: " + _fps);
            _fps = 0;
            _referenceTime = timeSinceStart;
            if (!_colorChanged)
            {
                _cam.backgroundColor = colorOne;
                _colorChanged = true;
            }
            else
            {
                _cam.backgroundColor = colorTwo;
                _colorChanged = false;
            }
        }
    }
}

[CustomEditor(typeof(Controller))]
public class ChangeBackgroundEditor : Editor
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
        myScript.changeDelay = EditorGUILayout.FloatField("Change delay (ms)", myScript.changeDelay);

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
    }
}
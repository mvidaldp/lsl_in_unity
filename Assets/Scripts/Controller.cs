using UnityEngine;

public class Controller : MonoBehaviour
{
    /* TODO:
        - Add audio stimuli
        - Upload on Github and tell EEG-VR-Group
        - Clean and comment code. Generate build and try it on windows
        - Try Unity4LSL on it
     */
    public Color colorOne = Color.black;
    public Color colorTwo = Color.white;

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

    public VSync vSync = VSync.Max; // enum instantiation
    public bool vSyncEnabled;
    public int targetFrameRate = 60;

    private float _fps;
    private float _referenceTime;
    private float _durationReference;

    // delays in ms
    public float refreshTime = 1000;
    public float durationOne = 200;
    public float durationTwo = 50;


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
        _referenceTime = Time.realtimeSinceStartup * 1000;
        _durationReference = Time.realtimeSinceStartup * 1000;
    }

    // Update is called once per frame
    void Update()
    {
        ++_fps;
        var timeSinceStart = Time.realtimeSinceStartup * 1000;
        var elapsedTime = timeSinceStart - _referenceTime;
        var showTime = timeSinceStart - _durationReference;
        var showSum = durationOne + durationTwo;

        if (showTime >= showSum)
        {
            _cam.backgroundColor = colorOne;
            _durationReference = timeSinceStart;
        }
        else if (showTime > durationOne && showTime < showSum)
        {
            _cam.backgroundColor = colorTwo;
        }

        if (elapsedTime >= refreshTime)
        {
            Debug.Log("FPS: " + (1000 * _fps) / elapsedTime);
            _fps = 0;
            _referenceTime = timeSinceStart;

            // update FPS settings
            if (!vSyncEnabled)
                QualitySettings.vSyncCount = 0;
            else
                QualitySettings.vSyncCount = (int) vSync;
            Application.targetFrameRate = targetFrameRate;
        }
    }
}
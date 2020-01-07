using UnityEngine;

public class Controller : MonoBehaviour
{
    /* TODO:
        - Send data through LSL protocol
        - Receive data from ANT system
        - Run test
     */
    // colors to show
    public Color colorOne = Color.black;
    public Color colorTwo = Color.white;

    private Camera _cam; // where we actually change the colors

    // FPS related vars:
    // it needs further instantiation as it's just an enumerator declaration
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

    // audio related vars
    private AudioSource _audioSource; // to play an audio clip

    public enum Key
    {
        C,
        Db,
        D,
        Eb,
        E,
        F,
        Gb,
        G,
        Ab,
        A,
        Bb,
        B
    }

    public Key key = Key.A; // set A as default key
    public int octave = 4; // default octave
    public int repeatToneAfter = 500; // tone repetition delay in ms

    private float _fps;
    private float _referenceTime;
    private float _durationReference;
    private float _audioReference;

    // time in ms
    public int refreshTime = 1000;
    public int durationOne = 200;
    public int durationTwo = 50;


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
        _audioReference = Time.realtimeSinceStartup * 1000;
        _audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void LateUpdate() // check the difference with FixedUpdate(), meant for higher sampling frequency
    {
        ++_fps;
        var timeSinceStart = Time.realtimeSinceStartup * 1000;
        var elapsedTime = timeSinceStart - _referenceTime;
        var showTime = timeSinceStart - _durationReference;
        var lastPlay = timeSinceStart - _audioReference;

        if (showTime >= durationOne + durationTwo)
        {
            _cam.backgroundColor = colorOne;
            _durationReference = timeSinceStart;
        }
        else if (showTime > durationOne && showTime < durationOne + durationTwo)
        {
            _cam.backgroundColor = colorTwo;
        }

        if (lastPlay >= repeatToneAfter)
        {
            _audioReference = timeSinceStart;
            var tone = Resources.Load<AudioClip>("Media/Audio/" + key + octave);
            _audioSource.PlayOneShot(tone);
        }

        if (!(elapsedTime >= refreshTime)) return; // what follows is like putting it into an else block
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
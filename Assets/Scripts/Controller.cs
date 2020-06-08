using System;
using UnityEngine;

public class Controller : MonoBehaviour
{
    // Game components
    private Camera _cam; // where the background color is changed
    private AudioSource _audioSource; // to play an audio clip

    // Colors to use as background (structs cannot be declared as const, only literals)
    private static readonly Color32 ColorOne = Color.black; // first (default)
    private static readonly Color32 ColorTwo = Color.white; // second (change, to detect)

    private const bool Timestamp = true; // true for sending Unity internal timestamp since start

    private float _referenceTime; // time to check for audio and color change
    private bool _isPlaying; // audio is playing
    private bool _colorChanged; // color has changed from default

    // LSL VARs
    // streams info
    private liblsl.StreamInfo _lslStreamInfoAudio;
    private liblsl.StreamInfo _lslStreamInfoColor;

    // streams outlets
    private liblsl.StreamOutlet _lslOutletAudio;
    private liblsl.StreamOutlet _lslOutletColor;

    private const string LslStreamNameAudio = "Audio"; // audio stream name
    private const string LslStreamNameColor = "Diode"; // color stream name
    private const string LslStreamType = "Marker"; // stream type (not relevant, just a label)
    private int _lslChannelCount; // length of the data to be sent (depends on Timestamp)
    private const double NominalRate = liblsl.IRREGULAR_RATE; // assuming irregular frame rate

    // ch format as float since we send {currentFrame (int), changed (0/1 : int), timestamp (seconds with decimals)}
    private const liblsl.channel_format_t LslChannelFormat = liblsl.channel_format_t.cf_float32;

    // global unique identifiers
    private string _guidColor;
    private string _guidAudio;

    // Start is called before the first frame update
    void Start()
    {
        // VARs initialization
        // get components and set background color
        _audioSource = GetComponent<AudioSource>(); // audio source to handle audio playing
        _cam = GetComponent<Camera>(); // camera to handle background color
        _cam.backgroundColor = ColorOne; // background color, set first (default)

        _isPlaying = false; // not playing
        _colorChanged = false; // not changed (first color)
        _referenceTime = 0.5f; // time until change in seconds (play audio, change color)

        _lslChannelCount = Timestamp ? 3 : 2; // length of the data to be sent (+1 if Timestamp)

        _guidAudio = Guid.NewGuid().ToString(); // generate GUID for audio stream
        _guidColor = Guid.NewGuid().ToString(); // generate GUID for color stream

        // audio stream definition
        _lslStreamInfoAudio = new liblsl.StreamInfo(
            LslStreamNameAudio,
            LslStreamType,
            _lslChannelCount,
            NominalRate,
            LslChannelFormat,
            _guidAudio);

        // color stream definition
        _lslStreamInfoColor = new liblsl.StreamInfo(
            LslStreamNameColor,
            LslStreamType,
            _lslChannelCount,
            NominalRate,
            LslChannelFormat,
            _guidColor);

        // create the outlet streams from the stream info defined before
        _lslOutletColor = new liblsl.StreamOutlet(_lslStreamInfoColor);
        _lslOutletAudio = new liblsl.StreamOutlet(_lslStreamInfoAudio);
    }

    // FixedUpdate has the frequency of the physics system; it is called every fixed frame-rate frame
    void FixedUpdate()
    {
        _referenceTime -= Time.deltaTime; // subtract the completion time in seconds since the last frame
        if (_referenceTime <= 0) // when ref time reaches 0
        {
            _audioSource.Play(); // play audio clip (if any specified)
            _cam.backgroundColor = ColorTwo; // set background color to second (change, to detect)
            _referenceTime = 0.5f; // reset ref time
        }
        else _cam.backgroundColor = ColorOne; // background color back to first (default)

        _colorChanged = _cam.backgroundColor == ColorTwo; // if color has been changed (from default/first)
        _isPlaying = _audioSource.isPlaying; // if audio is playing

        var cFrame = Time.frameCount; // current frame since start
        var time = Time.realtimeSinceStartup; // timestamp since start

        // define data arrays to be sent (push) 
        var dataColor = new float[_lslChannelCount];
        var dataAudio = new float[_lslChannelCount];

        // fill arrays (current frame, changed/playing, timestamp)
        dataColor[0] = cFrame;
        dataAudio[0] = cFrame;
        dataColor[1] = _colorChanged ? 1 : 0; // changed/playing encoded as 1/0 since LSL has no bool type
        dataAudio[1] = _isPlaying ? 1 : 0;
        if (_lslChannelCount == 3) // if Timestamp
        {
            dataColor[2] = time;
            dataAudio[2] = time;
        }

        // send data
        _lslOutletColor.push_sample(dataColor);
        _lslOutletAudio.push_sample(dataAudio);
    }
}
using System;
using System.Collections.Generic;
using LSL4Unity;
using UnityEngine;

public enum StreamType
{
    EEG,
    MoCap,
    Gaze,
    VideoRaw,
    VideoCompressed,
    Audio,
    Markers,
    SubjectInfo,
    EnvironmentInfo,
    ExperimentInfo,
    SynchronizationInfo
}

public enum StreamSource
{
    Audio,
    Color,
    ControllerTrigger,
    Dummy,
    KeyDown,
    MouseClick
}

// Custom serializable class
[Serializable]
public class Stream
{
    public string name = Enum.GetName(typeof(StreamSource), StreamSource.Dummy);
    public StreamType type = StreamType.Markers;
    public StreamSource source = StreamSource.Dummy;
    public bool timestamp = false;
}


public class DataStreams : MonoBehaviour
{
    public Stream[] outletStreams = new Stream[1];

    private List<liblsl.StreamInfo> _lslStreamInfo = new List<liblsl.StreamInfo>();
    private List<liblsl.StreamOutlet> _lslOutlet = new List<liblsl.StreamOutlet>();
    private Controller _controller;

    // Assuming that markers are never send in regular intervals
    private double _nominalRate = liblsl.IRREGULAR_RATE;

    private const liblsl.channel_format_t LslChannelFormat = liblsl.channel_format_t.cf_float32;

    // Start is called before the first frame update
    void Start()
    {
        _controller = GetComponent<Controller>();
        // create stream info and outlet
        foreach (var t in outletStreams)
        {
            Guid uuid = Guid.NewGuid();
            var lslChannelCount = t.timestamp ? 3 : 2;
            liblsl.StreamInfo streamInfo = new liblsl.StreamInfo(
                t.name,
                t.type.ToString(),
                lslChannelCount,
                _nominalRate,
                LslChannelFormat,
                uuid.ToString());
            _lslStreamInfo.Add(streamInfo);
            _lslOutlet.Add(new liblsl.StreamOutlet(streamInfo));
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var time = Time.realtimeSinceStartup;
        var cFrame = Time.frameCount;
        int i = 0;
        foreach (var stream in _lslOutlet)
        {
            var size = stream.info().channel_count();
            var value = 0;
            // if size 2 then {currentFrame, value}, otherwise {currentFrame, value, timestamp}
            var data = new float[size];
            data[0] = cFrame;
            switch (outletStreams[i].source.ToString())
            {
                case "Audio":
                    value = _controller.isPlaying ? 1 : 0;
                    break;
                case "Color":
                    value = _controller.colorChanged ? 1 : 0;
                    break;
                case "ControllerTrigger":
                    value = 0;
                    break;
                case "Dummy":
                    value = 0;
                    break;
                case "KeyDown":
                    value = 0;
                    break;
                case "MouseClick":
                    value = 0;
                    break;
            }
            data[1] = value;
            if (size == 3)
                data[2] = time;
            stream.push_sample(data);
            i++;
        }
    }
}
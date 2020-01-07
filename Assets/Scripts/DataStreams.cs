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

// Custom serializable class
[Serializable]
public class Stream
{
    public string sName = "Default";
    public StreamType sType = StreamType.Markers;
}

public class DataStreams : MonoBehaviour
{
    public Stream[] outletStreams = new Stream[1] {new Stream()};
    public Stream[] inletStreams = new Stream[1] {new Stream()};

    private liblsl.StreamInfo _lslStreamInfo;
    private liblsl.StreamOutlet _lslOutlet;
    private int lslChannelCount = 1;

    // Assuming that markers are never send in regular intervals
    private double _nominalRate = liblsl.IRREGULAR_RATE;

    private const liblsl.channel_format_t LslChannelFormat = liblsl.channel_format_t.cf_string;

    private string[] _sample = {"Hey!"};

    // private LSLMarkerStream _marker;

    // Start is called before the first frame update
    void Start()
    {
        _sample = new string[lslChannelCount];
        // _marker = FindObjectOfType<LSLMarkerStream>();
        Guid uuid = Guid.NewGuid();
        // create stream info and outlet
        foreach (var t in outletStreams)
        {
            _lslStreamInfo = new liblsl.StreamInfo(
                t.sName,
                t.sType.ToString(),
                lslChannelCount,
                _nominalRate,
                LslChannelFormat,
                uuid.ToString());
            _lslOutlet = new liblsl.StreamOutlet(_lslStreamInfo);
        }

        // foreach (var t in inletStreams)
        // {
        //     liblsl.StreamInfo info = new liblsl.StreamInfo(t.sName, t.sType.ToString(), 8, 100,
        //         liblsl.channel_format_t.cf_float32, uuid.ToString());
        //     liblsl.StreamInlet inlet = new liblsl.StreamInlet(info);
        // }
    }

    private void Write(string marker = "Hey!")
    {
        _sample[0] = marker;
        _lslOutlet.push_sample(_sample);
    }

    // Update is called once per frame
    void Update()
    {
        Write();
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    None 
}

// Custom serializable class
[Serializable]
public class Stream
{
    public string name = "default";
    public StreamType type = StreamType.Markers;
    public StreamSource source = StreamSource.None;
    // void GetSources()
    // {
    //     Controller[] components = GameObject.Find("Controller").GetComponents<Controller>();
    //     foreach (var component in components)
    //     {
    //         int cnt = 0;
    //         foreach (FieldInfo fi in component.GetType().GetFields())
    //         {
    //             StreamSource newSource = Enum.Parse(typeof(StreamSource), fi.Name); 
    //             
    //             // values.Add(fi.GetValue(component));
    //         }
    //     }
    // }
}


public class DataStreams : MonoBehaviour
{
    public Stream[] outletStreams = new Stream[1];
    public Dictionary<string, object> sourceAndValues = new Dictionary<string, object>();
    
    private List<liblsl.StreamInfo> _lslStreamInfo = new List<liblsl.StreamInfo>();
    private List<liblsl.StreamOutlet> _lslOutlet = new List<liblsl.StreamOutlet>();
    private int lslChannelCount = 1;

    // Assuming that markers are never send in regular intervals
    private double _nominalRate = liblsl.IRREGULAR_RATE;

    private const liblsl.channel_format_t LslChannelFormat = liblsl.channel_format_t.cf_string;

    private string[] _sample = {"Hey!"};

    // Start is called before the first frame update
    void Start()
    {

        _sample = new string[lslChannelCount];
        // _marker = FindObjectOfType<LSLMarkerStream>();
        // create stream info and outlet
        foreach (var t in outletStreams)
        {
            Guid uuid = Guid.NewGuid();
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

    private void Write(string marker = "Hey!")
    {
        _sample[0] = marker;
        foreach (var stream in _lslOutlet)
        {
            stream.push_sample(_sample);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Write();
    }
}
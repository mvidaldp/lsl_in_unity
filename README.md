# lsl_in_unity

Simple Unity 2D project with changing background color and click audio playing using [LabStreamingLayer](https://github.com/sccn/labstreaminglayer). This project is a very basic example meant for audio and video latency testing of EEG recordings in VR experiments using the LSL library for data streaming.

Color changing from black to white sample:

![Changing Background](https://raw.githubusercontent.com/mvidaldp/lsl_latency_analysis/master/img/background.gif)

To customize the playing audio, the changing colors or the exposition or repetition times, as well as how the data is sent via LSL, modify the script: [Assets/Scripts/Controller.cs](https://github.com/mvidaldp/lsl_in_unity/blob/master/Assets/Scripts/Controller.cs)

Use the LSL App [LabRecorder](https://github.com/labstreaminglayer/App-LabRecorder/releases) while running the build or playing the scene on the editor to see and record the LSL data streams.

For the further latency analysis check out this repo: https://github.com/mvidaldp/lsl_latency_analysis

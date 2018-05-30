using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
[TrackClipType(typeof(ExampleClip))]
[TrackBindingType(typeof(GameObject))]
public class ExampleTrack : TrackAsset {

    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return base.CreateTrackMixer(graph, go, inputCount);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class ExampleClip : PlayableAsset
{
    [SerializeField]
    private int m_ExampleId;
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<Behaviour>.Create(graph);
        Behaviour behaviour = playable.GetBehaviour();
        //设置behavior属性
        behaviour.exampleId = m_ExampleId;
        return playable;
    }

    private sealed class Behaviour : PlayableBehaviour
    {
        public int exampleId;
        private bool tickFlag = false;
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            base.ProcessFrame(playable, info, playerData);
            if (tickFlag)
                return;
            tickFlag = true;
            Debug.Log("TimeLine Example 开始执行"+exampleId);
            
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            base.OnBehaviourPause(playable, info);
            Debug.Log("TimeLine Example 结束"+exampleId);
        }
    }
}

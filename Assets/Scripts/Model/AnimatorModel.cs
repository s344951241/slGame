using System;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorModel : GlobalBase
{
    //
    // Fields
    //
    public Animator _animator;

    public int _curEventId;

    public float m_aniSpeedRecoverTime;

    public Dictionary<int, AnimationClip> m_dicAnimationClip;

    //
    // Properties
    //
    public Animator animator
    {
        get
        {
            if (this._animator == null)
            {
                this._animator = this.own.transform.GetComponent<Animator>();
                if (this._animator != null)
                {
                    if (!this._animator.enabled)
                    {
                        this._animator.enabled = true;
                    }
                    if (this._animator.logWarnings)
                    {
                        this._animator.logWarnings = false;
                    }
                    if (this._animator.cullingMode != AnimatorCullingMode.AlwaysAnimate)
                    {
                        this._animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
                    }
                }
            }
            return this._animator;
        }
    }

    public int CurEventId
    {
        get
        {
            return this._curEventId;
        }
    }

    public bool isLoop
    {
        get
        {
            return this.GetAnimationClip(this._curEventId).wrapMode == WrapMode.Loop;
        }
    }

    //
    // Constructors
    //
    public AnimatorModel(EntityBase own) : base(own)
    {
        this.m_dicAnimationClip = new Dictionary<int, AnimationClip>();
    }

    //
    // Methods
    //
    public void ActionBegin(int eventId, bool restart = true)
    {
        if (this._animator == null)
        {
            return;
        }
        if (!restart)
        {
            return;
        }
        if (!this.IsEventIdExistInController(eventId))
        {
            return;
        }
        this.TodoEvent(eventId);
    }

    public void ActionEnd(int eventId)
    {
        if (this._animator == null)
        {
            return;
        }
        if (this.IsCurState(eventId))
        {
            this.ActionBegin(0, true);
        }
    }

    public void AttackOver()
    {
        this.ActionEnd(this._curEventId);
    }

    public override void Destroy()
    {
        base.Destroy();
        if (this._animator != null)
        {
            this._animator.enabled = false;
        }
        this._animator = null;
    }

    public float getActSumTime(int eventId)
    {
        AnimationClip animationClip = this.GetAnimationClip(eventId);
        return animationClip.length;
    }

    public AnimationClip GetAnimationClip(int EventId)
    {
        if (this.m_dicAnimationClip.Count == 0)
        {
            AnimationClip[] animationClips = this.animator.runtimeAnimatorController.animationClips;
            AnimationClip[] array = animationClips;
            for (int i = 0; i < array.Length; i++)
            {
                AnimationClip animationClip = array[i];
                int key = int.Parse(animationClip.name.Split(new char[] {
                    '@'
                })[1]);
                if (!this.m_dicAnimationClip.ContainsKey(key))
                {
                    this.m_dicAnimationClip.Add(key, animationClip);
                }
            }
        }
        AnimationClip result = null;
        this.m_dicAnimationClip.TryGetValue(EventId, out result);
        return result;
    }

    public bool IsCurState(int eventId)
    {
        return eventId == this._curEventId;
    }

    public bool IsEventIdExistInController(int eventId)
    {
        RuntimeAnimatorController runtimeAnimatorController = this._animator.runtimeAnimatorController;
        return !(runtimeAnimatorController == null);
    }

    public override void OnUpdate(float dt)
    {
        if (this._animator == null)
        {
            return;
        }
        base.OnUpdate(dt);
        if (this.m_aniSpeedRecoverTime > 0)
        {
            this.m_aniSpeedRecoverTime -= dt;
        }
        if (this.m_aniSpeedRecoverTime < 0)
        {
            this.SetSpeed(1, 0);
        }
    }

    public override void Release(bool cache = true)
    {
        this.ActionBegin(0, true);
        this.SetActive(false);
        this.m_dicAnimationClip.Clear();
        this._animator = null;
    }

    public new void SetActive(bool boo)
    {
        if (boo)
        {
            if (this.animator != null)
            {
                this.animator.enabled = true;
                this.ActionBegin(0, true);
            }
        }
        else
        {
            if (this._animator != null)
            {
                this._animator.enabled = boo;
            }
        }
    }

    public void SetEvent()
    {
        this.WriteEventToUnity(this._curEventId);
    }

    public void SetSpeed(float speed, float t)
    {
        if (this._animator == null)
        {
            return;
        }
        this._animator.speed = speed;
        this.m_aniSpeedRecoverTime = t;
    }

    public void TodoEvent(int eventId)
    {
        if (this._animator == null)
        {
            return;
        }
        this.SetSpeed(1, 0);
        this._curEventId = eventId;
        this.SetEvent();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            this._animator.SetInteger("EVENT_ID", 200);
        }
    }

    public void WriteEventToUnity(int eventId)
    {
        this._animator.SetInteger("EVENT_ID", eventId);
    }
}

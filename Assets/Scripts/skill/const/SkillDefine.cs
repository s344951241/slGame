using UnityEngine;
using System.Collections;

public class SkillDefine {
}
    public enum SKILL_BREAK_STATE
    {
        替换,
        不替换,
        优先一 = 21,
        优先二,
        优先三,
        优先四,
        优先五,
        优先六,
        优先七,
        优先八,
        优先九
    }
    public enum SKILL_BULLET_PATH_TYPE
    {
        直线 = 1,
        抛物线
    }
    public enum SKILL_BULLET_TARGET_TYPE
    {
        目标 = 1,
        目标位置,
        射程位置,
        自定义终点,
        瞄准终点
    }
    public enum SKILL_BULLET_TYPE
    {
        普通 = 1,
        AOE
    }
    public enum SKILL_EVENT_TYPE
    {
        动作,
        子弹,
        特效,
        声音
    }
    public enum SKILL_GUN_HOLD_STATE
    {
        普通,
        隐藏,
        背着
    }
    public enum SKILL_HIT_TYPE
    {
        全部 = 1,
        友方,
        敌方
    }
    public enum SKILL_MOVE_STATE
    {
        正常,
        不能移动,
        可以移动,
        移动不做动作
    }
    public enum SKILL_ROTATE_STATE
    {
        可以转向,
        不能转向
    }
    public enum SKILL_STATE_TYPE
    {
        无,
        结束
    }
public struct SkillCasterData
{
    //
    // Fields
    //
    public int roleKey;

    public Vector3 oriPos;
}

public struct SkillDamageData
{
    //
    // Fields
    //
    public float value;
}
public struct SkillHitData
{
    //
    // Fields
    //
    public int gid;

    public Vector3 srcPos;

    public Vector3 hitPos;

    public Vector3 hitDir;
}

public struct SkillHitEndData
{
    //
    // Fields
    //
    public Vector3? srcPos;

    public Vector3? hitPos;

    public Vector3? hitDir;
}
public struct SkillPosData
{
    //
    // Fields
    //
    public Vector3 pos;

    public Vector3 dir;
}
public struct SkillPreData
{
    //
    // Fields
    //
    public Vector3? preBeginDir;

    public Vector3? preBeginPos;

    public Vector3? preEndPos;
}

public struct SkillStateData
{
    //
    // Fields
    //
    public SKILL_STATE_TYPE stateType;

    public float beginTime;

    public float duration;

    public bool triggerCD;

    public SKILL_BREAK_STATE breakState;

    public SKILL_MOVE_STATE moveState;

    public SKILL_ROTATE_STATE rotateState;

    public SKILL_GUN_HOLD_STATE holdState;
}

public struct SkillTargetData
{
    //
    // Fields
    //
    public int roleKey;

    public Vector3? oriPos;
}

using System;
using UnityEngine;

public struct LayerName
{
    //
    // Static Fields
    //
    public const string NPCLayer = "NPCLayer";

    public const string OffScreenObjLayer = "OffScreenRenderingObj";

    public const string ShadowProjectorLayer = "ShadowProjector";

    public const string ChatLayer = "Chat";

    public const string HinderLayer = "HinderLayer";

    public const string AirWallLayer = "AirWallLayer";

    public const string WallLayer = "WallLayer";

    public const string FrontObject = "FrontObject";

    public const string ShaderHooker = "ShaderHooker";

    public const string EffectLayer = "EffectLayer";

    public const string HUDLayer = "HUDLayer";

    public const string UILayer = "UI";

    public const string ModelLayer = "ModelLayer";

    public const string SceneLayer = "SceneLayer";

    public const string GroundLayer = "GroundLayer";

    //
    // Static Properties
    //
    public static int iAirWallLayer
    {
        get
        {
            return LayerMask.NameToLayer("AirWallLayer");
        }
    }

    public static int iChatLayer
    {
        get
        {
            return LayerMask.NameToLayer("Chat");
        }
    }

    public static int iEffectLayer
    {
        get
        {
            return LayerMask.NameToLayer("EffectLayer");
        }
    }

    public static int iFrontObject
    {
        get
        {
            return LayerMask.NameToLayer("FrontObject");
        }
    }

    public static int iGroundLayer
    {
        get
        {
            return LayerMask.NameToLayer("GroundLayer");
        }
    }

    public static int iHinderLayer
    {
        get
        {
            return LayerMask.NameToLayer("HinderLayer");
        }
    }

    public static int iHUDLayer
    {
        get
        {
            return LayerMask.NameToLayer("HUDLayer");
        }
    }

    public static int iModelLayer
    {
        get
        {
            return LayerMask.NameToLayer("ModelLayer");
        }
    }

    public static int iNPCLayer
    {
        get
        {
            return LayerMask.NameToLayer("NPCLayer");
        }
    }

    public static int iOffScreenRenderingLayer
    {
        get
        {
            return LayerMask.NameToLayer("OffScreenRenderingObj");
        }
    }

    public static int iSceneLayer
    {
        get
        {
            return LayerMask.NameToLayer("SceneLayer");
        }
    }

    public static int iShaderHooker
    {
        get
        {
            return LayerMask.NameToLayer("ShaderHooker");
        }
    }

    public static int iShadowProjectorLayer
    {
        get
        {
            return LayerMask.NameToLayer("ShadowProjector");
        }
    }

    public static int iUILayer
    {
        get
        {
            return LayerMask.NameToLayer("UI");
        }
    }

    public static int iWallLayer
    {
        get
        {
            return LayerMask.NameToLayer("WallLayer");
        }
    }
}

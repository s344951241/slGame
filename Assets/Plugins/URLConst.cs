using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class URLConst{
    public const string EXTEND_ASSETBUNDLE = ".u";
    public const string EXTEND_PM3 = ".mp3";
    public const string EXTEND_OGG = ".ogg";
    public const string EXTEND_WAV = ".wac";
    public const string EXTEND_UNITY = ".unity";

    public static readonly string LIGHTMAP_URL = "Scenes/{0}/LightmapFar-0";
    public static readonly string PLAYER_AVATAR_PATH = "Avatars/Players/";
    public static readonly string MONSTER_AVATAR_PATH = "Avatars/Monsters/";
    public static readonly string PET_AVATAR_PATH = "Avatars/Pets/";
    public static readonly string WEAPON_PATH = "Avatars/Equips/";
    public static readonly string SCENE_PATH = "Scenes/";
    public static readonly string LIGHT_PATH = "Lights/";
    public static readonly string SOUND_PATH = "Musics/";
    public static readonly string MOVE_PATH = "Movies/";
    public static readonly string DATA_CONFIG = "data" + EXTEND_ASSETBUNDLE;
    public static readonly string FONT_CONFIG = "UI/font/字体名"+EXTEND_ASSETBUNDLE;
    public static readonly string CONFIG_CONFIG = "configs"+EXTEND_ASSETBUNDLE;
    public static readonly string SHADER_CONFIG = "shaders"+EXTEND_ASSETBUNDLE;
    public static readonly string LUA_CONFIG = "luas"+EXTEND_ASSETBUNDLE;

    public static readonly string ICON_PATH = "Icon"+EXTEND_ASSETBUNDLE;
    public static readonly string ICON_ATLAS_PATH = "UI/IconAtlas"+EXTEND_ASSETBUNDLE;
    public static readonly string ICON1_ATLAS_PATH = "UI/Icon1Atlas"+EXTEND_ASSETBUNDLE;
    public static readonly string ICON2_ATLAS_PATH = "UI/Icon2Atlas"+EXTEND_ASSETBUNDLE;
    public static readonly string ICON_ATLAS_MATERIAL_PATH = "UI/IconAtlas_material"+EXTEND_ASSETBUNDLE;

    public static readonly string SHARED_PATH = "UI/Shared/Shared"+EXTEND_ASSETBUNDLE;
    public static readonly string SHARED_ALPHA_PATH = "UI/Shared/Shared_alpha"+EXTEND_ASSETBUNDLE;
    public static readonly string SHARED_ETC_PATH = "UI/Shared/Shared_etc"+EXTEND_ASSETBUNDLE;

    public static readonly string UI_ROOT = GetUI("UIRoot");
    public static readonly string UI_CREATER = GetUI("UICharacterCreateMain");
    public static readonly string UI_HUDCanvas = GetUI("UIHudCanvas");
    public static readonly string UI_LOADINF = GetUI("UILoading");

    public static readonly string ATLAS_COMMON_BTN = "atlas_common_btn";
    public static readonly string ATLAS_COMMON_OTHER = "atlas_common_other";
    public static readonly string MODEL_PATH = "Models/";
    public static readonly string UI_MAIN_VIEW = "UI/UIMainView"+EXTEND_ASSETBUNDLE;

    public static string GetModel(string name)
    {
        return MODEL_PATH+name+EXTEND_ASSETBUNDLE;
    }
    public static string GetLocalPath(string path)
    {   
        return "file:///"+Application.dataPath+"/Resources"+path;
    }
    public static string GetPlayerAvatar(string name)
    {
        return PLAYER_AVATAR_PATH+name+EXTEND_ASSETBUNDLE;
    }
    public static string GetMonsterAvatar(string name)
    {
        return MONSTER_AVATAR_PATH+name+EXTEND_ASSETBUNDLE;
    }
    public static string GetWeapon(string weaponId)
    {
        return WEAPON_PATH+weaponId+EXTEND_ASSETBUNDLE;
    }
    public static string GetScene(string sceneId)
    {
        return SCENE_PATH+"Scene"+sceneId+EXTEND_ASSETBUNDLE;
    }
    public static string GetSound(string soundId)
    {
        return SOUND_PATH + soundId + EXTEND_ASSETBUNDLE;
    }
    public static string GetUI(string name)
    {
        return "UI/"+name+EXTEND_ASSETBUNDLE;
    }
    public static string GetRunTimeMaterial(string name)
    {
        return "Materials"+name+EXTEND_ASSETBUNDLE;
    }
    public static string GetRunTimeTexture(string name)
    {
        return "Textures/"+name+EXTEND_ASSETBUNDLE;
    }
    public static string GetAtlas(string name)
    {
        return "Atlas/"+name+EXTEND_ASSETBUNDLE;
    }
    public static string GetEffect(string name)
    {
        return "Effects/"+name+EXTEND_ASSETBUNDLE;
    }
    public static string GetScenePrefab(string sceneId)
    {
        return SCENE_PATH+"ScenePrefab"+sceneId+EXTEND_ASSETBUNDLE;
    }
    public static List<string> listInitGameRes = new List<string>{
        SHADER_CONFIG,LUA_CONFIG,CONFIG_CONFIG,ICON_ATLAS_PATH,SHARED_ETC_PATH,GetUI("UIRoot")
    };
}

using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEditor.Animations;

public class BuildAnimation : EditorWindow
{
    private static BuildAnimation _editor;

    //生成出的Prefab的路径
    private static string PrefabPath = "Assets/Resources/Prefabs";
    //生成出的AnimationController的路径
    private static string AnimationControllerPath = "Assets/AnimationController";
    //生成出的Animation的路径
    private static string _aniPath;
    //美术给的原始图片路径
    private static string _imagePath;
    private static string _shadowPath;
    private static string _controllerName;

    private static string[] conStr = { "idle", "walk", "attack", "dead", "skillA", "born" };

    private static int _zhenIdle = 8;
    private static int _zhenWalk = 8;
    private static int _zhenAttack = 8;
    private static int _zhenDead = 8;
    private static int _zhenSkillA = 8;
    private static int _zhenBorn = 8;

    [MenuItem("Game Tools/BuildAnimaiton")]
    public static void Init()
    {
        _editor = BuildAnimation.GetWindow<BuildAnimation>(true, "动画编辑器", true);
        _editor.position = new Rect(400, 150, 600, 400);
    }

    void OnGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Space(10);
        GUILayout.Label("Image路径");
        _imagePath = getPathEditor(_imagePath);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(10);
        GUILayout.Label("shadow路径");
        _shadowPath = getPathEditor(_shadowPath);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(10);
        GUILayout.Label("animation路径");
        _aniPath = getPathEditor(_aniPath);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(10);
        GUILayout.Label("每秒播放几张图【待机动作idle】");
        _zhenIdle = EditorGUILayout.IntField(_zhenIdle);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(10);
        GUILayout.Label("每秒播放几张图【行走动作Walk】");
        _zhenWalk = EditorGUILayout.IntField(_zhenWalk);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(10);
        GUILayout.Label("每秒播放几张图【攻击动作Attack】");
        _zhenAttack = EditorGUILayout.IntField(_zhenAttack);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(10);
        GUILayout.Label("每秒播放几张图【死亡动作dead】");
        _zhenDead = EditorGUILayout.IntField(_zhenDead);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(10);
        GUILayout.Label("每秒播放几张图【技能动作SkillA】");
        _zhenSkillA = EditorGUILayout.IntField(_zhenSkillA);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Space(10);
        GUILayout.Label("每秒播放几张图【出生动作Born】");
        _zhenBorn = EditorGUILayout.IntField(_zhenBorn);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUI.Button(new Rect((_editor.position.width - 100) / 2, _editor.position.height - 50, 100, 25), "生成动画"))
        {
            FileInfo image = new FileInfo(_imagePath);
            CreateAnimator(_aniPath, image.Name.Split('_')[0]);
        }
    }

    private static void CreateAnimator(string actPath, string controllerName)
    {
        if (string.IsNullOrEmpty(_imagePath) || string.IsNullOrEmpty(actPath))
        {

        }
        else
        {
            BuildAniamtionSl(actPath, controllerName);
        }
    }

    private static void BuildAniamtionSl(string actPath,string controllerName)
    {
        object[] text = AssetDatabase.LoadAllAssetsAtPath(_imagePath);
        object[] textShadow = AssetDatabase.LoadAllAssetsAtPath(_shadowPath);
        List<AnimationClip> clips = new List<AnimationClip>();
        Sprite[] sprite = new Sprite[text.Length];
        Sprite[] spriteShadow = new Sprite[text.Length];

        for (int i = 0; i < text.Length; i++)
        {
            sprite[i] = text[i] as Sprite;
        }

        for (int i = 0; i < textShadow.Length; i++)
        {
            spriteShadow[i] = textShadow[i] as Sprite;
        }

        Dictionary<string, List<Sprite>> spDic = new Dictionary<string, List<Sprite>>();
        Dictionary<string, List<Sprite>> spShadowDic = new Dictionary<string, List<Sprite>>();

        for (int i = 0; i < conStr.Length; i++)
        {
            for (int j = 0; j < sprite.Length; j++)
            {
                if (sprite[j] == null)
                {
                    continue;
                }
                if (sprite[j].name.Contains(conStr[i]))
                {
                    if (spDic.ContainsKey(conStr[i]))
                    {
                        spDic[conStr[i]].Add(sprite[j]);
                    }
                    else
                    {
                        List<Sprite> list = new List<Sprite>();
                        list.Add(sprite[j]);
                        spDic.Add(conStr[i], list);
                    }
                }
            }

            for (int j = 0; j < spriteShadow.Length; j++)
            {
                if (spriteShadow[j] == null)
                    continue;
                if (spriteShadow[j].name.Contains(conStr[i]))
                {
                   
                    if (spShadowDic.ContainsKey(conStr[i]))
                    {
                        spShadowDic[conStr[i]].Add(spriteShadow[j]);
                    }
                    else
                    {
                        List<Sprite> list = new List<Sprite>();
                        list.Add(spriteShadow[j]);
                        spShadowDic.Add(conStr[i], list);
                    }
                }
            }
           
        }
        foreach (var item in spDic)
        {
            AnimationClip clip = new AnimationClip();
            EditorCurveBinding curveBinding = new EditorCurveBinding();
            curveBinding.type = typeof(SpriteRenderer);
            curveBinding.path = "";
            curveBinding.propertyName = "m_Sprite";
            ObjectReferenceKeyframe[] keyFrames = new ObjectReferenceKeyframe[item.Value.Count];
            ObjectReferenceKeyframe[] keyFrameShadows = new ObjectReferenceKeyframe[item.Value.Count];
            clip.frameRate = getZhen(item.Key);
            float frameTime = 1 / (float)clip.frameRate;
            for (int i = 0; i < item.Value.Count; i++)
            {
                Sprite sprite1 = item.Value[i];
                keyFrames[i] = new ObjectReferenceKeyframe();
                keyFrames[i].value = sprite1;

                if (spShadowDic[item.Key].Count > i)
                {
                    Sprite sprite2 = spShadowDic[item.Key][i];
                    keyFrameShadows[i] = new ObjectReferenceKeyframe();
                    keyFrameShadows[i].value = sprite2;
                }
                else
                {
                    Sprite sprite2 = spShadowDic[item.Key][0];
                    keyFrameShadows[i] = new ObjectReferenceKeyframe();
                    keyFrameShadows[i].value = sprite2;
                }
            }

            for (int i = 0; i < keyFrames.Length; i++)
            {
                for (int j = i; j < keyFrames.Length; j++)
                {
                    string s1 = (keyFrames[i].value.ToString());
                    string s2 = (keyFrames[j].value.ToString());
                    if (s1.Length > s2.Length)
                    {
                        ObjectReferenceKeyframe tmp = keyFrames[i];
                        keyFrames[i] = keyFrames[j];
                        keyFrames[j] = tmp;
                    }
                    else if (s1.CompareTo(s2) > 0)
                    {
                        ObjectReferenceKeyframe tmp = keyFrames[i];
                        keyFrames[i] = keyFrames[j];
                        keyFrames[j] = tmp;
                    }
                }
            }

            for (int i = 0; i < keyFrameShadows.Length; i++)
            {
                for (int j = i; j < keyFrameShadows.Length; j++)
                {
                    string s1 = (keyFrameShadows[i].value.ToString());
                    string s2 = (keyFrameShadows[j].value.ToString());
                    if (s1.Length > s2.Length)
                    {
                        ObjectReferenceKeyframe tmp = keyFrameShadows[i];
                        keyFrameShadows[i] = keyFrameShadows[j];
                        keyFrameShadows[j] = tmp;
                    }
                    else if (s1.CompareTo(s2) > 0)
                    {
                        ObjectReferenceKeyframe tmp = keyFrameShadows[i];
                        keyFrameShadows[i] = keyFrameShadows[j];
                        keyFrameShadows[j] = tmp;
                    }
                }
            }

            for (int i = 0; i < keyFrames.Length; i++)
            {
                keyFrames[i].time = frameTime * i;
                keyFrameShadows[i].time = frameTime * i;
            }


            if (item.Key.IndexOf("idle") >= 0)
            {
                SerializedObject serializedClip = new SerializedObject(clip);
                AnimationClipSettings clipSetting = new AnimationClipSettings(serializedClip.FindProperty("m_AnimationClipSettings"));
                clipSetting.loopTime = true;
                serializedClip.ApplyModifiedProperties();
            }
            EditorCurveBinding curveShadow = new EditorCurveBinding();
            curveShadow.type = typeof(SpriteRenderer);
            curveShadow.path = "shadow";
            curveShadow.propertyName = "m_Sprite";
            AnimationUtility.SetObjectReferenceCurve(clip, curveBinding, keyFrames);
            AnimationUtility.SetObjectReferenceCurve(clip, curveShadow, keyFrameShadows);
            int index = 0;
            for (int k = 0; k < conStr.Length; k++)
            {
                if (conStr[k].Contains(item.Key))
                {
                    index = k;
                }
            }
            AssetDatabase.CreateAsset(clip, actPath + _aniPath + "/" + item.Key + "@" + (index * 100).ToString() + ".anim");
            AssetDatabase.SaveAssets();

            AnimationClipCurveData[] data = AnimationUtility.GetAllCurves(clip);
            clips.Add(clip);
        }

        UnityEditor.Animations.AnimatorController controller = BuildAnimationController(clips, actPath, controllerName);

    }

    private static int getZhen(string zhen)
    {
        int result = 8;
        if (zhen.Equals(conStr[0]))
            result = _zhenIdle;
        if (zhen.Equals(conStr[1]))
            result = _zhenWalk;
        if (zhen.Equals(conStr[2]))
            result = _zhenAttack;
        if (zhen.Equals(conStr[3]))
            result = _zhenDead;
        if (zhen.Equals(conStr[4]))
            result = _zhenSkillA;
        if (zhen.Equals(conStr[5]))
            result = _zhenBorn;
        return result;

    }
    //static void BuildAniamtion()
    //{
    //    DirectoryInfo raw = new DirectoryInfo(ImagePath);
    //    foreach (DirectoryInfo dictorys in raw.GetDirectories())
    //    {
    //        List<AnimationClip> clips = new List<AnimationClip>();
    //        foreach (DirectoryInfo dictoryAnimations in dictorys.GetDirectories())
    //        {
    //            //每个文件夹就是一组帧动画，这里把每个文件夹下的所有图片生成出一个动画文件
    //            clips.Add(BuildAnimationClip(dictoryAnimations));
    //        }
    //        //把所有的动画文件生成在一个AnimationController里
    //        UnityEditor.Animations.AnimatorController controller = BuildAnimationController(clips, dictorys.Name);
    //        //最后生成程序用的Prefab文件
    //        BuildPrefab(dictorys, controller);
    //    }
    //}


    //static AnimationClip BuildAnimationClip(DirectoryInfo dictorys)
    //{
    //    string animationName = dictorys.Name;
    //    //查找所有图片，因为我找的测试动画是.jpg 
    //    FileInfo[] images = dictorys.GetFiles("*.jpg");
    //    AnimationClip clip = new AnimationClip();
    //    AnimationUtility.SetAnimationType(clip, ModelImporterAnimationType.Generic);
    //    EditorCurveBinding curveBinding = new EditorCurveBinding();
    //    curveBinding.type = typeof(SpriteRenderer);
    //    curveBinding.path = "";
    //    curveBinding.propertyName = "m_Sprite";
    //    ObjectReferenceKeyframe[] keyFrames = new ObjectReferenceKeyframe[images.Length];
    //    //动画长度是按秒为单位，1/10就表示1秒切10张图片，根据项目的情况可以自己调节
    //    float frameTime = 1 / 10f;
    //    for (int i = 0; i < images.Length; i++)
    //    {
    //        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(DataPathToAssetPath(images[i].FullName));
    //        keyFrames[i] = new ObjectReferenceKeyframe();
    //        keyFrames[i].time = frameTime * i;
    //        keyFrames[i].value = sprite;
    //    }
    //    //动画帧率，30比较合适
    //    clip.frameRate = 30;

    //    //有些动画我希望天生它就动画循环
    //    if (animationName.IndexOf("idle") >= 0)
    //    {
    //        //设置idle文件为循环动画
    //        SerializedObject serializedClip = new SerializedObject(clip);
    //        AnimationClipSettings clipSettings = new AnimationClipSettings(serializedClip.FindProperty("m_AnimationClipSettings"));
    //        clipSettings.loopTime = true;
    //        serializedClip.ApplyModifiedProperties();
    //    }
    //    string parentName = System.IO.Directory.GetParent(dictorys.FullName).Name;
    //    System.IO.Directory.CreateDirectory(AnimationPath + "/" + parentName);
    //    AnimationUtility.SetObjectReferenceCurve(clip, curveBinding, keyFrames);
    //    AssetDatabase.CreateAsset(clip, AnimationPath + "/" + parentName + "/" + animationName + ".anim");
    //    AssetDatabase.SaveAssets();
    //    return clip;
    //}

    static UnityEditor.Animations.AnimatorController BuildAnimationController(List<AnimationClip> clips, string path,string name)
    {
        UnityEditor.Animations.AnimatorController animatorController = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath(path + "/" + name + ".controller");
        animatorController.AddParameter("EVENT_ID",UnityEngine.AnimatorControllerParameterType.Int);
        UnityEngine.AnimatorControllerParameter param = animatorController.parameters[0];
        UnityEditor.Animations.AnimatorControllerLayer layer = animatorController.layers[0];
        UnityEditor.Animations.AnimatorStateMachine sm = layer.stateMachine;
        foreach (AnimationClip newClip in clips)
        {
            UnityEditor.Animations.AnimatorState state = sm.AddState(newClip.name.Split('@')[0]);
            state.motion = newClip;
            if (newClip.name.IndexOf("attack") >= 0)
            {

            }
            AnimatorStateTransition trans = sm.AddAnyStateTransition(state);
            trans.canTransitionToSelf = false;
            int key = int.Parse(newClip.name.Split('@')[1]);
            trans.AddCondition(AnimatorConditionMode.Equals, key, param.name);
            //state.SetAnimationClip(newClip, layer);
            //UnityEditor.Animations.AnimatorTransition trans = sm.AddAnyStateTransition(state);
            //trans.RemoveCondition(0);
        }
        AssetDatabase.SaveAssets();
        return animatorController;
    }

    static void BuildPrefab(DirectoryInfo dictorys, UnityEditor.Animations.AnimatorController animatorCountorller)
    {
        //生成Prefab 添加一张预览用的Sprite
        FileInfo images = dictorys.GetDirectories()[0].GetFiles("*.jpg")[0];
        GameObject go = new GameObject();
        go.name = dictorys.Name;
        SpriteRenderer spriteRender = go.AddComponent<SpriteRenderer>();
        spriteRender.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(DataPathToAssetPath(images.FullName));
        Animator animator = go.AddComponent<Animator>();
        animator.runtimeAnimatorController = animatorCountorller;
        PrefabUtility.CreatePrefab(PrefabPath + "/" + go.name + ".prefab", go);
        DestroyImmediate(go);
    }


    public static string DataPathToAssetPath(string path)
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
            return path.Substring(path.IndexOf("Assets\\"));
        else
            return path.Substring(path.IndexOf("Assets/"));
    }

    private string getPathEditor(string path)
    {
        Rect rect = EditorGUILayout.GetControlRect(GUILayout.Width(300));
        path = EditorGUI.TextField(rect, path);
        if ((Event.current.type == EventType.DragUpdated || Event.current.type == EventType.DragExited) && rect.Contains(Event.current.mousePosition)) {
            DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
            if (DragAndDrop.paths != null && DragAndDrop.paths.Length > 0)
            {
                path = DragAndDrop.paths[0];
            }
        }
        return path;
    }

    class AnimationClipSettings
    {
        SerializedProperty m_Property;

        private SerializedProperty Get(string property) { return m_Property.FindPropertyRelative(property); }

        public AnimationClipSettings(SerializedProperty prop) { m_Property = prop; }

        public float startTime { get { return Get("m_StartTime").floatValue; } set { Get("m_StartTime").floatValue = value; } }
        public float stopTime { get { return Get("m_StopTime").floatValue; } set { Get("m_StopTime").floatValue = value; } }
        public float orientationOffsetY { get { return Get("m_OrientationOffsetY").floatValue; } set { Get("m_OrientationOffsetY").floatValue = value; } }
        public float level { get { return Get("m_Level").floatValue; } set { Get("m_Level").floatValue = value; } }
        public float cycleOffset { get { return Get("m_CycleOffset").floatValue; } set { Get("m_CycleOffset").floatValue = value; } }

        public bool loopTime { get { return Get("m_LoopTime").boolValue; } set { Get("m_LoopTime").boolValue = value; } }
        public bool loopBlend { get { return Get("m_LoopBlend").boolValue; } set { Get("m_LoopBlend").boolValue = value; } }
        public bool loopBlendOrientation { get { return Get("m_LoopBlendOrientation").boolValue; } set { Get("m_LoopBlendOrientation").boolValue = value; } }
        public bool loopBlendPositionY { get { return Get("m_LoopBlendPositionY").boolValue; } set { Get("m_LoopBlendPositionY").boolValue = value; } }
        public bool loopBlendPositionXZ { get { return Get("m_LoopBlendPositionXZ").boolValue; } set { Get("m_LoopBlendPositionXZ").boolValue = value; } }
        public bool keepOriginalOrientation { get { return Get("m_KeepOriginalOrientation").boolValue; } set { Get("m_KeepOriginalOrientation").boolValue = value; } }
        public bool keepOriginalPositionY { get { return Get("m_KeepOriginalPositionY").boolValue; } set { Get("m_KeepOriginalPositionY").boolValue = value; } }
        public bool keepOriginalPositionXZ { get { return Get("m_KeepOriginalPositionXZ").boolValue; } set { Get("m_KeepOriginalPositionXZ").boolValue = value; } }
        public bool heightFromFeet { get { return Get("m_HeightFromFeet").boolValue; } set { Get("m_HeightFromFeet").boolValue = value; } }
        public bool mirror { get { return Get("m_Mirror").boolValue; } set { Get("m_Mirror").boolValue = value; } }
    }

}

public class CustomComparer : System.Collections.IComparer
{
    public int Compare(object x, object y)
    {
        string s1 = (string)x;
        string s2 = (string)y;
        if (s1.Length > s2.Length)
            return 1;
        if (s1.Length < s2.Length)
            return -1;
        for (int i = 0; i < s1.Length; i++)
        {
            if (s1[i] > s2[i])
                return 1;
            if(s1[i]<s2[i])
                return -1;
        }
        return 0;
    }
}
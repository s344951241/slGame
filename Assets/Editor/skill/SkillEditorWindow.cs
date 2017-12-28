using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class SkillEditorWindow:EditorWindow{
    [MenuItem("Game Tools/技能编辑器")]
    static void Init()
    {
        SkillEditorWindow window = (SkillEditorWindow)EditorWindow.GetWindow(typeof(SkillEditorWindow));
        window.Show();
        
        //.isSkillEditorOpen = true;
        Debug.Log("打开了");

    }

    public SkillEditorWindow()
    {
        GameTools.skillCallBack = delegate(int id)
        {
            if(_skillInfo._id==id)
                return;
            _skillInfo._id = id;
            LoadSkillInfo();
            Repaint();
        };
    }
    private int _rememberId = -1;
    private bool _warning;
    private SkillInfo _skillInfo = new SkillInfo();
    private int _EntityType;
    private int _ProtoId;
    private Vector2 _scrollPos;

    void OnGUI()
    {
        if(SkillEvent.isEditorItem)
        {
            SkillEvent.isEditorItem = false;
            SkillUpgradeEditorWindow ugWin = (SkillUpgradeEditorWindow)EditorWindow.GetWindow(typeof(SkillUpgradeEditorWindow));
            ugWin.Show();
        }
        EditorGUILayout.BeginVertical();
        EditorGUILayout.BeginHorizontal();
        //配置加载
        EditorGUILayout.BeginVertical();
        EditorGUILayout.BeginHorizontal();
        _skillInfo._id = EditorGUILayout.IntField("技能id",_skillInfo._id);
        _skillInfo._id = EditorTools.EditorPopup(_skillInfo._id,"AssetsLibrary/Config/skill","*.bytes","skillid");
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        if(GUILayout.Button("加载配置"))
        {
            LoadSkillInfo();
        }
        if(GUILayout.Button("打开"))
        {
            OpenSkillBytes();
        }
        if(GUILayout.Button("刷新"))
        {
            EditorTools.Refresh();
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space();
        //敌人
        EditorGUILayout.Space();
        EditorGUILayout.BeginVertical();
        EditorGUILayout.BeginHorizontal();
        _EntityType = EditorGUILayout.IntField("类型",_EntityType);
        _ProtoId = EditorGUILayout.IntField("protoid",_ProtoId);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        if(GUILayout.Button("加载敌人"))
        {
            LoadEnemy(_EntityType,_ProtoId);
        }
        if(GUILayout.Button("清除敌人"))
        {
            ClearEnemy();
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
        //技能编辑区
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
        _skillInfo.DrawUI();
        EditorGUILayout.EndScrollView();

        //保存
        _warning = _skillInfo._id!=_rememberId;
        string save = _warning?"！！！！！！警告":"";
        GUI.backgroundColor = _warning?Color.red:Color.white;
        if(GUILayout.Button(save+"保存并播放"))
        {
            SaveAndPlay();
            _warning = false;
        }
        if(GUILayout.Button(save+"修改节点时使用"))
        {
            SaveAllConfig();
            _warning = false;
        }
        if(GUILayout.Button("合并配置"))
        {
            CombineFile();
            _warning = false;
        }
        EditorGUILayout.EndVertical();
    }

    private void OpenSkillBytes()
    {
        try{
            System.Diagnostics.Process.Start(Application.dataPath+"/AssetsLibrary/Config/skill/");
        }
        catch{}
    }
    private void LoadSkillInfo()
    {
        SkillInfo info = null;
        try{
            Stream fs = File.Open("Assets/AssetsLibrary/Config/skill/"+_skillInfo._id+".bytes",FileMode.Open);
            BinaryReader br = new BinaryReader(fs);
            info = new SkillInfo();
            info.Read(br);
            br.Close();
            fs.Close();
        }
        catch{}
        if(info!=null)
        {
            _skillInfo = info;
            _rememberId = _skillInfo._id;
        }
    }
    private void SaveAndPlay()
    {
        if(_warning)
        {
            if(!EditorUtility.DisplayDialog("警告","目前的配置("+_rememberId+")可能覆盖掉配置("+_skillInfo._id+"),你确定这样做吗？","是的","不"))
            {
                return;
            }
        }
        if(_skillInfo._id!=0)
        {
            string path = "Assets/AssetsLibrary/Config/skill/"+_skillInfo._id+".bytes";
            SaveConfig(_skillInfo,path);
            _rememberId = _skillInfo._id;
            if(Application.isPlaying&&SkillInfoModel.Instance!=null)
            {
                SkillInfoModel.Instance.ResaveSkillInfo(_skillInfo);
            }
        }
    }

    private void SaveConfig(SkillInfo info,string path)
    {
        FileStream fs = File.Open(path,FileMode.Create);
        info.Write(fs);
        fs.Close();
    }
    private void SaveAllConfig()
    {
        if(Application.isPlaying)
        {
            string cfgPath = "Assets/AssetsLibrary/Config/skill/";
            Dictionary<int,SkillInfo> dic = new Dictionary<int,SkillInfo>();
            foreach(KeyValuePair<int,SkillInfo> kvp in dic)
            {
                SkillInfo info = kvp.Value;
                string path = cfgPath+info._id+".bytes";
                SaveConfig(info,path);
            }
        }
    }
    private void CombineFile()
    {
        string path = "Assets/Resources/GameAssets/Configs/skill/skillcfg.bytes";
        FileStream fs = File.Open(path,FileMode.Create);
        string cfgPath = "Assets/AssetsLibrary/Config/skill";
        CombieCfg(fs,cfgPath);
        fs.Close();
    }
    private void CombieCfg(FileStream fs,string path)
    {
        byte [] buffer;
        string [] paths = Directory.GetFiles(path);
        int count = paths.Length;
        int total = 0;
        for(int i=0;i<count;i++)
        {
            string url = paths[i];
            if(url.IndexOf(".meta")==-1)
            {
                total++;
            }
        }
        fs.WriteByte((byte)total);
        for(int i=0;i<count;i++)
        {
            string url = paths[i];
            if(url.IndexOf(".meta")==-1)
            {
                FileStream ufs = File.Open(url,FileMode.Open);
                long len = ufs.Length;
                buffer = new byte[len];
                int n = ufs.Read(buffer,0,(int)len);
                fs.Write(buffer,0,n);
                ufs.Close();
            }
        }
        System.Diagnostics.Process.Start(Application.dataPath+"/Resources/GameAssets/Configs/skill");
    }

    private void LoadEnemy(int type,int protoid)
    {

    }

    private void ClearEnemy()
    {

    }
    public void Destory()
    {
        Debug.Log("关闭了");
    }
}
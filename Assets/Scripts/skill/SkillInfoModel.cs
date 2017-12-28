using Engine;
using System;
using System.Collections.Generic;
using System.IO;

public class SkillInfoModel : Singleton<SkillInfoModel>
{
    //
    // Fields
    //
    public Dictionary<int, SkillInfo> _dicSkill;

    //
    // Constructors
    //
    public SkillInfoModel()
    {
        this._dicSkill = new Dictionary<int, SkillInfo>();
        this.ParseConfig("skillcfg", string.Empty);
    }

    //
    // Methods
    //
    public Dictionary<int, SkillInfo> GetAllConfig()
    {
        return this._dicSkill;
    }

    public SkillInfo GetSkillInfo(int id)
    {
        return this._dicSkill.GetValue(id);
    }

    public void InitSkillConfig(BinaryReader br)
    {
        SkillInfo skillInfo = new SkillInfo();
        skillInfo.Read(br);
        this._dicSkill[skillInfo._id] = skillInfo;
    }

    public void ParseConfig(string tableName, string errorTips)
    {
        byte[] bytes = Singleton<DataManager>.Instance.GetBytes(tableName);
        if (bytes != null)
        {
            MemoryStream input = new MemoryStream(bytes);
            BinaryReader binaryReader = new BinaryReader(input);
            int num = (int)binaryReader.ReadByte();
            for (int i = 0; i < num; i++)
            {
                this.InitSkillConfig(binaryReader);
            }
        }
    }

    public void ResaveSkillInfo(SkillInfo info)
    {
        this._dicSkill[info._id] = info;
    }
}

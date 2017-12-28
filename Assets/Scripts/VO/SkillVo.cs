using System;

[Serializable]
public class SkillVo : IConfig<string>
{
    //
    // Fields
    //
    public string key;

    public int id;

    public string name;

    public int atktype;

    public int preparetime;

    public int cd;

    public int damage;

    public int range;

    public int speed;

    public int type;

    public int damage_num;

    public int time;

    //
    // Methods
    //
    public string GetKey()
    {
        return this.key;
    }
}

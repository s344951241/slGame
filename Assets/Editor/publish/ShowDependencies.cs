using System;

public class SkillModel : BaseModel
{
    //
    // Constructors
    //
    public SkillModel()
    {
        base.InitData<SkillVo>();
    }

    //
    // Methods
    //
    public SkillVo GetVo(string key)
    {
        return base.__GetVo<SkillVo>(key);
    }
}

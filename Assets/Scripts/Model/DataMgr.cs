using System;

public class DataMgr
{
    //
    // Static Fields
    //
    //public static LoginModel _loginModel;

    //public static RoleModel _roleModel;

    public static SkillModel _skillModel;

    //
    // Static Properties
    //
    //public static LoginModel loginModel
    //{
    //    get
    //    {
    //        LoginModel arg_17_0;
    //        if ((arg_17_0 = DataMgr._loginModel) == null)
    //        {
    //            arg_17_0 = (DataMgr._loginModel = new LoginModel());
    //        }
    //        return arg_17_0;
    //    }
    //}

    //public static RoleModel roleModel
    //{
    //    get
    //    {
    //        RoleModel arg_17_0;
    //        if ((arg_17_0 = DataMgr._roleModel) == null)
    //        {
    //            arg_17_0 = (DataMgr._roleModel = new RoleModel());
    //        }
    //        return arg_17_0;
    //    }
    //}

    public static SkillModel skillModel
    {
        get
        {
            SkillModel arg_17_0;
            if ((arg_17_0 = DataMgr._skillModel) == null)
            {
                arg_17_0 = (DataMgr._skillModel = new SkillModel());
            }
            return arg_17_0;
        }
    }
}

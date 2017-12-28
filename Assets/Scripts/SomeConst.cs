using UnityEngine;
using System.Collections;

public class SomeConst {

}
public enum CONST_ENTITY_TYPE
{
    ENTITY_MOVEABLE,
    ENTITY_NOMOVE
}
public class CONST_EVENT_ID
{
    //
    // Static Fields
    //
    public const int DEFAUT = 0;

    public const int WALK = 100;

    public const int ATTACK = 200;

    public const int DEAD = 300;
}

public struct CONST_EVENT_NAME
{
    //
    // Static Fields
    //
    public const string UP = "EVENT_ID_UP";

    public const string DOWN = "EVENT_ID_DOWN";

    public const string ALL = "EVENT_ID_ALL";

    public const string FOOT_DIR_X = "FOOT_DIR_X";

    public const string FOOT_DIR_Y = "FOOT_DIR_Y";
}

public class CONST_LAYER
{
    //
    // Static Fields
    //
    public const int UP = 1;

    public const int DOWN = 2;

    public const int ALL = 3;
}

public struct CONST_MOVE_MODE
{
    //
    // Static Fields
    //
    public const int NONE = 0;

    public const int MOVE = 1;
}

public class CONST_SITUATION
{
    //
    // Static Fields
    //
    public const int DEFAULT = 0;

    public const int CITY_NORMAL = 1;

    public const int BATTLE_NORMAL = 2;

    public const int BATTLE_WEAK = 3;

    public const int BATTLE_ENGAGE = 4;
}


public static class CAMP_CONST
{
    //
    // Static Fields
    //
    public static int SELF = -1;

    public static int ENEMY = 1;

    //
    // Static Methods
    //
    public static void setCamp(int self, int enemy)
    {
        CAMP_CONST.SELF = self;
        CAMP_CONST.ENEMY = enemy;
    }
}
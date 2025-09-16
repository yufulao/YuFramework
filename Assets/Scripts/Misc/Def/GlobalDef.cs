// ******************************************************************
//@file         GlobalDef.cs
//@brief        全局定义类
//@author       yufulao, yufulao@qq.com
//@createTime   2024.05.18 01:33:09
// ******************************************************************

namespace Yu
{
    public static partial class GlobalDef
    {
        //存储文件名
        public const string Cfg = "Cfg.json";
        public const string Global = "Global.json";
        public const string SaveGameStructName = "SaveGameStruct"; //存档的数据结构key

        public const string SaveGameFold = "SaveGame";
        //public const string LastSaveGameIndex = "LastSaveGame";//最新的存档key

        public const string CurrentLanguageKey = "CurrentLanguageKey"; //当前语言key
        public const string InventoryDic = "InventoryDic"; //背包管理器key
        public const string PropertyContainerDic = "PropertyContainerDic"; //属性容器管理器key
        public const string GameTime = "GameTime"; //存档的游戏时间key
        public const string GameTimeScale = "GameTimeScale"; //存档的游戏时间速度
        public const string SaveGameScene = "SaveGameScene"; //存档的场景key
        public const string SaveGameCharacterPosition = "SaveGameCharacterPosition"; //存档的角色位置
        public const string SaveGameItemState = "SaveGameItemState"; //场景保存的物体状态key
        public const string PlayerInventory = "PlayerInventory"; //角色的背包id
        public const string InspirationInventory = "InspirationInventory"; //角色的灵感素材背包id
        public const string FoodInventory = "FoodInventory"; //食物背包id
        public const string CharacterPropertyContainer = "CharacterPropertyContainer"; //角色的属性容器id
        public const string CharacterBuffContainer = "CharacterBuffContainer"; //角色的buff容器key
        public const string CharacterDiseaseContainer = "CharacterDiseaseContainer"; //角色的疾病容器key
        public const string CreateData = "CreateData"; //创作数据key
        public const string HungryStartTime = "HungryStartTime"; //饥饿开始时间key
    }
}
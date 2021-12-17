using UnityEngine;
//###########################################################################################################
//===========================================================================================================
//##                               キャラクターのステータス情報をまとめるクラス                            ##
//===========================================================================================================
//###########################################################################################################
public class CharacterStatus
{
    public float Speed { get; set; }
    public float Jump_power { get; set; }
    public float Knockback_weight { get; set; }
    public int Touch_damage { get; set; }
    public int Max_HP { get; set; }
    public int Max_MP { get; set; }
    public int Score { get; set; }

    public string Name { get; set; }

    public string Comment { get; set; }

    public CharacterStatus(
        float speed,
        float jump_power,
        float knockback_weight,
        int touch_damage,
        int max_hp,
        int max_mp,
        int score = 0,
        string name = "",
        string comment = ""
        )
    {
        this.Speed = speed;
        this.Jump_power = jump_power;
        this.Knockback_weight = knockback_weight;
        this.Touch_damage = touch_damage;
        this.Max_HP = max_hp;
        this.Max_MP = max_mp;
        this.Score = score;
        this.Name = name;
        this.Comment = comment;
    }
}

//###########################################################################################################
//===========================================================================================================
//##                                    アイテムの基礎データをまとめるクラス                               ##
//===========================================================================================================
//###########################################################################################################
public class ItemData 
{
    // アイテムID
    public readonly int item_id;

    // 最大所持数
    public readonly int max_hold;

    // アイテム名
    public readonly string item_name;

    // ミセで買うときのアイテムの値段
    public readonly int price;

    // アイテムの説明
    public readonly string item_comment;

    // prefabの名前
    public readonly string prefab_name;

    public ItemData(int item_id,int max_hold,string item_name = "なまえ",int price = 0,string item_comment="てすと",string prefab_name = "")     
    {
        this.item_id = item_id;
        this.max_hold = max_hold;
        this.item_name = item_name;
        this.price = price;
        this.item_comment = item_comment;
        this.prefab_name = prefab_name;

    }
}

//###########################################################################################################
//===========================================================================================================
//##                                    ステージの基礎データをまとめるクラス                               ##
//===========================================================================================================
//###########################################################################################################
public class StageData 
{
    // ステージID
    public readonly int stage_id;
    // ステージ名
    public readonly string stage_name;
    // ステージの説明文
    public readonly string stage_comment;
    // prefabの名前
    public readonly string prefab_name;

    public StageData(int stage_id, string stage_name = "なまえ", string stage_comment = "", string prefab_name = "")
    {
        this.stage_id = stage_id;
        this.stage_name = stage_name;
        this.stage_comment = stage_comment;
        this.prefab_name = prefab_name;
    }
}

//###########################################################################################################
//===========================================================================================================
//##                                 敵キャラクターの基礎データをまとめるクラス                            ##
//===========================================================================================================
//###########################################################################################################
public class EnemyData 
{
    // キャラクターID
    public readonly int enemy_id;
    // prefabの名前
    public readonly string prefab_name;

    // 基礎キャラクターデータ
    public readonly CharacterStatus status;

    // 出現開始ウェーブ数
    public readonly int appear_wave;
    // 出現頻度 (スポーン判定何回ごとにスポーンさせるか)
    public readonly int spawn_interval;

    public EnemyData(int enemy_id,string prefab_name,CharacterStatus status,int appear_wave = 0,int spawn_interval = 1) 
    {
        this.enemy_id = enemy_id;
        this.prefab_name = prefab_name;
        this.status = status;
        this.appear_wave = appear_wave;
        this.spawn_interval = spawn_interval;
    }


}

//###########################################################################################################
//===========================================================================================================
//##                      後から変更されない固有値とゲームデータを定義する静的クラス                       ##
//===========================================================================================================
//###########################################################################################################
public static class EigenValue
{
    // 各種prefabのディレクトリの位置
    public static readonly string PREFAB_DIRECTORY_EFFECTS = "Prefabs/Effects/";
    public static readonly string PREFAB_DIRECTORY_ITEMS = "Prefabs/Items/";
    public static readonly string PREFAB_DIRECTORY_ENEMYS = "Prefabs/Enemys/";
    public static readonly string PREFAB_DIRECTORY_PLAYERS = "Prefabs/Players/";
    public static readonly string PREFAB_DIRECTORY_STAGES = "Prefabs/Stages/";
    public static readonly string PREFAB_DIRECTORY_UIS = "Prefabs/UIs/";

    // セーブデータ、オプションデータの位置
    public static readonly string SAVEDATA_PATH_InEDITOR = "SaveData/";
    public static string SAVEDATA_PATH_InAPP() {return Application.persistentDataPath + "/SaveData/"; }
    // オプションデータのファイル名
    public static readonly string OPTIONDATA_PATH_NAME = "OptionData";
    public static readonly string JOYSTICKDATA_PATH_NAME = "JoystickData";

    // 実装数
    public static readonly int IMPLEMENTED_CHARACTERS = 2;
    public static readonly int IMPLEMENTED_ENEMYS = 6;
    public static readonly int IMPLEMENTED_STAGES = 2;
    public static readonly int IMPLEMENTED_MODES = 3;

    // 各モード(上限があるモードのみ)の最大ウェーブ数
    public static readonly int MODE_000_MAX_WAVE = 100;
    public static readonly int MODE_001_MAX_WAVE = 10;
    
    // 透過カラーの固定値　(死亡キャラの透過オプションで使用)
    public static readonly Color TRANSPARENT_COLOR = new Color(0.75f, 0.75f, 0.75f, 0.5f);

    //###########################################################################################################
    //###########################################################################################################
    //##                                                各種固有値                                             ##
    //###########################################################################################################
    //###########################################################################################################
    #region eigenvalue
    // 難易度上昇係数
    // この分のウェーブを跨ぐごとに難易度が上がる(スポーン数の増加とか)
    public static readonly int DIFFICULTY_RANK_UP_COF = 10;
    // 各ウェーブのベースの時間(ウェーブ１の時の時間)
    public static readonly int WAVE_BASE_TIME = 30;
    // 敵キャラのスポーン感覚
    public static readonly float SPAWN_INTERVAL = 1f;
    #endregion

    //###########################################################################################################
    //###########################################################################################################
    //##                                              アイテムデータ                                           ##
    //###########################################################################################################
    //###########################################################################################################
    #region itemdata
    // [1] HPポーション
    public static readonly ItemData ITEM_HP_POTION = new ItemData(
        item_id: 1,
        max_hold: 10,
        item_name: "ＨＰポーション",
        price: 200,
        item_comment: "さわやかな色で飲みやすそう。ＨＰが５回復する。",
        prefab_name: "HP_Potion");
    // [2] MPポーション
    public static readonly ItemData ITEM_MP_POTION = new ItemData(
        item_id: 2,
        max_hold: 10,
        item_name: "ＭＰポーション",
        price: 300,
        item_comment: "アンシンとシンライのゲテモノ色。ＭＰが５回復する。",
        prefab_name: "MP_Potion");
    // [3] 体力祈願のおまもり
    public static readonly ItemData ITEM_HP_AMULET = new ItemData(
        item_id: 3,
        max_hold: 5,
        item_name: "体力祈願のおまもり",
        price: 10000,
        item_comment: "ハートの形をしたおまもり。ってなんだろう。持っていると最大ＨＰが２増加する。",
        prefab_name: "HP_Amulet");
    // [4] 魔力祈願のおまもり
    public static readonly ItemData ITEM_MP_AMULET = new ItemData(
        item_id: 4,
        max_hold: 5,
        item_name: "魔力祈願のおまもり",
        price: 10000,
        item_comment: "炎の形をしたおまもり。すごく強くなれそう。持っていると最大ＭＰが２増加する。",
        prefab_name: "MP_Amulet");
    // [5] 砥石
    //public static readonly int ITEM_ID_WHETSTONE = 5;
    // [6] (空き)(防御アップアイテムを実装予定)
    //public static readonly int ITEM_ID_ = 6;
    // [7] ブーメラン
    public static readonly ItemData ITEM_BOOMERANG = new ItemData(
        item_id: 7,
        max_hold: 5,
        item_name: "ブーメラン",
        price: 200,
        item_comment: "くるくるくる～って投げて使う。キャッチすることで何度も使える。でも、たまに戻ってこない。",
        prefab_name: "Boomerang");
    // [8] 爆弾
    public static readonly ItemData ITEM_BOMB = new ItemData(
        item_id: 8,
        max_hold: 10,
        item_name: "ばくだん",
        price: 100,
        item_comment: "ぶんなげて、しばらくしたら、どっか～ん。なお、水と火にはご用心。",
        prefab_name: "Bomb");
    // [9] 銃
    //public static readonly int ITEM_ID_GUN = 9;
    // [10] 弾薬
    //public static readonly int ITEM_ID_BULLETS = 10;
    #endregion
    //###########################################################################################################
    //###########################################################################################################
    //##                                             プレイヤーデータ                                          ##
    //###########################################################################################################
    //###########################################################################################################
    #region playerdata
    public static readonly CharacterStatus STATUS_PLAYER_001_KNIGHT = new CharacterStatus(
            speed: 80,
            jump_power: 2.5f,
            knockback_weight: 20,
            touch_damage: 0,
            max_hp: 10,
            max_mp: 10,
            name:"けんし"
            );
    public static readonly CharacterStatus STATUS_PLAYER_002_MAGE = new CharacterStatus(
            speed: 70,
            jump_power: 2.0f,
            knockback_weight: 20,
            touch_damage: 0,
            max_hp: 10,
            max_mp: 20,
            name:"まどうし"
        );
    #endregion
    //###########################################################################################################
    //###########################################################################################################
    //##                                        敵キャラクターの基礎データ                                     ##
    //###########################################################################################################
    //###########################################################################################################
    #region enemydata
    // [1] ゾンビ
    public static readonly EnemyData STATUS_000_ZOMBIE = new EnemyData(
        enemy_id: 0,
        prefab_name:"Zombie",
        new CharacterStatus(
            speed: 15,
            jump_power: 0,
            knockback_weight: 50,
            touch_damage: 1,
            max_hp: 20,
            max_mp: 0,
            score: 2,
            name: "ゾンビ"),
        appear_wave: 0,
        spawn_interval: 4
        );

    // [2] スケルトン
    public static readonly EnemyData STATUS_001_SKELETON = new EnemyData(
        enemy_id: 1,
        prefab_name:"Skeleton",
        new CharacterStatus(
            speed: 20,
            jump_power: 0,
            knockback_weight: 0,
            touch_damage: 0,
            max_hp: 15,
            max_mp: 0,
            score: 1,
            name: "スケルトン"),
         appear_wave: 3,
         spawn_interval: 7
        );
    // [3] ゴースト
    public static readonly EnemyData STATUS_002_GHOST = new EnemyData(
        enemy_id: 2,
        prefab_name:"Ghost",
        new CharacterStatus(
            speed: 10,
            jump_power: 0,
            knockback_weight: 200,
            touch_damage: 1,
            max_hp: 12,
            max_mp: 0,
            score: 1,
            name: "ゴースト"),
         appear_wave: 6,
         spawn_interval: 9
        );
    // [4] スライム
    public static readonly EnemyData STATUS_003_SLIME = new EnemyData(
    enemy_id: 3,
    prefab_name: "Slime",
    new CharacterStatus(
        speed: 60,
        jump_power: 2.5f,
        knockback_weight: 200,
        touch_damage: 1,
        max_hp: 7,
        max_mp: 0,
        score: 1,
        name: "スライム"),
     appear_wave: 10,
     spawn_interval: 13
    );
    // [5] ウィッチ
    public static readonly EnemyData STATUS_004_WITCH = new EnemyData(
    enemy_id: 4,
    prefab_name: "Witch",
    new CharacterStatus(
        speed: 60,
        jump_power: 2.5f,
        knockback_weight: 200,
        touch_damage: 0,
        max_hp: 30,
        max_mp: 0,
        score: 2,
        name: "ウィッチ"),
     appear_wave: 20,
     spawn_interval: 40
    );
    public static readonly EnemyData STATUS_005_GOLEM = new EnemyData(
    enemy_id: 5,
    prefab_name: "Golem",
    new CharacterStatus(
    speed: 10,
    jump_power: 0f,
    knockback_weight: 200,
    touch_damage: 2,
    max_hp: 150,
    max_mp: 0,
    score: 6,
    name: "ゴーレム"),
    appear_wave: 35,
    spawn_interval: 70
    );
    #endregion

    //###########################################################################################################
    //###########################################################################################################
    //##                                              ステージデータ                                           ##
    //###########################################################################################################
    //###########################################################################################################
    #region stagedata
    public static readonly StageData STATUS_STAGE_001_VILLAGE = new StageData(
            stage_id: 1,
            stage_name:"ムラ", 
            stage_comment: "もりに囲まれたムラ。いせきのようなものがある。特別なことは何もない、ただのムラ。ちなみに、住民はミセの主だけらしい。",
            prefab_name:""
        );
    public static readonly StageData STATUS_STAGE_002_FOREST = new StageData(
        stage_id: 2,
        stage_name: "モリ",
        stage_comment: "うっそうとしたモリ。大きな木には、テキもプレイヤーもかくれてしまう。起伏がはげしく、移動するのもたいへん。",
        prefab_name: ""
    );



    #endregion

    // アイテムデータをIDから取得する
    public static ItemData GetItemData(int item_id) 
    {
        return item_id switch
        {
            1 => ITEM_HP_POTION,
            2 => ITEM_MP_POTION,
            3 => ITEM_HP_AMULET,
            4 => ITEM_MP_AMULET,
            7 => ITEM_BOOMERANG,
            8 => ITEM_BOMB,
            _ => null,
        };
    }
    public static ItemData GetItemData(string prefab_name)
    {
        return prefab_name switch
        {
            "HP_Potion" => ITEM_HP_POTION,
            "MP_Potion" => ITEM_MP_POTION,
            "HP_Amulet" => ITEM_HP_AMULET,
            "MP_Amulet" => ITEM_MP_AMULET,
            "Boomerang" => ITEM_BOOMERANG,
            "Bomb" => ITEM_BOMB,
            _ => null,
        };
    }

    // プレイヤーデータをIDから取得する
    public static CharacterStatus GetCharacterStatus(int chara_id) 
    {
        return chara_id switch
        {
            1 => STATUS_PLAYER_001_KNIGHT,
            2 => STATUS_PLAYER_002_MAGE,
            _ => null,
        };
    }
    // 敵データをIDから取得する
    public static EnemyData GetEnemyData(int enemy_id)
    {
        return enemy_id switch
        {
            0 => STATUS_000_ZOMBIE,
            1 => STATUS_001_SKELETON,
            2 => STATUS_002_GHOST,
            3 => STATUS_003_SLIME,
            4 => STATUS_004_WITCH,
            5 => STATUS_005_GOLEM,
            _ => null,
        };
    }

    // ステージデータをIDから取得する
    public static StageData GetStageData(int stage_id) 
    {
        return stage_id switch
        {
            1 => STATUS_STAGE_001_VILLAGE,
            2 => STATUS_STAGE_002_FOREST,
            _ => null,
        };
    }

    public static int GetModeMaxWave(int mode_id) 
    {
        return mode_id switch
        {
            0 => MODE_000_MAX_WAVE,
            1 => MODE_001_MAX_WAVE,
            2 => 9999999,
            _ => 0,
        };
    }

    public static Sprite GetCharacterIcon_Sprite(int chara_id) 
    {
        // スプライトリストを統括するprefabを読み込む
        SpriteDatas spriteDatas = Resources.Load<GameObject>(("Prefabs/Sprite_lists")).GetComponent<SpriteDatas>();
        // リストから該当のスプライトを返す
        return spriteDatas.Character_icons[chara_id - 1];
    }
    public static Sprite GetStageIcon_Sprite(int stage_id)
    {
        // スプライトリストを統括するprefabを読み込む
        SpriteDatas spriteDatas = Resources.Load<GameObject>(("Prefabs/Sprite_lists")).GetComponent<SpriteDatas>();
        // リストから該当のスプライトを返す
        return spriteDatas.Stage_icons[stage_id - 1];
    }
    public static Sprite GetModeIcon_Sprite(int mode_id)
    {
        // スプライトリストを統括するprefabを読み込む
        SpriteDatas spriteDatas = Resources.Load<GameObject>(("Prefabs/Sprite_lists")).GetComponent<SpriteDatas>();
        // リストから該当のスプライトを返す
        return spriteDatas.Mode_icons[mode_id];
    }
    public static Sprite GetModeItem_Sprite(int item_id)
    {
        // スプライトリストを統括するprefabを読み込む
        SpriteDatas spriteDatas = Resources.Load<GameObject>(("Prefabs/Sprite_lists")).GetComponent<SpriteDatas>();
        // リストから該当のスプライトを返す
        return spriteDatas.Mode_icons[item_id - 1];
    }
    public static string GetSaveDataPath() 
    {
        if (Application.isEditor)
            return SAVEDATA_PATH_InEDITOR;
        else
            return SAVEDATA_PATH_InAPP();
    }
}

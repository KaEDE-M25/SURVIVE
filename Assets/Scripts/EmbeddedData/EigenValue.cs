using UnityEngine;
//###########################################################################################################
//===========================================================================================================
//##                               �L�����N�^�[�̃X�e�[�^�X�����܂Ƃ߂�N���X                            ##
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
//##                                    �A�C�e���̊�b�f�[�^���܂Ƃ߂�N���X                               ##
//===========================================================================================================
//###########################################################################################################
public class ItemData 
{
    // �A�C�e��ID
    public readonly int item_id;

    // �ő及����
    public readonly int max_hold;

    // �A�C�e����
    public readonly string item_name;

    // �~�Z�Ŕ����Ƃ��̃A�C�e���̒l�i
    public readonly int price;

    // �A�C�e���̐���
    public readonly string item_comment;

    // prefab�̖��O
    public readonly string prefab_name;

    public ItemData(int item_id,int max_hold,string item_name = "�Ȃ܂�",int price = 0,string item_comment="�Ă���",string prefab_name = "")     
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
//##                                    �X�e�[�W�̊�b�f�[�^���܂Ƃ߂�N���X                               ##
//===========================================================================================================
//###########################################################################################################
public class StageData 
{
    // �X�e�[�WID
    public readonly int stage_id;
    // �X�e�[�W��
    public readonly string stage_name;
    // �X�e�[�W�̐�����
    public readonly string stage_comment;
    // prefab�̖��O
    public readonly string prefab_name;

    public StageData(int stage_id, string stage_name = "�Ȃ܂�", string stage_comment = "", string prefab_name = "")
    {
        this.stage_id = stage_id;
        this.stage_name = stage_name;
        this.stage_comment = stage_comment;
        this.prefab_name = prefab_name;
    }
}

//###########################################################################################################
//===========================================================================================================
//##                                 �G�L�����N�^�[�̊�b�f�[�^���܂Ƃ߂�N���X                            ##
//===========================================================================================================
//###########################################################################################################
public class EnemyData 
{
    // �L�����N�^�[ID
    public readonly int enemy_id;
    // prefab�̖��O
    public readonly string prefab_name;

    // ��b�L�����N�^�[�f�[�^
    public readonly CharacterStatus status;

    // �o���J�n�E�F�[�u��
    public readonly int appear_wave;
    // �o���p�x (�X�|�[�����艽�񂲂ƂɃX�|�[�������邩)
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
//##                      �ォ��ύX����Ȃ��ŗL�l�ƃQ�[���f�[�^���`����ÓI�N���X                       ##
//===========================================================================================================
//###########################################################################################################
public static class EigenValue
{
    // �e��prefab�̃f�B���N�g���̈ʒu
    public static readonly string PREFAB_DIRECTORY_EFFECTS = "Prefabs/Effects/";
    public static readonly string PREFAB_DIRECTORY_ITEMS = "Prefabs/Items/";
    public static readonly string PREFAB_DIRECTORY_ENEMYS = "Prefabs/Enemys/";
    public static readonly string PREFAB_DIRECTORY_PLAYERS = "Prefabs/Players/";
    public static readonly string PREFAB_DIRECTORY_STAGES = "Prefabs/Stages/";
    public static readonly string PREFAB_DIRECTORY_UIS = "Prefabs/UIs/";

    // �Z�[�u�f�[�^�A�I�v�V�����f�[�^�̈ʒu
    public static readonly string SAVEDATA_PATH_InEDITOR = "SaveData/";
    public static string SAVEDATA_PATH_InAPP() {return Application.persistentDataPath + "/SaveData/"; }
    // �I�v�V�����f�[�^�̃t�@�C����
    public static readonly string OPTIONDATA_PATH_NAME = "OptionData";
    public static readonly string JOYSTICKDATA_PATH_NAME = "JoystickData";

    // ������
    public static readonly int IMPLEMENTED_CHARACTERS = 2;
    public static readonly int IMPLEMENTED_ENEMYS = 6;
    public static readonly int IMPLEMENTED_STAGES = 2;
    public static readonly int IMPLEMENTED_MODES = 3;

    // �e���[�h(��������郂�[�h�̂�)�̍ő�E�F�[�u��
    public static readonly int MODE_000_MAX_WAVE = 100;
    public static readonly int MODE_001_MAX_WAVE = 10;
    
    // ���߃J���[�̌Œ�l�@(���S�L�����̓��߃I�v�V�����Ŏg�p)
    public static readonly Color TRANSPARENT_COLOR = new Color(0.75f, 0.75f, 0.75f, 0.5f);

    //###########################################################################################################
    //###########################################################################################################
    //##                                                �e��ŗL�l                                             ##
    //###########################################################################################################
    //###########################################################################################################
    #region eigenvalue
    // ��Փx�㏸�W��
    // ���̕��̃E�F�[�u���ׂ����Ƃɓ�Փx���オ��(�X�|�[�����̑����Ƃ�)
    public static readonly int DIFFICULTY_RANK_UP_COF = 10;
    // �e�E�F�[�u�̃x�[�X�̎���(�E�F�[�u�P�̎��̎���)
    public static readonly int WAVE_BASE_TIME = 30;
    // �G�L�����̃X�|�[�����o
    public static readonly float SPAWN_INTERVAL = 1f;
    #endregion

    //###########################################################################################################
    //###########################################################################################################
    //##                                              �A�C�e���f�[�^                                           ##
    //###########################################################################################################
    //###########################################################################################################
    #region itemdata
    // [1] HP�|�[�V����
    public static readonly ItemData ITEM_HP_POTION = new ItemData(
        item_id: 1,
        max_hold: 10,
        item_name: "�g�o�|�[�V����",
        price: 200,
        item_comment: "����₩�ȐF�ň��݂₷�����B�g�o���T�񕜂���B",
        prefab_name: "HP_Potion");
    // [2] MP�|�[�V����
    public static readonly ItemData ITEM_MP_POTION = new ItemData(
        item_id: 2,
        max_hold: 10,
        item_name: "�l�o�|�[�V����",
        price: 300,
        item_comment: "�A���V���ƃV�����C�̃Q�e���m�F�B�l�o���T�񕜂���B",
        prefab_name: "MP_Potion");
    // [3] �̗͋F��̂��܂���
    public static readonly ItemData ITEM_HP_AMULET = new ItemData(
        item_id: 3,
        max_hold: 5,
        item_name: "�̗͋F��̂��܂���",
        price: 10000,
        item_comment: "�n�[�g�̌`���������܂���B���ĂȂ񂾂낤�B�����Ă���ƍő�g�o���Q��������B",
        prefab_name: "HP_Amulet");
    // [4] ���͋F��̂��܂���
    public static readonly ItemData ITEM_MP_AMULET = new ItemData(
        item_id: 4,
        max_hold: 5,
        item_name: "���͋F��̂��܂���",
        price: 10000,
        item_comment: "���̌`���������܂���B�����������Ȃꂻ���B�����Ă���ƍő�l�o���Q��������B",
        prefab_name: "MP_Amulet");
    // [5] �u��
    //public static readonly int ITEM_ID_WHETSTONE = 5;
    // [6] (��)(�h��A�b�v�A�C�e���������\��)
    //public static readonly int ITEM_ID_ = 6;
    // [7] �u�[������
    public static readonly ItemData ITEM_BOOMERANG = new ItemData(
        item_id: 7,
        max_hold: 5,
        item_name: "�u�[������",
        price: 200,
        item_comment: "���邭�邭��`���ē����Ďg���B�L���b�`���邱�Ƃŉ��x���g����B�ł��A���܂ɖ߂��Ă��Ȃ��B",
        prefab_name: "Boomerang");
    // [8] ���e
    public static readonly ItemData ITEM_BOMB = new ItemData(
        item_id: 8,
        max_hold: 10,
        item_name: "�΂�����",
        price: 100,
        item_comment: "�Ԃ�Ȃ��āA���΂炭������A�ǂ����`��B�Ȃ��A���Ɖ΂ɂ͂��p�S�B",
        prefab_name: "Bomb");
    // [9] �e
    //public static readonly int ITEM_ID_GUN = 9;
    // [10] �e��
    //public static readonly int ITEM_ID_BULLETS = 10;
    #endregion
    //###########################################################################################################
    //###########################################################################################################
    //##                                             �v���C���[�f�[�^                                          ##
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
            name:"����"
            );
    public static readonly CharacterStatus STATUS_PLAYER_002_MAGE = new CharacterStatus(
            speed: 70,
            jump_power: 2.0f,
            knockback_weight: 20,
            touch_damage: 0,
            max_hp: 10,
            max_mp: 20,
            name:"�܂ǂ���"
        );
    #endregion
    //###########################################################################################################
    //###########################################################################################################
    //##                                        �G�L�����N�^�[�̊�b�f�[�^                                     ##
    //###########################################################################################################
    //###########################################################################################################
    #region enemydata
    // [1] �]���r
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
            name: "�]���r"),
        appear_wave: 0,
        spawn_interval: 4
        );

    // [2] �X�P���g��
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
            name: "�X�P���g��"),
         appear_wave: 3,
         spawn_interval: 7
        );
    // [3] �S�[�X�g
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
            name: "�S�[�X�g"),
         appear_wave: 6,
         spawn_interval: 9
        );
    // [4] �X���C��
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
        name: "�X���C��"),
     appear_wave: 10,
     spawn_interval: 13
    );
    // [5] �E�B�b�`
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
        name: "�E�B�b�`"),
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
    name: "�S�[����"),
    appear_wave: 35,
    spawn_interval: 70
    );
    #endregion

    //###########################################################################################################
    //###########################################################################################################
    //##                                              �X�e�[�W�f�[�^                                           ##
    //###########################################################################################################
    //###########################################################################################################
    #region stagedata
    public static readonly StageData STATUS_STAGE_001_VILLAGE = new StageData(
            stage_id: 1,
            stage_name:"����", 
            stage_comment: "����Ɉ͂܂ꂽ�����B�������̂悤�Ȃ��̂�����B���ʂȂ��Ƃ͉����Ȃ��A�����̃����B���Ȃ݂ɁA�Z���̓~�Z�̎傾���炵���B",
            prefab_name:""
        );
    public static readonly StageData STATUS_STAGE_002_FOREST = new StageData(
        stage_id: 2,
        stage_name: "����",
        stage_comment: "���������Ƃ��������B�傫�Ȗ؂ɂ́A�e�L���v���C���[��������Ă��܂��B�N�����͂������A�ړ�����̂������ւ�B",
        prefab_name: ""
    );



    #endregion

    // �A�C�e���f�[�^��ID����擾����
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

    // �v���C���[�f�[�^��ID����擾����
    public static CharacterStatus GetCharacterStatus(int chara_id) 
    {
        return chara_id switch
        {
            1 => STATUS_PLAYER_001_KNIGHT,
            2 => STATUS_PLAYER_002_MAGE,
            _ => null,
        };
    }
    // �G�f�[�^��ID����擾����
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

    // �X�e�[�W�f�[�^��ID����擾����
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
        // �X�v���C�g���X�g�𓝊�����prefab��ǂݍ���
        SpriteDatas spriteDatas = Resources.Load<GameObject>(("Prefabs/Sprite_lists")).GetComponent<SpriteDatas>();
        // ���X�g����Y���̃X�v���C�g��Ԃ�
        return spriteDatas.Character_icons[chara_id - 1];
    }
    public static Sprite GetStageIcon_Sprite(int stage_id)
    {
        // �X�v���C�g���X�g�𓝊�����prefab��ǂݍ���
        SpriteDatas spriteDatas = Resources.Load<GameObject>(("Prefabs/Sprite_lists")).GetComponent<SpriteDatas>();
        // ���X�g����Y���̃X�v���C�g��Ԃ�
        return spriteDatas.Stage_icons[stage_id - 1];
    }
    public static Sprite GetModeIcon_Sprite(int mode_id)
    {
        // �X�v���C�g���X�g�𓝊�����prefab��ǂݍ���
        SpriteDatas spriteDatas = Resources.Load<GameObject>(("Prefabs/Sprite_lists")).GetComponent<SpriteDatas>();
        // ���X�g����Y���̃X�v���C�g��Ԃ�
        return spriteDatas.Mode_icons[mode_id];
    }
    public static Sprite GetModeItem_Sprite(int item_id)
    {
        // �X�v���C�g���X�g�𓝊�����prefab��ǂݍ���
        SpriteDatas spriteDatas = Resources.Load<GameObject>(("Prefabs/Sprite_lists")).GetComponent<SpriteDatas>();
        // ���X�g����Y���̃X�v���C�g��Ԃ�
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

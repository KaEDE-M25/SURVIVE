using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//--====================================================--
//--            ���C���Q�[�����Ǘ�����N���X            --
//--====================================================--
public class GameControll : MonoBehaviour
{
    public static readonly string CONFIRM_SAVE_TEXT = "�Q�[����ۑ����܂����H";
    // �t�B�[���h��ɑ��݂ł���G�L�����̐�  (���������h�~)
    public static readonly int MAX_ENEMYS = 300;

    // �e�X�g�p
    [SerializeField ,Header("�e�X�g���Ńv���C����ۂ̓`�F�b�N"), Tooltip("�`�F�b�N������ƁA�K���ȃX�e�[�^�X�����ăQ�[�����J�n����B�G�f�B�^�ł��̃V�[������Đ����鎞�Ƀ`�F�b�N����ꂽ���B")]
    bool test_env = false;

    // �����̏�����
    int money;
    // �L���J�E���g
    int kill_count;
    // ���݂̃E�F�[�u��
    int wave_count;
    // �X�R�A
    int score;

    // �A�C�e���X�g�b�N
    // 1������ -> �A�C�e��ID �A �Y���� -> �X�g�b�N�g�ԍ��@�A�l���O�̏ꍇ�͎����Ă��Ȃ��Ƃ���
    // 2������ -> �A�C�e���̏�����
    public ItemStocks Item_stocks { get; protected set; } = new ItemStocks();

    // �A�C�e���X�g�b�N�̑I���ʒu
    public int Item_stock_cursor { get; protected set; } = 0;
    // �|�[�Y���Ă��邩
    bool is_pause = false;

    // �o�ߎ��ԁi�X�|�[�������̑���Ɏg�p�j
    float time = 0f;

    // �X�|�[���t���O�i��������邲�ƂɂP�オ��A�ݒ肳�ꂽ���l�ɒB����ƃX�|�[�����O�ɖ߂�
    readonly int[] spawn_flags = new int[EigenValue.IMPLEMENTED_ENEMYS];

    // �X�|�[���t���O�ݒ�l�i�ŗL�l�����Ƃɂ��ā}�Q���炢�Ń����_���Ɍ��܂�j
    readonly int[] spawn_flags_max = new int[EigenValue.IMPLEMENTED_ENEMYS];

    // �Q�[���S�̂̏󋵂�ێ����邽�߂̐e�I�u�W�F�N�g
    [Header("�Q�[���S�̂̏󋵂��Ǘ�����e�I�u�W�F�N�g")]
    

    // �V���b�v�̓����������obj
    [SerializeField,Tooltip("�V���b�v�̓�����������I�u�W�F�N�g��ShopEntrancePoint�N���X�B")]
    ShopEntrancePoint shop_entrance_point;

    // �G�L�����̃X�|�[���|�C���g
    [SerializeField,Tooltip("�G���X�|�[������ʒu���܂Ƃ߂��e�I�u�W�F�N�g�B")]
    GameObject enemy_spawn_point;
    // �t�B�[���h�ɂ���G�L�����̃��X�g�ɂȂ�e�I�u�W�F�N�g
    [SerializeField,Tooltip("�t�B�[���h�ɑ��݂���G�L�����̈ꗗ�ƂȂ�e�I�u�W�F�N�g�B�G�L�����͂��ׂĂ���̎q�I�u�W�F�N�g�ɂ���B�����Ă��Ȃ��ƃ|�[�Y�Œ�~���Ȃ��B")]
    GameObject active_enemys_parent;
    // �G�t�F�N�g�̃��X�g�ɂȂ�e�I�u�W�F�N�g
    [SerializeField,Tooltip("�t�B�[���h�ōĐ�����Ă���G�t�F�N�g�̈ꗗ�ƂȂ�e�I�u�W�F�N�g�B�ȉ�Enemy_spawn_point�Ɠ��l")]
    GameObject active_effects_parent;
    // �t�B�[���h�ɗ����Ă�A�C�e���̃��X�g�ɂȂ�e�I�u�W�F�N�g
    [SerializeField,Tooltip("�t�B�[���h�ɑ��݂���A�C�e���ȉ�Enemy_spawn_point�Ɠ��l")]
    GameObject active_items_parent;
    // �E�F�[�u����\������e�L�X�g
    [SerializeField,Tooltip("���݂̃E�F�[�u����\������Text�R���|�[�l���g�B")]
    Text wave_count_text;
    // �A�C�e���X�g�b�N�̑�����s���e�I�u�W�F�N�g
    [Tooltip("�A�C�e���X�g�b�N�S�̂��܂Ƃ߂Ă���e�I�u�W�F�N�g�B�S�̂��ړ������邱�Ƃ�����B")]
    public GameObject item_stock_parent;
    // �A�C�e���X�g�b�N�J�[�\���̃I�u�W�F�N�g
    [Tooltip("�A�C�e���X�g�b�N�̃J�[�\����Image�R���|�[�l���g�B�J�[�\���𓮂�������V���b�v�ł̐����̎��Ɏg���B")]
    public Image item_stock_cursor_obj;

    [Header("���o�p�̐ݒ蕨")]

    // ��ʂ��Â����鏈�������邽�߂̃I�u�W�F�N�g
    [SerializeField, Tooltip("�|�[�Y���Ȃǂɉ�ʂ��Â����邽�߂̃I�u�W�F�N�g��SpriteRenderer�R���|�[�l���g�B")]
    SpriteRenderer dimmer_spre;

    [SerializeField, Tooltip("�v�f0��Ready...�A�v�f1��GO!!�A�̊J�n���o�Ɏg���I�u�W�F�N�g")]
    GameObject[] ready_go_effect = new GameObject[2];

    // �ړ��p�̃A�j���[�V����obj
    [SerializeField,Tooltip("�V�[���J�ڃA�j���[�V�����̃R���|�[�l���g�B")]
    MoveSceneAnimation mcanim;

    // �E�F�[�u�N���A�������̃e�L�X�g�G�t�F�N�g
    [SerializeField, Tooltip("�E�F�[�u�N���A�������̕���")]
    Text wave_clear_effect;
    [SerializeField, Tooltip("�R���e�B�j���[��ʂ̃G�t�F�N�g�S�̂��܂Ƃ߂��e�I�u�W�F�N�g�B")]
    GameObject continue_effect_parent;
    [SerializeField, Tooltip("�|�[�Y��ʂ̃G�t�F�N�g�S�̂��܂Ƃ߂��e�I�u�W�F�N�g�B")]
    GameObject pause_effect_parent;

    // �e��UI�G�t�F�N�g
    [SerializeField,Tooltip("�������l�������Ƃ���UI�G�t�F�N�g(Prefab)�B")]
    GameObject money_up_effect;
    [SerializeField,Tooltip("�L���J�E���g���������Ƃ���UI�G�t�F�N�g(Prefab)�B")]
    GameObject kill_up_effect;

    // �A�C�e�����̂Ă��Ƃ��̃G�t�F�N�g(prefab)
    [SerializeField,Tooltip("�A�C�e�����̂Ă��ۂ̎̂Ă�G�t�F�N�g��Prefab�B")]
    GameObject drop_item_effect_prefab;

    [Header("�����𕪂��Ă�R���|�[�l���g�Ƃ�")]

    // �e��R���|�[�l���g
    [SerializeField,Tooltip("���̃R���|�[�l���g�������Ă�Q�[���X�R�A(�X�R�A�A�����Ȃ�)���e�L�X�g�ɔ��f�����鏈����S���N���X�B")]
    CountControll count_controll;
    [SerializeField,Tooltip("�E�F�[�u�̐i�s�x��(�J�E���g�_�E����UI�Q�[�W�ւ̕\�����f)���Ǘ�����N���X�B")]
    WaveControll wave_controll;
    public CameraControll Camera_controll { get; protected set; }
    Timer wave_controll_timer;

    // �v���C���[�̃R���|�[�l���g
    Fighters fighter_comp;

    // �Q�[���f�[�^
    public PlayData Play_data { get; set; } = null;

    //##====================================================##
    //##                   Awake �������P                   ##
    //##====================================================##
    private void Awake()
    {
        // �R���|�[�l���g������
        wave_controll_timer = wave_controll.Timer();
        Camera_controll = GameObject.FindWithTag("MainCamera").GetComponent<CameraControll>();
    }

    //##====================================================##
    //##                   Start �������Q                   ##
    //##====================================================##
    private void Start()
    {

        // �|�[�Y�̍ۂ̃L�[�摜��ݒ�ɉ����Č��� (�Ή�����obj��active�ɂ���)
        pause_effect_parent.transform.Find("-PAUSE-/EXIT/" + OptionData.current_options.controller).gameObject.SetActive(true);
        pause_effect_parent.transform.Find("-PAUSE-/RESUME/" + OptionData.current_options.controller).gameObject.SetActive(true);
        continue_effect_parent.transform.Find("Yes/" + OptionData.current_options.controller).gameObject.SetActive(true);
        continue_effect_parent.transform.Find("No/" + OptionData.current_options.controller).gameObject.SetActive(true);

        if (test_env)
        {
            var stocks = new ItemStocks();
            stocks.item_stocks[0].item_id = 1;
            stocks.item_stocks[0].num = 8;
            stocks.item_stocks[1].item_id = 3;
            stocks.item_stocks[1].num = 5;
            stocks.item_stocks[2].item_id = 2;
            stocks.item_stocks[2].num = 8;
            stocks.item_stocks[3].item_id = 2;
            stocks.item_stocks[3].num = 5;
            stocks.item_stocks[4].item_id = 2;
            stocks.item_stocks[4].num = 5;
            Play_data = new PlayData(1, 1,2, 20, 10, 1957291, 2157192, 52191,49, stocks);

        }

        StartCoroutine(GameStart(Play_data));
        Pausing(true);
    }

    //##====================================================##
    //##                  Update ���C�����[�v               ##
    //##====================================================##
    private void Update()
    {
        // �|�[�Y�֘A�̏���
        PausingControll();

        // �|�[�Y���Ȃ瑼�����͍s��Ȃ�
        if (is_pause)
            return;

        // �v���C���[����A�N�e�B�u�Ȃ�Q�[���I�[�o�[���������s
        if (!fighter_comp.gameObject.activeSelf)
        {
            // �Q�[���I�[�o�[����
            GameOver();
        }

        // �G�̃X�|�[������
        if (active_enemys_parent.transform.childCount <= MAX_ENEMYS)
        { 
            time += Time.deltaTime;
            if (time >= EigenValue.SPAWN_INTERVAL / (Play_data.choose_mode_id == 1 ? 4f : 1f))
            {
                MonsterSpawn();
                time = 0f;
            }
        }


        // �V���b�v�ɓ��鏈��
        if (shop_entrance_point.Can_enter_shop)
            if (InputControll.GetInputDown(InputControll.INPUT_ID_UPARROW))
            {
                Pausing(true);
                StartCoroutine(ShopControll.CreateShopWindow(this));
            }


        // �G���������e�L�X�g���X�V
        count_controll.Set_Enemy_text(active_enemys_parent.transform.childCount);

        // �J�E���g�_�E�����~�܂��Ă��遁�I�����Ă���Ȃ�
        if (!wave_controll_timer.Is_count)
        {
            // �A�N�e�B�u�ȓG��0�ɂȂ�����
            if (active_enemys_parent.transform.childCount == 0)
            {
                // �N���A�e�L�X�g���Đ�
                if (!wave_clear_effect.gameObject.activeSelf)
                {
                    StartCoroutine(WaveClear());
                }
            }

        }

        // �A�C�e���X�g�b�N�J�[�\���𓮂�������
        ItemStockCursorControll();

    }

    //##====================================================##
    //##       �E�F�[�u�N���A���ɁA�N���A�G�t�F�N�g��       ##
    //##       �҂�����ɃR���e�B�j���[��ʂ֑J�ڂ���       ##
    //##====================================================##
    IEnumerator WaveClear() 
    {

        // �X�R�A�����Z
        Score_count(wave_count * 10 + EigenValue.WAVE_BASE_TIME);

        AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_UI, AudioFilePositions.UI.WAVECLEAR);

        // �N���A�G�t�F�N�g���o��
        wave_clear_effect.transform.localPosition = Vector3.zero;
        wave_clear_effect.gameObject.SetActive(true);
        wave_clear_effect.GetComponent<TypefaceAnimator>().Play();

        yield return new WaitForSeconds(2f);

        // �ő�E�F�[�u�𒴉߂�����N���A����
        if (wave_count >= EigenValue.GetModeMaxWave(Play_data.choose_mode_id))
        {
            StartCoroutine(GameClear());
        }
        else // ����ȊO�ł̓R���e�B�j���[���邩�̊m�F 
        {
            continue_effect_parent.SetActive(true);
            dimmer_spre.gameObject.SetActive(true);
            Pausing(true);
            wave_clear_effect.transform.DOLocalMoveY(10f, 0.5f).SetEase(Ease.OutSine);
        }

        yield break;
    }

    //##====================================================##
    //##               �e��Z�b�^�[�E�Q�b�^�[               ##
    //##     (�ꕔ�Z�b�^�[�ɃG�t�F�N�g�����������Ă���)     ##
    //##====================================================##
    #region setter_and_getter

    // �G�t�F�N�g�Q�̐e�I�u�W�F�N�g�̃Z�b�^�[
    public GameObject Active_Effects_Parent() { return active_effects_parent; }
    public GameObject Active_Items_Parent() { return active_items_parent; }

    // wave_count�̃Z�b�^�[�ƃQ�b�^�[
    public void Wave_count(int get)
    {
        wave_count += get;
    }
    public int Wave_count()
    {
        return wave_count;
    }
    // score_count�̃Z�b�^�[�ƃQ�b�^�[
    public void Score_count(int get)
    {
        score += get;
        // �e�L�X�g�X�V
        count_controll.Set_Score_text(score);
    }
    public int Score_count()
    {
        return score;
    }
    // kill_count�̃Z�b�^�[�ƃQ�b�^�[
    public void Kill_count(int get, int character_score)
    {

        kill_count += get;
        // �e�L�X�g�X�V
        count_controll.Set_kill_text(kill_count);

        // �㏸�������ꍇ�̓G�t�F�N�g����
        if (get > 0)
        {
            GameObject effect = Instantiate(kill_up_effect, count_controll.Kill_count().transform.position, transform.rotation);
            effect.transform.parent = count_controll.Kill_count().transform;
        }

        // �X�R�A���Z
        Score_count(character_score);

    }
    public int Kill_count()
    {
        return kill_count;
    }
    // Money�̃Z�b�^�[�ƃQ�b�^�[
    public void Money(int get)
    {

        money += get;
        // �e�L�X�g�X�V
        count_controll.Set_money_text(money);

        // �㏸�������ꍇ�̓G�t�F�N�g����
        if (get > 0)
        {
            GameObject effect = Instantiate(money_up_effect, count_controll.Money_count().transform.position, transform.rotation);
            effect.transform.parent = count_controll.Money_count().transform;

        }


    }
    public int Money()
    {
        return money;
    }
    #endregion

    //##====================================================##
    //##           �e�E�F�[�u�̐������Ԃ��v�Z����           ##
    //##====================================================##
    private int Compute_Wave_timer(int wave_num)
    {
        return (wave_num - 1) * 5 + EigenValue.WAVE_BASE_TIME;
    }

    //##====================================================##
    //##     �Q�[���̊J�n�����i�Q�[���f�[�^�����[�h����     ##
    //##====================================================##
    public IEnumerator GameStart(PlayData data)
    {
        this.enabled = false;

        // ���z�R���g���[���[���X�e�B�b�N�ɕύX
        InputControll.use_virtual_stick = true;

        // �J�E���^�[�n�̏����ݒ�
        money = data.money;
        kill_count = data.kill_count;
        wave_count = data.clear_waves;
        score = data.score;


        count_controll.Set_Score_text(score);
        count_controll.Set_money_text(money);
        count_controll.Set_kill_text(kill_count);
        Wave_proceed();

        // �X�|�[�������J�E���^�[�ő�l�̏�����
        for (int i = 0; i < spawn_flags_max.Length; i++)
            spawn_flags_max[i] = EigenValue.GetEnemyData(i).spawn_interval;


        // �A�C�e���X�g�b�N�̏�����
        Item_stocks = data.item_stocks;
        for (int i = 0; i < Item_stocks.item_stocks.Length; i++)
        {
            var stock = item_stock_parent.transform.Find("StockID_" + (i + 1));

            bool is_null = Item_stocks.item_stocks[i].item_id == 0;

            Set_Stock_info(stock,Item_stocks.item_stocks[i].item_id, Item_stocks.item_stocks[i].num);

            // �������Ă��邱�ƂŌ��ʂ𔭊�����A�C�e���i�����n�Ȃǁj�̌��ʂ𔭐�������
            if (!is_null)
                for (int j = 0; j < Item_stocks.item_stocks[i].num; j++)
                    Resources.Load<Items>((EigenValue.PREFAB_DIRECTORY_ITEMS + EigenValue.GetItemData(Item_stocks.item_stocks[i].item_id).prefab_name)).Hold_Effect();

        }

        // HP��MP������

        fighter_comp = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<Fighters>();
        fighter_comp.HP = data.hp;
        fighter_comp.MP = data.mp;

        // UI��̃Q�[�W���X�V
        fighter_comp.Hp_bar.RenewGaugeAmount();
        fighter_comp.Mp_bar.RenewGaugeAmount();

        yield return new WaitForSeconds(2);

        // READY... �̃G�t�F�N�g��\��
        ready_go_effect[0].SetActive(true);

        yield return new WaitForSeconds(2.5f);

        // GO!! �̃G�t�F�N�g��\�����ăQ�[���J�n
        this.enabled = true;
        Pausing(false);
        ready_go_effect[0].SetActive(false);
        ready_go_effect[1].SetActive(true);
        ready_go_effect[1].transform.DOLocalMoveX(0f, 1.5f).OnComplete(() =>
        {
            ready_go_effect[1].transform.DOLocalMoveY(360f, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
            {
                ready_go_effect[1].SetActive(false);

            });

        });

    }

    //##====================================================##
    //##           �����X�^�[�̃X�|�[���S�̂̊Ǘ�           ##
    //##====================================================##
    private void MonsterSpawn()
    {
        if (wave_controll_timer.Is_count)
        {
            // ��������Ă���e�G�L�����ɂ���
            for (int i = 0; i < spawn_flags.Length; i++)
            {
                spawn_flags[i]++;
                EnemyData enemyData = EigenValue.GetEnemyData(i);

                // �X�|�[�������ɒB���Ă����@���@�X�|�[���J�n�E�F�[�u���ɒB���Ă�����X�|�[���������J�n
                if (spawn_flags[i] >= spawn_flags_max[i] && wave_count >= enemyData.appear_wave / (Play_data.choose_mode_id == 1 ? 10 : 1))
                {
                    spawn_flags[i] = 0;
                    spawn_flags_max[i] = enemyData.spawn_interval + Random.Range(-2,2);

                    // �E�F�[�u���ɉ����ăX�|�[���|�C���g�̐�������
                    for (int j = 0; j < (wave_count < enemy_spawn_point.transform.childCount * (EigenValue.DIFFICULTY_RANK_UP_COF / (Play_data.choose_mode_id == 1 ? 10 : 1)) ? wave_count / EigenValue.DIFFICULTY_RANK_UP_COF + 1 : enemy_spawn_point.transform.childCount); j++)
                    {
                        Transform spawn_point = enemy_spawn_point.transform.GetChild(j);
                        // �X�|�[���|�C���g�̍��ƉE�ɂ���
                        for (int k = 0; k < spawn_point.childCount; k++)
                        {
                            Spawn(i, spawn_point.GetChild(k).position);
                        }
                    }
                }
            }
        }
    }

    //##====================================================##
    //##             �����X�^�[�̃X�|�[���i�P�́j           ##
    //##====================================================##
    void Spawn(int enemy_id,Vector3 position) 
    {
        Instantiate(Resources.Load<GameObject>(EigenValue.PREFAB_DIRECTORY_ENEMYS + EigenValue.GetEnemyData(enemy_id).prefab_name), position, transform.rotation, active_enemys_parent.transform);
    }

    //##====================================================##
    //##                 �E�F�[�u��i�s����                 ##
    //##====================================================##
    void Wave_proceed()
    {
        // 1�E�F�[�u�i�߂�
        Wave_count(1);
        // �^�C�}�[���X�V
        wave_controll_timer.TimerStart(Compute_Wave_timer(wave_count));
        // �e�L�X�g�ɔ��f
        wave_count_text.text = wave_count.ToString();

    }

    //##====================================================##
    //##       �A�C�e���X�g�b�N�̉摜��ύX���鏈��         ##
    //##     (��ɂ���ꍇ�̓A�C�e��ID = 0�Ƃ��ēn��)       ##
    //##====================================================##
    void Set_Stock_info(Transform target_of_stock,int item_id,int num) 
    {
        // �X�g�b�N�̉摜��ݒ�
        target_of_stock.Find("icon").GetComponent<Image>().sprite =
            item_id == 0 ? null : Resources.Load<Transform>((EigenValue.PREFAB_DIRECTORY_ITEMS + EigenValue.GetItemData(item_id).prefab_name)).Find("graphic").GetComponent<SpriteRenderer>().sprite;


        // �X�g�b�N���̃e�L�X�g��ݒ�
        target_of_stock.Find("text").GetComponent<Text>().text =
            item_id == 0 ? "" : num.ToString();

        // �A�C�e����������ɒB���Ă����珊�����̘g�F��ς���
        if (item_id != 0)
            target_of_stock.Find("text").GetComponent<Outline>().effectColor = num >= EigenValue.GetItemData(item_id).max_hold ? Color.red : Color.white;
 
    }

    //##====================================================##
    //##       �A�C�e���X�g�b�N�ɃA�C�e�����Z�b�g����       ##
    //##   (����͍l�����Ȃ�)  ���̒l������Ə����     ##
    //##====================================================##
    public void Set_item_stock(int item_id, int num, int index)
    {
        Transform target = item_stock_parent.transform.Find("StockID_" + (index + 1));

        // �G�t�F�N�g��\��
        if (num > 0)
            target.Find("GetItemEffect").gameObject.SetActive(true);

        // �A�C�e���X�g�b�N�̑Ώۂ̈ʒu����Ȃ�
        if (Item_stocks.item_stocks[index].item_id == 0)
        {

            Item_stocks.item_stocks[index].item_id = item_id;
            Item_stocks.item_stocks[index].num += num;

            Set_Stock_info(target,Item_stocks.item_stocks[index].item_id, Item_stocks.item_stocks[index].num);
        }
        else if (Item_stocks.item_stocks[index].item_id == item_id) // �Ώۂ̈ʒu������̃A�C�e���Ȃ�
        {
            // �g�p�܂��͏���
            Item_stocks.item_stocks[index].num += num;

            // �X�g�b�N�ʒu�̏��������O�ɂȂ�����A�C�e��ID����l�ɂ��Ă���
            if (Item_stocks.item_stocks[index].num <= 0)
                Item_stocks.item_stocks[index].item_id = 0;
            
            Set_Stock_info(target,Item_stocks.item_stocks[index].item_id, Item_stocks.item_stocks[index].num);

        }

    }

    //##====================================================##
    //##     �E�����A�C�e�����A�C�e���X�g�b�N�Ɋi�[����     ##
    //##                 (�ꏊ�͎�������)                   ##
    //##====================================================##
    public void Set_item_stock_from_catch(int item_id,Items item_comp = null)
    {
        for (int i = 0; i < Item_stocks.item_stocks.Length; i++)
        {
            // ������ނ��X�g�b�N���Ă���ʒu����������
            if (Item_stocks.item_stocks[i].item_id == item_id)
            {
                // �ő及�����ɓ��B���Ă��Ȃ���Βǉ�
                if (Item_stocks.item_stocks[i].num < EigenValue.GetItemData(item_id).max_hold)
                {
                    Set_item_stock(item_id, 1, i);
                    if (item_comp != null)
                        item_comp.Hold_Effect();
                    return;
                }
            }
        }
        // �ǂ��ɂ������A�C�e�����X�g�b�N���Ă��Ȃ�������
        for (int i = 0; i < Item_stocks.item_stocks.Length; i++)
        {
            // ��̃X�g�b�N���������炻�̈ʒu�ɒǉ�
            if (Item_stocks.item_stocks[i].item_id == 0)
            {
                Set_item_stock(item_id, 1, i);
                if (item_comp != null)
                    item_comp.Hold_Effect();
                return;
            }
        }
        // �A�C�e���X�g�b�N�ł��Ȃ�������̂Ă�G�t�F�N�g���o��
        Instantiate(drop_item_effect_prefab, fighter_comp.transform).GetComponent<ParticleSystem>()
            .textureSheetAnimation.SetSprite(0, 
            Resources.Load<Transform>(EigenValue.PREFAB_DIRECTORY_ITEMS + EigenValue.GetItemData(item_id).prefab_name).Find("graphic").GetComponent<SpriteRenderer>().sprite);

        return;
    }

    //##====================================================##
    //##   �A�C�e���X�g�b�N�̃J�[�\���ʒu�ɂ���A�C�e����   ##
    //##   �g�p���� (�g�p�ł��Ȃ��A�C�e���Ȃ�false��Ԃ�)   ##
    //##====================================================##
    public bool Use_item_from_stock()
    {
        // �J�[�\���̈ʒu�ɃA�C�e�����i�[����Ă���Ȃ�
        if (Item_stocks.item_stocks[Item_stock_cursor].item_id > 0 && Item_stocks.item_stocks[Item_stock_cursor].num > 0)
        {
            // �A�C�e���̎g�p�����݂�
            bool is_useable = Resources.Load<Items>(EigenValue.PREFAB_DIRECTORY_ITEMS + (EigenValue.GetItemData(Item_stocks.item_stocks[Item_stock_cursor].item_id).prefab_name)).Use();
            
            // �g�p�ł���A�C�e���͎g�p����True���Ԃ����
            if (is_useable)
            {
                // �A�C�e���X�g�b�N����A�C�e�������炷
                fighter_comp.is_itemuse = true;
                Set_item_stock(Item_stocks.item_stocks[Item_stock_cursor].item_id, -1, Item_stock_cursor);
                return true;
            }
        }
        return false;

    }

    //##====================================================##
    //##        �A�C�e���X�g�b�N�J�[�\���𓮂�������        ##
    //##                (����������true��Ԃ�)              ##
    //##====================================================##
    public bool ItemStockCursorControll()
    {
        // ���ֈړ�
        if (InputControll.GetInputDown(InputControll.INPUT_ID_TRIGGER_L))
        {
            AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_PLAYER, AudioFilePositions.UI.MOVE_ITEMSTOCK_CURSOR);

            if (Item_stock_cursor == 0)
                Item_stock_cursor = Item_stocks.item_stocks.Length - 1;
            else
                Item_stock_cursor -= 1;

            RenewCursorPos();
            return true;
        }
        // �E�ֈړ�
        else if (InputControll.GetInputDown(InputControll.INPUT_ID_TRIGGER_R))
        {
            AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_PLAYER, AudioFilePositions.UI.MOVE_ITEMSTOCK_CURSOR);

            if (Item_stock_cursor == Item_stocks.item_stocks.Length - 1)
                Item_stock_cursor = 0;
            else
                Item_stock_cursor += 1;

            RenewCursorPos();
            return true;
        }
        return false;
    }

    // �T�u�֐��F�J�[�\���̈ʒu���X�V����
    void RenewCursorPos()
    {
        item_stock_cursor_obj.transform.SetParent(item_stock_parent.transform.Find("StockID_" + (Item_stock_cursor + 1)));
        item_stock_cursor_obj.transform.localPosition = Vector3.forward * 8f;
    }

    //##====================================================##
    //##              �|�[�Y���̑�����s������              ##
    //##====================================================##
    private void PausingControll()
    {

        // �E�F�[�u��i�ނ��ǂ������߂Ă���Ƃ�
        if (continue_effect_parent.activeSelf) 
        {
            // �Q�[���𑱂���
            if (InputControll.GetInputDown(InputControll.INPUT_ID_A))
            {
                AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_UI, AudioFilePositions.UI.DECISION);
                Pausing(false);
                pause_effect_parent.SetActive(false);
                dimmer_spre.gameObject.SetActive(false);
                continue_effect_parent.SetActive(false);
                pause_effect_parent.transform.localPosition = Vector3.zero;
                // �E�F�[�u��i�߂�
                Wave_proceed();

                wave_clear_effect.transform.DOLocalMoveY(240f, 0.5f).SetEase(Ease.Linear).OnComplete(() => 
                {
                    wave_clear_effect.gameObject.SetActive(false);
                });

            }
            // �Z�[�u����
            else if (InputControll.GetInputDown(InputControll.INPUT_ID_B))
            {
                StartCoroutine(PopUpWindowControll.CreateSmallWindow(
                delegate () // �͂���I����������
                {
                    SceneManager.sceneLoaded += Send_datas;
                    // �Z�[�u��ʂ֑J��
                    InputControll.use_virtual_stick = false;
                    mcanim.MoveScene("SaveLoadGame");
                },
                delegate () // ��������I����������
                {
                    InputControll.use_virtual_stick = false;
                    mcanim.MoveScene("Menu");
                },
                this, new Vector2(200f, 160f), CONFIRM_SAVE_TEXT, transform.position));

            }


        }
        else�@// ����ȊO(�ʏ�̃|�[�Y�����̂Ƃ�)
        {
            // �|�[�Y���Ȃ�
            if (is_pause)
            {
                // �|�[�Y���������鑀��
                if (InputControll.GetInputDown(InputControll.INPUT_ID_START) || InputControll.GetInputDown(InputControll.INPUT_ID_A))
                {

                    AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_UI, AudioFilePositions.UI.DECISION);

                    Pausing(false);
                    pause_effect_parent.SetActive(false);
                    dimmer_spre.gameObject.SetActive(false);
                    pause_effect_parent.transform.localPosition = Vector3.zero;
                }
                // ���j���[��ʂɖ߂鑀��
                else if (InputControll.GetInputDown(InputControll.INPUT_ID_B))
                {
                    AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_UI, AudioFilePositions.UI.DECISION);
                    InputControll.use_virtual_stick = false;
                    mcanim.MoveScene("Menu");
                    this.enabled = false;
                }
            }
            else
            // �|�[�Y���łȂ��Ȃ�|�[�Y�J�n����
            {
                if (InputControll.GetInputDown(InputControll.INPUT_ID_START))
                {
                    AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_UI, AudioFilePositions.UI.PAUSE);
                    dimmer_spre.gameObject.SetActive(true);
                    pause_effect_parent.SetActive(true);
                    Pausing(true);
                }
            }
        }
    }

    //##====================================================##
    //##                   �|�[�Y�����鏈��                 ##
    //##  true������ƃ|�[�Y�Afalse������ƃ|�[�Y����   ##
    //##====================================================##
    public void Pausing(bool enable)
    {
        is_pause = enable;

        // �E�F�[�u�̐i�s���~�߂�
        wave_controll_timer.enabled = !enable;


        // �v���C���[�̓�����~�߂�
        fighter_comp.GetComponent<Rigidbody2D>().simulated = !enable;
        fighter_comp.GetComponent<Animator>().enabled = !enable;
        fighter_comp.enabled = !enable;
        fighter_comp.Mini_hp_bar.Active_gauge.transform.DOTogglePause();
        fighter_comp.Mini_hp_bar.Damage_gauge.transform.DOTogglePause();
        fighter_comp.Mini_hp_bar.Empty_gauge.transform.DOTogglePause();
        fighter_comp.Mini_hp_bar.enabled = !enable;

        // �G�t�F�N�g�̓�������ׂĎ~�߂�
        foreach (Transform effect in fighter_comp.transform)
        {
            if (effect.name.Contains("EF_"))
            {
                if (effect.gameObject.activeSelf)
                {
                    if (effect.TryGetComponent(out ParticleSystem ps)) if (enable) ps.Pause(true); else ps.Play(true);
                    effect.transform.DOTogglePause();
                    foreach (Transform child in effect)
                    {
                        child.DOTogglePause();
                    }

                }
            }

        }

        // �G�̓�������ׂĎ~�߂�
        foreach (Transform enemy in active_enemys_parent.transform)
        {
            enemy.GetComponent<Rigidbody2D>().simulated = !enable;
            enemy.GetComponent<Animator>().enabled = !enable;

            Character enemy_chara_cp = enemy.GetComponent<Character>();
            enemy_chara_cp.Mini_hp_bar.Active_gauge.transform.DOTogglePause();
            enemy_chara_cp.Mini_hp_bar.Damage_gauge.transform.DOTogglePause();
            enemy_chara_cp.Mini_hp_bar.Empty_gauge.transform.DOTogglePause();
            enemy_chara_cp.Mini_hp_bar.enabled = !enable;
            enemy_chara_cp.enabled = !enable;

        }

        // �G�t�F�N�g�̓�������ׂĎ~�߂�

        foreach (Transform effect in active_effects_parent.transform)
        {
            if (effect.TryGetComponent(out Rigidbody2D rb2d)) rb2d.simulated = !enable;
            if (effect.TryGetComponent(out ParticleSystem ps)) if (enable) ps.Pause(true); else ps.Play(true);
            foreach (Transform child in effect)
            {
                if (effect.TryGetComponent(out ps)) if (enable) ps.Pause(true); else ps.Play(true);
                child.DOTogglePause();
            }

            if (effect.TryGetComponent(out Character chara_cp))
            {
                chara_cp.Mini_hp_bar.Active_gauge.transform.DOTogglePause();
                chara_cp.Mini_hp_bar.Damage_gauge.transform.DOTogglePause();
                chara_cp.Mini_hp_bar.Empty_gauge.transform.DOTogglePause();
                chara_cp.Mini_hp_bar.enabled = !enable;
                chara_cp.enabled = !enable;
            }
        }

        // �t�B�[���h�ɗ����Ă�A�C�e���̓�������ׂĎ~�߂�
        foreach (Transform item in active_items_parent.transform)
        {
            item.GetComponent<Rigidbody2D>().simulated = !enable;
            item.GetComponent<Items>().enabled = !enable;

        }
        // �X�e�[�W�M�~�b�N���~�߂�
        //      ��Ŏ���
    }

    //##====================================================##
    //##                �Q�[���N���A���̏���                ##
    //##====================================================##
    IEnumerator GameClear() 
    {
        yield return new WaitForSeconds(5f);

        InputControll.use_virtual_stick = false;
        Pausing(true);
        SceneManager.sceneLoaded += Send_datas;
        mcanim.MoveScene("GameOver");
    }

    //##====================================================##
    //##               �Q�[���I�[�o�[���̏���               ##
    //##====================================================##
    void GameOver()
    {
        InputControll.use_virtual_stick = false;
        SceneManager.sceneLoaded += Send_datas;
        Pausing(true);
        mcanim.MoveScene("GameOver");
    }

    //##====================================================##
    //##          �J�ڐ�(���U���g���)�փf�[�^�𑗂�        ##
    //##====================================================##
    void Send_datas(Scene next,LoadSceneMode mode)
    {
        Play_data.score = score;
        Play_data.money = money;
        Play_data.kill_count = kill_count;
        Play_data.clear_waves = wave_count;
        Play_data.hp = fighter_comp.HP;
        Play_data.mp = fighter_comp.MP;
        Play_data.item_stocks = Item_stocks;

        // ���̃V�[���ɂ���ď���
        switch (next.name) 
        {
            case "GameOver":  // �Q�[���I�[�o�[�֑J�ڂ���Ƃ�
                {
                    GameOverControll gameOverControll = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameOverControll>();
                    gameOverControll.SetResult(Play_data, fighter_comp.HP > 0);

                    break;
                }

            case "SaveLoadGame":  // �Q�[���̕ۑ��֑J�ڂ���Ƃ�
                {
                    SaveLoadControll saveLoadControll = GameObject.FindGameObjectWithTag("GameController").GetComponent<SaveLoadControll>();
                    saveLoadControll.Initialize(true, Play_data);
                    
                    break;
                }

            default:
                return;
        
        }
        SceneManager.sceneLoaded -= Send_datas;
    }

}

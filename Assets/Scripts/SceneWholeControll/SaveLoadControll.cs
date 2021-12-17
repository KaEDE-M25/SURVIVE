using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;

//--====================================================--
//--         �V�[��SaveLoadGame�̊Ǘ��𓝊�����         --
//--====================================================--
public class SaveLoadControll : MonoBehaviour
{
    // �ۑ��f�[�^�̎�ޖ�(�F)
    static readonly string[] DATAS_NAME = new string[10] { "red", "orange", "yellow", "green", "glue", "indigo", "purple", "white", "grey", "black" };
    // �^�C�g���̃e�L�X�g
    static readonly string[] TITLE_TEXTS = new string[2] { "�k�n�`�c�@�f�`�l�d", "�r�`�u�d�@�f�`�l�d" };
    // �ۑ��m�F�e�L�X�g
    const string SAVE_CONFIRM_TEXT = "���̃J���[�ɃZ�[�u���܂��B�{���ɂ�낵���ł����H";
    // �ۑ������Ƀ��j���[�ɖ߂�e�L�X�g
    const string NOTSAVE_CONFIRM_TEXT = "�Z�[�u�����Ƀ��j���[�ɖ߂�܂��B\n�{���ɂ�낵���ł����H";
    // ���[�h�m�F�e�L�X�g
    const string LOAD_CONFIRM_TEXT = "���[�h�����J���[�͋�ɂȂ�܂��B\n�{���ɂ�낵���ł����H";

    // �ۑ�����Ă���f�[�^�̃��X�g
    readonly PlayData[] saved_datas = new PlayData[10];

    [SerializeField, Header("�e�X�g���Ńv���C����ۂ̓`�F�b�N"), Tooltip("�`�F�b�N������ƁA���[�h�Z�[�u�f�[�^�Ƃ��ĉ��f�[�^��p����B�G�f�B�^�ł��̃V�[������Đ����鎞�Ƀ`�F�b�N����ꂽ���B")]
    bool test_mode = false;

    [Header("��Ղ̑���")]

    [SerializeField,Tooltip("���j���[�̑I�����i�Z�[�u���[�h����J���[�j��obj")]
    GameObject[] menu_objs = new GameObject[10];
    [SerializeField,Tooltip("���j���[�J�[�\��")]
    GameObject cursor;
    [SerializeField,Tooltip("�^�C�g���̃e�L�X�gobj")]
    Text title_text;

    [SerializeField,Tooltip("�f�[�^������܂���I�@��\������obj")]
    GameObject not_data_parent;
    // �f�[�^������
    [SerializeField,Tooltip("���ɑ��݂���f�[�^��\������eobj")]
    GameObject exist_data_parent;

    [SerializeField,Tooltip("�Z�[�u���[�h����f�[�^���v���r���[�Ƃ��ĕ\������eobj")]
    GameObject save_data_preview_parent;

    [Header("�����f�[�^�̃v���r���[�ɕ\������鍀��")]

    [SerializeField,Tooltip("HP�o�[�@�i��fill����)")]
    Image hp_bar_obj;
    [SerializeField,Tooltip("MP�o�[ �@(��fill����)")]
    Image mp_bar_obj;
    [SerializeField,Tooltip("�A�C�e���X�g�b�N")]
    GameObject item_stock;
    [SerializeField,Tooltip("�e��J�E���^�A�����A�L���A�X�R�A")]
    Text[] counters;
    [SerializeField,Tooltip("�E�F�[�u���̍ő�l�i�̐eobj�j")]
    GameObject wave_max_parent;

    [Header("�Z�[�u���[�h�f�[�^�̃v���r���[�ɕ\������鍀��")]

    [SerializeField, Tooltip("HP�o�[�@�i��fill����)")]
    Image new_hp_bar_obj;
    [SerializeField, Tooltip("MP�o�[ �@(��fill����)")]
    Image new_mp_bar_obj;
    [SerializeField, Tooltip("�A�C�e���X�g�b�N")]
    GameObject new_item_stock;
    [SerializeField, Tooltip("�e��J�E���^�A�����A�L���A�X�R�A")]
    Text[] new_counters;
    [SerializeField, Tooltip("�E�F�[�u���̍ő�l�i�̐eobj�j")]
    GameObject new_wave_max_parent;

    [Header("���̑�")]

    [SerializeField,Tooltip("����������������郁�b�Z�[�W�Q�̐eobj")]
    GameObject controll_explainer;

    [SerializeField, Tooltip("�V�[���J�ڃA�j���[�V�����̃R���|�[�l���g")]
    MoveSceneAnimation mcanim;

    // �Z�[�u���邩���[�h���邩 true�Ȃ�Z�[�u����Afalse�Ȃ烍�[�h����
    bool Is_save_action { get; set; } = false;

    // �O�V�[������󂯎�����v���C�f�[�^
    PlayData Given_playdata { get; set; } = null;

    int choose = 0;
    bool is_effect = false;

    //##====================================================##
    //##          �t�@�C�����V���A���C�Y���ĕۑ�            ##
    //##====================================================##
    public static void Seriarize<T>(string path,T obj) 
    {
        using (FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write)) 
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();

            binaryFormatter.Serialize(fileStream, obj);
        }
    }

    //##====================================================##
    //##        �t�@�C�����f�V���A���C�Y���ēǂݍ���        ##
    //##====================================================##
    public static T Deserialize<T>(string path)
    {
        T obj;
        try
        {
            using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                obj = (T)binaryFormatter.Deserialize(fileStream);

            }
        }catch (FileNotFoundException) 
        {
            return default;
        }
        return obj;
    }

    //##====================================================##
    //##                     Awake ������                   ##
    //##====================================================##
    private void Awake()
    {
        if (test_mode)
        {
            var stocks = new ItemStocks();
            stocks.item_stocks[0].item_id = 1;
            stocks.item_stocks[0].num = 8;
            stocks.item_stocks[1].item_id = 3;
            stocks.item_stocks[1].num = 5;
            stocks.item_stocks[2].item_id = 2;
            stocks.item_stocks[2].num = 8;

            Initialize(true, new PlayData(1, 1, 2, 20, 10, 1957291, 2157192, 52191, 50, stocks)/*, null*/);
        }

        // �Z�[�u�f�[�^��SaveDatas�t�H���_����Ăяo���ă��X�g�𐶐�(�t�@�C���������F��null�ɂ���)
        for (int i=0;i<saved_datas.Length;i++)
        {
            saved_datas[i] = Deserialize<PlayData>(EigenValue.GetSaveDataPath()+DATAS_NAME[i]);
            if (saved_datas[i] == default(PlayData))
                saved_datas[i] = null;

            // �f�[�^����̂Ƃ��͑Ή��̐F�������񂾐F�ɂ���
            if(saved_datas[i] == null) 
            {
                SpriteRenderer spriteRenderer = menu_objs[i].transform.Find("background").GetComponent<SpriteRenderer>();
                spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0.1f);
            }
        }

        // �g�p���Ă���R���g���[���[�ɍ����{�^���A�C�R����\��������
        controll_explainer.transform.Find("Determine/icon-" + OptionData.current_options.controller).gameObject.SetActive(true);
        controll_explainer.transform.Find("BackTitle/icon-" + OptionData.current_options.controller).gameObject.SetActive(true);
        controll_explainer.transform.Find("Choose/icon-" + OptionData.current_options.controller).gameObject.SetActive(true);

        RenewPreviewBoard();
    }

    //##====================================================##
    //##              Update �Z�[�u�f�[�^�I��               ##
    //##====================================================##
    void Update()
    {
        // �V�[���J�ڃA�j���[�V�������łȂ���Α���ł���
        // �J�[�\���ړ��������łȂ����
        if (mcanim.fading <= 0 && !is_effect)
        {
            int prev_choose = choose;
            // �J�[�\������Ɉړ�
            if (InputControll.GetInputDown(InputControll.INPUT_ID_UPARROW))
            {
                AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_PLAYER, AudioFilePositions.UI.CURSOR_MOVE);
                if (choose <= 0)
                    choose = menu_objs.Length - 1;
                else
                    choose -= 1;
                Renew_Board(Vector2.up,prev_choose);
            }
            // �J�[�\�������Ɉړ�
            if (InputControll.GetInputDown(InputControll.INPUT_ID_DOWNARROW))
            {
                AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_PLAYER, AudioFilePositions.UI.CURSOR_MOVE);
                if (choose >= menu_objs.Length - 1)
                    choose = 0;
                else
                    choose += 1;
                Renew_Board(Vector2.down, prev_choose);
            }

            // �Z�[�u����E���[�h����
            if (InputControll.GetInputDown(InputControll.INPUT_ID_A))
            {
                // �Z�[�u������Ȃ�
                if (Is_save_action)
                {
                    this.enabled = false;
                    SetInformation_for_preview(save_data_preview_parent, Given_playdata);
                    save_data_preview_parent.transform.DOLocalMoveY(0f, 1f).SetEase(Ease.OutCirc);
                    (saved_datas[choose] != null ? exist_data_parent : not_data_parent).transform.DOLocalMoveY(-240f, 1f).SetEase(Ease.OutCirc);
                    {

                        StartCoroutine(PopUpWindowControll.CreateSmallWindow(
                            delegate () // �͂���I�����������̓Z�[�u
                            {
                                SaveLoadControll.Seriarize(EigenValue.GetSaveDataPath() + DATAS_NAME[choose], Given_playdata);

                                SpriteRenderer spriteRenderer = menu_objs[choose].transform.Find("background").GetComponent<SpriteRenderer>();

                                save_data_preview_parent.transform.Find("transparent_ground/light").transform.DOLocalMoveZ(0f, 1f).OnComplete(() =>
                                {
                                    spriteRenderer.DOColor(new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f), 1f).SetEase(Ease.OutCirc);
                                    save_data_preview_parent.transform.Find("transparent_ground/light").transform.DOLocalMoveX(0.85f, 1f).SetEase(Ease.InOutCirc).OnComplete(() =>
                                    {
                                        mcanim.MoveScene("Menu");
                                    });
                                });
                            },
                            delegate () // ��������I��������߂�
                            {
                                save_data_preview_parent.transform.DOLocalMoveY(240f, 1f).SetEase(Ease.OutCirc);
                                (saved_datas[choose] != null ? exist_data_parent : not_data_parent).transform.DOLocalMoveY(0f, 1f).SetEase(Ease.OutCirc).OnComplete(() =>
                                {

                                    this.enabled = true;

                                });

                            },
                            this, new Vector2(180f, 120f), SAVE_CONFIRM_TEXT, transform.position + new Vector3(-90f, 0f, 0f)));
                    }

                }
                // ���[�h������A�Y���f�[�^���󂶂�Ȃ����
                else if (saved_datas[choose] != null)
                {
                    StartCoroutine(PopUpWindowControll.CreateSmallWindow(
                        delegate () // �͂���I����������
                        {

                            // �Y���̃Z�[�u�f�[�^���폜
                            if (File.Exists(EigenValue.GetSaveDataPath() + DATAS_NAME[choose]))
                            {
                                try
                                {
                                    File.Delete(EigenValue.GetSaveDataPath() + DATAS_NAME[choose]);
                                }
                                catch (Exception e)
                                {
                                    Debug.LogException(e);
                                }
                            }
                            // �I�����Ă���J���[�������񂾐F�ɂ���

                            // ���[�h�G�t�F�N�g���o���ă��C���Q�[���֑J��
                            SpriteRenderer spriteRenderer = menu_objs[choose].transform.Find("background").GetComponent<SpriteRenderer>();
                            spriteRenderer.DOColor(new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0.1f), 1f).SetEase(Ease.OutCirc);
                            exist_data_parent.transform.Find("transparent_ground/light").transform.DOLocalMoveX(0.85f, 1f).SetEase(Ease.InOutCirc).OnComplete(() =>
                            {
                                exist_data_parent.transform.DOLocalMoveY(240f, 1f).SetEase(Ease.InOutCirc).OnComplete(() =>
                                {
                                    transform.parent = null;
                                    this.gameObject.SetActive(false);
                                    DontDestroyOnLoad(this.gameObject);
                                    SceneManager.sceneLoaded += Send_datas;
                                    mcanim.MoveScene("MainGame");
                                });
                            });

                        },
                        delegate () // ��������I����������
                        {
                            // �I���ɖ߂�
                            save_data_preview_parent.transform.DOLocalMoveY(240f, 1f).SetEase(Ease.OutCirc).OnComplete(() =>
                            {
                                this.enabled = true;
                            });
                        },
                        this, new Vector2(180f, 120f), LOAD_CONFIRM_TEXT, transform.position + new Vector3(-90f, 0f, 0f)));
                }
            }

            // �Z�[�u���L�����Z�����ă��j���[�ɖ߂�
            if (InputControll.GetInputDown(InputControll.INPUT_ID_B))
            {
                if (Is_save_action)
                    StartCoroutine(PopUpWindowControll.CreateSmallWindow(
                        delegate () // �͂���I����������
                        {
                            mcanim.MoveScene("Menu");
                        },
                        delegate () // ��������I����������
                        {
                            this.enabled = true;
                        },
                        this, new Vector2(180f, 120f), NOTSAVE_CONFIRM_TEXT, transform.position + new Vector3(-90f, 0f, 0f)));

                else
                {
                    AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_PLAYER, AudioFilePositions.UI.CANCEL);
                    mcanim.MoveScene("Menu");
                }
            }
        }
    }

    //##====================================================##
    //##                ���j���[���X�V���鏈��              ##
    //##====================================================##
    void Renew_Board(Vector2 direction,int prev_choose)
    {
        is_effect = true;


        // �Z�[�u�f�[�^�|�b�v�̈ʒu�ړ�
        for(int i=0;i<menu_objs.Length;i++)
        {
            // �ł��������ꂽ�F�ɂ��Ă͍ŏ㕔�ŉ����ֈړ�
            if (Mathf.Abs(i - prev_choose) == 5)
            {
                Vector3 vec = Vector3.zero;
                vec.y = 140f * direction.y;

                menu_objs[i].transform.localPosition = vec;
            }
            else // �c��̓A�j���[�V����
                menu_objs[i].transform.DOLocalMoveY(menu_objs[i].transform.localPosition.y + 35f * direction.y * -1f, 0.25f).SetEase(Ease.OutCirc).OnComplete(() =>
                {
                    is_effect = false;
                });
        }

        RenewPreviewBoard();
    }

    //##====================================================##
    //##    ���݂̃Z�[�u�f�[�^�̃v���r���[���X�V���鏈��    ##
    //##====================================================##
    void RenewPreviewBoard() 
    {
        not_data_parent.SetActive(saved_datas[choose] == null);
        exist_data_parent.SetActive(saved_datas[choose] != null);

        if (saved_datas[choose] != null)
            SetInformation_for_preview(exist_data_parent, saved_datas[choose]);
    }

    //##====================================================##
    //##     �J�ڎ��ɌĂяo���A�v���C�f�[�^���Z�b�g����     ##
    //##====================================================##
    public void Initialize(bool is_save,PlayData play_data) 
    {
        this.Is_save_action = is_save;
        this.Given_playdata = play_data;

        title_text.text = is_save ? TITLE_TEXTS[1] : TITLE_TEXTS[0];
    }

    //##====================================================##
    //##         �v���r���[�ɏ���\�������鏈��           ##
    //##�e�I�u�W�F�N�g���󂯎��A�q��T�����Ēl���Z�b�g����##
    //##====================================================##
    void SetInformation_for_preview(GameObject parent_obj, PlayData data) 
    {
        // �v���C���[�A�C�R��
        parent_obj.transform.Find("Chara_icon").GetComponent<Image>().sprite = EigenValue.GetCharacterIcon_Sprite(data.choose_chara_id);
        // �X�e�[�W�A�C�R��
        parent_obj.transform.Find("Stage_icon").GetComponent<Image>().sprite = EigenValue.GetStageIcon_Sprite(data.choose_stage_id);
        // ���[�h�A�C�R��
        parent_obj.transform.Find("Mode_icon").GetComponent<Image>().sprite = EigenValue.GetModeIcon_Sprite(data.choose_mode_id);

        // HP�o�[
        int max_hp = EigenValue.GetCharacterStatus(data.choose_chara_id).Max_HP;
        // MP�o�[
        int max_mp = EigenValue.GetCharacterStatus(data.choose_chara_id).Max_MP;

        // �ő�HP�EMP���A�C�e���X�g�b�N���猈��
        foreach (ItemStock itemstock in data.item_stocks.item_stocks) 
        {
            if (itemstock.item_id == EigenValue.ITEM_HP_AMULET.item_id) // �̗͋F��̂��܂���������Ă�����
                max_hp += 2 * itemstock.num;
            else if (itemstock.item_id == EigenValue.ITEM_MP_AMULET.item_id) // ���͋F��̂��܂���������Ă�����
                max_mp += 2 * itemstock.num;
        }

        // HP�o�[�EMP�o�[��ݒ�
        parent_obj.transform.Find("Hp_bar/active").GetComponent<Image>().fillAmount = (float)data.hp / (float)max_hp;
        parent_obj.transform.Find("Hp_bar/text").GetComponent<Text>().text = data.hp.ToString() +"/"+ max_hp.ToString();

        parent_obj.transform.Find("Mp_bar/active").GetComponent<Image>().fillAmount = (float)data.mp / (float)max_mp;
        parent_obj.transform.Find("Mp_bar/text").GetComponent<Text>().text = data.mp.ToString() + "/" + max_mp.ToString();

        // �A�C�e���X�g�b�N
        for (int i = 0; i < data.item_stocks.item_stocks.Length; i++)
        {
            var stock = parent_obj.transform.Find("Item_Stock/StockID_" + (i + 1));

            bool is_null = data.item_stocks.item_stocks[i].item_id == 0;

            // �X�g�b�N�̉摜��ݒ�
            stock.Find("icon").GetComponent<SpriteRenderer>().sprite =
                is_null ? null : Resources.Load<Transform>((EigenValue.PREFAB_DIRECTORY_ITEMS + EigenValue.GetItemData(data.item_stocks.item_stocks[i].item_id).prefab_name)).Find("graphic").GetComponent<SpriteRenderer>().sprite;

            // �X�g�b�N���̉摜��ݒ�
            stock.Find("text").GetComponent<Text>().text =
                is_null ? "0" : data.item_stocks.item_stocks[i].num.ToString();
        }

        // �e��J�E���^�[

        // �L���J�E���g
        parent_obj.transform.Find("Kill_count/counter").GetComponent<Text>().text = data.kill_count.ToString("D9");
        // ����
        parent_obj.transform.Find("Money_count/counter").GetComponent<Text>().text = data.money.ToString("D9");
        // �X�R�A
        parent_obj.transform.Find("Score_count/counter").GetComponent<Text>().text = data.score.ToString("D9");
        // �E�F�[�u��
        parent_obj.transform.Find("Wave_count/counter").GetComponent<Text>().text = data.clear_waves.ToString();

        // �ő�E�F�[�u�� (���[�h�ɂ���ĕ\����������̂𕪊�)
        for(int i=0;i<EigenValue.IMPLEMENTED_MODES; i++) 
            parent_obj.transform.Find("Wave_count/MaxWave/mode_" + i).gameObject.SetActive(i == data.choose_mode_id);

        // �w�i�̐F��I�����Ă���J���[�f�[�^�Ɠ����ɂ���
        SpriteRenderer renderer = menu_objs[choose].transform.Find("background").GetComponent<SpriteRenderer>();

        parent_obj.transform.Find("background").GetComponent<SpriteRenderer>().color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, 1f);
    }

    //##====================================================##
    //##       �J�ڐ�(���C���Q�[�����)�փf�[�^�𑗂�       ##
    //##====================================================##
    void Send_datas(Scene next, LoadSceneMode mode)
    {
        // �J�ڐ�̃R���|�[�l���g�Ƀ��[�h�����f�[�^�𖄂ߍ���
        GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControll>().Play_data = saved_datas[choose];

        SceneManager.sceneLoaded -= Send_datas;
    }
}

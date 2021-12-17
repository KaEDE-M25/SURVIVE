using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//--====================================================--
//--        �V�[��NewGameSetting�̊Ǘ��𓝊�����        --
//--====================================================--
public class GameSettingControll : MonoBehaviour
{
    const float EFFECT_TIME = 0.5f;

    [SerializeField,Tooltip("�J�[�\����obj")]
    GameObject mode_choose_cursor;

    [SerializeField,Tooltip("�ŏI�m�F��ʂ̃J�[�\����obj")]
    GameObject final_confirm_cursor;

    [SerializeField,Tooltip("���j���[�S�̂𓮂������߂̐e�I�u�W�F�N�g")]
    GameObject menu_all;

    [Header("�����ς݂̃L�����N�^�[�̏��")]

    [SerializeField,Tooltip("�L�����N�^�[�̖��O��\������obj")]
    GameObject[] chara_list_names = new GameObject[EigenValue.IMPLEMENTED_CHARACTERS];
    [SerializeField,Tooltip("�L�����N�^�[�̃C���[�W�摜��\������obj")]
    GameObject[] chara_play_images = new GameObject[EigenValue.IMPLEMENTED_CHARACTERS];
    [SerializeField,Tooltip("�L�����N�^�[�̐�������\������obj")]
    GameObject[] chara_explain_texts = new GameObject[EigenValue.IMPLEMENTED_CHARACTERS];

    [Header("�����ς݂̃X�e�[�W�̏��")]

    [SerializeField, Tooltip("�X�e�[�W�̖��O��\������obj")]
    GameObject[] stage_list_names = new GameObject[EigenValue.IMPLEMENTED_STAGES];
    [SerializeField, Tooltip("�X�e�[�W�̃C���[�W�摜��\������obj")]
    GameObject[] stage_play_images = new GameObject[EigenValue.IMPLEMENTED_STAGES];
    [SerializeField, Tooltip("�X�e�[�W�̐�������\������obj")]
    Text stage_explain_text;

    [Header("�����ς݂̃��[�h�̏��")]

    [SerializeField,Tooltip("���[�h�̖��O��\������obj")]
    GameObject[] mode_list = new GameObject[3];

    [SerializeField,Tooltip("���[�h�̐�������\������obj")]
    Text mode_explain_obj;

    [SerializeField,Tooltip("���[�h�̐�����")]
    string[] mode_explains = new string[3];

    [Header("�ŏI�m�F��ʂŗp����obj")]

    [SerializeField,Tooltip("�͂�or������ ��obj")]
    GameObject[] final_confirm_list = new GameObject[2];
    [SerializeField,Tooltip("�ŏI�m�F��ʂ̐ݒ��\������GameObj")]
    Text[] final_confirm_setobjs = new Text[3];

    [SerializeField,Tooltip("�ݒ肵���L�����N�^�[��\������obj")]
    SpriteRenderer chara_icon;
    [SerializeField, Tooltip("�ݒ肵���X�e�[�W��\������obj(�X�e�[�W�I����ʂƍŏI�m�F���)")]
    SpriteRenderer[] stage_icons = new SpriteRenderer[2];
    [SerializeField, Tooltip("�ݒ肵�����[�h��\������obj(���[�h�I����ʂƍŏI�m�F���)")]
    SpriteRenderer[] mode_icons = new SpriteRenderer[2];

    [Header("���̑�")]

    [SerializeField, Tooltip("�����p�J�[�\�� (�㉺���E�̏���)")]
    GameObject[] decorate_cursor = new GameObject[6];

    [SerializeField, Tooltip("�V�[���J�ڃA�j���[�V�����̃R���|�[�l���g�B")]
    MoveSceneAnimation mcanim;

    // �I�������v�f��ۑ����Ă����ϐ�
    int choose_chara_id = 1;
    int choose_stage_id = 1;
    int choose_mode_id = 1;

    Tween tween;
    readonly Tween[] decorate_cursors_tweens = new Tween[6];

    // ��ʂ̐i�s�� (0 = �L�����N�^�[�I���A1 = �X�e�[�W�I���A2 = ���[�h�I���A3 = �ŏI����)
    int proceed = 0;
    int choose = 0;
    int choose_max = 0;
    bool is_move = false;
    bool is_effect = false;

    // �v���C�f�[�^(�]���p)
    PlayData initialize_data = null;

    //##====================================================##
    //##                     Start ������                   ##
    //##====================================================##
    void Start()
    {
        choose_max = EigenValue.IMPLEMENTED_CHARACTERS - 1;

        decorate_cursors_tweens[0] = decorate_cursor[0].transform.DOLocalMoveY(50f, 0.5f).SetEase(Ease.OutCirc).SetLoops(-1, LoopType.Yoyo);
        decorate_cursors_tweens[1] = decorate_cursor[1].transform.DOLocalMoveY(-50f, 0.5f).SetEase(Ease.OutCirc).SetLoops(-1, LoopType.Yoyo);
        decorate_cursors_tweens[2] = decorate_cursor[2].transform.DOLocalMoveX(-70f, 0.5f).SetEase(Ease.OutCirc).SetLoops(-1, LoopType.Yoyo);
        decorate_cursors_tweens[3] = decorate_cursor[3].transform.DOLocalMoveX(70f, 0.5f).SetEase(Ease.OutCirc).SetLoops(-1, LoopType.Yoyo);
        decorate_cursors_tweens[4] = decorate_cursor[4].transform.DOLocalMoveX(-85f, 0.5f).SetEase(Ease.OutCirc).SetLoops(-1, LoopType.Yoyo);
        decorate_cursors_tweens[5] = decorate_cursor[5].transform.DOLocalMoveX(-27f, 0.5f).SetEase(Ease.OutCirc).SetLoops(-1, LoopType.Yoyo);
    }


    //##====================================================##
    //##                 Update ���j���[�I��                ##
    //##====================================================##
    void Update()
    {
        // �V�[���J�ڃA�j���[�V�������łȂ���Α���ł���
        if (mcanim.fading <= 0)
        {
            // ��ʑJ�ڒ��������̓J�[�\���ړ��G�t�F�N�g���������łȂ����
            if (!is_move && !is_effect)
            {
                // �J�[�\������Ɉړ� (�X�e�[�W�I����ʂ̂݉E�L�[�ňړ�������)
                if (InputControll.GetInputDown(proceed == 1 ? InputControll.INPUT_ID_RIGHTARROW : InputControll.INPUT_ID_UPARROW))
                {
                    AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_UI, AudioFilePositions.UI.CURSOR_MOVE);
                    int prev_choose = choose;
                    if (choose <= 0)
                        choose = choose_max;
                    else
                        choose -= 1;
                    Renew_Board(prev_choose, false);
                }
                // �J�[�\�������Ɉړ� (�X�e�[�W�I����ʂ̂ݍ��L�[�ňړ�������)
                else if (InputControll.GetInputDown(proceed == 1 ? InputControll.INPUT_ID_LEFTARROW : InputControll.INPUT_ID_DOWNARROW))
                {
                    AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_UI, AudioFilePositions.UI.CURSOR_MOVE);
                    int prev_choose = choose;
                    if (choose >= choose_max)
                        choose = 0;
                    else
                        choose += 1;
                    Renew_Board(prev_choose, true);
                }

                // ����L�[����������J��
                else if (InputControll.GetInputDown(InputControll.INPUT_ID_A))
                {
                    //�ŏI�����ʈȊO�ɂ��Ă�
                    if (proceed < 3)
                    {
                        switch (proceed)
                        {
                            case 0: // �L�����I���Ȃ�
                                {
                                    if (choose == 1)
                                    {
                                        StartCoroutine(CautionWindowControll.CreateSmallWindow(this, new Vector2(200f, 120f), "���̃L�����N�^�[�͂܂���������Ă��܂���I", Vector2.zero));
                                        return;
                                    }

                                    // �I�������L������ۑ�
                                    choose_chara_id = choose + 1;
                                    choose_max = EigenValue.IMPLEMENTED_STAGES - 1;
                                    choose = choose_stage_id - 1;

                                    // �A�C�R���𔽉f
                                    chara_icon.sprite = chara_list_names[choose_chara_id - 1].GetComponent<SpriteRenderer>().sprite;

                                    break;
                                }
                            case 1: // �X�e�[�W�I���Ȃ�
                                {
                                    if (choose == 1)
                                    {
                                        StartCoroutine(CautionWindowControll.CreateSmallWindow(this, new Vector2(200f, 120f), "���̃X�e�[�W�͂܂���������Ă��܂���I", Vector2.zero));
                                        return;
                                    }
                                    // �I�������X�e�[�W��ۑ�
                                    choose_stage_id = choose + 1;
                                    choose = choose_mode_id - 1;
                                    choose_max = 3 - 1;

                                    break;
                                }
                            case 2: // ���[�h�I���Ȃ�
                                {
                                    // �I���������[�h��ۑ�
                                    choose_mode_id = choose;

                                    final_confirm_setobjs[0].text = EigenValue.GetCharacterStatus(choose_chara_id).Name;
                                    final_confirm_setobjs[1].text = EigenValue.GetStageData(choose_stage_id).stage_name;
                                    final_confirm_setobjs[2].text = mode_list[choose_mode_id].GetComponent<Text>().text;

                                    // �J�[�\���ʒu��������
                                    choose = 0;
                                    final_confirm_cursor.transform.SetParent(final_confirm_list[0].transform);
                                    Vector3 vec = final_confirm_cursor.transform.localPosition;
                                    vec.y = 0f;
                                    final_confirm_cursor.transform.localPosition = vec;

                                    choose_max = 2 - 1;
                                    break;
                                }
                            default:
                                break;
                        }

                        AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_UI, AudioFilePositions.UI.DECISION);

                        proceed++;
                        is_move = true;
                        menu_all.transform.DOMoveX(proceed * -360f, 1f).SetEase(Ease.OutCirc).OnComplete(() =>
                        {
                            is_move = false;
                        });
                    }
                    else //�ŏI�����ʂł� 
                    {
                        switch (choose)
                        {
                            case 0: // �͂�
                                {
                                    // ���C���Q�[����ʂ�
                                    GameStart();
                                    break;
                                }
                            case 1: // ������
                                {
                                    AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_UI, AudioFilePositions.UI.CANCEL);
                                    Cancel();
                                    break;
                                }
                        }
                    }
                }
                // �L�����Z���L�[�������ꂽ���O�̉�ʂ֖߂�
                else if (InputControll.GetInputDown(InputControll.INPUT_ID_B))
                {
                    AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_UI, AudioFilePositions.UI.CANCEL);
                    Cancel();
                }
            }
        }
    }

    //##====================================================##
    //##                ���j���[���X�V���鏈��              ##
    //##====================================================##
    void Renew_Board(int prev_choose ,bool up)
    {
        is_effect = true;

        // ���݂̉�ʂɉ����ē���
        switch (proceed) 
        {
            case 0:  // �L�����I�����
                {
                    // �A�j���[�V���������\���ύX

                        // �L�����l�[��

                    // �I���X�V�O�̂��
                    chara_list_names[prev_choose].GetComponent<Text>().DOColor(Color.clear, EFFECT_TIME).SetEase(Ease.OutCirc);
                    chara_list_names[prev_choose].transform.DOLocalMoveY(20f * (up ? -1f : 1f), EFFECT_TIME).SetEase(Ease.OutCirc).OnComplete(()=> 
                    {
                        chara_list_names[prev_choose].SetActive(false);
                    });
                    // �I���X�V��̂��
                    chara_list_names[choose].transform.localPosition = (up ? Vector3.up : Vector3.down) * 20f;
                    Text text = chara_list_names[choose].GetComponent<Text>();
                    text.color = Color.clear;
                    chara_list_names[choose].SetActive(true);
                    text.DOColor(Color.black, EFFECT_TIME).SetEase(Ease.OutCirc);
                    chara_list_names[choose].transform.DOLocalMoveY(0f, EFFECT_TIME).SetEase(Ease.OutCirc).OnComplete(() =>
                    {
                        is_effect = false;
                    });

                        // �C���[�W�摜

                    // �I���X�V�O�̂��
                    chara_play_images[prev_choose].GetComponent<CanvasGroup>().DOFade(0f, EFFECT_TIME / 2f).SetEase(Ease.InSine).OnComplete(() =>
                    {
                        chara_list_names[prev_choose].SetActive(false);
                        // �I���X�V��̂��
                        chara_play_images[choose].SetActive(true);
                        chara_play_images[choose].GetComponent<CanvasGroup>().DOFade(1f, EFFECT_TIME / 2f).SetEase(Ease.OutSine);

                        // �e�L�X�g�������ŏ���
                        tween?.Complete();
                        tween?.Kill();
                        chara_explain_texts[prev_choose].SetActive(false);
                        chara_explain_texts[choose].SetActive(true);
                        Text text_comp = chara_explain_texts[choose].GetComponent<Text>();
                        string text = text_comp.text;
                        text_comp.text = "";
                        tween = chara_explain_texts[choose].GetComponent<Text>().DOText(text, 0.01f * text.Length).SetEase(Ease.Linear);
                    });
                    break;
                }
            case 1:  // �X�e�[�W�I�����
                {
                    // �A�j���[�V���������\���ύX

                        // �X�e�[�W�l�[��

                    // �I���X�V�O�̂��
                    stage_list_names[prev_choose].GetComponent<Text>().DOColor(Color.clear, EFFECT_TIME).SetEase(Ease.OutCirc);
                    stage_list_names[prev_choose].transform.DOLocalMoveX(60f * (up ? 1f : -1f), EFFECT_TIME).SetEase(Ease.OutCirc).OnComplete(() =>
                    {
                        stage_list_names[prev_choose].SetActive(false);

                    });
                    // �I���X�V��̂��
                    stage_list_names[choose].transform.localPosition = (up ? Vector3.left : Vector3.right) * 60f;
                    Text text = stage_list_names[choose].GetComponent<Text>();
                    text.color = Color.clear;
                    stage_list_names[choose].SetActive(true);
                    text.DOColor(Color.black, EFFECT_TIME).SetEase(Ease.OutCirc);
                    stage_list_names[choose].transform.DOLocalMoveX(0f, EFFECT_TIME).SetEase(Ease.OutCirc).OnComplete(() =>
                    {
                        is_effect = false;
                    });

                        // �C���[�W�摜

                    // �I���X�V�O�̂��
                    stage_play_images[prev_choose].GetComponent<CanvasGroup>().DOFade(0f, EFFECT_TIME / 2f).SetEase(Ease.InSine).OnComplete(() =>
                    {
                        stage_list_names[prev_choose].SetActive(false);
                        // �I���X�V��̂��
                        stage_play_images[choose].SetActive(true);
                        stage_play_images[choose].GetComponent<CanvasGroup>().DOFade(1f, EFFECT_TIME / 2f).SetEase(Ease.OutSine);

                        // �e�L�X�g�������ŏ���
                        tween?.Complete();
                        tween?.Kill();
                        Text text_comp = stage_explain_text.GetComponent<Text>();
                        string next_stage_comment = EigenValue.GetStageData(choose + 1).stage_comment;
                        text_comp.text = "";
                        tween = stage_explain_text.DOText(next_stage_comment, 0.01f * next_stage_comment.Length).SetEase(Ease.Linear);

                        // �A�C�R��
                        Sprite new_sprite = EigenValue.GetStageIcon_Sprite(choose + 1);

                        foreach (SpriteRenderer stage_icon in stage_icons)
                        {
                            stage_icon.sprite = new_sprite;
                        }
                    });
                    break;
                }
            case 2: // ���[�h�I�����
                {

                    // �J�[�\���̈ʒu��ύX
                    mode_choose_cursor.transform.SetParent(mode_list[choose].transform);
                    Vector3 vec = mode_choose_cursor.transform.localPosition;
                    vec.y = 0f;
                    mode_choose_cursor.transform.localPosition = vec;
                    mode_explain_obj.text = "";
                    
                    // �e�L�X�g����������
                    tween?.Kill();
                    tween = mode_explain_obj.DOText(mode_explains[choose], 0.01f * mode_explains[choose].Length).SetEase(Ease.Linear);

                    // �A�C�R��
                    Sprite new_sprite = EigenValue.GetModeIcon_Sprite(choose);

                    foreach (SpriteRenderer mode_icon in mode_icons)
                    {
                        mode_icon.sprite = new_sprite;
                    }

                    is_effect = false;
                    break;
                }
            case 3:
                {

                    // �J�[�\���̈ʒu��ύX
                    final_confirm_cursor.transform.SetParent(final_confirm_list[choose].transform);
                    Vector3 vec = final_confirm_cursor.transform.localPosition;
                    vec.y = 0f;
                    final_confirm_cursor.transform.localPosition = vec;

                    is_effect = false;
                    break;
                }

            default:
                break;
        }
    }

    //##====================================================##
    //##                ��O�̉�ʂ֖߂�                  ##
    //##====================================================##
    void Cancel() 
    {
        if (proceed <= 0)
        {
            // ���C�����j���[�ɖ߂�
            for (int i = 0; i < decorate_cursors_tweens.Length; i++)
                decorate_cursors_tweens[i]?.Kill();

            mcanim.MoveScene("Menu");
            return;
        }

        proceed--;
        switch (proceed)
        {
            case 0: // �L�����I���Ȃ�
                {
                    choose_stage_id = choose + 1;
                    choose_max = EigenValue.IMPLEMENTED_CHARACTERS - 1;
                    choose = choose_chara_id - 1;
                    break;
                }
            case 1: // �X�e�[�W�I���Ȃ�
                {
                    choose_mode_id = choose + 1;
                    choose_max = EigenValue.IMPLEMENTED_STAGES - 1;
                    choose = choose_stage_id - 1;
                    break;
                }
            case 2: // ���[�h�I���Ȃ�
                {
                    choose_max = 3 - 1;
                    choose = choose_mode_id;
                    break;
                }
            default:
                break;
        }

        is_move = true;
        menu_all.transform.DOMoveX(proceed * -360f, 1f).SetEase(Ease.OutCirc).OnComplete(() =>
        {
            is_move = false;
        });
    }

    //##====================================================##
    //##                �Q�[�����J�n���鏈��                ##
    //##====================================================##
    void GameStart()
    {
        AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_UI, AudioFilePositions.UI.DECISION);
        for (int i = 0; i < decorate_cursors_tweens.Length; i++)
            decorate_cursors_tweens[i]?.Kill();

        initialize_data = new PlayData(choose_chara_id, choose_stage_id, choose_mode_id, EigenValue.GetCharacterStatus(choose_chara_id).Max_HP, EigenValue.GetCharacterStatus(choose_chara_id).Max_MP, 0, 0, 0, 0, new ItemStocks());
        SceneManager.sceneLoaded += Send_datas;

        mcanim.MoveScene("MainGame");
    }

    //##====================================================##
    //##       �J�ڐ�(���C���Q�[�����)�փf�[�^�𑗂�       ##
    //##====================================================##
    void Send_datas(Scene next, LoadSceneMode mode)
    {

        GameControll gameControll = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControll>();
        gameControll.Play_data = initialize_data;

        SceneManager.sceneLoaded -= Send_datas;
    }
}
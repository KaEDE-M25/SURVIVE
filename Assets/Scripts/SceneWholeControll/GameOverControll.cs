using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//--====================================================--
//--           �V�[��GameOver�̊Ǘ��𓝊�����           --
//--====================================================--
public class GameOverControll : MonoBehaviour
{
    static readonly string RETRY_CONFIRM_TEXT = "�����ݒ�ł�����x�Q�[�����n�߂܂��B��낵���ł����H";

    [Header("���ʕ\���G�t�F�N�g�P�i�ڂŎg������")]

    [SerializeField,Tooltip("GAMEOVER..�@�̃e�L�X�g��\������obj")]
    GameObject gameover_title;
    [SerializeField,Tooltip("GAMECLEAR!�@�̃e�L�X�g��\������obj")]
    GameObject gameclear_title;

    [Header("���ʕ\���G�t�F�N�g�Q�i�ڂŎg������")]

    [SerializeField,Tooltip("���l�n�̃X�R�A�������������\������G�t�F�N�g�̐eobj�i�X�R�A�A�L���A�E�F�[�u�j")]
    GameObject result_ef;

    [Header("���ʕ\���G�t�F�N�g�R�i�ڂŎg������")]

    [SerializeField,Tooltip("���l�n�̃X�R�A�̎��ۂ̒l��\������G�t�F�N�g�̐eobj")]
    GameObject results;
    [SerializeField,Tooltip("�X�R�A���ےl��text�̉��Ɉ����Ă��鉺��")]
    GameObject results_line;
    [SerializeField,Tooltip("�I����obj")]
    GameObject choices;
    [SerializeField,Tooltip("�Q�[���ݒ��\������eobj")]
    GameObject settings;

    [Header("���̑�")]

    [SerializeField,Tooltip("�I�����̃J�[�\��")]
    GameObject cursor;
    [SerializeField,Tooltip("���j���[")]
    GameObject[] menu_objs = new GameObject[3];
    [SerializeField,Tooltip("�V�[���J�ڃA�j���[�V�����̃R���|�[�l���g�B")]
    MoveSceneAnimation mcanim;
    
    // �Q�[���N���A���Q�[���I�[�o�[���ǂ��炩
    bool is_clear = true;

    // ���ʕ\���G�t�F�N�g�̐i�s�x
    int proceed = 0;

    // ���j���[�őI�𒆂̑I����
    int choose = -1;
    Tween tween;

    // ���C���Q�[���V�[������󂯎�����Q�[���f�[�^
    PlayData recieved_data;
    // �������p�Q�[���f�[�^
    PlayData initialize_data;

    //##====================================================##
    //##                     Start ������                   ##
    //##====================================================##
    private void Start()
    {
        tween = cursor.transform.DOLocalMoveX(cursor.transform.localPosition.x + 3f, 0.5f).SetEase(Ease.OutCirc).SetLoops(-1, LoopType.Yoyo);
        // ���ʕ\���G�t�F�N�g�����s
        Invoke(nameof(EffectProceed), 1f);
    }

    //##====================================================##
    //##                 Update ���j���[�I��                ##
    //##====================================================##
    void Update()
    {
        // �V�[���J�ڃA�j���[�V�������łȂ���Α���ł���
        if (mcanim.fading <= 0)
        {
            // �J�[�\������Ɉړ�
            if (InputControll.GetInputDown(InputControll.INPUT_ID_UPARROW))
            {
                AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_PLAYER, AudioFilePositions.UI.CURSOR_MOVE);
                if (choose <= 0)
                    choose = menu_objs.Length - 1;
                else
                    choose -= 1;
                Renew_Board();
            }
            // �J�[�\�������Ɉړ�
            if (InputControll.GetInputDown(InputControll.INPUT_ID_DOWNARROW))
            {
                AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_PLAYER, AudioFilePositions.UI.CURSOR_MOVE);
                if (choose >= menu_objs.Length - 1)
                    choose = 0;
                else
                    choose += 1;
                Renew_Board();
            }

            // ����L�[����������J��
            if (InputControll.GetInputDown(InputControll.INPUT_ID_A))
            {
                switch (choose)
                {
                    case 0: // ���j���[�ɖ߂�
                        {
                            mcanim.MoveScene("Menu");
                            tween?.Kill();
                            break;
                        }
                    case 1: // �����ݒ�ł�����x�͂��߂�
                        {
                            StartCoroutine(PopUpWindowControll.CreateSmallWindow(
                            delegate () // �͂���I����������
                            {
                                initialize_data = new PlayData(recieved_data.choose_chara_id, recieved_data.choose_stage_id, recieved_data.choose_mode_id, EigenValue.GetCharacterStatus(recieved_data.choose_chara_id).Max_HP, EigenValue.GetCharacterStatus(recieved_data.choose_chara_id).Max_MP, 0, 0, 0, 0, new ItemStocks());
                                SceneManager.sceneLoaded += Send_datas;

                                mcanim.MoveScene("MainGame");
                            },
                            delegate () // ��������I����������
                            {
                                this.enabled = true;
                            },
                            this, new Vector2(180f, 120f), RETRY_CONFIRM_TEXT, transform.position + new Vector3(0f, 0f, 0f)));

                            return;
                        }
                    case 2: // �V�F�A����
                        {

                            // �܂�������
                            StartCoroutine(CautionWindowControll.CreateSmallWindow(this, new Vector2(200f, 120f), "���̋@�\�͂܂���������Ă��܂���I", Vector2.zero));
                            return;
                        }
                    default:
                        break;

                }
                AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_PLAYER, AudioFilePositions.UI.DECISION);
            }
        }
    }

    //##====================================================##
    //##�v���C���ʂ��󂯎��Z�b�g����(�V�[���J�ڂ̍ۂɎg�p)##
    //##====================================================##
    public void SetResult(PlayData datas,bool is_clear) 
    {
        results.transform.Find("Score").GetComponent<Text>().text = datas.score.ToString("D10");
        results.transform.Find("KillCount").GetComponent<Text>().text = datas.kill_count.ToString("D8");
        results.transform.Find("WaveCount").GetComponent<Text>().text = datas.clear_waves.ToString("D8");

        settings.transform.Find("Setting_battle_chara").GetComponent<SpriteRenderer>().sprite = EigenValue.GetCharacterIcon_Sprite(datas.choose_chara_id);
        settings.transform.Find("Setting_stage").GetComponent<SpriteRenderer>().sprite = EigenValue.GetStageIcon_Sprite(datas.choose_stage_id);
        settings.transform.Find("Setting_mode").GetComponent<SpriteRenderer>().sprite = EigenValue.GetModeIcon_Sprite(datas.choose_mode_id);

        this.is_clear = is_clear;

        // �����ݒ�ł�����x�v���C����p�Ƀv���C�f�[�^���܂Ƃ߂Ď󂯎���Ă���
        recieved_data = datas;
    }

    //##====================================================##
    //##            ���ʕ\���G�t�F�N�g��i�s����            ##
    //##====================================================##
    public void EffectProceed() 
    {
        switch (proceed) 
        {
            case 0: // �^�C�g��
                {
                    if (is_clear)
                    {
                        gameclear_title.SetActive(true);
                        gameclear_title.transform.DOLocalMoveY(0f, 0.5f).OnComplete(() =>
                        {
                            Transform light = gameclear_title.transform.Find("Light");
                            light.DOLocalMoveX(light.localPosition.x * -1f, 1f).SetEase(Ease.InOutCirc).OnComplete(() => 
                            {
                                EffectProceed();
                            });
                        });
                    }
                    else
                    {
                        gameover_title.SetActive(true);
                        gameover_title.transform.DOLocalMoveY(0f, 2f).OnComplete(() => { EffectProceed(); });
                    }
                    break;
                }
            case 1: // �X�R�A�̑������o��
                {
                    result_ef.SetActive(true);
                    results_line.transform.DOScaleX(300f, 1f).SetEase(Ease.InOutCirc);
                    (is_clear ? gameclear_title : gameover_title).transform.DOLocalMoveY(70f, 1f).SetEase(Ease.InOutCirc).OnComplete(() => { EffectProceed(); });
                
                    break;
                }
            case 2: // �c��i���ۂ̃X�R�A���l�Ɛݒ�ƃ��j���[�j���o��
                {
                    results.SetActive(true);
                    choices.transform.DOLocalMoveX(-60, 1f).SetEase(Ease.OutSine);
                    settings.transform.DOLocalMoveX(90, 1f).SetEase(Ease.OutSine).OnComplete(()=> 
                    {
                        choose = 0;
                    
                    });
                    break;
                }
            default:
                break;
        }

        ++proceed;

    }

    //##====================================================##
    //##                ���j���[���X�V���鏈��              ##
    //##====================================================##
    void Renew_Board()
    {
        cursor.transform.SetParent(menu_objs[choose].transform);
        Vector3 vec = cursor.transform.localPosition;
        vec.y = 0f;
        cursor.transform.localPosition = vec;
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

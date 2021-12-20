using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//--====================================================--
//--            �V�[��Menu�̊Ǘ��𓝊�����              --
//--====================================================--
public class MenuControll : MonoBehaviour
{
    [SerializeField,Tooltip("�I�����̐�������obj")]
    Text explain_text;
    [SerializeField,Tooltip("���j���[�̃J�[�\����obj")]
    GameObject cursor;
    
    [SerializeField,Tooltip("�I�����̐�����")]
    string[] explains = new string[5];

    [SerializeField,Tooltip("�I�����̃C���[�W�摜")]
    Image[] explain_images = new Image[5];

    [SerializeField,Tooltip("���j���[�̑I������obj")]
    GameObject[] menu_objs = new GameObject[5];

    [SerializeField,Tooltip("�V�[���J�ڃA�j���[�V�����̃R���|�[�l���g")]
    MoveSceneAnimation mcanim;

    Tween tween;
    Tween cursor_tween;
    int choose = 0;

    //##====================================================##
    //##                     Start ������                   ##
    //##====================================================##
    void Start()
    {
        cursor_tween = cursor.transform.DOLocalMoveX(cursor.transform.localPosition.x + 3f, 0.5f).SetEase(Ease.OutCirc).SetLoops(-1, LoopType.Yoyo);

        explain_text.text = explains[0];
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
                AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_UI, AudioFilePositions.UI.CURSOR_MOVE);
                if (choose <= 0)
                    choose = menu_objs.Length - 1;
                else
                    choose -= 1;
                Renew_Board();
            }
            // �J�[�\�������Ɉړ�
            else if (InputControll.GetInputDown(InputControll.INPUT_ID_DOWNARROW))
            {
                AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_UI, AudioFilePositions.UI.CURSOR_MOVE);
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
                    case 0: //�͂��߂���
                        {
                            mcanim.MoveScene("NewGameSetting");
                            break;
                        }
                    case 1: //�Â�����
                        {
                            mcanim.MoveScene("SaveLoadGame");
                            break;
                        }
                    case 2: //�����X�^�[������ (������)
                        {
                            StartCoroutine(CautionWindowControll.CreateSmallWindow(this, new Vector2(200f, 120f), "�܂���������Ă��܂���I", Vector2.zero));
                            return;
                        }
                    case 3: // �N���W�b�g (������)
                        {
                            StartCoroutine(CautionWindowControll.CreateSmallWindow(this, new Vector2(200f, 120f), "�܂���������Ă��܂���I", Vector2.zero));
                            return;
                        }
                    case 4: //�I�v�V����
                        {
                            mcanim.MoveScene("OptionChange");
                            break;
                        }

                    default:
                        throw new System.Exception("Invalid menu content.");

                }

                AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_UI, AudioFilePositions.UI.DECISION);
                tween?.Kill();
                cursor_tween?.Kill();
            }
        }
    }

    //##====================================================##
    //##                ���j���[���X�V���鏈��              ##
    //##====================================================##
    void Renew_Board()
    {
        explain_text.text = "";

        cursor.transform.SetParent(menu_objs[choose].transform);
        Vector3 vec = cursor.transform.localPosition;
        vec.y = 0f;
        cursor.transform.localPosition = vec;
        
        for(int i=0;i < explain_images.Length;i++)
            explain_images[i].gameObject.SetActive(i == choose);
        
        tween?.Kill();
        tween = explain_text.DOText(explains[choose], 0.01f * explains[choose].Length).SetEase(Ease.Linear);
    }
}

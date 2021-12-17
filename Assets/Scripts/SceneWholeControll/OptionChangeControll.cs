using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

//--====================================================--
//--         �V�[��OptionChange�̊Ǘ��𓝊�����         --
//--====================================================--
public class OptionChangeControll : MonoBehaviour
{
    public const int IMPLEMENTED_OPTION = 4;

    [SerializeField,Tooltip("�J�[�\����obj(Tween��K�p���郊�X�g)")]
    GameObject[] cursors;

    [SerializeField,Tooltip("���j���[�̑I������obj")]
    GameObject[] menu_objs = new GameObject[5];

    [SerializeField,Tooltip("�I�v�V�����̌��ݒl��\������e�L�X�gobj")]
    Text[] change_item_objs = new Text[4];

    [SerializeField,Tooltip("�V�[���J�ڃA�j���[�V�����̃R���|�[�l���g")]
    MoveSceneAnimation mcanim;

    // �e�I�v�V�����̑I������\�����郁�b�Z�[�W
    string[][] chooseMessages;
    readonly Tween[] cursor_tweens = new Tween[3];
    int choose = 0;
    // �e�I�����ɂ�����I�����
    int[] sub_choose;

    //##====================================================##
    //##                     Awake ������                   ##
    //##====================================================##
    private void Awake()
    {
        // ���b�Z�[�W�������ł���
        chooseMessages = new string[IMPLEMENTED_OPTION][];


        // WINDOWS���ł̓R���g���[���[����ȏ�ڑ�����Ă�����I�������Q�ɂ���
        #if UNITY_STANDALONE_WIN
        //Debug.Log("WINDOWS�ŋN�����Ă��܂�");
        var connected_controllers = CountController(Input.GetJoystickNames());
        //Debug.Log(connected_controllers);
        if(connected_controllers <= 0)
            chooseMessages[0] = new string[1] { "KEYBOARD"};
        else
            chooseMessages[0] = new string[2] { "KEYBOARD", "GAMEPAD"};
        //ANDROID��������IOS��
        #elif UNITY_ANDROID || UNITY_IOS
        //Debug.Log("�X�}�z�ŋN�����Ă��܂�");
        chooseMessages[0] = new string[1] { "SCREENPAD"};
        #else
        //Debug.Log("���̑��̂Ȃɂ��ŋN�����Ă��܂�");
        chooseMessages[0] = new string[3] { "KEYBOARD", "GAMEPAD","SCREENPAD"};
        #endif

        chooseMessages[1] = new string[2] { "�n�m", "�n�e�e" };
        chooseMessages[2] = new string[2] { "�ӂ�", "������"};
        chooseMessages[3] = new string[2] { "�ӂ�", "�ȗ�" };

        sub_choose = new int[change_item_objs.Length];
    }

    //##====================================================##
    //##             �R���g���[���̐����v������             ##
    //##====================================================##
    int CountController(string[] controller_names) 
    {
        int count = 0;
        foreach (string controller_name in controller_names)
            if (!controller_name.Equals("")) count++;

        return count;
    }

    //##====================================================##
    //##                  OnEnable ������                   ##
    //##====================================================##
    void OnEnable()
    {
        cursor_tweens[0] = cursors[0].transform.DOLocalMoveX(cursors[0].transform.localPosition.x + 3f, 0.5f).SetEase(Ease.OutCirc).SetLoops(-1, LoopType.Yoyo);
        cursor_tweens[1] = cursors[1].transform.DOLocalMoveX(cursors[1].transform.localPosition.x - 3f, 0.5f).SetEase(Ease.OutCirc).SetLoops(-1, LoopType.Yoyo);
        cursor_tweens[2] = cursors[2].transform.DOLocalMoveX(cursors[2].transform.localPosition.x + 3f, 0.5f).SetEase(Ease.OutCirc).SetLoops(-1, LoopType.Yoyo);
        sub_choose[0] = OptionData.current_options.controller switch
        {
            OptionData.CONTROLLER_NAME_KEYBOARD => Array.IndexOf(chooseMessages[0], "KEYBOARD"),
            OptionData.CONTROLLER_NAME_GAMEPAD => Array.IndexOf(chooseMessages[0], "KEYBOARD"),
            OptionData.CONTROLLER_NAME_SCREENPAD => Array.IndexOf(chooseMessages[0], "KEYBOARD"),
            _ => throw new Exception("�����ȃI�v�V�����ł�"),
        };
        sub_choose[1] = OptionData.current_options.is_play_shake ? 0 : 1;
        sub_choose[2] = OptionData.current_options.dead_chara_color;
        sub_choose[3] = OptionData.current_options.omitted_effect ? 1 : 0;

        Renew_Board();
    }

    //##====================================================##
    //##                Update �I�v�V�����I��               ##
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
            else if (InputControll.GetInputDown(InputControll.INPUT_ID_DOWNARROW))
            {
                AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_PLAYER, AudioFilePositions.UI.CURSOR_MOVE);
                if (choose >= menu_objs.Length - 1)
                    choose = 0;
                else
                    choose += 1;
                Renew_Board();
            }

            // �߂��I�����Ă��Ȃ����
            if (choose != menu_objs.Length - 1) 
            {
                if (InputControll.GetInputDown(InputControll.INPUT_ID_LEFTARROW))
                {
                    AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_PLAYER, AudioFilePositions.UI.CURSOR_MOVE);
                    if (sub_choose[choose] <= 0)
                        sub_choose[choose] = chooseMessages[choose].Length - 1;
                    else
                        sub_choose[choose] -= 1;
                    Renew_Board();
                }
                // �J�[�\�������Ɉړ�
                else if (InputControll.GetInputDown(InputControll.INPUT_ID_RIGHTARROW))
                {
                    AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_PLAYER, AudioFilePositions.UI.CURSOR_MOVE);
                    if (sub_choose[choose] >= chooseMessages[choose].Length - 1)
                        sub_choose[choose] = 0;
                    else
                        sub_choose[choose] += 1;
                    Renew_Board();
                }
            }

            // ���j���[�ɖ߂��I��������߂�
            if (InputControll.GetInputDown(InputControll.INPUT_ID_A))
            {
                if (choose == menu_objs.Length - 1)
                {
                    AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_PLAYER, AudioFilePositions.UI.DECISION);
                    SaveOption_and_Return_Menu();
                }
            }
            // B�{�^���Ŗ߂�
            if (InputControll.GetInputDown(InputControll.INPUT_ID_B))
            {
                AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_PLAYER, AudioFilePositions.UI.CANCEL);
                SaveOption_and_Return_Menu();
            }
        }
    }


    //##====================================================##
    //##          �I�v�V������ۑ����A���j���[�ɖ߂�        ##
    //##====================================================##
    void SaveOption_and_Return_Menu() 
    {
        // Tween���~�߂�
        for (int i = 0; i < cursor_tweens.Length; i++)
            cursor_tweens[i]?.Kill();


        // �I�������I�v�V�����𔽉f
        OptionData.current_options.controller = (chooseMessages[0][sub_choose[0]]) switch
        {
            "KEYBOARD" => OptionData.CONTROLLER_NAME_KEYBOARD,
            "GAMEPAD" => OptionData.CONTROLLER_NAME_GAMEPAD,
            "SCREENPAD" => OptionData.CONTROLLER_NAME_SCREENPAD,
            _ => throw new Exception("�����ȃI�v�V�����ł�"),
        };
        OptionData.current_options.is_play_shake = sub_choose[1] == 0;
        OptionData.current_options.dead_chara_color = sub_choose[2];
        OptionData.current_options.omitted_effect = sub_choose[3] == 1;

        // �t�@�C���ɕۑ�
        SaveLoadControll.Seriarize<OptionData>(EigenValue.GetSaveDataPath() + EigenValue.OPTIONDATA_PATH_NAME, OptionData.current_options);

        // �Q�[���p�b�h���g������ł̓Q�[���p�b�h�ݒ���ۑ�
        #if !(UNITY_ANDROID || UNITY_IOS)
        SaveLoadControll.Seriarize<Joystick_Setting>(EigenValue.GetSaveDataPath() + EigenValue.JOYSTICKDATA_PATH_NAME, InputControll.joystick_setting);
        #endif
        
        // ���j���[�֑J��
        mcanim.MoveScene("Menu");
    }

    //##====================================================##
    //##                ���j���[���X�V���鏈��              ##
    //##====================================================##
    void Renew_Board()
    {
        cursors[0].transform.SetParent(menu_objs[choose].transform);
        Vector3 vec = cursors[0].transform.localPosition;
        vec.y = 0f;
        cursors[0].transform.localPosition = vec;

        for (int i = 1; i < cursors.Length; i++)
        {
            // ���j���[�ɖ߂�@��I�����Ă��邾���E���̂Q�̃J�[�\���͏���
            if (choose == menu_objs.Length - 1)
            {
                cursors[i].SetActive(false);
            }
            else
            {
                cursors[i].SetActive(true);
                cursors[i].transform.SetParent(change_item_objs[choose].transform);
                vec = cursors[i].transform.localPosition;
                vec.y = 0f;
                cursors[i].transform.localPosition = vec;
            }
        }

        // ���݂̐ݒ�\�����X�V
        for(int i = 0; i < change_item_objs.Length; i++) 
        {
            change_item_objs[i].text = chooseMessages[i][sub_choose[i]];
        }
    }
}

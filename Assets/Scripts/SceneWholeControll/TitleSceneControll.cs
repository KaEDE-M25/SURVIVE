using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.IO;

//--====================================================--
//--            �V�[��Title�̊Ǘ��𓝊�����             --
//--====================================================--
public class TitleSceneControll : MonoBehaviour
{
    [SerializeField,Tooltip("�^�C�g�����S��IMAGE�R���|�[�l���g")]
    Image title_logo;
    [SerializeField,Tooltip("PRESS ANY BUTTON��TEXT�R���|�[�l���g")]
    Text press_any_button;
    [SerializeField, Tooltip("�V�[���J�ڃA�j���[�V�����̃R���|�[�l���g")]
    MoveSceneAnimation mcanim;

    bool start_move = false;
    float time = 0f;

    [Header("�e�X�g�p")]
    [SerializeField,Tooltip("�X�N���[���T�C�Y��\������text�R���|�[�l���g�i�e�X�g�p�j")]
    Text screensize;

    //##====================================================##
    //##               Update ���j���[�ւ̑J��              ##
    //##====================================================##
    void Update()
    {
        screensize.text = Screen.currentResolution.width.ToString() + " / " + Screen.currentResolution.height.ToString();

        if (!start_move)
        {

            // PRESS ANY BUTTON �̓_�ŏ���
            time += Time.deltaTime;
            if (time > 0.5f)
            {
                switch ((int)press_any_button.color.a)
                {
                    case 0:
                        press_any_button.color = Color.white;
                        break;

                    case 1:
                        press_any_button.color = Color.clear;
                        break;

                    default:
                        break;
                }
                time = 0f;
            }

            // �����{�^������������^�b�v������J�ڊJ�n
            if (Input.anyKeyDown)
            {
                start_move = true;
                press_any_button.color = Color.clear;

                AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_UI, AudioFilePositions.UI.TITLE_PUSH);

                // ���򉉏o
                title_logo.transform.Find("Light").DOLocalMoveX(135f, 1.5f).SetEase(Ease.OutExpo).OnComplete(() =>
                {
                    StartCoroutine(GameStart());
                });
            }
        }
    }

    //##====================================================##
    //##      �Q�[���J�n���� (�ݒ�̃��[�h�Ȃǂ�����)       ##
    //##====================================================##
    IEnumerator GameStart() 
    {
        if (!Directory.Exists(EigenValue.GetSaveDataPath()))
            Directory.CreateDirectory(EigenValue.GetSaveDataPath());
        if(File.Exists(EigenValue.GetSaveDataPath() + EigenValue.OPTIONDATA_PATH_NAME))
        OptionData.current_options = SaveLoadControll.Deserialize<OptionData>(EigenValue.GetSaveDataPath() + EigenValue.OPTIONDATA_PATH_NAME);
        if (File.Exists(EigenValue.GetSaveDataPath() + EigenValue.JOYSTICKDATA_PATH_NAME))
            InputControll.joystick_setting = SaveLoadControll.Deserialize<Joystick_Setting>(EigenValue.GetSaveDataPath() + EigenValue.JOYSTICKDATA_PATH_NAME);
        
        // ����������I�v�V���������ݒ������
        if (OptionData.current_options == default(OptionData))
        {
            string initial_controller;

            // OS�ɏ]���ď����R���g���[���[������
            #if UNITY_ANDROID || UNITY_IOS
            initial_controller = OptionData.CONTROLLER_NAME_SCREENPAD;
            #else
            initial_controller = OptionData.CONTROLLER_NAME_KEYBOARD;
            InputControll.joystick_setting = new Joystick_Setting();
            SaveLoadControll.Seriarize<Joystick_Setting>(EigenValue.GetSaveDataPath() + EigenValue.JOYSTICKDATA_PATH_NAME, InputControll.joystick_setting);
            #endif

            OptionData.current_options = new OptionData(controller: initial_controller);
            // �����������I�v�V������ۑ����Ă���
            SaveLoadControll.Seriarize<OptionData>(EigenValue.GetSaveDataPath() + EigenValue.OPTIONDATA_PATH_NAME, OptionData.current_options);
        }

        mcanim.MoveScene("Menu");
        yield break;
    }
}

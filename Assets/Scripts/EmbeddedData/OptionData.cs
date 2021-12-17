using UnityEngine;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;


//###########################################################################################################
//===========================================================================================================
//##                                     �I�v�V�����̍��ڂ��Ǘ�����f�[�^                                  ##
//===========================================================================================================
//###########################################################################################################
[Serializable]
public class OptionData
{

    // ���݂̃I�v�V�����f�[�^
    public static OptionData current_options = null;

    // ***** �R���g���[���[�Ƃ��ĉ��𗘗p���邩 *****
    //  "Keyboard" -> �L�[�{�[�h
    //  "GamePad" -> �Q�[���p�b�h(�W���C�X�e�B�b�N)
    //  "ScreenPad" -> �X�N���[���L�[�i�X�}�z�Ńv���C����ꍇ�ɉ�ʂɑ���L�[��\��������j
    public string controller;

    public const string CONTROLLER_NAME_KEYBOARD = "KeyBoard";
    public const string CONTROLLER_NAME_GAMEPAD = "GamePad";
    public const string CONTROLLER_NAME_SCREENPAD = "ScreenPad";


    // ***** �G�t�F�N�g���y�ʉ����邩�ǂ��� *****
    // �� Particle�G�t�F�N�g�̈ꕔ�̗��p���֎~���邱�ƂŌy�ʉ���}��
    public bool omitted_effect;


    // ***** �|���ꂽ�L�����̎��S���[�V�����̓��ߏ������ǂ����邩 *****
    // 0 -> �ʏ�
    // 1 -> ������
    public int dead_chara_color;

    // ***** �U���𔭐������鏈�� *****
    public bool is_play_shake;

    public OptionData(string controller = "Keyboard", bool omitted_effect = false,int dead_chara_color = 0,bool is_play_shake = true) 
    {
        this.controller = controller;
        this.omitted_effect = omitted_effect;
        this.dead_chara_color = dead_chara_color;
        this.is_play_shake = is_play_shake;
    }

}

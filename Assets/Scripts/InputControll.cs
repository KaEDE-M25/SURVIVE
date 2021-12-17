using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

//--====================================================--
//--       �W���C�X�e�B�b�N�̐ݒ���܂Ƃ߂��N���X       --
//--====================================================--
[Serializable]
public class Joystick_Setting
{
    public int BUTTON_A;
    public int BUTTON_B;
    public int BUTTON_X;
    public int BUTTON_Y;
    public int BUTTON_L;
    public int BUTTON_R;
    public int BUTTON_START;

    public Joystick_Setting(
        int BUTTON_A = 3, 
        int BUTTON_B = 4, 
        int BUTTON_X = 1, 
        int BUTTON_Y = 2, 
        int BUTTON_L = 5, 
        int BUTTON_R = 6, 
        int BUTTON_START = 12) 
    {
        this.BUTTON_A = BUTTON_A;
        this.BUTTON_B = BUTTON_B;
        this.BUTTON_X = BUTTON_X;
        this.BUTTON_Y = BUTTON_Y;
        this.BUTTON_L = BUTTON_L;
        this.BUTTON_R = BUTTON_R;
        this.BUTTON_START = BUTTON_START;
    }
}

//--====================================================--
//--         ���z�p�b�h�̏�Ԃ��܂Ƃ߂��\����           --
//--====================================================--
public struct Virtualpad_status
{
    // ���z�X�e�B�b�N�̓��͒l�A-1�`1�̒l���Ƃ�
    public Vector2 stick_pos;

    // ������Ă����Ԃ��ǂ���
    public bool a_pressed;
    public bool b_pressed;
    public bool x_pressed;
    public bool y_pressed;
    public bool l_pressed;
    public bool r_pressed;
    public bool start_pressed;
    public bool uparrow_pressed;
    public bool downarrow_pressed;
    public bool leftarrow_pressed;
    public bool rightarrow_pressed;

    // �����ꂽ�u�Ԃ��ǂ���
    public bool a_downed;
    public bool b_downed;
    public bool x_downed;
    public bool y_downed;
    public bool l_downed;
    public bool r_downed;
    public bool start_downed;
    public bool uparrow_downed;
    public bool downarrow_downed;
    public bool leftarrow_downed;
    public bool rightarrow_downed;
}

//--====================================================--
//--        �Q�[���ւ̑�����͂��Ǘ�����N���X          --
//--====================================================--
public class InputControll : MonoBehaviour
{
    // �{�^��ID�@�茳��XBOX�R���g���[���̃{�^���z�u���Q�l�ɂ��Ă���

    // �W�����v�L�[(�L�����Z���{�^��)�@�b�@B�L�[
    public const int INPUT_ID_B = 4;
    // �A�C�e���g�p�L�[�@�b�@Y�L�[
    public const int INPUT_ID_Y = 2;
    // �U���L�[(����{�^��)�@�b�@A�L�[
    public const int INPUT_ID_A = 3;
    // MP�U���L�[�@�b�@X�L�[
    public const int INPUT_ID_X = 1;
    // �|�[�Y�L�[�@�b�@�X�^�[�g�{�^��
    public const int INPUT_ID_PAUSE = 5;
    public const int INPUT_ID_START = 5;
    // �g���KL�L�[�@�b�@L�g���K
    public const int INPUT_ID_TRIGGER_L = 6;
    // �g���KR�L�[�@�b�@R�g���K
    public const int INPUT_ID_TRIGGER_R = 7;

    // ���L�[
    public const int INPUT_ID_UPARROW = -1;
    // ���L�[
    public const int INPUT_ID_DOWNARROW = -2;
    // ���L�[
    public const int INPUT_ID_LEFTARROW = -3;
    // �E�L�[
    public const int INPUT_ID_RIGHTARROW = -4;


    // ���z�p�b�h�̏�Ԃ�\������ϐ��i�C�x���g�g���K����Ăяo���ĕύX���A���Ɠ��l��GetInput�n����ǂݍ���ŗ��p�j
    public static Virtualpad_status virtualpad_status;

    [SerializeField,Tooltip("���z�p�b�h�̐eobj")]
    GameObject virtual_pad_parent;
    
    // �e�퉼�z�{�^���E���z�p�b�h
    public GameObject stick_obj;

    // ���{�^���̐eobj
    public GameObject arrow_button_obj_parent;

    // �W���C�X�e�B�b�N�̃X�e�B�b�N���|����Ă��邩�ǂ���
    static bool is_pressed_up_in_joystick = false;
    static bool is_pressed_down_in_joystick = false;
    static bool is_pressed_left_in_joystick = false;
    static bool is_pressed_right_in_joystick = false;

    // �W���C�X�e�B�b�N�̐ݒ�
    public static Joystick_Setting joystick_setting;

    // ���z�X�e�B�b�N��p���邩�i�p����Ȃ�true�A�X�E�B���h�E�ȂǂŖ����g�������Ƃ���false�ɂ���j
    public static bool use_virtual_stick = false;

    //##====================================================##
    //##                     Awake ������                   ##
    //##====================================================##
    private void Awake()
    {
        // �G�f�B�^�[�̏ꍇ�̓L�[�{�[�h�ŏ�����
        if (Application.isEditor)
        {
            if (OptionData.current_options == null)
            {
                OptionData.current_options = new OptionData(controller: OptionData.CONTROLLER_NAME_KEYBOARD);
            }
        }

        // ���z�p�b�h���g���ꍇ�̓A�N�e�B�u�ɂ���
        if (OptionData.current_options.controller.Equals(OptionData.CONTROLLER_NAME_SCREENPAD))
        {
            virtual_pad_parent.SetActive(true);
        }
    }

    //##====================================================##
    //##     LateUpdate ���z�p�b�h�̓��͂����Z�b�g����      ##
    //##====================================================##
    private void LateUpdate()
    {
        if (virtualpad_status.a_pressed) virtualpad_status.a_downed = false;
        if (virtualpad_status.b_pressed) virtualpad_status.b_downed = false;
        if (virtualpad_status.x_pressed) virtualpad_status.x_downed = false;
        if (virtualpad_status.y_pressed) virtualpad_status.y_downed = false;
        if (virtualpad_status.l_pressed) virtualpad_status.l_downed = false;
        if (virtualpad_status.r_pressed) virtualpad_status.r_downed = false;
        if (virtualpad_status.start_pressed) virtualpad_status.start_downed = false;
        if (virtualpad_status.uparrow_pressed) virtualpad_status.uparrow_downed = false;
        if (virtualpad_status.downarrow_pressed) virtualpad_status.downarrow_downed = false;
        if (virtualpad_status.leftarrow_pressed) virtualpad_status.leftarrow_downed = false;
        if (virtualpad_status.rightarrow_pressed) virtualpad_status.rightarrow_downed = false;

        // ���{�^���Ɖ��z�X�e�B�b�N�̂ǂ��炩��L����
        stick_obj.transform.parent.gameObject.SetActive(use_virtual_stick);
        arrow_button_obj_parent.SetActive(!use_virtual_stick);
    }

    //##====================================================##
    //##       �ړ��Ɋւ�����͂��󂯎��Afloat�ŕԂ�      ##
    //##====================================================##
    public static float GetInput_Move()
    {
        switch (OptionData.current_options.controller)
        {
            case OptionData.CONTROLLER_NAME_SCREENPAD: // ���z�p�b�h���g���ꍇ�͉��z�p�b�h�̓��͒l�����̂܂ܔ��f
                {
                    return Mathf.Abs(virtualpad_status.stick_pos.x) > 0.1f ? virtualpad_status.stick_pos.x : 0f;
                }
            case OptionData.CONTROLLER_NAME_KEYBOARD: // �L�[�{�[�h�̖�󂩂���͂��Ƃ�ꍇ�͒��Ԓl�͂Ȃ�
                {
                    if (Input.GetKey(KeyCode.LeftArrow))
                        return -1f;
                    else if (Input.GetKey(KeyCode.RightArrow))
                        return 1f;
                    else
                        return 0f;
                }

            default: // ���̑��̏ꍇ��GetAxis�Ŏ���̂ł���������
                {
                    float horizontal_input_value = Input.GetAxis("Horizontal");
                    // ��Βl0.1f�ȉ��̓��͂͐؂�̂�
                    return Mathf.Abs(horizontal_input_value) > 0.1f ? horizontal_input_value : 0f;
                }
        }
    }

    //##====================================================##
    //## �{�^���n�̓���(������Ă邩)���󂯎��Abool�ŕԂ� ##
    //##====================================================##
    public static bool GetInputDown(int input_id)
    {
        switch (OptionData.current_options.controller)
        {
            case OptionData.CONTROLLER_NAME_KEYBOARD: // �L�[�{�[�h��������Ă��邩�𔻒�
                {
                    var actual_keycode = Get_KeyID_from_input(input_id);
                    return actual_keycode != null && Input.GetKeyDown((KeyCode)actual_keycode);
                }

            case OptionData.CONTROLLER_NAME_GAMEPAD:
                {
                    // �{�^����������Ă��邩�擾
                    if (input_id > 0)
                    {
                        var actual_id = Get_buttonID_from_input(input_id) - 1;

                        return actual_id >= 0 && Input.GetKeyDown("joystick button " + actual_id);
                    }
                    else // �X�e�B�b�N���|����Ă��邩���擾
                    {
                        return GetStickDown(input_id);
                    }
                }
            case OptionData.CONTROLLER_NAME_SCREENPAD:
                {
                    switch (input_id)
                    {
                        case INPUT_ID_A:
                            if (virtualpad_status.a_downed) { virtualpad_status.a_downed = false; return true; }
                            return false;
                        case INPUT_ID_B:
                            if (virtualpad_status.b_downed) { virtualpad_status.b_downed = false; return true; }
                            return false;
                        case INPUT_ID_X:
                            if (virtualpad_status.x_downed) { virtualpad_status.x_downed = false; return true; }
                            return false;
                        case INPUT_ID_Y:
                            if (virtualpad_status.y_downed) { virtualpad_status.y_downed = false; return true; }
                            return false;
                        case INPUT_ID_TRIGGER_L:
                            if (virtualpad_status.l_downed) { virtualpad_status.l_downed = false; return true; }
                            return false;
                        case INPUT_ID_TRIGGER_R:
                            if (virtualpad_status.r_downed) { virtualpad_status.r_downed = false; return true; }
                            return false;
                        case INPUT_ID_START:
                            if (virtualpad_status.start_downed) { virtualpad_status.start_downed = false; return true; }
                            return false;
                        case INPUT_ID_UPARROW:
                            if (virtualpad_status.uparrow_downed || virtualpad_status.stick_pos.y > 0.5f) { virtualpad_status.uparrow_downed = false; return true; }
                            return false;
                        case INPUT_ID_DOWNARROW:
                            if (virtualpad_status.downarrow_downed || virtualpad_status.stick_pos.y < -0.5f) { virtualpad_status.downarrow_downed = false; return true; }
                            return false;
                        case INPUT_ID_LEFTARROW:
                            if (virtualpad_status.leftarrow_downed || virtualpad_status.stick_pos.x > 0.5f) { virtualpad_status.leftarrow_downed = false; return true; }
                            return false;
                        case INPUT_ID_RIGHTARROW:
                            if (virtualpad_status.rightarrow_downed || virtualpad_status.stick_pos.x < -0.5f) { virtualpad_status.rightarrow_downed = false; return true; }
                            return false;

                        default:
                            return false;
                    }
                }

            default:
                return false;
        }
    }

    //##====================================================##
    //##  �X�e�B�b�N���|����Ă��邩���󂯎��Abool�ŕԂ�  ##
    //##     JoyStick�Ŗ��{�^���I�ȓ��͂��s�����Ɏg�p     ##
    //##====================================================##
    private static bool GetStickDown(int input_id) 
    {
        var return_value = false;
        ref bool is_pressed_in_joystick = ref is_pressed_up_in_joystick;
        switch (input_id)
        {
            case INPUT_ID_UPARROW:
                {
                    return_value = Input.GetAxisRaw("Vertical") < -0.5f;
                    is_pressed_in_joystick = ref is_pressed_up_in_joystick;
                    break;
                }
            case INPUT_ID_DOWNARROW:
                {
                    return_value = Input.GetAxisRaw("Vertical") > 0.5f;
                    is_pressed_in_joystick = ref is_pressed_down_in_joystick;
                    break;
                }
            case INPUT_ID_LEFTARROW:
                {
                    return_value = Input.GetAxis("Horizontal") < -0.5f;
                    is_pressed_in_joystick = ref is_pressed_left_in_joystick;
                    break;
                }
            case INPUT_ID_RIGHTARROW:
                {
                    return_value = Input.GetAxis("Horizontal") > 0.5f;
                    is_pressed_in_joystick = ref is_pressed_right_in_joystick;
                    break;
                }
        }

        if (is_pressed_in_joystick)
        {
            if (!return_value)
                is_pressed_in_joystick = false;
            return false;

        }
        else
        {
            if (return_value)
            {
                is_pressed_in_joystick = true;
            }
            return return_value;
        }

    }

    //##====================================================##
    //## �{�^���n�̓���(������Ă邩)���󂯎��Abool�ŕԂ� ##
    //##====================================================##
    public static bool GetInputUp(int input_id)
    {
        switch (OptionData.current_options.controller)
        {
            case OptionData.CONTROLLER_NAME_KEYBOARD:
                {
                    var actual_keycode = Get_KeyID_from_input(input_id);
                    return actual_keycode != null && Input.GetKeyUp((KeyCode)actual_keycode);
                }

            case OptionData.CONTROLLER_NAME_GAMEPAD: 
                {
                    var actual_id = Get_buttonID_from_input(input_id) - 1;
                    return actual_id >= 0 && Input.GetKeyUp("joystick button " + actual_id);
                }
            case OptionData.CONTROLLER_NAME_SCREENPAD:
                {
                    return input_id switch
                    {
                        INPUT_ID_A => !virtualpad_status.a_pressed,
                        INPUT_ID_B => !virtualpad_status.b_pressed,
                        INPUT_ID_X => !virtualpad_status.x_pressed,
                        INPUT_ID_Y => !virtualpad_status.y_pressed,
                        INPUT_ID_TRIGGER_L => !virtualpad_status.l_pressed,
                        INPUT_ID_TRIGGER_R => !virtualpad_status.r_pressed,
                        INPUT_ID_START => !virtualpad_status.start_pressed,
                        INPUT_ID_UPARROW => !virtualpad_status.uparrow_pressed,
                        INPUT_ID_DOWNARROW => !virtualpad_status.downarrow_pressed,
                        INPUT_ID_LEFTARROW => !virtualpad_status.leftarrow_pressed,
                        INPUT_ID_RIGHTARROW => !virtualpad_status.rightarrow_pressed,
                        _ => false,
                    };
                }

            default:
                return false;
        }
        
    }

    //##====================================================##
    //##          ���͂�JoyStick�̃{�^���ɕR�Â���          ##
    //##====================================================##
    private static int Get_buttonID_from_input(int input_id) 
    {
        return input_id switch
        {
            INPUT_ID_B => joystick_setting.BUTTON_B,
            INPUT_ID_Y => joystick_setting.BUTTON_Y,
            INPUT_ID_A => joystick_setting.BUTTON_A,
            INPUT_ID_X => joystick_setting.BUTTON_X,
            INPUT_ID_PAUSE => joystick_setting.BUTTON_START,
            INPUT_ID_TRIGGER_L => joystick_setting.BUTTON_L,
            INPUT_ID_TRIGGER_R => joystick_setting.BUTTON_R,
            _ => 0,
        };
    }

    //##====================================================##
    //##               ���͂�KeyCode�ɕR�Â���              ##
    //##====================================================##
    private static KeyCode? Get_KeyID_from_input(int input_id)
    {
        return input_id switch
        {
            INPUT_ID_B => KeyCode.Space,
            INPUT_ID_Y => KeyCode.C,
            INPUT_ID_A => KeyCode.Z,
            INPUT_ID_X => KeyCode.X,
            INPUT_ID_PAUSE => KeyCode.P,
            INPUT_ID_TRIGGER_L => KeyCode.E,
            INPUT_ID_TRIGGER_R => KeyCode.R,
            INPUT_ID_UPARROW => KeyCode.UpArrow,
            INPUT_ID_DOWNARROW => KeyCode.DownArrow,
            INPUT_ID_LEFTARROW => KeyCode.LeftArrow,
            INPUT_ID_RIGHTARROW => KeyCode.RightArrow,
            _ => null,
        };
    }

    //##====================================================##
    //##     ���z�X�e�B�b�N��������Ă���ۂ̏��X�V       ##
    //##====================================================##
    public void RenewInformation_VirtualStick(BaseEventData event_data) 
    {
        // �X�e�B�b�N�̈ʒu���^�b�v�ʒu�ɂ���
        RectTransformUtility.ScreenPointToWorldPointInRectangle(stick_obj.GetComponent<RectTransform>(), ((PointerEventData)event_data).position, Camera.main,out Vector3 local_point);
        stick_obj.transform.position = local_point;

        // �͈͊O�ɏo�Ȃ��悤�ɐ���
        if (Vector2.Distance(stick_obj.transform.position, stick_obj.transform.parent.position) > 30f) 
            stick_obj.transform.localPosition = (stick_obj.transform.position - stick_obj.transform.parent.position).normalized * 30f;

        virtualpad_status.stick_pos = stick_obj.transform.localPosition.normalized;
    }

    //##====================================================##
    //##         ���z�X�e�B�b�N�̈ʒu�����Z�b�g����         ##
    //##====================================================##
    public void Reset_VirtualStick() 
    {
        stick_obj.transform.position = stick_obj.transform.parent.position;
        virtualpad_status.stick_pos = Vector2.zero;
    }

    //##====================================================##
    //##       ���z�p�b�h�̃{�^����������Ă��邩����       ##
    //##====================================================##
    public void ButtonDown_VirtualStick(string button_name) 
    {
        switch (button_name)
        {
            case "A":
                virtualpad_status.a_pressed = true; virtualpad_status.a_downed = true; return;
            case "B":
                virtualpad_status.b_pressed = true; virtualpad_status.b_downed = true; return;
            case "X":
                virtualpad_status.x_pressed = true; virtualpad_status.x_downed = true; return;
            case "Y":
                virtualpad_status.y_pressed = true; virtualpad_status.y_downed = true; return;
            case "L":
                virtualpad_status.l_pressed = true; virtualpad_status.l_downed = true; return;
            case "R":
                virtualpad_status.r_pressed = true; virtualpad_status.r_downed = true; return;
            case "START":
                virtualpad_status.start_pressed = true; virtualpad_status.start_downed = true; return;
            case "UP":
                virtualpad_status.uparrow_pressed = true; virtualpad_status.uparrow_downed = true; return;
            case "DOWN":
                virtualpad_status.downarrow_pressed = true; virtualpad_status.downarrow_downed = true; return;
            case "LEFT":
                virtualpad_status.leftarrow_pressed = true; virtualpad_status.leftarrow_downed = true; return;
            case "RIGHT":
                virtualpad_status.rightarrow_pressed = true; virtualpad_status.rightarrow_downed = true; return;

            default:
                return;
        }
    }

    //##====================================================##
    //##       ���z�p�b�h�̃{�^����������Ă��邩����       ##
    //##====================================================##
    public void ButtonUp_VirtualStick(string button_name) 
    {
        switch (button_name)
        {
            case "A":
                virtualpad_status.a_pressed = false; return;
            case "B":
                virtualpad_status.b_pressed = false; return;
            case "X":
                virtualpad_status.x_pressed = false; return;
            case "Y":
                virtualpad_status.y_pressed = false; return;
            case "L":
                virtualpad_status.l_pressed = false; return;
            case "R":
                virtualpad_status.r_pressed = false; return;
            case "START":
                virtualpad_status.start_pressed = false; return;
            case "UP":
                virtualpad_status.uparrow_pressed = false; return;
            case "DOWN":
                virtualpad_status.downarrow_pressed = false; return;
            case "LEFT":
                virtualpad_status.leftarrow_pressed = false; return;
            case "RIGHT":
                virtualpad_status.rightarrow_pressed = false; return;

            default:
                return;
        }
    }

    //##====================================================##
    //## ���z�p�b�h�̃{�^���̌����ڂ�������Ă����Ԃɂ��� ##
    //##====================================================##
    public void Renew_Button_Sprite_push(VirtualButton virtualButton) 
    {
        virtualButton.image.sprite = virtualButton.pushed_button_sprite;
    }

    //##====================================================##
    //## ���z�p�b�h�̃{�^���̌����ڂ𗣂���Ă����Ԃɂ��� ##
    //##====================================================##
    public void Renew_Button_Sprite_idle(VirtualButton virtualButton)
    {
        virtualButton.image.sprite = virtualButton.base_button_sprite;
    }
}

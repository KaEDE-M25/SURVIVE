using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//--====================================================--
//--    ScreenPad�ɂ��鉼�z�{�^���������ۂ̃f�[�^�W��   --
//-- �i�{�^����obj�Ɏ��t���ăC�x���g�œn���Ēl�𒲐��j--
//--====================================================--
public class VirtualButton : MonoBehaviour
{
    // �I�u�W�F�N�g�ɂ��Ă���Image�R���|�[�l���g
    public Image image;
    
    // ������ĂȂ���Ԃ̃{�^����Sprite
    public Sprite base_button_sprite;
    // ������Ă����Ԃ̃{�^����Sprite
    public Sprite pushed_button_sprite;
}

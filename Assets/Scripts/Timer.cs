using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//--====================================================--
//--                   �v���p�^�C�}�[                   --
//--====================================================--
public class Timer : MonoBehaviour
{
    [SerializeField, Tooltip("�J�n������̎���")]
    float total_time;
    // �O��������o���p�̎��Ԓl
    public int Current_time { get; private set; }

    public int Max_count { get; private set; }
    // �J�E���g�_�E�����J�E���g�A�b�v��
    int down;
    // �J�E���g����
    public bool Is_count { get; private set; }

    //##====================================================##
    //##                 Update �J�E���g����                ##
    //##====================================================##
    void Update()
    {
        if (Is_count)
        {
            total_time += Time.deltaTime * this.down;
            
            switch (down) 
            {
                // �J�E���g�_�E���̏ꍇ
                case -1:
                    {
                        Current_time = Mathf.CeilToInt(total_time);
                        if (total_time <= 0)
                        {
                            Current_time = 0;
                            Is_count = false;
                        }
                        break;
                    }
                // �J�E���g�A�b�v�̏ꍇ
                case 1:
                    {
                        Current_time = Mathf.FloorToInt(total_time);
                        if (total_time >= Max_count)
                        {
                            Current_time = 0;
                            Is_count = false;
                        }
                        break; 
                    }
            }
        }
    }

    //##====================================================##
    //##       ���Ԃ��w�肵�ă^�C�}�[���X�^�[�g������       ##
    //##====================================================##
    public void TimerStart(int time,bool down = true) 
    {
        total_time = (down) ? time: 0;
        this.down = (down) ? -1 : 1;
        Is_count = true;
        Max_count = time;
    }
}

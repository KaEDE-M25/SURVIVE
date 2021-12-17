using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//--====================================================--
//--                 �G�L�����@�X���C��                 --
//--====================================================--
public class Slime : Enemy
{
    // �W�����v�̃C���^�[�o�����v�Z���邽�߂̃J�E���^�i�b�j
    float jump_time = 0f;
    // �W�����v�Ԋu�i�b�j
    float frag_time = 3f;

    // ���ۂ̑��x�ƃW�����v�́i��b�l�ɗ����l�𑫂����l�j
    float actual_speed = 0f;
    float actual_jump_power = 0f;
    // ���� (Move�ɓn���Ƃ��Ɏg�p)
    float direction = 0f;
   

    protected override CharacterStatus CharacterStatus()
    {
        cant_knockback = true;
        return EigenValue.STATUS_003_SLIME.status;
    }


    //$$====================================================$$
    //$$            Start�ōs���X�e�[�^�X������             $$
    //$$====================================================$$
    protected override void Initialize() { }

    //##====================================================##
    //##        �A�C�e���h���b�v����                        ##
    //##====================================================##
    protected override void ItemDrop()
    {
    }
    //##====================================================##
    //##                   �ŗL�̍s������                   ##
    //##====================================================##
    protected override void OriginalAction()
    {
        // �X���C�����L�̍s������
        if (!is_jump) 
        {
            jump_time += Time.deltaTime;
            rb2d.velocity = rb2d.velocity * Vector2.up;

            if (jump_time >= frag_time)
            {
                actual_jump_power = my_status.Jump_power + Random.Range(-0.5f, 0.5f);
                actual_speed = my_status.Speed + Random.Range(-50f, 50f);
                direction = player.transform.position.x > transform.position.x ? 1f : -1f;

                Jump(actual_jump_power);
                jump_time = 0f;
                frag_time = 3f + Random.Range(-2f, 2f);
            }
        }

        if (!is_damage && player != null && is_jump)
        {
            Move(actual_speed, direction);
        }

    }

}

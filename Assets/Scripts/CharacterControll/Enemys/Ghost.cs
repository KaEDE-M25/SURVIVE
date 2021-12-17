using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//##====================================================##
//##                 �G�L�����@�S�[�X�g                 ##
//##====================================================##
public class Ghost : Enemy
{
    [SerializeField,Tooltip("�q�I�u�W�F�N�g�ɂ��Ă��鎀�S�p�[�e�B�N��")]
    GameObject ghost_dead_particle;


    protected override CharacterStatus CharacterStatus()
    {
        // ��s�L����
        is_fly_chara = true;

        return EigenValue.STATUS_002_GHOST.status;
    }


    //##====================================================##
    //##                   �ŗL�̍s������                   ##
    //##====================================================##
    protected override void OriginalAction()
    {
        // �S�[�X�g���L�̍s������
        if (!is_damage && player != null)
        {
            if (Distance(transform.position.x, player.transform.position.x) > 3f)
                Move(my_status.Speed, player);
        }
    }

    //##====================================================##
    //##            Start�ōs���X�e�[�^�X������             ##
    //##====================================================##
    protected override void Initialize() { }


    //##====================================================##
    //##        �A�C�e���h���b�v����                        ##
    //##====================================================##
    protected override void ItemDrop()
    {
        // �m���ŉ񕜃|�[�V�������h���b�v
        if (Random.Range(0, 100) == 0)
        {
            Drop("MP_Potion", Random.Range(1.125f, 2f), Random.Range(200f, 300f));
        }


    }

    //##====================================================##
    //##             ���S����(Animation����ďo)            ##
    //##====================================================##
    public override void Dead()
    {
        ghost_dead_particle.SetActive(true);
        ghost_dead_particle.transform.parent = GameObject.FindWithTag("GameController").GetComponent<GameControll>().Active_Effects_Parent().transform;

        base.Dead();
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//--====================================================--
//--                 �G�L�����@�]���r                   --
//--====================================================--
public class Zombie: Enemy
{
    protected override CharacterStatus CharacterStatus()
    {
        // �X�e�[�^�X�������l
        return EigenValue.STATUS_000_ZOMBIE.status;

    }

    //$$====================================================$$
    //$$            Start�ōs���X�e�[�^�X������             $$
    //$$====================================================$$
    protected override void Initialize(){}

    //##====================================================##
    //##        �A�C�e���h���b�v����                        ##
    //##====================================================##
    protected override void ItemDrop()
    {
        // �h���b�v���邨��������
        int drop_money = Random.Range(1, 20);
        // �h���b�v���邨��(�e��̐�)������
        int drop_gold = drop_money / 10;
        int drop_silver = drop_money % 10 / 5;
        int drop_bronze = drop_money % 10 % 5;

        // �����̃h���b�v
        for (int i = 0; i < drop_bronze; i++)
            Drop("Money_bronze", Random.Range(1.125f, 2f), Random.Range(200f, 300f));
        for (int i = 0; i < drop_silver; i++)
            Drop("Money_silver", Random.Range(1.125f, 2f), Random.Range(200f, 300f));
        for (int i = 0; i < drop_gold; i++)
            Drop("Money_gold", Random.Range(1.125f, 2f), Random.Range(200f, 300f));

        // �m���ŉ񕜃|�[�V�������h���b�v
        if (Random.Range(0, 100) == 0)
        {
            Drop("HP_Potion", Random.Range(1.125f, 2f), Random.Range(200f, 300f));
        }


    }

    //##====================================================##
    //##                   �ŗL�̍s������                   ##
    //##====================================================##
    protected override void OriginalAction()
    {
        
        // �_���[�W���󂯂Ă��Ȃ��A�v���C���[������Ȃ�
        if (!is_damage && player != null)
        {
            // �v���C���[�̕����ֈړ�
            Move(my_status.Speed, player.transform.position.x > transform.position.x ? 1f : -1f);
        }
    }

    
}

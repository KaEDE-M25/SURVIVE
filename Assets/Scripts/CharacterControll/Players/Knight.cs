using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//--====================================================--
//--                  �v���C���[�F����                --
//--====================================================--
public class Knight : Fighters
{
    // �G�t�F�N�g�Q(��prefab�̖��O)
    string attack_effect_1_1;
    string attack_effect_1_2;
    string attack_effect_2_1;
    string attack_effect_2_2;
    string attack_effect_3;

    // �G�t�F�N�g�Q(�̎q�I�u�W�F�N�g��ParticleSystem�R���|�[�l���g) 
    ParticleSystem attack_effect_MP_1;

    protected override void Initialize_Fighter()
    {
        // �ŗL�̍U���G�t�F�N�g��prefab�̖��O��ݒ�

        attack_effect_1_1 = "EF_Knight_attack_1-1";
        attack_effect_1_2 = "EF_Knight_attack_1-2";
        attack_effect_2_1 = "EF_Knight_attack_2-1";
        attack_effect_2_2 = "EF_Knight_attack_2-2";
        attack_effect_3 = "EF_Knight_attack_3";

        attack_effect_MP_1 = transform.Find("EF_Knight_MPattack_1").GetComponent<ParticleSystem>();
    }

    protected override CharacterStatus CharacterStatus()
    {
        return EigenValue.STATUS_PLAYER_001_KNIGHT;
    }

    //--====================================================--
    //--                �U�������鑀�쏈��                  --
    //--====================================================--
    protected override void Attack_Controll()
    {
        if (is_jump == false)
        {
            if (InputControll.GetInputDown(InputControll.INPUT_ID_A))
            {

                // �ʏ�U��

                if (Mathf.Abs(rb2d.velocity.x) <= 1f)
                {
                    // �U���P�i��
                    if (attackID == 0 && is_attack == false)
                    {
                        AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_PLAYER, AudioFilePositions.EFFECT.SWORD1);
                        attackID = 1;
                        is_attack = true;
                    }
                    // �U���Q�i��
                    else if (attackID == 1 && is_attack == true)
                    {
                        AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_PLAYER, AudioFilePositions.EFFECT.SWORD1);
                        attackID = 2;
                        is_attack = true;
                    }
                    // �U���R�i��
                    else if (attackID == 2 && is_attack == true)
                    {
                        AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_PLAYER, AudioFilePositions.EFFECT.SWORD2);
                        attackID = 3;
                        is_attack = true;
                    }
                    transform.localPosition += Vector3.right * -3f * transform.localScale.x;
                }
                // �_�b�V���U��
                /*else if (!is_attack)
                {
                    attackID = 4;
                    is_attack = true;
                    x_val = 0;
                    anim.Play("Attack_dash");
                    Jump(1f);
                    Move(Speed * 1f, -transform.localScale.x);
                }*/

            }
        }
        else
        {
            // X��MP�U��
            if (InputControll.GetInputDown(InputControll.INPUT_ID_X) && is_attack == false && MP > 0) 
            {
                ReductMP(1);
                this.gameObject.layer = 1;
                attack_effect_MP_1.gameObject.SetActive(true);
                attack_effect_MP_1.Play();
                attackID = 4;
                is_attack = true;
                rb2d.constraints = RigidbodyConstraints2D.FreezeAll;
            }
        }
    }

    //--====================================================--
    //--              �U���G�t�F�N�g���o������              --
    //--====================================================--
    protected override void Attack_Effect_Play(int attackID)
    {
        if (attackID > 0)
        {
            bool mirror = transform.localScale.x > 0;

            switch (attackID)
            {
                // ID=1 �R�i�U���̂P��
                case 1:
                    // �a���̎c��
                    Play_Effect(attack_effect_1_1, new Vector2(-4 * transform.localScale.x, 3), mirror);
                    // �Ռ��g
                    Play_Effect(attack_effect_1_2, new Vector2(-32 * transform.localScale.x, 10f), mirror);

                    break;

                // ID=2 �R�i�U���̂Q��
                case 2:

                    // �a���̎c��
                    Play_Effect(attack_effect_2_1, new Vector2(-4 * transform.localScale.x, 3), mirror);
                    // �Ռ��g
                    Play_Effect(attack_effect_2_2, new Vector2(-32 * transform.localScale.x, 0f), mirror);

                    break;
                // ID=3 �R�i�U���̂R��
                case 3:

                    // �Ռ��g
                    Play_Effect(attack_effect_3, new Vector2(-40 * transform.localScale.x, -2), mirror);

                    break;

                default:
                    break;

            }

        }
    }

    //--====================================================--
    //--              �d������(MP�U���p)Animation����ďo   --
    //--====================================================--
    void End_of_Freeze()
    {
        this.gameObject.layer = my_layer;
        rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;
    }


    //--====================================================--
    //--            �U�����Đ�(MP�U���p)Animation����ďo   --
    //--====================================================--
    void Play_MPAttackSound()
    {
        AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_PLAYER, AudioFilePositions.EFFECT.THUNDER);

    }


}

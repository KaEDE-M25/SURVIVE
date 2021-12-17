using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//--====================================================--
//--                  プレイヤー：けんし                --
//--====================================================--
public class Knight : Fighters
{
    // エフェクト群(のprefabの名前)
    string attack_effect_1_1;
    string attack_effect_1_2;
    string attack_effect_2_1;
    string attack_effect_2_2;
    string attack_effect_3;

    // エフェクト群(の子オブジェクトのParticleSystemコンポーネント) 
    ParticleSystem attack_effect_MP_1;

    protected override void Initialize_Fighter()
    {
        // 固有の攻撃エフェクトのprefabの名前を設定

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
    //--                攻撃をする操作処理                  --
    //--====================================================--
    protected override void Attack_Controll()
    {
        if (is_jump == false)
        {
            if (InputControll.GetInputDown(InputControll.INPUT_ID_A))
            {

                // 通常攻撃

                if (Mathf.Abs(rb2d.velocity.x) <= 1f)
                {
                    // 攻撃１段目
                    if (attackID == 0 && is_attack == false)
                    {
                        AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_PLAYER, AudioFilePositions.EFFECT.SWORD1);
                        attackID = 1;
                        is_attack = true;
                    }
                    // 攻撃２段目
                    else if (attackID == 1 && is_attack == true)
                    {
                        AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_PLAYER, AudioFilePositions.EFFECT.SWORD1);
                        attackID = 2;
                        is_attack = true;
                    }
                    // 攻撃３段目
                    else if (attackID == 2 && is_attack == true)
                    {
                        AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_PLAYER, AudioFilePositions.EFFECT.SWORD2);
                        attackID = 3;
                        is_attack = true;
                    }
                    transform.localPosition += Vector3.right * -3f * transform.localScale.x;
                }
                // ダッシュ攻撃
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
            // XでMP攻撃
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
    //--              攻撃エフェクトを出す処理              --
    //--====================================================--
    protected override void Attack_Effect_Play(int attackID)
    {
        if (attackID > 0)
        {
            bool mirror = transform.localScale.x > 0;

            switch (attackID)
            {
                // ID=1 ３段攻撃の１つ目
                case 1:
                    // 斬撃の残像
                    Play_Effect(attack_effect_1_1, new Vector2(-4 * transform.localScale.x, 3), mirror);
                    // 衝撃波
                    Play_Effect(attack_effect_1_2, new Vector2(-32 * transform.localScale.x, 10f), mirror);

                    break;

                // ID=2 ３段攻撃の２つ目
                case 2:

                    // 斬撃の残像
                    Play_Effect(attack_effect_2_1, new Vector2(-4 * transform.localScale.x, 3), mirror);
                    // 衝撃波
                    Play_Effect(attack_effect_2_2, new Vector2(-32 * transform.localScale.x, 0f), mirror);

                    break;
                // ID=3 ３段攻撃の３つ目
                case 3:

                    // 衝撃波
                    Play_Effect(attack_effect_3, new Vector2(-40 * transform.localScale.x, -2), mirror);

                    break;

                default:
                    break;

            }

        }
    }

    //--====================================================--
    //--              硬直解除(MP攻撃用)Animationから呼出   --
    //--====================================================--
    void End_of_Freeze()
    {
        this.gameObject.layer = my_layer;
        rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;
    }


    //--====================================================--
    //--            攻撃音再生(MP攻撃用)Animationから呼出   --
    //--====================================================--
    void Play_MPAttackSound()
    {
        AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_PLAYER, AudioFilePositions.EFFECT.THUNDER);

    }


}

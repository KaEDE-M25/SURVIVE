using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//--====================================================--
//--                 敵キャラ　スライム                 --
//--====================================================--
public class Slime : Enemy
{
    // ジャンプのインターバルを計算するためのカウンタ（秒）
    float jump_time = 0f;
    // ジャンプ間隔（秒）
    float frag_time = 3f;

    // 実際の速度とジャンプ力（基礎値に乱数値を足した値）
    float actual_speed = 0f;
    float actual_jump_power = 0f;
    // 方向 (Moveに渡すときに使用)
    float direction = 0f;
   

    protected override CharacterStatus CharacterStatus()
    {
        cant_knockback = true;
        return EigenValue.STATUS_003_SLIME.status;
    }


    //$$====================================================$$
    //$$            Startで行うステータス初期化             $$
    //$$====================================================$$
    protected override void Initialize() { }

    //##====================================================##
    //##        アイテムドロップ処理                        ##
    //##====================================================##
    protected override void ItemDrop()
    {
    }
    //##====================================================##
    //##                   固有の行動処理                   ##
    //##====================================================##
    protected override void OriginalAction()
    {
        // スライム特有の行動処理
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

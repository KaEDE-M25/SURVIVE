using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//--====================================================--
//--                 敵キャラ　ゾンビ                   --
//--====================================================--
public class Zombie: Enemy
{
    protected override CharacterStatus CharacterStatus()
    {
        // ステータス初期化値
        return EigenValue.STATUS_000_ZOMBIE.status;

    }

    //$$====================================================$$
    //$$            Startで行うステータス初期化             $$
    //$$====================================================$$
    protected override void Initialize(){}

    //##====================================================##
    //##        アイテムドロップ処理                        ##
    //##====================================================##
    protected override void ItemDrop()
    {
        // ドロップするお金を決定
        int drop_money = Random.Range(1, 20);
        // ドロップするお金(各種の数)を決定
        int drop_gold = drop_money / 10;
        int drop_silver = drop_money % 10 / 5;
        int drop_bronze = drop_money % 10 % 5;

        // お金のドロップ
        for (int i = 0; i < drop_bronze; i++)
            Drop("Money_bronze", Random.Range(1.125f, 2f), Random.Range(200f, 300f));
        for (int i = 0; i < drop_silver; i++)
            Drop("Money_silver", Random.Range(1.125f, 2f), Random.Range(200f, 300f));
        for (int i = 0; i < drop_gold; i++)
            Drop("Money_gold", Random.Range(1.125f, 2f), Random.Range(200f, 300f));

        // 確率で回復ポーションをドロップ
        if (Random.Range(0, 100) == 0)
        {
            Drop("HP_Potion", Random.Range(1.125f, 2f), Random.Range(200f, 300f));
        }


    }

    //##====================================================##
    //##                   固有の行動処理                   ##
    //##====================================================##
    protected override void OriginalAction()
    {
        
        // ダメージを受けていない、プレイヤーがいるなら
        if (!is_damage && player != null)
        {
            // プレイヤーの方向へ移動
            Move(my_status.Speed, player.transform.position.x > transform.position.x ? 1f : -1f);
        }
    }

    
}

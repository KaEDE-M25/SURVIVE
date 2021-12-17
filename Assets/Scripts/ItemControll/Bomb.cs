using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//--====================================================--
//--                 アイテム　ばくだん                 --
//--====================================================--
public class Bomb : Items
{
    public static readonly ItemData item_data = EigenValue.ITEM_BOMB;
    
    // 投擲距離（中心地）
    public static readonly float THROW_DISTANCE_CENTER = 200f;
    // 投擲距離（中心からのずれの最大値）
    public static readonly float THROW_DISTANCE_ERROR = 50f;

    protected override void GetItem()
    {
        GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControll>().Set_item_stock_from_catch(item_data.item_id);
    }

    public override bool Hold_Effect()
    {
        return false;
    }

    public override bool Drop()
    {
        return false;
    }

    public override bool Use()
    {
        GameObject player = GameObject.FindGameObjectsWithTag("Player")[0];
        Fighters chara_cp = player.GetComponent<Fighters>();
        // 投擲アニメーションを再生
        chara_cp.Anim.Play("ItemUse_throw");

        // エフェクト（爆弾のobj）を出す
        bool mirror = chara_cp.transform.localScale.x > 0;
        GameObject bomb_ef = chara_cp.Play_Effect("EF_bomb", Vector2.zero, mirror);
        bomb_ef.GetComponent<Rigidbody2D>().velocity = new Vector2(Random.Range(THROW_DISTANCE_CENTER - THROW_DISTANCE_ERROR,THROW_DISTANCE_CENTER + THROW_DISTANCE_ERROR) * (mirror ? -1f:1f), 300f);


        return true;
    }

}

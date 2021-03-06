using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//--====================================================--
//--             アイテム  魔力祈願のおまもり           --
//--====================================================--
public class MP_Amulet : Items
{
    public static readonly ItemData item_data = EigenValue.ITEM_MP_AMULET;

    protected override void GetItem()
    {
        GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControll>().Set_item_stock_from_catch(item_data.item_id,this);
    }

    public override bool Use()
    {
        return false;
    }

    public override bool Drop()
    {
        // 最大MPを減少させる処理
        Fighters player_cp = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<Fighters>();
        player_cp.Max_MP -= 2;

        // 最大MPの減少によって現在MPが超過したらその分は削る
        if (player_cp.MP > player_cp.Max_MP)
            player_cp.MP = player_cp.Max_MP;

        // UI上のゲージに反映
        player_cp.Mp_bar.RenewGaugeAmount();
        return true;
    }

    public override bool Hold_Effect()
    {
        // 最大MPを増加させる処理
        Fighters player_cp = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<Fighters>();
        player_cp.Max_MP += 2;
        // UI上のゲージに反映
        player_cp.Mp_bar.RenewGaugeAmount();

        return true;
    }
}

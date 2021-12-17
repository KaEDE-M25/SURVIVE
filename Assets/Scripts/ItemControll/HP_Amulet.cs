using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//--====================================================--
//--            アイテム　体力祈願のおまもり            --
//--====================================================--
public class HP_Amulet : Items
{
    public static readonly ItemData item_data = EigenValue.ITEM_HP_AMULET;

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
        // 最大HPを減少させる処理
        Fighters player_cp = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<Fighters>();
        player_cp.Max_HP -= 2;

        // 最大HPの減少によって現在MPが超過したらその分は削る
        if (player_cp.HP > player_cp.Max_HP)
            player_cp.HP = player_cp.Max_HP;


        // UI上のゲージに反映
        player_cp.Hp_bar.RenewGaugeAmount();
        return true;
    }

    public override bool Hold_Effect()
    {
        // 最大HPを増加させる処理
        Fighters player_cp = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<Fighters>();
        player_cp.Max_HP += 2;
        // UI上のゲージに反映
        player_cp.Hp_bar.RenewGaugeAmount();

        return true;
    }

}

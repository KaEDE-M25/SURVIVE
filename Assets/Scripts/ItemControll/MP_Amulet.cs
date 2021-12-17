using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//--====================================================--
//--             ƒAƒCƒeƒ€  –‚—Í‹FŠè‚Ì‚¨‚Ü‚à‚è           --
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
        // Å‘åMP‚ğŒ¸­‚³‚¹‚éˆ—
        Fighters player_cp = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<Fighters>();
        player_cp.Max_MP -= 2;

        // Å‘åMP‚ÌŒ¸­‚É‚æ‚Á‚ÄŒ»İMP‚ª’´‰ß‚µ‚½‚ç‚»‚Ì•ª‚Íí‚é
        if (player_cp.MP > player_cp.Max_MP)
            player_cp.MP = player_cp.Max_MP;

        // UIã‚ÌƒQ[ƒW‚É”½‰f
        player_cp.Mp_bar.RenewGaugeAmount();
        return true;
    }

    public override bool Hold_Effect()
    {
        // Å‘åMP‚ğ‘‰Á‚³‚¹‚éˆ—
        Fighters player_cp = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<Fighters>();
        player_cp.Max_MP += 2;
        // UIã‚ÌƒQ[ƒW‚É”½‰f
        player_cp.Mp_bar.RenewGaugeAmount();

        return true;
    }
}

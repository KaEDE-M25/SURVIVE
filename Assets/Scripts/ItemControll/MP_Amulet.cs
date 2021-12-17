using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//--====================================================--
//--             �A�C�e��  ���͋F��̂��܂���           --
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
        // �ő�MP�����������鏈��
        Fighters player_cp = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<Fighters>();
        player_cp.Max_MP -= 2;

        // �ő�MP�̌����ɂ���Č���MP�����߂����炻�̕��͍��
        if (player_cp.MP > player_cp.Max_MP)
            player_cp.MP = player_cp.Max_MP;

        // UI��̃Q�[�W�ɔ��f
        player_cp.Mp_bar.RenewGaugeAmount();
        return true;
    }

    public override bool Hold_Effect()
    {
        // �ő�MP�𑝉������鏈��
        Fighters player_cp = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<Fighters>();
        player_cp.Max_MP += 2;
        // UI��̃Q�[�W�ɔ��f
        player_cp.Mp_bar.RenewGaugeAmount();

        return true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//--====================================================--
//--            �A�C�e���@�̗͋F��̂��܂���            --
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
        // �ő�HP�����������鏈��
        Fighters player_cp = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<Fighters>();
        player_cp.Max_HP -= 2;

        // �ő�HP�̌����ɂ���Č���MP�����߂����炻�̕��͍��
        if (player_cp.HP > player_cp.Max_HP)
            player_cp.HP = player_cp.Max_HP;


        // UI��̃Q�[�W�ɔ��f
        player_cp.Hp_bar.RenewGaugeAmount();
        return true;
    }

    public override bool Hold_Effect()
    {
        // �ő�HP�𑝉������鏈��
        Fighters player_cp = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<Fighters>();
        player_cp.Max_HP += 2;
        // UI��̃Q�[�W�ɔ��f
        player_cp.Hp_bar.RenewGaugeAmount();

        return true;
    }

}

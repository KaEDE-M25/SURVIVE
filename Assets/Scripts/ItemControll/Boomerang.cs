using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//--====================================================--
//--                 �A�C�e���@�u�[������               --
//--====================================================--
public class Boomerang : Items
{
    public static readonly ItemData item_data = EigenValue.ITEM_BOOMERANG;

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
        // �����A�j���[�V�������Đ�
        chara_cp.Anim.Play("ItemUse_throw");

        // �G�t�F�N�g�i�u�[��������obj�j���o��
        bool mirror = chara_cp.transform.localScale.x > 0;
        chara_cp.Play_Effect("EF_boomerang", Vector2.zero,mirror);

        return true;
    }


}

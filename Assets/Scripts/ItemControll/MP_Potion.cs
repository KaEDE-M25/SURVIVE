using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//--====================================================--
//--               アイテム  MPポーション               --
//--====================================================--
public class MP_Potion : Items
{
    public static readonly ItemData item_data = EigenValue.ITEM_MP_POTION;

    protected override void GetItem()
    {
        GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControll>().Set_item_stock_from_catch(item_data.item_id);
    }

    public override bool Use()
    {
        GameObject player = GameObject.FindGameObjectsWithTag("Player")[0];
        Fighters chara_cp = player.GetComponent<Fighters>();
        // ポーション使用アニメーションを再生
        chara_cp.Anim.Play("ItemUse_raise");



        // エフェクトを、使用したアイテムに切り替えて再生
        GameObject effect = player.transform.Find("EF_ItemUse").gameObject;
        effect.transform.localPosition = Vector3.zero;
        effect.GetComponent<SpriteRenderer>().sprite = transform.Find("graphic").GetComponent<SpriteRenderer>().sprite;
        effect.SetActive(false);
        effect.SetActive(true);

        // 使用アイテムが上に飛んでくモーションの後にMP回復とエフェクト発生
        effect.transform.DOLocalMoveY(20f, 0.5f).SetEase(Ease.OutCirc).OnComplete(() =>
        {
            AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_PLAYER, AudioFilePositions.EFFECT.REGENERATION);
            chara_cp.ReductMP(-5);
            player.transform.Find("EF_MPregene").gameObject.SetActive(true);
            effect.SetActive(false);
        }
        );

        return true;
    }

    public override bool Drop()
    {
        return false;
    }

    public override bool Hold_Effect()
    {
        return false;
    }
}

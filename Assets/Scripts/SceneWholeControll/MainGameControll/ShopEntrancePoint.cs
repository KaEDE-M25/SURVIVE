using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//--====================================================--
//--              入店判定を管理するクラス              --
//--   店の入り口となる地点に置いたobjにアタッチする    --
//--         ※入店操作の管理はGameGontrollが行う       --
//--====================================================--
public class ShopEntrancePoint : MonoBehaviour
{ 
    // ミセに入れるかどうかの判定値
    public bool Can_enter_shop { get; private set; } = false;

    //##====================================================##
    //##      collisionに入ったら入店可能、出たら戻す       ##
    //##====================================================##
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Can_enter_shop = true;
            transform.Find(OptionData.current_options.controller).gameObject.SetActive(true);
        }

    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Can_enter_shop = false;
            transform.Find(OptionData.current_options.controller).gameObject.SetActive(false);
        }

    }
}

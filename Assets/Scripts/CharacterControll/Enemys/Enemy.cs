using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//--====================================================--
//--                   敵キャラクター                   --
//--====================================================--
public abstract class Enemy : Character
{
    // プレイヤーのobj
    protected GameObject player;
    
    
    void Start()
    {
        // プレイヤーを捕捉
        player = GameObject.FindGameObjectsWithTag("Player")[0];

        // その他必要な初期化をする
        Initialize();
        
    }


    //$$====================================================$$
    //$$            Startで行うステータス初期化             $$
    //$$====================================================$$
    protected abstract void Initialize();

    //--====================================================--
    //--    HPを増減させる処理(アイテムドロップ処理を追加)  --
    //--====================================================--
    public override void ReductHP(int reduct_num)
    {
        base.ReductHP(reduct_num);

        if (HP <= 0 && is_dead == true)
        {
            //アイテムドロップ
            ItemDrop();
        }

    }

    //##====================================================##
    //##             死亡処理(Animationから呼出)            ##
    //##====================================================##
    public override void Dead()
    {
        GameObject.FindWithTag("GameController").GetComponent<GameControll>().Kill_count(1, my_status.Score);
        Destroy(this.gameObject);
    }

    //$$====================================================$$
    //$$            アイテムドロップ処理(全体               $$
    //$$====================================================$$
    protected abstract void ItemDrop();


    //--====================================================--
    //--              アイテムドロップ(単体)                --
    //--====================================================--
    protected void Drop(string item_name, float angle, float weight)
    {
        GameObject effect = Instantiate(Resources.Load<GameObject>((EigenValue.PREFAB_DIRECTORY_ITEMS + item_name)), transform.localPosition, transform.rotation,game_controller.Active_Items_Parent().transform);
        effect.GetComponent<Rigidbody2D>().velocity = new Vector2(transform.localScale.x * weight * Mathf.Cos(angle), weight * Mathf.Sin(angle));

    }
}

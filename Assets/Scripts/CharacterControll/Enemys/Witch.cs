using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//--====================================================--
//--                 敵キャラ　ウィッチ                   --
//--====================================================--
public class Witch: Enemy
{
    static readonly Vector3 INITIAL_SHOT_POS = new Vector3(-13f, -1f, 0f);

    [SerializeField, Tooltip("ショットのrigidbody2D。発射処理で使用")]
    Rigidbody2D shot_rb2d;
    [SerializeField, Tooltip("発射エフェクトのobj")]
    GameObject fire_effect;

    // ショットエフェクトのレンダラ
    Renderer shot_renderer;
    // プレイヤーとの距離
    float dist_for_player;
    // 魔法攻撃のインターバルを計算するためのカウンタ
    int frame_count = 200;
    // 移動中かどうか
    bool is_move = false;
    // ショットを撃っている途中かどうか
    bool is_shot = false;




    protected override CharacterStatus CharacterStatus()
    {
        // ステータス初期化値
        return EigenValue.STATUS_004_WITCH.status;
    }

    //$$====================================================$$
    //$$            Startで行うステータス初期化             $$
    //$$====================================================$$
    protected override void Initialize() 
    {

        // レンダラーを取得
        shot_renderer = shot_rb2d.GetComponent<Renderer>();
        shot_rb2d.transform.parent = GameObject.FindWithTag("GameController").GetComponent<GameControll>().Active_Effects_Parent().transform;

        // エフェクト軽量化オプションを設定していたら
        if (OptionData.current_options.omitted_effect)
        {
            Destroy(shot_rb2d.transform.Find("detail_particle").gameObject);
        }

    }


    //##====================================================##
    //##                   固有の行動処理                   ##
    //##====================================================##
    protected override void OriginalAction()
    {
        // ウィッチ特有の行動
        
        // ショットがアクティブなら、画面外に飛んだら消す
        if (shot_rb2d.gameObject.activeSelf)
        {
            if (!shot_renderer.isVisible)
            {
                shot_rb2d.gameObject.SetActive(false);
            }

        }

        // ショットを打とうとしているときにダメージを受けたら中断
        if(is_shot) 
        {
            if (is_damage || is_knockback)
            {
                shot_rb2d.gameObject.SetActive(false);
                is_shot = false;
            }
        }
        else
        {
            // 一定距離を保つ（スケルトン参照）
            if (!is_damage && player != null)
            {
                dist_for_player = Distance(transform.position.x, player.transform.position.x);
                if (dist_for_player > 80f && dist_for_player < 100f)
                {
                    rb2d.velocity *= Vector2.up;
                    is_move = false;
                    // 動きを停止
                    if (!is_jump)
                    {
                        // プレイヤーの方を向く
                        Focus(player);

                        // カウント
                        frame_count++;

                        // 一定間隔ごとに魔法攻撃
                        if (frame_count > 100 && !shot_rb2d.gameObject.activeSelf)
                        {
                            is_shot = true;
                            MagicAttack();
                            frame_count = 0;
                        }
                    }
                }
                else // 一定距離を保つ
                {

                    if (rb2d.velocity.x == 0f && !is_jump && is_move)
                        Jump(my_status.Jump_power);

                    Move(my_status.Speed, player.transform.position.x > transform.position.x ^ dist_for_player >= 100f ? -1f : 1f);


                    /*
                    ※ player.transform.position.x > transform.position.x ^ dist_for_player >= 100f ? -1f : 1f
                    は、以下を簡略化したもの。

                    // プレイヤーが右、敵が左にいるなら
                    if (player.transform.position.x > transform.position.x)
                    {
                        // 遠すぎたら近づく
                        if (dist_for_player >= 100f)
                            Move(my_status.Speed, 1f);
                        else
                            Move(my_status.Speed, -1f);
                    }
                    else if (player.transform.position.x < transform.position.x)
                    {
                        if (dist_for_player >= 100f)
                            Move(my_status.Speed, -1f);
                        else
                            Move(my_status.Speed, 1f);
                    }
                    */
                    is_move = true;
                }
            }
        }
    }



    //##====================================================##
    //##                 ウィッチで使う処理                 ##
    //##====================================================##
    // 魔法攻撃をする(Animを開始する)
    void MagicAttack()
    {
        Anim.Play("magic");
    }

    // 魔法弾を作る(ように見せかけて子のショットobjをアクティブにする)
    void CreateShot()
    {
        if (!is_damage && !is_knockback)
        {
            shot_rb2d.transform.position = transform.position + INITIAL_SHOT_POS * transform.localScale.x;
            shot_rb2d.gameObject.SetActive(true);
        }
    }

    // ショットを放つ
    void FireShot()
    {
        if (!is_damage && !is_knockback) 
        {
            fire_effect.SetActive(true);
            shot_rb2d.velocity = new Vector2(transform.position.x - player.transform.position.x, transform.position.y - player.transform.position.y).normalized * -200f;            
        }
    }

    // ショット操作を終わる（animationから呼出）
    void End_of_shot_anim()
    {
        is_shot = false;
    }

    //##====================================================##
    //##             死亡処理(Animationから呼出)            ##
    //##====================================================##
    public override void Dead()
    {
        // 攻撃エフェクトの親をこのキャラクターに戻す（Destroyでまとめて消すため）
        shot_rb2d.gameObject.SetActive(false);
        shot_rb2d.transform.parent = this.transform;

        base.Dead();
    }


    //##====================================================##
    //##                 死亡した瞬間の処理                 ##
    //##====================================================##
    public override void Moment_of_Dead()
    {
        shot_rb2d.gameObject.SetActive(false);
        is_shot = false;
    }


    //##====================================================##
    //##        アイテムドロップ処理                        ##
    //##====================================================##
    protected override void ItemDrop()
    {
        // ドロップするお金を決定
        int drop_money = Random.Range(60, 80);
        // ドロップするお金(各種の数)を決定
        int drop_gold = drop_money / 10;
        int drop_silver = drop_money % 10 / 5;
        int drop_bronze = drop_money % 10 % 5;

        // お金のドロップ
        for (int i = 0; i < drop_bronze; i++)
            Drop("Money_bronze", Random.Range(1.125f, 2f), Random.Range(200f, 300f));
        for (int i = 0; i < drop_silver; i++)
            Drop("Money_silver", Random.Range(1.125f, 2f), Random.Range(200f, 300f));
        for (int i = 0; i < drop_gold; i++)
            Drop("Money_gold", Random.Range(1.125f, 2f), Random.Range(200f, 300f));

        // 確率でMPポーションをドロップ
        if (Random.Range(0, 10) == 0)
        {
            Drop("MP_Potion", Random.Range(1.125f, 2f), Random.Range(200f, 300f));
        }
    }



}

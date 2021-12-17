using System.Collections;
using System.Collections.Generic;
using UnityEngine;




//--====================================================--
//--              プレイヤーキャラクター                --
//--====================================================--
public abstract class Fighters : Character
{

    // UI上のバー
    public UI_StatusBar Hp_bar { get; protected set; }
    public UI_StatusBar Mp_bar { get; protected set; }

    // 攻撃ID
    protected int attackID = 0;
    // 攻撃中かどうか
    protected bool is_attack = false;
    // アイテム使用中かどうか
    public bool is_itemuse = false;

    protected override void Status_Initialize(CharacterStatus status)
    {
        base.Status_Initialize(status);

        // UI上のバーを指定
        Hp_bar = GameObject.Find("hp_bar").GetComponent<UI_StatusBar>();
        Mp_bar = GameObject.Find("mp_bar").GetComponent<UI_StatusBar>();


        // 起き上がり時間
        wakeup_time = 1f;

        Initialize_Fighter();
    }

    // 各プレイヤーキャラクターで個別に必要な処理
    protected abstract void Initialize_Fighter();

    protected override void OriginalAction()
    {
        if (!is_damage && !is_itemuse)
        {
            Attack_Controll();

            //　攻撃中でなければ
            if (!is_attack)
            {
                Jump_Controll();
                Move_Controll();
                if (!is_itemuse)
                    ItemUse_Controll();
            }
        }

    }
    //--====================================================--
    //--                HPを減少させる処理                  --
    //--====================================================--
    public override void ReductHP(int reduct_num)
    {
        base.ReductHP(reduct_num);
        // UI上のHPバーを減らす処理
        Hp_bar.GaugeReduct(reduct_num);
    }
    //--====================================================--
    //--                MPを減少させる処理                  --
    //--====================================================--
    public override void ReductMP(int reduct_num)
    {
        base.ReductMP(reduct_num);
        // UI上のMPバーを減らす処理
        Mp_bar.GaugeReduct(reduct_num);

    }
    //--====================================================--
    //--              Animatorに値を渡す処理                --
    //--====================================================--
    protected override void Set_AnimValue() 
    {
        base.Set_AnimValue();

        // 攻撃IDと攻撃中フラグを追加
        Anim.SetInteger("attackID", attackID);
        Anim.SetBool("attack", is_attack);
    }

    //##====================================================##
    //##            死亡時処理(Animationから呼出)           ##
    //##====================================================##
    public override void Dead()
    {
        this.gameObject.SetActive(false);
    }

    //--====================================================--
    //--           ジャンプ操作を管理する処理               --
    //--====================================================--
    void Jump_Controll()
    {
        // ジャンプボタンを押したらジャンプする
        if (InputControll.GetInputDown(InputControll.INPUT_ID_B) && !is_jump)
        {
            Jump(my_status.Jump_power);
            AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_PLAYER, AudioFilePositions.EFFECT.JUMP, GetComponent<Renderer>());
        }
        // ジャンプ中にジャンプボタンを離したらジャンプ中断~
        if (InputControll.GetInputUp(InputControll.INPUT_ID_B) && is_jump && rb2d.velocity.y > 0)
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, rb2d.velocity.y / 2f);
        }

    }

    //--====================================================--
    //--              移動操作を管理する処理                --
    //--====================================================--
    void Move_Controll()
    {
        int direction;

        // 移動量を入力から測定
        float x_val = InputControll.GetInput_Move();


        // 方向を決定
        if (x_val == 0)
            direction = 0;
        else
            direction = x_val > 0 ? 1 : -1;

        //移動
        x_val = Mathf.Abs(x_val);
        Move(my_status.Speed * x_val, direction);
    }



    //--====================================================--
    //--            アイテム使用操作を管理する処理          --
    //--====================================================--
    protected void ItemUse_Controll() 
    {
        // アイテム使用ボタンを押したらアイテム使用
        if (InputControll.GetInputDown(InputControll.INPUT_ID_Y) && !is_jump) 
        {
            if (game_controller.Use_item_from_stock())
            {
                rb2d.velocity = Vector2.zero;
                is_itemuse = true;
            }
        }


    }

    //--====================================================--
    //--     アイテムの使用を終了する   (Animから呼出)      --
    //--====================================================--
    protected void End_of_ItemUse() 
    {
        is_itemuse = false;
    }


    //--====================================================--
    //--              攻撃操作を管理する処理                --
    //--====================================================--
    protected abstract void Attack_Controll();

    //--====================================================--
    //--      攻撃Animをリセットする処理(Animから呼出)      --
    //--====================================================--
    void Reset_Attack_Animation()
    {
        attackID = 0;
        is_attack = false;
    }
    //--====================================================--
    //--                攻撃エフェクトの再生                --
    //--====================================================--
    protected abstract void Attack_Effect_Play(int attackID);

    //--====================================================--
    //--                 移動をリセットする                 -- // ダッシュ攻撃実装検討時の残骸
    //--====================================================--
    void Reset_Move()
    {
        rb2d.velocity *= Vector2.up;
    }




}

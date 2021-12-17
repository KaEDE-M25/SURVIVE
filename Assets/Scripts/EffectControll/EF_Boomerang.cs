using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//--====================================================--
//--               エフェクト：ブーメラン               --
//--====================================================--
public class EF_Boomerang : MonoBehaviour
{
    // ブーメランが帰ってこない確率
    public const float THROW_FAILED_PROBABILITY = 0.1f;
    // ブーメランの投擲距離
    public const float THROW_POWER = 200f;

    Rigidbody2D rb2d;
    new Renderer renderer;
    GameControll game_controll;
    bool throw_failed = false;


    private void Awake()
    {
        game_controll = GameObject.FindWithTag("GameController").GetComponent<GameControll>();
    }

    private void Start()
    {
        renderer = GetComponent<Renderer>();
        rb2d = GetComponent<Rigidbody2D>();
        rb2d.velocity = (THROW_POWER * Vector2.left * transform.localScale.x);
        // 投擲失敗(戻ってこなくなる)判定
        throw_failed = Random.value <= THROW_FAILED_PROBABILITY;
    }

    private void Update()
    {
        // 投擲失敗時はそのまま画面外に出たら消える
        if (throw_failed)
        {
            if (!renderer.isVisible)
                Destroy(this.gameObject);
        }
        // 投擲成功時は帰ってきている状況で画面外に出た時のみ消える
        else if (transform.localScale.x < 0f ? (rb2d.velocity.x < 0f) : (rb2d.velocity.x > 0f))
            if (!renderer.isVisible)
                Destroy(this.gameObject);
    }

    private void FixedUpdate()
    {
        // 戻ってくるように速度ベクトルを徐々に変更
        // 処理落ちにより必要以上に飛ぶのを避けるためfixedに入れている
        if (!throw_failed)
            rb2d.velocity += (Vector2.left * -transform.localScale.x * 3f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 帰ってきている時にプレイヤーに触れたらキャッチ(アイテムストックに再格納)
        if(transform.localScale.x < 0f ? (rb2d.velocity.x < 0f) : !(rb2d.velocity.x < 0f))
            if (collision.transform.CompareTag("Player"))
            {
                game_controll.Set_item_stock_from_catch(EigenValue.ITEM_BOOMERANG.item_id);
                Destroy(this.gameObject);

            }
    }
}

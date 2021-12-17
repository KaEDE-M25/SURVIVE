using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//--====================================================--
//--                      アイテム                      --
//--====================================================--

public abstract class Items : MonoBehaviour
{
    // プレイヤーのobj
    GameObject player;
    // 子objにあるアイテムを描画するオブジェクト
    [SerializeField]
    GameObject graphic = null;

    bool is_get = false;


    void Start()
    {
        player = GameObject.FindGameObjectsWithTag("Player")[0];
    }

    private void LateUpdate()
    {
        if (is_get) 
        {
            // 円運動の際にグラフィックが回転されないように元に戻す
            graphic.transform.rotation = Quaternion.identity;
        }
    }

    private void Update()
    {
        // 獲得判定モードに入ったら
        if (is_get) 
        {
            // 円運動
            transform.RotateAround(player.transform.position, Vector3.forward,360f / 0.5f * Time.deltaTime);

            // 獲得判定
            Vector2 var;
            var.x = player.transform.position.x - transform.position.x;
            var.y = player.transform.position.y - transform.position.y;

            transform.position = transform.position + (Vector3)var / 10f;

            // 一定距離内に入ったら獲得関数を実行
            if (var.magnitude < 3f)
            {
                AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_ITEM, AudioFilePositions.EFFECT.GETITEM);
                Destroy(this.gameObject);
                GetItem();
            }

        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 円形範囲にプレイヤーが入ったら獲得判定モードへ
        if (collision.gameObject == player) 
        {
            is_get = true;
            this.GetComponent<Rigidbody2D>().simulated = false;
            this.gameObject.layer = 1;
        }
    }



    // 床に落ちてるアイテムを手に入れた時の処理
    protected abstract void GetItem();
    // 使用した時の処理 (bool値を返し、Trueは使う系アイテム、Falseは所持系アイテムを示させる)
    public abstract bool Use();
    // 捨てたり売ったりした時の処理 (永続効果系アイテムの効果解除処理などに。なければfalseを示させる)
    public abstract bool Drop();
    // 所持していることで発揮する効果の処理（効果がない場合はfalseを示させる）（持った瞬間にする処理を記述）
    public abstract bool Hold_Effect();

}

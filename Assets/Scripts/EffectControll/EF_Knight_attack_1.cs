using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//--====================================================--
//--       エフェクト：けんしの通常攻撃１の衝撃波       --
//--====================================================--
public class EF_Knight_attack_1 : MonoBehaviour
{
    // 衝撃波の初期速度
    public const float INITIAL_SPEED = 300f;
    // 衝撃波の最高速度
    public const float MAX_SPEED = 400f;

    // 攻撃が当たった回数。
    [SerializeField]
    int attack_num = 0;

    Rigidbody2D rb2d;
    new Renderer renderer;

    GameControll game_controlll;
    Fighters fighter_comp;

    private void Awake()
    {
        game_controlll = GameObject.FindWithTag("GameController").GetComponent<GameControll>();
        fighter_comp = GameObject.FindWithTag("Player").GetComponent<Fighters>();
    }

    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<Renderer>();
        rb2d = GetComponent<Rigidbody2D>();
        rb2d.velocity += (INITIAL_SPEED * Vector2.left * transform.localScale.x);
    }

    // Update is called once per frame
    void Update()
    {
        // 加速
        rb2d.velocity += (Vector2.left * transform.localScale.x);
        
        // 画面外に出たら消滅
        if (!renderer.isVisible)
            Destroy(this.gameObject);

        // 停止したら(移動量が劇的に小さくなったら)消滅
        if(Mathf.Abs(rb2d.velocity.x) <= 10) 
        {
            // エフェクト省略オプションの適用
            if (OptionData.current_options.omitted_effect)
            {
                Transform child_transform = transform.GetChild(0);
                child_transform.parent = game_controlll.Active_Effects_Parent().transform;
                var sys = child_transform.GetComponent<ParticleSystem>().main;
                sys.loop = false;
                child_transform.localScale = transform.localScale;
            }
            Destroy(this.gameObject);
        }
        // 最高速度に制限
        else if (Mathf.Abs(rb2d.velocity.x) > MAX_SPEED)
        {
            rb2d.velocity = Vector2.right * MAX_SPEED * transform.localScale.x + rb2d.velocity * Vector2.up;
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.transform.CompareTag("Enemy"))
        {
            attack_num++;
            // 敵に当たった回数がプレイヤーのHP/2を上回ったら消滅
            if (attack_num >= fighter_comp.HP / 2)
                Destroy(this.gameObject);

        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//--====================================================--
//--       �G�t�F�N�g�F���񂵂̒ʏ�U���P�̏Ռ��g       --
//--====================================================--
public class EF_Knight_attack_1 : MonoBehaviour
{
    // �Ռ��g�̏������x
    public const float INITIAL_SPEED = 300f;
    // �Ռ��g�̍ō����x
    public const float MAX_SPEED = 400f;

    // �U�������������񐔁B
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
        // ����
        rb2d.velocity += (Vector2.left * transform.localScale.x);
        
        // ��ʊO�ɏo�������
        if (!renderer.isVisible)
            Destroy(this.gameObject);

        // ��~������(�ړ��ʂ����I�ɏ������Ȃ�����)����
        if(Mathf.Abs(rb2d.velocity.x) <= 10) 
        {
            // �G�t�F�N�g�ȗ��I�v�V�����̓K�p
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
        // �ō����x�ɐ���
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
            // �G�ɓ��������񐔂��v���C���[��HP/2�������������
            if (attack_num >= fighter_comp.HP / 2)
                Destroy(this.gameObject);

        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//--====================================================--
//--               �G�t�F�N�g�F�u�[������               --
//--====================================================--
public class EF_Boomerang : MonoBehaviour
{
    // �u�[���������A���Ă��Ȃ��m��
    public const float THROW_FAILED_PROBABILITY = 0.1f;
    // �u�[�������̓�������
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
        // �������s(�߂��Ă��Ȃ��Ȃ�)����
        throw_failed = Random.value <= THROW_FAILED_PROBABILITY;
    }

    private void Update()
    {
        // �������s���͂��̂܂܉�ʊO�ɏo���������
        if (throw_failed)
        {
            if (!renderer.isVisible)
                Destroy(this.gameObject);
        }
        // �����������͋A���Ă��Ă���󋵂ŉ�ʊO�ɏo�����̂ݏ�����
        else if (transform.localScale.x < 0f ? (rb2d.velocity.x < 0f) : (rb2d.velocity.x > 0f))
            if (!renderer.isVisible)
                Destroy(this.gameObject);
    }

    private void FixedUpdate()
    {
        // �߂��Ă���悤�ɑ��x�x�N�g�������X�ɕύX
        // ���������ɂ��K�v�ȏ�ɔ�Ԃ̂�����邽��fixed�ɓ���Ă���
        if (!throw_failed)
            rb2d.velocity += (Vector2.left * -transform.localScale.x * 3f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // �A���Ă��Ă��鎞�Ƀv���C���[�ɐG�ꂽ��L���b�`(�A�C�e���X�g�b�N�ɍĊi�[)
        if(transform.localScale.x < 0f ? (rb2d.velocity.x < 0f) : !(rb2d.velocity.x < 0f))
            if (collision.transform.CompareTag("Player"))
            {
                game_controll.Set_item_stock_from_catch(EigenValue.ITEM_BOOMERANG.item_id);
                Destroy(this.gameObject);

            }
    }
}

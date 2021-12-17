using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//--====================================================--
//--                      �A�C�e��                      --
//--====================================================--

public abstract class Items : MonoBehaviour
{
    // �v���C���[��obj
    GameObject player;
    // �qobj�ɂ���A�C�e����`�悷��I�u�W�F�N�g
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
            // �~�^���̍ۂɃO���t�B�b�N����]����Ȃ��悤�Ɍ��ɖ߂�
            graphic.transform.rotation = Quaternion.identity;
        }
    }

    private void Update()
    {
        // �l�����胂�[�h�ɓ�������
        if (is_get) 
        {
            // �~�^��
            transform.RotateAround(player.transform.position, Vector3.forward,360f / 0.5f * Time.deltaTime);

            // �l������
            Vector2 var;
            var.x = player.transform.position.x - transform.position.x;
            var.y = player.transform.position.y - transform.position.y;

            transform.position = transform.position + (Vector3)var / 10f;

            // ��苗�����ɓ�������l���֐������s
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
        // �~�`�͈͂Ƀv���C���[����������l�����胂�[�h��
        if (collision.gameObject == player) 
        {
            is_get = true;
            this.GetComponent<Rigidbody2D>().simulated = false;
            this.gameObject.layer = 1;
        }
    }



    // ���ɗ����Ă�A�C�e������ɓ��ꂽ���̏���
    protected abstract void GetItem();
    // �g�p�������̏��� (bool�l��Ԃ��ATrue�͎g���n�A�C�e���AFalse�͏����n�A�C�e������������)
    public abstract bool Use();
    // �̂Ă��蔄�����肵�����̏��� (�i�����ʌn�A�C�e���̌��ʉ��������ȂǂɁB�Ȃ����false����������)
    public abstract bool Drop();
    // �������Ă��邱�ƂŔ���������ʂ̏����i���ʂ��Ȃ��ꍇ��false����������j�i�������u�Ԃɂ��鏈�����L�q�j
    public abstract bool Hold_Effect();

}

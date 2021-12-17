using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//--====================================================--
//--                   �G�L�����N�^�[                   --
//--====================================================--
public abstract class Enemy : Character
{
    // �v���C���[��obj
    protected GameObject player;
    
    
    void Start()
    {
        // �v���C���[��ߑ�
        player = GameObject.FindGameObjectsWithTag("Player")[0];

        // ���̑��K�v�ȏ�����������
        Initialize();
        
    }


    //$$====================================================$$
    //$$            Start�ōs���X�e�[�^�X������             $$
    //$$====================================================$$
    protected abstract void Initialize();

    //--====================================================--
    //--    HP�𑝌������鏈��(�A�C�e���h���b�v������ǉ�)  --
    //--====================================================--
    public override void ReductHP(int reduct_num)
    {
        base.ReductHP(reduct_num);

        if (HP <= 0 && is_dead == true)
        {
            //�A�C�e���h���b�v
            ItemDrop();
        }

    }

    //##====================================================##
    //##             ���S����(Animation����ďo)            ##
    //##====================================================##
    public override void Dead()
    {
        GameObject.FindWithTag("GameController").GetComponent<GameControll>().Kill_count(1, my_status.Score);
        Destroy(this.gameObject);
    }

    //$$====================================================$$
    //$$            �A�C�e���h���b�v����(�S��               $$
    //$$====================================================$$
    protected abstract void ItemDrop();


    //--====================================================--
    //--              �A�C�e���h���b�v(�P��)                --
    //--====================================================--
    protected void Drop(string item_name, float angle, float weight)
    {
        GameObject effect = Instantiate(Resources.Load<GameObject>((EigenValue.PREFAB_DIRECTORY_ITEMS + item_name)), transform.localPosition, transform.rotation,game_controller.Active_Items_Parent().transform);
        effect.GetComponent<Rigidbody2D>().velocity = new Vector2(transform.localScale.x * weight * Mathf.Cos(angle), weight * Mathf.Sin(angle));

    }
}

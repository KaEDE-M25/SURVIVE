using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

//--====================================================--
//--        �G�t�F�N�g�F���񂵂̂l�o�U���̏Ռ��g        --
//--====================================================--
public class EF_Knight_attack_mp1 : MonoBehaviour
{
    [SerializeField]
    ParticleSystem ps;

    Tween tween;

    // �U������p
    GameControll game_controll;

    private void Awake()
    {
        game_controll = GameObject.FindWithTag("GameController").GetComponent<GameControll>();        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            // �U���̃R���[�`�����N��
            game_controll.Camera_controll.StartCoroutine(game_controll.Camera_controll.Shake(0.3f, 10f));
        }
    }



    private void OnEnable()
    {
        tween?.Kill();
        transform.rotation = Quaternion.identity;
        // tween�ɂ��Ռ��g�̔���pobj����]������
        tween = this.transform.DORotate(new Vector3(0f, 0f ,180f * transform.parent.parent.localScale.x),0.45f).SetEase(Ease.InOutCirc);
    }

    private void OnDisable()
    {
        tween?.Kill();
    }

}

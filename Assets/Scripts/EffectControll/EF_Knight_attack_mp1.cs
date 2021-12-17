using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

//--====================================================--
//--        エフェクト：けんしのＭＰ攻撃の衝撃波        --
//--====================================================--
public class EF_Knight_attack_mp1 : MonoBehaviour
{
    [SerializeField]
    ParticleSystem ps;

    Tween tween;

    // 振動操作用
    GameControll game_controll;

    private void Awake()
    {
        game_controll = GameObject.FindWithTag("GameController").GetComponent<GameControll>();        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            // 振動のコルーチンを起動
            game_controll.Camera_controll.StartCoroutine(game_controll.Camera_controll.Shake(0.3f, 10f));
        }
    }



    private void OnEnable()
    {
        tween?.Kill();
        transform.rotation = Quaternion.identity;
        // tweenにより衝撃波の判定用objを回転させる
        tween = this.transform.DORotate(new Vector3(0f, 0f ,180f * transform.parent.parent.localScale.x),0.45f).SetEase(Ease.InOutCirc);
    }

    private void OnDisable()
    {
        tween?.Kill();
    }

}

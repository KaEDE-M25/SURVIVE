using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//--====================================================--
//--        カメラの動作をコントロールするクラス        --
//--====================================================--
public class CameraControll : MonoBehaviour
{
    GameObject player;
    Vector3 vec;

    // 振動しているかどうか
    bool is_shake = false;
    // 振動している時間を記録するタイマー
    float elapsed = 0f;

    //##====================================================##
    //##                     Start 初期化                   ##
    //##====================================================##
    void Start()
    {
        // プレイヤーを取得
        player = GameObject.FindGameObjectsWithTag("Player")[0];
        vec.y = transform.position.y;
        vec.z = transform.position.z;

    }

    //##====================================================##
    //##                      LateUpdate                    ##
    //##====================================================##
    private void LateUpdate()
    {
        if (player != null && !is_shake)
        {
            // 描画前に位置を更新
            vec.x = player.transform.position.x;

            transform.position = vec;
        }
    }

    //##====================================================##
    //##                振動をするコルーチン                ##
    //##====================================================##
    public IEnumerator Shake(float duration, float magnitude)
    {
        // 振動をしないオプションだった場合は即停止
        if (!OptionData.current_options.is_play_shake)
            yield break;

        elapsed = 0f;

        if (is_shake)
            yield break;

        is_shake = true;

        while(elapsed < duration) 
        {

            vec.x = player.transform.position.x;

            transform.position = vec + Random.insideUnitSphere * magnitude;
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = vec;
        is_shake = false;
    }

}

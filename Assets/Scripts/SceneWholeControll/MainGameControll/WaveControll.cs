using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//--====================================================--
//--           ウェーブの進行を管理するクラス           --
//--====================================================--
public class WaveControll : MonoBehaviour
{
    // タイマー
    [SerializeField]
    Timer timer;
    // タイマーを表示するテキスト
    [SerializeField]
    Text timer_text;
    // ゲーム進行を操作するコンポーネント
    [SerializeField]
    GameControll game_controll;
    // ウェーブ進行度を表すバーのスプライト
    [SerializeField]
    Image fill_gauge;
    [SerializeField]
    Image empty_gauge;

    //　計算用変数
    Vector2 fill_gauge_size = new Vector2(150f,6f);

    private void Update()
    {
            // テキストに反映
            timer_text.text = timer.Current_time.ToString();
            // ゲージに反映
            fill_gauge_size.x = ((float)timer.Max_count - (float)timer.Current_time) / (float)timer.Max_count * empty_gauge.rectTransform.sizeDelta.x;
            fill_gauge.rectTransform.sizeDelta = fill_gauge_size;

    }

    // timerのゲッター
    public Timer Timer() { return timer; }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//--====================================================--
//--        UI上の各種カウンタを管理するクラス          --
//--====================================================--
public class CountControll : MonoBehaviour
{
    // 各種カウンターオブジェクトのTextコンポーネント
    [SerializeField]
    Text kill_count;
    [SerializeField]
    Text money_count;
    [SerializeField]
    Text wave_count;
    [SerializeField]
    Text score_count;
    [SerializeField]
    Text enemy_count;

    private void Awake()
    {
        // 初期化
        Set_kill_text(0);
        Set_money_text(0);
        Set_wave_text(0);
        Set_Enemy_text(0);
        Set_Score_text(0);
    }


    // 各カウンターへのセッター(値をセット)とゲッター(カウンターのobjそのものをゲット)
    public Text Kill_count() { return this.kill_count; }
    public Text Money_count() { return this.money_count; }
    public Text Wave_count() { return this.wave_count; }
    public Text Score_count() { return this.score_count; }
    public Text Enemy_count() { return this.enemy_count; }

    public void Set_kill_text(int kill_count) 
    {
        this.kill_count.text = kill_count.ToString("D9");
    }

    public void Set_money_text(int money_count)
    {
        this.money_count.text = money_count.ToString("D9");
    }

    public void Set_wave_text(int wave_count) 
    {
        this.wave_count.text = wave_count.ToString();
    }
    public void Set_Score_text(int score_count)
    {
        this.score_count.text = score_count.ToString("D9");
    }
    public void Set_Enemy_text(int enemy_count)
    {
        this.enemy_count.text = enemy_count.ToString();
    }
}

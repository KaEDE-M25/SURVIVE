using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//--====================================================--
//--                   計測用タイマー                   --
//--====================================================--
public class Timer : MonoBehaviour
{
    [SerializeField, Tooltip("開始時からの時間")]
    float total_time;
    // 外部から取り出す用の時間値
    public int Current_time { get; private set; }

    public int Max_count { get; private set; }
    // カウントダウンかカウントアップか
    int down;
    // カウント中か
    public bool Is_count { get; private set; }

    //##====================================================##
    //##                 Update カウント処理                ##
    //##====================================================##
    void Update()
    {
        if (Is_count)
        {
            total_time += Time.deltaTime * this.down;
            
            switch (down) 
            {
                // カウントダウンの場合
                case -1:
                    {
                        Current_time = Mathf.CeilToInt(total_time);
                        if (total_time <= 0)
                        {
                            Current_time = 0;
                            Is_count = false;
                        }
                        break;
                    }
                // カウントアップの場合
                case 1:
                    {
                        Current_time = Mathf.FloorToInt(total_time);
                        if (total_time >= Max_count)
                        {
                            Current_time = 0;
                            Is_count = false;
                        }
                        break; 
                    }
            }
        }
    }

    //##====================================================##
    //##       時間を指定してタイマーをスタートさせる       ##
    //##====================================================##
    public void TimerStart(int time,bool down = true) 
    {
        total_time = (down) ? time: 0;
        this.down = (down) ? -1 : 1;
        Is_count = true;
        Max_count = time;
    }
}

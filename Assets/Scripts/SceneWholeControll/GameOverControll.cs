using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//--====================================================--
//--           シーンGameOverの管理を統括する           --
//--====================================================--
public class GameOverControll : MonoBehaviour
{
    static readonly string RETRY_CONFIRM_TEXT = "同じ設定でもう一度ゲームを始めます。よろしいですか？";

    [Header("結果表示エフェクト１段目で使うもの")]

    [SerializeField,Tooltip("GAMEOVER..　のテキストを表示するobj")]
    GameObject gameover_title;
    [SerializeField,Tooltip("GAMECLEAR!　のテキストを表示するobj")]
    GameObject gameclear_title;

    [Header("結果表示エフェクト２段目で使うもの")]

    [SerializeField,Tooltip("数値系のスコアを示す文字列を表示するエフェクトの親obj（スコア、キル、ウェーブ）")]
    GameObject result_ef;

    [Header("結果表示エフェクト３段目で使うもの")]

    [SerializeField,Tooltip("数値系のスコアの実際の値を表示するエフェクトの親obj")]
    GameObject results;
    [SerializeField,Tooltip("スコア実際値のtextの下に引いている下線")]
    GameObject results_line;
    [SerializeField,Tooltip("選択肢obj")]
    GameObject choices;
    [SerializeField,Tooltip("ゲーム設定を表示する親obj")]
    GameObject settings;

    [Header("その他")]

    [SerializeField,Tooltip("選択肢のカーソル")]
    GameObject cursor;
    [SerializeField,Tooltip("メニュー")]
    GameObject[] menu_objs = new GameObject[3];
    [SerializeField,Tooltip("シーン遷移アニメーションのコンポーネント。")]
    MoveSceneAnimation mcanim;
    
    // ゲームクリアかゲームオーバーかどちらか
    bool is_clear = true;

    // 結果表示エフェクトの進行度
    int proceed = 0;

    // メニューで選択中の選択肢
    int choose = -1;
    Tween tween;

    // メインゲームシーンから受け取ったゲームデータ
    PlayData recieved_data;
    // 初期化用ゲームデータ
    PlayData initialize_data;

    //##====================================================##
    //##                     Start 初期化                   ##
    //##====================================================##
    private void Start()
    {
        tween = cursor.transform.DOLocalMoveX(cursor.transform.localPosition.x + 3f, 0.5f).SetEase(Ease.OutCirc).SetLoops(-1, LoopType.Yoyo);
        // 結果表示エフェクトを実行
        Invoke(nameof(EffectProceed), 1f);
    }

    //##====================================================##
    //##                 Update メニュー選択                ##
    //##====================================================##
    void Update()
    {
        // シーン遷移アニメーション中でなければ操作できる
        if (mcanim.fading <= 0)
        {
            // カーソルを上に移動
            if (InputControll.GetInputDown(InputControll.INPUT_ID_UPARROW))
            {
                AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_PLAYER, AudioFilePositions.UI.CURSOR_MOVE);
                if (choose <= 0)
                    choose = menu_objs.Length - 1;
                else
                    choose -= 1;
                Renew_Board();
            }
            // カーソルを下に移動
            if (InputControll.GetInputDown(InputControll.INPUT_ID_DOWNARROW))
            {
                AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_PLAYER, AudioFilePositions.UI.CURSOR_MOVE);
                if (choose >= menu_objs.Length - 1)
                    choose = 0;
                else
                    choose += 1;
                Renew_Board();
            }

            // 決定キーを押したら遷移
            if (InputControll.GetInputDown(InputControll.INPUT_ID_A))
            {
                switch (choose)
                {
                    case 0: // メニューに戻る
                        {
                            mcanim.MoveScene("Menu");
                            tween?.Kill();
                            break;
                        }
                    case 1: // 同じ設定でもう一度はじめる
                        {
                            StartCoroutine(PopUpWindowControll.CreateSmallWindow(
                            delegate () // はいを選択した処理
                            {
                                initialize_data = new PlayData(recieved_data.choose_chara_id, recieved_data.choose_stage_id, recieved_data.choose_mode_id, EigenValue.GetCharacterStatus(recieved_data.choose_chara_id).Max_HP, EigenValue.GetCharacterStatus(recieved_data.choose_chara_id).Max_MP, 0, 0, 0, 0, new ItemStocks());
                                SceneManager.sceneLoaded += Send_datas;

                                mcanim.MoveScene("MainGame");
                            },
                            delegate () // いいえを選択した処理
                            {
                                this.enabled = true;
                            },
                            this, new Vector2(180f, 120f), RETRY_CONFIRM_TEXT, transform.position + new Vector3(0f, 0f, 0f)));

                            return;
                        }
                    case 2: // シェアする
                        {

                            // まだ未実装
                            StartCoroutine(CautionWindowControll.CreateSmallWindow(this, new Vector2(200f, 120f), "この機能はまだ実装されていません！", Vector2.zero));
                            return;
                        }
                    default:
                        break;

                }
                AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_PLAYER, AudioFilePositions.UI.DECISION);
            }
        }
    }

    //##====================================================##
    //##プレイ結果を受け取りセットする(シーン遷移の際に使用)##
    //##====================================================##
    public void SetResult(PlayData datas,bool is_clear) 
    {
        results.transform.Find("Score").GetComponent<Text>().text = datas.score.ToString("D10");
        results.transform.Find("KillCount").GetComponent<Text>().text = datas.kill_count.ToString("D8");
        results.transform.Find("WaveCount").GetComponent<Text>().text = datas.clear_waves.ToString("D8");

        settings.transform.Find("Setting_battle_chara").GetComponent<SpriteRenderer>().sprite = EigenValue.GetCharacterIcon_Sprite(datas.choose_chara_id);
        settings.transform.Find("Setting_stage").GetComponent<SpriteRenderer>().sprite = EigenValue.GetStageIcon_Sprite(datas.choose_stage_id);
        settings.transform.Find("Setting_mode").GetComponent<SpriteRenderer>().sprite = EigenValue.GetModeIcon_Sprite(datas.choose_mode_id);

        this.is_clear = is_clear;

        // 同じ設定でもう一度プレイする用にプレイデータをまとめて受け取っておく
        recieved_data = datas;
    }

    //##====================================================##
    //##            結果表示エフェクトを進行する            ##
    //##====================================================##
    public void EffectProceed() 
    {
        switch (proceed) 
        {
            case 0: // タイトル
                {
                    if (is_clear)
                    {
                        gameclear_title.SetActive(true);
                        gameclear_title.transform.DOLocalMoveY(0f, 0.5f).OnComplete(() =>
                        {
                            Transform light = gameclear_title.transform.Find("Light");
                            light.DOLocalMoveX(light.localPosition.x * -1f, 1f).SetEase(Ease.InOutCirc).OnComplete(() => 
                            {
                                EffectProceed();
                            });
                        });
                    }
                    else
                    {
                        gameover_title.SetActive(true);
                        gameover_title.transform.DOLocalMoveY(0f, 2f).OnComplete(() => { EffectProceed(); });
                    }
                    break;
                }
            case 1: // スコアの装飾を出す
                {
                    result_ef.SetActive(true);
                    results_line.transform.DOScaleX(300f, 1f).SetEase(Ease.InOutCirc);
                    (is_clear ? gameclear_title : gameover_title).transform.DOLocalMoveY(70f, 1f).SetEase(Ease.InOutCirc).OnComplete(() => { EffectProceed(); });
                
                    break;
                }
            case 2: // 残り（実際のスコア数値と設定とメニュー）を出す
                {
                    results.SetActive(true);
                    choices.transform.DOLocalMoveX(-60, 1f).SetEase(Ease.OutSine);
                    settings.transform.DOLocalMoveX(90, 1f).SetEase(Ease.OutSine).OnComplete(()=> 
                    {
                        choose = 0;
                    
                    });
                    break;
                }
            default:
                break;
        }

        ++proceed;

    }

    //##====================================================##
    //##                メニューを更新する処理              ##
    //##====================================================##
    void Renew_Board()
    {
        cursor.transform.SetParent(menu_objs[choose].transform);
        Vector3 vec = cursor.transform.localPosition;
        vec.y = 0f;
        cursor.transform.localPosition = vec;
    }

    //##====================================================##
    //##       遷移先(メインゲーム画面)へデータを送る       ##
    //##====================================================##
    void Send_datas(Scene next, LoadSceneMode mode)
    {
        GameControll gameControll = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControll>();
        gameControll.Play_data = initialize_data;

        SceneManager.sceneLoaded -= Send_datas;
    }
}

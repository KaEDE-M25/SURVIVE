using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//--====================================================--
//--        シーンNewGameSettingの管理を統括する        --
//--====================================================--
public class GameSettingControll : MonoBehaviour
{
    const float EFFECT_TIME = 0.5f;

    [SerializeField,Tooltip("カーソルのobj")]
    GameObject mode_choose_cursor;

    [SerializeField,Tooltip("最終確認画面のカーソルのobj")]
    GameObject final_confirm_cursor;

    [SerializeField,Tooltip("メニュー全体を動かすための親オブジェクト")]
    GameObject menu_all;

    [Header("実装済みのキャラクターの情報")]

    [SerializeField,Tooltip("キャラクターの名前を表示するobj")]
    GameObject[] chara_list_names = new GameObject[EigenValue.IMPLEMENTED_CHARACTERS];
    [SerializeField,Tooltip("キャラクターのイメージ画像を表示するobj")]
    GameObject[] chara_play_images = new GameObject[EigenValue.IMPLEMENTED_CHARACTERS];
    [SerializeField,Tooltip("キャラクターの説明文を表示するobj")]
    GameObject[] chara_explain_texts = new GameObject[EigenValue.IMPLEMENTED_CHARACTERS];

    [Header("実装済みのステージの情報")]

    [SerializeField, Tooltip("ステージの名前を表示するobj")]
    GameObject[] stage_list_names = new GameObject[EigenValue.IMPLEMENTED_STAGES];
    [SerializeField, Tooltip("ステージのイメージ画像を表示するobj")]
    GameObject[] stage_play_images = new GameObject[EigenValue.IMPLEMENTED_STAGES];
    [SerializeField, Tooltip("ステージの説明文を表示するobj")]
    Text stage_explain_text;

    [Header("実装済みのモードの情報")]

    [SerializeField,Tooltip("モードの名前を表示するobj")]
    GameObject[] mode_list = new GameObject[3];

    [SerializeField,Tooltip("モードの説明文を表示するobj")]
    Text mode_explain_obj;

    [SerializeField,Tooltip("モードの説明文")]
    string[] mode_explains = new string[3];

    [Header("最終確認画面で用いるobj")]

    [SerializeField,Tooltip("はいorいいえ のobj")]
    GameObject[] final_confirm_list = new GameObject[2];
    [SerializeField,Tooltip("最終確認画面の設定を表示するGameObj")]
    Text[] final_confirm_setobjs = new Text[3];

    [SerializeField,Tooltip("設定したキャラクターを表示するobj")]
    SpriteRenderer chara_icon;
    [SerializeField, Tooltip("設定したステージを表示するobj(ステージ選択画面と最終確認画面)")]
    SpriteRenderer[] stage_icons = new SpriteRenderer[2];
    [SerializeField, Tooltip("設定したモードを表示するobj(モード選択画面と最終確認画面)")]
    SpriteRenderer[] mode_icons = new SpriteRenderer[2];

    [Header("その他")]

    [SerializeField, Tooltip("装飾用カーソル (上下左右の順番)")]
    GameObject[] decorate_cursor = new GameObject[6];

    [SerializeField, Tooltip("シーン遷移アニメーションのコンポーネント。")]
    MoveSceneAnimation mcanim;

    // 選択した要素を保存しておく変数
    int choose_chara_id = 1;
    int choose_stage_id = 1;
    int choose_mode_id = 1;

    Tween tween;
    readonly Tween[] decorate_cursors_tweens = new Tween[6];

    // 画面の進行状況 (0 = キャラクター選択、1 = ステージ選択、2 = モード選択、3 = 最終決定)
    int proceed = 0;
    int choose = 0;
    int choose_max = 0;
    bool is_move = false;
    bool is_effect = false;

    // プレイデータ(転送用)
    PlayData initialize_data = null;

    //##====================================================##
    //##                     Start 初期化                   ##
    //##====================================================##
    void Start()
    {
        choose_max = EigenValue.IMPLEMENTED_CHARACTERS - 1;

        decorate_cursors_tweens[0] = decorate_cursor[0].transform.DOLocalMoveY(50f, 0.5f).SetEase(Ease.OutCirc).SetLoops(-1, LoopType.Yoyo);
        decorate_cursors_tweens[1] = decorate_cursor[1].transform.DOLocalMoveY(-50f, 0.5f).SetEase(Ease.OutCirc).SetLoops(-1, LoopType.Yoyo);
        decorate_cursors_tweens[2] = decorate_cursor[2].transform.DOLocalMoveX(-70f, 0.5f).SetEase(Ease.OutCirc).SetLoops(-1, LoopType.Yoyo);
        decorate_cursors_tweens[3] = decorate_cursor[3].transform.DOLocalMoveX(70f, 0.5f).SetEase(Ease.OutCirc).SetLoops(-1, LoopType.Yoyo);
        decorate_cursors_tweens[4] = decorate_cursor[4].transform.DOLocalMoveX(-85f, 0.5f).SetEase(Ease.OutCirc).SetLoops(-1, LoopType.Yoyo);
        decorate_cursors_tweens[5] = decorate_cursor[5].transform.DOLocalMoveX(-27f, 0.5f).SetEase(Ease.OutCirc).SetLoops(-1, LoopType.Yoyo);
    }


    //##====================================================##
    //##                 Update メニュー選択                ##
    //##====================================================##
    void Update()
    {
        // シーン遷移アニメーション中でなければ操作できる
        if (mcanim.fading <= 0)
        {
            // 画面遷移中もしくはカーソル移動エフェクトが発生中でなければ
            if (!is_move && !is_effect)
            {
                // カーソルを上に移動 (ステージ選択画面のみ右キーで移動させる)
                if (InputControll.GetInputDown(proceed == 1 ? InputControll.INPUT_ID_RIGHTARROW : InputControll.INPUT_ID_UPARROW))
                {
                    AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_UI, AudioFilePositions.UI.CURSOR_MOVE);
                    int prev_choose = choose;
                    if (choose <= 0)
                        choose = choose_max;
                    else
                        choose -= 1;
                    Renew_Board(prev_choose, false);
                }
                // カーソルを下に移動 (ステージ選択画面のみ左キーで移動させる)
                else if (InputControll.GetInputDown(proceed == 1 ? InputControll.INPUT_ID_LEFTARROW : InputControll.INPUT_ID_DOWNARROW))
                {
                    AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_UI, AudioFilePositions.UI.CURSOR_MOVE);
                    int prev_choose = choose;
                    if (choose >= choose_max)
                        choose = 0;
                    else
                        choose += 1;
                    Renew_Board(prev_choose, true);
                }

                // 決定キーを押したら遷移
                else if (InputControll.GetInputDown(InputControll.INPUT_ID_A))
                {
                    //最終決定画面以外については
                    if (proceed < 3)
                    {
                        switch (proceed)
                        {
                            case 0: // キャラ選択なら
                                {
                                    if (choose == 1)
                                    {
                                        StartCoroutine(CautionWindowControll.CreateSmallWindow(this, new Vector2(200f, 120f), "そのキャラクターはまだ実装されていません！", Vector2.zero));
                                        return;
                                    }

                                    // 選択したキャラを保存
                                    choose_chara_id = choose + 1;
                                    choose_max = EigenValue.IMPLEMENTED_STAGES - 1;
                                    choose = choose_stage_id - 1;

                                    // アイコンを反映
                                    chara_icon.sprite = chara_list_names[choose_chara_id - 1].GetComponent<SpriteRenderer>().sprite;

                                    break;
                                }
                            case 1: // ステージ選択なら
                                {
                                    if (choose == 1)
                                    {
                                        StartCoroutine(CautionWindowControll.CreateSmallWindow(this, new Vector2(200f, 120f), "そのステージはまだ実装されていません！", Vector2.zero));
                                        return;
                                    }
                                    // 選択したステージを保存
                                    choose_stage_id = choose + 1;
                                    choose = choose_mode_id - 1;
                                    choose_max = 3 - 1;

                                    break;
                                }
                            case 2: // モード選択なら
                                {
                                    // 選択したモードを保存
                                    choose_mode_id = choose;

                                    final_confirm_setobjs[0].text = EigenValue.GetCharacterStatus(choose_chara_id).Name;
                                    final_confirm_setobjs[1].text = EigenValue.GetStageData(choose_stage_id).stage_name;
                                    final_confirm_setobjs[2].text = mode_list[choose_mode_id].GetComponent<Text>().text;

                                    // カーソル位置を初期化
                                    choose = 0;
                                    final_confirm_cursor.transform.SetParent(final_confirm_list[0].transform);
                                    Vector3 vec = final_confirm_cursor.transform.localPosition;
                                    vec.y = 0f;
                                    final_confirm_cursor.transform.localPosition = vec;

                                    choose_max = 2 - 1;
                                    break;
                                }
                            default:
                                break;
                        }

                        AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_UI, AudioFilePositions.UI.DECISION);

                        proceed++;
                        is_move = true;
                        menu_all.transform.DOMoveX(proceed * -360f, 1f).SetEase(Ease.OutCirc).OnComplete(() =>
                        {
                            is_move = false;
                        });
                    }
                    else //最終決定画面では 
                    {
                        switch (choose)
                        {
                            case 0: // はい
                                {
                                    // メインゲーム画面へ
                                    GameStart();
                                    break;
                                }
                            case 1: // いいえ
                                {
                                    AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_UI, AudioFilePositions.UI.CANCEL);
                                    Cancel();
                                    break;
                                }
                        }
                    }
                }
                // キャンセルキーが押されたら一つ前の画面へ戻る
                else if (InputControll.GetInputDown(InputControll.INPUT_ID_B))
                {
                    AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_UI, AudioFilePositions.UI.CANCEL);
                    Cancel();
                }
            }
        }
    }

    //##====================================================##
    //##                メニューを更新する処理              ##
    //##====================================================##
    void Renew_Board(int prev_choose ,bool up)
    {
        is_effect = true;

        // 現在の画面に応じて動作
        switch (proceed) 
        {
            case 0:  // キャラ選択画面
                {
                    // アニメーションさせつつ表示変更

                        // キャラネーム

                    // 選択更新前のやつ
                    chara_list_names[prev_choose].GetComponent<Text>().DOColor(Color.clear, EFFECT_TIME).SetEase(Ease.OutCirc);
                    chara_list_names[prev_choose].transform.DOLocalMoveY(20f * (up ? -1f : 1f), EFFECT_TIME).SetEase(Ease.OutCirc).OnComplete(()=> 
                    {
                        chara_list_names[prev_choose].SetActive(false);
                    });
                    // 選択更新後のやつ
                    chara_list_names[choose].transform.localPosition = (up ? Vector3.up : Vector3.down) * 20f;
                    Text text = chara_list_names[choose].GetComponent<Text>();
                    text.color = Color.clear;
                    chara_list_names[choose].SetActive(true);
                    text.DOColor(Color.black, EFFECT_TIME).SetEase(Ease.OutCirc);
                    chara_list_names[choose].transform.DOLocalMoveY(0f, EFFECT_TIME).SetEase(Ease.OutCirc).OnComplete(() =>
                    {
                        is_effect = false;
                    });

                        // イメージ画像

                    // 選択更新前のやつ
                    chara_play_images[prev_choose].GetComponent<CanvasGroup>().DOFade(0f, EFFECT_TIME / 2f).SetEase(Ease.InSine).OnComplete(() =>
                    {
                        chara_list_names[prev_choose].SetActive(false);
                        // 選択更新後のやつ
                        chara_play_images[choose].SetActive(true);
                        chara_play_images[choose].GetComponent<CanvasGroup>().DOFade(1f, EFFECT_TIME / 2f).SetEase(Ease.OutSine);

                        // テキストもここで処理
                        tween?.Complete();
                        tween?.Kill();
                        chara_explain_texts[prev_choose].SetActive(false);
                        chara_explain_texts[choose].SetActive(true);
                        Text text_comp = chara_explain_texts[choose].GetComponent<Text>();
                        string text = text_comp.text;
                        text_comp.text = "";
                        tween = chara_explain_texts[choose].GetComponent<Text>().DOText(text, 0.01f * text.Length).SetEase(Ease.Linear);
                    });
                    break;
                }
            case 1:  // ステージ選択画面
                {
                    // アニメーションさせつつ表示変更

                        // ステージネーム

                    // 選択更新前のやつ
                    stage_list_names[prev_choose].GetComponent<Text>().DOColor(Color.clear, EFFECT_TIME).SetEase(Ease.OutCirc);
                    stage_list_names[prev_choose].transform.DOLocalMoveX(60f * (up ? 1f : -1f), EFFECT_TIME).SetEase(Ease.OutCirc).OnComplete(() =>
                    {
                        stage_list_names[prev_choose].SetActive(false);

                    });
                    // 選択更新後のやつ
                    stage_list_names[choose].transform.localPosition = (up ? Vector3.left : Vector3.right) * 60f;
                    Text text = stage_list_names[choose].GetComponent<Text>();
                    text.color = Color.clear;
                    stage_list_names[choose].SetActive(true);
                    text.DOColor(Color.black, EFFECT_TIME).SetEase(Ease.OutCirc);
                    stage_list_names[choose].transform.DOLocalMoveX(0f, EFFECT_TIME).SetEase(Ease.OutCirc).OnComplete(() =>
                    {
                        is_effect = false;
                    });

                        // イメージ画像

                    // 選択更新前のやつ
                    stage_play_images[prev_choose].GetComponent<CanvasGroup>().DOFade(0f, EFFECT_TIME / 2f).SetEase(Ease.InSine).OnComplete(() =>
                    {
                        stage_list_names[prev_choose].SetActive(false);
                        // 選択更新後のやつ
                        stage_play_images[choose].SetActive(true);
                        stage_play_images[choose].GetComponent<CanvasGroup>().DOFade(1f, EFFECT_TIME / 2f).SetEase(Ease.OutSine);

                        // テキストもここで処理
                        tween?.Complete();
                        tween?.Kill();
                        Text text_comp = stage_explain_text.GetComponent<Text>();
                        string next_stage_comment = EigenValue.GetStageData(choose + 1).stage_comment;
                        text_comp.text = "";
                        tween = stage_explain_text.DOText(next_stage_comment, 0.01f * next_stage_comment.Length).SetEase(Ease.Linear);

                        // アイコン
                        Sprite new_sprite = EigenValue.GetStageIcon_Sprite(choose + 1);

                        foreach (SpriteRenderer stage_icon in stage_icons)
                        {
                            stage_icon.sprite = new_sprite;
                        }
                    });
                    break;
                }
            case 2: // モード選択画面
                {

                    // カーソルの位置を変更
                    mode_choose_cursor.transform.SetParent(mode_list[choose].transform);
                    Vector3 vec = mode_choose_cursor.transform.localPosition;
                    vec.y = 0f;
                    mode_choose_cursor.transform.localPosition = vec;
                    mode_explain_obj.text = "";
                    
                    // テキストを書き直す
                    tween?.Kill();
                    tween = mode_explain_obj.DOText(mode_explains[choose], 0.01f * mode_explains[choose].Length).SetEase(Ease.Linear);

                    // アイコン
                    Sprite new_sprite = EigenValue.GetModeIcon_Sprite(choose);

                    foreach (SpriteRenderer mode_icon in mode_icons)
                    {
                        mode_icon.sprite = new_sprite;
                    }

                    is_effect = false;
                    break;
                }
            case 3:
                {

                    // カーソルの位置を変更
                    final_confirm_cursor.transform.SetParent(final_confirm_list[choose].transform);
                    Vector3 vec = final_confirm_cursor.transform.localPosition;
                    vec.y = 0f;
                    final_confirm_cursor.transform.localPosition = vec;

                    is_effect = false;
                    break;
                }

            default:
                break;
        }
    }

    //##====================================================##
    //##                一つ前の画面へ戻る                  ##
    //##====================================================##
    void Cancel() 
    {
        if (proceed <= 0)
        {
            // メインメニューに戻る
            for (int i = 0; i < decorate_cursors_tweens.Length; i++)
                decorate_cursors_tweens[i]?.Kill();

            mcanim.MoveScene("Menu");
            return;
        }

        proceed--;
        switch (proceed)
        {
            case 0: // キャラ選択なら
                {
                    choose_stage_id = choose + 1;
                    choose_max = EigenValue.IMPLEMENTED_CHARACTERS - 1;
                    choose = choose_chara_id - 1;
                    break;
                }
            case 1: // ステージ選択なら
                {
                    choose_mode_id = choose + 1;
                    choose_max = EigenValue.IMPLEMENTED_STAGES - 1;
                    choose = choose_stage_id - 1;
                    break;
                }
            case 2: // モード選択なら
                {
                    choose_max = 3 - 1;
                    choose = choose_mode_id;
                    break;
                }
            default:
                break;
        }

        is_move = true;
        menu_all.transform.DOMoveX(proceed * -360f, 1f).SetEase(Ease.OutCirc).OnComplete(() =>
        {
            is_move = false;
        });
    }

    //##====================================================##
    //##                ゲームを開始する処理                ##
    //##====================================================##
    void GameStart()
    {
        AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_UI, AudioFilePositions.UI.DECISION);
        for (int i = 0; i < decorate_cursors_tweens.Length; i++)
            decorate_cursors_tweens[i]?.Kill();

        initialize_data = new PlayData(choose_chara_id, choose_stage_id, choose_mode_id, EigenValue.GetCharacterStatus(choose_chara_id).Max_HP, EigenValue.GetCharacterStatus(choose_chara_id).Max_MP, 0, 0, 0, 0, new ItemStocks());
        SceneManager.sceneLoaded += Send_datas;

        mcanim.MoveScene("MainGame");
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
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;

//--====================================================--
//--         シーンSaveLoadGameの管理を統括する         --
//--====================================================--
public class SaveLoadControll : MonoBehaviour
{
    // 保存データの種類名(色)
    static readonly string[] DATAS_NAME = new string[10] { "red", "orange", "yellow", "green", "glue", "indigo", "purple", "white", "grey", "black" };
    // タイトルのテキスト
    static readonly string[] TITLE_TEXTS = new string[2] { "ＬＯＡＤ　ＧＡＭＥ", "ＳＡＶＥ　ＧＡＭＥ" };
    // 保存確認テキスト
    const string SAVE_CONFIRM_TEXT = "このカラーにセーブします。本当によろしいですか？";
    // 保存せずにメニューに戻るテキスト
    const string NOTSAVE_CONFIRM_TEXT = "セーブせずにメニューに戻ります。\n本当によろしいですか？";
    // ロード確認テキスト
    const string LOAD_CONFIRM_TEXT = "ロードしたカラーは空になります。\n本当によろしいですか？";

    // 保存されているデータのリスト
    readonly PlayData[] saved_datas = new PlayData[10];

    [SerializeField, Header("テスト環境でプレイする際はチェック"), Tooltip("チェックを入れると、ロードセーブデータとして仮データを用いる。エディタでこのシーンから再生する時にチェックを入れたし。")]
    bool test_mode = false;

    [Header("基盤の装飾")]

    [SerializeField,Tooltip("メニューの選択肢（セーブロードするカラー）のobj")]
    GameObject[] menu_objs = new GameObject[10];
    [SerializeField,Tooltip("メニューカーソル")]
    GameObject cursor;
    [SerializeField,Tooltip("タイトルのテキストobj")]
    Text title_text;

    [SerializeField,Tooltip("データがありません！　を表示するobj")]
    GameObject not_data_parent;
    // データがある
    [SerializeField,Tooltip("既に存在するデータを表示する親obj")]
    GameObject exist_data_parent;

    [SerializeField,Tooltip("セーブロードするデータをプレビューとして表示する親obj")]
    GameObject save_data_preview_parent;

    [Header("既存データのプレビューに表示される項目")]

    [SerializeField,Tooltip("HPバー　（のfill部分)")]
    Image hp_bar_obj;
    [SerializeField,Tooltip("MPバー 　(のfill部分)")]
    Image mp_bar_obj;
    [SerializeField,Tooltip("アイテムストック")]
    GameObject item_stock;
    [SerializeField,Tooltip("各種カウンタ、お金、キル、スコア")]
    Text[] counters;
    [SerializeField,Tooltip("ウェーブ数の最大値（の親obj）")]
    GameObject wave_max_parent;

    [Header("セーブロードデータのプレビューに表示される項目")]

    [SerializeField, Tooltip("HPバー　（のfill部分)")]
    Image new_hp_bar_obj;
    [SerializeField, Tooltip("MPバー 　(のfill部分)")]
    Image new_mp_bar_obj;
    [SerializeField, Tooltip("アイテムストック")]
    GameObject new_item_stock;
    [SerializeField, Tooltip("各種カウンタ、お金、キル、スコア")]
    Text[] new_counters;
    [SerializeField, Tooltip("ウェーブ数の最大値（の親obj）")]
    GameObject new_wave_max_parent;

    [Header("その他")]

    [SerializeField,Tooltip("動かし方を説明するメッセージ群の親obj")]
    GameObject controll_explainer;

    [SerializeField, Tooltip("シーン遷移アニメーションのコンポーネント")]
    MoveSceneAnimation mcanim;

    // セーブするかロードするか trueならセーブする、falseならロードする
    bool Is_save_action { get; set; } = false;

    // 前シーンから受け取ったプレイデータ
    PlayData Given_playdata { get; set; } = null;

    int choose = 0;
    bool is_effect = false;

    //##====================================================##
    //##          ファイルをシリアライズして保存            ##
    //##====================================================##
    public static void Seriarize<T>(string path,T obj) 
    {
        using (FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write)) 
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();

            binaryFormatter.Serialize(fileStream, obj);
        }
    }

    //##====================================================##
    //##        ファイルをデシリアライズして読み込み        ##
    //##====================================================##
    public static T Deserialize<T>(string path)
    {
        T obj;
        try
        {
            using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                obj = (T)binaryFormatter.Deserialize(fileStream);

            }
        }catch (FileNotFoundException) 
        {
            return default;
        }
        return obj;
    }

    //##====================================================##
    //##                     Awake 初期化                   ##
    //##====================================================##
    private void Awake()
    {
        if (test_mode)
        {
            var stocks = new ItemStocks();
            stocks.item_stocks[0].item_id = 1;
            stocks.item_stocks[0].num = 8;
            stocks.item_stocks[1].item_id = 3;
            stocks.item_stocks[1].num = 5;
            stocks.item_stocks[2].item_id = 2;
            stocks.item_stocks[2].num = 8;

            Initialize(true, new PlayData(1, 1, 2, 20, 10, 1957291, 2157192, 52191, 50, stocks)/*, null*/);
        }

        // セーブデータをSaveDatasフォルダから呼び出してリストを生成(ファイルが無い色はnullにする)
        for (int i=0;i<saved_datas.Length;i++)
        {
            saved_datas[i] = Deserialize<PlayData>(EigenValue.GetSaveDataPath()+DATAS_NAME[i]);
            if (saved_datas[i] == default(PlayData))
                saved_datas[i] = null;

            // データが空のときは対応の色をくすんだ色にする
            if(saved_datas[i] == null) 
            {
                SpriteRenderer spriteRenderer = menu_objs[i].transform.Find("background").GetComponent<SpriteRenderer>();
                spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0.1f);
            }
        }

        // 使用しているコントローラーに合うボタンアイコンを表示させる
        controll_explainer.transform.Find("Determine/icon-" + OptionData.current_options.controller).gameObject.SetActive(true);
        controll_explainer.transform.Find("BackTitle/icon-" + OptionData.current_options.controller).gameObject.SetActive(true);
        controll_explainer.transform.Find("Choose/icon-" + OptionData.current_options.controller).gameObject.SetActive(true);

        RenewPreviewBoard();
    }

    //##====================================================##
    //##              Update セーブデータ選択               ##
    //##====================================================##
    void Update()
    {
        // シーン遷移アニメーション中でなければ操作できる
        // カーソル移動処理中でなければ
        if (mcanim.fading <= 0 && !is_effect)
        {
            int prev_choose = choose;
            // カーソルを上に移動
            if (InputControll.GetInputDown(InputControll.INPUT_ID_UPARROW))
            {
                AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_PLAYER, AudioFilePositions.UI.CURSOR_MOVE);
                if (choose <= 0)
                    choose = menu_objs.Length - 1;
                else
                    choose -= 1;
                Renew_Board(Vector2.up,prev_choose);
            }
            // カーソルを下に移動
            if (InputControll.GetInputDown(InputControll.INPUT_ID_DOWNARROW))
            {
                AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_PLAYER, AudioFilePositions.UI.CURSOR_MOVE);
                if (choose >= menu_objs.Length - 1)
                    choose = 0;
                else
                    choose += 1;
                Renew_Board(Vector2.down, prev_choose);
            }

            // セーブする・ロードする
            if (InputControll.GetInputDown(InputControll.INPUT_ID_A))
            {
                // セーブをするなら
                if (Is_save_action)
                {
                    this.enabled = false;
                    SetInformation_for_preview(save_data_preview_parent, Given_playdata);
                    save_data_preview_parent.transform.DOLocalMoveY(0f, 1f).SetEase(Ease.OutCirc);
                    (saved_datas[choose] != null ? exist_data_parent : not_data_parent).transform.DOLocalMoveY(-240f, 1f).SetEase(Ease.OutCirc);
                    {

                        StartCoroutine(PopUpWindowControll.CreateSmallWindow(
                            delegate () // はいを選択した処理はセーブ
                            {
                                SaveLoadControll.Seriarize(EigenValue.GetSaveDataPath() + DATAS_NAME[choose], Given_playdata);

                                SpriteRenderer spriteRenderer = menu_objs[choose].transform.Find("background").GetComponent<SpriteRenderer>();

                                save_data_preview_parent.transform.Find("transparent_ground/light").transform.DOLocalMoveZ(0f, 1f).OnComplete(() =>
                                {
                                    spriteRenderer.DOColor(new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f), 1f).SetEase(Ease.OutCirc);
                                    save_data_preview_parent.transform.Find("transparent_ground/light").transform.DOLocalMoveX(0.85f, 1f).SetEase(Ease.InOutCirc).OnComplete(() =>
                                    {
                                        mcanim.MoveScene("Menu");
                                    });
                                });
                            },
                            delegate () // いいえを選択したら戻る
                            {
                                save_data_preview_parent.transform.DOLocalMoveY(240f, 1f).SetEase(Ease.OutCirc);
                                (saved_datas[choose] != null ? exist_data_parent : not_data_parent).transform.DOLocalMoveY(0f, 1f).SetEase(Ease.OutCirc).OnComplete(() =>
                                {

                                    this.enabled = true;

                                });

                            },
                            this, new Vector2(180f, 120f), SAVE_CONFIRM_TEXT, transform.position + new Vector3(-90f, 0f, 0f)));
                    }

                }
                // ロードをする、該当データが空じゃなければ
                else if (saved_datas[choose] != null)
                {
                    StartCoroutine(PopUpWindowControll.CreateSmallWindow(
                        delegate () // はいを選択した処理
                        {

                            // 該当のセーブデータを削除
                            if (File.Exists(EigenValue.GetSaveDataPath() + DATAS_NAME[choose]))
                            {
                                try
                                {
                                    File.Delete(EigenValue.GetSaveDataPath() + DATAS_NAME[choose]);
                                }
                                catch (Exception e)
                                {
                                    Debug.LogException(e);
                                }
                            }
                            // 選択しているカラーをくすんだ色にする

                            // ロードエフェクトを出してメインゲームへ遷移
                            SpriteRenderer spriteRenderer = menu_objs[choose].transform.Find("background").GetComponent<SpriteRenderer>();
                            spriteRenderer.DOColor(new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0.1f), 1f).SetEase(Ease.OutCirc);
                            exist_data_parent.transform.Find("transparent_ground/light").transform.DOLocalMoveX(0.85f, 1f).SetEase(Ease.InOutCirc).OnComplete(() =>
                            {
                                exist_data_parent.transform.DOLocalMoveY(240f, 1f).SetEase(Ease.InOutCirc).OnComplete(() =>
                                {
                                    transform.parent = null;
                                    this.gameObject.SetActive(false);
                                    DontDestroyOnLoad(this.gameObject);
                                    SceneManager.sceneLoaded += Send_datas;
                                    mcanim.MoveScene("MainGame");
                                });
                            });

                        },
                        delegate () // いいえを選択した処理
                        {
                            // 選択に戻る
                            save_data_preview_parent.transform.DOLocalMoveY(240f, 1f).SetEase(Ease.OutCirc).OnComplete(() =>
                            {
                                this.enabled = true;
                            });
                        },
                        this, new Vector2(180f, 120f), LOAD_CONFIRM_TEXT, transform.position + new Vector3(-90f, 0f, 0f)));
                }
            }

            // セーブをキャンセルしてメニューに戻る
            if (InputControll.GetInputDown(InputControll.INPUT_ID_B))
            {
                if (Is_save_action)
                    StartCoroutine(PopUpWindowControll.CreateSmallWindow(
                        delegate () // はいを選択した処理
                        {
                            mcanim.MoveScene("Menu");
                        },
                        delegate () // いいえを選択した処理
                        {
                            this.enabled = true;
                        },
                        this, new Vector2(180f, 120f), NOTSAVE_CONFIRM_TEXT, transform.position + new Vector3(-90f, 0f, 0f)));

                else
                {
                    AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_PLAYER, AudioFilePositions.UI.CANCEL);
                    mcanim.MoveScene("Menu");
                }
            }
        }
    }

    //##====================================================##
    //##                メニューを更新する処理              ##
    //##====================================================##
    void Renew_Board(Vector2 direction,int prev_choose)
    {
        is_effect = true;


        // セーブデータポップの位置移動
        for(int i=0;i<menu_objs.Length;i++)
        {
            // 最も遠く離れた色については最上部最下部へ移動
            if (Mathf.Abs(i - prev_choose) == 5)
            {
                Vector3 vec = Vector3.zero;
                vec.y = 140f * direction.y;

                menu_objs[i].transform.localPosition = vec;
            }
            else // 残りはアニメーション
                menu_objs[i].transform.DOLocalMoveY(menu_objs[i].transform.localPosition.y + 35f * direction.y * -1f, 0.25f).SetEase(Ease.OutCirc).OnComplete(() =>
                {
                    is_effect = false;
                });
        }

        RenewPreviewBoard();
    }

    //##====================================================##
    //##    現在のセーブデータのプレビューを更新する処理    ##
    //##====================================================##
    void RenewPreviewBoard() 
    {
        not_data_parent.SetActive(saved_datas[choose] == null);
        exist_data_parent.SetActive(saved_datas[choose] != null);

        if (saved_datas[choose] != null)
            SetInformation_for_preview(exist_data_parent, saved_datas[choose]);
    }

    //##====================================================##
    //##     遷移時に呼び出し、プレイデータをセットする     ##
    //##====================================================##
    public void Initialize(bool is_save,PlayData play_data) 
    {
        this.Is_save_action = is_save;
        this.Given_playdata = play_data;

        title_text.text = is_save ? TITLE_TEXTS[1] : TITLE_TEXTS[0];
    }

    //##====================================================##
    //##         プレビューに情報を表示させる処理           ##
    //##親オブジェクトを受け取り、子を探索して値をセットする##
    //##====================================================##
    void SetInformation_for_preview(GameObject parent_obj, PlayData data) 
    {
        // プレイヤーアイコン
        parent_obj.transform.Find("Chara_icon").GetComponent<Image>().sprite = EigenValue.GetCharacterIcon_Sprite(data.choose_chara_id);
        // ステージアイコン
        parent_obj.transform.Find("Stage_icon").GetComponent<Image>().sprite = EigenValue.GetStageIcon_Sprite(data.choose_stage_id);
        // モードアイコン
        parent_obj.transform.Find("Mode_icon").GetComponent<Image>().sprite = EigenValue.GetModeIcon_Sprite(data.choose_mode_id);

        // HPバー
        int max_hp = EigenValue.GetCharacterStatus(data.choose_chara_id).Max_HP;
        // MPバー
        int max_mp = EigenValue.GetCharacterStatus(data.choose_chara_id).Max_MP;

        // 最大HP・MPをアイテムストックから決定
        foreach (ItemStock itemstock in data.item_stocks.item_stocks) 
        {
            if (itemstock.item_id == EigenValue.ITEM_HP_AMULET.item_id) // 体力祈願のおまもりを持っていたら
                max_hp += 2 * itemstock.num;
            else if (itemstock.item_id == EigenValue.ITEM_MP_AMULET.item_id) // 魔力祈願のおまもりを持っていたら
                max_mp += 2 * itemstock.num;
        }

        // HPバー・MPバーを設定
        parent_obj.transform.Find("Hp_bar/active").GetComponent<Image>().fillAmount = (float)data.hp / (float)max_hp;
        parent_obj.transform.Find("Hp_bar/text").GetComponent<Text>().text = data.hp.ToString() +"/"+ max_hp.ToString();

        parent_obj.transform.Find("Mp_bar/active").GetComponent<Image>().fillAmount = (float)data.mp / (float)max_mp;
        parent_obj.transform.Find("Mp_bar/text").GetComponent<Text>().text = data.mp.ToString() + "/" + max_mp.ToString();

        // アイテムストック
        for (int i = 0; i < data.item_stocks.item_stocks.Length; i++)
        {
            var stock = parent_obj.transform.Find("Item_Stock/StockID_" + (i + 1));

            bool is_null = data.item_stocks.item_stocks[i].item_id == 0;

            // ストックの画像を設定
            stock.Find("icon").GetComponent<SpriteRenderer>().sprite =
                is_null ? null : Resources.Load<Transform>((EigenValue.PREFAB_DIRECTORY_ITEMS + EigenValue.GetItemData(data.item_stocks.item_stocks[i].item_id).prefab_name)).Find("graphic").GetComponent<SpriteRenderer>().sprite;

            // ストック数の画像を設定
            stock.Find("text").GetComponent<Text>().text =
                is_null ? "0" : data.item_stocks.item_stocks[i].num.ToString();
        }

        // 各種カウンター

        // キルカウント
        parent_obj.transform.Find("Kill_count/counter").GetComponent<Text>().text = data.kill_count.ToString("D9");
        // お金
        parent_obj.transform.Find("Money_count/counter").GetComponent<Text>().text = data.money.ToString("D9");
        // スコア
        parent_obj.transform.Find("Score_count/counter").GetComponent<Text>().text = data.score.ToString("D9");
        // ウェーブ数
        parent_obj.transform.Find("Wave_count/counter").GetComponent<Text>().text = data.clear_waves.ToString();

        // 最大ウェーブ数 (モードによって表示させるものを分岐)
        for(int i=0;i<EigenValue.IMPLEMENTED_MODES; i++) 
            parent_obj.transform.Find("Wave_count/MaxWave/mode_" + i).gameObject.SetActive(i == data.choose_mode_id);

        // 背景の色を選択しているカラーデータと同じにする
        SpriteRenderer renderer = menu_objs[choose].transform.Find("background").GetComponent<SpriteRenderer>();

        parent_obj.transform.Find("background").GetComponent<SpriteRenderer>().color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, 1f);
    }

    //##====================================================##
    //##       遷移先(メインゲーム画面)へデータを送る       ##
    //##====================================================##
    void Send_datas(Scene next, LoadSceneMode mode)
    {
        // 遷移先のコンポーネントにロードしたデータを埋め込む
        GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControll>().Play_data = saved_datas[choose];

        SceneManager.sceneLoaded -= Send_datas;
    }
}

using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//--====================================================--
//--            メインゲームを管理するクラス            --
//--====================================================--
public class GameControll : MonoBehaviour
{
    public static readonly string CONFIRM_SAVE_TEXT = "ゲームを保存しますか？";
    // フィールド上に存在できる敵キャラの数  (処理落ち防止)
    public static readonly int MAX_ENEMYS = 300;

    // テスト用
    [SerializeField ,Header("テスト環境でプレイする際はチェック"), Tooltip("チェックを入れると、適当なステータスを入れてゲームを開始する。エディタでこのシーンから再生する時にチェックを入れたし。")]
    bool test_env = false;

    // お金の所持数
    int money;
    // キルカウント
    int kill_count;
    // 現在のウェーブ数
    int wave_count;
    // スコア
    int score;

    // アイテムストック
    // 1次元目 -> アイテムID 、 添え字 -> ストック枠番号　、値が０の場合は持っていないとする
    // 2次元目 -> アイテムの所次数
    public ItemStocks Item_stocks { get; protected set; } = new ItemStocks();

    // アイテムストックの選択位置
    public int Item_stock_cursor { get; protected set; } = 0;
    // ポーズしているか
    bool is_pause = false;

    // 経過時間（スポーン周期の測定に使用）
    float time = 0f;

    // スポーンフラグ（周期が回るごとに１上がり、設定された数値に達するとスポーンし０に戻る
    readonly int[] spawn_flags = new int[EigenValue.IMPLEMENTED_ENEMYS];

    // スポーンフラグ設定値（固有値をもとにして±２くらいでランダムに決まる）
    readonly int[] spawn_flags_max = new int[EigenValue.IMPLEMENTED_ENEMYS];

    // ゲーム全体の状況を保持するための親オブジェクト
    [Header("ゲーム全体の状況を管理する親オブジェクト")]
    

    // ショップの入り口を示すobj
    [SerializeField,Tooltip("ショップの入り口を示すオブジェクトのShopEntrancePointクラス。")]
    ShopEntrancePoint shop_entrance_point;

    // 敵キャラのスポーンポイント
    [SerializeField,Tooltip("敵がスポーンする位置をまとめた親オブジェクト。")]
    GameObject enemy_spawn_point;
    // フィールドにいる敵キャラのリストになる親オブジェクト
    [SerializeField,Tooltip("フィールドに存在する敵キャラの一覧となる親オブジェクト。敵キャラはすべてこれの子オブジェクトにする。入っていないとポーズで停止しない。")]
    GameObject active_enemys_parent;
    // エフェクトのリストになる親オブジェクト
    [SerializeField,Tooltip("フィールドで再生されているエフェクトの一覧となる親オブジェクト。以下Enemy_spawn_pointと同様")]
    GameObject active_effects_parent;
    // フィールドに落ちてるアイテムのリストになる親オブジェクト
    [SerializeField,Tooltip("フィールドに存在するアイテム以下Enemy_spawn_pointと同様")]
    GameObject active_items_parent;
    // ウェーブ数を表示するテキスト
    [SerializeField,Tooltip("現在のウェーブ数を表示するTextコンポーネント。")]
    Text wave_count_text;
    // アイテムストックの操作を行う親オブジェクト
    [Tooltip("アイテムストック全体をまとめている親オブジェクト。全体を移動させることがある。")]
    public GameObject item_stock_parent;
    // アイテムストックカーソルのオブジェクト
    [Tooltip("アイテムストックのカーソルのImageコンポーネント。カーソルを動かしたりショップでの整理の時に使う。")]
    public Image item_stock_cursor_obj;

    [Header("演出用の設定物")]

    // 画面を暗くする処理をするためのオブジェクト
    [SerializeField, Tooltip("ポーズ時などに画面を暗くするためのオブジェクトのSpriteRendererコンポーネント。")]
    SpriteRenderer dimmer_spre;

    [SerializeField, Tooltip("要素0→Ready...、要素1→GO!!、の開始演出に使うオブジェクト")]
    GameObject[] ready_go_effect = new GameObject[2];

    // 移動用のアニメーションobj
    [SerializeField,Tooltip("シーン遷移アニメーションのコンポーネント。")]
    MoveSceneAnimation mcanim;

    // ウェーブクリアした時のテキストエフェクト
    [SerializeField, Tooltip("ウェーブクリアした時の文字")]
    Text wave_clear_effect;
    [SerializeField, Tooltip("コンティニュー画面のエフェクト全体をまとめた親オブジェクト。")]
    GameObject continue_effect_parent;
    [SerializeField, Tooltip("ポーズ画面のエフェクト全体をまとめた親オブジェクト。")]
    GameObject pause_effect_parent;

    // 各種UIエフェクト
    [SerializeField,Tooltip("お金を獲得したときのUIエフェクト(Prefab)。")]
    GameObject money_up_effect;
    [SerializeField,Tooltip("キルカウントが増えたときのUIエフェクト(Prefab)。")]
    GameObject kill_up_effect;

    // アイテムを捨てたときのエフェクト(prefab)
    [SerializeField,Tooltip("アイテムを捨てた際の捨てるエフェクトのPrefab。")]
    GameObject drop_item_effect_prefab;

    [Header("処理を分けてるコンポーネントとか")]

    // 各種コンポーネント
    [SerializeField,Tooltip("このコンポーネントが持ってるゲームスコア(スコア、お金など)をテキストに反映させる処理を担うクラス。")]
    CountControll count_controll;
    [SerializeField,Tooltip("ウェーブの進行度合(カウントダウンとUIゲージへの表示反映)を管理するクラス。")]
    WaveControll wave_controll;
    public CameraControll Camera_controll { get; protected set; }
    Timer wave_controll_timer;

    // プレイヤーのコンポーネント
    Fighters fighter_comp;

    // ゲームデータ
    public PlayData Play_data { get; set; } = null;

    //##====================================================##
    //##                   Awake 初期化１                   ##
    //##====================================================##
    private void Awake()
    {
        // コンポーネント初期化
        wave_controll_timer = wave_controll.Timer();
        Camera_controll = GameObject.FindWithTag("MainCamera").GetComponent<CameraControll>();
    }

    //##====================================================##
    //##                   Start 初期化２                   ##
    //##====================================================##
    private void Start()
    {

        // ポーズの際のキー画像を設定に応じて決定 (対応するobjをactiveにする)
        pause_effect_parent.transform.Find("-PAUSE-/EXIT/" + OptionData.current_options.controller).gameObject.SetActive(true);
        pause_effect_parent.transform.Find("-PAUSE-/RESUME/" + OptionData.current_options.controller).gameObject.SetActive(true);
        continue_effect_parent.transform.Find("Yes/" + OptionData.current_options.controller).gameObject.SetActive(true);
        continue_effect_parent.transform.Find("No/" + OptionData.current_options.controller).gameObject.SetActive(true);

        if (test_env)
        {
            var stocks = new ItemStocks();
            stocks.item_stocks[0].item_id = 1;
            stocks.item_stocks[0].num = 8;
            stocks.item_stocks[1].item_id = 3;
            stocks.item_stocks[1].num = 5;
            stocks.item_stocks[2].item_id = 2;
            stocks.item_stocks[2].num = 8;
            stocks.item_stocks[3].item_id = 2;
            stocks.item_stocks[3].num = 5;
            stocks.item_stocks[4].item_id = 2;
            stocks.item_stocks[4].num = 5;
            Play_data = new PlayData(1, 1,2, 20, 10, 1957291, 2157192, 52191,49, stocks);

        }

        StartCoroutine(GameStart(Play_data));
        Pausing(true);
    }

    //##====================================================##
    //##                  Update メインループ               ##
    //##====================================================##
    private void Update()
    {
        // ポーズ関連の処理
        PausingControll();

        // ポーズ中なら他処理は行わない
        if (is_pause)
            return;

        // プレイヤーが非アクティブならゲームオーバー処理を実行
        if (!fighter_comp.gameObject.activeSelf)
        {
            // ゲームオーバー処理
            GameOver();
        }

        // 敵のスポーン処理
        if (active_enemys_parent.transform.childCount <= MAX_ENEMYS)
        { 
            time += Time.deltaTime;
            if (time >= EigenValue.SPAWN_INTERVAL / (Play_data.choose_mode_id == 1 ? 4f : 1f))
            {
                MonsterSpawn();
                time = 0f;
            }
        }


        // ショップに入る処理
        if (shop_entrance_point.Can_enter_shop)
            if (InputControll.GetInputDown(InputControll.INPUT_ID_UPARROW))
            {
                Pausing(true);
                StartCoroutine(ShopControll.CreateShopWindow(this));
            }


        // 敵数を示すテキストを更新
        count_controll.Set_Enemy_text(active_enemys_parent.transform.childCount);

        // カウントダウンが止まっている＝終了しているなら
        if (!wave_controll_timer.Is_count)
        {
            // アクティブな敵が0になったら
            if (active_enemys_parent.transform.childCount == 0)
            {
                // クリアテキストを再生
                if (!wave_clear_effect.gameObject.activeSelf)
                {
                    StartCoroutine(WaveClear());
                }
            }

        }

        // アイテムストックカーソルを動かす処理
        ItemStockCursorControll();

    }

    //##====================================================##
    //##       ウェーブクリア時に、クリアエフェクトを       ##
    //##       待った後にコンティニュー画面へ遷移する       ##
    //##====================================================##
    IEnumerator WaveClear() 
    {

        // スコアを加算
        Score_count(wave_count * 10 + EigenValue.WAVE_BASE_TIME);

        AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_UI, AudioFilePositions.UI.WAVECLEAR);

        // クリアエフェクトを出す
        wave_clear_effect.transform.localPosition = Vector3.zero;
        wave_clear_effect.gameObject.SetActive(true);
        wave_clear_effect.GetComponent<TypefaceAnimator>().Play();

        yield return new WaitForSeconds(2f);

        // 最大ウェーブを超過したらクリア処理
        if (wave_count >= EigenValue.GetModeMaxWave(Play_data.choose_mode_id))
        {
            StartCoroutine(GameClear());
        }
        else // それ以外ではコンティニューするかの確認 
        {
            continue_effect_parent.SetActive(true);
            dimmer_spre.gameObject.SetActive(true);
            Pausing(true);
            wave_clear_effect.transform.DOLocalMoveY(10f, 0.5f).SetEase(Ease.OutSine);
        }

        yield break;
    }

    //##====================================================##
    //##               各種セッター・ゲッター               ##
    //##     (一部セッターにエフェクト処理が入っている)     ##
    //##====================================================##
    #region setter_and_getter

    // エフェクト群の親オブジェクトのセッター
    public GameObject Active_Effects_Parent() { return active_effects_parent; }
    public GameObject Active_Items_Parent() { return active_items_parent; }

    // wave_countのセッターとゲッター
    public void Wave_count(int get)
    {
        wave_count += get;
    }
    public int Wave_count()
    {
        return wave_count;
    }
    // score_countのセッターとゲッター
    public void Score_count(int get)
    {
        score += get;
        // テキスト更新
        count_controll.Set_Score_text(score);
    }
    public int Score_count()
    {
        return score;
    }
    // kill_countのセッターとゲッター
    public void Kill_count(int get, int character_score)
    {

        kill_count += get;
        // テキスト更新
        count_controll.Set_kill_text(kill_count);

        // 上昇だった場合はエフェクト発生
        if (get > 0)
        {
            GameObject effect = Instantiate(kill_up_effect, count_controll.Kill_count().transform.position, transform.rotation);
            effect.transform.parent = count_controll.Kill_count().transform;
        }

        // スコア加算
        Score_count(character_score);

    }
    public int Kill_count()
    {
        return kill_count;
    }
    // Moneyのセッターとゲッター
    public void Money(int get)
    {

        money += get;
        // テキスト更新
        count_controll.Set_money_text(money);

        // 上昇だった場合はエフェクト発生
        if (get > 0)
        {
            GameObject effect = Instantiate(money_up_effect, count_controll.Money_count().transform.position, transform.rotation);
            effect.transform.parent = count_controll.Money_count().transform;

        }


    }
    public int Money()
    {
        return money;
    }
    #endregion

    //##====================================================##
    //##           各ウェーブの制限時間を計算する           ##
    //##====================================================##
    private int Compute_Wave_timer(int wave_num)
    {
        return (wave_num - 1) * 5 + EigenValue.WAVE_BASE_TIME;
    }

    //##====================================================##
    //##     ゲームの開始処理（ゲームデータをロードする     ##
    //##====================================================##
    public IEnumerator GameStart(PlayData data)
    {
        this.enabled = false;

        // 仮想コントローラーをスティックに変更
        InputControll.use_virtual_stick = true;

        // カウンター系の初期設定
        money = data.money;
        kill_count = data.kill_count;
        wave_count = data.clear_waves;
        score = data.score;


        count_controll.Set_Score_text(score);
        count_controll.Set_money_text(money);
        count_controll.Set_kill_text(kill_count);
        Wave_proceed();

        // スポーン周期カウンター最大値の初期化
        for (int i = 0; i < spawn_flags_max.Length; i++)
            spawn_flags_max[i] = EigenValue.GetEnemyData(i).spawn_interval;


        // アイテムストックの初期化
        Item_stocks = data.item_stocks;
        for (int i = 0; i < Item_stocks.item_stocks.Length; i++)
        {
            var stock = item_stock_parent.transform.Find("StockID_" + (i + 1));

            bool is_null = Item_stocks.item_stocks[i].item_id == 0;

            Set_Stock_info(stock,Item_stocks.item_stocks[i].item_id, Item_stocks.item_stocks[i].num);

            // 所持していることで効果を発揮するアイテム（お守り系など）の効果を発生させる
            if (!is_null)
                for (int j = 0; j < Item_stocks.item_stocks[i].num; j++)
                    Resources.Load<Items>((EigenValue.PREFAB_DIRECTORY_ITEMS + EigenValue.GetItemData(Item_stocks.item_stocks[i].item_id).prefab_name)).Hold_Effect();

        }

        // HPとMPを決定

        fighter_comp = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<Fighters>();
        fighter_comp.HP = data.hp;
        fighter_comp.MP = data.mp;

        // UI上のゲージを更新
        fighter_comp.Hp_bar.RenewGaugeAmount();
        fighter_comp.Mp_bar.RenewGaugeAmount();

        yield return new WaitForSeconds(2);

        // READY... のエフェクトを表示
        ready_go_effect[0].SetActive(true);

        yield return new WaitForSeconds(2.5f);

        // GO!! のエフェクトを表示してゲーム開始
        this.enabled = true;
        Pausing(false);
        ready_go_effect[0].SetActive(false);
        ready_go_effect[1].SetActive(true);
        ready_go_effect[1].transform.DOLocalMoveX(0f, 1.5f).OnComplete(() =>
        {
            ready_go_effect[1].transform.DOLocalMoveY(360f, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
            {
                ready_go_effect[1].SetActive(false);

            });

        });

    }

    //##====================================================##
    //##           モンスターのスポーン全体の管理           ##
    //##====================================================##
    private void MonsterSpawn()
    {
        if (wave_controll_timer.Is_count)
        {
            // 実装されている各敵キャラについて
            for (int i = 0; i < spawn_flags.Length; i++)
            {
                spawn_flags[i]++;
                EnemyData enemyData = EigenValue.GetEnemyData(i);

                // スポーン周期に達していた　＆　スポーン開始ウェーブ数に達していたらスポーン処理を開始
                if (spawn_flags[i] >= spawn_flags_max[i] && wave_count >= enemyData.appear_wave / (Play_data.choose_mode_id == 1 ? 10 : 1))
                {
                    spawn_flags[i] = 0;
                    spawn_flags_max[i] = enemyData.spawn_interval + Random.Range(-2,2);

                    // ウェーブ数に応じてスポーンポイントの数を決定
                    for (int j = 0; j < (wave_count < enemy_spawn_point.transform.childCount * (EigenValue.DIFFICULTY_RANK_UP_COF / (Play_data.choose_mode_id == 1 ? 10 : 1)) ? wave_count / EigenValue.DIFFICULTY_RANK_UP_COF + 1 : enemy_spawn_point.transform.childCount); j++)
                    {
                        Transform spawn_point = enemy_spawn_point.transform.GetChild(j);
                        // スポーンポイントの左と右について
                        for (int k = 0; k < spawn_point.childCount; k++)
                        {
                            Spawn(i, spawn_point.GetChild(k).position);
                        }
                    }
                }
            }
        }
    }

    //##====================================================##
    //##             モンスターのスポーン（単体）           ##
    //##====================================================##
    void Spawn(int enemy_id,Vector3 position) 
    {
        Instantiate(Resources.Load<GameObject>(EigenValue.PREFAB_DIRECTORY_ENEMYS + EigenValue.GetEnemyData(enemy_id).prefab_name), position, transform.rotation, active_enemys_parent.transform);
    }

    //##====================================================##
    //##                 ウェーブを進行する                 ##
    //##====================================================##
    void Wave_proceed()
    {
        // 1ウェーブ進める
        Wave_count(1);
        // タイマーを更新
        wave_controll_timer.TimerStart(Compute_Wave_timer(wave_count));
        // テキストに反映
        wave_count_text.text = wave_count.ToString();

    }

    //##====================================================##
    //##       アイテムストックの画像を変更する処理         ##
    //##     (空にする場合はアイテムID = 0として渡す)       ##
    //##====================================================##
    void Set_Stock_info(Transform target_of_stock,int item_id,int num) 
    {
        // ストックの画像を設定
        target_of_stock.Find("icon").GetComponent<Image>().sprite =
            item_id == 0 ? null : Resources.Load<Transform>((EigenValue.PREFAB_DIRECTORY_ITEMS + EigenValue.GetItemData(item_id).prefab_name)).Find("graphic").GetComponent<SpriteRenderer>().sprite;


        // ストック数のテキストを設定
        target_of_stock.Find("text").GetComponent<Text>().text =
            item_id == 0 ? "" : num.ToString();

        // アイテム所持上限に達していたら所持数の枠色を変える
        if (item_id != 0)
            target_of_stock.Find("text").GetComponent<Outline>().effectColor = num >= EigenValue.GetItemData(item_id).max_hold ? Color.red : Color.white;
 
    }

    //##====================================================##
    //##       アイテムストックにアイテムをセットする       ##
    //##   (上限は考慮しない)  負の値を入れると消費する     ##
    //##====================================================##
    public void Set_item_stock(int item_id, int num, int index)
    {
        Transform target = item_stock_parent.transform.Find("StockID_" + (index + 1));

        // エフェクトを表示
        if (num > 0)
            target.Find("GetItemEffect").gameObject.SetActive(true);

        // アイテムストックの対象の位置が空なら
        if (Item_stocks.item_stocks[index].item_id == 0)
        {

            Item_stocks.item_stocks[index].item_id = item_id;
            Item_stocks.item_stocks[index].num += num;

            Set_Stock_info(target,Item_stocks.item_stocks[index].item_id, Item_stocks.item_stocks[index].num);
        }
        else if (Item_stocks.item_stocks[index].item_id == item_id) // 対象の位置が同種のアイテムなら
        {
            // 使用または消費
            Item_stocks.item_stocks[index].num += num;

            // ストック位置の所次数が０になったらアイテムIDを空値にしておく
            if (Item_stocks.item_stocks[index].num <= 0)
                Item_stocks.item_stocks[index].item_id = 0;
            
            Set_Stock_info(target,Item_stocks.item_stocks[index].item_id, Item_stocks.item_stocks[index].num);

        }

    }

    //##====================================================##
    //##     拾ったアイテムをアイテムストックに格納する     ##
    //##                 (場所は自動決定)                   ##
    //##====================================================##
    public void Set_item_stock_from_catch(int item_id,Items item_comp = null)
    {
        for (int i = 0; i < Item_stocks.item_stocks.Length; i++)
        {
            // 同じ種類をストックしている位置があったら
            if (Item_stocks.item_stocks[i].item_id == item_id)
            {
                // 最大所次数に到達していなければ追加
                if (Item_stocks.item_stocks[i].num < EigenValue.GetItemData(item_id).max_hold)
                {
                    Set_item_stock(item_id, 1, i);
                    if (item_comp != null)
                        item_comp.Hold_Effect();
                    return;
                }
            }
        }
        // どこにも同じアイテムをストックしていなかったら
        for (int i = 0; i < Item_stocks.item_stocks.Length; i++)
        {
            // 空のストックがあったらその位置に追加
            if (Item_stocks.item_stocks[i].item_id == 0)
            {
                Set_item_stock(item_id, 1, i);
                if (item_comp != null)
                    item_comp.Hold_Effect();
                return;
            }
        }
        // アイテムストックできなかったら捨てるエフェクトを出す
        Instantiate(drop_item_effect_prefab, fighter_comp.transform).GetComponent<ParticleSystem>()
            .textureSheetAnimation.SetSprite(0, 
            Resources.Load<Transform>(EigenValue.PREFAB_DIRECTORY_ITEMS + EigenValue.GetItemData(item_id).prefab_name).Find("graphic").GetComponent<SpriteRenderer>().sprite);

        return;
    }

    //##====================================================##
    //##   アイテムストックのカーソル位置にあるアイテムを   ##
    //##   使用する (使用できないアイテムならfalseを返す)   ##
    //##====================================================##
    public bool Use_item_from_stock()
    {
        // カーソルの位置にアイテムが格納されているなら
        if (Item_stocks.item_stocks[Item_stock_cursor].item_id > 0 && Item_stocks.item_stocks[Item_stock_cursor].num > 0)
        {
            // アイテムの使用を試みる
            bool is_useable = Resources.Load<Items>(EigenValue.PREFAB_DIRECTORY_ITEMS + (EigenValue.GetItemData(Item_stocks.item_stocks[Item_stock_cursor].item_id).prefab_name)).Use();
            
            // 使用できるアイテムは使用してTrueが返される
            if (is_useable)
            {
                // アイテムストックからアイテムを減らす
                fighter_comp.is_itemuse = true;
                Set_item_stock(Item_stocks.item_stocks[Item_stock_cursor].item_id, -1, Item_stock_cursor);
                return true;
            }
        }
        return false;

    }

    //##====================================================##
    //##        アイテムストックカーソルを動かす処理        ##
    //##                (動かしたらtrueを返す)              ##
    //##====================================================##
    public bool ItemStockCursorControll()
    {
        // 左へ移動
        if (InputControll.GetInputDown(InputControll.INPUT_ID_TRIGGER_L))
        {
            AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_PLAYER, AudioFilePositions.UI.MOVE_ITEMSTOCK_CURSOR);

            if (Item_stock_cursor == 0)
                Item_stock_cursor = Item_stocks.item_stocks.Length - 1;
            else
                Item_stock_cursor -= 1;

            RenewCursorPos();
            return true;
        }
        // 右へ移動
        else if (InputControll.GetInputDown(InputControll.INPUT_ID_TRIGGER_R))
        {
            AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_PLAYER, AudioFilePositions.UI.MOVE_ITEMSTOCK_CURSOR);

            if (Item_stock_cursor == Item_stocks.item_stocks.Length - 1)
                Item_stock_cursor = 0;
            else
                Item_stock_cursor += 1;

            RenewCursorPos();
            return true;
        }
        return false;
    }

    // サブ関数：カーソルの位置を更新する
    void RenewCursorPos()
    {
        item_stock_cursor_obj.transform.SetParent(item_stock_parent.transform.Find("StockID_" + (Item_stock_cursor + 1)));
        item_stock_cursor_obj.transform.localPosition = Vector3.forward * 8f;
    }

    //##====================================================##
    //##              ポーズ中の操作を行う処理              ##
    //##====================================================##
    private void PausingControll()
    {

        // ウェーブを進むかどうか決めているとき
        if (continue_effect_parent.activeSelf) 
        {
            // ゲームを続ける
            if (InputControll.GetInputDown(InputControll.INPUT_ID_A))
            {
                AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_UI, AudioFilePositions.UI.DECISION);
                Pausing(false);
                pause_effect_parent.SetActive(false);
                dimmer_spre.gameObject.SetActive(false);
                continue_effect_parent.SetActive(false);
                pause_effect_parent.transform.localPosition = Vector3.zero;
                // ウェーブを進める
                Wave_proceed();

                wave_clear_effect.transform.DOLocalMoveY(240f, 0.5f).SetEase(Ease.Linear).OnComplete(() => 
                {
                    wave_clear_effect.gameObject.SetActive(false);
                });

            }
            // セーブする
            else if (InputControll.GetInputDown(InputControll.INPUT_ID_B))
            {
                StartCoroutine(PopUpWindowControll.CreateSmallWindow(
                delegate () // はいを選択した処理
                {
                    SceneManager.sceneLoaded += Send_datas;
                    // セーブ画面へ遷移
                    InputControll.use_virtual_stick = false;
                    mcanim.MoveScene("SaveLoadGame");
                },
                delegate () // いいえを選択した処理
                {
                    InputControll.use_virtual_stick = false;
                    mcanim.MoveScene("Menu");
                },
                this, new Vector2(200f, 160f), CONFIRM_SAVE_TEXT, transform.position));

            }


        }
        else　// それ以外(通常のポーズ処理のとき)
        {
            // ポーズ中なら
            if (is_pause)
            {
                // ポーズを解除する操作
                if (InputControll.GetInputDown(InputControll.INPUT_ID_START) || InputControll.GetInputDown(InputControll.INPUT_ID_A))
                {

                    AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_UI, AudioFilePositions.UI.DECISION);

                    Pausing(false);
                    pause_effect_parent.SetActive(false);
                    dimmer_spre.gameObject.SetActive(false);
                    pause_effect_parent.transform.localPosition = Vector3.zero;
                }
                // メニュー画面に戻る操作
                else if (InputControll.GetInputDown(InputControll.INPUT_ID_B))
                {
                    AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_UI, AudioFilePositions.UI.DECISION);
                    InputControll.use_virtual_stick = false;
                    mcanim.MoveScene("Menu");
                    this.enabled = false;
                }
            }
            else
            // ポーズ中でないならポーズ開始する
            {
                if (InputControll.GetInputDown(InputControll.INPUT_ID_START))
                {
                    AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_UI, AudioFilePositions.UI.PAUSE);
                    dimmer_spre.gameObject.SetActive(true);
                    pause_effect_parent.SetActive(true);
                    Pausing(true);
                }
            }
        }
    }

    //##====================================================##
    //##                   ポーズをする処理                 ##
    //##  trueを入れるとポーズ、falseを入れるとポーズ解除   ##
    //##====================================================##
    public void Pausing(bool enable)
    {
        is_pause = enable;

        // ウェーブの進行を止める
        wave_controll_timer.enabled = !enable;


        // プレイヤーの動作を止める
        fighter_comp.GetComponent<Rigidbody2D>().simulated = !enable;
        fighter_comp.GetComponent<Animator>().enabled = !enable;
        fighter_comp.enabled = !enable;
        fighter_comp.Mini_hp_bar.Active_gauge.transform.DOTogglePause();
        fighter_comp.Mini_hp_bar.Damage_gauge.transform.DOTogglePause();
        fighter_comp.Mini_hp_bar.Empty_gauge.transform.DOTogglePause();
        fighter_comp.Mini_hp_bar.enabled = !enable;

        // エフェクトの動作をすべて止める
        foreach (Transform effect in fighter_comp.transform)
        {
            if (effect.name.Contains("EF_"))
            {
                if (effect.gameObject.activeSelf)
                {
                    if (effect.TryGetComponent(out ParticleSystem ps)) if (enable) ps.Pause(true); else ps.Play(true);
                    effect.transform.DOTogglePause();
                    foreach (Transform child in effect)
                    {
                        child.DOTogglePause();
                    }

                }
            }

        }

        // 敵の動作をすべて止める
        foreach (Transform enemy in active_enemys_parent.transform)
        {
            enemy.GetComponent<Rigidbody2D>().simulated = !enable;
            enemy.GetComponent<Animator>().enabled = !enable;

            Character enemy_chara_cp = enemy.GetComponent<Character>();
            enemy_chara_cp.Mini_hp_bar.Active_gauge.transform.DOTogglePause();
            enemy_chara_cp.Mini_hp_bar.Damage_gauge.transform.DOTogglePause();
            enemy_chara_cp.Mini_hp_bar.Empty_gauge.transform.DOTogglePause();
            enemy_chara_cp.Mini_hp_bar.enabled = !enable;
            enemy_chara_cp.enabled = !enable;

        }

        // エフェクトの動作をすべて止める

        foreach (Transform effect in active_effects_parent.transform)
        {
            if (effect.TryGetComponent(out Rigidbody2D rb2d)) rb2d.simulated = !enable;
            if (effect.TryGetComponent(out ParticleSystem ps)) if (enable) ps.Pause(true); else ps.Play(true);
            foreach (Transform child in effect)
            {
                if (effect.TryGetComponent(out ps)) if (enable) ps.Pause(true); else ps.Play(true);
                child.DOTogglePause();
            }

            if (effect.TryGetComponent(out Character chara_cp))
            {
                chara_cp.Mini_hp_bar.Active_gauge.transform.DOTogglePause();
                chara_cp.Mini_hp_bar.Damage_gauge.transform.DOTogglePause();
                chara_cp.Mini_hp_bar.Empty_gauge.transform.DOTogglePause();
                chara_cp.Mini_hp_bar.enabled = !enable;
                chara_cp.enabled = !enable;
            }
        }

        // フィールドに落ちてるアイテムの動作をすべて止める
        foreach (Transform item in active_items_parent.transform)
        {
            item.GetComponent<Rigidbody2D>().simulated = !enable;
            item.GetComponent<Items>().enabled = !enable;

        }
        // ステージギミックを止める
        //      後で実装
    }

    //##====================================================##
    //##                ゲームクリア時の処理                ##
    //##====================================================##
    IEnumerator GameClear() 
    {
        yield return new WaitForSeconds(5f);

        InputControll.use_virtual_stick = false;
        Pausing(true);
        SceneManager.sceneLoaded += Send_datas;
        mcanim.MoveScene("GameOver");
    }

    //##====================================================##
    //##               ゲームオーバー時の処理               ##
    //##====================================================##
    void GameOver()
    {
        InputControll.use_virtual_stick = false;
        SceneManager.sceneLoaded += Send_datas;
        Pausing(true);
        mcanim.MoveScene("GameOver");
    }

    //##====================================================##
    //##          遷移先(リザルト画面)へデータを送る        ##
    //##====================================================##
    void Send_datas(Scene next,LoadSceneMode mode)
    {
        Play_data.score = score;
        Play_data.money = money;
        Play_data.kill_count = kill_count;
        Play_data.clear_waves = wave_count;
        Play_data.hp = fighter_comp.HP;
        Play_data.mp = fighter_comp.MP;
        Play_data.item_stocks = Item_stocks;

        // 次のシーンによって処理
        switch (next.name) 
        {
            case "GameOver":  // ゲームオーバーへ遷移するとき
                {
                    GameOverControll gameOverControll = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameOverControll>();
                    gameOverControll.SetResult(Play_data, fighter_comp.HP > 0);

                    break;
                }

            case "SaveLoadGame":  // ゲームの保存へ遷移するとき
                {
                    SaveLoadControll saveLoadControll = GameObject.FindGameObjectWithTag("GameController").GetComponent<SaveLoadControll>();
                    saveLoadControll.Initialize(true, Play_data);
                    
                    break;
                }

            default:
                return;
        
        }
        SceneManager.sceneLoaded -= Send_datas;
    }

}

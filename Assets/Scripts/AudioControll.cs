using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


//--====================================================--
//--       サウンドファイルの位置を管理するクラス       --
//--====================================================--
public static class AudioFilePositions
{
    // UI系
    public static class UI
    {
        public const string FOLDER_NAME = "UI";

        public const string CURSOR_MOVE = FOLDER_NAME + "/" + "CursorMove";
        public const string MENUOPENCLOSE = FOLDER_NAME + "/" + "MenuOpenClose";
        public const string MOVE_ITEMSTOCK_CURSOR = FOLDER_NAME + "/" + "Move_Itemstock_Cursor";
        public const string CANCEL = FOLDER_NAME + "/" + "No";
        public const string DECISION = FOLDER_NAME + "/" + "Yes";
        public const string TITLE_PUSH = FOLDER_NAME + "/" + "TitlePush";
        public const string WAVECLEAR = FOLDER_NAME + "/" + "WaveClear";
        public const string PAUSE = FOLDER_NAME + "/" + "Pause";
        public const string ITEMDROP = FOLDER_NAME + "/" + "ItemDrop";
    }

    // エフェクト系
    public static class EFFECT 
    {
        public const string FOLDER_NAME = "Effect";

        // キャラに関係のない汎用エフェクト
        public const string CRUSH = FOLDER_NAME + "/" + "Crush";
        public const string DAMAGE = FOLDER_NAME + "/" + "Damage";
        public const string GETITEM = FOLDER_NAME + "/" + "GetItem";

        // プレイヤー専用
        public const string JUMP = FOLDER_NAME + "/" + "Jump";

        // その他通常エフェクト
        public const string REGENERATION = FOLDER_NAME + "/" + "Regeneration";
        public const string SWORD1 = FOLDER_NAME + "/" + "Sword1";
        public const string SWORD2 = FOLDER_NAME + "/" + "Sword2";
        public const string THUNDER = FOLDER_NAME + "/" + "Thunder";
    }
}

// 効果音の再生方法
/*

例：
    AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_UI, AudioFilePositions.UI.CURSOR_MOVE);

第１引数に再生する次元を、第２引数に再生する音声のファイル位置を指定
ファイル位置はAudioFilePositionsクラスで管理

*/

//--====================================================--
//--         効果音を再生する処理を管理するクラス       --
//--====================================================--
public class AudioControll : MonoBehaviour
{
    public const int SOUND_PLAYER_ID_BGM = 0;
    public const int SOUND_PLAYER_ID_PLAYER = 1;
    public const int SOUND_PLAYER_ID_ENEMY = 2;
    public const int SOUND_PLAYER_ID_ITEM = 3;
    public const int SOUND_PLAYER_ID_UI = 4;
    public const int SOUND_PLAYER_ID_CRUSHandDAMAGE = 5;

    // 効果音を再生するためのcomponent
    public AudioSource[] sound_player;

    // 現在のシーンのAudioControll
    static AudioControll current_scenes_ac = null;

    // そのシーンで利用するAudioClipのキャッシュ辞書（シーンの開始時に設定する）
    public static Dictionary<string, AudioClip> Cache_AudioClip { get; protected set; } = null;

    //##====================================================##
    //##                Awake     初期設定                  ##
    //##====================================================##
    private void Awake()
    {
        // シーン開始時にAudioControllを初期設定(既にDontDestroyOnLoadにあったらスルー)
        if (current_scenes_ac == null)
        {
            // 自身のobjを消えなくする
            DontDestroyOnLoad(this.gameObject);
            // 静的変数に設定
            current_scenes_ac = this.gameObject.GetComponent<AudioControll>();
            // シーン遷移時にキャッシュを初期化する処理を追加
            SceneManager.sceneLoaded += Clear_AudioCache;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    //##====================================================##
    //##          効果音キャッシュにClipを追加する          ##
    //##====================================================##
    public static void Set_AudioClipCache(AudioClip input_clip,string file_pos)
    {
        Cache_AudioClip.Add(file_pos, input_clip);
    }

    //##====================================================##
    //##    シーン遷移の際に効果音キャッシュを初期化する    ##
    //##====================================================##
    void Clear_AudioCache(Scene next, LoadSceneMode mode)
    {
        Cache_AudioClip = new Dictionary<string, AudioClip>();
    }

    //##====================================================##
    //##  効果音を再生する（clipの名前を渡して辞書から再生）##
    //##====================================================##
    public static void PlaySE(int sound_player_id, string clip_name, Renderer called_objs_renderer = null)
    {
        // 画面内に描写されていなければ何もしない
        if (called_objs_renderer != null)
            if (!called_objs_renderer.isVisible)
                return;

        // 現在のシーンのAudioControllがなければ新しく検索して設定
        if (current_scenes_ac == null)
        {
            current_scenes_ac = GameObject.FindWithTag("AudioController").GetComponent<AudioControll>();
        }
        //Debug.Log(clip_name);

        // キャッシュから取り出す、無ければキャッシュに追加
        if (!Cache_AudioClip.TryGetValue(clip_name, out AudioClip audio_clip))
        {
            audio_clip = Resources.Load<AudioClip>("Sounds/SE/" + clip_name);
            Set_AudioClipCache(audio_clip,clip_name);
        }

        current_scenes_ac.sound_player[sound_player_id].PlayOneShot(audio_clip);
    }

    //##====================================================##
    //##    BGMを再生する（clipの名前を渡して辞書から再生） ##
    //##====================================================##
    public static void PlayBGM(string clip_name, bool loop = false)
    {

        // 現在のシーンのAudioControllがなければ新しく検索して設定
        if (current_scenes_ac == null)
        {
            current_scenes_ac = GameObject.FindWithTag("AudioController").GetComponent<AudioControll>();
        }
        //Debug.Log(clip_name);

        current_scenes_ac.sound_player[SOUND_PLAYER_ID_BGM].clip = Resources.Load<AudioClip>("Sounds/SE/" + clip_name);
        current_scenes_ac.sound_player[SOUND_PLAYER_ID_BGM].loop = loop;
        current_scenes_ac.sound_player[SOUND_PLAYER_ID_BGM].Play();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


//--====================================================--
//--       �T�E���h�t�@�C���̈ʒu���Ǘ�����N���X       --
//--====================================================--
public static class AudioFilePositions
{
    // UI�n
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

    // �G�t�F�N�g�n
    public static class EFFECT 
    {
        public const string FOLDER_NAME = "Effect";

        // �L�����Ɋ֌W�̂Ȃ��ėp�G�t�F�N�g
        public const string CRUSH = FOLDER_NAME + "/" + "Crush";
        public const string DAMAGE = FOLDER_NAME + "/" + "Damage";
        public const string GETITEM = FOLDER_NAME + "/" + "GetItem";

        // �v���C���[��p
        public const string JUMP = FOLDER_NAME + "/" + "Jump";

        // ���̑��ʏ�G�t�F�N�g
        public const string REGENERATION = FOLDER_NAME + "/" + "Regeneration";
        public const string SWORD1 = FOLDER_NAME + "/" + "Sword1";
        public const string SWORD2 = FOLDER_NAME + "/" + "Sword2";
        public const string THUNDER = FOLDER_NAME + "/" + "Thunder";
    }
}

// ���ʉ��̍Đ����@
/*

��F
    AudioControll.PlaySE(AudioControll.SOUND_PLAYER_ID_UI, AudioFilePositions.UI.CURSOR_MOVE);

��P�����ɍĐ����鎟�����A��Q�����ɍĐ����鉹���̃t�@�C���ʒu���w��
�t�@�C���ʒu��AudioFilePositions�N���X�ŊǗ�

*/

//--====================================================--
//--         ���ʉ����Đ����鏈�����Ǘ�����N���X       --
//--====================================================--
public class AudioControll : MonoBehaviour
{
    public const int SOUND_PLAYER_ID_BGM = 0;
    public const int SOUND_PLAYER_ID_PLAYER = 1;
    public const int SOUND_PLAYER_ID_ENEMY = 2;
    public const int SOUND_PLAYER_ID_ITEM = 3;
    public const int SOUND_PLAYER_ID_UI = 4;
    public const int SOUND_PLAYER_ID_CRUSHandDAMAGE = 5;

    // ���ʉ����Đ����邽�߂�component
    public AudioSource[] sound_player;

    // ���݂̃V�[����AudioControll
    static AudioControll current_scenes_ac = null;

    // ���̃V�[���ŗ��p����AudioClip�̃L���b�V�������i�V�[���̊J�n���ɐݒ肷��j
    public static Dictionary<string, AudioClip> Cache_AudioClip { get; protected set; } = null;

    //##====================================================##
    //##                Awake     �����ݒ�                  ##
    //##====================================================##
    private void Awake()
    {
        // �V�[���J�n����AudioControll�������ݒ�(����DontDestroyOnLoad�ɂ�������X���[)
        if (current_scenes_ac == null)
        {
            // ���g��obj�������Ȃ�����
            DontDestroyOnLoad(this.gameObject);
            // �ÓI�ϐ��ɐݒ�
            current_scenes_ac = this.gameObject.GetComponent<AudioControll>();
            // �V�[���J�ڎ��ɃL���b�V�������������鏈����ǉ�
            SceneManager.sceneLoaded += Clear_AudioCache;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    //##====================================================##
    //##          ���ʉ��L���b�V����Clip��ǉ�����          ##
    //##====================================================##
    public static void Set_AudioClipCache(AudioClip input_clip,string file_pos)
    {
        Cache_AudioClip.Add(file_pos, input_clip);
    }

    //##====================================================##
    //##    �V�[���J�ڂ̍ۂɌ��ʉ��L���b�V��������������    ##
    //##====================================================##
    void Clear_AudioCache(Scene next, LoadSceneMode mode)
    {
        Cache_AudioClip = new Dictionary<string, AudioClip>();
    }

    //##====================================================##
    //##  ���ʉ����Đ�����iclip�̖��O��n���Ď�������Đ��j##
    //##====================================================##
    public static void PlaySE(int sound_player_id, string clip_name, Renderer called_objs_renderer = null)
    {
        // ��ʓ��ɕ`�ʂ���Ă��Ȃ���Ή������Ȃ�
        if (called_objs_renderer != null)
            if (!called_objs_renderer.isVisible)
                return;

        // ���݂̃V�[����AudioControll���Ȃ���ΐV�����������Đݒ�
        if (current_scenes_ac == null)
        {
            current_scenes_ac = GameObject.FindWithTag("AudioController").GetComponent<AudioControll>();
        }
        //Debug.Log(clip_name);

        // �L���b�V��������o���A������΃L���b�V���ɒǉ�
        if (!Cache_AudioClip.TryGetValue(clip_name, out AudioClip audio_clip))
        {
            audio_clip = Resources.Load<AudioClip>("Sounds/SE/" + clip_name);
            Set_AudioClipCache(audio_clip,clip_name);
        }

        current_scenes_ac.sound_player[sound_player_id].PlayOneShot(audio_clip);
    }

    //##====================================================##
    //##    BGM���Đ�����iclip�̖��O��n���Ď�������Đ��j ##
    //##====================================================##
    public static void PlayBGM(string clip_name, bool loop = false)
    {

        // ���݂̃V�[����AudioControll���Ȃ���ΐV�����������Đݒ�
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

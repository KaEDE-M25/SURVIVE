using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

//--====================================================--
//--       画面遷移時のエフェクトを操作するクラス       --
//--====================================================--
public class MoveSceneAnimation : MonoBehaviour
{
    [SerializeField,Tooltip("タイルのグラフィック。この中からランダムに敷き詰める。")]
    TileBase[] tilebases = new TileBase[6];
    // 開始座標
    readonly Vector2Int start_pos = new Vector2Int(15,16);
    // このオブジェクトのTimemapコンポーネント
    Tilemap this_tilemap;
    // フェード処理の状態
    public int fading = 0; // 0=default 1=fadein -1=fadeout 2=cover

    // フェード処理に使用するカウンタ
    int target_tile_x;
    int[] target_tile_y = new int[33];
    int count = 0;

    // 遷移先シーンの名前
    public string to = "";
    // メインカメラのCameraControllコンポーネント
    CameraControll camera_ctrl;

    //##====================================================##
    //##                Awake       初期化処理              ##
    //##====================================================##
    private void Awake()
    {
        this_tilemap = GetComponent<Tilemap>();
        camera_ctrl = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraControll>();
    }

    //##====================================================##
    //##               Update      フェード処理             ##
    //##====================================================##
    void Update()
    {
        switch (fading)
        {
            case 1: //フェードイン処理
                {
                    for (int i = 0; i < count; i++)
                    {
                        if(target_tile_y[i] > -12) 
                        {
                            Vector3Int vec = Vector3Int.zero;

                            vec.x = start_pos.x - i;
                            vec.y = target_tile_y[i];

                            this_tilemap.SetTile(vec, tilebases[Random.Range(0,tilebases.Length)]);

                            target_tile_y[i]--;
                        }
                    }

                    if (count < target_tile_y.Length - 1)
                    {
                        count++;
                        target_tile_x--;
                        target_tile_y[count] = 12;
                    }

                    if (target_tile_y[target_tile_y.Length - 2] <= -12)
                    {
                        fading = 2;
                        transform.parent.SetParent(null);
                        DontDestroyOnLoad(transform.parent.gameObject);
                        SceneManager.LoadScene(to);
                        
                        transform.parent.position = Vector3.zero;
                        Invoke(nameof(FadeOut), 0.5f);
                    }
                    break;
                }

            case -1: // フェードアウト処理
                {
                    for (int i = 0; i < count; i++)
                    {
                        if (target_tile_y[i] > -12)
                        {
                            Vector3Int vec = Vector3Int.zero;

                            vec.x = start_pos.x - i;
                            vec.y = target_tile_y[i];

                            this_tilemap.SetTile(vec, null);

                            target_tile_y[i]--;
                        }
                    }

                    if (count < target_tile_y.Length - 1)
                    {
                        count++;
                        target_tile_x--;
                        target_tile_y[count] = 12;
                    }

                    if (target_tile_y[target_tile_y.Length - 2] <= -12)
                    {
                        fading = 0;
                        Destroy(transform.parent.gameObject);
                                        
                    }
                    break;
                }

            default:
                break;
        }
    }

    //##====================================================##
    //##        LateUpdate      エフェクト位置の調整        ##
    //##====================================================##
    private void LateUpdate()
    {
        // メインカメラがあるなら同じ位置に移動(x座標しか動かないのでx座標にのみ適用)
        if (camera_ctrl == null)
            transform.parent.position = Vector3.zero;
        else
            transform.parent.position = camera_ctrl.transform.position * Vector2.right;
    }

    //##====================================================##
    //##            フェードインを開始する処理              ##
    //##====================================================##
    public void FadeIn() 
    {
        if (fading < 1)
        {
            fading = 1;
            count = 0;
            target_tile_x = start_pos.x;
            target_tile_y[0] = 12;
            target_tile_y = new int[33];
        }
    }

    //##====================================================##
    //##           フェードアウトを開始する処理             ##
    //##====================================================##
    public void FadeOut() 
    {
        if (fading > -1)
        {
            fading = -1;
            count = 0;
            target_tile_x = start_pos.x;
            target_tile_y = new int[33];
        }
    }

    //##====================================================##
    //##                シーン遷移を行う処理                ##
    //##====================================================##
    public void MoveScene(string to)
    {
        this.to = to;
        FadeIn();
    }
}

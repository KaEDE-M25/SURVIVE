using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class MoveSceneAnimation : MonoBehaviour
{
    [SerializeField]
    TileBase[] tilebases = new TileBase[6];
    Vector2Int start_pos = new Vector2Int(15,16);
    [SerializeField]
    Tilemap this_tilemap;
    public int fading = 0; // 0=default 1=fadein -1=fadeout 2=cover
    int target_tile_x;
    [SerializeField]
    int[] target_tile_y = new int[33];
    int count = 0;

    public string to = "";
    [SerializeField]
    CameraControll camera_ctrl;
    Vector3 camera_pos_onlyX = Vector3.zero;


    private void OnEnable()
    {
        camera_ctrl = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraControll>();
    }


    // Update is called once per frame
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

    private void LateUpdate()
    {
        if (camera_ctrl == null)
            transform.parent.position = Vector3.zero;
        else
        { 
        camera_pos_onlyX.x = camera_ctrl.transform.position.x;
        transform.parent.position = camera_pos_onlyX;
        }
    }


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

    public void MoveScene(string to)
    {
        this.to = to;
        FadeIn();
    }



}

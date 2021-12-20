using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

//--====================================================--
//--       ��ʑJ�ڎ��̃G�t�F�N�g�𑀍삷��N���X       --
//--====================================================--
public class MoveSceneAnimation : MonoBehaviour
{
    [SerializeField,Tooltip("�^�C���̃O���t�B�b�N�B���̒����烉���_���ɕ~���l�߂�B")]
    TileBase[] tilebases = new TileBase[6];
    // �J�n���W
    readonly Vector2Int start_pos = new Vector2Int(15,16);
    // ���̃I�u�W�F�N�g��Timemap�R���|�[�l���g
    Tilemap this_tilemap;
    // �t�F�[�h�����̏��
    public int fading = 0; // 0=default 1=fadein -1=fadeout 2=cover

    // �t�F�[�h�����Ɏg�p����J�E���^
    int target_tile_x;
    int[] target_tile_y = new int[33];
    int count = 0;

    // �J�ڐ�V�[���̖��O
    public string to = "";
    // ���C���J������CameraControll�R���|�[�l���g
    CameraControll camera_ctrl;

    //##====================================================##
    //##                Awake       ����������              ##
    //##====================================================##
    private void Awake()
    {
        this_tilemap = GetComponent<Tilemap>();
        camera_ctrl = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraControll>();
    }

    //##====================================================##
    //##               Update      �t�F�[�h����             ##
    //##====================================================##
    void Update()
    {
        switch (fading)
        {
            case 1: //�t�F�[�h�C������
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

            case -1: // �t�F�[�h�A�E�g����
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
    //##        LateUpdate      �G�t�F�N�g�ʒu�̒���        ##
    //##====================================================##
    private void LateUpdate()
    {
        // ���C���J����������Ȃ瓯���ʒu�Ɉړ�(x���W���������Ȃ��̂�x���W�ɂ̂ݓK�p)
        if (camera_ctrl == null)
            transform.parent.position = Vector3.zero;
        else
            transform.parent.position = camera_ctrl.transform.position * Vector2.right;
    }

    //##====================================================##
    //##            �t�F�[�h�C�����J�n���鏈��              ##
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
    //##           �t�F�[�h�A�E�g���J�n���鏈��             ##
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
    //##                �V�[���J�ڂ��s������                ##
    //##====================================================##
    public void MoveScene(string to)
    {
        this.to = to;
        FadeIn();
    }
}

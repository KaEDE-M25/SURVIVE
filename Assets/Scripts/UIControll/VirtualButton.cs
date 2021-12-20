using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//--====================================================--
//--    ScreenPadにある仮想ボタンを扱う際のデータ集合   --
//-- （ボタンのobjに取り付けてイベントで渡して値を調整）--
//--====================================================--
public class VirtualButton : MonoBehaviour
{
    // オブジェクトについているImageコンポーネント
    public Image image;
    
    // 押されてない状態のボタンのSprite
    public Sprite base_button_sprite;
    // 押されている状態のボタンのSprite
    public Sprite pushed_button_sprite;
}

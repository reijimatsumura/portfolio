using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RotaionSpeedController : MonoBehaviour
{
    //エフェクトの再生速度の値をいじる
    public ItemDateController itemDateController;
    //画像を入れるところ
    public Image moveSpeedButtonImage;
    //速度ごとに表示する画像を入れる
    public Sprite[] moveSpeedImages;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    /// <summary>
    /// クリックしたとき速度と画像を変更する
    /// </summary>
    public void MoveSpeedButtonClick()
    {
        itemDateController.kilnMoveSpeed++;

        //3倍を超えたか
        if(itemDateController.kilnMoveSpeed > 3)
        {
            itemDateController.kilnMoveSpeed = 1;
        }

        moveSpeedButtonImage.sprite = moveSpeedImages[(int)itemDateController.kilnMoveSpeed - 1];
    }
}

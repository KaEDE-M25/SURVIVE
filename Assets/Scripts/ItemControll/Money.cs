using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//--====================================================--
//--                   ƒAƒCƒeƒ€  ‚¨‹à                   --
//--====================================================--
public class Money : Items
{
    // Šl“¾‚Å‚«‚é‚¨‹à‚Ì—Ê
    [SerializeField]
    int MONEY_AMOUNT = 0;


    protected override void GetItem()
    {
        GameObject.FindWithTag("GameController").GetComponent<GameControll>().Money(MONEY_AMOUNT);
    }

    public override bool Use()
    {
        return false;
    }

    public override bool Drop()
    {
        return false;
    }

    public override bool Hold_Effect()
    {
        return false;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpPotion : Item
{

    [SerializeField] float _healingAmount;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        //Custom hp potion start

    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void Use()
    {
        base.Use();
        PlayerController.main.GetHealed(_healingAmount);
        Deplete();
    }
}

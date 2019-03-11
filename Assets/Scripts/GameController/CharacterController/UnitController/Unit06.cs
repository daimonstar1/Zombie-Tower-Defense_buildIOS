using UnityEngine;
using System.Collections;

public class Unit06 : UnitController
{
    // Use this for initialization
    public override void Shoot()
    {
        if (action.Dead || !status.IsCanShoot || status.IsFreezing) return;

        Master.Gameplay.UnitGetExp(data.UnitID, Master.UnitData.GetExpPerShootAtLevel(data.Level));

        status.CurrentShoot++;
        status.CurrentAmmo--;
        SetTimeToShoot();
        if (!status.IsUpgraded)
        {
            CreateBullet(0.4f);
        }
        else
        {
            CreateBullet(0.4f);
            Master.WaitAndDo(0.3f, () =>
            {
                CreateBullet(0.4f);
            }, this);
        }
        
    }

}

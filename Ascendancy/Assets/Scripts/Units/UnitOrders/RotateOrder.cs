﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateOrder : UnitOrder
{
    private Vector3 orientation;

    public RotateOrder(Unit unit, Vector3 orientation) : base(unit)
    {
        this.orientation = orientation;
    }

    public override Vector3 CurrentDestination
    {
        get { return unit.transform.position; }
    }

    public override bool Fulfilled
    {
        get { return Vector3.Angle(unit.transform.forward, orientation) < 6; }
    }
    
    public override void Update()
    {
        unit.transform.GetComponent<UnitRotator>().RotateTowards(orientation);
    }

    public Vector3 TargetOrientation
    {
        get { return orientation; }
    }
}

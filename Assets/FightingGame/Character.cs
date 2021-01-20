﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(Moveset))]
public class Character : MonoBehaviour
{
    private Moveset moveset;
    private Animator animator;

    void Start()
    {
        Application.targetFrameRate = 60;
        animator = GetComponent<Animator>();
        moveset = GetComponent<Moveset>();


        moveset.Initialize();
        //moveset = new Moveset(animator);
        //moveset.Initialize();


    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            moveset.ChangeState("idle");
        if (Input.GetKeyDown(KeyCode.W))
            moveset.ChangeState("attack");

        moveset.DoFixedUpdate();
    }
}

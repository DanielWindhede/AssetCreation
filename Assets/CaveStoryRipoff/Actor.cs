using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    private float deltaX;
    private float deltaY;

    private Vector2 moveAmount;

    [SerializeField] private BoxCollider2D xCollision;
    [SerializeField] private BoxCollider2D yCollision;

    private struct Bounds
    {
        
    }

    // Start is called before the first frame update
    public virtual void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //https://www.youtube.com/watch?v=Hnr98Ga-uLc
    //https://www.youtube.com/watch?v=ny14i0GxGZw&t=942s
    private void Collisions()
    {
        deltaX = moveAmount.x;
        deltaY = moveAmount.y;

        if (deltaX >= 0) // right side first, otherwise, left side
        {

        }

        if (deltaY <= 0) // do bottom first
        {

        }
    }

    public void Move(Vector2 move)
    {
        moveAmount += move;
    }
}

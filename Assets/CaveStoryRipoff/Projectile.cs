using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    Up = 2,
    Right = 1,
    Down = -2,
    Left = -1,
}

[RequireComponent(typeof(SpriteRenderer))]
public class Projectile : MonoBehaviour
{
    public int damage;
    public int requiredExperience;
    public Weapon owner;

    [SerializeField, Tooltip("Up, Right, Down, Left. If only two are used, they will be mirrored")]
    protected List<Sprite> directionalSprites;

    private SpriteRenderer spriteRenderer;
    private Sprite CurrentSprite
    { 
        get { return spriteRenderer.sprite; } 
        set { spriteRenderer.sprite = value; }
    }

    private Direction _direction;
    public Direction Direction
    {
        get { return _direction; }
        set
        {
            _direction = value;
            UpdateSpriteDirection();
        }
    }

    public Vector2 GetVecFromDirection(Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                return Vector2.up;
            case Direction.Right:
                return Vector2.right;
            case Direction.Down:
                return Vector2.down;
            case Direction.Left:
                return Vector2.left;
            default:
                return Vector2.zero;
        }
    }

    private void UpdateSpriteDirection()
    {
        spriteRenderer.flipX = false;
        spriteRenderer.flipY = false;

        switch (_direction)
        {
            case Direction.Up:
                CurrentSprite = directionalSprites[0];
                break;
            case Direction.Right:
                CurrentSprite = directionalSprites[1];
                break;
            case Direction.Down:
                if (directionalSprites.Count > 2)
                    CurrentSprite = directionalSprites[2];
                else
                {
                    CurrentSprite = directionalSprites[0];
                    spriteRenderer.flipY = true;
                }
                break;
            case Direction.Left:
                if (directionalSprites.Count > 2)
                    CurrentSprite = directionalSprites[3];
                else
                {
                    CurrentSprite = directionalSprites[1];
                    spriteRenderer.flipX = true;
                }
                break;
            default:
                break;
        }
    }

    protected virtual void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    protected virtual void Start() { }

    // Update is called once per frame
    protected virtual void Update() { }
}

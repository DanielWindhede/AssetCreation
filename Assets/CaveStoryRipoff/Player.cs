using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : Actor
{
    private int _currentWeaponIndex;
    private int WeaponIndex
    {
        get { return _currentWeaponIndex; }
        set
        {
            if (value < 0)
                _currentWeaponIndex = weapons.Count - 1;
            else if (value >= weapons.Count)
                _currentWeaponIndex = 0;
            else
                _currentWeaponIndex = value;

            for (int i = 0; i < 3; i++)
            {
                UIWeaponIcons[i].color = new Color(1, 1, 1, 0);
            }

            if (weapons.Count > 0)
            {
                weaponSpriteRenderer.sprite = CurrentWeapon.weaponSprite;
                weaponSpriteRenderer.color = new Color(1, 1, 1, 1);
                for (int i = 0; i < (weapons.Count >= 3 ? 3 : weapons.Count); i++)
                {
                    UIWeaponIcons[i].color = new Color(1, 1, 1, 1);
                    UIWeaponIcons[i].sprite = GetWeapon(_currentWeaponIndex + i).icon;
                }
            }
            else
                weaponSpriteRenderer.color = new Color(1, 1, 1, 0);

            /*
            firstWeaponIcon.sprite = weapons.Count > 0 ? GetWeapon(_currentWeaponIndex).icon : null;
            secondWeaponIcon.sprite = weapons.Count > 1 ? GetWeapon(_currentWeaponIndex + 1).icon : null;
            thirdWeaponIcon.sprite = weapons.Count > 2 ? GetWeapon(_currentWeaponIndex + 2).icon : null;
            */
        }
    }

    [SerializeField] private List<Image> UIWeaponIcons;
    [SerializeField] private SpriteRenderer weaponSpriteRenderer;
    [SerializeField] List<Weapon> weapons;
    [SerializeField] Rigidbody2D body;
    [SerializeField] SpriteRenderer spriteRenderer;
    Animator animator;
    Direction direction;
    public Weapon CurrentWeapon { get { return weapons[_currentWeaponIndex]; } }
    public Weapon GetWeapon(int index)
    {
        int value = index;
        if (value < 0)
            value = weapons.Count - 1;
        else if (value >= weapons.Count)
            value = index - weapons.Count;

        return weapons[value]; 
    }

    // Start is called before the first frame update
    public override void Start()
    {
        body = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        direction = Direction.Right;
        WeaponIndex = 0;
    }

    // Update is called once per frame
    void Update()
    {
        InputHanding();
        UpdateWeapon();
        RigidbodyMovement();
    }

    Vector2 input;
    bool jump;
    private void InputHanding()
    {
        input.x = (Input.GetKey(KeyCode.RightArrow) ? 1 : 0) + (Input.GetKey(KeyCode.LeftArrow) ? -1 : 0);
        input.y = (Input.GetKey(KeyCode.UpArrow) ? 1 : 0) + (Input.GetKey(KeyCode.DownArrow) ? -1 : 0);
        jump = Input.GetKey(KeyCode.Z);

        if (input.x < 0)
        {
            transform.rotation = Quaternion.Euler(Vector3.up * 180);
            direction = Direction.Left;
        }
        else if (input.x > 0)
        {
            transform.rotation = Quaternion.Euler(Vector3.zero);
            direction = Direction.Right;
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            WeaponIndex--;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            WeaponIndex++;
        }

        if (input.y > 0)
            direction = Direction.Up;
        //else if (input.y < 0 && inAir)

        animator.SetFloat("Speed", body.velocity.x * Mathf.Sign(body.velocity.x));
        animator.SetInteger("X", (int)input.x);
        animator.SetInteger("Y", (int)input.y);

        CurrentWeapon.Direction = direction;

        print(input);
    }

    private void RigidbodyMovement()
    {
        Vector2 velocity = body.velocity;
        velocity.x = input.x * 5;

        if (jump)
            velocity.y = 5;

        body.velocity = velocity;
    }

    private void UpdateWeapon()
    {
        if (weapons.Count > 0)
            CurrentWeapon.DoUpdate();
    }
}

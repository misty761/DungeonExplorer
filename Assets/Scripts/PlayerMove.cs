using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMove : MonoBehaviour
{
    // player speed
    public float speed = 1f;
    float speedInit;

    // Hp bar
    public float hpBarOffsetY = 0.1f;
    public int healthMax = 100;
    int healthMaxInit;
    public int health;
    public GameObject prefHpBar;
    RectTransform hpBar;
    Image nowHpbar;
    GameObject canvas;
    Camera myCamera;
    Image topHpbar;

    // text damage
    public float textDamageOffsetY = 0.15f;
    public GameObject prefTextDamage;

    // animation
    public Animator animator;
    bool isLookingRight;
    int directionAttack;  // 공격 방향, 0:up, 1:down, 2: right or left

    // control
    Joystick joystick;
    bool isMoving;
    float h;
    float v;

    // hit effect
    public GameObject effectHit;

    // money
    public int money;
    Text textGold;

    // level up
    public int level;
    public int exprience;
    public int exprienceNextLevel = 30;
    Text textLevel;
    Image expBar;
    int healthIncrement;
    float speedIncrement;
    public int power;
    public int powerIncrement = 10;
    int powerInit;
    float attackSpeedFactor;
    float attackSpeedPlayer;
    readonly float attackSpeedIncrement = 0.01f;

    // weapon
    public float attackSpeedWeapon;
    
    // Start is called before the first frame update
    void Start()
    {
        canvas = GameObject.Find("Canvas");
        isLookingRight = true;
        joystick = FindObjectOfType<Joystick>();
        animator = GetComponent<Animator>();
        
        // hp bar
        hpBar = Instantiate(prefHpBar, canvas.transform).GetComponent<RectTransform>();
        nowHpbar = hpBar.Find("Health").GetComponent<Image>();
        hpBar.gameObject.SetActive(false);
        health = healthMax;
        myCamera = GameObject.Find("Camera").GetComponent<Camera>();      

        // level
        healthIncrement = healthMax;
        healthMaxInit = healthMax;
        speedInit = speed;
        speedIncrement = speed * 0.01f;
        powerInit = power;
        Init();
        UpdateExpBar();
    }

    public void Init()
    {
        directionAttack = 2;
        money = 0;
        level = 1;
        exprience = 0;
        power = powerInit;
        speed = speedInit;
        attackSpeedFactor = 1f;
        attackSpeedPlayer = 1f;
        healthMax = healthMaxInit;
        health = healthMax;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.state != GameManager.State.Play)
        {
            //hpBar.gameObject.SetActive(false);
            return;
        } 
       
        // hp bar
        if (myCamera == null)
        {
            myCamera = GameObject.Find("Camera").GetComponent<Camera>();
        }
        Vector2 _hpBarPos = myCamera.WorldToScreenPoint(new Vector2(transform.position.x, transform.position.y + hpBarOffsetY));
        hpBar.position = _hpBarPos;
        hpBar.gameObject.SetActive(true);

        // joystick
        joystick = FindObjectOfType<Joystick>();
        float joystickFactor = 2f;
        h = joystick.Horizontal * joystickFactor;
        v = joystick.Vertical * joystickFactor;
        if (h > 1f) h = 1f;
        else if (h < -1f) h = -1f;
        if (v > 1f) v = 1f;
        else if (v < -1f) v = -1f;

        // player move
        float joystickMoveMin = 0.1f;
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Translate(Vector2.left * Time.deltaTime * speed);
            isLookingRight = false;
            isMoving = true;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Translate(Vector2.right * Time.deltaTime * speed);
            isLookingRight = true;
            isMoving = true;
        }
        else if (h < -joystickMoveMin || h > joystickMoveMin)
        {
            transform.Translate(Vector2.right * Time.deltaTime * speed * h);
            isMoving = true;
            if (h < -joystickMoveMin)
            {
                isLookingRight = false;
            }
            else
            {
                isLookingRight = true;
            }
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.Translate(Vector2.up * Time.deltaTime * speed);
            isMoving = true;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.Translate(Vector2.down * Time.deltaTime * speed);
            isMoving = true;
        }
        else if (v < -joystickMoveMin || v > joystickMoveMin)
        {
            transform.Translate(Vector2.up * Time.deltaTime * speed * v);
            isMoving = true;
        }

        if (!Input.GetKey(KeyCode.LeftArrow)
            && !Input.GetKey(KeyCode.RightArrow)
            && !Input.GetKey(KeyCode.UpArrow)
            && !Input.GetKey(KeyCode.DownArrow)
            && h > -joystickMoveMin
            && h < joystickMoveMin
            && v > -joystickMoveMin
            && v < joystickMoveMin)
        {
            isMoving = false;
        }

        // 바라보고 있는 방향에 따른 스프라이트 회전
        if (isLookingRight)
        {
            transform.localScale = new Vector2(1, 1);
        }
        else
        {
            transform.localScale = new Vector2(-1, 1);
        }

        // 공격 방향, 0:up, 1:down, 2: right or left
        if (v > 0.5f || Input.GetKey(KeyCode.UpArrow)) directionAttack = 0;
        else if (v < -0.5f || Input.GetKey(KeyCode.DownArrow)) directionAttack = 1;
        else if (h > 0.5f || h < -0.5f
            || Input.GetKey(KeyCode.RightArrow)
            || Input.GetKey(KeyCode.LeftArrow)) directionAttack = 2;

        // 애니메이션
        animator.SetBool("Running", isMoving);
        animator.SetInteger("AttackDirection", directionAttack);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            Damaged(collision.gameObject, collision);
        }
    }

    public void Damaged(GameObject go)
    {
        // 공격 당함
        SoundManager.instance.PlaySound(SoundManager.instance.audioDamaged, 1f);
        if (go.layer == LayerMask.NameToLayer("FloorSpikes"))
        {
            FloorSpikes floorSpikes = go.GetComponent<FloorSpikes>();
            int damage = Random.Range(1, floorSpikes.power + 1);
            health -= damage;
            if (damage >= floorSpikes.power * 0.9f)
            {
                SetFloatingText(damage, "ff0000");
            }
            else
            {
                SetFloatingText(damage, "ffffff");
            }
        }

        if (health <= 0)
        {
            // 죽음
            death();
        }

        UpdateHpBar();
    }

    public void Damaged(GameObject go, Collision2D collision)
    {
        SoundManager.instance.PlaySound(SoundManager.instance.audioDamaged, 1f);
        
        // hit effect
        Instantiate(effectHit, collision.contacts[0].point, transform.rotation);
     
        if (go.layer == LayerMask.NameToLayer("Enemy"))
        {
            EnemyBehavior enemy = go.GetComponent<EnemyBehavior>();
            int damage = Random.Range(1, enemy.power + 1);
            health -= damage;
            if (damage >= enemy.power * 0.9f)
            {
                SetFloatingText(damage, "ff0000");
            }
            else
            {
                SetFloatingText(damage, "ffffff");
            }
        }
        else if (go.layer == LayerMask.NameToLayer("EnemyWeapon"))
        {
            EnemyWeaponThrow weapon = go.GetComponent<EnemyWeaponThrow>();
            int damage = Random.Range(1, weapon.power + 1);
            health -= damage;
            if (damage >= weapon.power * 0.9f)
            {
                SetFloatingText(damage, "ff0000");
            }
            else
            {
                SetFloatingText(damage, "ffffff");
            }
        }

        if (health <= 0)
        {
            // 죽음
            death();
        }

        UpdateHpBar();
    }


    void death()
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Die"))
        {
            animator.SetTrigger("Die");
            GameManager.instance.GameOver();
        }
    }

    public void AddHealth(float addPercent)
    {
        SoundManager.instance.PlaySound(SoundManager.instance.audioLifeUp, 1f);
        int add = (int) (healthMax * addPercent / 100);
        health += add;
        SetFloatingText("+" + add + " hp", "00ff00");
        if (health > healthMax) health = healthMax;
        UpdateHpBar();
    }

    public void AddCoin(int price)
    {
        SoundManager.instance.PlaySound(SoundManager.instance.audioCoin, 1f);
        money += price;
        SetFloatingText("+" + price + " gold", "ffff00");
        UpdateCoin();
    }

    public void UpdateCoin()
    {
        textGold = GameObject.Find("Text Gold").GetComponent<Text>();
        textGold.text = "" + money;
    }

    public void UpdateHpBar()
    {
        try
        {
            nowHpbar.fillAmount = (float)health / (float)healthMax;
            topHpbar = GameObject.Find("Top Health").GetComponent<Image>();
            topHpbar.fillAmount = (float)health / (float)healthMax;
        }
        catch
        {
            Debug.Log("Fail to update HP bar!");
        }
    }

    void SetFloatingText(int message, string color)     // color : 000000~ffffff
    {
        Text text = Instantiate(prefTextDamage, canvas.transform).GetComponent<Text>();
        Vector2 pos = myCamera.WorldToScreenPoint(new Vector2(transform.position.x, transform.position.y + textDamageOffsetY));
        text.transform.position = pos;
        text.text = "<color=#" + color + ">" + message + "</color>";
    }

    void SetFloatingText(string message, string color)     // color : 000000~ffffff
    {
        Text text = Instantiate(prefTextDamage, canvas.transform).GetComponent<Text>();
        Vector2 pos = myCamera.WorldToScreenPoint(new Vector2(transform.position.x, transform.position.y + textDamageOffsetY));
        text.transform.position = pos;
        text.text = "<color=#" + color + ">" + message + "</color>";
    }

    public void AddExp(int point)
    {
        // EXP ++
        exprience += point;
        UpdateExpBar();

        // level up
        if (exprience >= exprienceNextLevel)
        {
            SoundManager.instance.PlaySound(SoundManager.instance.audioLevelUp, 1f);
            level++;
            exprience = 0;
            exprienceNextLevel = exprienceNextLevel * 2;
            UpdateLevel();
            Invoke("DelayLevelUpText", 0.1f);
            //SetFloatingText("level up!", "0000ff");
            
            // level up 에 따른 능력 향상
            healthMax += healthIncrement;
            AddHealth(healthIncrement/healthMax);
            speed += speedIncrement;
            power += powerIncrement;
            attackSpeedPlayer += attackSpeedIncrement;
            SetAttackSpeed();
        }

        // Score ++
        GameManager.instance.AddScore(point);
    }

    public void UpdateLevel()
    {
        textLevel = GameObject.Find("Text Player Level").GetComponent<Text>();
        textLevel.text = "lv. " + level;
    }

    void SetAttackSpeed()
    {
        attackSpeedFactor = attackSpeedPlayer * attackSpeedWeapon;
        animator.SetFloat("AttackSpeed", attackSpeedFactor);
    }

    void DelayLevelUpText()
    {
        SetFloatingText("level up!", "0000ff");
    }

    public void UpdateExpBar()
    {
        try
        {
            expBar = GameObject.Find("EXP Bar").GetComponent<Image>();
            expBar.fillAmount = (float)exprience / (float)exprienceNextLevel;
        }
        catch
        {
            // Exception
        }
    }

    public void Attack()
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Attack")
                && !animator.GetCurrentAnimatorStateInfo(0).IsName("AttackUp")
                && !animator.GetCurrentAnimatorStateInfo(0).IsName("AttackDown"))
        {
            SoundManager.instance.PlaySound(SoundManager.instance.audioAttack, 1f);
            animator.SetTrigger("Attack");
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBehavior : MonoBehaviour
{
    // hp bar
    public int healthMax = 100;
    public float hpBarOffsetY = 0f;
    public GameObject prefHpBar;
    GameObject canvas;
    RectTransform hpBar;
    Camera myCamera;
    Image nowHpbar;
    int health;

    // damage text
    public float textDamageOffsetY = 0f;
    public GameObject prefTextDamage;

    // move
    public float speed = 0.5f;
    public float sqrDistanceMove = 2.9f;
    public float sqrDistanceFollow = 1.4f;
    public float patrolDistanceX;
    public float patrolDistanceY;
    Vector3 targetPosition;
    Vector2 positionOriginal;
    bool isArrived;

    // attack
    public int power = 2;
    public bool isMeleeAttacker = true;
    public GameObject prefTrowingWeapon;
    public float intervalMaxThrowingWeapon = 2f;
    float intervalThrowingWeapon;
    float timeThrowingWeapon;
    
    // animation
    Animator animator;
    bool isLookingRight;
    bool isRunning;

    // 죽을 때 아이템 떨어뜨림
    public GameObject prefPotionSmall;
    public GameObject prefPotionBig;
    public GameObject prefCoin;
    public GameObject prefChest;
    public float probPotionSmall = 0.1f;
    public float probPotionBig = 0.01f;
    public float probCoin = 0.8f;

    // hit effect
    public GameObject effectHit;

    // score
    int point;

    // Summoner
    public bool isSummoner = false;
    public GameObject prefSummonedEnemy;
    GameObject summonedEnemy;

    PlayerMove player;

    enum State
    {
        Wait,
        Move,
        Follow,
        Return
    }

    State state;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        canvas = GameObject.Find("Canvas");

        // hp bar
        hpBar = Instantiate(prefHpBar, canvas.transform).GetComponent<RectTransform>();
        myCamera = GameObject.Find("Camera").GetComponent<Camera>();
        nowHpbar = hpBar.Find("Health").GetComponent<Image>();
        hpBar.gameObject.SetActive(false);

        player = FindObjectOfType<PlayerMove>();
        float powerFactor = GameManager.instance.enemyPowerFactor;
        healthMax = (int)(healthMax * powerFactor);
        health = healthMax;
        speed = speed * powerFactor;
        power = (int)(power * powerFactor);
        isArrived = false;
        state = State.Wait;
        isLookingRight = true;
        positionOriginal = transform.position;
        isRunning = false;
        targetPosition = transform.position;
        GetIntervalThrowingWeapon();
        point = (int) (((health + power * 10) * speed) / 100);
        if (point <= 0) point = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.state != GameManager.State.Play)
        {
            hpBar.gameObject.SetActive(false);
            return;
        }

        // hp bar
        Vector2 _hpBarPos = myCamera.WorldToScreenPoint(new Vector2(transform.position.x, transform.position.y + hpBarOffsetY));
        hpBar.position = _hpBarPos;
        hpBar.gameObject.SetActive(true);


        // 바라보고 있는 방향에 따른 스프라이트 회전
        if (isLookingRight)
        {
            transform.localScale = new Vector2(1, 1);
        }
        else
        {
            transform.localScale = new Vector2(-1, 1);
        }

        // 플레이어와의 거리에 따라 move or follow
        float sqrDistanceToPlayer = (transform.position - player.transform.position).sqrMagnitude;
        if (sqrDistanceToPlayer < sqrDistanceMove && state != State.Move && state != State.Return)
        {
            state = State.Move;
            isRunning = true;
        }
        else if (sqrDistanceToPlayer < sqrDistanceFollow && state != State.Follow && state != State.Return)
        {
            state = State.Follow;
        }

        if (state == State.Return)
        {
            ReturnPosition();
        }
        else if (state == State.Move)
        {
            Move();
        }
        else if (state == State.Follow)
        {
            Follow();
        }

        // 애니메이션
        animator.SetBool("Running", isRunning);
    }

    private void Move()
    {
        if (!isArrived)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, speed * Time.deltaTime);
            float sqrDistance = (transform.position - targetPosition).sqrMagnitude;
            if (sqrDistance < 0.01f)
            {
                isArrived = true;
            }
        }
        else
        {
            SetPatrolPosition(patrolDistanceX, patrolDistanceY);
            isArrived = false;
        }

        CheckLookingDirection(0);
    }

    

    void Follow()
    {
        // 근접 공격
        if (isMeleeAttacker)
        {
            CheckLookingDirection(0.5f);
            SetTargetPosition(player.transform.position.x, player.transform.position.y);
            transform.position = Vector3.Lerp(transform.position, targetPosition, speed * Time.deltaTime);   
        } 
        else
        {
            // 원거리 공격
            if (!isSummoner)
            {
                CheckLookingDirection(0f);
                ThrowWeapon();
            } 
            // summoner
            else
            {
                CheckLookingDirection(0.5f);
                if (!summonedEnemy) SummonEnemy();
            }
        }

        
    }

    void SummonEnemy()
    {
        summonedEnemy = Instantiate(prefSummonedEnemy, transform.position, transform.rotation);
    }

    void ThrowWeapon()
    {
        timeThrowingWeapon += Time.deltaTime;
        if (timeThrowingWeapon > intervalThrowingWeapon)
        {
            // 플레이어 방향을 봄
            float distanceX = player.transform.position.x - transform.position.x;
            if (distanceX > 0)
            {
                isLookingRight = true;
            }
            else
            {
                isLookingRight = false;
            }

            GetIntervalThrowingWeapon();
            Instantiate(prefTrowingWeapon, transform.position, transform.rotation);
        } 
    }

    void GetIntervalThrowingWeapon()
    {
        timeThrowingWeapon = 0f;
        intervalThrowingWeapon = Random.Range(0.1f, intervalMaxThrowingWeapon);
    }
    
    void ReturnPosition()
    {
        Move();

        if (isArrived)
        {
            state = State.Move;
        }
    }

    void CheckLookingDirection(float hysteresis)
    {
        float distanceX = transform.position.x - targetPosition.x;
        if (distanceX > hysteresis)
        {
            isLookingRight = false;
        }
        else if (distanceX < -hysteresis)
        {
            isLookingRight = true;
        }
    }

    public void SetPatrolPosition(float patrolX, float patrolY)
    {
        patrolDistanceX = patrolX;
        patrolDistanceY = patrolY;
        float randomX = Random.Range(transform.position.x - patrolX, transform.position.x + patrolX);
        float randomY = Random.Range(transform.position.y - patrolY, transform.position.y + patrolY);
        targetPosition = new Vector2(randomX, randomY);
    }

    public void SetTargetPosition(float x, float y)
    {
        targetPosition = new Vector2(x, y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 적이 플레이어의 무기에 맞음
        if (collision.gameObject.layer == LayerMask.NameToLayer("PlayerWeapon"))
        {
            //SoundManager.instance.PlaySound(SoundManager.instance.audioPunch, 0.5f);

            // hit effect
            Instantiate(effectHit, collision.contacts[0].point, transform.rotation);

            // 피해를 받음
            Weapon playerWeapon = collision.gameObject.GetComponent<Weapon>();
            int damage = Random.Range(playerWeapon.damageMin, playerWeapon.damageMax + player.power + 1);
            health -= damage;
            // 데미지 표시
            if (damage >= playerWeapon.damageMax * 0.9f)
            {
                SetFloatingText(damage, "ff0000");
            }
            else
            {
                SetFloatingText(damage, "ffffff");
            }

            // 죽음
            if (health <= 0)
            {
                // 죽음
                death();
            }

            // hp bar
            nowHpbar.fillAmount = (float)health / (float)healthMax;
        }

        // 적이 이동하다 벽에 부딪힘
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            SetTargetPosition(positionOriginal.x, positionOriginal.y);
            state = State.Return;
        }
    }

    void death()
    {
        // 죽을 때 아이템 떨어뜨림
        SpawnItem();

        // 플레이어 점수 ++
        player.AddExp(point);

        // GameObject 파괴
        Destroy(gameObject);
    }

    void SpawnItem()
    {
        float prob = Random.Range(0f, 1f);
        if (prob < probPotionBig)
        {
            Instantiate(prefPotionBig, transform.position, transform.rotation);
        }
        else if (prob < probPotionBig + probPotionSmall)
        {
            Instantiate(prefPotionSmall, transform.position, transform.rotation);
        }
        else if (prob < probPotionBig + probPotionSmall + probCoin)
        {
            Coin coin = Instantiate(prefCoin, transform.position, transform.rotation).GetComponent<Coin>();
            coin.price = point;
        }

        if (gameObject.tag == "Boss")
        {
            Instantiate(prefChest, transform.position, transform.rotation);
        }   
    }

    void SetFloatingText(int message, string color)     // color : 000000~ffffff
    {
        Text text = Instantiate(prefTextDamage, canvas.transform).GetComponent<Text>();
        Vector2 pos = myCamera.WorldToScreenPoint(new Vector2(transform.position.x, transform.position.y + textDamageOffsetY));
        text.transform.position = pos;
        text.text = "<color=#" + color + ">" + message + "</color>";
    }

    private void OnDestroy()
    {
        if (hpBar != null) Destroy(hpBar.gameObject);
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{

    public static PlayerController main;

    public Animator anim;
    private float incapacitatedTime;    // if > 0, Player cannot move, attack, nor use items

    [Header("Stats")]

    float totalHealth = 30;
    float currentHealth = 0;
    bool dead;

    public int mummiesKilled;

    public int level = 1;
    private Text levelText;
    public float experience { get; private set; }
    private Transform experienceBar;
    private Transform healthBar;

    [Header("Movement")]
    private bool canMove = true;
    private bool isJumping = false;
    public float movementSpeed;
    public float velocity;
    public float jumpStrength;
    public Rigidbody rb;

    [Header("Combat")]
    private List<Transform> enemiesInRange = new List<Transform>();
    private bool canAttack = true;
    private bool attacking;
    public float weaponDamage;
    public float bonusDamage;
    public float attackDamage;
    public float attackSpeed;
    public float attackRange;


    //	[Header("Build")]
    //character attributes are calculated with the stats + equipment + buffs
    public float strength;// {get; private set;}
    public float vitality { get; private set; }
    public float agility { get; private set; }
    public float intelligence { get; private set; }
    public float dexterity;// {get; private set;}
    public float cunningness { get; private set; }

    // Use this for initialization
    void Start()
    {
        if (main == null) main = this;

        AnimationEvents.OnSlashAnimationHit += DealDamage;
        AnimationEvents.OnJumpAnimationJump += JumpCallback;
        experienceBar = UIManager.instance.canvas.Find("Character Info/Experience");
        healthBar = UIManager.instance.canvas.Find("Character Info/Health");
        levelText = UIManager.instance.canvas.Find("Character Info/Level_Text").GetComponent<Text>();
        SetExperience(0);
        SetHealth(totalHealth);
        SetAttackDamage();
    }

    void SetAttackDamage()
    {
        attackDamage = GameLogic.CalculatePlayerBaseAttackDamage(this) + weaponDamage + bonusDamage;
    }

    // Update is called once per frame
    void Update()
    {
        if (incapacitatedTime > 0) return;
        GetInput();
        Move();
    }

    void GetInput()
    {
        if (Input.GetMouseButtonUp(0))
        {

        }
        //move left
        if (Input.GetKey(KeyCode.A))
        {
            SetVelocity(-1);
        }
        else if (Input.GetKeyUp(KeyCode.A))
        {
            SetVelocity(0);
            anim.SetInteger("Condition", 0);
        }
        //move right
        if (Input.GetKey(KeyCode.D))
        {
            SetVelocity(1);
        }
        else if (Input.GetKeyUp(KeyCode.D))
        {
            SetVelocity(0);
            anim.SetInteger("Condition", 0);
        }
        //Jump
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W))
        {
            Jump();
        }
    }
    #region MOVEMENT
    void Move()
    {
        if (velocity == 0)
        {
            //			anim.SetInteger("Condition", 0);
            return;
        }
        else
        {
            //we can only move if we are not doing something that doesn't allow us to move.
            if (canMove)
            {
                if (!isJumping)
                {
                    anim.SetInteger("Condition", 1);
                    rb.MovePosition(transform.position + (Vector3.right * velocity * movementSpeed * Time.deltaTime));
                }
                else
                { //if we are indeed jumping
                    rb.MovePosition(transform.position + (Vector3.right * velocity * movementSpeed / 5 * Time.deltaTime));
                }
            }
        }

    }
    void SetVelocity(float dir)
    {
        //Look left or right depending on the (- +) of dir.
        if (dir < 0) transform.LookAt(transform.position + Vector3.left);
        else if (dir > 0) transform.LookAt(transform.position + Vector3.right);
        velocity = dir;
    }

    void Jump()
    {
        rb.velocity = transform.forward * Mathf.Abs(velocity) * 4;  //Keep momentum when Jumping
        rb.useGravity = true;
        anim.SetInteger("Condition", 5);
        isJumping = true;
    }

    void JumpCallback()
    {
        rb.AddForce(Vector3.up * jumpStrength);
    }
    #endregion

    #region COMBAT
    public void Attack()
    {
        if (!canAttack) return; //Changed 
        anim.speed = attackSpeed;
        anim.SetInteger("Condition", 2);
        StartCoroutine(AttackRoutine());
        StartCoroutine(AttackCooldown()); //new
    }

    void DealDamage()
    {
        print("deal damage!");
        GetEnemiesInRange();
        foreach (Transform enemy in enemiesInRange)
        {
            EnemyController ec = enemy.GetComponent<EnemyController>();
            if (ec == null) continue;
            ec.GetHit(attackDamage);
        }
    }

    IEnumerator AttackRoutine()
    {
        canMove = false;
        yield return new WaitForSeconds(0.1f);
        anim.SetInteger("Condition", 0);
        yield return new WaitForSeconds(1 / attackSpeed);
        canMove = true;
    }

    IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(1 / attackSpeed);
        canAttack = true;
        canMove = true;
        anim.speed = 1;
    }

    void GetEnemiesInRange()
    {
        enemiesInRange.Clear();
        foreach (Collider c in Physics.OverlapSphere((transform.position + transform.forward * attackRange), attackRange))
        {
            if (c.gameObject.CompareTag("Enemy"))
            {
                enemiesInRange.Add(c.transform);
            }
        }
    }
    #endregion

    /// <summary>
    /// Add exp to the player's current experience. Call level up if necessary.
    /// </summary>
    /// <param name="exp">Exp.</param>
    public void SetExperience(float exp)
    {
        experience += exp;
        float experienceNeeded = GameLogic.ExperienceForNextLevel(level);
        float previousExperience = GameLogic.ExperienceForNextLevel(level - 1);
        //Level Up
        while (experience >= experienceNeeded)
        {
            LevelUp();
            experienceNeeded = GameLogic.ExperienceForNextLevel(level);
            previousExperience = GameLogic.ExperienceForNextLevel(level - 1);
        }
        experienceBar.Find("Fill").GetComponent<Image>().fillAmount = (experience - previousExperience) / (experienceNeeded - previousExperience);
    }

    void LevelUp()
    {
        level++;
        levelText.text = "Lv. " + level.ToString("00");
        StartCoroutine(ShowLevelUpTextRoutine());
    }

    IEnumerator ShowLevelUpTextRoutine()
    {
        UIManager.instance.levelUpText.gameObject.SetActive(true);
        Vector2 positionOfText = UIManager.instance.WorldToCanvasPoint(transform.position + (Vector3.up * 2));
        UIManager.instance.levelUpText.GetComponent<RectTransform>().anchoredPosition = positionOfText;
        yield return new WaitForSeconds(1.5f);
        UIManager.instance.levelUpText.gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Floor")) StartCoroutine(OnLandRoutine());
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Floor")) OnLeaveFloor();
    }

    void OnLeaveFloor()
    {
        anim.SetInteger("Condition", 7); //Set animation to Mid Air
        isJumping = true;
        rb.useGravity = true;
    }

    IEnumerator OnLandRoutine()
    {
        Debug.Log("Landed");
        anim.SetInteger("Condition", 6);
        yield return new WaitForSeconds(0.01f);
        anim.SetInteger("Condition", 0);
        isJumping = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    private void GetIncapacitated(float time)
    {
        if (incapacitatedTime < time)
        {
            StopCoroutine("GetIncapacitatedRoutine");
            incapacitatedTime = time;
            StartCoroutine("GetIncapacitatedRoutine");
        }
    }

    public void GetHealed(float amount)
    {
        if (dead) return;
        SetHealth(currentHealth + amount);
    }

    public void GetHit(float damage)
    {
        if (dead) return;
        anim.SetInteger("Condition", 3);
        SetHealth(currentHealth - damage);
        GetIncapacitated(0.2f);
    }

    public void SetHealth(float health)
    {
        if (health > totalHealth) currentHealth = totalHealth;
        else if (health <= 0)
        {
            Die();
            currentHealth = 0;
        }
        else currentHealth = health;
        healthBar.Find("Fill").GetComponent<Image>().fillAmount = currentHealth / totalHealth;
        healthBar.Find("Text").GetComponent<Text>().text = currentHealth + "/" + totalHealth;
    }

    void Die()
    {
        dead = true;
        anim.SetInteger("Condition", 4);
        //Show Options

    }

    IEnumerator GetIncapacitatedRoutine()
    {
        while (incapacitatedTime > 0)
        {
            yield return new WaitForSeconds(0.1f);
            incapacitatedTime -= 0.1f;
        }
    }

}

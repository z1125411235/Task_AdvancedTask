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

    public float totalHealth = 30f;
    float currentHealth = 0;

    public float totalSP = 50f;
    float currentSP= 0;
    bool dead = false;

    public int mummiesKilled;

    public int level = 1;
    private Text levelText;
    public float experience { get; private set; }
    public Transform experienceBar;
    private Transform healthBar;
    private Transform StaminaBar;

    [Header("Movement")]
    private bool canMove = true;
    private bool isJumping = false;
    public float movementSpeed;
    public float velocity;
    public float jumpStrength = 500;
    public Rigidbody rb;

    [Header("Combat")]
    private List<Transform> enemiesInRange = new List<Transform>();
    private bool canAttack = true;
    public float weaponDamage = 5f;
    public float bonusDamage = 3f;
    public float attackDamage = 20f;
    public float attackSpeed = 1.5f;
    public float attackRange = 0.5f;


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
        experienceBar = UIManager.instance.canvas.Find("PlayerInfor/Experience");
        healthBar = UIManager.instance.canvas.Find("PlayerInfor/hitpoint");
        StaminaBar = UIManager.instance.canvas.Find("PlayerInfor/Stamina");
        levelText = UIManager.instance.canvas.Find("PlayerInfor/LevelText").GetComponent<Text>();
        SetExperience(0);
        SetHealth(totalHealth);
        SetAttackDamage();
        SetSp(totalSP);

        StartCoroutine(regenerationSP());
    }

    void SetAttackDamage()
    {
        attackDamage = GameLogic.CalculatePlayerBaseAttackDamage(this) + weaponDamage + bonusDamage;
    }

    
    // Update is called once per frame
    void Update()
    {
        if (incapacitatedTime > 0 || dead) return;
        GetInput();
        Move();
        
    }

    IEnumerator regenerationSP () {
        while (!dead)
        {
            yield return new WaitForSeconds(0.1f);
            if (currentSP <=totalSP)
            SetSp( currentSP += 0.55f);


        }
        
    }

    void GetInput()
    {
        if (currentSP < 10)
            canAttack = false;
        else
            canAttack = true;

        if (Input.GetMouseButtonUp(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                //Check if is an NPC
                NPCController npcController = hit.transform.GetComponent<NPCController>();
                if (npcController != null)
                {
                    //npcController.ShowDialogue();
                    //npcController.dialogueIndex++;
                    npcController.OnClick();
                    Debug.Log("npc onclick");
                    return; //avoid doing attack
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            Attack();
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
            {//changed
                if (!isJumping) anim.SetInteger("Condition", 1);
                rb.MovePosition(transform.position + (Vector3.right * velocity * movementSpeed * Time.deltaTime));
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
        //		rb.constraints = RigidbodyConstraints.None;
        //		rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.constraints = RigidbodyConstraints.FreezePositionX;
        rb.constraints = RigidbodyConstraints.FreezePositionZ;
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
    void Attack()
    {
        if (!canAttack) return; //Changed 
        anim.speed = attackSpeed;
        anim.SetInteger("Condition", 2);
        SetSp(currentSP - 10);
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
        experienceBar.Find("fill").GetComponent<Image>().fillAmount = (experience - previousExperience) / (experienceNeeded - previousExperience);
        experienceBar.Find("showValue").GetComponent<Text>().text = experience.ToString() + "/" + experienceNeeded.ToString();
    }

    void LevelUp()
    {
        level++;
        levelText.text = "Lv. " + level.ToString("00");
        SetHealth(totalHealth = GameLogic.CalculatePlayerTotalHealth(this));
        SetSp(totalSP =  GameLogic.CalculatePlayerTotalStamina(this));
        StartCoroutine(ShowLevelUpTextRoutine());
    }

    IEnumerator ShowLevelUpTextRoutine()
    {
        UIManager.instance.LevelUPText.gameObject.SetActive(true);
        Vector2 positionOfText = UIManager.instance.WorldToCanvansPoint(transform.position + (Vector3.up * 2));
        UIManager.instance.LevelUPText.GetComponent<RectTransform>().anchoredPosition = positionOfText;

        yield return new WaitForSeconds(2);

        UIManager.instance.LevelUPText.gameObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision other)
    {
        StartCoroutine(OnLandRoutine(other));
    }

    IEnumerator OnLandRoutine(Collision other)
    {
        if (other.gameObject.CompareTag("Floor"))
        {
            anim.SetInteger("Condition", 6);
            yield return new WaitForSeconds(0.05f);
            anim.SetInteger("Condition", 0);
            isJumping = false;
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }
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

    public void GetHit(float damage)
    {
        if (dead) return;
        anim.SetInteger("Condition", 3);
        SetHealth(currentHealth - damage);
        GetIncapacitated(0.5f);

    }

    void SetHealth(float health)
    {
        if (health > totalHealth) currentHealth = totalHealth;
        else if (health <= 0)
        {
            Die();
            currentHealth = 0;
        }
        else currentHealth = health;
        healthBar.Find("fill").GetComponent<Image>().fillAmount = currentHealth / totalHealth;
        healthBar.Find("Text").GetComponent<Text>().text = currentHealth + "/" + totalHealth;
    }

    void SetSp(float sp)
    {
        if (sp > totalSP) currentSP = totalSP;
        else if (sp <= 0)
        {
            currentSP = 0;
        }
        else currentSP = sp;
        StaminaBar.Find("fill").GetComponent<Image>().fillAmount = currentSP / totalSP;
        StaminaBar.Find("Text").GetComponent<Text>().text = currentSP.ToString("0") + "/" + totalSP;
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

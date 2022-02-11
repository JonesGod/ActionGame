using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    private CharacterController characterController;
    public FolowCamera followCamera;

    private Animator m_Am;
    private PlayerInput m_Input; //準備獲取玩家輸入

    public float playerHp = 100;//玩家生命
    public float playerMaxHp = 100;//玩家最大生命
    private float rotateSpeed = 10.0f;//轉向速度
    private float speed = 6.0f;//移動速度
    private float gravity = 20.0f;//重力
    private float rollSpeed = 10.0f;//翻滾速度
    private float statetime;//動畫進行時間(百分比)
    private float fallSpeed;//角色落下速度
    private float mouseSlide;//滑鼠滑動輸入
    private float normalMove;//一般狀態下的WASD輸入總和
    private float bowRightMove;//弓狀態下的WASD輸入總和
    private float lockDistancs = 5.0f;//一般攻擊自動鎖定距離
    private float attackMoveSpeed;//攻擊時的移動速度

    private List<FSMBase> monster;//存取怪物資訊

    public int sensitivity=12;//弓狀態下的滑鼠控制相機靈敏度

    AnimatorStateInfo stateinfo;//當前Animation存取
    AnimatorStateInfo nextStateinfo;//下個Animation存取
    AnimatorStateInfo nextStateinfoOne;//第1層的下個Animation存取

    readonly int hashAttack01 = Animator.StringToHash("attack01");
    readonly int hashAttack02 = Animator.StringToHash("attack02");
    readonly int hashAttack03 = Animator.StringToHash("attack03");
    readonly int hashAttack04 = Animator.StringToHash("attack04");
    readonly int hashSpecialAttack=Animator.StringToHash("specialAttack");
    readonly int hashSpecialAttack1_1 = Animator.StringToHash("specialAttack1_1");
    readonly int hashSpecialAttack1_2 = Animator.StringToHash("specialAttack1_2");
    readonly int hashSpecialAttack2_1 = Animator.StringToHash("specialAttack2_1");
    readonly int hashSpecialAttack2_2 = Animator.StringToHash("specialAttack2_2");
    readonly int hashSpecialAttack2_3 = Animator.StringToHash("specialAttack2_3");
    readonly int hashRoll=Animator.StringToHash("Roll");
    readonly int hashIdle= Animator.StringToHash("Idle");
    readonly int m_StateTime = Animator.StringToHash("StateTime");
    readonly int hashBowIdle = Animator.StringToHash("BowIdle");
    readonly int hashBowShoot = Animator.StringToHash("BowShoot");
    readonly int hashHurt = Animator.StringToHash("Hurt");
    readonly int hashDead = Animator.StringToHash("dead");
    readonly int hashGetup = Animator.StringToHash("getup");

    /// 動畫播放狀態
    private bool attackState;//所有一般攻擊Animation
    private bool rollState;//翻滾Animation
    private bool idleIsNext;//下個Animation是Idle
    private bool rollIsNext;//下個Animation是翻滾
    private bool isTrasition;//混接中
    private bool bowIsNext;//下個Animation是弓
    private bool bowShoot;//射擊動作
    private bool hurt;//受傷動作

    public bool dead;//死亡動畫
    public bool getup;//起身動畫

    Vector3 move = Vector3.zero;//角色總移動量
    Vector3 targetVector;//自動鎖定的方向

    Vector2 moveInput;//存取按鍵WASD，主要用在轉向，不太需要管Input.GetAxis的數值變化
    Vector2 runInput;//存取WASD，需要Input.GetAxis的數值變化來用在blend tree

    Relive reliveObserver;//負責復活的觀察者
    public enum PlayerState
    {
        live=1,
        dead=2,
    }
    void Start()
    {
        characterController = GetComponent<CharacterController>();    
       
        m_Am = GetComponent<Animator>();
        m_Input = GetComponent<PlayerInput>();

        monster = new List<FSMBase>();
        GameObject[] allMonster =GameManager.Instance.allMonster;//將場景裡tag為Monster的物件存起來
       if(allMonster!=null || allMonster.Length>0)
       {           
            foreach(GameObject m in allMonster)
            {
                monster.Add(m.GetComponent<FSMBase>());
            }
       }
        playerCurrnetState = PlayerState.live;
    }
    void Update()
    {
        BowAngle();
        TargetSearch();

        moveInput = PlayerInput.Instance.MoveInput;
        runInput = PlayerInput.Instance.MoveInput;
    }
    void FixedUpdate()
    {
        stateinfo = m_Am.GetCurrentAnimatorStateInfo(0);
        nextStateinfo = m_Am.GetNextAnimatorStateInfo(0);
        isTrasition = m_Am.IsInTransition(0);
        nextStateinfoOne= m_Am.GetNextAnimatorStateInfo(1);
        
        m_Am.SetFloat(m_StateTime, Mathf.Repeat(m_Am.GetCurrentAnimatorStateInfo(0).normalizedTime, 1f));//讓statetime不斷從0數到1
        statetime = m_Am.GetFloat("StateTime");

        CalculateGravity();

        if (m_Input.bowState)   //弓狀態與一般狀態的基本參數改變
        {
            BowBasicValue();
        }
        else
        {
            NormalBasicValue();
        }

        CantRollToBow();
        if (m_Input.bowState && !attackState)
        {
            m_Am.SetBool("BowBool", true);
        }
        else
        {
            m_Am.SetBool("BowBool", false);
        }

        if (m_Input.moveFlag)
        {
            if(statetime<=0.4f)
                ResetAttackTrigger();

            m_Am.SetBool("RunBool", true);
        }
        else
        {
            moveInput = Vector2.zero;
            m_Am.SetBool("RunBool", false);
        }

        ResetTrigger();
        if (m_Input.avoid )      //迴避
        {   
            ResetAttackTrigger();         

            if (!(statetime <= 0.5f && attackState) && !isTrasition && !rollState)
            {
                RollRotating(moveInput.x, moveInput.y);
            }
            
            m_Am.SetTrigger("AvoidTrigger");
            PlayerInput.Instance.bowState = false;
            m_Input.avoid = false;
        }
        if (m_Input.attack)   //左鍵攻擊
        {
            m_Am.ResetTrigger("SpecialAttackTrigger");
            m_Am.SetTrigger("AttackTrigger");
            m_Input.attack = false;
        }
        if(m_Input.specialAttack)
        {
            m_Am.ResetTrigger("AttackTrigger");
            m_Am.SetTrigger("SpecialAttackTrigger");
            m_Input.specialAttack = false;
        }
        
        GetAttackState();
        GetCurrentState();
        GetNextState();
        if (m_Input.moveFlag && !attackState && !rollState && !rollIsNext)
            Rotating(moveInput.x, moveInput.y);
        
    }
    void OnAnimatorMove()
    {
 
        RaycastHit hit;
        Ray ray = new Ray(transform.position + Vector3.up, -Vector3.up);//在林克身上做一條與Y軸平行的雷射用以偵測四周
        if (Physics.Raycast(ray, out hit, 1.0f, Physics.AllLayers))
        {
            move = Vector3.ProjectOnPlane(m_Am.deltaPosition, hit.normal);
        }

        if (!attackState || rollIsNext)//讀取按鍵決定方向
            move = followCamera.horizontalVector * moveInput.y + followCamera.cameraRight * moveInput.x;
        else
            move = Vector3.zero;

        if (rollState || rollIsNext)//翻滾時採用翻滾速度
            move = transform.forward * rollSpeed * Time.deltaTime;
        else
            move = Vector3.Normalize(move) * speed * Time.deltaTime;

        if (idleIsNext || bowIsNext)//轉換到Idle與弓狀態時減速
            move = transform.forward * 0.0f;

        if (hurt)//受傷時移動量為0
            move = Vector3.zero;

        move += fallSpeed * Vector3.up * Time.deltaTime;//加上落下速度
        move += m_Am.deltaPosition;//加上美術位移
        move += transform.forward*attackMoveSpeed*Time.deltaTime;//加上攻擊時的移動速度

        characterController.Move(move);
    }
    /// <summary>
    /// 重力計算
    /// </summary>
    void CalculateGravity()
    {        
        if (characterController.isGrounded)
        {
            fallSpeed = -gravity * 0.3f;
        }
        else
        {
            fallSpeed -= gravity * Time.deltaTime;
        }
    }
    /// <summary>
    /// 一般轉向
    /// </summary>
    /// <param name="moveH"></param>
    /// <param name="moveV"></param>
    void Rotating(float moveH, float moveV)
    {
        // 建立角色目標方向的向量                  
        Vector3 newDirectionVector = followCamera.horizontalVector * moveV + followCamera.cameraRight * moveH;       
        if(newDirectionVector != Vector3.zero)
        {
            Quaternion newRotation = Quaternion.LookRotation(newDirectionVector, Vector3.up);
            characterController.transform.rotation = Quaternion.Lerp(characterController.transform.rotation, newRotation, Time.deltaTime * rotateSpeed);
        }    
    }
    /// <summary>
    /// 以瞬間轉向為主的翻滾轉向
    /// </summary>
    /// <param name="moveH"></param>
    /// <param name="moveV"></param>
    void RollRotating(float moveH, float moveV)
    {
        if (moveV > 0)          ///此處的moveV與moveH只需取最大值作為翻滾方向判斷來使用
            moveV = 1;
        else if (moveV < 0)
            moveV = -1;
        if (moveH > 0)
            moveH = 1;
        else if (moveH < 0)
            moveH = -1;

        Vector3 newDirectionVector = (followCamera.horizontalVector * moveV + followCamera.cameraRight * moveH).normalized;
        if (newDirectionVector != Vector3.zero)
        {
            Quaternion newRotation = Quaternion.LookRotation(newDirectionVector, Vector3.up);
            characterController.transform.rotation = newRotation;
        }
    }
    /// <summary>
    /// 在Animation的event使用
    /// 一般攻擊時的自動轉向
    /// </summary>
    void AttackRotating()
    {
        targetVector.y = 0f;
        Quaternion newRotation= Quaternion.LookRotation(targetVector, Vector3.up);        
        characterController.transform.rotation = newRotation;
    }
    /// <summary>
    /// 將所有一般攻擊狀態取出，來判斷是否在一般攻擊中
    /// </summary>
    void GetAttackState()
    {                                 
        if(stateinfo.shortNameHash == hashAttack01 ||
           stateinfo.shortNameHash == hashAttack02 ||
           stateinfo.shortNameHash == hashAttack03 ||
           stateinfo.shortNameHash == hashAttack04 ||
           stateinfo.shortNameHash == hashSpecialAttack ||
           stateinfo.shortNameHash == hashSpecialAttack1_1 ||
           stateinfo.shortNameHash == hashSpecialAttack1_2 ||
           stateinfo.shortNameHash == hashSpecialAttack2_1 ||
           stateinfo.shortNameHash == hashSpecialAttack2_2 ||
           stateinfo.shortNameHash == hashSpecialAttack2_3 ||
           nextStateinfo.shortNameHash== hashAttack01)
        {
            attackState = true;
        }
        else        
            attackState = false;

        PlayerInput.Instance.attackState = attackState;
        Sword.Instance.attackState = attackState;
    }
    /// <summary>
    /// 獲取當前Animation
    /// </summary>
    void GetCurrentState()
    {        
        if (stateinfo.shortNameHash == hashRoll)
            rollState = true;
        else
            rollState = false;

        if (nextStateinfoOne.shortNameHash == hashBowShoot)
            bowShoot = true;
        else
            bowShoot = false;

        if (stateinfo.shortNameHash == hashHurt)
            hurt = true;
        else
            hurt = false;

        if (stateinfo.shortNameHash == hashDead)
            dead = true;
        else
            dead = false;

        PlayerInput.Instance.rollState = rollState;
        PlayerInput.Instance.bowShoot = bowShoot;
    }    
    /// <summary>
    /// 獲取下個Animation
    /// </summary>
    void GetNextState()
    {
        if (nextStateinfo.shortNameHash == hashRoll)
            rollIsNext = true;
        else
            rollIsNext = false;

        if (nextStateinfo.shortNameHash == hashIdle)
            idleIsNext = true;
        else
            idleIsNext = false;

        if (nextStateinfo.shortNameHash == hashBowIdle)
            bowIsNext = true;
        else
            bowIsNext = false;

        PlayerInput.Instance.rollIsNext = rollIsNext;
    }
    /// <summary>
    /// 重製迴避觸發
    /// </summary>
    void ResetTrigger()
    {      
        m_Am.ResetTrigger("AvoidTrigger");
    }
    /// <summary>
    /// 重製攻擊觸發
    /// </summary>
    void ResetAttackTrigger()
    {
        m_Am.ResetTrigger("AttackTrigger");
        m_Am.ResetTrigger("SpecialAttackTrigger");
    }
    /// <summary>
    /// 控制角色弓狀態下的上下角度
    /// </summary>
    void BowAngle()
    {
        mouseSlide-=PlayerInput.Instance.MouseInput.y*sensitivity;
        if (mouseSlide > 250f)
            mouseSlide = 250f;
        else if (mouseSlide < -240f)
            mouseSlide = -240f;

        m_Am.SetFloat("BowAngle",mouseSlide+500f);
    }
    /// <summary>
    /// 一般狀態下的基礎參數
    /// </summary>
    void NormalBasicValue()
    {
        normalMove = (Mathf.Abs(runInput.x) + Mathf.Abs(runInput.y)) * 2;
        if (normalMove > 1f)
            normalMove = 1f;
        m_Am.SetFloat("RunBlend", Mathf.Abs(normalMove));

        speed = 10.0f;
    }
    /// <summary>
    /// 弓狀態下的基礎參數
    /// </summary>
    void BowBasicValue()
    {
        normalMove = runInput.y;
        bowRightMove = runInput.x;

        m_Am.SetFloat("BowTotalMoveInput",Mathf.Abs(normalMove + bowRightMove));
        m_Am.SetFloat("RunBlend", normalMove);
        m_Am.SetFloat("RightRunBlend", bowRightMove);
        
        speed = 4.0f;
    }
    /// <summary>
    /// 時翻滾不能馬上切換到弓狀態
    /// </summary>
    void CantRollToBow()
    {
        if(statetime<0.5f && rollState)
            PlayerInput.Instance.rollToBow = false;
        else
            PlayerInput.Instance.rollToBow = true;
    }
    /// <summary>
    /// 尋找距離小於3.0f的最短距離目標
    /// </summary>
    void TargetSearch()
    {
        Vector3 vec;
        Vector3 lastVec=transform.forward;
        float last = 100.0f;//比鎖定距離還長的隨意數值用來給迴圈的第一圈比較用
        for (int i = 0; i < monster.Count; i++)
        {
            vec = monster[i].transform.position - transform.position; //獲得鎖定的方向
            if (vec.magnitude >= lockDistancs || (monster[i].currentState == FSMState.Dead))
            {               
                continue;
            }
            if (vec.magnitude < last)
            {
                lastVec = vec;
                last = vec.magnitude;
            }
        }
        targetVector = lastVec;
    }
    /// <summary>
    /// 玩家受傷
    /// </summary>
    public void PlayerHurt(int damage)
    {
        playerHp -= damage;
        if (playerHp >= playerMaxHp)
            playerHp = playerMaxHp;
        if (playerHp <= 0)
        {
            playerHp = 0;
            playerCurrnetState = PlayerState.dead;
            //PlayerInput.Instance.playerCurrnetState= (PlayerInput.PlayerState)PlayerState.dead;
            m_Am.SetTrigger("dead");
        }
        else
        {
            m_Am.SetTrigger("HurtTrigger");
        }
        
        UIMain.Instance().UpdateHpBar(playerHp / 100.0f);

        PlayerInput.Instance.bowState = false;
    }
    void PlayerRelive()
    {
        m_Am.SetTrigger("getup");
    }
    /// <summary>
    /// 開始攻擊中移動
    /// 給event使用
    /// </summary>
    void AttackMove()
    {
        attackMoveSpeed = 5f;
    }
    /// <summary>
    /// 結束攻擊中移動
    /// </summary>
    void AttackMoveStop()
    {
        attackMoveSpeed = 0f;
    }
    /// <summary>
    /// 儲存要給誰觀察
    /// </summary>
    /// <param name="ob"></param>
    public void Subscribe(Relive ob)
    {
        reliveObserver = ob;
    }
    /// <summary>
    /// 資料發生改變時要做的行動
    /// </summary>
    protected void Notify()
    {
        reliveObserver.DeadProcess();    
    }
    /// <summary>
    /// 玩家生死狀態設定
    /// </summary>
    public PlayerState playerCurrnetState
    {
        set 
        { 
                Notify(); 
        }
        get { return playerCurrnetState; }
    }
}

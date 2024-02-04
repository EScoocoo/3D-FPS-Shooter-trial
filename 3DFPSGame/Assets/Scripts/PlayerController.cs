using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
public class PlayerController : MonoBehaviour
{
    [SerializeField] private CharacterController characterController;

    public float speed = 20f;
    public float gravity = -14f;
    public int playerHealth = 100;

    private Vector3 gravityVector;
    //GroundCheck
    public Transform groundCheckPoint;
    public float groundCheckRadius=0.35f;
    public LayerMask groundLayer;
    public bool isOnGround=false;

    //UI
    public Slider healthSlider;
    public Text healthText;
    private GameManager gameManager;
    public CanvasGroup damageScreenUI;

    //SoundFX
    public AudioSource playerHurtSound;
    



    public float jumpSpeed = 5f;
    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        gameManager = FindObjectOfType<GameManager>();
        damageScreenUI.alpha = 0;
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
        GroundCheck();
        JumpAndGravity();
        DamageScreenCleaner();


    }
    void MovePlayer()
    {
        Vector3 moveVector = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;

        characterController.Move(moveVector * speed * Time.deltaTime);
    }
    void GroundCheck()
    {
        isOnGround = Physics.CheckSphere(groundCheckPoint.position, groundCheckRadius, groundLayer);
    }
    void JumpAndGravity()
    {
        gravityVector.y += gravity * Time.deltaTime;

        characterController.Move(gravityVector * Time.deltaTime);

        if (isOnGround = true && gravityVector.y < 0)
        {
            gravityVector.y = -3f;
        }
        if (Input.GetButtonDown("Jump") && isOnGround)
        {
            gravityVector.y = Mathf.Sqrt(jumpSpeed * -2f * gravity);
        }
    }
    public void PlayerTakeDamage(int damageAmount)
    {
        playerHealth -= damageAmount;
        healthSlider.value -= damageAmount;
        HealthTextUpdate();
        damageScreenUI.alpha = 1f;
        playerHurtSound.Play();

        if(playerHealth<=0)
        {
            PlayerDeath();
            HealthTextUpdate();
            healthSlider.value = 0;
        }

    }
    void PlayerDeath()
    {
        //SceneManagerdan sahneyi tekrar yükle veya bir gamemanager yapýp oyunu yeniden baþlatma görevini oraya tayabiliriz
        gameManager.RestartGame();
    }

    void HealthTextUpdate()
    {
        healthText.text = playerHealth.ToString();
    }
    void DamageScreenCleaner()
    {
        if(damageScreenUI.alpha>0)
        {
            damageScreenUI.alpha -= Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("EndTrigger"))
        {
            gameManager.WinLevel();
        }
    }
}

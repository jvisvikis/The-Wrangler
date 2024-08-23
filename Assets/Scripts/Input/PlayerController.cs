using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public float pullStrength;
    [SerializeField] private Collider2D playerCollider;
    [SerializeField] private LassoBelt lassoBelt;
    [SerializeField] private LayerMask merchantMask;
    [SerializeField] private Transform wrangleCam;
    [SerializeField] private float lassoRange = 5f;
    [SerializeField] private float lassoChargeTime = 1f;
    [SerializeField] private float followSharpness = 2f;
    [SerializeField] private float followDistance = 2f; 
    [SerializeField] private float pullTime;
    [SerializeField] private float speed = 2f;
    private Controls playerControls;
    private Merchant merchant;
    private Rigidbody2D rb2d;
    private PlayerWorldUI playerWorldUI;
    private SwitchCamera switchCam;
    private float pullTimer;
    private float moveCounter;
    
    private float lassoTimer;
    private int timesPulled;
    private bool lassoing;
    private bool bringingBackLasso;
    private bool bringingAnimalBack;
    private bool animalFollowing;
    

    void Awake()
    {
        playerControls = new Controls();
        playerControls.Player.Wrangle.performed += ctx => Wrangle();
        playerControls.Player.Wrangle.canceled += ctx => ReleaseLasso();
        playerControls.Player.Release.performed += ctx => ReleaseAnimal();

        lassoBelt = FindObjectOfType<LassoBelt>();
        merchant = FindObjectOfType<Merchant>();
        rb2d = GetComponent<Rigidbody2D>();
        playerWorldUI = GetComponent<PlayerWorldUI>();
        switchCam = GetComponent<SwitchCamera>();

        playerWorldUI.SetCanvas(false);
        playerWorldUI.FillPullBar(0f);
    }

    void OnEnable()
    {
        playerControls.Enable();
    }

    void OnDisable()
    {
        playerControls.Disable();
    }

    void Update()
    {
        if(!lassoing) rb2d.velocity = GetDirection() * speed;
        else rb2d.velocity = Vector2.zero;

        if(bringingAnimalBack && pullTimer <= 0) 
        {
            AnimalEscaped();
            switchCam.SwitchPriority();
        }
        else pullTimer -= Time.deltaTime;

    }

    public Vector2 GetDirection()
    {
        return playerControls.Player.Move.ReadValue<Vector2>();
    }

    public Vector2 GetLassoPosition()
    {
        return (Vector2)lassoBelt.GetFreeLasso().transform.position;
    }

    public List<string> GetAnimalNames()
    {
        return lassoBelt.GetAnimalNames();
    }

    private void Wrangle()
    {
        if(!bringingBackLasso && lassoBelt.GetFreeLasso() != null) StartCoroutine(ChargeLasso());
        if(bringingAnimalBack) 
        {
            moveCounter -= pullStrength;
            if(moveCounter <= 0)
            {
                pullTimer = pullTime;
                moveCounter = lassoBelt.GetFreeLasso().animal.moveNum;
                timesPulled++;
                StartCoroutine(BringAnimalBack(0.25f));
            }
            playerWorldUI.FillPullBar(1f - moveCounter/lassoBelt.GetFreeLasso().animal.moveNum);
        }
    }

    public void AnimalEscaped()
    {
        if(lassoBelt.GetFreeLasso().animal != null)
        {
            playerWorldUI.SetCanvas(false);
            playerWorldUI.FillPullBar(0f);
            animalFollowing = false;
            bringingAnimalBack = false;
            lassoBelt.GetFreeLasso().ReleaseAnimal();
            lassoBelt.GetFreeLasso().transform.parent = lassoBelt.transform.parent;
            StartCoroutine(BringLassoBack(0.5f,0.25f));
        }
    }

    public void ReleaseAnimal()
    {
        if(lassoBelt.GetLastInUse() != null)
        {
            if(playerCollider.IsTouchingLayers(merchantMask)) 
                merchant.TakeAnimal(lassoBelt.GetLastInUse().animal);

            lassoBelt.ReleaseLast();
            lassoBelt.GetFreeLasso().transform.parent = lassoBelt.transform.parent;
            timesPulled = 0;
            bringingAnimalBack = false;
            StartCoroutine(BringLassoBack(0.5f,0.25f));
        }
    }

    private void ReleaseLasso()
    {
        lassoing = false;
        if(!bringingBackLasso && lassoBelt.GetFreeLasso().animal != null && !animalFollowing) 
        {
            bringingAnimalBack = true;
            bringingBackLasso = true;
            lassoBelt.BringAnimal(transform.position-lassoBelt.GetFreeLasso().transform.position);
            moveCounter = lassoBelt.GetFreeLasso().animal.moveNum;
            lassoBelt.GetFreeLasso().isWrangling = true;
            pullTimer = pullTime;
            switchCam.SwitchPriority();
            Vector2 point = GetMidPoint(Vector2.zero, (Vector2)lassoBelt.GetFreeLasso().transform.localPosition);
            wrangleCam.localPosition = new Vector3(point.x, point.y, -10); 
            playerWorldUI.SetCanvas(true);
            playerWorldUI.FillPullBar(0f);
        }
        if(!bringingBackLasso && lassoBelt.GetFreeLasso().animal == null) StartCoroutine(BringLassoBack(0.5f, 0.25f));
    }

    private Vector2 GetMidPoint(Vector2 start, Vector2 end)
    {
        float x = (end.x - start.x)/2;
        float y = (end.y - start.y)/2;
        return new Vector2(x,y);
    }

    private IEnumerator ChargeLasso()
    {
        lassoing = true;
        Vector3 mousePos = Mouse.current.position.ReadValue();   
        mousePos.z = Camera.main.nearClipPlane;
        Vector3 worldPos=Camera.main.ScreenToWorldPoint(mousePos);
        Vector2 dir = ((Vector2)worldPos - (Vector2)transform.position).normalized;
        Lasso lasso = lassoBelt.GetFreeLasso();
        lasso.transform.position = transform.position;
        lasso.transform.parent = lassoBelt.transform;
        while(lassoing)
        {
            lasso.transform.localPosition = Vector2.Lerp(Vector2.zero, dir*lassoRange, Mathf.PingPong(lassoTimer/lassoChargeTime,1));
            lassoTimer += Time.deltaTime;
            yield return null;
        }
        lassoTimer = 0;
    }

    private IEnumerator BringAnimalBack(float duration)
    {
        bringingAnimalBack = false;
        Vector2 startPos = (Vector2)lassoBelt.GetFreeLasso().transform.localPosition;
        Vector2 endPos = new Vector2(startPos.x/3*2, startPos.y/3*2);
        Vector3 camStartPoint = wrangleCam.localPosition;
        Vector2 point = GetMidPoint(Vector2.zero, endPos);
        Vector3 camEndPoint = new Vector3(point.x, point.y, -10);
        float timer = 0;
        
        while(timer < duration)
        {
            lassoBelt.GetFreeLasso().transform.localPosition = Vector2.Lerp(startPos, endPos, timer/duration);
            wrangleCam.localPosition = Vector3.Lerp(camStartPoint, camEndPoint, timer/duration);
            timer += Time.deltaTime;       
            yield return null;
        }
        
        if(timesPulled >= 3)
        {
            lassoBelt.GetFreeLasso().transform.parent = null;
            lassoBelt.GrabAnimal();
            animalFollowing = true;
            bringingBackLasso = false;
            timesPulled = 0;
            switchCam.SwitchPriority();
            playerWorldUI.SetCanvas(false);
        }
        else
        {
            bringingAnimalBack = true;
        }
    }

    private IEnumerator BringLassoBack(float duration, float delay)
    {
        bringingBackLasso = true;
        Lasso lasso = lassoBelt.GetFreeLasso();
        Vector2 startPos = lasso.transform.localPosition;
        float timer = 0;
        yield return new WaitForSeconds(delay);
        while(timer < duration)
        {
            float stepAmount = Mathf.Pow (timer * 1.5f, 2);
            lasso.transform.position = Vector2.Lerp(lasso.transform.position, transform.position, stepAmount);
            timer += Time.deltaTime;
            
            yield return null;
        }
        lassoTimer = 0;
        bringingBackLasso = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public float pullStrength;
    public float speed;
    public LassoBelt lassoBelt {get; private set;}
    [SerializeField] private Collider2D playerCollider;
    [SerializeField] private LayerMask merchantMask;
    [SerializeField] private Transform wrangleCam;
    [SerializeField] private float lassoRange = 5f;
    [SerializeField] private float lassoChargeTime = 1f; 
    [SerializeField] private float pullTime;
    private Controls playerControls;
    private Merchant merchant;
    private Rigidbody2D rb2d;
    private PlayerWorldUI playerWorldUI;
    private SwitchCamera switchCam;
    private float pullTimer;
    private float moveCounter;
    
    private float lassoTimer;
    private int timesPulled;
    
    private enum State{Charging, Wrangling, Roaming};
    private State state = State.Roaming;
    void Awake()
    {
        playerControls = new Controls();
        playerControls.Player.Wrangle.performed += ctx => Wrangle();
        playerControls.Player.Wrangle.canceled += ctx => ReleaseLasso();
        playerControls.Player.Release.performed += ctx => ReleaseAnimal();

        lassoBelt = GetComponent<LassoBelt>();
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
        if(state == State.Roaming) rb2d.velocity = GetDirection() * speed;
        else rb2d.velocity = Vector2.zero;

        lassoBelt.followPlayer = !(state == State.Wrangling);

        if(state == State.Wrangling && pullTimer <= 0) 
        {
            AnimalEscaped();
            switchCam.SwitchPriority();
            state = State.Roaming;
        }
        else 
        {
            pullTimer -= Time.deltaTime;
            playerWorldUI.FillTimeBar(pullTimer/pullTime);
        }
            

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
        if(state == State.Roaming && lassoBelt.GetFreeLasso() != null) 
        {
            state = State.Charging;
            StartCoroutine(ChargeLasso());
        }
        if(state == State.Wrangling) 
        {
            moveCounter -= pullStrength;
            playerWorldUI.FillPullBar(1f - moveCounter/lassoBelt.GetFreeLasso().animal.moveNum); 
            if(moveCounter <= 0)
            {
                pullTimer = pullTime;
                moveCounter = lassoBelt.GetFreeLasso().animal.moveNum;
                timesPulled++;
                playerWorldUI.ResetFillBars(); 
                if(timesPulled >= 3)
                {
                    lassoBelt.GetFreeLasso().transform.parent = lassoBelt.transform.parent;
                    lassoBelt.GrabAnimal();
                    switchCam.SwitchPriority();
                    UIManager.instance.SetGamePanel(true);
                    playerWorldUI.SetCanvas(false);
                    timesPulled = 0;
                    state = State.Roaming;
                }
                else
                {
                    StartCoroutine(BringAnimalBack(0.25f));
                }  
            }  
        }
    }

    public void AnimalEscaped()
    {
        if(lassoBelt.GetFreeLasso().animal != null)
        {
            timesPulled = 0;
            UIManager.instance.SetGamePanel(true);
            playerWorldUI.SetCanvas(false);
            playerWorldUI.ResetFillBars();
            lassoBelt.GetFreeLasso().ReleaseAnimal();
            lassoBelt.GetFreeLasso().transform.parent = lassoBelt.transform.parent;
            lassoBelt.GetFreeLasso().isWrangling = false;
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
        }
    }

    private void ReleaseLasso()
    {
        if(state == State.Charging && lassoBelt.GetFreeLasso().animal != null) 
        {
            pullTimer = pullTime;
            lassoBelt.BringAnimal(transform.position-lassoBelt.GetFreeLasso().transform.position);
            moveCounter = lassoBelt.GetFreeLasso().animal.moveNum;
            switchCam.SwitchPriority();
            Vector2 point = GetMidPoint(Vector2.zero, (Vector2)lassoBelt.GetFreeLasso().transform.localPosition);
            wrangleCam.localPosition = new Vector3(point.x, point.y, -10); 
            UIManager.instance.SetGamePanel(false);
            playerWorldUI.SetCanvas(true);
            playerWorldUI.ResetFillBars();
            state = State.Wrangling;
        }
        if(state == State.Charging && lassoBelt.GetFreeLasso().animal == null) 
        {
            state = State.Roaming;
            lassoBelt.GetFreeLasso().transform.parent = transform.parent;
        }
    }

    private Vector2 GetMidPoint(Vector2 start, Vector2 end)
    {
        float x = (end.x - start.x)/2;
        float y = (end.y - start.y)/2;
        return new Vector2(x,y);
    }

    private IEnumerator ChargeLasso()
    {
        Vector3 mousePos = Mouse.current.position.ReadValue();   
        mousePos.z = Camera.main.nearClipPlane;
        Vector3 worldPos=Camera.main.ScreenToWorldPoint(mousePos);
        Vector2 dir = ((Vector2)worldPos - (Vector2)transform.position).normalized;
        
        Lasso lasso = lassoBelt.GetFreeLasso();
        lasso.transform.position = transform.position;
        lasso.transform.parent = lassoBelt.transform;
        while(state == State.Charging)
        {
            lasso.transform.localPosition = Vector2.Lerp(Vector2.zero, dir*lassoRange, Mathf.PingPong(lassoTimer/lassoChargeTime,1));
            lassoTimer += Time.deltaTime;
            yield return null;
        }
        lassoTimer = 0;
    }

    private IEnumerator BringAnimalBack(float duration)
    {
        Vector2 startPos = (Vector2)lassoBelt.GetFreeLasso().transform.localPosition;
        Vector2 endPos = new Vector2(startPos.x/3*2, startPos.y/3*2);
        Vector3 camStartPoint = wrangleCam.localPosition;
        Vector2 point = GetMidPoint(Vector2.zero, endPos);
        Vector3 camEndPoint = new Vector3(point.x, point.y, -10);
        float timer = 0;
        
        while(timer < duration && state == State.Wrangling)
        {
            lassoBelt.GetFreeLasso().transform.localPosition = Vector2.Lerp(startPos, endPos, timer/duration);
            wrangleCam.localPosition = Vector3.Lerp(camStartPoint, camEndPoint, timer/duration);
            timer += Time.deltaTime;
               
            yield return null;
        }
    }
}

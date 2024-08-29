using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public float strength;
    public float speed;
    public LassoBelt lassoBelt {get; private set;}
    [SerializeField] private Collider2D playerCollider;
    [SerializeField] private LayerMask merchantMask;
    [SerializeField] private Transform wrangleCam;
    [SerializeField] private GameObject playerSprite;
    [SerializeField] private float lassoRange = 5f;
    [SerializeField] private float lassoChargeTime = 1f;
    [SerializeField] private float lassoCooldown;
    [SerializeField] private float pullTime;
    [SerializeField] private float speedModifier;
    [SerializeField] private float strengthModifier;

    [Header("FMOD")]
    [SerializeField] private FMODUnity.StudioEventEmitter fmodLassoThrow;
    [SerializeField] private FMODUnity.StudioEventEmitter fmodLassoCharge;
    [SerializeField] private FMODUnity.StudioEventEmitter fmodLassoMiss;
    [SerializeField] private FMODUnity.StudioEventEmitter fmodWranglePress;
    [SerializeField] private FMODUnity.StudioEventEmitter fmodWranglePull;
    [SerializeField] private FMODUnity.StudioEventEmitter fmodWrangleLoop;
    [SerializeField] private FMODUnity.StudioEventEmitter fmodWrangleSuccess;

    private Controls playerControls;
    private LassoAimer lassoAimer;
    private Merchant merchant;
    private Rigidbody2D rb2d;
    private PlayerWorldUI playerWorldUI;
    private SwitchCamera switchCam;
    private Vector3 originalSpriteScale;
    private float pullTimer;
    private float moveCounter;
    private float lassoReadyTime;
    private float lassoTimer;
    private int timesPulled;
    private bool lassoReady => lassoReadyTime <= GameManager.instance.timeElapsed;

    private enum State{Charging, Throwing, Wrangling, Roaming};
    private State state = State.Roaming;
    void Awake()
    {
        playerControls = new Controls();
        playerControls.Player.Wrangle.performed += ctx => Wrangle();
        playerControls.Player.Wrangle.canceled += ctx => ReleaseLasso();
        playerControls.Player.Release.performed += ctx => ReleaseAnimal();

        merchant = FindObjectOfType<Merchant>();
        lassoAimer = FindObjectOfType<LassoAimer>();
        lassoBelt = GetComponent<LassoBelt>();
        rb2d = GetComponent<Rigidbody2D>();
        playerWorldUI = GetComponent<PlayerWorldUI>();
        switchCam = GetComponent<SwitchCamera>();

        lassoAimer.gameObject.SetActive(false);
        playerWorldUI.SetCanvas(false);
        playerWorldUI.FillPullBar(0f);
        originalSpriteScale = playerSprite.transform.localScale;
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
        if(state == State.Roaming && !GameManager.instance.pickingUpgrade) 
        {
            Vector2 dir = GetDirection();
            rb2d.velocity = dir * speed;
            if(dir.x!=0)
                SetSpriteDirection(dir.x<0);
            
        }
        else 
            rb2d.velocity = Vector2.zero;


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
    public void SetSpriteDirection(bool isLeft)
    {
        if(isLeft)
            playerSprite.transform.localScale = new Vector3(-originalSpriteScale.x,originalSpriteScale.y,originalSpriteScale.z);
        else
            playerSprite.transform.localScale = originalSpriteScale;
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
        if(state == State.Roaming && lassoBelt.GetFreeLasso() != null && lassoReady)
        {
            state = State.Charging;
            StartCoroutine(ChargeLasso());
        }
        if(state == State.Wrangling)
        {
            moveCounter -= strength;
            float progress = 1f - moveCounter/lassoBelt.GetFreeLasso().animal.moveNum;
            playerWorldUI.FillPullBar(progress);

            if(moveCounter <= 0)
            {
                pullTimer = pullTime;
                moveCounter = lassoBelt.GetFreeLasso().animal.moveNum;
                timesPulled++;
                playerWorldUI.ResetFillBars();
                if(timesPulled >= 3)
                {
                    lassoBelt.GrabAnimal();
                    switchCam.SwitchPriority();
                    UIManager.instance.SetGamePanel(true);
                    playerWorldUI.SetCanvas(false);
                    timesPulled = 0;
                    state = State.Roaming;
                    StopFMODWrangleLoop(true);
                }
                else
                {
                    StartCoroutine(BringAnimalBack(0.25f));
                    fmodWranglePull.Play();
                }
            }
            else
            {
                fmodWranglePress.Play();
                fmodWranglePress.EventInstance.setParameterByName("wranglePressProgress", progress);
            }
        }
    }

    private void StopFMODWrangleLoop(bool success)
    {
        fmodWrangleLoop.EventInstance.setParameterByName("stop", 1);
        StartCoroutine(AsyncSetFMODParameter(fmodWrangleLoop, "stop", 0));
        if (success)
        {
            fmodWrangleSuccess.Play();
        }
    }

    private IEnumerator AsyncSetFMODParameter(FMODUnity.StudioEventEmitter emitter, string name, float value, float delay = 0.1f)
    {
        yield return new WaitForSeconds(delay);
        emitter.EventInstance.setParameterByName(name, value);
    }

    public void AnimalEscaped()
    {
        if(lassoBelt.GetFreeLasso().animal != null)
        {
            timesPulled = 0;
            UIManager.instance.SetGamePanel(true);
            playerWorldUI.SetCanvas(false);
            playerWorldUI.ResetFillBars();
            lassoBelt.GetFreeLasso().ReleaseAnimal(true);
            lassoBelt.GetFreeLasso().isWrangling = false;
            StopFMODWrangleLoop(false);
        }
    }

    public void ReleaseAnimal()
    {
        if(lassoBelt.GetLastInUse() != null)
        {
            bool isFree = true;
            if(playerCollider.IsTouchingLayers(merchantMask))
            {
                 merchant.TakeAnimal(lassoBelt.GetLastInUse().animal);
                 isFree = false;
            }

            lassoBelt.ReleaseLast(isFree);
        }
    }

    private void ReleaseLasso()
    {
        if(state == State.Charging && lassoReady)
        {
            lassoReadyTime = GameManager.instance.timeElapsed + lassoCooldown;
            StartCoroutine(ThrowLasso(0.25f));
        }
    }

    private Vector2 GetMidPoint(Vector2 start, Vector2 end)
    {
        float x = (end.x - start.x)/2;
        float y = (end.y - start.y)/2;
        return new Vector2(x,y);
    }

    public void UpgradePlayerStat(string stat)
    {
        if(stat.Contains("speed"))
            speed += speedModifier;

        if(stat.Contains("strength"))
            strength += strengthModifier;

        if(stat.Contains("lasso"))
            lassoBelt.AddLasso();
    }

    private IEnumerator ChargeLasso()
    {
        Vector3 mousePos = Mouse.current.position.ReadValue();
        mousePos.z = Camera.main.nearClipPlane;
        Vector3 worldPos=Camera.main.ScreenToWorldPoint(mousePos);
        Vector2 dir = ((Vector2)worldPos - (Vector2)transform.position).normalized;
        lassoAimer.gameObject.SetActive(true);
        fmodLassoCharge.Play();

        while(state == State.Charging)
        {
            lassoAimer.transform.localPosition = Vector2.Lerp(Vector2.zero, dir*lassoRange, Mathf.PingPong(lassoTimer/lassoChargeTime,1));
            lassoTimer += Time.deltaTime;
            yield return null;
        }

        fmodLassoCharge.Stop();
        lassoTimer = 0;
    }

    private IEnumerator BringAnimalBack(float duration)
    {
        Vector2 startPos = (Vector2)lassoBelt.GetFreeLasso().animal.transform.position;
        Vector2 endPos = (Vector2)transform.position + (startPos - (Vector2)transform.position)/3*2;
        Vector3 camStartPoint = wrangleCam.localPosition;
        Vector2 point = GetMidPoint((Vector2)transform.position, endPos);
        Vector3 camEndPoint = new Vector3(point.x, point.y, -10);
        float timer = 0;

        while(timer < duration && state == State.Wrangling)
        {
            lassoBelt.GetFreeLasso().animal.transform.position = Vector2.Lerp(startPos, endPos, timer/duration);
            wrangleCam.localPosition = Vector3.Lerp(camStartPoint, camEndPoint, timer/duration);
            timer += Time.deltaTime;

            yield return null;
        }
    }

    private IEnumerator ThrowLasso(float duration)
    {
        fmodLassoThrow.Play();
        state = State.Throwing;
        float timer = 0;
        //Calculate the 3 points for curved path
        Vector2 startPoint = (Vector2)lassoBelt.GetFreeLasso().transform.position;
        Vector2 endPoint = (Vector2)lassoAimer.transform.position;
        Vector2 midPoint = startPoint + (endPoint-startPoint)/2 + Vector2.up;
        while(timer<duration)
        {
            Vector2 p1 = Vector2.Lerp(startPoint,midPoint, timer/duration);
            Vector2 p2 = Vector2.Lerp(midPoint,endPoint, timer/duration);
            lassoBelt.GetFreeLasso().SetPosition(Vector2.Lerp(p1,p2,timer/duration));
            timer += Time.deltaTime;
            yield return null;
        }
        fmodLassoThrow.Stop();
        LassoThrown(lassoAimer.animal != null && !lassoAimer.animal.captured);
    }

    private void LassoThrown(bool successfulThrow)
    {
        lassoBelt.GetFreeLasso().SetAnimal(lassoAimer.animal);
        lassoAimer.gameObject.SetActive(false);
        if(successfulThrow)
        {
            pullTimer = pullTime;
            moveCounter = lassoBelt.GetFreeLasso().animal.moveNum;
            lassoBelt.BringAnimal(transform.position-lassoAimer.transform.position);
            switchCam.SwitchPriority();
            SetWrangleCamMidPos(lassoAimer.transform);
            SetWranglingUI(true);
            state = State.Wrangling;
            fmodWrangleLoop.Play();
        }
        else
        {
            state = State.Roaming;
            fmodLassoMiss.Play();
        }
    }

    private void SetWranglingUI(bool isWrangling)
    {
        if(isWrangling)
        {
            UIManager.instance.SetGamePanel(false);
            playerWorldUI.SetCanvas(true);
            playerWorldUI.ResetFillBars();
        }

    }

    private void SetWrangleCamMidPos(Transform endObj)
    {
        Vector2 point = GetMidPoint(Vector2.zero, (Vector2)endObj.localPosition);
        wrangleCam.localPosition = new Vector3(point.x, point.y, -10);
    }
}

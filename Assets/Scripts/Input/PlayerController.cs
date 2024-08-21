using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public float pullStrength;
    [SerializeField] private Collider2D playerCollider;
    [SerializeField] private Lasso lasso;
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
    private float lassoTimer;
    private float moveCounter;
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
        if(!lassoing && !bringingBackLasso) rb2d.velocity = GetDirection() * speed;
        else rb2d.velocity = Vector2.zero;

        if(animalFollowing && Vector2.Distance(transform.position, lasso.transform.position) > followDistance) 
            lasso.transform.position += (transform.position - lasso.transform.position) * followSharpness;

        if(bringingAnimalBack && pullTimer <= 0) 
        {
            ReleaseAnimal();
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
        return (Vector2)lasso.transform.position;
    }

    public string GetAnimalName()
    {
        if(lasso.gotAnimal) return lasso.animal.animalName;
        else return null;
    }

    private void Wrangle()
    {
        if(!bringingBackLasso && lasso.animal == null) StartCoroutine(ChargeLasso());
        if(bringingAnimalBack) 
        {
            moveCounter -= pullStrength;
            if(moveCounter <= 0)
            {
                pullTimer = pullTime;
                moveCounter = lasso.animal.moveNum;
                timesPulled++;
                StartCoroutine(BringAnimalBack(0.25f));
            }
            playerWorldUI.FillPullBar(1f - moveCounter/lasso.animal.moveNum);
        }
    }

    public void ReleaseAnimal()
    {
        if(lasso.gotAnimal)
        {
            playerWorldUI.SetCanvas(false);
            playerWorldUI.FillPullBar(0f);
            if(playerCollider.IsTouchingLayers(merchantMask)) merchant.TakeAnimal(lasso.animal);
            timesPulled = 0;
            lasso.ReleaseAnimal();
            lasso.transform.parent = transform;
            animalFollowing = false;
            bringingAnimalBack = false;
            StartCoroutine(BringLassoBack(0.5f,0.25f));
        }
    }

    private void ReleaseLasso()
    {
        lassoing = false;
        if(!bringingBackLasso && lasso.animal != null && !animalFollowing) 
        {
            bringingAnimalBack = true;
            bringingBackLasso = true;
            lasso.BringAnimal(transform.position-lasso.transform.position);
            moveCounter = lasso.animal.moveNum;
            pullTimer = pullTime;
            switchCam.SwitchPriority();
            Vector2 point = GetMidPoint(Vector2.zero, (Vector2)lasso.transform.localPosition);
            wrangleCam.localPosition = new Vector3(point.x, point.y, -10); 
            playerWorldUI.SetCanvas(true);
            playerWorldUI.FillPullBar(0f);
        }
        if(!bringingBackLasso && lasso.animal == null) StartCoroutine(BringLassoBack(0.5f, 0.25f));
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
        Vector2 startPos = (Vector2)lasso.transform.localPosition;
        Vector2 endPos = new Vector2(startPos.x/3*2, startPos.y/3*2);
        Vector3 camStartPoint = wrangleCam.localPosition;
        Vector2 point = GetMidPoint(Vector2.zero, endPos);
        Vector3 camEndPoint = new Vector3(point.x, point.y, -10);


        float timer = 0;
        
        while(timer < duration)
        {
            lasso.transform.localPosition = Vector2.Lerp(startPos, endPos, timer/duration);
            wrangleCam.localPosition = Vector3.Lerp(camStartPoint, camEndPoint, timer/duration);
            timer += Time.deltaTime;       
            yield return null;
        }
        
        if(timesPulled >= 3)
        {
            lasso.transform.parent = null;
            lasso.AnimalWrangled();
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
        Vector2 startPos = lasso.transform.localPosition;
        float timer = 0;
        yield return new WaitForSeconds(delay);
        while(timer < duration)
        {
            float stepAmount = Mathf.Pow (timer * 1.5f, 2);
            lasso.transform.localPosition = Vector2.Lerp(lasso.transform.localPosition, Vector2.zero, stepAmount);
            timer += Time.deltaTime;
            
            yield return null;
        }
        lasso.transform.localPosition = Vector2.zero;
        lassoTimer = 0;
        bringingBackLasso = false;
    }
}

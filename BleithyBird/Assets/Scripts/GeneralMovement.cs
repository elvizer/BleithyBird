using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using FishNet.Object;
using FishNet.Object.Prediction;
using FishNet.Transporting;
using FishNet.Object.Synchronizing;

public struct MoveData : IReplicateData
{
    public float Horizontal;
    public bool CanJump;

    private uint _tick;
    public void Dispose() { }
    public uint GetTick() => _tick;
    public void SetTick(uint value) => _tick = value;
}

public struct ReconcileData : IReconcileData
{
    public Vector3 Position;
    public Quaternion Rotation;
    public Vector2 Velocity;
    public float JumpTimer;

    private uint _tick;
    public void Dispose() { }
    public uint GetTick() => _tick;
    public void SetTick(uint value) => _tick = value;
}

public class GeneralMovement : NetworkBehaviour
{
    private Rigidbody2D rb;
    private Quaternion targetRotation = Quaternion.identity;
    private float jumpForce = 15f;
    private float moveForce = 2500f;

    private float timer;

    private float jumpDelay = 0.3f;
    private bool canJump = true;
    private bool jumpedQueued = false;

    private bool facingRight = true;

    private Animator animator;

    private ScreenShake shaker;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    public override void OnStartNetwork()
    {
        base.OnStartNetwork();
        if(base.IsServer || base.IsClient)
        {
            base.TimeManager.OnTick += TimeManager_OnTick;
            base.TimeManager.OnPostTick += TimeManager_OnPostTick;
        }
            
    }

    public override void OnStopNetwork()
    {
        base.OnStopNetwork();
        if(base.TimeManager != null)
        {
            base.TimeManager.OnTick -= TimeManager_OnTick;
            base.TimeManager.OnPostTick -= TimeManager_OnPostTick;
        }
            
    }
    public override void OnStartClient()
    {
        base.OnStartClient();

        if (base.IsOwner)
        {
            Camera playerCam = Camera.main;
        }
        else
        {
            gameObject.GetComponent<GeneralMovement>().enabled = false;
        }
    }

    void Update()
    {
        if (!base.IsOwner)
            return;

        jumpedQueued |= Input.GetKeyDown(KeyCode.Space);
       

        if (rb.velocity.y >= 0.4f)
        {
            animator.SetBool("isJumping", true);
            animator.SetBool("isFalling", false);
        }
       else if (rb.velocity.y <= -0.4f)
        {
            animator.SetBool("isJumping", false);
            animator.SetBool("isFalling", true);
        }
        else
        {
            animator.SetBool("isJumping", false);
            animator.SetBool("isFalling", false);
        }
    }

    private void TimeManager_OnTick()
    {
        if (base.IsOwner)
        {
            Reconcile(default, false);
            BuildActions(out MoveData md);
            Move(md, false);
        }
        if (base.IsServer)
        {
            Move(default, true);
            ReconcileData rd = new ReconcileData()
            {
                Position = transform.position,
                Rotation = transform.rotation,
                Velocity = rb.velocity
            };
            Reconcile(rd, true);
        }
    }

    private void TimeManager_OnPostTick()
    {
        if (base.IsServer)
        {
            ReconcileData rd = new ReconcileData()
            {
                Position = transform.position,
                Rotation = transform.rotation,
                Velocity = rb.velocity
            };
            Reconcile(rd, true);
        }
    }

    private void BuildActions(out MoveData moveData)
    {
        moveData = default;
        moveData.Horizontal = Input.GetAxisRaw("Horizontal");
        moveData.CanJump = jumpedQueued;

        //Unset queued values.
        jumpedQueued = false;
    }

    [Replicate]
    private void Move(MoveData moveData, bool asServer, Channel channel = Channel.Unreliable, bool replaying = false)
    {
        rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -6, 6), Mathf.Clamp(rb.velocity.y, -16, 9));

        if (transform.position.x <= -9.8)
        {
            rb.velocity = Vector2.zero;
            transform.position = new Vector3(-4, 2, transform.position.z);
        }

        float delta = (float)base.TimeManager.TickDelta;

        timer += delta;

        if(timer > jumpDelay)
        {
            canJump = true;
        }
        else
        {
            canJump = false;
        }

        if (moveData.CanJump && canJump)
        {
            timer = 0f;
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        if (transform.position.x >= -1.1f)
        {
            if (moveData.Horizontal == 1f || rb.velocity.x > 0) rb.velocity = new Vector2(0, rb.velocity.y);

            Vector3 startPos = transform.position;

            transform.position = Vector3.Slerp(startPos, new Vector3(-1.1f, transform.position.y, transform.position.z), 1.5f * delta);
        }

        Vector2 moveDir = new Vector2(moveData.Horizontal, 0f).normalized;

        rb.AddForce(moveDir * delta * moveForce, ForceMode2D.Force);

        if (moveData.Horizontal == 1f && !facingRight)
        {
            StartCoroutine(Rotate(new Vector3(0, 0, 0), 0.1f));
            facingRight = true;
        }

        if (moveData.Horizontal == -1f && facingRight)
        {
            StartCoroutine(Rotate(new Vector3(0, 180, 0), 0.1f));
            facingRight = false;
        }

    }

    IEnumerator Rotate(Vector3 rot, float lerpDuration)
    {
        Quaternion currentRot = transform.localRotation;
        float timeElapsed = 0;
        targetRotation = Quaternion.Euler(rot);
        while (timeElapsed < lerpDuration)
        {
            transform.localRotation = Quaternion.Slerp(currentRot, targetRotation, timeElapsed / lerpDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }

    [Reconcile]
    private void Reconcile(ReconcileData recData, bool asServer, Channel channel = Channel.Unreliable)
    {
        transform.position = recData.Position;
        transform.rotation = recData.Rotation;
        rb.velocity = recData.Velocity;
        timer = recData.JumpTimer;
    }
}

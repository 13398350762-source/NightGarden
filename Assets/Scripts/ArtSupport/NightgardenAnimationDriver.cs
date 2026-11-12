using UnityEngine;

[RequireComponent(typeof(Animator), typeof(SpriteRenderer))]
public class NightgardenAnimationDriver : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private static readonly int Moving = Animator.StringToHash("IsMoving");
    private static readonly int AttackTrigger = Animator.StringToHash("Charge");
    public bool previewAutomatically;
    private float nextAttack;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        nextAttack = 1.5f;
    }

    public void SetMoving(bool moving) { animator.SetBool(Moving, moving); }
    // Ignore tiny horizontal noise while moving vertically; keep the last facing.
    public void FaceDirection(float horizontal) { if (Mathf.Abs(horizontal) > 0.025f) spriteRenderer.flipX = horizontal < 0; }
    public void SetVelocity(Vector2 velocity)
    {
        SetMoving(velocity.sqrMagnitude > 0.000625f);
        FaceDirection(velocity.x);
    }
    public void Attack() { foreach (var p in animator.parameters) if (p.nameHash == AttackTrigger) { animator.SetTrigger(AttackTrigger); break; } }
    public void Charge() { Attack(); }

    private void Update()
    {
        if (!previewAutomatically) return;
        SetMoving(true);
        nextAttack -= Time.deltaTime;
        if (nextAttack <= 0) { Attack(); nextAttack = 2.3f; }
    }
}

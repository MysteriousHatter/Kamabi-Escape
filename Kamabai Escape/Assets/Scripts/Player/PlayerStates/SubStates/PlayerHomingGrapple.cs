
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;

public class PlayerHomingGrapple : PlayerAbilityState
{

    public bool canGrapple { get; private set; }
    private bool isHolding;
    private bool isTapping;
    private bool grappleInputStop;
    private bool jumpInput;
    private float lastGrappleTime;
    private float swingHeight = 0f;

    float rotationY;
    float rotationX;
    private Vector3 grappleDirectionInput;
    private Vector3 targetPos;
    private RaycastHit hit;
    private Vector3 lineStartPos;
    private float currentLength = 1.0f;

    //Arrow Rotation
    private Vector3 grappleDirection;
    float currentStartRotation = 0f;
    bool hittingItem = false;

    Transform grapplePoint;

    private enum GrappleTypes
    {
        pullingToPoint,
        pullingObject,
        swingingOnObject,
        reelInorOut,
        swingReelInorOut,
        Enemy,
        noGrapple
    }

    private GrappleTypes grappleType = GrappleTypes.noGrapple;

    private bool beingPulled = false;
    private bool pullingObject = false;

    bool isDrawing = false;
    private Coroutine drawLineCoroutine;
    private float distanceAnimation;
    private float counter;
    bool finishedDrawing = false;
    Vector3 curentEndpointPosition;

    public PlayerHomingGrapple(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
        
    }

    public override void Enter()
    {
        base.Enter();
        player.InputHandler.UseTappingGrapple();
        canGrapple = false;
        isTapping = true;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        DrawLine();
        SwingControls();
        jumpInput = player.InputHandler.JumpInput;
        
        if (player.inputType == Player.GrappleInputs.TapButton)
        {
            Debug.Log("Zero Breach");
            if (isTapping)
            {
                Debug.Log("first breach");
                Debug.Log("Begin methods " + isAbilityDone);
                isTapping = false;
                //player.playerInput.SwitchCurrentActionMap("Grapple Gameplay");
                GameObject target = FindTarget();
                //isAbilityDone = true;
                this.player.joint = this.player.gameObject.AddComponent<SpringJoint>();
                this.player.joint.autoConfigureConnectedAnchor = false;
                if (target != null)
                {
                    grapplePoint = target.transform.GetChild(0);
                    //player.playerInput.SwitchCurrentActionMap("Grapple Gameplay");
                    Debug.Log("The child " + grapplePoint.gameObject);
                    if (target.CompareTag("HookPoint-Automatic") || target.CompareTag("HookPoint-Manual"))
                    {
                        CalculatePlatformDistance(target);
                    }
                    player.line.enabled = true;
                    isDrawing = true;
                }
            }
            else
            {
                player.GrappleDirectionIndicator.gameObject.SetActive(false);
                player.line.enabled = false;
                isAbilityDone = true;
                lastGrappleTime = Time.time;
                grappleType = GrappleTypes.noGrapple;
                grapplePoint = null;


            }
        }
        isAbilityDone = true;
    }

    private void CalculatePlatformDistance(GameObject targetObject)
    {
        this.player.joint.spring = 4.5f;
        this.player.joint.damper = 7f;
        this.player.joint.massScale = 4.5f;
        if (targetObject.CompareTag("HookPoint-Automatic")) { grappleType = GrappleTypes.pullingToPoint; }
        else if (targetObject.CompareTag("HookPoint-Manual")) { grappleType = GrappleTypes.swingReelInorOut; }

        player.joint.connectedBody = targetObject.GetComponent<Rigidbody>();
        player.joint.connectedAnchor = targetObject.transform.InverseTransformPoint(grapplePoint.transform.position);

        playerData.maxDistance = Vector3.Distance(player.playerHand.transform.position, grapplePoint.transform.position);
        playerData.minDistance = Vector3.Distance(player.playerHand.transform.position, player.joint.transform.position);
        currentLength = Vector3.Distance(player.joint.transform.position, player.joint.connectedAnchor);
        player.joint.maxDistance = playerData.maxDistance;

        distanceAnimation = Vector3.Distance(player.joint.transform.position, player.joint.connectedAnchor);
    }

    GameObject FindTarget()
    {
        GameObject[] automaticTargets = GameObject.FindGameObjectsWithTag("HookPoint-Automatic");  // Change the tag to match your target objects
        GameObject[] manunalTargets = GameObject.FindGameObjectsWithTag("HookPoint-Manual");
        GameObject[] targets = automaticTargets.Concat(manunalTargets).ToArray();
        GameObject targetPlaceholder = null;

        float closestDistance = Mathf.Infinity;
        foreach (GameObject targetObject in targets)
        {
            float distance = Vector3.Distance(player.transform.position, targetObject.transform.position);
            if (distance < closestDistance && distance < playerData.homingRadius)
            {
                targetPlaceholder = targetObject;
                closestDistance = distance;

            }
        }

        // Spawn or destroy the reticle based on the target
        //if (target != null)
        //{
        //    if (reticle == null)
        //    {
        //        reticle = Instantiate(reticlePrefab);
        //    }
        //    reticle.SetActive(true);
        //    reticle.transform.position = target.transform.position;
        //}
        //else
        //{
        //    if (reticle != null)
        //    {
        //        reticle.SetActive(false);
        //    }
        //}

        Debug.Log("The name of the automatic object " + targetPlaceholder);
        return targetPlaceholder;
    }


    public override void Exit()
    {
        player.inputType = Player.GrappleInputs.NoInput;
       // player.InputHandler.SwitchActionMaps();
        base.Exit();
    }


    private void DrawLine()
    {
        if (isDrawing)
        {
            if (counter < distanceAnimation)
            {
                finishedDrawing = false;
                counter += (.1f / playerData.animationDuration) * 5f;

                float x = Mathf.Lerp(0, distanceAnimation, counter);

                Vector3 pointA = player.playerHand.transform.position;
                Vector3 pointB = grapplePoint.transform.position;

                Vector3 pointAlongLine = Vector3.Lerp(pointA, pointB, x);

                player.line.SetPosition(0, player.playerHand.transform.position);
                player.line.SetPosition(1, pointAlongLine);
            }
            else
            {
                if (grappleType.Equals(GrappleTypes.reelInorOut) || grappleType.Equals(GrappleTypes.pullingToPoint) || grappleType.Equals(GrappleTypes.swingReelInorOut)) { PullPlayer(); }
            }
        }

    }

    private void PullPlayer()
    {
        //Challenge 2:

        if (player.joint.connectedAnchor != null)
        {
            // Reduce the joint distance over time
            if (grappleType.Equals(GrappleTypes.reelInorOut) || grappleType.Equals(GrappleTypes.swingReelInorOut))
            {
                if (player.InputHandler.ReelInput > 0)
                {
                    Vector3 directionToPoint = grapplePoint.transform.position - player.transform.position;
                    player.RB.AddForce(directionToPoint.normalized * playerData.forwardThrustForce * Time.deltaTime);

                    float distanceFromPoint = Vector3.Distance(player.transform.position, grapplePoint.transform.position);

                    player.joint.maxDistance = distanceFromPoint * 0.8f;
                    player.joint.minDistance = distanceFromPoint * 0.25f;
                }
                else if (player.InputHandler.ReelInput < 0)
                {
                    float extendedDistanceFromPoint = Vector3.Distance(player.transform.position, grapplePoint.transform.position) + playerData.extendCableSpeed;

                    player.joint.maxDistance = extendedDistanceFromPoint * 0.8f;
                    player.joint.minDistance = extendedDistanceFromPoint * 0.15f;
                }

            }
            else if (grappleType.Equals(GrappleTypes.pullingToPoint))
            {
                ZippingToPoint();
            }


            ////// Update the line's end position to the new player position
            player.line.SetPosition(0, player.playerHand.transform.position);
            player.line.SetPosition(1, grapplePoint.transform.position);



        }
        if (player.joint.maxDistance <= 1f || player.InputHandler.cancelInput)
        {
            // Stop grappling if we've reached the grapple point
            player.joint.connectedBody = null;
            player.joint.enableCollision = false;
            player.joint.enablePreprocessing = false;
            player.line.enabled = false;
            this.player.joint.spring = 0f;
            this.player.joint.damper = 0f;
            this.player.joint.massScale = 0f;
            isAbilityDone = true;
            lastGrappleTime = Time.time;
            grappleType = GrappleTypes.noGrapple;
        }
    }


    private void SwingControls()
    {
        if (grappleType.Equals(GrappleTypes.swingingOnObject))
        {
            ZippingToPoint();
            if (player.joint.maxDistance <= swingHeight)
            {
                // Stop grappling if we've reached the grapple point
                this.player.joint.spring = 1f;
                this.player.joint.damper = 3f;
                this.player.joint.massScale = 2f;

                if (player.InputHandler.NormInputZ > 0) { player.RB.AddForce(Vector3.forward * playerData.SwingThrustForce * Time.deltaTime); }
                else if (player.InputHandler.NormInputX > 0) { player.RB.AddForce(Vector3.right * playerData.SwingThrustForce * Time.deltaTime); }
                else if (player.InputHandler.NormInputX < 0) { player.RB.AddForce(-Vector3.right * playerData.SwingThrustForce * Time.deltaTime); }
            }

            player.line.SetPosition(0, player.playerHand.transform.position);
            player.line.SetPosition(1, grapplePoint.transform.position);

            if (jumpInput)
            {
                player.joint.connectedBody = null;
                player.joint.enableCollision = false;
                player.joint.enablePreprocessing = false;
                player.line.enabled = false;
                this.player.joint.spring = 0f;
                this.player.joint.damper = 0f;
                this.player.joint.massScale = 0f;
                isAbilityDone = true;
                lastGrappleTime = Time.time;
                grappleType = GrappleTypes.noGrapple;
                stateMachine.ChangeState(player.JumpState);
            }
            else if (player.InputHandler.cancelInput)
            {
                player.joint.connectedBody = null;
                player.joint.enableCollision = false;
                player.joint.enablePreprocessing = false;
                player.line.enabled = false;
                this.player.joint.spring = 0f;
                this.player.joint.damper = 0f;
                this.player.joint.massScale = 0f;
                isAbilityDone = true;
                lastGrappleTime = Time.time;
                grappleType = GrappleTypes.noGrapple;
            }

        }
        else if (grappleType.Equals(GrappleTypes.swingReelInorOut))
        {
            if (player.InputHandler.NormInputZ > 0) { player.RB.AddForce(Vector3.forward * playerData.SwingThrustForce * Time.deltaTime); }
            else if (player.InputHandler.NormInputX > 0) { player.RB.AddForce(Vector3.right * playerData.SwingThrustForce * Time.deltaTime); }
            else if (player.InputHandler.NormInputX < 0) { player.RB.AddForce(-Vector3.right * playerData.SwingThrustForce * Time.deltaTime); }

            if (jumpInput)
            {
                player.joint.connectedBody = null;
                player.joint.enableCollision = false;
                player.joint.enablePreprocessing = false;
                player.line.enabled = false;
                this.player.joint.spring = 0f;
                this.player.joint.damper = 0f;
                this.player.joint.massScale = 0f;
                isAbilityDone = true;
                lastGrappleTime = Time.time;
                grappleType = GrappleTypes.noGrapple;
                stateMachine.ChangeState(player.JumpState);
            }
            else if (player.InputHandler.cancelInput)
            {
                player.joint.connectedBody = null;
                player.joint.enableCollision = false;
                player.joint.enablePreprocessing = false;
                player.line.enabled = false;
                this.player.joint.spring = 0f;
                this.player.joint.damper = 0f;
                this.player.joint.massScale = 0f;
                isAbilityDone = true;
                lastGrappleTime = Time.time;
                grappleType = GrappleTypes.noGrapple;
            }
        }


    }

    private void ZippingToPoint()
    {
        Debug.Log("Pulling to point");
        player.joint.maxDistance = player.joint.maxDistance - ((Time.deltaTime * 5) * playerData.grappleSpeed);

        Vector3 playerPos = player.playerHand.transform.position;
        Vector3 grapplePos = player.joint.connectedBody.transform.position;
        Vector3 direction = (grapplePos - playerPos).normalized;
        Debug.Log("The direction " + direction);

        // Calculate the offset from the grapple point based on the player's orientation
        Vector3 playerForward = player.transform.forward;
        Vector3 playerRight = player.transform.right;
        float offset = Vector3.Dot(playerRight, direction) * player.joint.maxDistance;
        Vector3 targetPos = grapplePos - playerForward * offset;


        // Move the player towards the target position
        Vector3 displacement = (targetPos - player.transform.position).normalized * player.joint.maxDistance * playerData.grappleSpeed * Time.deltaTime;
        this.player.transform.position += displacement;

        Vector3.Lerp(this.player.transform.forward, targetPos, 20f);
    }

}

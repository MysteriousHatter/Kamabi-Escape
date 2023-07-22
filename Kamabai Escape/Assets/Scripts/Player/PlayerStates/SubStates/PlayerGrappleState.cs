
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Windows;
using System;
using UnityEngine.InputSystem;
using Cinemachine;
using Unity.VisualScripting;

public class PlayerGrappleState : PlayerAbilityState
{

    public bool canGrapple { get; private set; }
    private bool isHolding;
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

    Quaternion targetRotation;
    GameObject previousObject;
    bool hittingGrapplePoint = false;
    Vector3 Direction = Vector3.zero;
    private Vector3 initialPositionOffset;
    private string animBoolName;

    private GameObject movingPlatform;
    private Transform childComponent;
    private GameObject enemy;


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

    public PlayerGrappleState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName = null) : base(player, stateMachine, playerData, animBoolName)
    {
        this.animBoolName = "grapple";
    }

    public override void Enter()
    {
        base.Enter();
        rotationY = 180f;
        rotationX = 0f;
        hit = new RaycastHit();

        lineStartPos = player.playerHand.transform.position;
        

        canGrapple = false;
        player.InputHandler.UseHoldingGrapple();
        player.InputHandler.isSwinging = false;

        isHolding = true;
        grappleDirection = core.Movement.FacingDirection;
        Time.timeScale = playerData.holdTimeScale;
        startTime = Time.unscaledTime;

        grappleDirection = Quaternion.Euler(0f, core.Movement.FacingDirection.y, 0f) * Vector3.forward;

        player.GrappleDirectionIndicator.gameObject.SetActive(true);

        // Calculate the angle between the arrow and the up vector
        float angle = Vector3.SignedAngle(Vector3.up, grappleDirection, Vector3.up);

        player.GrappleDirectionIndicator.localRotation = Quaternion.Euler(0f, angle - playerData.angleOffset, 0f);
        player.Anim.SetBool(this.animBoolName, true);
        Debug.Log("The direction of the angle: " + angle);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        DrawLine();
        SwingControls();
        jumpInput = player.InputHandler.JumpInput;


        if (isHolding)
        {
            ////// Move this code to Update In GrappleState script;
            player.changeColorScale.GetComponent<PostProcessTest>().enabled = true;
            grappleInputStop = player.InputHandler.GrappleInputStop;
            GrappleRotation();
            DetectingAGrappleObject();
            if (grappleInputStop)
            {
                isHolding = false;
                Time.timeScale = 1f;
                startTime = Time.time;
                player.changeColorScale.GetComponent<PostProcessTest>().enabled = false;
                Debug.Log("The current time " + Time.timeScale);
                player.InputHandler.SwitchActionMapToGrapple();
                if (Physics.Raycast(player.GrappleDirectionIndicator.transform.position, -player.GrappleDirectionIndicator.transform.forward, out hit, playerData.distance, playerData.mask))
                {
                    Debug.Log("Detecting the item");
                    player.src.clip = player.grappleSFX;
                    player.src.Play();
                    GameObject targetObject = hit.collider.gameObject;
                    this.player.joint = this.player.gameObject.AddComponent<SpringJoint>();
                    this.player.joint.autoConfigureConnectedAnchor = false;

                    player.GrappleDirectionIndicator.gameObject.SetActive(false);
                    if (targetObject.CompareTag("Platform") || targetObject.CompareTag("Platform-Reel") || targetObject.CompareTag("Swing-Reel"))
                    {
                     
                        Debug.Log("This is an automatic platform");
                        CalculatePlatformDistance(targetObject);
                    }
                    else if (targetObject.CompareTag("Collectiable") || targetObject.CompareTag("Grab-Platform"))
                    {
                        CalculateCollectiableDistance(targetObject);
                    }
                    else if (targetObject.CompareTag("Enemy"))
                    {
                        targetObject.GetComponent<Guard>().isStunned = true;
                        player.transform.SetParent(null);
                        targetObject.GetComponent<Guard>().theKid = null;
                        CalculateEnemyDistance(targetObject);
                    }
                    else if (targetObject.CompareTag("Swing"))
                    {
                        CalculateSwingDistance(targetObject);
                    }

                    player.line.enabled = true;
                    isDrawing = true;
                }
                else
                {
                    player.GrappleDirectionIndicator.gameObject.SetActive(false);
                    player.line.enabled = false;
                    isAbilityDone = true;
                    lastGrappleTime = Time.time;
                    grappleType = GrappleTypes.noGrapple;



                }

            }
        }

    }

    private void DetectingAGrappleObject()
    {
        Vector3 rayDirection = -player.GrappleDirectionIndicator.transform.forward;
        Vector3 rayEndPosition = player.GrappleDirectionIndicator.transform.position + rayDirection * playerData.distance;

        Debug.DrawRay(player.GrappleDirectionIndicator.transform.position, rayDirection * playerData.distance, Color.red);
        player.lineShadow.SetPosition(0, player.GrappleDirectionIndicator.transform.position);
        player.lineShadow.SetPosition(1, rayEndPosition);
        Debug.Log("Is Grapple InputStop " + grappleInputStop);
        Debug.Log("The current time " + Time.timeScale);
        if (Physics.Raycast(player.GrappleDirectionIndicator.transform.position, -player.GrappleDirectionIndicator.transform.forward, out hit, playerData.distance, playerData.mask))
        {
            Debug.Log("The current gameObject " + hit.collider.name);
            if (hit.collider.gameObject.layer == 7)
            {
                hit.collider.gameObject.GetComponent<Renderer>().material.SetFloat("_Emission", 1f);
                hit.collider.gameObject.GetComponent<Renderer>().material.SetFloat("_Blend", 0f);
                previousObject = hit.collider.gameObject;
                hittingGrapplePoint = true;
            }
            else { hittingGrapplePoint = false; }

        }
        else if(hittingGrapplePoint == true)
        {
            hittingGrapplePoint = false;
            previousObject.GetComponent<Renderer>().material.SetFloat("_Emission", 0f);
            previousObject.GetComponent<Renderer>().material.SetFloat("_Blend", 1f);
            //previousObject.GetComponent<Renderer>().material.SetFloat("_Glossines", 0.831f);
        }
    }

    private void CalculateSwingDistance(GameObject targetObject)
    {
        this.player.joint.spring = 4.5f;
        this.player.joint.damper = 7f;
        this.player.joint.massScale = 4.5f;
        Debug.Log("Are we hitting something");
        grappleType = GrappleTypes.swingingOnObject;

        player.joint.connectedBody = targetObject.GetComponent<Rigidbody>();
        player.joint.connectedAnchor = targetObject.transform.InverseTransformPoint(hit.point);

        playerData.maxDistance = Vector3.Distance(player.playerHand.transform.position, hit.point);
        playerData.minDistance = Vector3.Distance(player.playerHand.transform.position, player.joint.transform.position);
        currentLength = Vector3.Distance(player.joint.transform.position, player.joint.connectedAnchor);
        player.joint.maxDistance = playerData.maxDistance;
        swingHeight = player.joint.maxDistance / 0.004f;

        distanceAnimation = Vector3.Distance(player.joint.transform.position, player.joint.connectedAnchor);
    }

    private void CalculatePlatformDistance(GameObject targetObject)
    {
        this.player.joint.spring = 30f;
        this.player.joint.damper = 20f;
        this.player.joint.massScale = 30f;
        movingPlatform = targetObject;
        if(movingPlatform.GetComponent<MovingPlatform>() != null) { movingPlatform.GetComponent<MovingPlatform>().hitByGrapple = true; }

        Debug.Log("Are we hitting something");
        if (targetObject.CompareTag("Platform-Reel")) { grappleType = GrappleTypes.reelInorOut; }
        else if (targetObject.CompareTag("Platform")) { grappleType = GrappleTypes.pullingToPoint; }
        else if (targetObject.CompareTag("Swing-Reel")) { grappleType = GrappleTypes.swingReelInorOut; }

        if(targetObject.GetComponent<Oscillator>() != null) { targetObject.GetComponent<Oscillator>().canMove = false; }
        else if(targetObject.GetComponentInParent<Oscillator>() != null) { targetObject.GetComponentInParent<Oscillator>().canMove = false; }



        player.joint.connectedBody = movingPlatform.GetComponent<Rigidbody>();
        player.joint.connectedAnchor = movingPlatform.transform.InverseTransformPoint(movingPlatform.transform.position);

        playerData.maxDistance = Vector3.Distance(player.playerHand.transform.position, movingPlatform.transform.position);
        playerData.minDistance = Vector3.Distance(player.playerHand.transform.position, player.joint.transform.position);
        currentLength = Vector3.Distance(player.joint.transform.position, player.joint.connectedAnchor);
        player.joint.maxDistance = playerData.maxDistance - 2f;

        distanceAnimation = Vector3.Distance(player.joint.transform.position, player.joint.connectedAnchor);
    }

    private void CalculateCollectiableDistance(GameObject targetObject)
    {
        this.player.joint.spring = 0f;
        this.player.joint.damper = 0f;
        this.player.joint.massScale = 0f;
        grappleType = GrappleTypes.pullingObject;
        player.joint.enableCollision = true;
        player.joint.enablePreprocessing = true;
        player.line.enabled = true;

        player.joint.autoConfigureConnectedAnchor = true;

        player.joint.connectedBody = targetObject.GetComponent<Rigidbody>();
        player.joint.connectedAnchor = targetObject.transform.InverseTransformPoint(hit.point);

        //Vector3 targetPos = hit.point - player.playerHand.transform.position;
        playerData.maxDistance = Vector3.Distance(player.playerHand.transform.position, hit.point);
        playerData.minDistance = 2.0f; //Our desired distance
        currentLength = Vector3.Distance(player.joint.transform.position, player.joint.connectedAnchor);
        player.joint.maxDistance = playerData.maxDistance;

        distanceAnimation = Vector3.Distance(player.joint.transform.position, player.joint.connectedAnchor);
    }

    private void CalculateEnemyDistance(GameObject targetObject)
    {
        this.player.joint.spring = 0f;
        this.player.joint.damper = 0f;
        this.player.joint.massScale = 0f;
        grappleType = GrappleTypes.Enemy;
        player.joint.enableCollision = true;
        player.joint.enablePreprocessing = true;
        player.line.enabled = true;

        player.joint.autoConfigureConnectedAnchor = true;

        player.joint.connectedBody = targetObject.GetComponent<Rigidbody>();
        player.joint.connectedAnchor = targetObject.transform.InverseTransformPoint(hit.point);
        curentEndpointPosition = player.joint.connectedBody.position;
        //Vector3 targetPos = hit.point - player.playerHand.transform.position;
        playerData.maxDistance = Vector3.Distance(player.playerHand.transform.position, hit.point);
        playerData.minDistance = 2.0f; //Our desired distance
        currentLength = Vector3.Distance(player.joint.transform.position, player.joint.connectedAnchor);
        player.joint.maxDistance = playerData.maxDistance;

        distanceAnimation = Vector3.Distance(player.joint.transform.position, player.joint.connectedAnchor);
    }

    public override void Exit()
    {
        player.GrappleDirectionIndicator.localRotation = Quaternion.Euler(0, 0, 0);
        ResetCancelGrapple();
        player.RB.drag = 0f;
        player.line.SetPosition(0, Vector3.zero);
        player.line.SetPosition(1,Vector3.zero);
        player.Anim.SetBool(this.animBoolName, false);
        player.InputHandler.isSwinging = false;
        animBoolName = null;
        if (!player.playerDead) { player.InputHandler.SwitchActionToGameplay(); }
        base.Exit();
    }

    private void SwingControls()
    {
        if(grappleType.Equals(GrappleTypes.swingReelInorOut))
        {
            if (player.InputHandler.NormInputZ > 0) { player.RB.AddForce(Vector3.forward * playerData.SwingThrustForce * Time.deltaTime ); }
            else if (player.InputHandler.NormInputZ < 0) { player.RB.AddForce(-Vector3.forward * playerData.SwingThrustForce * Time.deltaTime); }
            else if (player.InputHandler.NormInputX > 0) { player.RB.AddForce(Vector3.right * playerData.SwingThrustForce * Time.deltaTime); Debug.Log("Swinging Horizontally"); }
            else if (player.InputHandler.NormInputX < 0) { player.RB.AddForce(-Vector3.right * playerData.SwingThrustForce * Time.deltaTime);}

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
                Vector3 releaseForceDirection = player.RB.velocity.normalized; // Use the current velocity direction as the release direction
                player.RB.AddForce(releaseForceDirection * playerData.releaseForceMagnitude, ForceMode.Impulse);
                stateMachine.ChangeState(player.InAirState);
            }
            else if (player.InputHandler.cancelInput)
            {
                player.joint.connectedBody = null;
                player.RB.drag = 0f;
                player.joint.enableCollision = false;
                player.joint.enablePreprocessing = false;
                player.line.enabled = false;
                this.player.joint.spring = 0f;
                this.player.joint.damper = 0f;
                this.player.joint.massScale = 0f;
                isAbilityDone = true;
                player.OnDestroyJoint();
                lastGrappleTime = Time.time;
                grappleType = GrappleTypes.noGrapple;
            }
        }

    }
    private void GrappleRotation()
    {
        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;

        // Make it flat
        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        Vector3 desiredMoveDirection = forward * player.InputHandler.NormInputZ + right * player.InputHandler.NormInputX;
        desiredMoveDirection.Normalize();

        float inputY = -desiredMoveDirection.x * core.Movement.arrowDirection;
        float inputX = -player.InputHandler.NormInputZ;

        Debug.Log("The current inputY " + inputY);
        Debug.Log("The current inputX " + inputX);
        Debug.Log("The current rotation " + player.GrappleDirectionIndicator.localRotation);

        if (Mathf.Abs(inputX) > 0.1f || Mathf.Abs(inputY) > 0.1f)
        {
            rotationY -= inputY * playerData.sensitivityY * Time.deltaTime;
            rotationX -= inputX * playerData.sensitivityX * Time.deltaTime;

            rotationY = Mathf.Clamp(rotationY, playerData.minimumY, playerData.maximumY);
            rotationX = Mathf.Clamp(rotationX, playerData.minimumX, playerData.maximumX);

            Quaternion grappleRotationY = Quaternion.Euler(0f, rotationY, 0f);
            Quaternion grappleRotationX = Quaternion.Euler(rotationX, 0f, 0f);
            Quaternion targetRotation = grappleRotationY * grappleRotationX;

            Quaternion currentRotation = player.GrappleDirectionIndicator.localRotation;

            // Calculate the clamped rotation
            Quaternion clampedRotation = ClampRotation(targetRotation, currentRotation, playerData.maximumAngle);

            Quaternion newRotation = Quaternion.Slerp(currentRotation, clampedRotation, playerData.rotationSpeed * Time.deltaTime);
            player.GrappleDirectionIndicator.localRotation = newRotation;
        }
    }


    private Quaternion ClampRotation(Quaternion targetRotation, Quaternion currentRotation, float maxAngle)
{
    // Calculate the angle between the target rotation and current rotation
    Quaternion deltaRotation = Quaternion.Inverse(currentRotation) * targetRotation;

    // Convert the delta rotation to Euler angles
    Vector3 deltaEulerAngles = deltaRotation.eulerAngles;

    // Clamp the delta rotation angles
    float clampedAngleX = Mathf.Clamp(deltaEulerAngles.x, -maxAngle, maxAngle);
    float clampedAngleY = Mathf.Clamp(deltaEulerAngles.y, -maxAngle, maxAngle);
    float clampedAngleZ = Mathf.Clamp(deltaEulerAngles.z, -maxAngle, maxAngle);

    Quaternion clampedDeltaRotation = Quaternion.Euler(clampedAngleX, clampedAngleY, clampedAngleZ);

    // Calculate the clamped target rotation
    Quaternion clampedTargetRotation = currentRotation * clampedDeltaRotation;

    return clampedTargetRotation;
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
                Vector3 pointB = hit.point;

                Vector3 pointAlongLine = Vector3.Lerp(pointA, pointB, x);

                player.line.SetPosition(0, player.playerHand.transform.position);
                player.line.SetPosition(1, pointAlongLine);
            }
            else
            {
                if (grappleType.Equals(GrappleTypes.reelInorOut) || grappleType.Equals(GrappleTypes.pullingToPoint) || grappleType.Equals(GrappleTypes.swingReelInorOut)) { PullPlayer(); }
                else if (grappleType.Equals(GrappleTypes.pullingObject)) { PullObject(); }
                else if(grappleType.Equals(GrappleTypes.Enemy)) { GrappleEnemy(); }
            }
        }

    }

    private void GrappleEnemy()
    {
        /// Method for callling the method in our enemyAI script

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


        ////// Update the line's end position to the new player position

        curentEndpointPosition = Vector3.Lerp(curentEndpointPosition, player.playerHand.transform.position, Time.deltaTime * 20f);
        player.line.SetPosition(0, player.playerHand.transform.position);
        player.line.SetPosition(1, curentEndpointPosition);
        Debug.Log("The current point position " + curentEndpointPosition.magnitude + "and the player " + player.playerHand.transform.position.magnitude);
        if ((int)curentEndpointPosition.magnitude == (int)player.playerHand.transform.position.magnitude || player.InputHandler.cancelInput)
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

    private void PullPlayer()
    {
        //Challenge 2:

        if (player.joint.connectedAnchor != null)
        {
            // Reduce the joint distance over time
            if (grappleType.Equals(GrappleTypes.reelInorOut) || grappleType.Equals(GrappleTypes.swingReelInorOut))
            {
                player.InputHandler.isSwinging = true;
                Debug.Log("The current animation " + animBoolName);
                player.Anim.SetBool(animBoolName, false);
                animBoolName = "swing";
                Debug.Log("New animation name " +  animBoolName);
                player.Anim.SetBool(animBoolName, true);
                if (player.InputHandler.ReelInput < 0) 
                {
                    //player.RB.drag = 0f;
                    Vector3 directionToPoint = movingPlatform.transform.position - player.transform.position;
                    player.RB.AddForce(directionToPoint.normalized * playerData.forwardThrustForce * Time.deltaTime);
                    if(grappleType.Equals(GrappleTypes.reelInorOut)) { player.RB.drag = 6f; }

                        float distanceFromPoint = Vector3.Distance(player.transform.position, movingPlatform.transform.position);

                    player.joint.maxDistance = distanceFromPoint * 0.8f;
                    player.joint.minDistance = distanceFromPoint * 0.25f;
                }
                else if(player.InputHandler.ReelInput > 0)
                {
                    //if (grappleType.Equals(GrappleTypes.reelInorOut)) { player.RB.drag = 6f; }
                    float extendedDistanceFromPoint = Vector3.Distance(player.transform.position, movingPlatform.transform.position) + playerData.extendCableSpeed;

                    player.joint.maxDistance = extendedDistanceFromPoint * 0.8f;
                    player.joint.minDistance = extendedDistanceFromPoint * 0.15f;
                }
                else
                {
                    Debug.Log("We are not reeling");
                    player.RB.drag = 0f;
                }

            }
            else if (grappleType.Equals(GrappleTypes.pullingToPoint))
            {
                ZippingToPoint();
            }


            ////// Update the line's end position to the new player position
            player.line.SetPosition(0, player.playerHand.transform.position);
            player.line.SetPosition(1, movingPlatform.transform.position);



        }
        if (player.InputHandler.cancelInput || jumpInput)
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
            player.OnDestroyJoint();
            grappleType = GrappleTypes.noGrapple;
        }
        else if(grappleType.Equals(GrappleTypes.pullingToPoint)) 
        {
            if(Vector3.Distance(player.playerHand.transform.position, player.joint.connectedBody.transform.position) <= player.joint.connectedAnchor.magnitude + 6 || player.InputHandler.cancelInput || jumpInput)
            {
                // Stop grappling if we've reached the grapple point
                if (player.joint.connectedBody.GetComponent<Oscillator>() != null) { player.joint.connectedBody.GetComponent<Oscillator>().canMove = true; }
                else if (player.joint.connectedBody.GetComponentInParent<Oscillator>() != null) { player.joint.connectedBody.GetComponentInParent<Oscillator>().canMove = true; }
                player.joint.connectedBody = null;
                player.joint.enableCollision = false;
                player.joint.enablePreprocessing = false;
                player.line.enabled = false;
                this.player.joint.spring = 0f;
                this.player.joint.damper = 0f;
                this.player.joint.massScale = 0f;
                isAbilityDone = true;
                lastGrappleTime = Time.time;
                player.OnDestroyJoint();
                grappleType = GrappleTypes.noGrapple;
            }
        }
    }

    private void ZippingToPoint()
    {
        Debug.Log("Pulling to point");
        player.joint.maxDistance = player.joint.maxDistance - ((Time.deltaTime * 2) * playerData.grappleSpeed);

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

    private void PullObject()
    {
        float distanceToMove = 0f;
        if (player.joint.connectedAnchor != null)
        {
            if (player.joint.connectedBody.gameObject.CompareTag("Collectiable")) { player.joint.connectedBody.useGravity = false; }
            // Reduce the joint distance over time
            Vector3 playerPos = player.playerHand.transform.position;
            Vector3 grapplePos = player.joint.connectedBody.transform.position;
            Vector3 direction = (playerPos - grapplePos).normalized;

            // Calculate the difference between the current and desired distances
            float distanceDelta = currentLength - playerData.minDistance;

            // Adjust the joint's linear limit to move the object closer to the player
            Debug.Log("Pull the object");
            player.joint.maxDistance -= distanceDelta * Time.deltaTime;
            distanceToMove = Mathf.Min(distanceDelta, Time.deltaTime * playerData.pullSpeed); // Limit the distance moved per frame

            // Move the connected body towards the player, only changing X and Z coordinates
            Vector3 newPosition = player.joint.connectedBody.transform.position + direction * distanceToMove;
            newPosition.y = player.joint.connectedBody.transform.position.y; // Keep Y coordinate constant
            player.joint.connectedBody.transform.position = newPosition;

        }
         curentEndpointPosition = player.joint.connectedBody.position;

        curentEndpointPosition = Vector3.MoveTowards(curentEndpointPosition, player.playerHand.transform.position, Time.deltaTime * 1.2f);
        player.line.SetPosition(0, player.playerHand.transform.position);
        player.line.SetPosition(1, curentEndpointPosition);
        Debug.Log(Vector3.Distance(player.playerHand.transform.position, player.joint.connectedBody.transform.position));
        if (Vector3.Distance(player.playerHand.transform.position, player.joint.connectedBody.transform.position) <= 4f || player.InputHandler.cancelInput)
        {
            // Stop grappling if we've reached the grapple point
            if (player.joint.connectedBody.gameObject.CompareTag("Collectiable")) { player.joint.connectedBody.useGravity = true; }
            player.joint.connectedBody = null;
            player.joint.enableCollision = false;
            player.joint.enablePreprocessing = false;
            player.joint.connectedAnchor = Vector3.zero;
            player.line.enabled = false;
            this.player.joint.spring = 0f;
            this.player.joint.damper = 0f;
            this.player.joint.massScale = 0f;
            isAbilityDone = true;
            lastGrappleTime = Time.time;
            player.OnDestroyJoint();
            grappleType = GrappleTypes.noGrapple;
        }
    }


    public bool CheckIfCanGrapple()
    {
        return canGrapple && Time.time >= lastGrappleTime + playerData.grappleCooldown;
    }

    public void ResetCanGrapple() => canGrapple = true;

    private void ResetCancelGrapple() => player.InputHandler.cancelInput = false;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimationAndMovementController : MonoBehaviour
{
    
    PlayerInput playerInput; 
    CharacterController characterController;
    Animator animator;

    Vector2 currentMovementInput;
    Vector3 currentMovement;
    Vector3 currentRunMovement;
    bool IsMovementPressed;
    bool IsRunPressed;
    float rotationFactorPerFrame = 15.0f;
    float runMultiplier = 3.0f;

    void Awake()
    {

        playerInput = new PlayerInput();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        playerInput.CharacterControls.Move.started += onMovementInput;
        playerInput.CharacterControls.Move.canceled += onMovementInput;
        playerInput.CharacterControls.Move.performed += onMovementInput;
        playerInput.CharacterControls.Run.started += onRun;
        playerInput.CharacterControls.Run.canceled += onRun;

    }

    void onRun (InputAction.CallbackContext context){
        IsRunPressed = context.ReadValueAsButton();
    }

    void handleRotation(){
        Vector3 positionToLookAt;

        positionToLookAt.x = currentMovement.x;
        positionToLookAt.y = 0.0f;
        positionToLookAt.z = currentMovement.z;
        Quaternion currentRotation = transform.rotation;

        if (IsMovementPressed){
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationFactorPerFrame);
        }
    }

    void onMovementInput (InputAction.CallbackContext context){
        currentMovementInput = context.ReadValue<Vector2>();
        currentMovement.x = currentMovementInput.x;
        currentMovement.z = currentMovementInput.y;
        currentRunMovement.x = currentMovementInput.x * runMultiplier;
        currentRunMovement.z = currentMovementInput.y * runMultiplier;
        IsMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
    }

    void handleAnimation()
    {
        bool IsWalking = animator.GetBool("IsWalking");
        bool IsRunning = animator.GetBool("IsRunning");

        if (IsMovementPressed && !IsWalking){
            animator.SetBool("IsWalking", true);
        }

        else if (!IsMovementPressed && IsWalking){
            animator.SetBool("IsWalking", false);
        }

        if ((IsMovementPressed && IsRunPressed) && !IsRunning){
            animator.SetBool("IsRunning", true);
        }

        else if ((!IsMovementPressed || !IsRunPressed) && IsRunning){
            animator.SetBool("IsRunning", false);
        }

    }

    // void handleGravity(){
    //     if(characterController.isGrounded){
    //         float groundedGravity = -.05f;
    //         currentMovement.y = groundedGravity;
    //         currentRunMovement.y = groundedGravity;
    //     } else{
    //         float gravity = -9.8f;
    //         currentMovement.y += gravity;
    //         currentRunMovement += gravity;
    //     }
    // }

    // Update is called once per frame
    void Update()
    {
        handleRotation();
        handleAnimation();

        if(IsRunPressed){
            characterController.Move(currentRunMovement * Time.deltaTime);
        } else{
            characterController.Move(currentMovement * Time.deltaTime);
        }

        characterController.Move(currentMovement * Time.deltaTime);
    }

    void OnEnable()
    {
        playerInput.CharacterControls.Enable();
    }

    void OnDisable()
    {
        playerInput.CharacterControls.Disable();
    }
}

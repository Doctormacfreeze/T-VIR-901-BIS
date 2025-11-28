using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;




public class WatchMenusScript : MonoBehaviour
{

public GameObject[] menus;
public int currentIndex = 0;
public InputActionReference navigateAction;
private bool isAttached = false;
private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab;
private Transform watchSlot;
private Rigidbody rb;  
private bool canChangeObj = false;

    void Awake()
    {
        grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();
    }


    void Start()
    {
        foreach(GameObject menu in menus){
            menu.SetActive(false);
        }

        menus[currentIndex].SetActive(true);
        
    }

    public void reset(){
        menus[currentIndex].SetActive(false);
        menus[currentIndex].SetActive(true);
    }

    void OnEnable()
    {
        if (grab != null)
        {
            grab.selectEntered.AddListener(OnGrabbed);
            grab.selectExited.AddListener(OnGrabReleased);
        }

        if(navigateAction != null){
            navigateAction.action.started += OnNavigateMenus;
            navigateAction.action.Enable();
        }
    }

    void OnDisable()
    {

        if (grab != null)
        {
            grab.selectEntered.RemoveListener(OnGrabbed);
            grab.selectExited.RemoveListener(OnGrabReleased);
        }

        if(navigateAction != null){
            navigateAction.action.started -= OnNavigateMenus;
            navigateAction.action.Disable();
        }

    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        isAttached = false;
    }


    public void OnNavigateMenus(InputAction.CallbackContext context){

        if(isAttached == false) return;

        if(context.started)
        {
            
            Vector2 input = context.ReadValue<Vector2>();

            if(input.x > 0.05f)
            {
                NavigateRight();
            }
            
            else if(input.x < -0.05f)
            {
                NavigateLeft();
            }
        }
    }

    private void NavigateRight(){
        menus[currentIndex].SetActive(false);

        currentIndex = (currentIndex + 1) % menus.Length;

        menus[currentIndex].SetActive(true);
    }

    private void NavigateLeft(){
        menus[currentIndex].SetActive(false);

        currentIndex = (currentIndex - 1 + menus.Length) % menus.Length;

        menus[currentIndex].SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("WatchSlot")){
            watchSlot = other.transform;
        }
    }

    private void OnGrabReleased(SelectExitEventArgs args)
    {
        if(watchSlot){
            isAttached = true;

            if(canChangeObj == false && isAttached == true){
                canChangeObj = true;
                ObjectiveManager.Instance.CurrentObjectiveCompleted();
            }

            rb.isKinematic = true;
            rb.useGravity = false;

            transform.SetParent(watchSlot);
            transform.localPosition = new Vector3(0f,-0.7f,0f);
            transform.localRotation = Quaternion.Euler(0f, 180f, 0f);

            grab.enabled =  false;
        }
    }


}

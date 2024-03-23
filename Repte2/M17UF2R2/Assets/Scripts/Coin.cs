using UnityEngine;

public class Coin : Item
{
    PlayerController playerInstance;
    private void Awake()
    {
        this.Name = "Coin";
    }
    private void Start()
    {
        playerInstance = PlayerController.Instance;
    }
    public override void Collect()
    {
        GameObject parent = playerInstance.GetFreePouchSpace();
        if (parent != null)
        {
            Transform parentTransform = parent.transform;
     
            transform.SetParent(parentTransform);
            transform.localScale = Vector3.one/2;
            transform.position = parentTransform.position;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;


            PlayerController.inventory.AddToInventory(new ItemData(this));
            GetComponent<BoxCollider>().enabled = false;
            Animator animator = GetComponentInChildren<Animator>();
            animator.SetTrigger("Taken");

        }
        else {
            Debug.Log("Not enough space");
        }
      
    }

}

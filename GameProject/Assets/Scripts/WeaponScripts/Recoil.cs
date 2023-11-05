using UnityEngine;

public class Recoil : MonoBehaviour
{
    //Rotation
    private Vector3 currentRotation;
    private Vector3 targetRotation;

    public float returnSpd = 0f;
    public float snappinss = 0f;

    void Start()
    {
        
    }

    void Update()
    {
        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, returnSpd * Time.deltaTime);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, snappinss * Time.fixedDeltaTime);
        transform.localRotation = Quaternion.Euler(currentRotation);
    }

    public void RecoilFire(float recX, float recY, float recZ, float RSpeed, float Snap)
    {
        returnSpd = RSpeed;
        snappinss = Snap;
        targetRotation += new Vector3(recX, Random.Range(-recY, recY), Random.Range(-recZ, recZ));
    }
}

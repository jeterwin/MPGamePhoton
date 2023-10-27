using Fusion;
using UnityEngine;

public class PlayerWeaponController : NetworkBehaviour, IBeforeUpdate
{
    public Quaternion LocalQuaternion { get; private set; }

    [SerializeField] private Camera cam;
    [SerializeField] private Transform pivotToRotate;

    [Networked] private Quaternion currentPlayerRotation { get; set; }
    public void BeforeUpdate()
    {
        if(Runner.IsPlayerValid(Runner.LocalPlayer))
        {
	        Vector2 positionOnScreen = cam.WorldToViewportPoint(transform.position);
		
	        //Get the Screen position of the mouse
	        Vector2 mouseOnScreen = (Vector2)cam.ScreenToViewportPoint(Input.mousePosition);
		
	        //Get the angle between the points
	        float angle = AngleBetweenTwoPoints(mouseOnScreen, positionOnScreen);

	        LocalQuaternion = Quaternion.Euler(new Vector3(0f,0f,angle));
        }
    }
    float AngleBetweenTwoPoints(Vector3 a, Vector3 b) 
    {
		return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
	}
    public override void FixedUpdateNetwork()
    {
        //if(GetInput(out PlayerData input))
        if(Runner.TryGetInputForPlayer<PlayerData>(Object.InputAuthority, out var input))
        {
            currentPlayerRotation = input.GunPivotRotation;
        }

        pivotToRotate.rotation = currentPlayerRotation;
    }
}

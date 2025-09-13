using UnityEngine;

public class Player : MemberBase
{
    public bool canMove = true;
    public void Update()
    {
        if (canMove)
            UseArrowMove();
    }
    public void UseArrowMove()
    {
        Vector3 movement = Vector3.zero;

        if (Input.GetKey(KeyCode.LeftArrow)) movement.x = -1f;
        if (Input.GetKey(KeyCode.RightArrow)) movement.x = 1f;
        if (Input.GetKey(KeyCode.UpArrow)) movement.y = 1f;
        if (Input.GetKey(KeyCode.DownArrow)) movement.y = -1f;

        if (movement != Vector3.zero)
        {
            Move(movement);
        }
    }

    
}

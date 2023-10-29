using UnityEngine;

public class MouseCursorController : MonoBehaviour
{
    public Camera cam;
    public Texture2D defaultCursor;
    public Texture2D attackCursor;
    public Texture2D itemCursor;
    
    void Start()
    {
        ChangeCursor(defaultCursor);
    }

    void Update()
    {
        if(MultiScene.Instance.isDead) return;
        ChangeCursorBasedOnMouseOver();
    }

    void ChangeCursorBasedOnMouseOver()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        int layerMask = ~(1 << LayerMask.NameToLayer("Player")); //Player 레이어 무시

        if (Physics.Raycast(ray, out var hit, Mathf.Infinity, layerMask))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                ChangeCursor(attackCursor);
            }
            else if (hit.collider.CompareTag("Item"))
            {
                ChangeCursor(itemCursor);
            }
            else
            {
                ChangeCursor(defaultCursor);
            }
        }
    }

    void ChangeCursor(Texture2D cursorTexture)
    {
        Cursor.SetCursor(cursorTexture, new Vector2(cursorTexture.width / 3, 0), CursorMode.Auto);
    }
}
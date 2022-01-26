using UnityEngine;

public class Tile : MonoBehaviour
{
	[SerializeField] private SpriteRenderer spriteRenderer;
	private bool isSelected;

	private Vector3 normalScale;
	private Vector3 selectScale;

	public SpriteRenderer SpriteRenderer => spriteRenderer;
	public bool IsSelected => isSelected;

	private void Start()
	{
		normalScale = gameObject.transform.localScale;
		selectScale = new Vector3(normalScale.x * 1.2f, normalScale.y * 1.2f, normalScale.z);
	}

	public bool IsEnabled
	{
		get
		{
			return spriteRenderer.enabled;
		}
	}

	public void Select()
	{
		isSelected = true;
		gameObject.transform.localScale = selectScale;
	}

	public void Deselect()
	{
		isSelected = false;
		gameObject.transform.localScale = normalScale;
	}

	public void Disable()
	{
		spriteRenderer.enabled = false;
	}

	public void Enabled()
	{
		spriteRenderer.enabled = true;
	}
}

using UnityEngine;

public class Game : MonoBehaviour
{
	[SerializeField] private Board board;
	private GameSetting gameSetting;

	private void Awake()
	{
		gameSetting = GetComponent<GameSetting>();
	}
	void Start()
	{
		board.Init(gameSetting.xSize, gameSetting.ySize, gameSetting.tilePref, gameSetting.colorsTile);
		board.CreateBoard();
	}

	void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			RaycastHit2D ray = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition));
			if(ray != false)
			{
				board.CheckSelectTile(ray.collider.gameObject.GetComponent<Tile>());
				board.CheckShiftTile();
			}
		}
	}
}

using UnityEngine;
using System.Collections.Generic;

public class Board : MonoBehaviour
{
	private int xSize, ySize;
	private Tile tilePref;
	private Color[] colorsTile;

	private Tile[,] arrTile;
	private Tile oldSelectTile;

	private bool isFindMatch;
	private bool isShift;
	private bool isSearchDisableTile;

	public event System.Action DelTile;

	private Vector2[] dirRay = new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

	private List<Tile> shiftTiles = new List<Tile>();

	public void Init(int xSize, int ySize, Tile tilePref, Color[] colors)
	{
		this.xSize = xSize;
		this.ySize = ySize;
		this.tilePref = tilePref;
		colorsTile = colors;
	}
	
	public void CreateBoard()
	{
		arrTile = new Tile[xSize, ySize];
		float xPos = transform.position.x;
		float yPos = transform.position.y;
		Vector2 tileSize = tilePref.SpriteRenderer.bounds.size;

		for(int i = 0; i < xSize; i++)
		{
			for(int j = 0; j < ySize; j++)
			{
				Tile newTile = Instantiate(tilePref);
				newTile.transform.position = new Vector3(xPos + tileSize.x * i, yPos + tileSize.y * j, 0);
				newTile.transform.parent = transform;

				arrTile[i, j] = newTile;
				List<Color> tempArr = new List<Color>();
				tempArr.AddRange(colorsTile);
				if (j > 0)
					tempArr.Remove(arrTile[i, j - 1].SpriteRenderer.color);
				if (i > 0)
					tempArr.Remove(arrTile[i - 1, j].SpriteRenderer.color);
				newTile.SpriteRenderer.color = tempArr[Random.Range(0, tempArr.Count)];
			}
		}
	}

	public void CheckSelectTile(Tile tile)
	{
		if (!tile.IsEnabled || isShift)
		{
			return;
		}

		if (tile.IsSelected)
		{
			DeselectTile(tile);
		}
		else if(oldSelectTile == null)
		{
			SelectTile(tile);
		}
		else if(NeighboringTiles().Contains(tile))
		{
			SwapTwoTiles(tile);
			bool tile1 = FindAllMatch(tile);
			bool tile2 = FindAllMatch(oldSelectTile);
			if(!tile1 && !tile2)
			{
				SwapTwoTiles(tile);
			}
			DeselectTile(oldSelectTile);
		}
		else
		{
			DeselectTile(oldSelectTile);
			SelectTile(tile);
		}
	}

	public void CheckShiftTile()
	{
		if (isSearchDisableTile)
		{
			SearchDisableTile();
		}
	}

	private void SelectTile(Tile tile)
	{
		tile.Select();
		oldSelectTile = tile;
	}

	private void DeselectTile(Tile tile)
	{
		tile.Deselect();
		oldSelectTile = null;
	}

	private void SwapTwoTiles(Tile tile)
	{
		if(oldSelectTile.SpriteRenderer.color == tile.SpriteRenderer.color)
		{
			return;
		}
		Color oldColor = oldSelectTile.SpriteRenderer.color;
		oldSelectTile.SpriteRenderer.color = tile.SpriteRenderer.color;
		tile.SpriteRenderer.color = oldColor;
	}

	private List<Tile> NeighboringTiles()
	{
		List<Tile> tiles = new List<Tile>();
		for(int i = 0; i < dirRay.Length; i++)
		{
			RaycastHit2D hit = Physics2D.Raycast(oldSelectTile.transform.position, dirRay[i]);
			if(hit.collider != null)
			{
				tiles.Add(hit.collider.gameObject.GetComponent<Tile>());
			}
		}
		return tiles;
	}

	private List<Tile> FindMatch(Tile tile, Vector2 dir)
	{
		List<Tile> findTiles = new List<Tile>();
		RaycastHit2D hit = Physics2D.Raycast(tile.transform.position, dir);
		while(hit.collider != null && hit.collider.gameObject.GetComponent<Tile>().SpriteRenderer.color == tile.SpriteRenderer.color)
		{
			findTiles.Add(hit.collider.gameObject.GetComponent<Tile>());
			hit = Physics2D.Raycast(hit.collider.gameObject.transform.position, dir);
		}
		return findTiles;
	}

	private bool DeleteMatch(Tile tile, Vector2[] dirArr)
	{
		List<Tile> delTiles = new List<Tile>();
		for(int i = 0; i < dirArr.Length; i++)
		{
			delTiles.AddRange(FindMatch(tile, dirArr[i]));
		}
		if(delTiles.Count >= 2)
		{
			for(int i = 0; i < delTiles.Count; i++)
			{
				delTiles[i].Disable();
				DelTile?.Invoke();
			}
			return true;
		}
		return false;
	}

	private bool FindAllMatch(Tile tile)
	{
		if (!tile.IsEnabled)
		{
			return false;
		}
		bool vertical = DeleteMatch(tile, new Vector2[2] { Vector2.up, Vector2.down });
		bool horizontal = DeleteMatch(tile, new Vector2[2] { Vector2.left, Vector2.right });
		if (vertical || horizontal)
		{
			tile.Disable();
			DelTile?.Invoke();
			isSearchDisableTile = true;
			return true;
		}
		return false;
	}

	private void FindAllMatchAfterShift()
	{
		for(int i = 0; i < shiftTiles.Count; i++)
		{
			FindAllMatch(shiftTiles[i]);
		}
		shiftTiles.Clear();
		CheckShiftTile();
	}

	private void SearchDisableTile()
	{
		for(int i = 0; i < xSize; i++)
		{
			for(int j = 0; j < ySize; j++)
			{
				if (!arrTile[i, j].IsEnabled)
				{
					ShiftTileDown(i, j);
					shiftTiles.AddRange(GetShiftTile(i, j));
				}
			}
		}
		isSearchDisableTile = false;
		if (shiftTiles.Count > 0)
		{
			FindAllMatchAfterShift();
		}
	}

	private List<Tile> GetShiftTile(int xPos, int yPos)
	{
		List<Tile> tiles = new List<Tile>();
		for(int y = yPos; y < ySize; y++)
		{
			tiles.Add(arrTile[xPos, y]);
		}
		return tiles;
	}

	private void ShiftTileDown(int xPos, int yPos)
	{
		isShift = true;

		for (int y = yPos; y < ySize - 1; y++)
		{
			arrTile[xPos, y].SpriteRenderer.color = arrTile[xPos, y + 1].SpriteRenderer.color;
			if(arrTile[xPos, y + 1].IsEnabled)
			{
				arrTile[xPos, y].Enabled();
			}
		}
		arrTile[xPos, ySize - 1].SpriteRenderer.color = GetNewColor(xPos, ySize - 1);
		arrTile[xPos, ySize - 1].Enabled();

		if(!arrTile[xPos, yPos].IsEnabled)
		{
			ShiftTileDown(xPos, yPos);
		}

		isShift = false;
	}

	private Color GetNewColor(int xPos, int yPos)
	{
		List<Color> colors = new List<Color>();
		colors.AddRange(colorsTile);
		if(xPos > 0)
		{
			colors.Remove(arrTile[xPos - 1, yPos].SpriteRenderer.color);
		}
		if(xPos < xSize - 1)
		{
			colors.Remove(arrTile[xPos + 1, yPos].SpriteRenderer.color);
		}
		if(yPos > 0)
		{
			colors.Remove(arrTile[xPos, yPos - 1].SpriteRenderer.color);
		}

		return colors[Random.Range(0, colors.Count)];
	}
}

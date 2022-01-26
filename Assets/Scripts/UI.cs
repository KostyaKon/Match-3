using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{

	[SerializeField] Board board;
	[SerializeField] float reward = 100f;
	[SerializeField] Text textScore;

	private float score;

	private void Awake()
	{
		board.DelTile += ChangeScore;
	}

	private void Start()
	{
		score = 0;
		textScore.text = "Score: " + score.ToString();
	}

	private void ChangeScore()
	{
		score += reward;
		textScore.text = "Score: " + score.ToString();
	}
}

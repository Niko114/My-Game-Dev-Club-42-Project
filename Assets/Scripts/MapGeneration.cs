using System.Collections.Generic;
using UnityEngine;

public class MapGeneration : MonoBehaviour
{
	[Header ("Variables")]
	public int			seed;
	public Vector2		lowest_border = new(-6.5f, -4.5f);
	public Vector2		highest_border = new(6.5f, 4.5f);
	public float		size = 1;	//size of wall blocks
	public float		pow_in_calc;
	[Header ("Stuff")]
	public GameObject	pref_wall;
	public GameObject	go_parent;
	public float[]		prob_case;
	public float[]		calc_probs;

	private List<GameObject>	walls = new();

	void Start()
	{
		CreateWalls();
	}

	void CreateWalls()
	{
		//Vector2		lowest_border = new(-6.5f, -4.5f);
		//Vector2		highest_border = new(6.5f, 4.5f);
		//float		size = 1;
		int[,]		Field;

		//set size of field
		Field = new int[
			highest_border.x >= lowest_border.x ?
				Mathf.FloorToInt((highest_border.x - lowest_border.x) / size) + 1 :
				0,
			highest_border.y >= lowest_border.y ?
				Mathf.FloorToInt((highest_border.y - lowest_border.y) / size) + 1 :
				0
			];
		//Set seed for 'random' functions
		if (seed == 0)
			seed = Random.Range(0, int.MaxValue);
		Random.InitState(seed);

		PathAlgo(Field);

		//print field in console
		{
			string	printer = "";
			for (int i = Field.GetLength(1) - 1; i >= 0; i--)	// y / rows
			{
				for (int j = 0; j < Field.GetLength(0); j++)	// x / columns
					printer += Field[j, i];
				printer += "\n";
			}
			print(printer);
		}
		//Instantiate walls
		for (int i = 0; i < Field.GetLength(0); i++)	//x
		{
			for (int j = 0; j < Field.GetLength(1); j++)	//y
			{
				if (Field[i, j] == 0)
				{
				
					GameObject go = Instantiate(pref_wall);
					walls.Add(go);
					go.transform.position = new(
						lowest_border.x + i * size,
						lowest_border.y + j * size,
						-1.5f);
					go.transform.localScale = new(size, size, size);
					go.transform.parent = go_parent.transform;
				}
			}
		}
	}

	void PathAlgo(int[,] Field)
	{
		//prob_case: up[0]; down[1]; right / continue[2]
		Vector2Int	curr;	//current spot(coordinate in the field(list)

		//Set the first tile of path
		curr = new(0, Field.GetLength(1) - 1); //-1 for wally walls-----
		while (curr.y == Field.GetLength(1) - 1) //-1 for wally walls-----
			//or while (curr.y >= Field.GetLength(1) - 1 || curr.y == 0)
		{
			//curr.y = Mathf.FloorToInt(Random.Range(0, Field.GetLength(1)));
			curr.y = Mathf.FloorToInt(Random.Range(0 + 1, Field.GetLength(1) - 1)); //for wally walls-----
			if (curr.y == Field.GetLength(1) - 1) //-1 for wally walls-----
				print("It got the Random.value = 1 case, hooray");
		}
		Field[curr.x, curr.y] = 1;

		//Modify probability
		calc_probs = new float[Field.GetLength(1)];	//calculated probabilities (i = distance from border in looking direction)
		for (int i = 0; i < calc_probs.Length; i++)
			//calc_probs[i] = Mathf.Pow(2f * i / calc_probs.Length, 2) * 2;
			calc_probs[i] = Mathf.Pow(2f * (i - 1) / calc_probs.Length, pow_in_calc) * 2; //for wally walls-----

		//Set all other path tiles
		while (curr.x < Field.GetLength(0) - 1)
		{
			prob_case[0] = curr.y < Field.GetLength(1) - 1 && curr.x > 0 ?
				calc_probs[Field.GetLength(1) - 1 - curr.y]
				* (GetInt(Field, curr.x, curr.y + 1) == 0 ? 1 : 0)
				* (GetInt(Field, curr.x - 1, curr.y + 1) == 0 ? 1 : 0) :
				0;
			prob_case[1] = curr.y > 0 && curr.x > 0 ?
				calc_probs[curr.y]
				* (GetInt(Field, curr.x, curr.y - 1) == 0 ? 1 : 0)
				* (GetInt(Field, curr.x - 1, curr.y - 1) == 0 ? 1 : 0) :
				0;
			float	prob_sum = prob_case[0] + prob_case[1] + prob_case[2];
			float	ran = Random.Range(0, prob_sum);

			if (ran < prob_case[0])
				curr.y++;
			else if (ran < prob_case[1])
				curr.y--;
			else
				curr.x++;
			if (curr.y >= Field.GetLength(1) || curr.y < 0)
			{
				print("Error occured");
				return;
			}
			Field[curr.x, curr.y] = 1;
		}
	}

	int GetInt(int[,] list, int x, int y)
		=> x < list.GetLength(0) && x >= 0 && y < list.GetLength(1) && y >= 0 ?
			list[x, y] :
			1;

	void	Ft_Reload()
	{
		//Remove all walls
		for (int i = 0; i < walls.Count; i++)
			Destroy(walls[i]);
		walls.Clear();

		seed = 0;
		CreateWalls();
	}

	private void OnGUI()
	{
		Event e = Event.current;
		//if (e.type != EventType.Repaint && e.type != EventType.Layout)
		//	print(e.type);
		if (e.type == EventType.MouseUp)
			Ft_Reload();
	}
}

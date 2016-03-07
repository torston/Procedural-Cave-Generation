using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour 
{
	public int bulletsCount = 100;
	public int nodes = 1000;


	public int Nodes
	{
		get{ return nodes;}
		set 
		{
			if (value != nodes) 
			{
				nodes = value;
				nodesL.text = nodes.ToString ();
				
			} 
		}
		
	}

	public int Bullets
	{
		get{ return bulletsCount;}
		set 
		{
			if (value != bulletsCount) 
			{
				bulletsCount = value;
				bullets.text = bulletsCount.ToString ();

			} 
		}

	}

	public Text bullets;
	public Text nodesL;
	public Button reset;

	public static Menu Instance;


	void Awake() {

		Instance = this;
		reset.onClick.AddListener(Reset); 
		Nodes = 1000;
		Bullets = 100;
	}

	public void Reset() {
		SceneManager.LoadScene (0);
	}
}

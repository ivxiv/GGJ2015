using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Hub : MonoBehaviour 
{
	[SerializeField]
	private GameObject[] m_puzzles = new GameObject[0];

	[SerializeField]
	private GameObject[] m_completedText = new GameObject[0];

	[SerializeField]
	private GameObject m_youWin = null;

	HashSet<int> m_completedPuzzles = new HashSet<int>();

	private int m_currentPuzzle = 0;

	public void Start()
	{
		GameManager.Instance.puzzleCompleteCallback = () => 
		{
			m_completedPuzzles.Add( m_currentPuzzle );

			for( int i = 0; i < m_puzzles.Length; ++i )
			{
				m_puzzles[i].SetActive( false );
			}
			
			gameObject.SetActive (true);

			for( int i = 0; i < m_completedText.Length; ++i )
			{
				m_completedText[i].SetActive( m_completedPuzzles.Contains(i) );
			}

			WinCheck();
		};
	}

	public void OnPuzzleClicked( int puzzleIndex )
	{
		Debug.Log (puzzleIndex);

		if( m_completedPuzzles.Contains( puzzleIndex ) )
		{
			return;
		}

		for( int i = 0; i < m_puzzles.Length; ++i )
		{
			m_puzzles[i].SetActive( i == puzzleIndex );
		}

		gameObject.SetActive (false);

		m_currentPuzzle = puzzleIndex;
	}

	private void WinCheck()
	{
		if( m_completedPuzzles.Count == m_puzzles.Length )
		{
			foreach( Button puzzle in GetComponentsInChildren<Button>() )
			{
				puzzle.gameObject.SetActive( false );
			}

			m_youWin.SetActive(true);
		}
	}


}

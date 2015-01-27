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
	
	const int kPicturesPuzzle= 0;
	const int kOuijaPuzzle= 1;
	const int kDollsPuzzle= 2;
	const int kCandlesPuzzle= 3;
	const int kTVPuzzle= 4;

	public void Start()
	{
		GameManager.Instance.puzzleCompleteCallback = () => 
		{
			m_completedPuzzles.Add( m_currentPuzzle );

			/* GameManager will do this
			for( int i = 0; i < m_puzzles.Length; ++i )
			{
				m_puzzles[i].SetActive( false );
			}
			*/
			
			gameObject.SetActive (true);

			for( int i = 0; i < m_completedText.Length; ++i )
			{
				m_completedText[i].SetActive( m_completedPuzzles.Contains(i) );
			}

			WinCheck();
			
			switch (m_currentPuzzle)
			{
				case kPicturesPuzzle: GameManager.Instance.SetDesiredGameState(GameManager.eGameState.PictureSwapPuzzleOutro); break;
				case kOuijaPuzzle: GameManager.Instance.SetDesiredGameState(GameManager.eGameState.OuijaPuzzleOutro); break;
				case kDollsPuzzle: GameManager.Instance.SetDesiredGameState(GameManager.eGameState.DollsPuzzleOutro); break;
				case kCandlesPuzzle: GameManager.Instance.SetDesiredGameState(GameManager.eGameState.CandelabraPuzzleOutro); break;
				case kTVPuzzle: GameManager.Instance.SetDesiredGameState(GameManager.eGameState.TVPuzzleOutro); break;
			}
			
		};
	}

	public void OnPuzzleClicked( int puzzleIndex )
	{
		if( m_completedPuzzles.Contains( puzzleIndex ) )
		{
			return;
		}

		/* GameManager will do this
		for( int i = 0; i < m_puzzles.Length; ++i )
		{
			m_puzzles[i].SetActive( i == puzzleIndex );
		}
		*/

		gameObject.SetActive (false);

		m_currentPuzzle = puzzleIndex;
		
		switch (m_currentPuzzle)
		{
			case kPicturesPuzzle: GameManager.Instance.SetDesiredGameState(GameManager.eGameState.PictureSwapPuzzleIntro); break;
			case kOuijaPuzzle: GameManager.Instance.SetDesiredGameState(GameManager.eGameState.OuijaPuzzleIntro); break;
			case kDollsPuzzle: GameManager.Instance.SetDesiredGameState(GameManager.eGameState.DollsPuzzleIntro); break;
			case kCandlesPuzzle: GameManager.Instance.SetDesiredGameState(GameManager.eGameState.CandelabraPuzzleIntro); break;
			case kTVPuzzle: GameManager.Instance.SetDesiredGameState(GameManager.eGameState.TVPuzzleIntro); break;
		}
	}

	private void WinCheck()
	{
		if( m_completedPuzzles.Count == m_puzzles.Length )
		{
            Button[] puzzles = GetComponentsInChildren<Button>();

            foreach (Button puzzle in puzzles)
			{
				puzzle.gameObject.SetActive( false );
			}

			m_youWin.SetActive(true);
		}
	}


}

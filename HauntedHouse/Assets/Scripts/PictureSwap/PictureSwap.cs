using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PictureSwap : Puzzle
{
	private class Affinities
	{
		public FamilyMember m_lovedOne;
		public FamilyMember m_hatedOne;
		
		public Affinities(FamilyMember lovedOne, FamilyMember hatedOne)
		{
			m_lovedOne = lovedOne;
			m_hatedOne = hatedOne;
		}
	}
	
	[SerializeField] private List< Picture >	    m_pictures;
	[SerializeField] private List< Hanger >         m_hangers;
	[SerializeField] private AudioClip				m_successClip;
	[SerializeField] private AudioClip				m_swapClip;
	
	private Dictionary< FamilyMember, Affinities >    m_solution;
	
	public List< Picture >  Pictures
	{
		get
		{
			return m_pictures;
		}
	}
	
	public bool CanSelectPicture
	{
		get
		{
			return true;
		}
	}
	
	void Awake()
	{
		m_solution = new Dictionary< FamilyMember, Affinities >();
		CreateSolution();
		RandomizePositions();
	}
	
	protected override void OnTimeUp()
	{
	}
	
	protected override void OnComplete()
	{
		GameManager.Instance.OnPuzzleComplete();
		Debug.Log ("OnComplete called");
	}
	
	public void CheckForWin()
	{
		if (CheckSolution())
		{
			GameManager.Instance.PlaySoundHauntedClient(m_successClip);
			GameManager.Instance.PlaySoundPsychicServer(m_successClip);
			Debug.Log("CORRECT SOLUTION");
			
			StartCoroutine(StartWinSequence());
		}
	}
	
	public void PlaySwapSound()
	{
		GameManager.Instance.PlaySoundHauntedClient(m_swapClip);
		//GameManager.Instance.PlaySoundPsychicServer(m_swapClip);
	}
	
	private bool CheckSolution()
	{
		int correctCount = 0;
		
		foreach (Picture pic in m_pictures)
		{
			Affinities solutionAffinity = m_solution[pic.FamilyMember];
			
			if (solutionAffinity.m_lovedOne != FamilyMember.NONE)
			{
				if (pic.Hanger.Neighbors.Contains(solutionAffinity.m_lovedOne))
				{
					++correctCount;
				}
			}
			else if (solutionAffinity.m_hatedOne != FamilyMember.NONE)
			{
				if (!pic.Hanger.Neighbors.Contains(solutionAffinity.m_hatedOne))
				{
					++correctCount;
				}
			}
			else
			{
				++correctCount;
			}
		}
		
		if (correctCount == 5)
		{
			return true;
		}
		else
		{
			return false;
		}
	}
	
	private void CreateSolution()
	{
		RandomizePositions();
		
		foreach (Picture pic in m_pictures)
		{
			pic.ChooseClue(pic.Hanger.Neighbors);
			
			switch (pic.ChosenSound.m_affinity)
			{
			case Affinity.Love:
				m_solution.Add(pic.FamilyMember, new Affinities(pic.ChosenSound.m_familyMember, FamilyMember.NONE));
				break;
				
			case Affinity.Hate:
				m_solution.Add(pic.FamilyMember, new Affinities(FamilyMember.NONE, pic.ChosenSound.m_familyMember));
				break;
				
			case Affinity.NONE:
				m_solution.Add(pic.FamilyMember, new Affinities(FamilyMember.NONE, FamilyMember.NONE));
				break;
			}
		}
		
		while (CheckSolution())
		{
			RandomizePositions();
		}
	}
	
	private void RandomizePositions()
	{
		List< Picture > pictures = new List< Picture >(m_pictures);
		List< Hanger > hangers = new List< Hanger >(m_hangers);
		
		foreach (Picture pic in pictures)
		{
			int index = Random.Range(0, hangers.Count);
			pic.Hanger = hangers[index];
			
			hangers.RemoveAt(index);
		}
	}
	
	private IEnumerator StartWinSequence()
	{
		foreach (Picture pic in m_pictures)
		{
			pic.StartSpinAnimation();
			
			yield return new WaitForSeconds(0.4f);
		}
		
		yield return new WaitForSeconds(1.5f);
		
		OnComplete();
	}
}

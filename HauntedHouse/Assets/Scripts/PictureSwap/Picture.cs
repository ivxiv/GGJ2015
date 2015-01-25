using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Picture : MonoBehaviour
{
	[System.Serializable]
	public class FeedbackSound
	{
		public FamilyMember m_familyMember = FamilyMember.NONE;
		public Affinity     m_affinity = Affinity.NONE;
		public AudioClip    m_clip = null;
	}
	
	private const int       kTotalShakes = 2;
	private const string    kShakeAnimationName = "Shake";
	private const string	kSpinAnimationName = "Spin";
	private const float     kEpsilon = 1.0f;
	
	[SerializeField] private FamilyMember           m_familyMember;
	[SerializeField] private List< FeedbackSound >  m_feedbackSounds;
	
	private Animation   m_animation;
	private PictureSwap m_controller;
	private Hanger      m_hanger;
	
	private bool    m_isShaking;
	private int     m_shakes;
	
	private FeedbackSound   m_chosenSound; 
	
	private Vector3 m_startingPosition;
	
	public Hanger Hanger
	{
		get
		{
			return m_hanger;
		}
		set
		{
			m_hanger = value;
			
			transform.parent = m_hanger.transform;
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;
			
			if (m_animation != null)
			{
				m_animation.Play(kShakeAnimationName);
			}
		}
	}
	
	public FamilyMember FamilyMember
	{
		get
		{
			return m_familyMember;
		}
	}
	
	public FeedbackSound ChosenSound
	{
		get
		{
			return m_chosenSound;
		}
	}
	
	void Start()
	{
		m_animation = GetComponent< Animation >();
		m_controller = GetComponentInParent< PictureSwap >();
		m_hanger = GetComponentInParent< Hanger >();
		
		m_isShaking = false;
		m_shakes = 0;
	}
	
	void Update()
	{
		if (m_isShaking)
		{
			UpdateShake();
		}
	}
	
	void OnMouseDown()
	{
		SpriteRenderer[] sprites = GetComponentsInChildren< SpriteRenderer >();
		
		foreach (SpriteRenderer sprite in sprites)
		{
			sprite.sortingOrder += 2;
		}
		
		m_startingPosition = Input.mousePosition;
	}
	
	void OnMouseUp()
	{
		SpriteRenderer[] sprites = GetComponentsInChildren< SpriteRenderer >();
		
		foreach (SpriteRenderer sprite in sprites)
		{
			sprite.sortingOrder -= 2;
		}
		
		Picture toSwapPic = null;
		float toPicDistance = Mathf.Infinity;
		
		foreach (Picture pic in m_controller.Pictures)
		{
			if (pic != this)
			{
				BoxCollider2D otherCollider = pic.GetComponent< BoxCollider2D >();
				Rect otherRect = new Rect(otherCollider.transform.position.x - (otherCollider.size.x * 0.5f),
				                          otherCollider.transform.position.y + (otherCollider.size.y * 0.5f),
				                          otherCollider.size.x,
				                          otherCollider.size.y);
				
				BoxCollider2D thisCollider = GetComponent< BoxCollider2D >();
				Rect thisRect = new Rect(transform.position.x - (thisCollider.size.x * 0.5f),
				                         transform.position.y + (thisCollider.size.y * 0.5f),
				                         thisCollider.size.x,
				                         thisCollider.size.y);
				
				if (otherRect.Overlaps(thisRect))
				{
					float newDistance = (pic.transform.position - transform.position).magnitude;
					
					if (newDistance < toPicDistance)
					{
						toSwapPic = pic;
						toPicDistance = newDistance;
					}
				}
			}
		}
		
		if (toSwapPic == null)
		{
			transform.localPosition = new Vector3(0.0f, 0.0f, transform.localPosition.z);
			GameManager.Instance.PlaySoundPsychicServer(m_chosenSound.m_clip);
			StartShake();
			
			string affinityString;
			switch (m_chosenSound.m_affinity)
			{
			case Affinity.Love:
				affinityString = "loves";
				break;
				
			case Affinity.Hate:
				affinityString = "hates";
				break;
				
			default:
				affinityString = "is ambivalent towards";
				break;
			}
			
			string selectedMemberName;
			switch (m_familyMember)
			{
			case FamilyMember.Dad:
				selectedMemberName = "Dad";
				break;
				
			case FamilyMember.Daughter:
				selectedMemberName = "Daughter";
				break;
				
			case FamilyMember.Dog:
				selectedMemberName = "Dog";
				break;
				
			case FamilyMember.Grandma:
				selectedMemberName = "Grandma";
				break;
				
			case FamilyMember.Son:
				selectedMemberName = "Son";
				break;
				
			default:
				selectedMemberName = "Ghost of Cthulu";
				break;
			}
			
			string targetMemberName;
			switch (m_chosenSound.m_familyMember)
			{
			case FamilyMember.Dad:
				targetMemberName = "Dad";
				break;
				
			case FamilyMember.Daughter:
				targetMemberName = "Daughter";
				break;
				
			case FamilyMember.Dog:
				targetMemberName = "Dog";
				break;
				
			case FamilyMember.Grandma:
				targetMemberName = "Grandma";
				break;
				
			case FamilyMember.Son:
				targetMemberName = "Son";
				break;
				
			default:
				targetMemberName = "Ghost of Cthulu";
				break;
			}
			
			Debug.Log(selectedMemberName + " " + affinityString + " " + targetMemberName);
		}
		else
		{
			Hanger tempHanger = toSwapPic.Hanger;
			
			toSwapPic.Hanger = m_hanger;
			Hanger = tempHanger;
			
			m_controller.PlaySwapSound();
			m_controller.CheckForWin();
		}
	}
	
	void OnMouseDrag()
	{
		if ((Input.mousePosition - m_startingPosition).magnitude > kEpsilon)
		{
			Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			transform.position = new Vector3(mousePos.x, mousePos.y, transform.position.z);
		}
	}
	
	public void StartShake()
	{
		m_animation.Play(kShakeAnimationName);
		m_isShaking = true;
		m_shakes = 0;
	}
	
	public void ChooseClue(List< FamilyMember > lovedOnes)
	{
		List< FeedbackSound > possibleSounds = new List< FeedbackSound >();
		
		foreach (FeedbackSound fbSound in m_feedbackSounds)
		{
			if ((lovedOnes.Contains(fbSound.m_familyMember) && fbSound.m_affinity == Affinity.Love) ||
			    !lovedOnes.Contains(fbSound.m_familyMember) && fbSound.m_affinity == Affinity.Hate)
			{
				possibleSounds.Add(fbSound);
			}
		}
		
		if (possibleSounds.Count <= 0)
		{
			m_chosenSound = m_feedbackSounds[0];
		}
		else
		{
			int randIndex = Random.Range(0, possibleSounds.Count);
			m_chosenSound = possibleSounds[randIndex];
		}
	}
	
	public void StartSpinAnimation()
	{
		m_animation.Play(kSpinAnimationName);
	}
	
	private void UpdateShake()
	{
		if (!m_animation.isPlaying)
		{
			++m_shakes;
			
			if (m_shakes >= kTotalShakes)
			{
				m_isShaking = false;
			}
			else
			{
				m_animation.Play(kShakeAnimationName);
			}
		}
	}
}

using UnityEngine;
using System.Collections.Generic;

using UnityEngine.UI;
using SpaceSandbox;

using UnityEngine.EventSystems;

public class UIController : MonoBehaviour
{
	static public UIController Instance { get; private set; }

	public GameObject m_selectionGroupPrefab;

	public RectTransform m_selectListTransform;
	public RectTransform m_commandsTransform;
	public RectTransform m_hpBarTransform;
	public RectTransform m_devInterface;
	

	private Dictionary<ContainerView, GameObject> selections = new Dictionary<ContainerView, GameObject>();

	private CommandsStack commands = new CommandsStack();
	private DeveloperInterface devUI;

	private void Awake()
	{
		Instance = this;
		devUI = m_devInterface.GetComponent<DeveloperInterface>();
	}

	private void Start()
	{
		commands.InitializeStack(m_commandsTransform, 5);
	}

	private void Update()
	{
		UtilsKeys();
		CameraControls();
	}
	
	private void UtilsKeys()
	{
		if( Input.GetKeyUp(KeyCode.R) )
		{
			Application.LoadLevel( Application.loadedLevel );
		}

		if( Input.GetKeyUp(KeyCode.Escape) )
		{
			devUI.CleanAllContent();
			m_devInterface.gameObject.SetActive(false);
			EventSystem.current.SetSelectedGameObject( null );
		}
	}
	
	private void CameraControls()
	{
		if( EventSystem.current.currentSelectedGameObject == null )
		{
			Vector3 input = Vector3.zero; 
			if( Input.GetKey(KeyCode.UpArrow) )
				input += Vector3.forward;
			
			if( Input.GetKey(KeyCode.DownArrow) )
				input += Vector3.back;
			
			if( Input.GetKey(KeyCode.LeftArrow) )
				input += Vector3.left;
			
			if( Input.GetKey(KeyCode.RightArrow) )
				input = Vector3.right;
			
			Camera.main.transform.position += input * Time.deltaTime;
		}
		
		Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize + Input.GetAxis( "Mouse ScrollWheel"), 1f, 10f);
	}

	public void OnContainerSelected( ContainerView container )
	{
		m_hpBarTransform.gameObject.SetActive(true);
		commands.InitializeContainerView(container);

		if( selections.ContainsKey( container ) )
		{
			selections[container].SetActive(true);
			return;
		}

		GenerateNewSelection( container );
	}

	public void OnContainerDeselected( ContainerView container )
	{
	//	m_devInterface.gameObject.SetActive(false);
		selections[container].SetActive(false);
		commands.CleanCommandsStack();
	}

	public void OnContainerUpdated( ContainerView container )
	{
		selections[container].transform.localPosition = Camera.main.WorldToScreenPoint( container.transform.position );
		
		Ship ship = container.m_contain as Ship;
		Asteroid aster = container.m_contain as Asteroid;
		
		if( ship != null )
			selections[container].transform.FindChild("Cargo").GetComponent<Text>().text = "Cargo: " +
				ship.m_cargo.SpaceTaken.ToString("0.0") + " / " + ship.m_cargo.Capacity.ToString("0.0");
		
		if( aster != null )
			selections[container].transform.FindChild("Cargo").GetComponent<Text>().text = "Cargo: " +
				aster.Containment.Amount.ToString("0.0");



		Vector3 position = Vector3.zero;
		position.x = container.transform.position.x;
		position.z = container.transform.position.z;
		position.y = Camera.main.transform.position.y;


		Camera.main.transform.position = position;
	}

	public void OnContainerDeveloperConsole( ContainerView container )
	{
		Ship ship = container.m_contain as Ship;
		if( ship != null )
		{
			m_devInterface.gameObject.SetActive(true);
			devUI.InitializeInteface(ship);
		}
	}
	
	private void GenerateNewSelection( ContainerView container )
	{
		Vector2 selectionScreenPos = Camera.main.WorldToScreenPoint( container.transform.position );

		GameObject newSelection = Instantiate(m_selectionGroupPrefab);
		newSelection.transform.parent = m_selectListTransform;
		
		RectTransform newSelectTransform = newSelection.GetComponent<RectTransform>();
		newSelectTransform.localPosition = selectionScreenPos;
		newSelectTransform.localRotation = Quaternion.identity;

		newSelectTransform.localScale = Vector3.one;

		newSelection.transform.FindChild("Name").GetComponent<Text>().text = container.name;

		
		selections.Add( container, newSelection );
	}
	

	private void OnDestroy()
	{
		selections.Clear();
	}
}

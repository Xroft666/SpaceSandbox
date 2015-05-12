using UnityEngine;

using SpaceSandbox;
using System.Collections.Generic;

using BehaviourScheme;

[RequireComponent(typeof( BoxCollider2D ))]
[RequireComponent(typeof( EventTriggerInitializer ))]
public class ContainerRepresentation : MonoBehaviour 
{

	public Container m_contain = new Container();
	
	private void Awake()
	{
//		DTimer timer1 = new DTimer();
//		
//		timer1.SetUpTimer(2f);
//		
//		m_contain.IntegratedDevice.InstallEquipment ( new List<Device>(){ timer1 } );
//		
//		BlueprintScheme scheme = new BlueprintScheme();
//		
//		BSEntry entry1 = scheme.CreateEntry("OnTimerTrigger", timer1);
//
//
//		Device containerDevice = m_contain.IntegratedDevice;
//		
//		BSExit exit1 = scheme.CreateExit("exit1", containerDevice);
//		BSExit exit2 = scheme.CreateExit("exit2", containerDevice);
//		
//
//		BSEntry entry2 = scheme.CreateEntry("exit1", containerDevice);
//
//		entry1.AddChild( exit1 );
//		entry2.AddChild( exit2 );
	}

	private void Start()
	{
		m_contain.Initialize();
	}

	private void Update()
	{
		m_contain.Update();
	}

	private void OnDestroy()
	{
		m_contain.Delete();
	}

//	public List<Container> _contains = new List<Container>();
//
//	void Start () 
//	{
//		foreach( Container container in _contains )
//		{
//			foreach( Entity entity in container.cargo )
//			{
//				GameObject entityDetail = new GameObject(entity.name); 
//				EntityRepresentation representation = entityDetail.AddComponent<EntityRepresentation>();
//				representation.detailToRepresent = entity;
//			}
//		}
//	}
	

}

﻿using System;
using System.Text;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SharpNeat.Phenomes;
using Voxel2D;

public class ShipBuilderBrain : UnitController {

	Dictionary<Voxel2D.IntVector2,int> takenPosition = new Dictionary<Voxel2D.IntVector2, int>();

    public VoxelSystem voxelSystem{get; private set; }
	IBlackBox box;

	int[] blockCounts;

	int shipSize = 10;

	bool invalid = false;

	bool isRunning = false;

    public int _wallHits = 0; 
	float _closestDistance = Mathf.Infinity;

    public float _totalRotation = 0;
    private float _prevRot;


    private string objectName = "E1";
    private string path = Application.dataPath + "/Data/SavedVoxelSystems/";

	List<Engine> engines = new List<Engine>();
    public float _targetScore;

    enum BlockType
	{
		engine
	}
	
	// Use this for initialization
	void Awake () {

//		GameObject g = new GameObject("Ship");
		gameObject.layer = 8;
		gameObject.transform.position = Vector3.zero;//new Vector3(Random.Range(-10,10),Random.Range(-10,10),0);
	
		voxelSystem = gameObject.AddComponent<VoxelSystem>();
		voxelSystem.rigidbody2D.drag = 1;
	    voxelSystem.rigidbody2D.angularDrag = 1;
		voxelSystem.SetGridSize(shipSize);
	}
	
	// Update is called once per frame
	void Update () {
		// testing goes here
		// let's say we need to move the vessel from A to B through bunch of obstalces

		// here we use the brain:
		// let's assume that we have inputs:
		// 	- sensors which tell where are the obstacles
		//  the output would be:
		//  - which direction to take

		// and the fitness would be distance to the goal

		if( isRunning )
		{
			
            /*
            float frontSensor = 0f;
			float leftFrontSensor = 0f;
			float leftSensor = 0f;
			float rightFrontSensor = 0f;
			float rightSensor = 0f;
			float SensorRange = 10f;

			RaycastHit2D hit;
			LayerMask mask = 1 << 9; // "Obstacles" layer

			hit = Physics2D.Raycast(transform.position, transform.TransformDirection(new Vector3(0f, 1f, 0f).normalized), SensorRange, mask);
			if( hit.collider != null )
				frontSensor = 1f - hit.distance / SensorRange;

			hit = Physics2D.Raycast(transform.position, transform.TransformDirection(new Vector3(0.5f, 1f, 0f).normalized), SensorRange, mask);
			if( hit.collider != null )
				rightFrontSensor = 1f - hit.distance / SensorRange;

			hit = Physics2D.Raycast(transform.position, transform.TransformDirection(new Vector3(1f, 0f, 0f).normalized), SensorRange, mask);
			if( hit.collider != null )
				rightSensor = 1f - hit.distance / SensorRange;

			hit = Physics2D.Raycast(transform.position, transform.TransformDirection(new Vector3(-0.5f, 1f, 0f).normalized), SensorRange, mask);
			if( hit.collider != null )
				leftFrontSensor = 1f - hit.distance / SensorRange;

			hit = Physics2D.Raycast(transform.position, transform.TransformDirection(new Vector3(-1f, 0f, 0f).normalized), SensorRange, mask);
			if( hit.collider != null )
				leftSensor = 1f - hit.distance / SensorRange;
            */

			ISignalArray inputArr = box.InputSignalArray;
			FillInputs(ref inputArr);
            /*
            inputArr[0] = frontSensor;
			inputArr[1] = leftFrontSensor;
			inputArr[2] = leftSensor;
			inputArr[3] = rightFrontSensor;
			inputArr[4] = rightSensor;

			inputArr[5] = (voxelSystem.transform.position - GotoTarget.Position).magnitude;
			*/
			box.Activate();
			
			ISignalArray outputArr = box.OutputSignalArray;

			engines.Clear();

			foreach(VoxelData e in voxelSystem.GetVoxelData())
				if(e is Engine)
					engines.Add( (Engine)e );

			for( int i = 0; i < engines.Count; i++ )
			{
				float pullForce = (float) outputArr[i];
				engines[i].OnActivate( pullForce );
			}
			
//			float steer = (float)outputArr[0] * 2f - 1f;
//			float gas = (float)outputArr[1] * 2f - 1f;

//			List<Engine> leftEngines = new List<Engine>();
//			List<Engine> rightEngines = new List<Engine>();
//			List<Engine> forwardEngines = new List<Engine>();
//			List<Engine> backwardEngines = new List<Engine>();
			
//			CollectThrusters(ref leftEngines, ref rightEngines, ref forwardEngines, ref backwardEngines);
//			InputThrusters(gas, steer, ref leftEngines, ref rightEngines, ref forwardEngines, ref backwardEngines);


			float currentDist = (voxelSystem.transform.position - GotoTarget.Position).magnitude;
			if( currentDist < _closestDistance )
				_closestDistance = currentDist;


		    _totalRotation += Mathf.Abs(voxelSystem.transform.rotation.eulerAngles.z - _prevRot);
		    _prevRot = voxelSystem.transform.rotation.eulerAngles.z;
		}
	}

    public void FillInputs(ref ISignalArray inputArr)
    {
        Vector3 shipPos = voxelSystem.transform.TransformPoint(voxelSystem.GetCenter());

        Vector3 targetDir = shipPos - GotoTarget.Position;
        Vector3 shipDir = voxelSystem.transform.up;

        float angle = Vector3.Angle(shipDir, targetDir);
        Vector3 cross = Vector3.Cross(shipDir, targetDir);
        if (cross.y < 0) angle = -angle;

        angle = angle / 360;

        inputArr[0] = angle;

        inputArr[1] = Mathf.Clamp((shipPos - GotoTarget.Position).magnitude / 100, 0, 1);

        Vector3 localDeltaPos = voxelSystem.transform.InverseTransformPoint(GotoTarget.Position);
        localDeltaPos.x = Mathf.Clamp(localDeltaPos.x/100, 0, 1);
        localDeltaPos.y = Mathf.Clamp(localDeltaPos.y / 100, 0, 1);

        inputArr[2] = localDeltaPos.x;
        inputArr[3] = localDeltaPos.y;

        inputArr[4] = Mathf.Clamp(voxelSystem.rigidbody2D.velocity.magnitude, 0, 25);
        inputArr[5] = Mathf.Clamp(voxelSystem.rigidbody2D.angularVelocity, 0, 10);

        inputArr[6] = Mathf.Clamp((voxelSystem.transform.rotation.z - _prevRot), -1, 1);
    }

	public void CollectThrusters(ref List<Engine> left, ref List<Engine> right, ref List<Engine> forward, ref List<Engine> backward)
	{
		foreach(VoxelData e in voxelSystem.GetVoxelData())
		{
			if(e is Engine)
			{
				switch(e.rotation)
				{
				case 0:
					forward.Add((Engine)e);
					break;
				case 90:
					left.Add((Engine)e);
					break;
				case 270:
					right.Add((Engine)e);
					break;
				case 180:
					backward.Add((Engine)e);
					break;
				}
			}
		}
	}
	
    
	public void InputThrusters(float gas, float steer, ref List<Engine> left, ref List<Engine> right, ref List<Engine> forward, ref List<Engine> backward )
	{
		foreach(Engine e in forward)
			if( gas > 0f )
				e.OnActivate(gas);

		foreach(Engine e in forward)
			if( gas == 0f && e.enabled )
				e.OnDeactivate();

		foreach(Engine e in backward)
			if( gas > 0f )
				e.OnActivate(gas);

		foreach(Engine e in backward)
			if( gas == 0f && e.enabled )
				e.OnDeactivate();

		foreach(Engine e in right)
			if( steer > 0f )
				e.OnActivate(steer);

		foreach(Engine e in right)
			if( gas == 0f && e.enabled )
				e.OnDeactivate();

		foreach(Engine e in left)
			if( steer < 0f )
				e.OnActivate(steer);

		foreach(Engine e in left)
			if( gas == 0f && e.enabled )
				e.OnDeactivate();
	}


	IEnumerator StopCall(float seconds)
	{
		yield return new WaitForSeconds(seconds);
		isRunning = false;
	}
	
	public override void Activate(IBlackBox box, params object[] blackBoxExtraData){
		this.box = box;
//		bool running = true;
		isRunning = true;

	    _targetScore = 0;
	    _totalRotation = 0;

		//GenerateVoxelSystem((List<VoxelRawData>) blackBoxExtraData[0]);
        Load();

//		StartCoroutine( StopCall(3f) );
//		while(running){
//			running = NextStep();
//		}

		return;
	}

    
    private void Load()
    {
        VoxelSystemDataConverter VSD = Serializer.Load<VoxelSystemDataConverter>(path + objectName + ".space");
        VoxelSystem voxelSystem = GetComponent<VoxelSystem>();
        VSD.FillVoxelSystem(ref voxelSystem);
    }

	private void GenerateVoxelSystem(List<VoxelRawData> voxelData)
	{
		foreach( VoxelRawData voxel in voxelData )
		{
			Voxel2D.IntVector2 localCoord = new Voxel2D.IntVector2(voxel._xPos, voxel._yPos);
			
			if(!takenPosition.ContainsKey(localCoord) && voxelSystem.CanAddVoxel(localCoord))
			{
				takenPosition.Add(localCoord,voxel._deviceType);
				VoxelData vd = null;

				int elementType = voxel._materialType;
				
			    int rotationAngle = voxel._rotation*90;
				

				switch( voxel._deviceType )
				{
				case 0:
					vd = new Wall(elementType,localCoord,rotationAngle,voxelSystem);
					break;
				case 1:
					vd = new Cannon(elementType,localCoord,rotationAngle,voxelSystem,10,1);
					break;
				case 2:
					vd = new Laser(elementType,localCoord,rotationAngle,voxelSystem,250);
					break;
				case 3:
					vd = new Engine(elementType,localCoord,rotationAngle,voxelSystem,100);
					break;
				case 4:
					vd = new Wall(elementType,localCoord,rotationAngle,voxelSystem);
					break;
				default:
					break;
				}

				voxelSystem.AddVoxel(vd);
			}
		}
	}
	
	public override void Stop(){
		Destroy(voxelSystem.gameObject);
	}
	
	public override float GetFitness(){

		
//		VoxelData[,] data = voxelSystem.GetVoxelData();
//		blockCounts = new int[4];
//		foreach(VoxelData vd in data){
//			if(vd != null && vd.GetType().Name == "Wall"){
//				blockCounts[0]++;
//			}else if(vd != null && vd.GetType().Name == "Cannon"){
//				blockCounts[1]++;
//			}else if(vd != null && vd.GetType().Name == "Laser"){
//				blockCounts[2]++;
//			}else if(vd != null && vd.GetType().Name == "Engine"){
//				blockCounts[3]++;
//			} 
//		}
//        return blockCounts[0]+blockCounts[1]*2+blockCounts[2]*3+blockCounts[3]*10;



		/*
        if(invalid){
			//return 0;
		}*/
		//Debug.Log("fitness "+voxelSystem.voxelCount);
		
//		return voxelSystem.voxelCount;
		 
		//return voxelSystem.voxelCount;


        /*
		VoxelData[,] data = voxelSystem.GetVoxelData();

		int engines = 0;
		foreach(VoxelData vd in data)
			if(vd != null && vd.GetType().Name == "Engine")
				engines++;

		return engines;
		*/

		// fitness for flying the maximum distance
//		return voxelSystem.transform.position.magnitude;
        /*
        Vector3 shipPos = voxelSystem.transform.TransformPoint(voxelSystem.GetCenter());

	    float fitness = 100;
	    fitness -= (shipPos-GotoTarget.Position).magnitude;

	    fitness -= _wallHits*0.01f;
	    fitness -= Mathf.Clamp(voxelSystem.rigidbody2D.velocity.magnitude/100, 0, 10);
        fitness -= Mathf.Clamp(voxelSystem.rigidbody2D.angularVelocity/10, 0, 10);
        fitness -= Mathf.Clamp(_totalRotation/100,0,25);

	    fitness = Mathf.Clamp(fitness, 0,999999);
        */
	    return FitnessFunction.GetFitness(this);
	}
	
//	bool NextStep(){
//
//		VoxelData[,] data = voxelSystem.GetVoxelData();
//		
//		blockCounts = new int[4];
//		
//		foreach(VoxelData vd in data){
//			if(vd != null && vd.GetType().Name == "Wall"){
//				blockCounts[0]++;
//			}else if(vd != null && vd.GetType().Name == "Cannon"){
//				blockCounts[1]++;
//			}else if(vd != null && vd.GetType().Name == "Laser"){
//				blockCounts[2]++;
//			}else if(vd != null && vd.GetType().Name == "Engine"){
//				blockCounts[3]++;
//			} 
//		}
//		
//		ISignalArray inputArr = box.InputSignalArray;
//
//		for(int i=0;i<blockCounts.Length;i++){
//			inputArr[i] = blockCounts[i];
//		}
//		
//		box.Activate();
//		
//		ISignalArray outputArr = box.OutputSignalArray;
//		
//		int type = Mathf.Abs(Mathf.RoundToInt((float)outputArr[0]*4));
//		int x = Mathf.Abs(Mathf.RoundToInt((float)outputArr[1]*shipSize));
//		int y = Mathf.Abs(Mathf.RoundToInt((float)outputArr[2]*shipSize));
//		int rot = Mathf.Abs(Mathf.RoundToInt((float)outputArr[3])*90);
//		Voxel2D.IntVector2 iv2 = new Voxel2D.IntVector2(x,y);
//
//		if(!takenPosition.ContainsKey(iv2) && voxelSystem.CanAddVoxel(iv2)){
//			takenPosition.Add(iv2,type);
//			VoxelData vd = null;
//			if(type == 0){
//				vd = new Wall(1,iv2,rot,voxelSystem);
//			}else if(type == 1){
//				vd = new Cannon(1,iv2,rot,voxelSystem,10,1);
//			}else if(type == 2){
//				vd = new Laser(1,iv2,rot,voxelSystem,250);
//			}else if(type == 3){
//				vd = new Engine(1,iv2,rot,voxelSystem,100);
//			}else{
//				vd = new Wall(1,iv2,rot,voxelSystem);
//			}
//			voxelSystem.AddVoxel(vd);
//			//print("ADDED VOXEL "+voxelSystem.voxelCount);
//			return true;
//		}else{
//			//Debug.Log("invalid "+voxelSystem.voxelCount);
//			//invalid = true;
//			return false;
//		}
//	}

	
	/*
	void OnGUI(){
		if(GUI.Button(new Rect(0,0,50,50),"NextShip")){
			NextStep();
		}
	}*/


	void OnCollisionEnter2D(Collision2D collision)
	{
        _wallHits++; 
	}

    void OnTriggerStay2D(Collider2D col)
    {
        if (col.tag == "Target")
        {
            _targetScore += 1f;
        }
    }
}

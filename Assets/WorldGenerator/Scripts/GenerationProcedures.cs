﻿using UnityEngine;
using System.Collections;


namespace WorldGen{
	public class GenerationProcedures{

		int[,] map;

		public GenerationProcedures(ref int[,] map)
		{
			this.map = map;
		}

		void RunIteration(CellularAutomata c, int rounds)
		{
			for (int i=0; i<rounds; i++) {
				c.nextIteration();
			}
		}
		
		public void GenAstroid()
		{

			CellularAutomata CA = new CellularAutomata(ref map);
			CellularAutomata.CaveConfig.SquareRules rules = CA.caveConfig.squareRules;
			CA.neighborType = CellularAutomata.NeighborType.Square;
			
			CA.ClearMapEdges(2);

			int rounds = 0;

			rules.blackChangeThreshold = 0.55f;
			rules.whileChangeThreshold = 0.55f;
			rules.radius = 2;
			rounds = 10;
			RunIteration (CA,rounds);

			rules.blackChangeThreshold = 0.6f;
			rules.whileChangeThreshold = 0.6f;
			rules.radius = 1;
			rounds = 2;
			RunIteration (CA,rounds);

			map = CA.GetMap();
		}

		/*
		void method2(){
			CellularAutomata CA = new CellularAutomata(voxelSize,voxelSize);
			CellularAutomata.CaveConfig.SquareRules r = CA.caveConfig.squareRules;
			
			CA.neighborType = CellularAutomata.NeighborType.Square;
			
			r.blackChangeThreshold = 0.6f;
			r.whileChangeThreshold = 0.6f;
			r.radius = 1;
			RunIteration (CA,25);
			
		}
		
		void method3(){
			CellularAutomata CA = new CellularAutomata(voxelSize,voxelSize);
			CellularAutomata.CaveConfig.SquareRules r = CA.caveConfig.squareRules;
			
			CA.neighborType = CellularAutomata.NeighborType.Square;
			
			
			r.blackChangeThreshold = 0.7f;
			r.whileChangeThreshold = 0.7f;
			r.radius = 3;
			RunIteration (CA,3);
			
			r.blackChangeThreshold = 0.6f;
			r.whileChangeThreshold = 0.53f;
			r.radius = 4;
			RunIteration (CA,2);
			
			r.blackChangeThreshold = 0.6f;
			r.whileChangeThreshold = 0.6f;
			r.radius = 1;
			RunIteration (CA,10);
		}
		*/

		
	}
}

﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Voxel2D{
	public static class VoxelIslandDetector{
		
		/*
		public static bool CalculateIslandStartingPoints(bool [,] binaryImage, out IslandDetector.Region[] islands, out IslandDetector.Region[] seaRegions) {
			IslandDetector mIslandDetector = new IslandDetector();
			
			int[,] islandClassificationImage = null;
			islands = null;
			seaRegions = null;
			
			mIslandDetector.DetectIslandsFromBinaryImage(binaryImage, out islandClassificationImage, out islands, out seaRegions);
			return (islands.Length > 0);	
		}*/
		
		public static List<bool[,]> findIslands(bool[,] land) {
			List<bool[,]> islands = new List<bool[,]>();
			bool[,] visited  = new bool[land.GetLength(0),land.GetLength(0)];
			for(int x=0;x<land.GetLength(0);x++){
				for(int y=0;y<land.GetLength(1);y++){
					if(land[x,y] && !visited[x,y]) { // new island detected
						bool[,] island = new bool[land.GetLength(0),land.GetLength(1)];
						visitNeighbours(x, y, land, ref visited, ref island);
						islands.Add(island);
					} 
				}
			}
			return islands;
		}
		
		private static void visitNeighbours(int x, int y, bool[,] land,ref bool[,] visited,ref bool[,] island) {
			if(land[x,y] && !visited[x,y]) {
				visited[x,y] = true;
				island[x,y] = true;
				if(x<land.GetLength(0)-1){
					visitNeighbours(x+1, y, land, ref visited,ref island);
				}
				if(x>0){
					visitNeighbours(x-1, y, land, ref visited,ref island);
				}
				if(y<land.GetLength(1)-1){
					visitNeighbours(x, y+1, land, ref visited,ref island);
				}
				if(y>0){
					visitNeighbours(x, y-1, land, ref visited,ref island);
				}
				
			}else if(!visited[x,y]){
				visited[x,y] = true;
			}
		}
		
		public static List<bool[,]> SplitAndReturnOtherIslands(VoxelData[,] grid, VoxelSystem vox){
			List<bool[,]> islands = findIslands(VoxelUtility.VoxelDataToBool(grid));
			if(islands.Count >0){
				islands.RemoveAt(0);
				//List<bool[,]> toReturn = new List<bool[,]>();
				if(islands.Count>0){
					foreach(bool[,] island in islands){
						//toReturn.Add(island);
						
						VoxelData[,] voxelDataGrid = new VoxelData[grid.GetLength(0),grid.GetLength(1)];
						for (int x = 0; x < grid.GetLength(0); x++) {
							for (int y = 0; y < grid.GetLength(1); y++) {
								if(island[x,y]){
									voxelDataGrid[x,y] = grid[x,y];
								}
							}
						}
						
						GameObject g = new GameObject("Astroid Piece "+Random.seed);
						g.transform.position = vox.gameObject.transform.position;
						g.transform.rotation = vox.gameObject.transform.rotation;
						VoxelSystem v = g.AddComponent<VoxelSystem>();
						v.SetVoxelGrid(voxelDataGrid);
						g.rigidbody2D.velocity = vox.rigidbody2D.velocity;
						g.rigidbody2D.angularVelocity = vox.rigidbody2D.angularVelocity;
						
					}
					
				}else{
					
				}
				return islands;
			}return null;
			
		}
		
		/// <summary>
		/// Splits voxel grid into islands, fills current voxel grid with first island, and creates and fills new voxelsystems for excess islands.
		/// </summary>
		/// <returns>The islands.</returns>
		/// <param name="grid">Grid.</param>
		/// <param name="vox">Voxel system.</param>
		public static VoxelData[,] SplitAndReturnFirstIslands(VoxelData[,] grid, VoxelSystem vox){
			List<bool[,]> islands = findIslands(VoxelUtility.VoxelDataToBool(grid));
			if(islands.Count>0){
				List<VoxelData[,]> voxelIslands = new List<VoxelData[,]>();
				for(int i=0;i<islands.Count;i++){
					voxelIslands.Add (new VoxelData[grid.GetLength(0),grid.GetLength(1)]);
					
					for (int x = 0; x < grid.GetLength(0); x++) {
						for (int y = 0; y < grid.GetLength(1); y++) {
							if(islands[i][x,y]){
								voxelIslands[i][x,y] = grid[x,y];
							}
						}
					}
					if(i==0){
						//FillVoxelGrid(voxelIslands[i]);
					}else{
						GameObject g = new GameObject("Astroid Piece "+Random.seed);
						g.transform.position = vox.gameObject.transform.position;
						g.transform.rotation = vox.gameObject.transform.rotation;
						VoxelSystem v = g.AddComponent<VoxelSystem>();
						v.SetVoxelGrid(voxelIslands[i]);
						g.rigidbody2D.velocity = vox.rigidbody2D.velocity;
						g.rigidbody2D.angularVelocity = vox.rigidbody2D.angularVelocity;
						
					}
				}
				return(voxelIslands[0]);
			}else{
				return null;
			}
			
			
			
		}
	}
}
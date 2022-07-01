using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PossionDiskSampling
{
	public static List<Vector3> CreatePoints(float radius, int k, float mapSize,
	float XOriginOffset, float YOriginOffset)
	{
		List<Vector3> symbolPoints = new List<Vector3>();


		float SquaredRadius = Mathf.Pow(radius, 2);

		/*
			Setup the grid of a specific size with default values
		*/
		Vector2[] GridPoints; //the vectors, each of which lie inside a single grid square
		float CellSize = radius / Mathf.Sqrt(2); //on the actual grid, the size of a cell

		//determine the width and height of the grid of points, using the actual space avaliable(IslandSize) and the size of each cell(CellSize)
		int GridWidth = Mathf.CeilToInt(mapSize / CellSize); //round down to nearest whole number so can fit into an array
		int GridHeight = Mathf.CeilToInt(mapSize / CellSize); //clamped at 1 so no matter what size have, at least 1 grid sqaure exists

		GridPoints = new Vector2[GridWidth * GridHeight];

		/*
			Add an inital first value to the grid
		*/
		//set first point to be just the centre of the map
		float InitalXValue = mapSize / 2;
		float InitalYValue = mapSize / 2;

		//determine the index of the point within the grid i.e the grid cell the point belongs too
		int XPosition = Mathf.Clamp(Mathf.FloorToInt(InitalXValue / CellSize), 0, GridWidth - 1);
		int YPosition = Mathf.Clamp(Mathf.FloorToInt(InitalYValue / CellSize), 0, GridHeight - 1);
		int InitialGridLocation = YPosition * GridWidth + XPosition;
		//add the inital point into the grid
		GridPoints[InitialGridLocation] = new Vector2(InitalXValue, InitalYValue);


		//add the point as a valid location. Need the offset value as grid values centred around 0,0 so will shift the point to be around the maps location
		symbolPoints.Add(new Vector3(InitalXValue + XOriginOffset, 0, InitalYValue + YOriginOffset));


		//a list of all points which can still have a symbol placed around it
		List<int> ActiveList = new List<int>(); //holds the index of the symbols grid location within the GridPoints Array
		ActiveList.Add(InitialGridLocation); //add the initial grid location too it

		/*
			Add / check new points which get added until no new ones can exist
		*/
		while (ActiveList.Count > 0 && GridWidth * GridHeight > 1) //while a point can still have neighbours continue to add/ check
		{
			//random active index of the grid
			int ActiveGridIndex = Random.Range(0, ActiveList.Count - 1);
			int ActiveGridIndexValue = ActiveList[ActiveGridIndex]; //get the value stored at this grid location
			Vector2 ActiveIndexLocation = GridPoints[ActiveGridIndexValue]; //get the vertex of the point within this grid location

			//test upto k points until a valid one around 2*Radius of the ActivePoint is found or all k return invalid
			bool bValidCandidate = false;
			for (int a = 0; a < k; a++)
			{
				//determine location of new point to check as anywhere in a circle around active location
				float Angle = Random.Range(0.0f, 1.0f) * Mathf.PI * 2;
				Vector2 OffsetDirection = new Vector2(Mathf.Cos(Angle), Mathf.Sin(Angle));//get a random direction to offset the current active point by
				float OffsetDistance = 2 * radius; //the distance to offset the new point from the active point by

				//determine the location and cell the offset point belongs too
				Vector2 OffsetPosition = ActiveIndexLocation + OffsetDirection * OffsetDistance; //the new point will be an offset of the active point, in a random direction based on above parameters
				int OffsetGridXPosition = Mathf.Clamp(Mathf.FloorToInt(OffsetPosition.x / CellSize), 0, GridWidth - 1); //the position of this item within the grid, clamped between the min and max size of the grid
				int OffsetGridYPosition = Mathf.Clamp(Mathf.FloorToInt(OffsetPosition.y / CellSize), 0, GridHeight - 1);

				//due to the setup only need to check the neighbours grid cell one cell away from the current point
				bool bOffsetValid = true;
				for (int i = -1; i <= 1; i++) //loop through all neighbouring grid points
				{
					for (int j = -1; j <= 1; j++)
					{
						//as long as the new point falls within the bounds of the map it can be used
						if (OffsetGridXPosition + j >= 0 && OffsetGridXPosition + j < GridWidth && OffsetGridYPosition + i >= 0 && OffsetGridYPosition + i < GridHeight)
						{
							int NeighbourGridIndex = (OffsetGridYPosition + i) * GridWidth + (OffsetGridXPosition + j);
							float Distance = Mathf.Pow(Vector2.Distance(OffsetPosition, GridPoints[NeighbourGridIndex]), 2); //determine the distance between the active point and its neighbour's location
							if (Distance < SquaredRadius) //if the offset point testing is too close to another already existing point
								bOffsetValid = false;
						}
						else //as outside array bounds it is also invalid so do not use this point
							bOffsetValid = false;
					}
				}

				//if the offseted point is valid then add it as a new point
				if (bOffsetValid)
				{
					int OffsetGridIndex = (OffsetGridYPosition) * GridWidth + (OffsetGridXPosition); //get grid location to use
					GridPoints[OffsetGridIndex] = OffsetPosition; //assign the postion to it


					//add the point as a valid location
					symbolPoints.Add(new Vector3(OffsetPosition.x + XOriginOffset, 0, OffsetPosition.y + YOriginOffset));
					ActiveList.Add(OffsetGridIndex); //add the index of the point to the active list

					bValidCandidate = true;
					break; //as valid point added, no need to do more searching with this point
				}

			}
			if (!bValidCandidate) //as no valid point was found, remove it from the active list so no new points can spawn around it
				ActiveList.Remove(ActiveGridIndexValue);
		}

		return symbolPoints; //return the list of points with appropriate locations found
	}
}

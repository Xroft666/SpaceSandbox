﻿using UnityEngine;
using System.Collections;


namespace MaterialSystem{
	public class ElementStats{
		public ElementStats(int ID){
			this.ID = ID;
			e = ElementList.Instance.elements[ID];
		}
		private ElementSpecs e;

		public int ID;

		public float sizeModifier = 0; 	//how much of the full size the object is

		public float temperature;	//temperature
		public float fragmention;	//increases each impact, overall strength is (hardness*flexibility)*(1-fragmentationRate)

		/// <summary>
		/// Gets the energy required to break this voxel
		/// </summary>
		/// <value>The destruction energy.</value>
		public float destructionEnergy {
			get{
				return e.flexibility*e.hardness*(1-fragmention)*sizeModifier;
			}
		}
	
		public float mass{
			get{
				return e.mass*sizeModifier;
			}
		}
	
	
	
	
	
	}
}

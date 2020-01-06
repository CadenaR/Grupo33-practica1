using UnityEngine;
using System.Collections;
using System;

namespace Assets.Scripts.Grupo33.Solucion1
{
    public class Node : IComparable {
		
		public float gCost;
		public float fCost;
		public bool isVisited;
		public Node parent;
		public int[] position; // Sera un arreglo de dos componentes [ColumnID,RowID]
		public int direction;
		
		public Node() {
			this.fCost = 0.0f;
			this.gCost = 1.0f;
			this.parent = null;
			this.isVisited = false;
			this.direction = -1;
		}

		public Node(int[] position) {
			this.fCost = 0.0f;
			this.gCost = 1.0f;
			this.isVisited = false;
			this.parent = null;
			this.position = position;
			this.direction = -1;
		}

		//Se modifica este método para que el sort de la lista ordene los nodos
		public int CompareTo(object obj) {
			Node node = (Node)obj;
			
			if (this.fCost < node.fCost)
			return -1;
			
			if (this.fCost > node.fCost) return 1;
			return 0;
		}
	}
}

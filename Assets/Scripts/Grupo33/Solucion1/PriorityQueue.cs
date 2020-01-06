using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Grupo33.Solucion1
{	
	public class PriorityQueue {
		private List<Node> nodes = new List<Node>();
		
		public int Length {
			get { return this.nodes.Count; }
		}

		public bool Contains(Node node) {
			return this.nodes.Contains(node);
		}

		public Node First() {
			if (this.nodes.Count > 0) {
			return this.nodes[0];
			}
			return null;
		}

		public void Push(Node node) {
			this.nodes.Add(node);
			this.nodes.Sort();
		}

		public void Remove(Node node) {
			this.nodes.Remove(node);
			//Ensure the list is sorted
			this.nodes.Sort();
		}
	}
}
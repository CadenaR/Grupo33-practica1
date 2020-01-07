using System.Collections.Generic;
using System.Linq;
//using System.Runtime.Remoting.Messaging;
using Assets.Scripts.DataStructures;
using Assets.Scripts.Grupo33.Solucion2;
using UnityEngine;

namespace Assets.Scripts.Grupo33.Solucion1
{
    public class AStar : AbstractPathMind {
		private Node[,] nodes { get; set; }
		private bool nodesSet = false;
		private List<Node> openList;
		private Stack<int> currentPlan;
		int direction = -1;
		private bool allEliminated = false;

		public override void Repath()
        {
            currentPlan.Clear();
			
        }

		//Método para movimiento del personaje
		public override Locomotion.MoveDirection GetNextMove(BoardInfo board, CellInfo currentPos, CellInfo[] goals){
			//si hay enemigos en la lista y aun no han sido eliminados, procede a perseguirlos
			if(!allEliminated){
				SearchEnemy getEnemy = new SearchEnemy();
				direction = getEnemy.Search(board, currentPos, goals);
				if(direction == 9999)
					allEliminated = true;
			}		
			else{ //Al no haber enemigos, avanza a la meta
				if(!nodesSet){
					CreateNodesBoard(board);
					currentPlan = FindPath(board, nodes[currentPos.ColumnId,currentPos.RowId], nodes[goals[0].ColumnId,goals[0].RowId]);
					nodesSet = true;
				} 
				if(currentPlan.Count() > 0){
					direction = currentPlan.Pop();
				}

			}
			// Se lee el número asignado a "direction" para hacer el movimiento segun el caso
			switch(direction){
				case 0:
					return Locomotion.MoveDirection.Up;

				case 1:
					return Locomotion.MoveDirection.Right;

				case 2:
					return Locomotion.MoveDirection.Down;

				case 3:
					return Locomotion.MoveDirection.Left;
				
				default:
					return Locomotion.MoveDirection.None;
			}
			
			
		}

		//Crea el tablero representado en nodos
		private void CreateNodesBoard(BoardInfo board){
			int cols = board.NumColumns;
			int rows = board.NumRows;
			nodes = new Node[cols,rows];
			for (int i = 0; i < cols; i++) {
				for (int j = 0; j < rows; j++) {
					Node node = new Node(new int[] {i,j});
					nodes[i, j] = node;
				}
			}
		}

		/*	
		Metodo A*: Asigna el nodo de inicio a la lista abierta para luego comenzar a expandir 
		los nodos vecinos verificando que no fueran visitados anteriormente, posteriormente 
		asigna los valores de la fórmula f(n) = g(n) + h(n), asigna la dirección que fue tomada
		para llegar a ellos y como nodo padre el nodo que se esta expandiendo. Finalmente se agrega
		a la lista abierta cada nodo no visitado, se elimina el actual y se ordena la lista abierta.
		*/
		private Stack<int> FindPath(BoardInfo board, Node start, Node goal) {
			openList = new List<Node>();
			openList.Add(start);
			start.gCost = 0.0f;
			start.fCost = ManhatanDistance(start, goal);
			Node node = null;
			while (openList.Count != 0) {
				node = openList[0];
					//Revisa si el nodo actual es el nodo meta
				if (node.position == goal.position ) {
					return CalculatePath(node);
				}
				//Crea una lista para almacenar la posición y dirección tomada 
				List<int[]> neighbours = GetNeighbours(node, board);
				for (int i = 0; i < neighbours.Count; i++) {
					int[] neighbourNode = neighbours[i];
					int x = neighbourNode[0];
					int y = neighbourNode[1];
					if (!nodes[x,y].isVisited) {
						float neighbourHCost = ManhatanDistance(nodes[x,y], goal);
						nodes[x,y].gCost += node.gCost; //Asigna el nuevo costo al vecino
						nodes[x,y].parent = node; //Asigna al nodo vecino el nodo padre (nodo actual)
						nodes[x,y].direction = neighbourNode[2]; // Asigna la dirección tomada para llegar a él
						nodes[x,y].fCost = nodes[x,y].gCost + neighbourHCost;
						if (!openList.Contains(nodes[x,y])) {
							openList.Add(nodes[x,y]);
							openList.Sort();
						}
					}
				}
				//Nodo se cambia a visitado
				node.isVisited = true;
				//se elimina de la lista abierta
				openList.Remove(node);
				openList.Sort();
			}
			if (node.position != goal.position) {
				Debug.LogError("Goal Not Found");
				return null;
			}
			return CalculatePath(node);
		}

		/*
		Regresa la posición y dirección en que se encuantran los vecinos a los que se puede acceder 
		utilizando el metodo de CellInfo WalkableNeighbours
		*/
		private List<int[]> GetNeighbours (Node node, BoardInfo board){
			List<int[]> neighbours = new List<int[]>();
			CellInfo currentCell = board.CellInfos[node.position[0],node.position[1]];
			CellInfo[] walkNeighbours = currentCell.WalkableNeighbours(board);
			for(int i = 0; i < 4; i++){
				if(walkNeighbours[i] != null){
					neighbours.Add(new int[] {walkNeighbours[i].ColumnId, walkNeighbours[i].RowId, i});
				}
			}
			return neighbours;
		}

		// Calcula distancia Manhattan entre nodo actual y la meta
		private float ManhatanDistance(Node curNode, Node goalNode) {
			return (Mathf.Abs(goalNode.position[0] - curNode.position[0]) + Mathf.Abs(goalNode.position[1] - curNode.position[1]));
		}

		//Ingresa las direcciones en número entero al stack, comenzando por la meta
		private Stack<int> CalculatePath(Node node) {
			Stack<int> stack = new Stack<int>();
			while (node != null) {
				stack.Push(node.direction);
				node = node.parent;
			}
			
			return stack;
		}
	}
} 
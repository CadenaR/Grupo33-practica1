using System.Collections.Generic;
using System.Linq;
//using System.Runtime.Remoting.Messaging;
using Assets.Scripts.DataStructures;
using UnityEngine;
using Assets.Scripts.Grupo33.Solucion1;

namespace Assets.Scripts.Grupo33.Solucion2
{
    public class SearchEnemy
    {
        private Node[,] nodes { get; set; }
        private List<Node> openList;
		float enemyDist = 999;//Identificador de que todos los enemigos han sido eliminados
		GameObject enemy;

        //Método para movimiento del personaje
        public int Search(BoardInfo board, CellInfo currentPos, CellInfo[] goals)
        {	
            CreateNodesBoard(board, goals[0]);
            return FindNextMovement(board, nodes[currentPos.ColumnId, currentPos.RowId], nodes[goals[0].ColumnId, goals[0].RowId]);

        }

        //Crea el tablero representado en nodos
        private void CreateNodesBoard(BoardInfo board, CellInfo goal)
        {
            int cols = board.NumColumns;
            int rows = board.NumRows;
            nodes = new Node[cols, rows];
            for (int i = 0; i < cols; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    Node node = new Node(new int[] { i, j });
                    nodes[i, j] = node;
                }
            }
			nodes[goal.ColumnId, goal.RowId].isVisited = true;
        }

        /*	
		Metodo de búsqueda: Primero busca el enemigo mas cercano y lo asigna a una variable global.
		Teniendo un enemigo como objetivo, asigna el nodo de inicio a la lista abierta para luego comenzar a expandir 
		los nodos vecinos hasta llegar al nivel 3 (gCos), verificando que no fueran visitados anteriormente, posteriormente 
		asigna los valores de la fórmula f(n) = g(n) + h(n), asigna la dirección que fue tomada
		para llegar a ellos y como nodo padre el nodo que se esta expandiendo. Finalmente se agrega
		a la lista abierta cada nodo no visitado, se elimina el actual y se ordena la lista abierta, para regresar el primer 
		noto que se encuentre en el nivel 3, ya que al estar ordenada la lista, es el mas cercano para realizar el movimiento.
		*/
        private int FindNextMovement(BoardInfo board, Node start, Node goal)
        {
			// Obtiene el enemigo mas cercano para la iteración en curso
			foreach(GameObject enem in board.Enemies){
				if(enem != null){
					float manhattanD = ManhatanDistance(start, enem.transform.position.x, enem.transform.position.y);
					if(manhattanD < enemyDist){
						enemyDist = manhattanD;
						enemy = enem;
					}
				}
			}
			if(enemy == null){ // cuando todos los enemigos han sido eliminados  regresa 9999
				return 9999;
			}

            openList = new List<Node>();
            openList.Add(start);
            start.gCost = 0.0f;
            start.fCost = enemyDist;
            Node node = null;
            while (openList.Count != 0)
            {
                node = openList[0];
				// Si el nodo en curso tiene gCost 3, significa que esta en nivel 3 y regresa la función "CalculatePath" con este nodo
				if(node.gCost >= 3){
					return CalculatePath(node);
				}
				// si el nodo actual tiene la misma posición que el enemigo, regresa la función "CalculatePath" con este nodo
                if (node.position[0] == (int)enemy.transform.position.x && node.position[1] == (int)enemy.transform.position.y)
                {
                    return CalculatePath(node);
                }
                //Crea una lista para almacenar la posición y dirección tomada 
                List<int[]> neighbours = GetNeighbours(node, board);
                for (int i = 0; i < neighbours.Count; i++)
                {
                    int[] neighbourNode = neighbours[i];
                    int x = neighbourNode[0];
                    int y = neighbourNode[1];
                    if (!nodes[x, y].isVisited)
                    {
                        float neighbourHCost = ManhatanDistance(nodes[x, y], enemy.transform.position.x, enemy.transform.position.y);
                        nodes[x, y].gCost += node.gCost;
                        nodes[x, y].parent = node;
                        nodes[x, y].direction = neighbourNode[2];
                        nodes[x, y].fCost = nodes[x, y].gCost + neighbourHCost;
                        if (!openList.Contains(nodes[x, y]))
                        {
                            openList.Add(nodes[x, y]);
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
            return CalculatePath(node);
        }

        /*
		Regresa la posición y dirección en que se encuantran los vecinos a los que se puede acceder 
		utilizando el metodo de CellInfo WalkableNeighbours
		*/
        private List<int[]> GetNeighbours(Node node, BoardInfo board)
        {
            List<int[]> neighbours = new List<int[]>();
            CellInfo currentCell = board.CellInfos[node.position[0], node.position[1]];
            CellInfo[] walkNeighbours = currentCell.WalkableNeighbours(board);
            for (int i = 0; i < 4; i++)
            {
                if (walkNeighbours[i] != null)
                {
                    neighbours.Add(new int[] { walkNeighbours[i].ColumnId, walkNeighbours[i].RowId, i });
                }
            }
            return neighbours;
        }

        // Calcula distancia Manhattan entre nodo actual y el enemigo
        private float ManhatanDistance(Node curNode, float x, float y)
        {
            return (Mathf.Abs(x - curNode.position[0]) + Mathf.Abs(y - curNode.position[1]));
        }

        //Recorre todos los nodos padres desde el nodo recibido y regresa la dirección del primer hijo en entero
        private int CalculatePath(Node node)
        {
            int movement = -1;
            while (node.parent != null)
            {
				movement = node.direction;
                node = node.parent;
            }
			
			return movement;
        }
    }
}
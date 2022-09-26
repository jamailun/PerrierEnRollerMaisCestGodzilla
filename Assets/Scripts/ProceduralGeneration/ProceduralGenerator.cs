using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProceduralGenerator : MonoBehaviour {

	private readonly Dictionary<Color, Node> pixels = new();

	public ProceduralGenerator(Texture2D input) {
		// Itère dans tous les pixels, et ajoute les couleurs adjacentes.
		for(int x = 0; x < input.width; x++) {
			for(int y = 0; y < input.height; y++) {
				var col = input.GetPixel(x, y);
				// ajout si on connait pas.
				if( ! pixels.ContainsKey(col))
					pixels.Add(col, new Node(col));
				
				var node = pixels[col];

				// ajout au copains
				if(x > 0)
					node.Left.Add(input.GetPixel(x - 1, y));
				if(x < input.width - 1)
					node.Right.Add(input.GetPixel(x + 1, y));
				if(y > 0)
					node.Bot.Add(input.GetPixel(x, y - 1));
				if(y < input.height - 1)
					node.Top.Add(input.GetPixel(x, y + 1));
			}
		}
	}

	struct Point {
		public int x, y;
		public LinkNode from;
		public Point(int x, int y, LinkNode from) {
			this.x = x;
			this.y = y;
			this.from = from;
		}
	}

	public Texture2D GenerateTexture(int w, int h) {
		// Create a new 2x2 texture ARGB32 (32 bit with alpha) and no mipmaps
		var texture = new Texture2D(w, h, TextureFormat.ARGB32, false);

		// set the pixel values
		Color first = GetRandomFirst();
		int x = w / 2;
		int y = h / 2;
		texture.SetPixel(x, y, first);

		HashSet<Point> todo = new();

		Node node = pixels[first];
		if(x < w - 1)
			todo.Add(new Point(x + 1, y, node.Right));
		if(x > 0)
			todo.Add(new Point(x - 1, y, node.Left));
		if(y < h - 1)
			todo.Add(new Point(x, y + 1, node.Top));
		if(y > 0)
			todo.Add(new Point(x, y - 1, node.Bot));

		Point pt;
		while(todo.Any()) {
			pt = todo.GetEnumerator().Current;
			var col = pt.from.Pick();
			texture.SetPixel(pt.x, pt.y, col);
			//TODO pas fini xdd
		}



		// Apply all SetPixel calls
		texture.Apply();

		// connect texture to material of GameObject this script is attached to
		return texture;
	}

	public Color GetRandomFirst() {
		return pixels.GetEnumerator().Current.Key;
	}


	class LinkNode {
		// associe proba à un type
		private readonly Dictionary<Color, float> probas = new();

		public void Add(Color nodeId) {
			if(probas.ContainsKey(nodeId))
				probas[nodeId] += 1;
			else
				probas[nodeId] = 1;
		}

		public void Normalize() {
			float size = 0;
			foreach(float f in probas.Values)
				size += f;
			foreach(var k in probas.Keys)
				probas[k] /= size;
		}

		public Color Pick() {
			float v = (float) Random.value;
			foreach(var e in probas) {
				if(v <= e.Value)
					return e.Key;
				v += e.Value;
			}
			return Color.black;
		}

	}

	class Node {
		private Color id;
		public LinkNode Top, Right, Bot, Left;
		public Node(Color id) {
			this.id = id;
			Top = new();
			Right = new();
			Bot = new();
			Left = new();
		}
	}

}
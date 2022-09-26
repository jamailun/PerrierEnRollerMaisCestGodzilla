using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProceduralGenerator {

	private static System.Random RAND = new();

	private readonly Dictionary<Color, Node> pixels = new();

	public ProceduralGenerator(Texture2D input) {
		// Itère dans tous les pixels, et ajoute les couleurs adjacentes.
		for(int x = 0; x < input.width; x++) {
			for(int y = 0; y < input.height; y++) {
				var col = input.GetPixel(x, y);
				// ajout si on connait pas.
				if(!pixels.ContainsKey(col)) {
					pixels.Add(col, new Node());
					Debug.Log("Ok, added " + col + " to the pixels types.");
				}
				
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

		// Normalize probas
		foreach(var node in pixels.Values)
			node.Normalize();
	}

	struct Point {
		public int x, y;
		public LinkNode from;
		public Point(int x, int y, LinkNode from) {
			this.x = x;
			this.y = y;
			this.from = from;
		}
		public Pos AsPos() {
			return new Pos(x, y);
		}
	}
	struct Pos {
		public int x, y;
		public Pos(int x, int y) {
			this.x = x;
			this.y = y;
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
		Debug.Log("FIRST = " + first + ", to (" + x + "," + y + ")");

		HashSet<Point> todo = new();
		HashSet<Pos> done = new();

		GenerateTextute_AddSet(todo, done, first, new Pos(x, y), w, h);

		Point pt;
		while(todo.Any()) {
			pt = todo.ElementAt(RAND.Next(todo.Count));

			todo.Remove(pt);

			var pos = pt.AsPos();

			if(done.Contains(pos))
				continue;

			var col = pt.from.Pick();

			texture.SetPixel(pt.x, pt.y, col);
			GenerateTextute_AddSet(todo, done, col, pos, w, h);

			Debug.Log("colored " + pos + " with " + col);
		}

		// Apply all SetPixel calls
		texture.filterMode = FilterMode.Point;
		texture.Apply();

		// connect texture to material of GameObject this script is attached to
		return texture;
	}

	private void GenerateTextute_AddSet(HashSet<Point> set, HashSet<Pos> poss, Color color, Pos pos, int w, int h) {
		Node node = pixels[color];

		int x = pos.x;
		int y = pos.y;
		if(x < w - 1)
			set.Add(new Point(x + 1, y, node.Right));
		if(x > 0)
			set.Add(new Point(x - 1, y, node.Left));
		if(y < h - 1)
			set.Add(new Point(x, y + 1, node.Top));
		if(y > 0)
			set.Add(new Point(x, y - 1, node.Bot));
		poss.Add(pos);
	}

	public Color GetRandomFirst() {
		return pixels.ElementAt(RAND.Next(0, pixels.Count)).Key;
	}


	class LinkNode {
		// associe proba à un type
		private Dictionary<Color, float> probas = new();

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

			Dictionary<Color, float> normalized = new();
			foreach(var key in probas.Keys) {
				normalized[key] = probas[key] / size;
			}

			probas = normalized;
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
		public LinkNode Top, Right, Bot, Left;
		public Node() {
			Top = new();
			Right = new();
			Bot = new();
			Left = new();
		}
		public void Normalize() {
			Top.Normalize();
			Right.Normalize();
			Bot.Normalize();
			Left.Normalize();
		}
	}

}
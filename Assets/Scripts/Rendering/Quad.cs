/*
Copyright © 2011, Stephan Krohn
All rights reserved.
krohn.stephan@googlemail.com
*/

using UnityEngine;
using System.Collections;

public class Quad {

	/// <summary>
	/// points[0] is top left
	/// </summary>
	public Vector2[] points;

	public Quad(Vector2[] points)
	{
		this.points = points;
	}

	bool IsOnRightSide(Vector2[] line, Vector2 point) {
		return (point.y - line[0].y) * (line[1].x - line[0].x) - (point.x - line[0].x) * (line[1].y - line[0].y) <= 0;
	}

	public bool Contains(Vector2 point) {
		for (int i = 1; i <= points.Length; i++) {
			Vector2[] line = {points[i-1], points[i % points.Length]};
			if (!IsOnRightSide(line, point)) {
				return false;
			}
		}

		return true;
	}

}

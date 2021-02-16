﻿/// <summary>
/// Several ancilliary functions to sanitize polygons
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using WorldMapStrategyKit.Poly2Tri;

namespace WorldMapStrategyKit.PolygonClipping {
	public class PolygonSanitizer {

		static Line2D[] lines;

		/// <summary>
		/// Searches for segments that crosses themselves and removes the shorter until there're no one else
		/// </summary>
		public static bool RemoveCrossingSegments (ref Vector2[] points) {

			bool changes = false;
			while (points.Length > 5) {
				if (!DetectCrossingSegment (ref points)) {
					break;
				}
				changes = true;
			}
			return changes;
		}

		static bool DetectCrossingSegment (ref Vector2[] points) {
			int max = points.Length;
			if (lines == null || lines.Length < max) {
				lines = new Line2D[max];
				for (int k = 0; k < max - 1; k++) {
					lines [k] = new Line2D (points [k], points [k + 1], k, k + 1);
				}
				lines [max - 1] = new Line2D (points [max - 1], points [0], max - 1, 0);
			} else {
				for (int k = 0; k < max - 1; k++) {
					lines [k].Set (points [k], points [k + 1], k, k + 1);
				}
				lines [max - 1].Set (points [max - 1], points [0], max - 1, 0);
			}

			for (int k = 0; k < max; k++) {
				Line2D line1 = lines [k];
				for (int j = k + 2; j < max; j++) {
					Line2D line2 = lines [j];
					if (line2.intersectsLine (line1)) {
						if (line1.sqrMagnitude < line2.sqrMagnitude) {
							points = points.Purge (line1.P1Index, line1.P2Index);
							return true;
						} else {
							points = points.Purge (line2.P1Index, line2.P2Index);
							return true;
						}
					}
				}
			}
			return false;
		}

	}

}


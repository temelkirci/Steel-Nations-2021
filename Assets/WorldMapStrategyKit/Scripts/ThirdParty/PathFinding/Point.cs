using System;

namespace WorldMapStrategyKit.PathFinding {
				public struct Point {
								public int X;
								public int Y;

								public Point (int x, int y) {
												this.X = x;
												this.Y = y;
								}
//
//								public int X { get { return this._x; } set { this._x = value; } }
//
//								public int Y { get { return this._y; } set { this._y = value; } }
//
								// For debugging
								public override string ToString () {
												return string.Format ("{0}, {1}", this.X, this.Y);
								}
				}
}

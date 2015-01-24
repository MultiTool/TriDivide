using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

/*
to do:
even distribution on sphere 
hanging connections
each point (tri ctr) has a list of other connected tris. is there a direction? 

mesh up all tree fractal lines with real connected points. 

*/

namespace TriDivide {
	class TriDivide {
		// Recursive triangle division by 2, for hypercube
		double Radius = 1.0;
		// const int NumVDims = 8;// Virtual dimensions
		const int NumVDims = 5;
		// Virtual dimensions
		// const int NumVDims = 4; // Virtual dimensions
		int MaxDepth = NumVDims;
		int NumTris;
		Tri[] TriRay;
		int NumPnts;
		Pnt[] PntRay;
		int NumRows, NumCols;
		// String BasePath = @"C:\Users\BCZINRW\Pictures\Blend\";
		String BasePath = @".\";
		List<Line> LineRay = new List<Line> ();
		/* ********************************************************************************************************* */
		public TriDivide ()
		{
			NumTris = 1 << MaxDepth;
			TriRay = new Tri[NumTris];

			int HalfNumVDims = NumVDims >> 1;
			int SqrtOfNumTris = 1 << HalfNumVDims;// square root of NumTris

			int IsOdd = (NumVDims % 2);
			this.NumRows = ((SqrtOfNumTris) + 1);
			this.NumCols = (((SqrtOfNumTris) * (1 + IsOdd)) + 1);

			NumPnts = NumRows * NumCols;// inefficent, use triangle grid instead 
			PntRay = new Pnt[NumPnts];

			/*
triangle grid indexing 
isodd = (NumVDims mod 2)
num of rows will be (sqrt(2^NumVDims) + 1)
num of cols will be {[sqrt(2^NumVDims) * (1+isodd)] + 1}
             */
			Run ();
			DumpPntRay ();
			DumpTriVerts ();
			// DumpLines();
			// DumpTriCenters();
		}
		/* ********************************************************************************************************* */
		public void DumpPntRay()
		{
			Pnt pt;
			StringBuilder sb = new StringBuilder ();
			sb.Append ("o Points" + Environment.NewLine);
			for (int pcnt = 0; pcnt < this.NumPnts; pcnt++) {
				pt = PntRay [pcnt];
				if (pt != null) {
					String txtln = String.Format ("v {0:0.000000} {1:0.000000} {2:0.000000}", pt.Loc [0], pt.Loc [1], pt.Loc [2]);
					sb.Append (txtln + Environment.NewLine);
				}
			}
			File.WriteAllText (BasePath + @"pcloud5.obj", sb.ToString ());
		}
		/* ********************************************************************************************************* */
		public void DumpTriVerts()
		{
			Pnt pt;
			StringBuilder sb = new StringBuilder ();
			sb.Append ("o Cloud" + Environment.NewLine);
			for (int tcnt = 0; tcnt < NumTris; tcnt++) {
				Tri tri = TriRay [tcnt];
				for (int pcnt = 0; pcnt < 3; pcnt++) {
					pt = tri.Vtx [pcnt];
					String txtln = String.Format ("v {0:0.000000} {1:0.000000} {2:0.000000}", pt.Loc [0], pt.Loc [1], pt.Loc [2]);
					sb.Append (txtln + Environment.NewLine);
				}
			}
			File.WriteAllText (BasePath + @"pcloud4.obj", sb.ToString ());
		}
		/* ********************************************************************************************************* */
		public void DumpTris()
		{
			Pnt pt;
			StringBuilder sb = new StringBuilder ();
			sb.Append ("o Tris" + Environment.NewLine);
			#if false
            int pcnt = 0;
            for (int lcnt = 0; lcnt < LineRay.Count; lcnt++)
            {
                Line ln = LineRay[lcnt];

                pcnt++; ln.Pdex[0] = pcnt;
                pt0 = ln.Vtx[0];

                pcnt++; ln.Pdex[1] = pcnt;
                pt1 = ln.Vtx[1];

                String txtln0 = String.Format("v {0:0.000000} {1:0.000000} {2:0.000000}", pt0.Loc[0], pt0.Loc[1], pt0.Loc[2]);
                String txtln1 = String.Format("v {0:0.000000} {1:0.000000} {2:0.000000}", pt1.Loc[0], pt1.Loc[1], pt1.Loc[2]);

                sb.Append(txtln0 + Environment.NewLine);
                sb.Append(txtln1 + Environment.NewLine);
            }
            for (int lcnt = 0; lcnt < LineRay.Count; lcnt++)
            {
                Line ln = LineRay[lcnt];
                String txtln0 = String.Format("l {0} {1}", ln.Pdex[0], ln.Pdex[1]);
                sb.Append(txtln0 + Environment.NewLine);
            }
			
            for (int tcnt = 0; tcnt < NumTris; tcnt++)
            {
                Tri tri = TriRay[tcnt];
                for (int pcnt = 0; pcnt < 3; pcnt++)
                {
                    pt = tri.Vtx[pcnt];
                    String txtln = String.Format("v {0:0.000000} {1:0.000000} {2:0.000000}", pt.Loc[0], pt.Loc[1], pt.Loc[2]);
					String txtln = String.Format("f {0} {1} {2}", ln.Pdex[0], ln.Pdex[1]);
                    sb.Append(txtln + Environment.NewLine);
                }
            }
			#endif
			File.WriteAllText (BasePath + @"pcloud4.obj", sb.ToString ());
		}
		/* ********************************************************************************************************* */
		public void DumpTriCenters()
		{
			Pnt pt = new Pnt ();
			StringBuilder sb = new StringBuilder ();
			sb.Append ("o Cloud" + Environment.NewLine);
			for (int tcnt = 0; tcnt < NumTris; tcnt++) {
				Tri tri = TriRay [tcnt];
				tri.GetCenter (pt);
				String txtln = String.Format ("v {0:0.000000} {1:0.000000} {2:0.000000}", pt.Loc [0], pt.Loc [1], pt.Loc [2]);
				sb.Append (txtln + Environment.NewLine);
			}
			File.WriteAllText (BasePath + @"pcloud2.obj", sb.ToString ());
			/*
# Blender v2.71 (sub 0) OBJ File: ''
# www.blender.org
o Cube
v 1.000000 -1.000000 -1.000000
v 1.000000 -1.000000 1.000000
v -1.000000 -1.000000 1.000000
v -1.000000 -1.000000 -1.000000
v 1.000000 1.000000 -0.999999
v 0.999999 1.000000 1.000001
v -1.000000 1.000000 1.000000
v -1.000000 1.000000 -1.000000
s off
f 1 2 3 4
f 5 8 7 6
f 1 5 6 2
f 2 6 7 3
f 3 7 8 4
f 5 1 4 8

*/
		}
		/* ********************************************************************************************************* */
		public void DumpLines()
		{
			Pnt pt0, pt1;
			StringBuilder sb = new StringBuilder ();
			sb.Append ("o Frame" + Environment.NewLine);
			int pcnt = 0;
			for (int lcnt = 0; lcnt < LineRay.Count; lcnt++) {
				Line ln = LineRay [lcnt];

				pcnt++;
				ln.Pdex [0] = pcnt;
				pt0 = ln.Vtx [0];

				pcnt++;
				ln.Pdex [1] = pcnt;
				pt1 = ln.Vtx [1];

				String txtln0 = String.Format ("v {0:0.000000} {1:0.000000} {2:0.000000}", pt0.Loc [0], pt0.Loc [1], pt0.Loc [2]);
				String txtln1 = String.Format ("v {0:0.000000} {1:0.000000} {2:0.000000}", pt1.Loc [0], pt1.Loc [1], pt1.Loc [2]);

				sb.Append (txtln0 + Environment.NewLine);
				sb.Append (txtln1 + Environment.NewLine);
			}
			for (int lcnt = 0; lcnt < LineRay.Count; lcnt++) {
				Line ln = LineRay [lcnt];
				String txtln0 = String.Format ("l {0} {1}", ln.Pdex [0], ln.Pdex [1]);
				sb.Append (txtln0 + Environment.NewLine);
			}
			File.WriteAllText (BasePath + @"wires.obj", sb.ToString ());
			/*
                # Blender v2.71 (sub 0) OBJ File: ''
                # www.blender.org
                mtllib wireframe.mtl
                o Cube
                v 1.000000 -1.000000 -1.000000
                v 1.000000 -1.000000 1.000000
                v -1.000000 -1.000000 1.000000
                v -1.000000 -1.000000 -1.000000
                v 1.000000 1.000000 -0.999999
                v 0.999999 1.000000 1.000001
                v -1.000000 1.000000 1.000000
                v -1.000000 1.000000 -1.000000
                l 1 2
                l 1 4
                l 1 5
                l 2 3
                l 2 6
                l 3 4
                l 3 7
                l 4 8
                l 5 6
                l 5 8
                l 6 7
                l 7 8
            */
		}
		/* ********************************************************************************************************* */
		public void Run()
		{
			Tri octant = new Tri ();// Start with (0,0,1), (0,1,0), (1,0,0) for one octant of sphere 
			octant.Vtx [0].Assign (0, 0, 1);
			octant.Vtx [1].Assign (0, 1, 0);
			octant.Vtx [2].Assign (1, 0, 0);

			octant.Vtx [0].AssignDex (0, 0);// indexes on big triangle grid 
			octant.Vtx [1].AssignDex (0, this.NumRows - 1);
			octant.Vtx [2].AssignDex (this.NumCols - 1, this.NumRows - 1);

			InsertPnt (octant.Vtx [0]);
			InsertPnt (octant.Vtx [1]);
			InsertPnt (octant.Vtx [2]);

			Tri_Split (octant, 0, 0);
		}
		/* ********************************************************************************************************* */
		public void InsertPnt(Pnt pnt)
		{
			int PDex = MapToPntRay (pnt.Dex [0], pnt.Dex [1]);
			this.PntRay [PDex] = pnt;
		}
		/* ********************************************************************************************************* */
		public class Pnt {
			public double[] Loc = new double[3];
			public int[] Dex = new int[2];
			public Pnt CloneMe()
			{
				Pnt child = new Pnt ();
				for (int dcnt = 0; dcnt < 3; dcnt++) {
					child.Loc [dcnt] = this.Loc [dcnt];
				}
				return child;
			}
			public void Copy(Pnt other)
			{
				for (int dcnt = 0; dcnt < 3; dcnt++) {
					this.Loc [dcnt] = other.Loc [dcnt];
				}
				for (int dcnt = 0; dcnt < 2; dcnt++) {
					this.Dex [dcnt] = other.Dex [dcnt];
				}
			}
			public void Zero()
			{
				for (int dcnt = 0; dcnt < 3; dcnt++) {
					this.Loc [dcnt] = 0.0; 
				}
			}
			public void Add(Pnt other)
			{
				for (int dcnt = 0; dcnt < 3; dcnt++) {
					this.Loc [dcnt] += other.Loc [dcnt];
				}
			}
			public void Difference(Pnt other, Pnt Result)
			{
				for (int dcnt = 0; dcnt < 3; dcnt++) {
					Result.Loc [dcnt] = this.Loc [dcnt] - other.Loc [dcnt];
				}
			}
			public void Multiply(double factor)
			{
				for (int dcnt = 0; dcnt < 3; dcnt++) {
					this.Loc [dcnt] *= factor;
				}
			}
			public double GetMagnitude()
			{ // from 0,0,0
				double SumSq = 0;
				double Value;
				for (int dcnt = 0; dcnt < 3; dcnt++) {
					Value = this.Loc [dcnt];
					SumSq += Value * Value;
				}
				SumSq = Math.Sqrt (SumSq);
				return SumSq;
			}
			/* ********************************************************************************************************* */
			public void MapToSphere(double Radius)
			{
				double dist = this.GetMagnitude (); // from 0,0,0
				double factor = Radius / dist;
				for (int dcnt = 0; dcnt < 3; dcnt++) {
					this.Loc [dcnt] *= factor;
				}
			}
			/* ********************************************************************************************************* */
			public void Assign(double d0, double d1, double d2)
			{
				this.Loc [0] = d0;
				this.Loc [1] = d1;
				this.Loc [2] = d2;
			}
			/* ********************************************************************************************************* */
			public void AssignDex(int d0, int d1)
			{
				this.Dex [0] = d0;
				this.Dex [1] = d1;
			}
			/* ********************************************************************************************************* */
			public void Delete(){ }
		}
		/* ********************************************************************************************************* */
		public class Line {
			public Pnt[] Vtx = new Pnt[2];
			public int[] Pdex = new int[2];
			// indexes to vertex list for my endpoints
			/* ********************************************************************************************************* */
			public Line ()
			{
				for (int pcnt = 0; pcnt < 2; pcnt++) {
					this.Vtx [pcnt] = new Pnt ();
				}
			}
			/* ********************************************************************************************************* */
			public void Connect(Pnt p0, Pnt p1)
			{
				this.Vtx [0]=p0; this.Vtx [1]=p1;
			}
			/* ********************************************************************************************************* */
			public void Assign(Pnt p0, Pnt p1)
			{
				this.Vtx [0].Copy (p0);
				this.Vtx [1].Copy (p1);
			}
			/* ********************************************************************************************************* */
			public void Subtract(double Chop)
			{
				Pnt p0 = new Pnt (), p1 = new Pnt ();
				p0.Copy (this.Vtx [0]);
				p1.Copy (this.Vtx [1]);
				p0.Multiply (-1);
				p1.Add (p0);// move this line to 0,0,0
				double len = p1.GetMagnitude ();
				double NextLen = len - Chop;
				double Ratio = NextLen / len;
				p1.Multiply (Ratio);// mult this line by that ratio
				p1.Add (this.Vtx [0]);// move this line back to its place 
				this.Vtx [1].Copy (p1);
#if false
                shorten this line by Chop amount
                first, get magnitude of this line
                then subtract chop from that
                then get ratio of shorter/longer
                move this line to 0,0,0
                mult this line by that ratio
                move this line back to its place 
#endif
			}
		}
		/* ********************************************************************************************************* */
		public class Tri {
			public int BitAddress = 0;
			// assign adecuados shift bits
			public int RecurDepth = 0;
			// for whatever reason
			public Pnt[] Vtx = new Pnt[3];
			public int[] Pdex = new int[3];
			
			public Tri ()
			{
				for (int pcnt = 0; pcnt < 3; pcnt++) {
					this.Vtx [pcnt] = new Pnt ();
				}
			}
			/* ********************************************************************************************************* */
			public void Connect(Pnt p0, Pnt p1, Pnt p2)
			{
				this.Vtx [0] = p0;
				this.Vtx [1] = p1;
				this.Vtx [2] = p2;
			}
			/* ********************************************************************************************************* */
			public void Assign(Pnt p0, Pnt p1, Pnt p2)
			{
				this.Vtx [0].Copy (p0);
				this.Vtx [1].Copy (p1);
				this.Vtx [2].Copy (p2);
			}
			/* ********************************************************************************************************* */
			public void GetCenter(Pnt ptresult)
			{// Return the center of the triangle
				ptresult.Zero ();// average them all 
				for (int pcnt = 0; pcnt < 3; pcnt++) {
					ptresult.Add (this.Vtx [pcnt]);
				}
				ptresult.Multiply (1.0 / 3.0);
			}
			/* ********************************************************************************************************* */
			public void Delete()
			{
				for (int pcnt = 0; pcnt < 3; pcnt++) {
					this.Vtx [pcnt].Delete ();
				}
			}
		}
		/* ********************************************************************************************************* */
		public int MapToPntRay(int XDex, int YDex)
		{
			int Dex = (YDex * this.NumCols) + XDex;// simple inefficient rectangular grid 
			return Dex;
		}
		int[] Dex = new int[2];
		/* ********************************************************************************************************* */
		void Tri_Split(Tri tri, int SplitVtxDex, int RecurDepth)
		{
			tri.RecurDepth = RecurDepth;// maybe needed sometime? 
			if (RecurDepth >= MaxDepth) {
				TriRay [tri.BitAddress] = tri;
				return;
			}

			Pnt SplitVtx = tri.Vtx [SplitVtxDex];
			int LeftVertDex = (SplitVtxDex + 1);
			if (LeftVertDex >= 3) {
				LeftVertDex -= 3;
			}
			int RightVertDex = (SplitVtxDex - 1);
			if (RightVertDex < 0) {
				RightVertDex += 3;
			}

			Pnt RightVert = tri.Vtx [RightVertDex];// ClockVert
			Pnt LeftVert = tri.Vtx [LeftVertDex];// AntiClockVert

			Pnt ResultVtx;
			int PDex;

			if (true) {// Discover or create new point, bisecting opposite side
				SplitIndexes (LeftVert, RightVert, Dex);
				PDex = MapToPntRay (Dex [0], Dex [1]);
				ResultVtx = this.PntRay [PDex];
				if (ResultVtx == null) {
					ResultVtx = new Pnt ();// not found, create new point bisecting opposite side
					SplitLine (LeftVert, RightVert, ResultVtx);
					for (int dcnt = 0; dcnt < 2; dcnt++) {
						ResultVtx.Dex [dcnt] = Dex [dcnt];
					}
					// ResultVtx.MapToSphere(Radius);// move pt to sphere
					this.PntRay [PDex] = ResultVtx;
				}
			}

			if (false) {
				ResultVtx.MapToSphere (Radius);// move pt to sphere
			} else {
				Line ln = new Line ();
				ln.Assign(ResultVtx, tri.Vtx[SplitVtxDex]);
				ln.Subtract (0.05);
				this.LineRay.Add (ln);
			}

			// Create tri 0, 'left' of bisecting line 
			Tri tri0 = new Tri ();// apex + anticlock_angle + newpt;
			tri0.Connect (ResultVtx, SplitVtx, LeftVert);
			tri0.BitAddress = tri.BitAddress;
			tri0.BitAddress <<= 1;
			tri0.BitAddress |= 0x0;// nop, just a note to self

			// Create tri 1, 'right' of bisecting line 
			Tri tri1 = new Tri ();// apex + clock_angle + newpt;
			tri1.Connect (ResultVtx, RightVert, SplitVtx);
			tri1.BitAddress = tri.BitAddress;
			tri1.BitAddress <<= 1;
			tri1.BitAddress |= 0x1;

			Tri_Split (tri0, 0, RecurDepth + 1);// 0 is index of Result vertex for both sub-triangles 
			Tri_Split (tri1, 0, RecurDepth + 1);

			// tri0.Delete(); tri1.Delete();
		}
		/* ********************************************************************************************************* */
		void SplitLine(Pnt pt0, Pnt pt1, Pnt ptresult)
		{// Return the midpoint of a line 
			for (int dcnt = 0; dcnt < 3; dcnt++) {// from 0,0,0
				ptresult.Loc [dcnt] = (pt0.Loc [dcnt] + pt1.Loc [dcnt]) / 2.0;
			}
			for (int dcnt = 0; dcnt < 2; dcnt++) {// calculate new index in 2d point grid 
				ptresult.Dex [dcnt] = (pt0.Dex [dcnt] + pt1.Dex [dcnt]) >> 1;
			}
		}
		/* ********************************************************************************************************* */
		void SplitIndexes(Pnt pt0, Pnt pt1, int[] Dex)
		{
			for (int dcnt = 0; dcnt < 2; dcnt++) {// calculate new index in 2d point grid 
				Dex [dcnt] = (pt0.Dex [dcnt] + pt1.Dex [dcnt]) >> 1;
			}
		}
		/* ********************************************************************************************************* */
		void SuspendConnection(Pnt ctr, double Radius, Pnt pt0, Pnt pt1)
		{
			Pnt ChordVect = new Pnt ();
			pt1.Difference (pt0, ChordVect);// straight line from p0 to p1
			double ChordLen = ChordVect.GetMagnitude ();
			double HalfChordLen = (ChordLen / 2);

			// Radius is redundant, we could get it from magnitude of pt0 or pt1 
			// CtrToChord is ctr to ChordVect, magnitude 
			double CtrToChord = Math.Sqrt ((Radius * Radius) - (HalfChordLen * HalfChordLen)); // pythag 
			double TangentRatio = HalfChordLen / CtrToChord;// ratio of opposite/adjacent 
			double extra = TangentRatio * HalfChordLen;// extra distance from CtrToChord to orbit 
			double orbitmag = CtrToChord + extra;// full orbit distance from center 
			double orbitratio = orbitmag / CtrToChord;

			Pnt ChordMiddle = new Pnt ();// Get vect from ctr to middle of chord by averaging the endpoints. 
			ChordMiddle.Copy (pt0);
			ChordMiddle.Add (pt1);
			ChordMiddle.Multiply (0.5);

			Pnt orbitpoint = new Pnt ();// orbitpoint = ChordMiddle * orbitratio;// center of circle/sphere whose edges are perpendicular to surface 
			orbitpoint.Copy (ChordMiddle);
			orbitpoint.Multiply (orbitratio);// stretch the chord middle out to orbit 

			Pnt EdgeToMoon = new Pnt ();// dist from chord endpoint to orbitpoint is radius of moon 
			orbitpoint.Difference (pt0, EdgeToMoon);
			double MoonRadius = EdgeToMoon.GetMagnitude ();

			double endangle = Math.Asin (HalfChordLen / MoonRadius); // opposite over hypot 
			double startangle = -endangle;
			double step = (endangle - startangle) / 10.0;
			for (double angle = startangle; angle <= endangle; angle += step) {
				// XTravel = HalfChordLen + (sin(angle) * MoonRadius); // not right, starts in the wrong place 
				// or 
				// XTravelFactor = (sin(angle)) - sin(startangle); // wrong, also need to scale by length of real line 
				// slowly grow ChordVect along XTravelFactor, and use its 2d X value in circle formula to get arch 
			}
#if false
/*
hcube:
ChordVect = delta surface points;
ChordLen = len of ChordVect;
HalfChordLen = (ChordLen/2)
CtrToChord(ctr to ChordVect magnitude) = sqrt( (radius^2) - (HalfChordLen^2) ); // pythag 
TangentRatio = (ChordLen/2)/CtrToChord;// ratio of opposite/adjacent 
extra = TangentRatio * (HalfChordLen);// extra distance from CtrToChord to orbit 
orbitmag = CtrToChord + extra;// full orbit distance from center 
orbitratio = orbitmag / CtrToChord;
orbitpoint = ChordVect * orbitratio;// center of circle/sphere whose edges are perpendicular to surface 

next?  hang curves between points on sphere. could just walk along the straight line, get dist from orbit and project to radius of moon. 
better: in 2d, walk angle around the moon and get growing X value. use that as along-the-way ratio for above formula. 
endangle =  arcsin((HalfChordLen) / MoonRadius); // opposite over hypot 
startangle = -endangle;

for (double angle=startangle to endangle step 0.whatever){
  // XTravel = HalfChordLen + (sin(angle) * MoonRadius); // not right, starts in the wrong place 
  or 
  XTravelFactor = (sin(angle)) - sin(startangle); // wrong, also need to scale by length of real line 
  // slowly grow ChordVect along XTravelFactor, and use its 2d X value in circle formula to get arch 
}

#endif
		}
	}
}

#if false
/*
-------------------------------------
triangle grid indexing 
isodd = (VDims mod 2)
num of rows will be (sqrt(2^VDims) + 1)
num of cols will be {[sqrt(2^VDims) * (1+isodd)] + 1}

2 rows: subtract 1, divide by 2 
0 1 
2 3  - 3 (2)

3 rows: subtract 2, divide by 2 
0 1 2
3 4 5 
6 7 8  - 8 (6)

4 rows: subtract 3, divide by 2 
0 1 2 3 
4 5 6 7
8 9 0 1
2 3 4 5  - 15 (12)

5 rows: subtract 4, divide by 2 
0 1 2 3 4 
5 6 7 8 9 
0 1 2 3 4 
5 6 7 8 9 
0 1 2 3 4  - 24 (20)

6 rows: subtract 5, divide by 2 
0 1 2 3 4 5 
6 7 8 9 0 1 
2 3 4 5 6 7 
8 9 0 1 2 3 
4 5 6 7 8 9
0 1 2 3 4 5  - 35  (30) 

0  - ? (0)
1 2  - 2 (1)
3 4 5  - 5 (3)
6 7 8 9  - 9 (6)
0 1 2 3 4  - 14 (10) 
5 6 7 8 9 0  - 20 (15) 
1 2 3 4 5 6 7  - 27 (21) 

indexing is:

TriDex = ( (YDex * (YDex-1)) / 2) + XDex;

TriDex = ( ( ( (YDex+1) * (YDex)) / 2) - 0) + XDex; *** looks good 

( (0 * (0 - 1)) / 2) = 0 
( (1 * (1 - 1)) / 2) = 0 
( (2 * (2 - 1)) / 2) = 1 
( (3 * (3 - 1)) / 2) = 3 
( (4 * (4 - 1)) / 2) = 6 
( (5 * (5 - 1)) / 2) = 10 
( (6 * (6 - 1)) / 2) = 15 
( (7 * (7 - 1)) / 2) = 21 

0 + 1 + 2 + 3 + 4 + 5 + 6 

*/
#endif


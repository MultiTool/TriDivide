using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

/*
to do:
hanging connections as continuous lines 
export tree fractal lines with real connected points. 
mesh up all tree fractal lines for continuous surface. 
even distribution on sphere 

*/

namespace TriDivide
{
    class TriDivide
    {
        // Recursive triangle division by 2, for hypercube
        double Radius = 1.0;
        const int SpaceDims = 3;
        const int TotalDims = 6;
        // Virtual dimensions
        //const int NumVDims = 1;
        //const int NumVDims = 2;
        //const int NumVDims = 4;
        //const int NumVDims = 5;
        //const int NumVDims = 6;
        //const int NumVDims = 8;
        //const int NumVDims = 10;
        const int NumVDims = TotalDims - SpaceDims;

        int MaxDimDex = NumVDims - 1; // Virtual dimensions
        int NumTris;
        Tri[] TriRay;
        int NumPnts;
        Pnt[] PntRay;
        int NumTriRows, NumTriCols;
        String BasePath = @".\";
        List<Line> LineRay = new List<Line>();
        List<Line> TreeRay = new List<Line>();
        /* ********************************************************************************************************* */
        public TriDivide()
        {
            // NumTris = 1 << NumVDims;// 2 to the power of number of dimensions 
            NumTris = 1 << TotalDims;// 2 to the power of number of dimensions 
            TriRay = new Tri[NumTris];

            {// vertex collision table?
                int HalfNumVDims = NumVDims >> 1;
                int SqrtOfNumTris = 1 << HalfNumVDims;// square root of NumTris

                /* triangle grid indexing */
                int IsOdd = (NumVDims % 2);
                this.NumTriRows = ((SqrtOfNumTris) + 1);
                this.NumTriCols = (((SqrtOfNumTris) * (1 + IsOdd)) + 1);
            }
            NumPnts = NumTriRows * NumTriCols;// inefficent, use triangle grid instead 
            PntRay = new Pnt[NumPnts];

            Shape shape = new Shape();
            Run(shape);
            DumpLines(TreeRay, @"tree.obj");
            DumpLines(shape.LineRay, @"connections.obj");
            DumpPntRay();
            DumpTriVerts();
            // DumpLines();

            for (int dcnt = 0; dcnt < NumVDims; dcnt++)
            {
                DumpTriCenters(String.Format(@"trictrs{0:D2}.obj", dcnt), dcnt);
            }
        }
        /* ********************************************************************************************************* */
        public void DumpPntRay()
        {
            Pnt pt;
            StringBuilder sb = new StringBuilder();
            sb.Append("o Points" + Environment.NewLine);
            for (int pcnt = 0; pcnt < this.NumPnts; pcnt++)
            {
                pt = PntRay[pcnt];
                if (pt != null)
                {
                    String txtln = String.Format("v {0:0.000000} {1:0.000000} {2:0.000000}", pt.Loc[0], pt.Loc[1], pt.Loc[2]);
                    sb.Append(txtln + Environment.NewLine);
                }
            }
            File.WriteAllText(BasePath + @"pcloud5.obj", sb.ToString());
        }
        /* ********************************************************************************************************* */
        public void DumpTriVerts()
        {
            Pnt pt;
            StringBuilder sb = new StringBuilder();
            sb.Append("o Cloud" + Environment.NewLine);
            for (int tcnt = 0; tcnt < NumTris; tcnt++)
            {
                Tri tri = TriRay[tcnt];
                for (int pcnt = 0; pcnt < 3; pcnt++)
                {
                    pt = tri.Vtx[pcnt];
                    String txtln = String.Format("v {0:0.000000} {1:0.000000} {2:0.000000}", pt.Loc[0], pt.Loc[1], pt.Loc[2]);
                    sb.Append(txtln + Environment.NewLine);
                }
            }
            File.WriteAllText(BasePath + @"pcloud4.obj", sb.ToString());
        }
        /* ********************************************************************************************************* */
        public void DumpTris()
        {
            Pnt pt;
            StringBuilder sb = new StringBuilder();
            sb.Append("o Tris" + Environment.NewLine);
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
            File.WriteAllText(BasePath + @"pcloud4.obj", sb.ToString());
        }
        /* ********************************************************************************************************* */
        public void DumpTriCenters(String FileName, int bitnum)
        {
            Pnt pt = new Pnt();
            StringBuilder sb = new StringBuilder();
            sb.Append("o Cloud" + Environment.NewLine);
            for (int tcnt = 0; tcnt < NumTris; tcnt++)
            {
                Tri tri = TriRay[tcnt];

                if (((tri.BitAddress >> bitnum) & 0x1) > 0)
                // if ((tri.BitAddress & 0x1) > 0)
                // if ((tcnt & 0x1) > 0)
                {
                    tri.GetCenter(pt);
                    String txtln = String.Format("v {0:0.000000} {1:0.000000} {2:0.000000}", pt.Loc[0], pt.Loc[1], pt.Loc[2]);
                    sb.Append(txtln + Environment.NewLine);
                }
            }
            File.WriteAllText(BasePath + FileName, sb.ToString());
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
        public void DumpLines(List<Line> LineList, String FileName)
        {
            Pnt pt0, pt1;
            StringBuilder sb = new StringBuilder();
            sb.Append("o Frame" + Environment.NewLine);
            int pcnt = 0;
            for (int lcnt = 0; lcnt < LineList.Count; lcnt++)
            {
                Line ln = LineList[lcnt];

                pcnt++;
                ln.Pdex[0] = pcnt;
                pt0 = ln.Vtx[0];

                pcnt++;
                ln.Pdex[1] = pcnt;
                pt1 = ln.Vtx[1];

                String txtln0 = String.Format("v {0:0.000000} {1:0.000000} {2:0.000000}", pt0.Loc[0], pt0.Loc[1], pt0.Loc[2]);
                String txtln1 = String.Format("v {0:0.000000} {1:0.000000} {2:0.000000}", pt1.Loc[0], pt1.Loc[1], pt1.Loc[2]);

                sb.Append(txtln0 + Environment.NewLine);
                sb.Append(txtln1 + Environment.NewLine);
            }
            for (int lcnt = 0; lcnt < LineList.Count; lcnt++)
            {
                Line ln = LineList[lcnt];
                String txtln0 = String.Format("l {0} {1}", ln.Pdex[0], ln.Pdex[1]);
                sb.Append(txtln0 + Environment.NewLine);
            }
            File.WriteAllText(BasePath + FileName, sb.ToString());
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
        public void Run(Shape shape)
        {
            Tri octant = new Tri();// Start with (0,0,1), (0,1,0), (1,0,0) for one octant of sphere 

            octant.BitAddress = 0;
            octant.Vtx[0].Assign(0, 0, 1);
            octant.Vtx[1].Assign(0, 1, 0);
            octant.Vtx[2].Assign(1, 0, 0);

            octant.Vtx[0].AssignDex(0, 0);// indexes on big triangle grid 
            octant.Vtx[1].AssignDex(0, this.NumTriRows - 1);
            octant.Vtx[2].AssignDex(this.NumTriCols - 1, this.NumTriRows - 1);

            InsertPnt(octant.Vtx[0]);
            InsertPnt(octant.Vtx[1]);
            InsertPnt(octant.Vtx[2]);

            Tri_Split(octant, 0, 0, false);

            ConnectTris(shape);
        }
        /* ********************************************************************************************************* */
        public void InsertPnt(Pnt pnt)
        {
            int PDex = MapToPntRay(pnt.Dex[0], pnt.Dex[1]);
            this.PntRay[PDex] = pnt;
        }
        /* ********************************************************************************************************* */
        public class Pnt
        {
            public double[] Loc = new double[3];
            public int[] Dex = new int[2];
            public int MyNumber;
            /* ********************************************************************************************************* */
            public Pnt CloneMe()
            {
                Pnt child = new Pnt();
                for (int dcnt = 0; dcnt < 3; dcnt++) { child.Loc[dcnt] = this.Loc[dcnt]; }
                return child;
            }
            /* ********************************************************************************************************* */
            public void CopyFrom(Pnt other)
            {
                for (int dcnt = 0; dcnt < 3; dcnt++) { this.Loc[dcnt] = other.Loc[dcnt]; }
                for (int dcnt = 0; dcnt < 2; dcnt++) { this.Dex[dcnt] = other.Dex[dcnt]; }
            }
            /* ********************************************************************************************************* */
            public void Zero()
            {
                for (int dcnt = 0; dcnt < 3; dcnt++) { this.Loc[dcnt] = 0.0; }
            }
            /* ********************************************************************************************************* */
            public void Add(Pnt other)
            {
                for (int dcnt = 0; dcnt < 3; dcnt++) { this.Loc[dcnt] += other.Loc[dcnt]; }
            }
            /* ********************************************************************************************************* */
            public void Difference(Pnt other, Pnt Result)
            {
                for (int dcnt = 0; dcnt < 3; dcnt++) { Result.Loc[dcnt] = this.Loc[dcnt] - other.Loc[dcnt]; }
            }
            /* ********************************************************************************************************* */
            public void Multiply(double factor, Pnt Result)
            {// non destructive
                for (int dcnt = 0; dcnt < 3; dcnt++) { Result.Loc[dcnt] = this.Loc[dcnt] * factor; }
            }
            /* ********************************************************************************************************* */
            public void Multiply(double factor)
            {
                for (int dcnt = 0; dcnt < 3; dcnt++) { this.Loc[dcnt] *= factor; }
            }
            /* ********************************************************************************************************* */
            public double GetMagnitude()
            { // from 0,0,0
                double SumSq = 0;
                double Value;
                for (int dcnt = 0; dcnt < 3; dcnt++)
                {
                    Value = this.Loc[dcnt];
                    SumSq += Value * Value;
                }
                SumSq = Math.Sqrt(SumSq);
                return SumSq;
            }
            /* ********************************************************************************************************* */
            public void Normalize()
            {
                double Magnitude = this.GetMagnitude();
                for (int dcnt = 0; dcnt < 3; dcnt++) { this.Loc[dcnt] /= Magnitude; }
            }
            /* ********************************************************************************************************* */
            public void MapToSphere(double Radius)
            {
                double dist = this.GetMagnitude(); // from 0,0,0
                double factor = Radius / dist;
                for (int dcnt = 0; dcnt < 3; dcnt++)
                {
                    this.Loc[dcnt] *= factor;
                }
            }
            /* ********************************************************************************************************* */
            public void Assign(double d0, double d1, double d2)
            {
                this.Loc[0] = d0;
                this.Loc[1] = d1;
                this.Loc[2] = d2;
            }
            /* ********************************************************************************************************* */
            public void AssignDex(int d0, int d1)
            {
                this.Dex[0] = d0;
                this.Dex[1] = d1;
            }
            /* ********************************************************************************************************* */
            public void Delete() { }
        }
        /* ********************************************************************************************************* */
        public class Line
        {
            public Pnt[] Vtx = new Pnt[2];
            public int[] Pdex = new int[2];// indexes to vertex list for my endpoints
            /* ********************************************************************************************************* */
            public Line()
            {
                for (int pcnt = 0; pcnt < 2; pcnt++)
                {
                    this.Vtx[pcnt] = new Pnt();
                }
            }
            /* ********************************************************************************************************* */
            public void Connect(Pnt p0, Pnt p1)
            {
                this.Vtx[0] = p0;
                this.Vtx[1] = p1;
            }
            /* ********************************************************************************************************* */
            public void Assign(Pnt p0, Pnt p1)
            {
                this.Vtx[0].CopyFrom(p0);
                this.Vtx[1].CopyFrom(p1);
            }
            /* ********************************************************************************************************* */
            public void Subtract(double Chop)
            {
                Pnt p0 = new Pnt(), p1 = new Pnt();
                p0.CopyFrom(this.Vtx[0]);
                p1.CopyFrom(this.Vtx[1]);
                p0.Multiply(-1);
                p1.Add(p0);// move this line to 0,0,0
                double len = p1.GetMagnitude();
                double NextLen = len - Chop;
                double Ratio = NextLen / len;
                p1.Multiply(Ratio);// mult this line by that ratio
                p1.Add(this.Vtx[0]);// move this line back to its place 
                this.Vtx[1].CopyFrom(p1);
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
        public class Sector
        {
            public int BitAddress = 0; // assign adecuados shift bits
            public int RecurDepth = 0;// for whatever reason
        }
        /* ********************************************************************************************************* */
        public class ApexSector : Sector
        {
            public Pnt Apex = new Pnt();
            /* ********************************************************************************************************* */
            public void Assign(double d0, double d1, double d2) { this.Apex.Assign(d0, d1, d2); }
            /* ********************************************************************************************************* */
            public void AssignDex(int d0, int d1) { this.Apex.AssignDex(d0, d1); }
            /* ********************************************************************************************************* */
            public ApexSector CloneMe()
            {
                ApexSector child = new ApexSector();
                child.Apex.CopyFrom(this.Apex);
                return child;
            }
        }
        /* ********************************************************************************************************* */
        public class Tri : Sector
        {
            public Pnt[] Vtx = new Pnt[3];
            public int[] Pdex = new int[3];

            public Tri()
            {
                for (int pcnt = 0; pcnt < 3; pcnt++)
                {
                    this.Vtx[pcnt] = new Pnt();
                }
            }
            /* ********************************************************************************************************* */
            public void Connect(Pnt p0, Pnt p1, Pnt p2)
            {
                this.Vtx[0] = p0;
                this.Vtx[1] = p1;
                this.Vtx[2] = p2;
            }
            /* ********************************************************************************************************* */
            public void Assign(Pnt p0, Pnt p1, Pnt p2)
            {
                this.Vtx[0].CopyFrom(p0);
                this.Vtx[1].CopyFrom(p1);
                this.Vtx[2].CopyFrom(p2);
            }
            /* ********************************************************************************************************* */
            public void GetCenter(Pnt ptresult)
            {// Return the center of the triangle
                ptresult.Zero();// average them all 
                for (int pcnt = 0; pcnt < 3; pcnt++)
                {
                    ptresult.Add(this.Vtx[pcnt]);
                }
                ptresult.Multiply(1.0 / 3.0);
            }
            /* ********************************************************************************************************* */
            public void Delete()
            {
                for (int pcnt = 0; pcnt < 3; pcnt++)
                {
                    this.Vtx[pcnt].Delete();
                }
            }
        }
        /* ********************************************************************************************************* */
        public class Shape
        {
            private List<Pnt> PntRay;
            public List<Line> LineRay;
            public List<Tri> TriRay;
            /* ********************************************************************************************************* */
            public Shape()
            {
                PntRay = new List<Pnt>(); LineRay = new List<Line>(); TriRay = new List<Tri>();
            }
            /* ********************************************************************************************************* */
            public void AddPnt(Pnt pnt)
            {
                pnt.MyNumber = this.PntRay.Count;
                this.PntRay.Add(pnt);
            }
        }
        /* ********************************************************************************************************* */
        void Sphere_Split()
        {// split a sphere into hemispheres 
            ApexSector ApexXNeg = new ApexSector(); ApexXNeg.Assign(-1.0, 0.0, 0.0);
            ApexSector ApexXPos = new ApexSector(); ApexXPos.Assign(+1.0, 0.0, 0.0);

            int RecurDepth = 0;
            int FlipBit = 0; // FlipBit = Flipped ? 1 : 0;

            int LeftBit = FlipBit;
            int RightBit = (~FlipBit) & 1;// opposite of left

            ApexXNeg.BitAddress = 0;
            ApexXNeg.BitAddress |= (LeftBit << (MaxDimDex - RecurDepth));

            ApexXPos.BitAddress = 0;
            ApexXPos.BitAddress |= (RightBit << (MaxDimDex - RecurDepth));

            Hemisphere_Split(ApexXNeg, RecurDepth, false);
            Hemisphere_Split(ApexXPos, RecurDepth, true);
        }
        /* ********************************************************************************************************* */
        void Hemisphere_Split(ApexSector Apex, int RecurDepth, bool Flipped)
        {// split a hemisphere into quadrants
            int FlipBit = Flipped ? 1 : 0;
            int LeftBit = FlipBit;
            int RightBit = (~FlipBit) & 1;// opposite of left

            ApexSector ApexYNeg = Apex.CloneMe(); ApexYNeg.Apex.Loc[1] = -1.0;
            ApexSector ApexYPos = Apex.CloneMe(); ApexYPos.Apex.Loc[1] = +1.0;

            ApexYNeg.BitAddress = Apex.BitAddress | (LeftBit << (MaxDimDex - RecurDepth));
            ApexYPos.BitAddress = Apex.BitAddress | (RightBit << (MaxDimDex - RecurDepth));

            Quadrant_Split(ApexYNeg, RecurDepth + 1, false);
            Quadrant_Split(ApexYPos, RecurDepth + 1, true);
        }
        /* ********************************************************************************************************* */
        void Quadrant_Split(ApexSector Apex, int RecurDepth, bool Flipped)
        {// split a quarter sphere into octants (triangles) 
            Tri OctantZPos, OctantZNeg;
            int FlipBit = Flipped ? 1 : 0;
            int LeftBit = FlipBit;
            int RightBit = (~FlipBit) & 1;// opposite of left

            // need a master plan for this 
            {
                OctantZNeg = new Tri();// Start with (0,0,1), (0,1,0), (1,0,0) for one octant of sphere 
                OctantZNeg.BitAddress = 0;
                OctantZNeg.Vtx[0].Assign(0, 0, -1.0);// Z Negative
                OctantZNeg.Vtx[1].Assign(0, Apex.Apex.Loc[1], 0);
                OctantZNeg.Vtx[2].Assign(Apex.Apex.Loc[0], 0, 0);

                OctantZNeg.Vtx[0].AssignDex(0, 0);// indexes on big triangle grid 
                OctantZNeg.Vtx[1].AssignDex(0, this.NumTriRows - 1);
                OctantZNeg.Vtx[2].AssignDex(this.NumTriCols - 1, this.NumTriRows - 1);

                OctantZNeg.BitAddress = Apex.BitAddress | (LeftBit << (MaxDimDex - RecurDepth));

                InsertPnt(OctantZNeg.Vtx[0]); InsertPnt(OctantZNeg.Vtx[1]); InsertPnt(OctantZNeg.Vtx[2]);
                Tri_Split(OctantZNeg, 0, RecurDepth + 1, false);
                ClearPntRay();
            }
            {
                OctantZPos = new Tri();// Start with (0,0,1), (0,1,0), (1,0,0) for one octant of sphere 
                OctantZPos.BitAddress = 0;
                OctantZPos.Vtx[0].Assign(0, 0, +1.0);// Z Positive
                OctantZPos.Vtx[1].Assign(0, Apex.Apex.Loc[1], 0);
                OctantZPos.Vtx[2].Assign(Apex.Apex.Loc[0], 0, 0);

                OctantZPos.Vtx[0].AssignDex(0, 0);// indexes on big triangle grid 
                OctantZPos.Vtx[1].AssignDex(0, this.NumTriRows - 1);
                OctantZPos.Vtx[2].AssignDex(this.NumTriCols - 1, this.NumTriRows - 1);

                OctantZPos.BitAddress = Apex.BitAddress | (RightBit << (MaxDimDex - RecurDepth));

                InsertPnt(OctantZPos.Vtx[0]); InsertPnt(OctantZPos.Vtx[1]); InsertPnt(OctantZPos.Vtx[2]);
                Tri_Split(OctantZPos, 0, RecurDepth + 1, true);
                ClearPntRay();
            }
        }
        /* ********************************************************************************************************* */
        public void ClearPntRay()
        {// ONLY FOR TRIANGLE COLLISION ARRAY
            for (int pcnt = 0; pcnt < NumPnts; pcnt++) { PntRay[pcnt] = null; }
            // PntRay = new Pnt[NumPnts]; // or this 
        }
        /* ********************************************************************************************************* */
        public int MapToPntRay(int XDex, int YDex)
        {
            int Dex = (YDex * this.NumTriCols) + XDex;// simple inefficient rectangular grid 
            return Dex;
        }
        int[] Dex = new int[2];
        /* ********************************************************************************************************* */
        void Tri_Split(Tri tri, int SplitVtxDex, int RecurDepth, bool Flipped)
        {
            tri.RecurDepth = RecurDepth;// maybe needed sometime? 

            if (RecurDepth >= NumVDims)
            {
                TriRay[tri.BitAddress] = tri;
                return;
            }

            int ShiftVal = (MaxDimDex - RecurDepth);

            // Every triangle's children must be arranged symmetrically with those of its twin triangle, so always flip el triangulo derecho. 
            int FlipBit;
            FlipBit = Flipped ? 1 : 0;

            int LeftBit = FlipBit;
            int RightBit = (~FlipBit) & 1;// opposite of left

            Pnt SplitVtx = tri.Vtx[SplitVtxDex];
            int LeftVertDex = (SplitVtxDex + 1);
            if (LeftVertDex >= 3) { LeftVertDex -= 3; }
            int RightVertDex = (SplitVtxDex - 1);
            if (RightVertDex < 0) { RightVertDex += 3; }

            Pnt RightVert = tri.Vtx[RightVertDex];// ClockVert
            Pnt LeftVert = tri.Vtx[LeftVertDex];// AntiClockVert

            Pnt ResultVtx;
            int PDex;

            {// Discover or create new point, bisecting opposite side
                SplitIndexes(LeftVert, RightVert, Dex);
                PDex = MapToPntRay(Dex[0], Dex[1]);
                ResultVtx = this.PntRay[PDex];
                if (ResultVtx == null)
                {
                    ResultVtx = new Pnt();// not found, create new point bisecting opposite side
                    SplitLine(LeftVert, RightVert, ResultVtx);
                    for (int dcnt = 0; dcnt < 2; dcnt++)
                    {
                        ResultVtx.Dex[dcnt] = Dex[dcnt];
                    }
                    ResultVtx.MapToSphere(Radius);// move pt to sphere
                    this.PntRay[PDex] = ResultVtx;
                }
            }

            if (true)
            {
                Line ln = new Line();
                ln.Assign(ResultVtx, tri.Vtx[SplitVtxDex]);
                ln.Subtract(0.05);
                this.TreeRay.Add(ln);
            }

            // Create child triangle left of bisecting line 
            Tri TriLeft = new Tri();// apex + anticlock_angle + newpt;
            TriLeft.Connect(ResultVtx, SplitVtx, LeftVert);
            //TriLeft.BitAddress = tri.BitAddress; TriLeft.BitAddress <<= 1; TriLeft.BitAddress |= LeftBit;
            TriLeft.BitAddress = tri.BitAddress | (LeftBit << (MaxDimDex - RecurDepth));

            // Create child triangle right of bisecting line 
            Tri TriRight = new Tri();// apex + clock_angle + newpt;
            TriRight.Connect(ResultVtx, RightVert, SplitVtx);
            //TriRight.BitAddress = tri.BitAddress; TriRight.BitAddress <<= 1; TriRight.BitAddress |= RightBit;
            TriRight.BitAddress = tri.BitAddress | (RightBit << (MaxDimDex - RecurDepth));

            if (Flipped)
            {// If we are in a right hand (flipped) triangle, then recurse children right to left. This creates all leaf triangles in adjacent order. 
                Tri_Split(TriRight, 0, RecurDepth + 1, true);
                Tri_Split(TriLeft, 0, RecurDepth + 1, false);// 0 is index of Result vertex for both sub-triangles 
            }
            else
            {
                Tri_Split(TriLeft, 0, RecurDepth + 1, false);// 0 is index of Result vertex for both sub-triangles 
                Tri_Split(TriRight, 0, RecurDepth + 1, true);
            }

            // tri0.Delete(); tri1.Delete();
        }
        /* ********************************************************************************************************* */
        void SplitLine(Pnt pt0, Pnt pt1, Pnt ptresult)
        {// Return the midpoint of a line 
            for (int dcnt = 0; dcnt < 3; dcnt++)
            {// from 0,0,0
                ptresult.Loc[dcnt] = (pt0.Loc[dcnt] + pt1.Loc[dcnt]) / 2.0;
            }
            for (int dcnt = 0; dcnt < 2; dcnt++)
            {// calculate new index in 2d point grid 
                ptresult.Dex[dcnt] = (pt0.Dex[dcnt] + pt1.Dex[dcnt]) >> 1;
            }
        }
        /* ********************************************************************************************************* */
        void SplitIndexes(Pnt pt0, Pnt pt1, int[] Dex)
        {
            for (int dcnt = 0; dcnt < 2; dcnt++)
            {// calculate new index in 2d point grid 
                Dex[dcnt] = (pt0.Dex[dcnt] + pt1.Dex[dcnt]) >> 1;
            }
        }
        /* ********************************************************************************************************* */
        void ConnectTris(Shape shape)
        {
            Pnt tctr = new Pnt();
            Pnt nbrctr = new Pnt();
            for (int tcnt = 0; tcnt < this.NumTris; tcnt++)
            {
                Tri tri = this.TriRay[tcnt];
                tri.GetCenter(tctr);
                tctr.Normalize();
                // System.Console.WriteLine("tcnt:{0}", Convert.ToString(tcnt, 2).PadLeft(8, '0'));
                for (int dcnt = 0; dcnt < NumVDims; dcnt++)
                {
                    int nbrdex = tcnt ^ (0x1 << dcnt);
                    // System.Console.WriteLine("nbrdex:{0}", Convert.ToString(nbrdex, 2).PadLeft(8, '0'));
                    Tri nbr = this.TriRay[nbrdex];
                    nbr.GetCenter(nbrctr);
                    nbrctr.Normalize();
                    CreateArchedConnection(Radius, tctr, nbrctr, shape);
                }
            }
        }
        /* ********************************************************************************************************* */
        void CreateArchedConnection(double Radius, Pnt pt0, Pnt pt1, Shape shape)
        {
            // pt0 and pt1 must be on the surface of the sphere defined by Radius
            // Radius is redundant, we could get it from magnitude of pt0 or pt1 
            int NumSteps = 16;

            Line ln;

            Pnt ChordVect = new Pnt();
            pt1.Difference(pt0, ChordVect);// straight line from p0 to p1
            double ChordLen = ChordVect.GetMagnitude();
            double HalfChordLen = (ChordLen / 2);

            // CtrToChord is magnitude of ctr to midway of ChordVect 
            double RadSq = Radius * Radius;
            double CtrToChord = Math.Sqrt((RadSq) - (HalfChordLen * HalfChordLen)); // pythag 
            double TangentRatio = HalfChordLen / CtrToChord;// ratio of opposite/adjacent 
            double extra = TangentRatio * HalfChordLen;// extra distance from CtrToChord to orbit 
            double orbitmag = CtrToChord + extra;// full orbit distance from center 
            double orbitratio = orbitmag / CtrToChord;

            Pnt ChordMiddle = new Pnt();// Get vect from ctr to middle of chord by averaging the endpoints. 
            ChordMiddle.CopyFrom(pt0); ChordMiddle.Add(pt1);
            ChordMiddle.Multiply(0.5);// location halfway along the chord 

            Pnt orbitpoint = new Pnt();// orbitpoint = ChordMiddle * orbitratio;// center of circle/sphere whose edges are perpendicular to surface 
            orbitpoint.CopyFrom(ChordMiddle);
            orbitpoint.Multiply(orbitratio);// stretch the chord middle out to orbit 

            Pnt EdgeToMoon = new Pnt();// dist from chord endpoint to orbitpoint is radius of moon 
            orbitpoint.Difference(pt0, EdgeToMoon);
            double MoonRadius = EdgeToMoon.GetMagnitude();
            double MoonRadSq = MoonRadius * MoonRadius;
            double EndAngle = Math.Asin(HalfChordLen / MoonRadius); // opposite over hypot 
            if (HalfChordLen > MoonRadius)
            {// this would be an error
                System.Console.WriteLine();
            }
            double StartAngle = -EndAngle;
            double StartSine = Math.Sin(StartAngle);
            double StartCosine = Math.Cos(StartAngle);
            double StartHeight = StartCosine * MoonRadius;
            double EndSine = Math.Sin(EndAngle);
            double SineRange = EndSine - StartSine;
            double XTravel = 0, XTravelFactor, XTravelFactorPositive, XTravelFactorNormed;
            Pnt Walker = new Pnt();// walker walks from pt0 to pt1 
            Pnt OffsetNormed = new Pnt();
            Pnt OffsetScaled = new Pnt();
            OffsetNormed.CopyFrom(ChordMiddle); OffsetNormed.Multiply(-1.0); OffsetNormed.Normalize(); // direction from chord toward center 
            Pnt PrevPnt = new Pnt();
            PrevPnt.CopyFrom(pt0);
            shape.AddPnt(PrevPnt.CloneMe());
            for (int LCnt = 0; LCnt <= NumSteps; LCnt++)
            {
                double Percent = LCnt / ((double)NumSteps);
                double Angle = StartAngle + ((EndAngle - StartAngle) * Percent);

                XTravelFactor = (Math.Sin(Angle)); // negative to postive range 
                XTravel = (XTravelFactor * MoonRadius); // negative to postive range 
                XTravelFactorPositive = XTravelFactor - StartSine; // 0 to postive range 
                XTravelFactorNormed = XTravelFactorPositive / SineRange;// fraction along the way, 0.0 to 1.0  

                // slowly grow ChordVect along XTravelFactorNormed, and use its 2d X value in circle formula to get arch 
                ChordVect.Multiply(XTravelFactorNormed, Walker);
                Walker.Add(pt0);// walker walks from pt0 to pt1 
                // but first walker must be offset by height of Y arch 
                double YHeight = Math.Sqrt(MoonRadSq - (XTravel * XTravel));// circle formula
                YHeight -= StartHeight;
                OffsetNormed.Multiply(YHeight, OffsetScaled);
                Walker.Add(OffsetScaled);

                ln = new Line();
                ln.Assign(PrevPnt, Walker);
                shape.LineRay.Add(ln);
                shape.AddPnt(Walker.CloneMe());
                PrevPnt.CopyFrom(Walker);
            }
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

If even number of dimensions
TriDex = ( ((YDex+1) * YDex) / 2) + XDex; *** looks good 

If odd number of dimensions
TriDex = (YDex * YDex) + XDex;

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


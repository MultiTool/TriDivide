
import java.io.File;
import java.util.ArrayList;
import java.util.List;


/*
 to do:
 even distribution on sphere 
 hanging connections
 each point (tri ctr) has a list of other connected tris. is there a direction? 

 mesh up all tree fractal lines with real connected points. 

 */
class TriDivide {// Recursive triangle division by 2, for hypercube
  double Radius = 1.0;
  int NumVDims = 8; // Virtual dimensions
  // int MaxDepth = 16;
  int MaxDepth = NumVDims;
  int NumTris;
  Tri[] TriRay;
  List<Line> LineRay = new ArrayList<Line>();
  /* ********************************************************************************************************* */
  public TriDivide() {
    NumTris = 1 << MaxDepth;
    TriRay = new Tri[NumTris];
    Run();
    DumpLines();
    // DumpTris();
  }
  /* ********************************************************************************************************* */
  public void DumpTris() {
    Pnt pt = new Pnt();
    StringBuilder sb = new StringBuilder();
    sb.Append("o Cloud" + Environment.NewLine);
    for (int tcnt = 0; tcnt < NumTris; tcnt++) {
      Tri tri = TriRay[tcnt];
      tri.GetCenter(pt);
      String txtln = String.format("v {0:0.000000} {1:0.000000} {2:0.000000}", pt.Loc[0], pt.Loc[1], pt.Loc[2]);
      sb.Append(txtln + Environment.NewLine);
    }
    File.WriteAllText("pcloud2.obj", sb.toString());
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
  public void DumpLines() {
    Pnt pt0, pt1;
    StringBuilder sb = new StringBuilder();
    sb.Append("o Frame" + Environment.NewLine);
    int pcnt = 0;
    for (int lcnt = 0; lcnt < LineRay.Count; lcnt++) {
      Line ln = LineRay[lcnt];
      
      pcnt++;
      ln.Pdex[0] = pcnt;
      pt0 = ln.Vtx[0];
      
      pcnt++;
      ln.Pdex[1] = pcnt;
      pt1 = ln.Vtx[1];
      
      String txtln0 = String.format("v {0:0.000000} {1:0.000000} {2:0.000000}", pt0.Loc[0], pt0.Loc[1], pt0.Loc[2]);
      String txtln1 = String.format("v {0:0.000000} {1:0.000000} {2:0.000000}", pt1.Loc[0], pt1.Loc[1], pt1.Loc[2]);
      
      sb.Append(txtln0 + Environment.NewLine);
      sb.Append(txtln1 + Environment.NewLine);
    }
    for (int lcnt = 0; lcnt < LineRay.Count; lcnt++) {
      Line ln = LineRay.get(lcnt);
      String txtln0 = String.format("l {0} {1}", ln.Pdex[0], ln.Pdex[1]);
      sb.Append(txtln0 + Environment.NewLine);
    }
    File.WriteAllText("wires.obj", sb.ToString());
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
  public void Run() {
    Tri octant = new Tri();// Start with (0,0,1), (0,1,0), (1,0,0) for one octant of sphere 
    octant.Vtx[0].Assign(0, 0, 1);
    octant.Vtx[1].Assign(0, 1, 0);
    octant.Vtx[2].Assign(1, 0, 0);
    Tri_Split(octant, 0, 0);
  }
  /* ********************************************************************************************************* */
  class Pnt {
    public double[] Loc = new double[3];
    public Pnt CloneMe() {
      Pnt child = new Pnt();
      for (int dcnt = 0; dcnt < 3; dcnt++) {
        child.Loc[dcnt] = this.Loc[dcnt];
      }
      return child;
    }
    public void Copy(Pnt other) {
      for (int dcnt = 0; dcnt < 3; dcnt++) {
        this.Loc[dcnt] = other.Loc[dcnt];
      }
    }
    public void Zero() {
      for (int dcnt = 0; dcnt < 3; dcnt++) {
        this.Loc[dcnt] = 0.0;
      }
    }
    public void Add(Pnt other) {
      for (int dcnt = 0; dcnt < 3; dcnt++) {
        this.Loc[dcnt] += other.Loc[dcnt];
      }
    }
    public void Multiply(double factor) {
      for (int dcnt = 0; dcnt < 3; dcnt++) {
        this.Loc[dcnt] *= factor;
      }
    }
    public double GetMagnitude() { // from 0,0,0
      double SumSq = 0;
      double Value;
      for (int dcnt = 0; dcnt < 3; dcnt++) {
        Value = this.Loc[dcnt];
        SumSq += Value * Value;
      }
      SumSq = Math.sqrt(SumSq);
      return SumSq;
    }
    /* ********************************************************************************************************* */
    public void MapToSphere(double Radius) {
      double dist = this.GetMagnitude(); // from 0,0,0
      double factor = Radius / dist;
      for (int dcnt = 0; dcnt < 3; dcnt++) {
        this.Loc[dcnt] *= factor;
      }
    }
    /* ********************************************************************************************************* */
    public void Assign(double d0, double d1, double d2) {
      this.Loc[0] = d0;
      this.Loc[1] = d1;
      this.Loc[2] = d2;
    }
    /* ********************************************************************************************************* */
    public void Delete() {
    }
  }
  /* ********************************************************************************************************* */
  class Line {
    public Pnt[] Vtx = new Pnt[2];
    public int[] Pdex = new int[2];// indexes to vertex list for my endpoints 
            /* ********************************************************************************************************* */
    public Line() {
      for (int pcnt = 0; pcnt < 2; pcnt++) {
        this.Vtx[pcnt] = new Pnt();;
      }
    }
    /* ********************************************************************************************************* */
    public void Assign(Pnt p0, Pnt p1) {
      this.Vtx[0].Copy(p0);
      this.Vtx[1].Copy(p1);
    }
    /* ********************************************************************************************************* */
    public void Subtract(double Chop) {
      Pnt p0 = new Pnt(), p1 = new Pnt();
      p0.Copy(this.Vtx[0]);
      p1.Copy(this.Vtx[1]);
      p0.Multiply(-1);
      p1.Add(p0);// move this line to 0,0,0
      double len = p1.GetMagnitude();
      double NextLen = len - Chop;
      double Ratio = NextLen / len;
      p1.Multiply(Ratio);// mult this line by that ratio
      p1.Add(this.Vtx[0]);// move this line back to its place 
      this.Vtx[1].Copy(p1);
      /*
       shorten this line by Chop amount
       first, get magnitude of this line
       then subtract chop from that
       then get ratio of shorter/longer
       move this line to 0,0,0
       mult this line by that ratio
       move this line back to its place 
       */
    }
  }
  /* ********************************************************************************************************* */
  class Tri {
    public int BitAddress = 0; // assign adecuados shift bits 
    public int RecurDepth = 0; // for whatever reason
    public Pnt[] Vtx = new Pnt[3];
    public Tri() {
      for (int pcnt = 0; pcnt < 3; pcnt++) {
        this.Vtx[pcnt] = new Pnt();;
      }
    }
    public void Assign(Pnt p0, Pnt p1, Pnt p2) {
      this.Vtx[0].Copy(p0);
      this.Vtx[1].Copy(p1);
      this.Vtx[2].Copy(p2);
    }
    /* ********************************************************************************************************* */
    public void GetCenter(Pnt ptresult) {// Return the center of the triangle
      ptresult.Zero();// average them all 
      for (int pcnt = 0; pcnt < 3; pcnt++) {
        ptresult.Add(this.Vtx[pcnt]);
      }
      ptresult.Multiply(1.0 / 3.0);
    }
    /* ********************************************************************************************************* */
    public void Delete() {
      for (int pcnt = 0; pcnt < 3; pcnt++) {
        this.Vtx[pcnt].Delete();
      }
    }
  }
  /* ********************************************************************************************************* */
  void Tri_Split(Tri tri, int SplitVtxDex, int RecurDepth) {
    tri.RecurDepth = RecurDepth;// maybe needed sometime? 
    if (RecurDepth >= MaxDepth) {
      TriRay[tri.BitAddress] = tri;
      return;
    }
    
    Pnt SplitVtx = tri.Vtx[SplitVtxDex];
    int LeftVertDex = (SplitVtxDex + 1);
    if (LeftVertDex >= 3) {
      LeftVertDex -= 3;
    }
    int RightVertDex = (SplitVtxDex - 1);
    if (RightVertDex < 0) {
      RightVertDex += 3;
    }
    
    Pnt RightVert = tri.Vtx[RightVertDex];// ClockVert
    Pnt LeftVert = tri.Vtx[LeftVertDex];// AntiClockVert

    // Create new point, bisecting opposite side
    Pnt ResultVtx = new Pnt();// split opposite line
    SplitLine(LeftVert, RightVert, ResultVtx);
    if (false) {
      ResultVtx.MapToSphere(Radius);// move pt to sphere
    } else {
      Line ln = new Line();
      ln.Assign(ResultVtx, tri.Vtx[SplitVtxDex]);
      ln.Subtract(0.05);
      this.LineRay.add(ln);
    }

    // Create tri 0, 'left' of bisecting line 
    Tri tri0 = new Tri();// apex + anticlock_angle + newpt;
    tri0.Assign(ResultVtx, SplitVtx, LeftVert);
    tri0.BitAddress = tri.BitAddress;
    tri0.BitAddress <<= 1;
    tri0.BitAddress |= 0x0;// nop, just a note to self

    // Create tri 1, 'right' of bisecting line 
    Tri tri1 = new Tri();// apex + clock_angle + newpt;
    tri1.Assign(ResultVtx, RightVert, SplitVtx);
    tri1.BitAddress = tri.BitAddress;
    tri1.BitAddress <<= 1;
    tri1.BitAddress |= 0x1;
    
    ResultVtx.Delete();
    
    Tri_Split(tri0, 0, RecurDepth + 1);// 0 is index of Result vertex for both sub-triangles 
    Tri_Split(tri1, 0, RecurDepth + 1);
    
    tri0.Delete();
    tri1.Delete();
  }
  /* ********************************************************************************************************* */
  void SplitLine(Pnt pt0, Pnt pt1, Pnt ptresult) {// Return the midpoint of a line 
    for (int dcnt = 0; dcnt < 3; dcnt++)// from 0,0,0
    {
      ptresult.Loc[dcnt] = (pt0.Loc[dcnt] + pt1.Loc[dcnt]) / 2.0;
    }
  }
}
/*
 hcube:
 diffln = delta surface points;
 diffmag = len of diffln;
 halfdiffmag = (diffmag/2)
 cdm(ctr to diffln magnitude) = sqrt( (radius^2) - (0.5*(halfdiffmag)) ); // pythag 
 ratio = (diffmag/2)/cdm;// ratio of opposite/adjacent 
 extra = ratio * (halfdiffmag);// extra distance from cdm to orbit 
 orbitmag = cdm + ratio;// full orbit distance from center 
 orbitratio = orbitmag / cdm;
 orbitpoint = diffln * orbitratio;// center of circle/sphere whose edges are perpendicular to surface 

 next?  hang curves between points on sphere. could just walk along the straight line, get dist from orbit and project to radius of moon. 
 better: in 2d, walk angle around the moon and get growing X value. use that as along-the-way ratio for above formula. 
 endangle =  arcsin((halfdiffmag) / moonradius); // opposite over hypot 
 startangle = -endangle;

 for (double angle=startangle to endangle step 0.whatever){
 // XTravel = halfdiffmag + (sin(angle) * moonradius); // not right, starts in the wrong place 
 or 
 XTravelFactor = (sin(angle)) - sin(startangle); // wrong, also need to scale by length of real line 
 // slowly grow diffln along XTravelFactor, and use its 2d X value in circle formula to get arch 
 }
 */

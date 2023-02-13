using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlacingIsolateWithCutVoid
{
  
    public static class GetGeometry
    {
       
        public static void newFamily(Document doc, FamilySymbol symbol, Element el, Options op)
        {
            PlanarFace face = GetFaceBottom(el, op);
            //Face face2 = face as Face;

            //Get Location point
            BoundingBoxUV bboxUV = face.GetBoundingBox();
            UV center = (bboxUV.Max + bboxUV.Min) / 2.0;
            XYZ location = face.Evaluate(center);

            //Get vector refDir
            XYZ normal = face.ComputeNormal(center);
            XYZ refDir = normal.CrossProduct(XYZ.BasisZ);

            //Stat code
            Transaction tran = new Transaction(doc, "chay");
            tran.Start();
            FamilyInstance newFamilys = doc.Create.NewFamilyInstance(face, location, refDir, symbol);
            Element els = newFamilys as Element;
            //SolidSolidCutUtils.AddCutBetweenSolids(doc,el,els,false);
            InstanceVoidCutUtils.AddInstanceVoidCut(doc, el, els);
            tran.Commit();

        }
        private static PlanarFace GetFaceBottom(Element el, Options op)
        {
            GeometryElement GeoElement = el.get_Geometry(op);
            List<Face> faceListSolid = new List<Face>();
            List<PlanarFace> faceElement = new List<PlanarFace>();
            foreach (GeometryObject GeoObj in GeoElement)
            {
                Solid solidElement = GeoObj as Solid;
                if (solidElement != null)
                {
                    foreach (Face face in solidElement.Faces)
                    {
                        faceListSolid.Add(face);
                    }
                    foreach (PlanarFace f in faceListSolid)
                    {
                        if (f.FaceNormal.IsAlmostEqualTo(new XYZ(0, 0, -1)))
                        {
                            faceElement.Add(f);
                            break;
                        }
                    }
                }
                else
                {
                    GeometryInstance geoIn = GeoObj as GeometryInstance;
                    GeometryElement geoel = geoIn.GetInstanceGeometry();
                    foreach (GeometryObject elgeo in geoel)
                    {
                        Solid solidin = elgeo as Solid;
                        if (solidin != null)
                        {
                            foreach (Face face in solidin.Faces)
                            {
                                faceListSolid.Add(face);
                            }
                            foreach (PlanarFace f in faceListSolid)
                            {
                                if (f.FaceNormal.IsAlmostEqualTo(new XYZ(0, 0, -1)))
                                {
                                    faceElement.Add(f);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return faceElement[0];

        }//ReturnFace
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI.Selection;

namespace PlacingIsolateWithCutVoid
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class NewFamilyIsolate : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp =  commandData.Application;
            UIDocument uidoc =  uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;


            ICollection<ElementId> selectAll = uidoc.Selection.GetElementIds();
            //Get familySymbol
            Reference r2 = uidoc.Selection.PickObject(ObjectType.Element);
            ElementId id2 = r2.ElementId;
            Element el2 = doc.GetElement(id2);

            FamilyInstance familyInstance = el2 as FamilyInstance;
            FamilySymbol symbol = familyInstance.Symbol;


            //GetOption
            Options op = new Options();
            op.ComputeReferences = true;
            op.DetailLevel = ViewDetailLevel.Fine;

            foreach (ElementId id in selectAll)
            {
                Element el = doc.GetElement(id);
                GetGeometry.newFamily(doc, symbol, el, op);
            }

            return Result.Succeeded;
        }
    }
}

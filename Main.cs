using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitAPI4._1
{
    [Transaction(TransactionMode.Manual)]
    public class Main : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
                        
            string wallInfo = string.Empty;

            var walls = new FilteredElementCollector(doc, doc.ActiveView.Id)
                .OfClass(typeof(Wall))
                .Cast<Wall>()
                .ToList();

            foreach (Wall wall in walls)
            {
                Parameter wallType = wall.get_Parameter(BuiltInParameter.ELEM_FAMILY_AND_TYPE_PARAM);
                Parameter wallVolume = wall.get_Parameter(BuiltInParameter.HOST_VOLUME_COMPUTED);
                string wallName = wallType.AsValueString();
                string wallVol = UnitUtils.ConvertFromInternalUnits(wallVolume.AsDouble(), UnitTypeId.CubicMeters).ToString();
                wallInfo += $"{wallName}\t{wallVol}{Environment.NewLine}";
            }

            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string csvPath = Path.Combine(desktopPath, "wallInfo.csv");

            File.WriteAllText(csvPath, wallInfo);

            TaskDialog.Show("Selection", "Данные успешно записаны");

            return Result.Succeeded;
        }
    }
}

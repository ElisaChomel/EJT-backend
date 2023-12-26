using ClosedXML.Excel;
using judo_backend.Models;
using judo_backend.Models.Enum;
using judo_backend.Services.Interfaces;

namespace judo_backend.Services
{
    public class ExcelGeneratorService : IExcelGeneratorService
    {
        public XLWorkbook GenerateEjtAdherentList(string title, List<EjtAdherent> data)
        {
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add(title);

            //************************************* CSS ************************************
            worksheet.Columns("A:A").Width = 3;
            worksheet.Columns("H:H").Width = 3;

            worksheet.Columns("B:G").Width = 25;
            worksheet.Ranges("B2:G2").Style.Fill.SetBackgroundColor(XLColor.DodgerBlue);
            worksheet.Ranges($"B2:G{data.Count + 2}").Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
            worksheet.Ranges($"B2:G{data.Count + 2}").Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);

            worksheet.Row(1).Height = 30;
            worksheet.Row(1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
            worksheet.Row(1).Group();

            //*********************************** Titre ***********************************
            worksheet.Cell("A1").Value = title;

            //*********************************** Entête ***********************************
            worksheet.Cell("B2").Value = "Licence";
            worksheet.Cell("C2").Value = "Nom";
            worksheet.Cell("D2").Value = "Prénom";
            worksheet.Cell("E2").Value = "Date de naissance";
            worksheet.Cell("F2").Value = "Ceinture";
            worksheet.Cell("G2").Value = "Poids";

            //************************************ Corps ***********************************
            int index = 3;
            foreach (var item in data)
            {
                worksheet.Cell($"B{index}").Value = item.LicenceCode;
                worksheet.Cell($"C{index}").Value = item.Lastname;
                worksheet.Cell($"D{index}").Value = item.Firstname;
                worksheet.Cell($"E{index}").Value = item.Birthday.ToString("dd/MM/yyyy");
                worksheet.Cell($"F{index}").Value = item.Belt.HasValue ? GetBeltName(item.Belt.Value) : "Blanche";
                worksheet.Cell($"G{index}").Value = $"{item.Weight} kg";

                index++;
            }

            return workbook;
        }

        public string GetBeltName(BeltType beltType)
        {
            switch(beltType)
            {
                case BeltType.White1Yellow:
                    return "Blanche 1 liseré";
                case BeltType.White2Yellow:
                    return "Blanche 2 liserés";
                case BeltType.WhiteYellow:
                    return "Blanche et jaune";
                case BeltType.Yellow:
                    return "Jaune";
                case BeltType.YellowOrange:
                    return "Jaune et orange";
                case BeltType.Orange:
                    return "Orange";
                case BeltType.OrangeGreen:
                    return "Orange et verte";
                case BeltType.Green:
                    return "Verte";
                case BeltType.GreenBlue:
                    return "Verte et bleue";
                case BeltType.Blue:
                    return "Bleue";
                case BeltType.BlueBrown:
                    return "Bleue et marron";
                case BeltType.Brown:
                    return "Marron";
                case BeltType.Black:
                    return "Noire";
                default:
                    return "Blanche";

            }
        }
    }
}

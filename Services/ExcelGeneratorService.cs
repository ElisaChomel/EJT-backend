using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Spreadsheet;
using judo_backend.Models;
using judo_backend.Models.Enum;
using judo_backend.Services.Interfaces;

namespace judo_backend.Services
{
    public class ExcelGeneratorService : IExcelGeneratorService
    {
        public XLWorkbook GenerateCommandeList(List<ClotheOrderItem> items, List<ClotheOrder> orders)
        {
            var workbook = new XLWorkbook();
            var worksheet1 = workbook.Worksheets.Add("Commande de vêtements");
            var worksheet2 = workbook.Worksheets.Add("Detail des commandes");

            //************************************* CSS ***********************************

            worksheet1.Columns("A:A").Width = 3;
            worksheet1.Columns("F:F").Width = 3;

            worksheet1.Columns("B:E").Width = 25;
            worksheet1.Ranges("B2:E2").Style.Fill.SetBackgroundColor(XLColor.DodgerBlue);
            worksheet1.Ranges($"B2:E{items.Count + 2}").Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
            worksheet1.Ranges($"B2:E{items.Count + 2}").Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);

            worksheet1.Row(1).Height = 30;
            worksheet1.Row(1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
            worksheet1.Row(1).Group();


            worksheet2.Columns("A:A").Width = 3;
            worksheet2.Columns("H:H").Width = 3;

            worksheet2.Columns("B:G").Width = 25;
            worksheet2.Ranges("B2:G2").Style.Fill.SetBackgroundColor(XLColor.DodgerBlue);
            worksheet2.Ranges($"B2:G{orders.Sum(x => x.Items.Count) + 2}").Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
            worksheet2.Ranges($"B2:G{orders.Sum(x => x.Items.Count) + 2}").Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);

            worksheet2.Row(1).Height = 30;
            worksheet2.Row(1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
            worksheet2.Row(1).Group();

            //*********************************** Titre ***********************************
            worksheet1.Cell("A1").Value = "Commande de vêtements";
            worksheet2.Cell("A1").Value = "Detail des commandes";

            //*********************************** Entête 1 ********************************
            worksheet1.Cell("B2").Value = "REF";
            worksheet1.Cell("C2").Value = "TAILLE";
            worksheet1.Cell("D2").Value = "QUANTITE";
            worksheet1.Cell("E2").Value = "PRIX";

            //*********************************** Entête 2 ********************************
            worksheet2.Cell("B2").Value = "COMMANDE";
            worksheet2.Cell("C2").Value = "EMAIL";
            worksheet2.Cell("D2").Value = "REF";
            worksheet2.Cell("E2").Value = "TAILLE";
            worksheet2.Cell("F2").Value = "QUANTITE";
            worksheet2.Cell("G2").Value = "PRIX";


            //************************************ Corps 1 *********************************
            int index1 = 3;
            foreach (var item in items)
            {
                worksheet1.Cell($"B{index1}").Value = item.Reference;
                worksheet1.Cell($"C{index1}").Value = item.Size;
                worksheet1.Cell($"D{index1}").Value = item.Quantity;
                worksheet1.Cell($"E{index1}").Value = $"{item.Price}€";

                index1++;
            }


            //************************************ Corps 2 *********************************
            int index2 = 3;
            foreach (var order in orders)
            {
                foreach (var item in order.Items)
                {
                    worksheet2.Cell($"B{index2}").Value = order.Reference;
                    worksheet2.Cell($"C{index2}").Value = order.Email;
                    worksheet2.Cell($"D{index2}").Value = item.Reference;
                    worksheet2.Cell($"E{index2}").Value = item.Size;
                    worksheet2.Cell($"F{index2}").Value = item.Quantity;
                    worksheet2.Cell($"G{index2}").Value = $"{item.Price}€";

                    index2++;
                }
            }

            return workbook;
        }

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
            switch (beltType)
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

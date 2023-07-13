using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;


using NMS.Data;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;

using Newtonsoft.Json;
using NMS;
using NMS.Tools;
using NMS;

namespace NMS.Controllers
{
    public class ReportViewController : BaseController
    {
        // 
        // GET: /ReportView/
        Inventory_ManagementXEntities db = new Inventory_ManagementXEntities();
        public void ViewReport(string strReportName, int id = 0)
        {
            try
            {
                using (ReportDocument rd = new ReportDocument())
                {
                    Tools tool = new Tools();
                    //  string lang = CommonUtils.GetLanguage();
                    if (string.IsNullOrEmpty(strReportName))
                        strReportName = Convert.ToString(Session["ReportName"]);
                    string strRptPath = tool.getProgectPath() + strReportName + ".rpt";
                    //When Publis Add Trade after getProjectPath() 
                    // Response.Write("<H2>Path=" + strRptPath + "</H2>");



                    if (Session["ReportData"] != null)
                    {
                        var ReportData = Session["ReportData"];

                        rd.Load(strRptPath);
                        
                        if (Session["ReportOption"] != null )
                        {
                            //Tst Stak Over Fllow

                            rd.SetDataSource(new[] { ReportData });

                            //if (Session["ReportOption"].ToString() == "Report")
                            //{
                            //    rd.Database.Tables[0].SetDataSource(ReportData);
                            //    DataSet dsTempReport = new DataSet();
                            //    dsTempReport.ReadXml(Server.MapPath("~/App_Data/CarText.xml"));
                            //    if (rd.Database.Tables.Count > 1)
                            //        rd.Database.Tables[1].SetDataSource(dsTempReport);
                            //    Session["ReportOption"] = null;
                            //}
                        }
                        else
                        
                            rd.SetDataSource(ReportData);


                        List<KeyValuePair<string, string>> reportParameters = (List<KeyValuePair<string, string>>)Session["ReportParameter"];
                        if (reportParameters != null)
                        {
                            //List<KeyValuePair<string, string>> reportParameters = (List<KeyValuePair<string, string>>)Session["ReportParameter"];
                            foreach (var reportParameter in reportParameters)
                            {
                                rd.SetParameterValue("@" + reportParameter.Key, reportParameter.Value);
                            }
                        }



                        string receviedType = Convert.ToString(Session["ReportType"]);
                        var type = ExportFormatType.PortableDocFormat;
                        switch (receviedType)
                        {
                            case "excel":
                                type = ExportFormatType.ExcelWorkbook;
                                break;
                            case "word":
                                type = ExportFormatType.WordForWindows;
                                break;
                            default:
                                type = ExportFormatType.PortableDocFormat;
                                break;
                        }



                        rd.ExportToHttpResponse(type, System.Web.HttpContext.Current.Response, false, "Report");
                    }
                    else if (id != 0)
                    {

                        rd.Load(strRptPath);
                        if (Session["ReportParameter"] != null)
                        {
                            List<KeyValuePair<string, string>> reportParameters = (List<KeyValuePair<string, string>>)Session["ReportParameter"];
                            foreach (var reportParameter in reportParameters)
                            {
                                rd.SetParameterValue("@" + reportParameter.Key, reportParameter.Value);
                            }
                        }
                        rd.SetParameterValue("@Id", id);

                        if (strReportName.Contains("rptPrintRecord"))
                        {

                            rd.ExportToHttpResponse(ExportFormatType.WordForWindows, System.Web.HttpContext.Current.Response,
                                true, "Report");
                        }
                        else
                        {
                            rd.ExportToHttpResponse(ExportFormatType.PortableDocFormat, System.Web.HttpContext.Current.Response, false, "Report");

                        }
                    }
                    else
                    {
                        Response.Write("<H2>Nothing Found, No Data To Show</H2>");
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write("<H2>" + ex.Message + "</H2>");
            }
            finally
            {
                Session["ReportData"] = null;
                Session["ReportParameter"] = null;
                Session["ReportType"] = null;
            }
        }



    }
}

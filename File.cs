using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using NewConsole.Managers;
using NewConsole.Models;
using Newtonsoft.Json;
using RCRE.Filters;
using StarterWebProject.Managers;

namespace NewConsole.Controllers
{

    [LoginFilter]
    [MessageFilter]
    public class HQController : ApplicationController
    {
		
	[AuthorityFilter(7)]
        [HttpPost]
        public ActionResult OutsideSourceForm(FormCollection form)
        {
            //Posting a New or Existing Outside Source Form.

            long fid = long.TryParse(form["fid"], out fid) ? fid : 0;
            long aid = long.TryParse(form["aid"], out aid) ? aid : 0;

            string date = form["dateBox"];

            var osa = getOutsideSourceAccount(aid);

            if (osa == null || osa.ID <= 0 || aid == 0)
            {
                TempData["Error"] = "Could not locate Outside Source Account in databse 2.";
                return RedirectToRoute(new { controller = "HQ", action = "OutsideSourcePanel" });
            }

            DateTime failDate = CodeManager.GetCentralTime().AddYears(-100).Date;
            DateTime dt = DateTime.TryParse(date, out dt) ? dt.Date : failDate;

            if (dt.Date == failDate.Date)
            {
                ViewBag.Error = "Failed to parse the date, or the date changed.";
                return RedirectToRoute(new { controller = "HQ", action = "OutsideSourcePanel" });
            }


            var frm = (from x in db.OutsideSources where x.ID == fid && x.OutsideSourceID == aid select x).FirstOrDefault();
            if (frm == null || frm.ID <= 0)
            {
                //Creating a New Form
                frm = new OutsideSource();

                frm.VendorID = VENDOR.ID;
                frm.OutsideSourceID = aid;

                frm.Flow_Total = CodeManager.FixNullableDecimalValue(form["iBox1"]);
                frm.TruckWeight = CodeManager.FixNullableDecimalValue(form["iBox2"]);
                frm.ORP = CodeManager.FixNullableDecimalValue(form["iBox3"]);
                frm.Temperature = CodeManager.FixNullableDecimalValue(form["iBox4"]);
                frm.pH = CodeManager.FixNullableDecimalValue(form["iBox5"]);

                frm.COD_lbs_per_day = CodeManager.FixNullableDecimalValue(form["iBox6"]);
                frm.COD_Dilution = CodeManager.FixNullableDecimalValue(form["iBox7"]);
                frm.COD_Value = CodeManager.FixNullableDecimalValue(form["iBox8"]);
                frm.COD_mg_L = CodeManager.FixNullableDecimalValue(form["iBox9"]);

                frm.TP_lbs_per_day = CodeManager.FixNullableDecimalValue(form["iBox10"]);
                frm.TP_Dilution = CodeManager.FixNullableDecimalValue(form["iBox11"]);
                frm.TP_Value = CodeManager.FixNullableDecimalValue(form["iBox12"]);
                frm.TP_mg_L = CodeManager.FixNullableDecimalValue(form["iBox13"]);

                frm.Date = dt;

                frm.Cost_Per_Gallon_instance = osa.Cost_Per_Gallon;
                frm.Date_Billed = null;
                frm.Has_Been_Billed = false;
                frm.Outside_Source_Name = osa.OutsideSource_Name;
                frm.Surcharge_amount_per_gallon_instance = osa.Surcharge_amount_per_gallon;

                db.OutsideSources.Add(frm);
                db.SaveChanges();

                TempData["Success"] = "You have successfully saved a new Outside Source Form.";
                return RedirectToRoute(new { controller = "HQ", action = "Lab" });
            }


            //Editing an existing form.

            frm.VendorID = VENDOR.ID;
            frm.OutsideSourceID = aid;

            frm.Flow_Total = CodeManager.FixNullableDecimalValue(form["iBox1"]);
            frm.TruckWeight = CodeManager.FixNullableDecimalValue(form["iBox2"]);
            frm.ORP = CodeManager.FixNullableDecimalValue(form["iBox3"]);
            frm.Temperature = CodeManager.FixNullableDecimalValue(form["iBox4"]);
            frm.pH = CodeManager.FixNullableDecimalValue(form["iBox5"]);

            frm.COD_lbs_per_day = CodeManager.FixNullableDecimalValue(form["iBox6"]);
            frm.COD_Dilution = CodeManager.FixNullableDecimalValue(form["iBox7"]);
            frm.COD_Value = CodeManager.FixNullableDecimalValue(form["iBox8"]);
            frm.COD_mg_L = CodeManager.FixNullableDecimalValue(form["iBox9"]);

            frm.TP_lbs_per_day = CodeManager.FixNullableDecimalValue(form["iBox10"]);
            frm.TP_Dilution = CodeManager.FixNullableDecimalValue(form["iBox11"]);
            frm.TP_Value = CodeManager.FixNullableDecimalValue(form["iBox12"]);
            frm.TP_mg_L = CodeManager.FixNullableDecimalValue(form["iBox13"]);

            frm.Date = dt;

            frm.Cost_Per_Gallon_instance = osa.Cost_Per_Gallon;
            frm.Outside_Source_Name = osa.OutsideSource_Name;
            frm.Surcharge_amount_per_gallon_instance = osa.Surcharge_amount_per_gallon;

            db.SaveChanges();

            TempData["Success"] = "You have successfully saved your Outside Source Form.";
            return RedirectToRoute(new { controller = "HQ", action = "Lab" });

        }
		
	}
	
}

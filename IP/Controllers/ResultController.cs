﻿using System.Linq;
using System.Web.Mvc;
using PagedList;
using System.Collections.Generic;
using System;
using IP.Models;
using System.Threading.Tasks;
using System.Data.Entity;

namespace IP.Controllers
{
    public class ResultController : Controller
    {
        FlowDatas db = new FlowDatas();
        static List<IP.Models.Table> Result;

        public ActionResult Index(DateTime? SSTime, DateTime? SETime, int? Srisk,string addrSearch, string type, int? page)
        {
            Models.Database1Entities db = new Models.Database1Entities();
            List<IP.Models.Table> result = (db.Table.OrderBy(p => p.Id_F).ThenBy(n => n.IPAddr_F).ToList());




            if (addrSearch != "")
            {
                result = (result.Where(x => x.IPAddr_F == addrSearch || addrSearch == null).ToList());
            }
            if (Srisk == 1|| Srisk == 2|| Srisk == 3)
            {
                result = (result.Where(x => x.Result_F == Srisk).ToList());
            }
            if (SSTime!=null || SETime != null)
            {

                if (SSTime != null )
                {
                    result = (result.Where(x => x.Time_F >= SSTime).ToList());
                }
                if (SETime != null)
                {
                    result = (result.Where(x => x.Time_F <= SETime).ToList());
                }

            }



            switch (type)
            {
                case "Risk":
                    result = (result.OrderBy(p => p.Result_F).ThenBy(n => n.IPAddr_F).ToList());
                    break;
                case "Time":
                    result = (result.OrderBy(p => p.Time_F).ThenBy(n => n.IPAddr_F).ToList());
                    break;
                default:
                    result = (result.OrderBy(p => p.Id_F).ThenBy(n => n.IPAddr_F).ToList());
                    break;
            }
            Result = result;
            return View(Result.ToPagedList(page ?? 1, 20));  
        }

        public async Task<ActionResult> HistoricalRecord(DateTime? Date)
        {
            List<AnalysisResults> analysisResults;
            if (Date == null)
            {
                analysisResults = await db.AnalysisResults.ToListAsync();
                Date = DateTime.Today;
            }
            else
            {
                DateTime EndTime = Date.Value.AddDays(1);
                analysisResults = await db.AnalysisResults.Where(c => c.AnalysisTime >= Date && c.AnalysisTime < EndTime).ToListAsync();
            }
            ViewBag.datetimepicker = Date.Value.Year + "-" + Date.Value.Month + "-" + Date.Value.Day;
            return View(analysisResults);
        }

        public ActionResult RefreshData( string type, int? page)
        {
            return View(Result.ToPagedList(page ?? 1, 20));
        }

        public ActionResult Chart(string addr)
        {
            Models.Database1Entities db = new Models.Database1Entities();
            var result = (db.Table.Where(x => x.IPAddr_F == addr));
            foreach (var item in result)
            {

            }
            ViewBag.a = result;

            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}




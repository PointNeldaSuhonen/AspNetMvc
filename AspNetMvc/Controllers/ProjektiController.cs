using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AspNetMvc.Models;
using Newtonsoft.Json;

namespace AspNetMvc.Controllers
{
    public class ProjektiController : Controller
    {
        // GET: Projekti
        public ActionResult Index()
        {
                ViewBag.OmaTieto = "Tämä on oikeaa tietoa!!";

                HarjoitusEntities entities = new HarjoitusEntities();
                List<Projektit> model = entities.Projektit.ToList();
                entities.Dispose();

                return View(model);
         }

            public ActionResult Index2()
            {
                return View();
            }

            public ActionResult Index3()
            {
                return View();
            }

        public JsonResult GetList()
            {
                HarjoitusEntities entities = new HarjoitusEntities();
                // List<Projektit> model = entities.Projektit.ToList();

                var model = (from c in entities.Projektit
                             select new
                             {
                                 ProjektiID = c.ProjektiID,
                                 Nimi = c.Nimi,
                             }).ToList();

                string json = JsonConvert.SerializeObject(model);
                entities.Dispose();

            //välimuistin hallinta
            //Response.Expires = -1;
            //Response.CacheControl = "no-cache";

            return Json(json, JsonRequestBehavior.AllowGet);
            }
        public JsonResult GetSingleProjekti(string id)
        {
            HarjoitusEntities entities = new HarjoitusEntities();
            int projektiid = int.Parse(id);
            var model = (from c in entities.Projektit
                         where c.ProjektiID == projektiid
                         select new
                         {
                             ProjektiID = c.ProjektiID,
                             Nimi = c.Nimi
                         }).FirstOrDefault();

            string json = JsonConvert.SerializeObject(model);
            entities.Dispose();

            return Json(json, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Update(Projektit proj)
        {

            HarjoitusEntities entities = new HarjoitusEntities();

            //oletetaan että tallennusoperaatio ei onnistu
            bool OK = false;

            // onko kyseessä muokkaus vai uuden lisääminen?
            //if (id == "(uusi)")
            if (proj.ProjektiID == 0)
            //if (id == null)
            {
                // kyseessä on uuden asiakkaan lisääminen, kopioidaan kentät
                Projektit dbItem = new Projektit()
                {
                    //ProjektiID = proj.ProjektiID,
                    Nimi = proj.Nimi
                };

                // tallennus tietokantaan
                entities.Projektit.Add(dbItem);
                entities.SaveChanges();
                OK = true;
            }
            else
            {
                //haetaan id:n perusteella rivi SQL tietokannasta
                Projektit dbItem = (from h in entities.Projektit
                                   where h.ProjektiID == proj.ProjektiID
                                   select h).FirstOrDefault(); //haetaan vain yhden projektin tiedot

                //jos tiedot löytyvät eli ei ole null
                if (dbItem != null)
                {
                    //dbItem.ProjektiID = proj.ProjektiID;  //tätä ei käytetä
                    dbItem.Nimi = proj.Nimi;

                    // tallennus SQL tietokantaan
                    entities.SaveChanges();

                    //jos tallennus onnistuu
                    OK = true;
                }
            }
            //entiteettiolion vapauttaminen
            entities.Dispose();

            // palautetaan 'json' muodossa
            return Json(OK, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(string id)
        {
            HarjoitusEntities entities = new HarjoitusEntities();

            //etsitään id:n perusteella projektit kannasta
            int projektiid = int.Parse(id);
            bool OK = false;
            Projektit dbItem = (from h in entities.Projektit
                                where h.ProjektiID == projektiid
                                select h).FirstOrDefault();
             
            if (dbItem != null)
            {
                //tietokannasta poisto
                entities.Projektit.Remove(dbItem);
                //tallennus SQL tietokantaan
                entities.SaveChanges();

                //jos tallennus onnistuu
                OK = true;
            }

            entities.Dispose();

            return Json(OK, JsonRequestBehavior.AllowGet);
        }
    }
}
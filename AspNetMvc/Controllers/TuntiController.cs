using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AspNetMvc.Models;
using Newtonsoft.Json;

namespace AspNetMvc.Controllers
{
    public class TuntiController : Controller
    {
        // GET: Tunti
        public ActionResult Index()
        {
            ViewBag.OmaTieto = "oikeaa tietoa";

            HarjoitusEntities entities = new HarjoitusEntities();
            List<Tunnit> model = entities.Tunnit.ToList();
            entities.Dispose();

            return View(model);
        }

        public ActionResult Index2() //sama kuin index, mutta eri tavalla tuotu tiedot
        {
            return View();
        }

        public ActionResult Index3() //sama kuin index2, mutta lisätty muokkaustoiminnot
        {
            return View();
        }

        public JsonResult GetList()
        {
            HarjoitusEntities entities = new HarjoitusEntities();
            // List<Tunnit> model = entities.Tunnit.ToList();

            var model = (from c in entities.Tunnit
                         select new
                         {
                             TuntiID = c.TuntiID,
                             ProjektiID = c.ProjektiID,
                             HenkiloID = c.HenkiloID,
                             Pvm = c.Pvm,
                             ProjektiTunnit = c.ProjektiTunnit
                         }).ToList();

            string json = JsonConvert.SerializeObject(model);
            entities.Dispose();
            //välimuistin hallinta
            Response.Expires = -1;
            Response.CacheControl = "no-cache";

            return Json(json, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSingleTunti(string id)
        {
            HarjoitusEntities entities = new HarjoitusEntities();
            int tuntiid = int.Parse(id);
            var model = (from c in entities.Tunnit
                         where c.TuntiID == tuntiid
                         select new
                         {
                             TuntiID = c.TuntiID,
                             ProjektiID = c.ProjektiID,
                             HenkiloID = c.HenkiloID,
                             Pvm = c.Pvm,
                             ProjektiTunnit = c.ProjektiTunnit
                         }).FirstOrDefault();

            string json = JsonConvert.SerializeObject(model);
            entities.Dispose();

            return Json(json, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Update(Tunnit tunn)
        {

        HarjoitusEntities entities = new HarjoitusEntities();

         //oletetaan että tallennusoperaatio ei onnistu        
        bool OK = false;

            // onko kyseessä muokkaus vai uuden lisääminen?
            //if (id == "(uusi)")
            if (tunn.TuntiID == 0)
            //if (id == null)
            {
                // kyseessä on uuden asiakkaan lisääminen, kopioidaan kentät
                Tunnit dbItem = new Tunnit()
                {
                    //TuntiID = tunn.TuntiID,
                    ProjektiID = tunn.ProjektiID,
                    HenkiloID = tunn.HenkiloID,
                    Pvm = tunn.Pvm,
                    ProjektiTunnit = tunn.ProjektiTunnit
                };

             // tallennus tietokantaan
                entities.Tunnit.Add(dbItem);
                entities.SaveChanges();
                OK = true;
            }
            else
            {
                //haetaan id:n perusteella rivi SQL tietokannasta
                Tunnit dbItem = (from h in entities.Tunnit
                                   where h.TuntiID == tunn.TuntiID
                                   select h).FirstOrDefault(); //haetaan vain yhden henkilön tiedot

                //jos tiedot löytyvät eli ei ole null
                if (dbItem != null)
                {
                    //dbItem.TuntiID = tunn.TuntiID;  //tätä ei käytetä
                    dbItem.ProjektiID = tunn.ProjektiID;
                    dbItem.HenkiloID = tunn.HenkiloID;
                    dbItem.Pvm = tunn.Pvm;
                    dbItem.ProjektiTunnit = tunn.ProjektiTunnit;


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

            //etsitään id:n perusteella tunnit kannasta
            int tuntiid = int.Parse(id);
            bool OK = false;
            Tunnit dbItem = (from h in entities.Tunnit
                             where h.TuntiID == tuntiid
                             select h).FirstOrDefault();

            if (dbItem != null)
            {
                //tietokannasta poista
                entities.Tunnit.Remove(dbItem);
                // tallennus SQL tietokantaan
                entities.SaveChanges();

                //jos tallennus onnistuu
                OK = true;
            }

            entities.Dispose();

            return Json(OK,JsonRequestBehavior.AllowGet);
        }
    }
}
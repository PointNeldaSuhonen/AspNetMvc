using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AspNetMvc.Models;
using Newtonsoft.Json;

namespace AspNetMvc.Controllers
{
    public class HenkiloController : Controller
    {
        // GET: Henkilo
        public ActionResult Index()
        {
            ViewBag.OmaTieto = "ABC123";

            HarjoitusEntities entities = new HarjoitusEntities();
            List<Henkilot> model = entities.Henkilot.ToList();
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

        public ActionResult Index4()
        {
            return View();
        }

        public JsonResult GetList()
        {
            HarjoitusEntities entities = new HarjoitusEntities();
            // List<Henkilot> model = entities.Henkilot.ToList();

            var model = (from c in entities.Henkilot
                         select new
                         {
                             HenkiloID = c.HenkiloID,
                             Etunimi = c.Etunimi,
                             Sukunimi = c.Sukunimi,
                             Osoite = c.Osoite,
                             Esimies = c.Esimies
                         }).ToList();

            string json = JsonConvert.SerializeObject(model);
            entities.Dispose();
            //välimuistin hallinta
            Response.Expires = -1;
            Response.CacheControl = "no-cache";

            return Json(json, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSingleHenkilo(string id)
        {
            HarjoitusEntities entities = new HarjoitusEntities();
            int henkiloid = int.Parse(id);
            var model = (from c in entities.Henkilot
                         where c.HenkiloID == henkiloid
                         select new
                         {
                             HenkiloID = c.HenkiloID,
                             Etunimi = c.Etunimi,
                             Sukunimi = c.Sukunimi,
                             Osoite = c.Osoite,
                             Esimies = c.Esimies
                         }).FirstOrDefault();

            string json = JsonConvert.SerializeObject(model);
            entities.Dispose();

            return Json(json, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Update(Henkilot henk)
        {

            HarjoitusEntities entities = new HarjoitusEntities();

            //oletetaan että tallennusoperaatio ei onnistu
            bool OK = false;

            // onko kyseessä muokkaus vai uuden lisääminen?
            //if (id == "(uusi)")
            if (henk.HenkiloID == 0)
            //if (id == null)
            {
                // kyseessä on uuden asiakkaan lisääminen, kopioidaan kentät
                Henkilot dbItem = new Henkilot()
                {
                    //HenkiloID = henk.HenkiloID,
                    Etunimi = henk.Etunimi,
                    Sukunimi = henk.Sukunimi,
                    Osoite = henk.Osoite,
                    Esimies = henk.Esimies
                };

                // tallennus tietokantaan
                entities.Henkilot.Add(dbItem);
                entities.SaveChanges();
                OK = true;
            }
            else
            {
                //haetaan id:n perusteella rivi SQL tietokannasta
                Henkilot dbItem = (from h in entities.Henkilot
                                   where h.HenkiloID == henk.HenkiloID
                                   select h).FirstOrDefault(); //haetaan vain yhden henkilön tiedot

                //jos tiedot löytyvät eli ei ole null
                if (dbItem != null)
                {
                    //dbItem.HenkiloID = henk.HenkiloID;  //tätä ei käytetä
                    dbItem.Etunimi = henk.Etunimi;
                    dbItem.Sukunimi = henk.Sukunimi;
                    dbItem.Osoite = henk.Osoite;
                    dbItem.Esimies = henk.Esimies;


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

            //etsitään id:n perusteella henkilöt kannasta
            int henkiloid = int.Parse(id);
            bool OK = false;
            Henkilot dbItem = (from h in entities.Henkilot
                               where h.HenkiloID == henkiloid
                               select h).FirstOrDefault();

            if (dbItem != null)
            {
                //tietokannasta poisto
                entities.Henkilot.Remove(dbItem);
                //tallennus SQL tietokantaan
                entities.SaveChanges();

                //jos tallennus onnistuu
                OK = true;
            }

            entities.Dispose();

            return Json(OK,JsonRequestBehavior.AllowGet);
        }
    }
}
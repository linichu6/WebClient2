using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebClient2.Models;

namespace WebClient2.Controllers
{
    public class PatienController : Controller
    {
        private string _url = "https://localhost:7127";

        private HttpClient CreateHttpClient()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }
        // GET: PatienController
        public ActionResult Index()
        {
            ViewBag.ShowErr = false;
            var pList = GetPatiens().Result;
            return View(pList);
        }

        private async Task<IEnumerable<Patien>> GetPatiens()
        {
            using (var client = CreateHttpClient())
            {
                // https://localhost:7127/api/Patiens/GetAllPatiens
                // TODO: move to appsettings
                client.BaseAddress = new Uri($"{_url}/api/Patiens/GetAllPatiens");
                
                try
                {
                    using (var req = new System.Net.Http.HttpRequestMessage())
                    {
                        req.Method = new HttpMethod("GET");
                        req.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(
                                                                                                    System.Text.Encoding.UTF8.GetBytes($"{HttpContext.Session.GetString("username")}:{HttpContext.Session.GetString("password")}")));

                        var response = client.Send(req);

                        if (response.IsSuccessStatusCode)
                        {
                            var responeTxt = await response.Content.ReadAsStringAsync();
                            var pList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Patien>>(responeTxt);
                            
                            return await Task.FromResult(pList);
                        }
                    }
                }
                catch (Exception ex)
                { 
                }
                
            }

            return Enumerable.Empty<Patien>();
        }

        

        // GET: PatienController/Create
        public ActionResult Create()
        {
            ViewBag.ShowErr = false;
            return View();
        }

        // POST: PatienController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                
                Patien patien = new Patien() { Name = collection["name"] , Email = collection["email"], CreatedDate = DateTime.Now};
                AddPatien(patien);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        private async Task<bool> AddPatien(Patien patien)
        {
            using (var client = CreateHttpClient())
            {

                client.BaseAddress = new Uri($"{_url}/api/Patiens/AddPatien");
                
                try
                {
                    using (var req = new System.Net.Http.HttpRequestMessage())
                    {
                        req.Method = new HttpMethod("Post");
                        req.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(
                                                                                                    System.Text.Encoding.UTF8.GetBytes($"{HttpContext.Session.GetString("username")}:{HttpContext.Session.GetString("password")}")));
                        req.Content = new System.Net.Http.StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(patien));
                        req.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                        
                        var response = client.Send(req);

                        if (response.IsSuccessStatusCode)
                        {
                            var responeTxt = response.Content;

                            return true;
                        }
                    }
                }
                catch (Exception ex)
                {
                }

            }

            return false;
        }

        // GET: PatienController/Edit/5
        public ActionResult Edit(int id, string name, string email)
        {
            ViewBag.ShowErr = false;
            Patien patien = new Patien() {Id=id, Name = name, Email = email};
            return View(patien);
        }

        // POST: PatienController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                Patien patien = new Patien() {Id = id, Name = collection["name"], Email = collection["email"] };
                if(!EditPatien(patien))
                {
                    ViewBag.ShowErr = true;
                    ViewBag.ErrorMessage = "Save data failed";
                    return View(patien);
                }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        private bool EditPatien(Patien patien)
        {
            using (var client = CreateHttpClient())
            {

                client.BaseAddress = new Uri($"{_url}/api/Patiens/EditPatien/");
                
                try
                {
                    using (var req = new System.Net.Http.HttpRequestMessage())
                    {
                        req.Method = new HttpMethod("Put");
                        req.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(
                                                                                                    System.Text.Encoding.UTF8.GetBytes($"{HttpContext.Session.GetString("username")}:{HttpContext.Session.GetString("password")}")));
                        req.Content = new System.Net.Http.StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(patien));
                        req.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

                        var response = client.Send(req);

                        if (response.IsSuccessStatusCode)
                        {
                            var responeTxt = response.Content;

                            return true;
                        }
                    }
                }
                catch (Exception ex)
                {
                }

            }

            return false;
        }

        // GET: PatienController/Delete/5
        public ActionResult Delete(int id)
        {
            ViewBag.ShowErr = false;
            DeletePatien(id);
            return RedirectToAction(nameof(Index));
        }

        private bool DeletePatien(int id)
        {
            using (var client = CreateHttpClient())
            {
                client.BaseAddress = new Uri($"{_url}/api/Patiens/DeletePatien/{id}");
                
                try
                {
                    using (var req = new System.Net.Http.HttpRequestMessage())
                    {
                        req.Method = new HttpMethod("Delete");
                        req.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(
                                                                                                    System.Text.Encoding.UTF8.GetBytes($"{HttpContext.Session.GetString("username")}:{HttpContext.Session.GetString("password")}")));

                        var response = client.Send(req);

                        if (response.IsSuccessStatusCode)
                        {
                            var responeTxt = response.Content;
                            
                            return true;
                        }
                    }
                }
                catch (Exception ex)
                {
                }

            }

            return false;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Hair.Models;
using Microsoft.AspNetCore.Http;

namespace Hair.Controllers
{
    public class MHAppointmentDetailsController : Controller
    {
        private readonly HairContext _context;

        public MHAppointmentDetailsController(HairContext context)
        {
            _context = context;
        }

        // GET: MHAppointmentDetails
		//Write this part down for the final
        public async Task<IActionResult> Index(int? id)
        {
			//write this part down
			if (id != null)
			{
				HttpContext.Session.SetInt32("appointmentId", Convert.ToInt32(id));
			}

			//write this down 
			else
			{
				if (HttpContext.Session.GetInt32("appointmentId") != null)
				{
					id = HttpContext.Session.GetInt32("appointmentId");
				}

				else
				{
					TempData["Message"] = "Please select an appointment";
					return RedirectToAction("Index", "MHAppointments");
				}
			}

			//write this down for exam
            var hairContext = _context.AppointmentDetail
				.Include(a => a.Appointment)
				.Include(a => a.Product)
				.Where(a=> a.AppointmentId == id);

			//write all this shit down mofo!!!!
			ViewData["id"] = id;
			ViewBag.id = id;
			ViewBag.Total = hairContext.Sum(t=> t.Total);
			ViewBag.Procedure = hairContext.Sum(t => t.ProcedureMinutes);



            return View(await hairContext.ToListAsync());
        }

        // GET: MHAppointmentDetails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointmentDetail = await _context.AppointmentDetail
                .Include(a => a.Appointment)
                .Include(a => a.Product)
                .SingleOrDefaultAsync(m => m.AppointmentDetailId == id);
            if (appointmentDetail == null)
            {
                return NotFound();
            }

            return View(appointmentDetail);
        }

        // GET: MHAppointmentDetails/Create
        public IActionResult Create(int? appointmentId)
        {
			TempData["id"] = appointmentId;
            //ViewData["AppointmentId"] = new SelectList(_context.Appointment, "AppointmentId", "AppointmentId");
            //ViewData["ProductId"] = new SelectList(_context.Product, "ProductId", "Name");
            return View();
        }

        // POST: MHAppointmentDetails/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int? appointmentId,[Bind("AppointmentDetailId,AppointmentId,ProductId,ProcedureMinutes,RetailPrice,Quantity,Discount,Total,Comments")] AppointmentDetail appointmentDetail)
        {
			TempData["id"] = ViewBag.id;
			if (TempData["id"] != null)
			{
				appointmentDetail.AppointmentId = Convert.ToInt32(TempData["id"]);
			}
			try
			{
				if (ModelState.IsValid)
				{
					_context.Add(appointmentDetail);
					await _context.SaveChangesAsync();
					return RedirectToAction(nameof(Index));
				}
				ViewData["AppointmentId"] = new SelectList(_context.Appointment, "AppointmentId", "AppointmentId", appointmentDetail.AppointmentId);
				ViewData["ProductId"] = new SelectList(_context.Product, "ProductId", "Name", appointmentDetail.ProductId);
				return View(appointmentDetail);
			}
			catch (Exception ex)
			{
				//write this down
				TempData["Message"]= ex.InnerException.Message;
				//write this down
				return View(appointmentDetail);
			}
        }

        // GET: MHAppointmentDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointmentDetail = await _context.AppointmentDetail.SingleOrDefaultAsync(m => m.AppointmentDetailId == id);
            if (appointmentDetail == null)
            {
                return NotFound();
            }
            ViewData["AppointmentId"] = new SelectList(_context.Appointment, "AppointmentId", "AppointmentId", appointmentDetail.AppointmentId);
            ViewData["ProductId"] = new SelectList(_context.Product, "ProductId", "Name", appointmentDetail.ProductId);
            return View(appointmentDetail);
        }

        // POST: MHAppointmentDetails/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AppointmentDetailId,AppointmentId,ProductId,ProcedureMinutes,RetailPrice,Quantity,Discount,Total,Comments")] AppointmentDetail appointmentDetail)
        {
            if (id != appointmentDetail.AppointmentDetailId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(appointmentDetail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppointmentDetailExists(appointmentDetail.AppointmentDetailId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AppointmentId"] = new SelectList(_context.Appointment, "AppointmentId", "AppointmentId", appointmentDetail.AppointmentId);
            ViewData["ProductId"] = new SelectList(_context.Product, "ProductId", "Name", appointmentDetail.ProductId);
            return View(appointmentDetail);
        }

        // GET: MHAppointmentDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointmentDetail = await _context.AppointmentDetail
                .Include(a => a.Appointment)
                .Include(a => a.Product)
                .SingleOrDefaultAsync(m => m.AppointmentDetailId == id);
            if (appointmentDetail == null)
            {
                return NotFound();
            }

            return View(appointmentDetail);
        }

        // POST: MHAppointmentDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appointmentDetail = await _context.AppointmentDetail.SingleOrDefaultAsync(m => m.AppointmentDetailId == id);
            _context.AppointmentDetail.Remove(appointmentDetail);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AppointmentDetailExists(int id)
        {
            return _context.AppointmentDetail.Any(e => e.AppointmentDetailId == id);
        }
    }
}

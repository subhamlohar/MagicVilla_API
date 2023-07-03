using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Logging;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_VillaAPI.Controllers
{
	//[Route("api/[Controller]")]
	[Route("api/VillaAPI")]
	[ApiController]
	public class VillaAPIController : ControllerBase
	{
		private readonly ILogging _logger;

		public VillaAPIController(ILogging logger)
		{
			_logger = logger;
			
		}

		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public ActionResult<IEnumerable<VillaDTO>> GetVillas()
		{
			_logger.Log("Getting all villas","");
			return Ok(VillaStore.villaList);
		}

		[HttpGet("{id:int}",Name ="GetVilla")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		//[ProducesResponseType(200,Type=typeof(VillaDTO))]
		public ActionResult<VillaDTO> GetVilla(int id)
		{
			var villa= VillaStore.villaList.FirstOrDefault(u => u.Id == id);

			if(id== 0)
			{
				_logger.Log("Get Villa Error with Id " + id, "error");
				return BadRequest();
			}
			else if(villa == null)
			{
				return NotFound();
			}

			return Ok(villa);
		}

		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public ActionResult<VillaDTO> CreateVilla([FromBody]VillaDTO villaDTO)
		{
			if (VillaStore.villaList.FirstOrDefault(u => u.Name.ToLower() == villaDTO.Name.ToLower()) != null)
			{
				ModelState.AddModelError("CustomError", "Villa Already Exists!");
				return BadRequest(ModelState);
			}
			if(villaDTO== null)
			{
				return BadRequest(villaDTO);
			}
			if(villaDTO.Id>0)
			{
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
			villaDTO.Id=VillaStore.villaList.OrderByDescending(u => u.Id).FirstOrDefault().Id+1;
			VillaStore.villaList.Add(villaDTO);

			return CreatedAtRoute("GetVilla", new {id=villaDTO.Id},villaDTO);
		}

		[HttpDelete("{id:int}", Name = "DeleteVilla")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public IActionResult DeleteVilla(int id)
		{
			if(id==0)
			{
				return BadRequest();
			}
			if (id == null)
			{
				return NotFound();
			}
			var villa=VillaStore.villaList.FirstOrDefault(u=>u.Id==id);
			VillaStore.villaList.Remove(villa);
			return NoContent();
		}

		
		[HttpPut("{id:int}", Name = "UpdateVilla")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public IActionResult UpdateVilla(int id, [FromBody]VillaDTO villaDTO)
		{
			if(villaDTO==null || id!=villaDTO.Id)
			{
				return BadRequest();
			}
			var villa=VillaStore.villaList.FirstOrDefault(u=>u.Id== id);
			villa.Name=villaDTO.Name;
			villa.sqft=villaDTO.sqft;
			villa.Occupancy=villaDTO.Occupancy;

			return NoContent();
		}

		[HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public IActionResult UpdatePartialVilla(int id,JsonPatchDocument<VillaDTO>patchDTO)
		{
			if(patchDTO==null ||id==0)
			{
				return BadRequest();
			}
			var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);
			if(villa==null)
			{
				return BadRequest();
			}
			patchDTO.ApplyTo(villa, ModelState);
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			return NoContent();
		}


	}
}

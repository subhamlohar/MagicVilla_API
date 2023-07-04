using System.Linq.Expressions;
using AutoMapper;
using MagicVilla_Web.Data;
using MagicVilla_Web.Models;
using MagicVilla_Web.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_Web.Repository
{
	public class VillaRepository :Repository<Villa>, IVillaRepository
	{
		private readonly ApplicationDbContext _db;	
		public VillaRepository(ApplicationDbContext db):base(db)
		{
			_db = db;
			
		}
		public async Task<Villa> UpdateAsync(Villa entity)
		{
			entity.UpdateDate = DateTime.Now;
			_db.Villas.Update(entity);
			await _db.SaveChangesAsync();
			return entity;
		}
	}
}

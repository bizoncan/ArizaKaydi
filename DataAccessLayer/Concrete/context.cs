using EntityLayer.Concrete;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Concrete
{
	public class context : DbContext
	{
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlServer("server=DESKTOP-8OU0GKQ\\SQLEXPRESS;database=ArizaKayit;integrated security=true;TrustServerCertificate=True ;", options =>
			{
				options.CommandTimeout(3000);
			});
		}
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<imageCollection>()
				.HasOne(ic => ic.workName)
				.WithMany()
				.HasForeignKey(ic => ic.workId)
				.OnDelete(DeleteBehavior.Cascade);
			modelBuilder.Entity<imageCollection>()
				.HasOne(ic => ic.errorName)
				.WithMany()
				.HasForeignKey(ic => ic.errorId)
				.OnDelete(DeleteBehavior.Cascade);// Önemli kısım burası
			modelBuilder.Entity<error>()
				.HasOne(ic => ic.userName)
				.WithMany()
				.HasForeignKey(ic => ic.userId)
				.OnDelete(DeleteBehavior.SetNull);
			modelBuilder.Entity<workOrder>()
				.HasOne(ic => ic.userI)
				.WithMany()
				.HasForeignKey(ic => ic.userId)
				.OnDelete(DeleteBehavior.SetNull);
			modelBuilder.Entity<work>()
				.HasOne(ic => ic.userI)
				.WithMany()
				.HasForeignKey(ic => ic.userId)
				.OnDelete(DeleteBehavior.SetNull);
			modelBuilder.Entity<work>()
			.HasOne(ic => ic.machine)
			.WithMany()
			.HasForeignKey(ic => ic.machineId)
			.OnDelete(DeleteBehavior.Cascade);
			modelBuilder.Entity<workOrder>()
			.HasOne(ic => ic.machine)
			.WithMany()
			.HasForeignKey(ic => ic.machineId)
			.OnDelete(DeleteBehavior.Cascade);
			modelBuilder.Entity<error>()
			.HasOne(ic => ic.machines)
			.WithMany()
			.HasForeignKey(ic => ic.machineId)
			.OnDelete(DeleteBehavior.Cascade);
			modelBuilder.Entity<machineNotifications>()
			.HasOne(ic => ic.machineName)
			.WithMany()
			.HasForeignKey(ic => ic.machineId)
			.OnDelete(DeleteBehavior.Cascade);
			modelBuilder.Entity<machinePart>()
			.HasOne(ic => ic.machineName)
			.WithMany()
			.HasForeignKey(ic => ic.machineId)
			.OnDelete(DeleteBehavior.Cascade);
			modelBuilder.Entity<workOrder>()
			.HasOne(ic => ic.machinePart)
			.WithMany()
			.HasForeignKey(ic => ic.machinePartId)
			.OnDelete(DeleteBehavior.Cascade);
			modelBuilder.Entity<error>()
			.HasOne(ic => ic.machinePartName)
			.WithMany()
			.HasForeignKey(ic => ic.machinePartId)
			.OnDelete(DeleteBehavior.Cascade);
			modelBuilder.Entity<machineNotifications>()
			.HasOne(ic => ic.machinePartName)
			.WithMany()
			.HasForeignKey(ic => ic.machinePartId)
			.OnDelete(DeleteBehavior.Cascade);
			modelBuilder.Entity<work>()
			.HasOne(ic => ic.machinePart)
			.WithMany()
			.HasForeignKey(ic => ic.machinePartId)
			.OnDelete(DeleteBehavior.Cascade);
			modelBuilder.Entity<work>()
			.HasOne(ic => ic.workOrder)
			.WithMany()
			.HasForeignKey(ic => ic.workOrderId)
			.OnDelete(DeleteBehavior.Cascade);
		}
		public context(DbContextOptions<context> options) : base(options)
		{
		}
		public DbSet<error> Errors { get; set; }
		public DbSet<machine> Machines { get; set; }
		public DbSet<machineNotifications> MachineNotifications { get; set; }
		public DbSet<machinePart> MachineParts { get; set; }
		public DbSet<machinePartError> MachinePartsError { get; set; }
		public DbSet<user> Users { get; set; }
		public DbSet<imageCollection> ImageCollection { get; set; }
		public DbSet<workOrder> WorkOrders { get; set; }
		public DbSet<work> Works { get; set; }
		public DbSet<mtQueries> MTQueries { get; set; }
	}
}

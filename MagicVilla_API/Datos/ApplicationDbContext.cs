using MagicVilla_API.Modelos;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_API.Datos
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :base(options)
        {
                
        }
        public DbSet<Villa> Villas {get; set;}//

        //sobreescritura del metodo para inggresar registros a la base de datos
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Villa>().HasData
                (
                    new Villa()
                        {
                            Id = 1,
                            Nombre = "Villa Real",
                            Detalle = "cabañas, naturaleza",
                            ImagenUrl = "",
                            Area = 50,
                            Tarifa = 150,
                            Amenidad = "Zoo",
                            FechaCreacion = DateTime.Now,
                            FechaActualizacion = DateTime.Now,

                        },
                    new Villa()
                    {
                        Id = 2,
                        Nombre = "Villa Nueva",
                        Detalle = "cabañas, mar",
                        ImagenUrl = "",
                        Area = 70,
                        Tarifa = 180,
                        Amenidad = "Mar",
                        FechaCreacion = DateTime.Now,
                        FechaActualizacion = DateTime.Now,

                    }
                );
                
        }
    }
}



/* 








 */

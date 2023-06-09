using MagicVilla_API.Datos;
using MagicVilla_API.Modelos;
using MagicVilla_API.Modelos.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_API.Controllers //la ruta [Route("api/[controller]")] utiliza [controller] como marcador de posición para el nombre del controlador en la ruta
{                                   //Cuando se utiliza [controller] en una ruta, ASP.NET Core reemplazará automáticamente ese marcador de posición con el nombre del 
    [Route("api/[controller]")]     // controlador correspondiente en tiempo de ejecución.
    [ApiController]                 //. Por ejemplo, si tienes un controlador llamado VillaController, la ruta se resolverá como api/Villa durante la ejecución.



    public class VillaController : ControllerBase         
    {
        private readonly ILogger<VillaController> _logger;
        private readonly ApplicationDbContext _db;  //MOD - agregagamos el DbContext (los datos de la base de datos) -inyecion de dependencia
        public VillaController(ILogger<VillaController> logger, ApplicationDbContext db) //MOD - agregamos el DBContext "ApplicationDbContext _db" al controlador
        {
            _logger= logger;
            _db = db;//MOD - 

        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<VillaDto>> GetVillas()
        {
            //return new List<Villa>
            //{
            //    new Villa {id=1, Nombre="Vista a la picina"},
            //    new Villa {id=2, Nombre="Vista a la playa"}
            //};
            _logger.LogInformation("obtener las villas");
            //return Ok(VillaStore.villaList)
            return Ok(_db.Villas.ToList());//MOD - 

        }

        [HttpGet("id:int", Name = "GetVilla")]//se le asigna un nombre para poderlo llamar y evitar ambiguedad 
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<VillaDto> GetVilla(int id)
        {
            if (id == 0)
            {
                _logger.LogError("Error al traer villa con Id " + id);
                return BadRequest(); //consulta erronea
            }

            //var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);
            var villa = _db.Villas.FirstOrDefault(v => v.id == id);//MOD - 

            if (villa == null)
                return NotFound();//consulta nula

            return Ok(villa);//consulta correcta
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<VillaDto> CrearVilla([FromBody] VillaDto villaDto) 
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //if (VillaStore.villaList.FirstOrDefault(v => v.Nombre.ToLower() == villaDto.Nombre.ToLower()) != null)
            if (_db.Villas.FirstOrDefault(v => v.Nombre.ToLower() == villaDto.Nombre.ToLower()) != null)//MOD - 
            {
                ModelState.AddModelError("NombreExiste", "La villa con ese nombre ya existe");
                return BadRequest(ModelState);
            }

            if (villaDto == null)
                return BadRequest(villaDto);

            if (villaDto.Id > 0)
                return StatusCode(StatusCodes.Status500InternalServerError);

            //villaDto.Id = VillaStore.villaList.OrderByDescending(v => v.Id).FirstOrDefault().Id+1;
            //VillaStore.villaList.Add(villaDto);

            Villa modelo = new() //MOD - 
            {
                Nombre = villaDto.Nombre,
                Detalle = villaDto.Detalle,
                ImagenUrl = villaDto.ImagenUrl,
                Area = villaDto.Area,
                Tarifa = villaDto.Tarifa,
                Amenidad = villaDto.Amenidad
            };
            _db.Villas.Add(modelo);//MOD - 
            _db.SaveChanges();//MOD - 


            return CreatedAtRoute("GetVilla", new { id = villaDto.Id }, villaDto); //llamamos a la ruta "GetVilla", se le pasa el parametro y el modelo completo
                                                                                   //para que esta nos retorne el id que le estamos pasando

        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteVilla(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }

            //var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);
            var villa= _db.Villas.FirstOrDefault(v=> v.id == id);//MOD - 
            if (villa == null)
            {
                return BadRequest();
            }

            //VillaStore.villaList.Remove(villa);
            _db.Remove(villa);//MOD - 
            _db.SaveChanges();//MOD - 
            return NoContent();
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UpdateVilla(int id, [FromBody] VillaDto villaDto) 
        {
            if(villaDto == null || id!=villaDto.Id) 
            {
                return BadRequest();
            }

            //var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);
            //villa.Nombre= villaDto.Nombre;
            //villa.Ocupantes= villaDto.Ocupantes;
            //villa.Area= villaDto.Area;

            Villa modelo = new()//MOD - 
            {
                id = villaDto.Id,
                Nombre = villaDto.Nombre,
                Detalle = villaDto.Detalle,
                ImagenUrl = villaDto.ImagenUrl,
                Area = villaDto.Area,
                Tarifa = villaDto.Tarifa,
                Amenidad = villaDto.Amenidad
            };
            _db.Villas.Update(modelo);//MOD - 
            _db.SaveChanges();//MOD - 

            return NoContent();

        }

        //metodo para cambiar una sola propiedad
        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UpdatePartialVilla(int id, JsonPatchDocument<VillaDto> patchDto)
        {
            if (patchDto == null || id ==0)
            {
                return BadRequest();
            }

            //var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);
            var villa = _db.Villas.AsNoTracking().FirstOrDefault(v => v.id == id);//MOD -control de Tracking
            VillaDto villaDto = new()//MOD - 
            {
                Id = villa.id,
                Nombre = villa.Nombre,
                Detalle = villa.Detalle,
                ImagenUrl = villa.ImagenUrl,
                Area = villa.Area,
                Tarifa = villa.Tarifa,
                Amenidad = villa.Amenidad
            };

            if(villa==null) return BadRequest();

            //patchDto.ApplyTo(villa, ModelState);
            patchDto.ApplyTo(villaDto, ModelState);//MOD -
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Villa modelo = new()//MOD - 
            {
                id = villaDto.Id,
                Nombre = villaDto.Nombre,
                Detalle = villaDto.Detalle,
                ImagenUrl = villaDto.ImagenUrl,
                Area = villaDto.Area,
                Tarifa = villaDto.Tarifa,
                Amenidad = villaDto.Amenidad
            };
            _db.Villas.Update(modelo);
            _db.SaveChanges();

            return NoContent();

        }

    }
}

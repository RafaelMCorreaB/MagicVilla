using AutoMapper;
using MagicVilla_API.Datos;
using MagicVilla_API.Modelos;
using MagicVilla_API.Modelos.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System.Collections;
using System.Collections.Generic;

namespace MagicVilla_API.Controllers //la ruta [Route("api/[controller]")] utiliza [controller] como marcador de posición para el nombre del controlador en la ruta
{                                   //Cuando se utiliza [controller] en una ruta, ASP.NET Core reemplazará automáticamente ese marcador de posición con el nombre del 
    [Route("api/[controller]")]     // controlador correspondiente en tiempo de ejecución.
    [ApiController]                 //. Por ejemplo, si tienes un controlador llamado VillaController, la ruta se resolverá como api/Villa durante la ejecución.



    public class VillaController : ControllerBase         
    {
        private readonly ILogger<VillaController> _logger;
        private readonly ApplicationDbContext _db;  //MOD - agregagamos el DbContext (los datos de la base de datos) -inyecion de dependencia
        private readonly IMapper _mapper;

        public VillaController(ILogger<VillaController> logger, ApplicationDbContext db, IMapper mapper) //MOD - agregamos el DBContext "ApplicationDbContext _db" al controlador
        {
            _logger= logger;
            _db = db;//MOD - 
            _mapper = mapper;//mod mapeo

        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<VillaDto>>> GetVillas()
        {
            //return new List<Villa>
            //{
            //    new Villa {id=1, Nombre="Vista a la picina"},
            //    new Villa {id=2, Nombre="Vista a la playa"}
            //};
            _logger.LogInformation("obtener las villas");
            IEnumerable<Villa> villaList = await _db.Villas.ToListAsync();//lista IEnumerable de tipo villa llamada vllaList


            //return Ok(VillaStore.villaList)
            //return Ok(await _db.Villas.ToListAsync());//MOD - 
            return Ok(_mapper.Map<IEnumerable<VillaDto>>(villaList));//se retorna un villaDto y la fuente es villaList

        }

        [HttpGet("id:int", Name = "GetVilla")]//se le asigna un nombre para poderlo llamar y evitar ambiguedad 
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<VillaDto>> GetVilla(int id)
        {
            if (id == 0)
            {
                _logger.LogError("Error al traer villa con Id " + id);
                return BadRequest(); //consulta erronea
            }

            //var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);
            var villa = await _db.Villas.FirstOrDefaultAsync(v => v.Id == id);//MOD - 

            if (villa == null)
                return NotFound();//consulta nula

            //return Ok(villa);//consulta correcta
            return Ok(_mapper.Map<Villa>(villa));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<VillaDto>> CrearVilla([FromBody] VillaCreateDto createDto) 
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //if (VillaStore.villaList.FirstOrDefault(v => v.Nombre.ToLower() == villaDto.Nombre.ToLower()) != null)
            if (await _db.Villas.FirstOrDefaultAsync(v => v.Nombre.ToLower() == createDto.Nombre.ToLower()) != null)//MOD - 
            {
                ModelState.AddModelError("NombreExiste", "La villa con ese nombre ya existe");
                return BadRequest(ModelState);
            }

            if (createDto == null)
                return BadRequest(createDto);

            //if (villaDto.Id > 0)
            //    return StatusCode(StatusCodes.Status500InternalServerError);

            //villaDto.Id = VillaStore.villaList.OrderByDescending(v => v.Id).FirstOrDefault().Id+1;
            //VillaStore.villaList.Add(villaDto);
            Villa modelo =_mapper.Map<Villa>(createDto);

            //Villa modelo = new() //MOD - 
            //{
            //    Nombre = villaDto.Nombre,
            //    Detalle = villaDto.Detalle,
            //    ImagenUrl = villaDto.ImagenUrl,
            //    Area = villaDto.Area,
            //    Tarifa = villaDto.Tarifa,
            //    Amenidad = villaDto.Amenidad
            //};
            await _db.Villas.AddAsync(modelo);//MOD - 
            await _db.SaveChangesAsync();//MOD - 


            return CreatedAtRoute("GetVilla", new { id = modelo.Id }, modelo); //llamamos a la ruta "GetVilla", se le pasa el parametro y el modelo completo
                                                                                   //para que esta nos retorne el id que le estamos pasando

        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteVilla(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }

            //var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);
            var villa= await _db.Villas.FirstOrDefaultAsync(v=> v.Id == id);//MOD - 
            if (villa == null)
            {
                return BadRequest();
            }

            //VillaStore.villaList.Remove(villa);
            _db.Remove(villa);//MOD - 
            await _db.SaveChangesAsync();//MOD - 
            return NoContent();
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateVilla(int id, [FromBody] VillaUpdateDto updateDto) 
        {
            if(updateDto == null || id!= updateDto.Id) 
            {
                return BadRequest();
            }

            //var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);
            //villa.Nombre= villaDto.Nombre;
            //villa.Ocupantes= villaDto.Ocupantes;
            //villa.Area= villaDto.Area;

            Villa modelo = _mapper.Map<Villa>(updateDto);

            //Villa modelo = new()//MOD - 
            //{
            //    Id = villaDto.Id,
            //    Nombre = villaDto.Nombre,
            //    Detalle = villaDto.Detalle,
            //    ImagenUrl = villaDto.ImagenUrl,
            //    Area = villaDto.Area,
            //    Tarifa = villaDto.Tarifa,
            //    Amenidad = villaDto.Amenidad
            //};
            _db.Villas.Update(modelo);//MOD - 
            await _db.SaveChangesAsync();//MOD -

            return NoContent();

        }

        //metodo para cambiar una sola propiedad
        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDto> patchDto)
        {
            if (patchDto == null || id ==0)
            {
                return BadRequest();
            }

            //var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);
            var villa = await _db.Villas.AsNoTracking().FirstOrDefaultAsync(v => v.Id == id);//MOD -control de Tracking

            VillaUpdateDto villaDto = _mapper.Map<VillaUpdateDto>(villa);


            //VillaUpdateDto villaDto = new()//MOD - 
            //{
            //    Id = villa.Id,
            //    Nombre = villa.Nombre,
            //    Detalle = villa.Detalle,
            //    ImagenUrl = villa.ImagenUrl,
            //    Area = villa.Area,
            //    Tarifa = villa.Tarifa,
            //    Amenidad = villa.Amenidad
            //};

            if(villa==null) return BadRequest();

            //patchDto.ApplyTo(villa, ModelState);
            patchDto.ApplyTo(villaDto, ModelState);//MOD -
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Villa modelo=_mapper.Map<Villa>(villaDto); 

            //Villa modelo = new()//MOD - 
            //{
            //    Id = villaDto.Id,
            //    Nombre = villaDto.Nombre,
            //    Detalle = villaDto.Detalle,
            //    ImagenUrl = villaDto.ImagenUrl,
            //    Area = villaDto.Area,
            //    Tarifa = villaDto.Tarifa,
            //    Amenidad = villaDto.Amenidad
            //};
            _db.Villas.Update(modelo);
            await _db.SaveChangesAsync();//MOD -

            return NoContent();

        }

    }
}

/*  
A. Configurar git
    1. git config --global user.email "tu@email.com" //Configurar tu email
    2. git --global user.name "Tu Nombre" //Configura tu nombre de usuario

B. Iniciar el repositorio
    1. crear el repositorio en GitHub
    2. git init                                         //iniciar git
    3. git add .                                        // Sube todo los archivos de la carpeta actual
    4. git commit -m "Nombre descriptivo primer commit"  //se crea foto del estado actual del proyecto y asigna nombre
    5. git branch -m main                               //escogemos la rama o linea de trabajo
    6. ejecutamos el ling del repositorio en GitHub     //asociar el repositorio local con el repositorio en la nube
    7. git push origin main                            // subir los cambios a la rama de trabajo 

B. Actualizar, subir los cambios a git
    1. git add .                                        //subir todo
    2. git commit -m "Nombre descriptivo primer commit" //foto
    3. git push -u origin main                          //subir a rama de trabajo seleccionada

 */
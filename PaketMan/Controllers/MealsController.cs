using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PaketMan.Contracts;
using PaketMan.Dtos.Meal;
using PaketMan.Models;
using PaketMan.Models.Api;
using PaketMan.Models.Api.Meal;
using PaketMan.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace PaketMan.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MealsController : ControllerBase
    {
        readonly private IMealRepository _mealRepository;
        readonly private IRestaurantRepository _restaurantRepository;
        public MealsController(IMealRepository mealRepository, IRestaurantRepository restaurantRepository)
        {
            _mealRepository = mealRepository;
            _restaurantRepository = restaurantRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetMeals([FromQuery] MealRequestParams filter)
        {
            try
            {
                var pagedData = filter.RestaurantId.HasValue? await _mealRepository.GetByRestaurantId(filter.RestaurantId.Value) : await _mealRepository.GetAll();

                if (!string.IsNullOrWhiteSpace(filter.SearchText))
                    pagedData = pagedData.Where(x => x.Name.Contains(filter.SearchText));


                pagedData = pagedData.OrderBy(filter.Sort);



                pagedData = pagedData.Skip((filter.PageNumber - 1) * filter.PageSize)
                    .Take(filter.PageSize);

                var totalRecords = (await _mealRepository.GetAll()).Count();
                var totalPagees = Math.Ceiling((decimal)totalRecords / filter.PageSize);

                return Ok(new PagedResponse<List<Meal>>(pagedData.ToList(), filter.PageNumber, filter.PageSize) { TotalRecords = totalRecords, TotalPages = (int)totalPagees });

            }
            catch (Exception ex)
            {
                //log error
                return BadRequest(new FailedResponse
                {
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpGet("{id}", Name = "MealById")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetMeal(int id)
        {
            try
            {
                var Meal = await _mealRepository.GetById(id);
                if (Meal == null)
                    return NotFound();
                return Ok(Meal);
            }
            catch (Exception ex)
            {
                //log error
                return BadRequest(new FailedResponse
                {
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateMeal(MealCreateDto obDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new FailedResponse
                    {
                        Errors = ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage))
                    });
                
                if (obDto.Price <= 0)
                    return BadRequest(new FailedResponse
                    {
                        Errors = new List<string> { "Price must be more than ZERO!!" }
                    });


                var restaurant = await _restaurantRepository.GetByIdAsync(obDto.RestaurantId);
                if (restaurant == null)
                    return BadRequest(new FailedResponse
                    {
                        Errors = new List<string> { "Restaurant not valid!!" }
                    }) ;


                var createdMeal = await _mealRepository.Create(obDto);
                return CreatedAtRoute("MealById", new { id = createdMeal.Id }, createdMeal);
            }
            catch (Exception ex)
            {
                //log error
                return BadRequest(new FailedResponse
                {
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMeal(int id, MealUpdateDto obDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new FailedResponse
                    {
                        Errors = ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage))
                    });

                if (obDto.Price <= 0)
                    return BadRequest(new FailedResponse
                    {
                        Errors = new List<string> { "Price must be more than ZERO!!" }
                    });


                var restaurant = await _restaurantRepository.GetByIdAsync(obDto.RestaurantId);
                if (restaurant == null)
                    return BadRequest(new FailedResponse
                    {
                        Errors = new List<string> { "Restaurant not valid!!" }
                    });

                var mealInDb = await _mealRepository.GetById(id);
                if (mealInDb == null)
                    return NotFound();
                await _mealRepository.Update(id, obDto);
                return NoContent();
            }
            catch (Exception ex)
            {
                //log error
                return BadRequest(new FailedResponse
                {
                    Errors = new List<string> { ex.Message }
                });
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMeal(int id)
        {
            try
            {
                var MealInDb = await _mealRepository.GetById(id);
                if (MealInDb == null)
                    return NotFound();
                await _mealRepository.Delete(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                //log error
                return BadRequest(new FailedResponse
                {
                    Errors = new List<string> { ex.Message }
                });
            }
        }
    }
}

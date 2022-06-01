using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PaketMan.Contracts;
using PaketMan.Dtos.Meal;
using PaketMan.Models;
using PaketMan.Models.Api;
using PaketMan.Models.Api.Meal;
using PaketMan.Extensions;


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
                var validFilter = new MealRequestParams(filter.PageNumber, filter.PageSize);
                var pagedData = await _mealRepository.GetAll();

                if (!string.IsNullOrWhiteSpace(validFilter.SearchText))
                    pagedData = pagedData.Where(x => x.Name.Contains(validFilter.SearchText));


                pagedData = pagedData.OrderBy(filter.Sort);



                pagedData = pagedData.Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                    .Take(validFilter.PageSize);

                var totalRecords = (await _mealRepository.GetAll()).Count();
                var totalPagees = Math.Ceiling((decimal)totalRecords / validFilter.PageSize);

                return Ok(new PagedResponse<List<Meal>>(pagedData.ToList(), validFilter.PageNumber, validFilter.PageSize) { TotalRecords = totalRecords, TotalPages = (int)totalPagees });

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

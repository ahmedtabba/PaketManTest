using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PaketMan.Contracts;
using PaketMan.Dtos.Restaurant;
using PaketMan.Models;
using PaketMan.Models.Api;
using PaketMan.Models.Api.Restaurant;
using PaketMan.Extensions;

namespace PaketMan.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RestaurantsController : ControllerBase
    {
        readonly private IRestaurantRepository _restaurantRepository;
        public RestaurantsController(IRestaurantRepository restaurantRepository)
        {
            _restaurantRepository = restaurantRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetRestaurants([FromQuery] RestaurantRequestParams filter)
        {
            try
            {
                var validFilter = new RestaurantRequestParams(filter.PageNumber, filter.PageSize);
                var pagedData = await _restaurantRepository.GetAll();

                if (!string.IsNullOrWhiteSpace(validFilter.SearchText))
                    pagedData = pagedData.Where(x => x.Description.Contains(validFilter.SearchText) || x.Name.Contains(validFilter.SearchText));


                pagedData = pagedData.OrderBy(filter.Sort);



                pagedData = pagedData.Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                    .Take(validFilter.PageSize);

                var totalRecords = (await _restaurantRepository.GetAll()).Count();
                var totalPagees = Math.Ceiling((decimal)totalRecords / validFilter.PageSize);

                return Ok(new PagedResponse<List<Restaurant>>(pagedData.ToList(), validFilter.PageNumber, validFilter.PageSize) { TotalRecords = totalRecords, TotalPages = (int)totalPagees });


               
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

        [HttpGet]
        [Route("getAllRestaurantWithDetails")]
        public async Task<IActionResult> GetRestaurantsWithDetails([FromQuery] RestaurantRequestParams filter)
        {
            try
            {
                var validFilter = new RestaurantRequestParams(filter.PageNumber, filter.PageSize);
                var pagedData = await _restaurantRepository.GetRestaurantsWithDetails();

                if (!string.IsNullOrWhiteSpace(validFilter.SearchText))
                    pagedData = pagedData.Where(x => x.Description.Contains(validFilter.SearchText) || x.Name.Contains(validFilter.SearchText));


                pagedData = pagedData.OrderBy(filter.Sort);



                pagedData = pagedData.Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                    .Take(validFilter.PageSize);

                var totalRecords = (await _restaurantRepository.GetRestaurantsWithDetails()).Count();
                var totalPagees = Math.Ceiling((decimal)totalRecords / validFilter.PageSize);

                return Ok(new PagedResponse<List<Restaurant>>(pagedData.ToList(), validFilter.PageNumber, validFilter.PageSize) { TotalRecords = totalRecords, TotalPages = (int)totalPagees });


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

        [HttpGet]
        [Route("getByMealsPrices")]
        public async Task<IActionResult> GetRestaurantsWithMealsPricesSumOverThousand([FromQuery] RestaurantRequestParams filter)
        {
            try
            {
                var validFilter = new RestaurantRequestParams(filter.PageNumber, filter.PageSize);
                var pagedData = await _restaurantRepository.GetAllWithMealsPricesSumOverThousand();

                if (!string.IsNullOrWhiteSpace(validFilter.SearchText))
                    pagedData = pagedData.Where(x => x.Description.Contains(validFilter.SearchText) || x.Name.Contains(validFilter.SearchText));


                pagedData = pagedData.OrderBy(filter.Sort);



                pagedData = pagedData.Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                    .Take(validFilter.PageSize);

                var totalRecords = (await _restaurantRepository.GetAllWithMealsPricesSumOverThousand()).Count();
                var totalPagees = Math.Ceiling((decimal)totalRecords / validFilter.PageSize);

                return Ok(new PagedResponse<List<Restaurant>>(pagedData.ToList(), validFilter.PageNumber, validFilter.PageSize) { TotalRecords = totalRecords, TotalPages = (int)totalPagees });

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

        [HttpGet("{id}", Name = "GetById")]
        public async Task<IActionResult> GetRestaurant(int id)
        {
            try
            {
                var restaurant = await _restaurantRepository.GetByIdAsync(id);
                if (restaurant == null)
                    return NotFound();
                return Ok(restaurant);
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
        public async Task<IActionResult> CreateRestaurant(RestaurantCreateDto restaurant)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new FailedResponse
                    {
                        Errors = ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage))
                    });



                var createdRestaurant = await _restaurantRepository.Create(restaurant);
                return CreatedAtRoute("GetById", new { id = createdRestaurant.Id }, createdRestaurant);
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
        public async Task<IActionResult> UpdateRestaurant(int id, RestaurantUpdateDto restaurant)
        {
            try
            {

                if (!ModelState.IsValid)
                    return BadRequest(new FailedResponse
                    {
                        Errors = ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage))
                    });


                var dbCompany = await _restaurantRepository.GetByIdAsync(id);
                if (dbCompany == null)
                    return NotFound();
                await _restaurantRepository.Update(id, restaurant);
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
        public async Task<IActionResult> DeleteRestaurant(int id)
        {
            try
            {
                var restaurantInDb = await _restaurantRepository.GetByIdAsync(id);
                if (restaurantInDb == null)
                    return NotFound();
                await _restaurantRepository.Delete(id);
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

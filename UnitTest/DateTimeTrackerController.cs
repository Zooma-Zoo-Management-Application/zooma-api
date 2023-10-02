using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using zooma_api.DTO;

namespace zooma_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DateTimeTrackerController : ControllerBase
    {
        [HttpPost]
        [Route("date-time-tracker")]
        public IActionResult CheckDate(DateTimeBody dateTime)
        {
            try
            {
                if (isValidDate(dateTime.year, dateTime.month, dateTime.day)){
                    return Ok($"{dateTime.year}/{dateTime.month}/{dateTime.day} is a valid date!");
                }
                else
                {
                    return BadRequest($"{dateTime.year}/{dateTime.month}/{dateTime.day} is a invalid date!");
                }

            }catch (Exception ex)
            {
                return BadRequest("Error: " + ex.Message);
            }
        }

        public int DaysInMonth(int year, int month)
        {
            int[] monthHas31 = { 1, 3, 5, 7, 8, 10, 12 };
            int[] monthHas30 = { 4, 6, 9, 11 };

            if (monthHas31.Contains(month))
            {
                return 31;
            }
            else if (monthHas30.Contains(month))
            {
                return 30;
            }
            else if (month == 2)
            {
                if (year % 400 == 0) return 29;
                else if (year % 100 == 0) return 28;
                else if (year % 4 == 0) return 29;
                else return 28;
            }
            else
            {
                return 0;
            }
        }
        private Boolean isValidDate(int year, int month, int day)
        {
            if(month >= 0 && month < 12)
            {
                if(day >= 1)
                {
                    if (day <= DaysInMonth(year, month))
                    {
                        return true;
                    }
                    else throw new Exception("Day exceeds max days in month!");
                }
                else throw new Exception("Date cannot below 1!");
            }
            else
            {
                throw new Exception("Invalid month!");
            }
        }
    }
}

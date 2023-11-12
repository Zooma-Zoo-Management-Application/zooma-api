using Microsoft.EntityFrameworkCore;
using zooma_api.DTO;
using zooma_api.Interfaces;
using zooma_api.Models;

namespace zooma_api.Repositories
{
    public class DietRepository : IDietRepository
    {
        public double CountEnergyOfDiet(int dietId)
        {
            using(var _context = new zoomadbContext())
            {
                try
                {
                    //lấy ra list diet details của diet 
                    var dietDetailsId = _context.DietDetails.Where(e => e.DietId == dietId).Include(e => e.Food).ToList();

                    double totalEnergy = 0;

                    // vòng lặp xử lý từng phần tử diet details trong list 
                    foreach (var item in dietDetailsId)
                    {
                        // kiểm tra xem diet details đó có thức ăn được gán vào nó hay không
                        if (item != null && item.Food != null)
                        {
                            double energy = item.Food.EnergyValue * (double)item.Quantity;

                            DateTime startDay = (DateTime) item.ScheduleAt;
                            DateTime endDay = (DateTime) item.EndAt;

                            double countDays = 0;

                            int[] dayInWeek = null;

                            // Kiểm tra xem có lịch ăn của diet details đó không
                            if (!string.IsNullOrEmpty(item.FeedingDate))
                            {
                                // hàm phân tách chuỗi lịch ăn thành 1 mảng dựa trên dấu ,
                                string[] days = item.FeedingDate.Split(',');

                                // tạo mảng số nguyên có độ dài bằng mảng ở trên
                                dayInWeek = new int[days.Length];

                                // gán giá trị vào mảng số nguyên
                                for (int i = 0; i < dayInWeek.Length; i++)
                                {
                                    if (int.TryParse(days[i], out int day))
                                    {
                                        dayInWeek[i] = day;
                                    }
                                }
                            }

                            // hàm tính tổng số ngày từ lúc schedule cho tới lúc kết thúc
                            for (DateTime date = startDay; date <= endDay; date = date.AddDays(1))
                            {
                                int dayOfWeekAsInt = (int)date.DayOfWeek;

                                if (dayOfWeekAsInt == 0) 
                                {
                                    dayOfWeekAsInt = 7; 
                                }

                                if (dayInWeek.Contains(dayOfWeekAsInt))
                                {
                                    countDays++;
                                }
                            }

                            // tổng năng lượng dựa trên 1 cái dietDetails
                            totalEnergy += energy * countDays;
                        }
                    }
                    return totalEnergy;
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

        public List<Diet> GetAllDiets()
        {
            using(var _context = new zoomadbContext())
            {
                try
                {
                    var diet = _context.Diets.Include(a => a.Animals).ToList();

                    return diet;
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

        public Diet GetDietById(int id)
        {
            using (var _context = new zoomadbContext())
            {
                try
                {
                    var diet = _context.Diets.Include(a => a.Animals).FirstOrDefault(e => e.Id == id);

                    return diet;
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

        public Diet GetDietByName(string name)
        {
            using (var _context = new zoomadbContext())
            {
                try
                {
                    var diet = _context.Diets.SingleOrDefault(d => d.Name == name);

                    return diet;
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }
    }
}

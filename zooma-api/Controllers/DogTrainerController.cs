﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using zooma_api.Models;
using static System.Net.WebRequestMethods;

namespace zooma_api.Controllers
{
    //Ten Api them s (so nhieu) dog-trainer -> dog-trainers

    // EXAMPLE: --
    // HTTP GET http://api.example.com/user-management/users -> get tất cả user
    // HTTP POST http://api.example.com/user-management/users -> tạo user mới
    // HTTP GET http://api.example.com/user-management/users/{id} -> get 1 user cụ thể
    // HTTP PUT http://api.example.com/user-management/users/{id} -> sửa 1 user cụ thể
    // HTTP DELETE http://api.example.com/user-management/users/{id} -> xoá 1 user cụ thể

    // 200 OK – Trả về thành công cho tất cả phương thức
    // 201 Created – Trả về khi một Resource được tạo thành công.
    // 204 No Content – Trả về khi Resource xoá thành công.
    // 304 Not Modified – Client có thể sử dụng dữ liệu cache.
    // 400 Bad Request – Request không hợp lệ
    // 401 Unauthorized – Request cần có auth.
    // 403 Forbidden – bị từ chối không cho phép.
    // 404 Not Found – Không tìm thấy resource từ URI.
    // 405 Method Not Allowed – Phương thức không cho phép với user hiện tại.
    // 410 Gone – Resource không còn tồn tại, Version cũ đã không còn hỗ trợ.
    // 415 Unsupported Media Type – Không hỗ trợ kiểu Resource này.
    // 422 Unprocessable Entity – Dữ liệu không được xác thực.
    // 429 Too Many Requests – Request bị từ chối do bị giới hạn.

    [Route("api/[controller]")]
    [ApiController]
    public class DogTrainerController : ControllerBase
    {
        [HttpGet]
        [Route("v1/dog-trainers")]
        public IActionResult GetAllUsers()
        {
            try
            {
                var data = new DogTrainer{
                    Id = 11723,
                    Username = $"user Uy",
                    Password = $"password Uy",
                    Fullname = $"fullname Uy Le",
                    IsActive = true
                };

                return Ok(data);
            } catch (Exception ex)
            {
                return BadRequest("GetAllUsers Failed: " + ex.Message);
            }
        }

        // POST api/Users/CreateNew
        [HttpPost]
        [Route("v1/dog-trainers")]
        public IActionResult CreateNew(string Username, string Password, string Fullname, Boolean IsActive)
        {
            try
            {


                return Ok("Success");
            }
            catch (Exception ex)
            {
                return BadRequest("Create User Failed: " + ex.Message);
            }
        }

        [HttpPut]
        [Route("v1/dog-trainers")]
        public IActionResult PutExample(int id)
        {
            try
            {
                

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        private List<DogTrainer> AllUser()
        {
            List<DogTrainer> list = new List<DogTrainer>();
            for (int i = 1; i < 10; i++)   // Tạo ra 6 User
            {
                DogTrainer u = new DogTrainer()   // Tạo ra user mới
                {
                    Id = i,
                    Username = $"user {i}",
                    Password = $"password {i}",
                    Fullname = $"fullname {i}",
                    IsActive = true
                };
                list.Add(u);   // Thêm vào danh sách User
            }
            return list;
        }
    }
}

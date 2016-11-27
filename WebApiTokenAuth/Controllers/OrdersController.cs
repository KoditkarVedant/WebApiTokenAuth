using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApiTokenAuth.Controllers
{
    [RoutePrefix("api/orders")]
    public class OrdersController : ApiController
    {
        [Authorize]
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetAll()
        {
            return Ok(Order.CreateOrders());
        }
    }

    public class Order
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public int Quntity { get; set; }

        public static List<Order> CreateOrders()
        {
            return new List<Order>()
            {
                new Order()
                {
                    Id = 1,
                    ProductName = "something",
                    Quntity = 100
                },new Order()
                {
                    Id = 2,
                    ProductName = "nothing",
                    Quntity = 10
                }
            };
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Project_HMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly DataAccess _dataAccess;

        public RoomController(DataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Room>> GetRooms()
        {
            var sql = "select * from Room;";
            var ds = ExecuteQuery(sql);
            var rooms = ds.Tables[0].AsEnumerable().Select(row => new Room
            {
                RId = row.Field<int>("RId"),
                Category = row.Field<string>("Category"),
                IsBooked = row.Field<string>("IsBooked"),
                RoomCost = row.Field<double>("RoomCost")
            }).ToList();
            return Ok(rooms);
        }

        [HttpGet("{id}")]
        public ActionResult<Room> GetRoom(int id)
        {
            var sql = $"select * from Room where RId = {id};";
            var ds = ExecuteQuery(sql);
            if (ds.Tables[0].Rows.Count == 0)
            {
                return NotFound();
            }
            var row = ds.Tables[0].Rows[0];
            var room = new Room
            {
                RId = row.Field<int>("RId"),
                Category = row.Field<string>("Category"),
                IsBooked = row.Field<string>("IsBooked"),
                RoomCost = row.Field<double>("RoomCost")
            };
            return Ok(room);
        }

        [HttpPost]
        public ActionResult AddRoom(Room room)
        {
            var sql = $"insert into Room values ({room.RId}, '{room.Category}', '{room.IsBooked}', {room.RoomCost}, NULL);";
            var count = ExecuteDML(sql);
            if (count == 1)
            {
                return CreatedAtAction(nameof(GetRoom), new { id = room.RId }, room);
            }
            return BadRequest();
        }

        [HttpPut("{id}")]
        public ActionResult UpdateRoom(int id, Room room)
        {
            var sql = $"select * from Room where RId = {id};";
            var ds = ExecuteQuery(sql);
            if (ds.Tables[0].Rows.Count == 0)
            {
                return NotFound();
            }
            var updateSql = $"update Room set Category = '{room.Category}', IsBooked = '{room.IsBooked}', RoomCost = {room.RoomCost} where RId = {id};";
            var count = ExecuteDML(updateSql);
            if (count == 1)
            {
                return NoContent();
            }
            return BadRequest();
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteRoom(int id)
        {
            var sql = $"delete from Room where RId = {id};";
            var count = ExecuteDML(sql);
            if (count == 1)
            {
                return NoContent();
            }
            return BadRequest();
        }

        private DataSet ExecuteQuery(string sql)
        {
            return _dataAccess.ExecuteQuery(sql);
        }

        private int ExecuteDML(string sql)
        {
            return _dataAccess.ExecuteDML(sql);
        }
    }

    public class Room
    {
        public int RId { get; set; }
        public string Category { get; set; }
        public string IsBooked { get; set; }
        public double RoomCost { get; set; }
    }
}
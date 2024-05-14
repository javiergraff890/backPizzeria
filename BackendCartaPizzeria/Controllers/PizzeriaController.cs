using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;



namespace BackendCartaPizzeria.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PizzeriaController : ControllerBase
    {
        private IConfiguration _configuration ;

        public PizzeriaController(IConfiguration config) {
            this._configuration = config;
        }


        [HttpGet]
        [Route("GetCarta")]
        public JsonResult GetCarta()
        {
            string query = "select * from dbo.Pagina";
            DataTable table = new DataTable();
            table.Clear();
            table.Columns.Add("id", typeof(int));
            table.Columns.Add("nombre", typeof(string));
            table.Columns.Add("productos", typeof(object));
            string SqlDataSource = _configuration.GetConnectionString("localDbConnection");
            SqlDataReader myReader;
            using (SqlConnection mycon = new SqlConnection(SqlDataSource))
            {
                mycon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, mycon))
                {
                    myReader = myCommand.ExecuteReader();
                    //table.Load(myReader);
                    while (myReader.Read()) { 
                        int id = myReader.GetInt32("id");
                        string nombre = myReader.GetString("nombre");
                        DataTable productos = GetProds(id);
                        DataRow _dr = table.NewRow();
                        _dr["id"] = id;
                        _dr["nombre"] = nombre;
                        _dr["productos"] = productos;
                        table.Rows.Add(_dr);
                    }

                    mycon.Close();
                }
            }


            return new JsonResult(table);

        }

        [HttpGet]
        [Route("GetPaginas")]
        public JsonResult GetNotes()
        {
            string query = "select * from dbo.Pagina";
            DataTable table = new DataTable();
            string SqlDataSource = _configuration.GetConnectionString("localDbConnection");
            SqlDataReader myReader;
            using (SqlConnection mycon = new SqlConnection(SqlDataSource))
            {
                mycon.Open();
                using( SqlCommand myCommand = new SqlCommand(query, mycon))
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    mycon.Close();
                }
            }

            return new JsonResult(table);
        }

        private DataTable GetProds(int id)
        {
            string query = "SELECT * FROM dbo.Producto WHERE paginaid = @id";
            DataTable table = new DataTable();
            string SqlDataSource = _configuration.GetConnectionString("localDbConnection");
            SqlDataReader myReader;
            using (SqlConnection mycon = new SqlConnection(SqlDataSource))
            {
                mycon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, mycon))
                {
                    myCommand.Parameters.AddWithValue("@id", id);
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    mycon.Close();

                }

            }
            return table;
        }

        [HttpGet]
        [Route("GetProductos/{id}")]
        public JsonResult GetProductos([FromRoute] int id)
        {
            string query = "SELECT * FROM dbo.Producto WHERE paginaid = @id";
            DataTable table = new DataTable();
            string SqlDataSource = _configuration.GetConnectionString("localDbConnection");
            SqlDataReader myReader;
            using (SqlConnection mycon = new SqlConnection(SqlDataSource))
            {
                mycon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, mycon))
                {
                    myCommand.Parameters.AddWithValue("@id", id);
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    mycon.Close();

                }

            }
            return new JsonResult(table);
        }


        [HttpPost]
        [Route("NuevaPagina")]
        public ActionResult NuevaPagina([FromForm] string nombre)
        {
            Thread.Sleep(4000);
            Debug.WriteLine("nombre = " + nombre);
            string query = "INSERT INTO dbo.Pagina (nombre) values (@nombre)";
            string SqlDataSource = _configuration.GetConnectionString("localDbConnection");
            //SqlDataReader myReader;
            using (SqlConnection mycon = new SqlConnection(SqlDataSource))
            {
                mycon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, mycon))
                {
                    myCommand.Parameters.AddWithValue("@nombre", nombre);
                    //myReader = myCommand.ExecuteReader();
                    int rowsAffected = myCommand.ExecuteNonQuery();
                    Debug.WriteLine("Rows affected: " + rowsAffected);
                    mycon.Close();
                    return Ok();

                }
            }
                
        }

        [HttpDelete]
        [Route("EliminarPagina")]
        public ActionResult EliminarPagina([FromForm] int id)
        {
            Thread.Sleep(4000);
            string queryPrevio = "DELETE FROM dbo.Producto where paginaid = @id";
            string query = "DELETE FROM dbo.Pagina where id = @id";


            string SqlDataSource = _configuration.GetConnectionString("localDbConnection");
            //SqlDataReader myReader;
            using (SqlConnection mycon = new SqlConnection(SqlDataSource))
            {
                mycon.Open();

                using (SqlCommand myCommand1 = new SqlCommand(queryPrevio, mycon))
                {
                    myCommand1.Parameters.AddWithValue("@id", id);
                    int rowsAffected = myCommand1.ExecuteNonQuery();
                    Debug.WriteLine("Rows affected: " + rowsAffected);
                    //myReader = myCommand1.ExecuteReader();
                }

                using (SqlCommand myCommand = new SqlCommand(query, mycon))
                {
                    myCommand.Parameters.AddWithValue("@id", id);
                    int rowsAffected = myCommand.ExecuteNonQuery();
                    Debug.WriteLine("Rows affected: " + rowsAffected);
                    //myReader = myCommand.ExecuteReader();
                    mycon.Close();
                    return Ok();
                }
            }
        }

        [HttpPost]
        [Route("NuevoProducto")]
        public ActionResult NuevoProducto([FromForm] int idPagina, [FromForm] string nombre, [FromForm] string precioString, [FromForm] string ingredientes)
        {
            //Thread.Sleep(5000);
            string precioStringFormat = precioString.Replace('.', ',');
            Debug.WriteLine(precioString);
            Decimal precio = Decimal.Parse(precioStringFormat);
            Debug.WriteLine("precio =" + precio);
            string query = "INSERT INTO dbo.Producto (nombre, precio, ingredientes, paginaid) VALUES (@nombre, @precio, @ingredientes, @idPagina)";
            string SqlDataSource = _configuration.GetConnectionString("localDbConnection");
            //SqlDataReader myReader;
            using (SqlConnection mycon = new SqlConnection(SqlDataSource))
            {
                mycon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, mycon))
                {
                    myCommand.Parameters.AddWithValue("@nombre", nombre);
                    myCommand.Parameters.AddWithValue("@precio", precio);
                    myCommand.Parameters.AddWithValue("@ingredientes", ingredientes);
                    myCommand.Parameters.AddWithValue("@idPagina", idPagina);
                    //myReader = myCommand.ExecuteReader();
                    int rowsAffected = myCommand.ExecuteNonQuery();
                    Debug.WriteLine("Rows affected: " + rowsAffected);
                    mycon.Close();
                    return Ok();
                }
            }
        }

        [HttpDelete]
        [Route("EliminarProducto")]
        public ActionResult EliminarProducto([FromForm] int idProducto)
        {
            Thread.Sleep(5000);
            string query = "DELETE FROM dbo.Producto where id = @idProducto";
            string SqlDataSource = _configuration.GetConnectionString("localDbConnection");
            //SqlDataReader myReader;
            using (SqlConnection mycon = new SqlConnection(SqlDataSource))
            {
                mycon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, mycon))
                {
                    myCommand.Parameters.AddWithValue("@idProducto", idProducto);
                    //myReader = myCommand.ExecuteReader();
                    int rowsAffected = myCommand.ExecuteNonQuery();
                    Debug.WriteLine("Rows affected: " + rowsAffected);
                    mycon.Close();
                    return Ok();
                }
            }
        }

        [HttpPut]
        [Route("EditarProducto")]
        public ActionResult EditarProducto([FromForm] int id, [FromForm] string nombre, [FromForm] string precioString, [FromForm] string ingredientes)
        {
            Thread.Sleep(5000);
            string precioStringFormat = precioString.Replace('.', ',');
            Decimal precio = Decimal.Parse(precioStringFormat);

            string query = "UPDATE dbo.Producto SET nombre = @nombre, precio = @precio, ingredientes = @ingredientes WHERE id = @id";
            string SqlDataSource = _configuration.GetConnectionString("localDbConnection");
            //SqlDataReader myReader;
            using (SqlConnection mycon = new SqlConnection(SqlDataSource))
            {
                mycon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, mycon))
                {
                    myCommand.Parameters.AddWithValue("@nombre", nombre);
                    myCommand.Parameters.AddWithValue("@precio", precio);
                    myCommand.Parameters.AddWithValue("@ingredientes", ingredientes);
                    myCommand.Parameters.AddWithValue("@id", id);
                    //myReader = myCommand.ExecuteReader();
                    int rowsAffected = myCommand.ExecuteNonQuery();
                    Debug.WriteLine("Rows affected: " + rowsAffected);
                    mycon.Close();
                    return Ok();
                }
            }
        }

        [HttpPut]
        [Route("EditarPagina")]
        public ActionResult EditarPagina([FromForm] int id, [FromForm] string nombre)
        {
            Thread.Sleep(5000);
            string query = "UPDATE dbo.Pagina SET nombre = @nombre WHERE id=@id";
            string SqlDataSource = _configuration.GetConnectionString("localDbConnection");

            using (SqlConnection mycon = new SqlConnection(SqlDataSource))
            {
                mycon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, mycon))
                {
                    myCommand.Parameters.AddWithValue("@nombre", nombre);
                    myCommand.Parameters.AddWithValue("@id", id);
                    int rowsAffected = myCommand.ExecuteNonQuery();
                    Debug.WriteLine("Rows affected: " + rowsAffected);
                    return Ok();

                }
            }
        }

    }
}
